module JimBroBot.UserCommands

open Discord
open Discord.WebSocket

open JimBroBot.DomainTypes

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
                .WithDescription("Log a set (reps + weight)")
                .WithType(ApplicationCommandOptionType.SubCommand)
        )
