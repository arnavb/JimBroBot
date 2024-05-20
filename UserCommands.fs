module JimBroBot.UserCommands

open Discord

let createAddExerciseCommand =
    let builder =
        new SlashCommandBuilder()
        |> _.WithName("add")
        |> _.WithDescription("Add a new exercise for tracking")
        |> _.AddOption(
            new SlashCommandOptionBuilder()
            |> _.WithName("exercise type")
            |> _.WithDescription(
                "Type of exercise (set based (reps + weight), time based (duration),  speed based (miles and duration))"
            )
            |> _.WithRequired(true)
            |> _.AddChoice("Set", "set")
            |> _.AddChoice("Time", "time")
            |> _.AddChoice("Speed", "speed")
            |> _.WithType(ApplicationCommandOptionType.String)
        )


    builder.Build
