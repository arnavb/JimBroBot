module JimBroBot.UserCommands

open Discord
open Discord.WebSocket
open TimeSpanParserUtil

open JimBroBot.DomainTypes
open JimBroBot.Converters

let addExerciseBuilder name =
    (new SlashCommandBuilder())
        .WithName(name)
        .WithDescription("Add a new exercise for tracking")
        .AddOption(
            (new SlashCommandOptionBuilder())
                .WithName("exercisetype")
                .WithDescription("Type of exercise (set (reps + weight), time (duration),  speed (miles and duration))")
                .WithRequired(true)
                .AddChoice("Set", "set")
                .AddChoice("Time", "time")
                .AddChoice("Speed", "speed")
                .WithType(ApplicationCommandOptionType.String)
        )
        .AddOption(
            "name",
            ApplicationCommandOptionType.String,
            "Name of exercise (must be unique)",
            isRequired = true,
            choices = Array.empty
        )

let addExerciseResponder (command: SocketSlashCommand) =
    let user = command.User
    let allOptions = command.Data.Options |> List.ofSeq |> List.map _.Value

    match allOptions with
    | rawType :: rawName :: _ ->
        task {
            let exerciseType = rawType :?> string |> stringToExerciseType
            let exerciseName = rawName :?> string

            do! command.RespondAsync $"{user.GlobalName} wants to add {exerciseName} of type {exerciseType}"
        }
    | _ -> failwith "Not possible"

let logExerciseBuilder name =
    (new SlashCommandBuilder())
        .WithName(name)
        .WithDescription("Log an exercise")
        .AddOption(
            (new SlashCommandOptionBuilder())
                .WithName("set")
                .WithDescription("Log a set")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(
                    "name",
                    ApplicationCommandOptionType.String,
                    "Name of exercise",
                    isRequired = true,
                    choices = Array.empty
                )
                .AddOption(
                    "count",
                    ApplicationCommandOptionType.Integer,
                    "Number of sets",
                    isRequired = true,
                    minValue = 0,
                    maxValue = 5,
                    choices = Array.empty
                )
                .AddOption(
                    "reps",
                    ApplicationCommandOptionType.Integer,
                    "Number of reps for each set",
                    minValue = 0.0,
                    isRequired = true,
                    choices = Array.empty
                )
                .AddOption(
                    "weight",
                    ApplicationCommandOptionType.Number,
                    "Weight for each rep",
                    isRequired = true,
                    minValue = 0.0,
                    choices = Array.empty
                )
                .AddOption(
                    "warmup",
                    ApplicationCommandOptionType.Boolean,
                    "Whether this is a warmup set or not",
                    isRequired = false,
                    choices = Array.empty
                )
        )
        .AddOption(
            (new SlashCommandOptionBuilder())
                .WithName("speed")
                .WithDescription("Log a speed based exercise")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(
                    "name",
                    ApplicationCommandOptionType.String,
                    "Name of exercise",
                    isRequired = true,
                    choices = Array.empty
                )
                .AddOption(
                    "duration",
                    ApplicationCommandOptionType.String,
                    "Duration for this exercise (e.g 1h 30, 1:30, 1h 30 mins 0 secs)",
                    isRequired = true,
                    choices = Array.empty
                )
                .AddOption(
                    "distance",
                    ApplicationCommandOptionType.Number,
                    "Distance travelled for this exercise (miles)",
                    isRequired = true,
                    minValue = 0.0,
                    choices = Array.empty
                )
        )
        .AddOption(
            (new SlashCommandOptionBuilder())
                .WithName("time")
                .WithDescription("Log a time based exercise")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(
                    "name",
                    ApplicationCommandOptionType.String,
                    "Name of exercise",
                    isRequired = true,
                    choices = Array.empty
                )
                .AddOption(
                    "duration",
                    ApplicationCommandOptionType.String,
                    "Duration for this exercise (e.g 1h 30, 1:30, 1h 30 mins 0 secs)",
                    isRequired = true,
                    choices = Array.empty
                )
        )

let logSetHelper (command: SocketSlashCommand) (options: obj list) =
    let (name, count, reps, weight, warmup) =
        match options with
        | rawName :: rawCount :: rawReps :: rawWeight :: rawWarmup :: _ ->
            let name = rawName :?> string
            let count = rawCount :?> int64
            let reps = rawReps :?> int64
            let weight = rawWeight :?> double
            let warmup = rawWarmup :?> bool

            name, count, reps, weight, warmup
        | rawName :: rawCount :: rawReps :: rawWeight :: _ ->
            let name = rawName :?> string
            let count = rawCount :?> int64
            let reps = rawReps :?> int64
            let weight = rawWeight :?> double

            name, count, reps, weight, false
        | _ -> failwith "Missing parameter (should be impossible)"

    task {
        do! command.RespondAsync $"Logging {count} set(s) of {name} with {reps} reps of {weight} lbs each ({warmup})"
    }

let logSpeedHelper (command: SocketSlashCommand) (options: obj list) =
    let (name, duration, distance) =
        match options with
        | rawName :: rawDuration :: rawDistance :: _ ->
            let name = rawName :?> string
            let duration = rawDuration :?> string
            let distance = rawDistance :?> double

            name, duration, distance
        | _ -> failwith "Missing parameter (should be impossible)"


    task {
        match TimeSpanParser.TryParse duration with
        | (true, parsedDuration) ->
            do! command.RespondAsync $"Logging {name} for {parsedDuration} and distance {distance}"
        | (false, _) ->
            do!
                command.RespondAsync(
                    $"{duration} is not a valid duration! Examples of valid durations are 1 hour 30 mins, 1:30, 20 mins, etc. \
                    Note that 1:30 will parse as 1 HOUR 30 mins, not 1 min 30 seconds.",
                    ephemeral = true
                )
    }

let logTimeHelper (command: SocketSlashCommand) (options: obj list) =
    let (name, duration) =
        match options with
        | rawName :: rawDuration :: _ ->
            let name = rawName :?> string
            let duration = rawDuration :?> string

            name, duration
        | _ -> failwith "Missing parameter (should be impossible)"


    task {
        match TimeSpanParser.TryParse duration with
        | (true, parsedDuration) -> do! command.RespondAsync $"Logging {name} for {parsedDuration}"
        | (false, _) ->
            do!
                command.RespondAsync(
                    $"{duration} is not a valid duration! Examples of valid durations are 1 hour 30 mins, 1:30, 20 mins, etc. \
                    Note that 1:30 will parse as 1 HOUR 30 mins, not 1 min 30 seconds.",
                    ephemeral = true
                )
    }


let logExerciseResponder (command: SocketSlashCommand) =
    let user = command.User

    let allOptions = command.Data.Options |> List.ofSeq

    let logExerciseType = allOptions |> List.head |> _.Name |> stringToExerciseType

    let parameters =
        allOptions |> List.head |> _.Options |> List.ofSeq |> List.map _.Value

    let helper =
        match logExerciseType with
        | SetBased -> logSetHelper
        | SpeedBased -> logSpeedHelper
        | TimeBased -> logTimeHelper

    task { do! helper command parameters }


let undoExerciseBuilder name =
    (new SlashCommandBuilder())
        .WithName(name)
        .WithDescription("Undo most recent exercise entry")

let undoExerciseResponder (command: SocketSlashCommand) =
    task { do! command.RespondAsync "Done!" }
