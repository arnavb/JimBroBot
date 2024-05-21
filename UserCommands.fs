module JimBroBot.UserCommands

open Discord

let addExerciseBuilder name =
    new SlashCommandBuilder()
    |> _.WithName(name)
    |> _.WithDescription("Add a new exercise for tracking")
    |> _.AddOption(
        new SlashCommandOptionBuilder()
        |> _.WithName("exercisetype")
        |> _.WithDescription("Type of exercise (set (reps + weight), time (duration),  speed (miles and duration))")
        |> _.WithRequired(true)
        |> _.AddChoice("Set", "set")
        |> _.AddChoice("Time", "time")
        |> _.AddChoice("Speed", "speed")
        |> _.WithType(ApplicationCommandOptionType.String)
    )

let logExerciseBuilder name =
    new SlashCommandBuilder()
    |> _.WithName(name)
    |> _.WithDescription("Log an exercise")
    |> _.AddOption(
        new SlashCommandOptionBuilder()
        |> _.WithName("set")
        |> _.WithDescription("Log a set (reps + weight)")
        |> _.WithType(ApplicationCommandOptionType.SubCommand)
    )
