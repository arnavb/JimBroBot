module JimBroBot.WorkoutModule

open System.Threading.Tasks

open Discord.Interactions
open Discord
open Discord.WebSocket

type public WorkoutModule() =
    inherit InteractionModuleBase<SocketInteractionContext>()

    member private this.respondAsync text = base.RespondAsync text

    [<SlashCommand("echo", "Add a new workout")>]
    member public this.Say(text: string) =
        task {
            let! _ = this.respondAsync "Hello, World!"
            ()
        }
        :> Task
