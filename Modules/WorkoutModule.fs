module JimBroBot.WorkoutModule

open System.Threading.Tasks

open Discord.Interactions

type public ExerciseModule() =
    inherit InteractionModuleBase<SocketInteractionContext>()

    member private this.respondAsync text = base.RespondAsync text

    [<SlashCommand("add", "Add a new type of exercise")>]
    member public this.addWorkout
        (
            name: string,
            [<Choice("Set (measured in reps and weight)", "set");
              Choice("Time (measured in time)", "time");
              Choice("Speed (measured in duration and distance)", "speed")>] exerciseType: string
        ) =
        task {
            let! _ = this.respondAsync $"Added exercise {name}"
            ()
        }
        :> Task
