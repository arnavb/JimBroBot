module JimBroBot.Program

open dotenv.net
open dotenv.net.Utilities
open Discord
open Discord.Interactions
open Discord.WebSocket
open System.Threading.Tasks
open System.Reflection
open System

open JimBroBot.DomainTypes

let loadBotConfig () =
    DotEnv.Load(new DotEnvOptions(probeForEnv = true, ignoreExceptions = false))

    let botTokenSuccess, botToken = EnvReader.TryGetStringValue("JIM_BRO_BOT_TOKEN")
    let testGuildIdSuccess, testGuildId = EnvReader.TryGetStringValue("TEST_GUILD_ID")

    match botTokenSuccess, testGuildIdSuccess with
    | true, true ->
        Ok
            { BotToken = botToken
              TestGuildId = Some(Convert.ToUInt64 testGuildId) }
    | true, false ->
        Ok
            { BotToken = botToken
              TestGuildId = None }
    | false, _ -> Error NoEnvBotToken


let log (message: LogMessage) =
    printfn $"{message.ToString()}"
    Task.CompletedTask

let ready (interactionService: InteractionService) testGuildId () =
    task {
        printfn "Bot is ready!"
        do! interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), null) :> Task

        interactionService.Modules
        |> Seq.iter (fun md -> printfn $"{md.Name} has {md.SlashCommands.Count} slash commands")

        match testGuildId with
        | Some testGuildId -> do! interactionService.RegisterCommandsToGuildAsync testGuildId :> Task
        | None -> do! interactionService.RegisterCommandsGloballyAsync() :> Task
    }
    :> Task

let interactionCreated client (interactionService: InteractionService) message =
    let ctx = new SocketInteractionContext(client, message)

    task { do! interactionService.ExecuteCommandAsync(ctx, null) :> Task } :> Task


let createAndStartClient botConfig =
    use client =
        new DiscordSocketClient(new DiscordSocketConfig(GatewayIntents = GatewayIntents.GuildMessageReactions))

    use interactionService = new InteractionService(client.Rest)

    // Register event handlers
    client.add_Log log
    client.add_Ready (ready interactionService botConfig.TestGuildId)
    client.add_InteractionCreated (interactionCreated client interactionService)

    task {
        try
            do! client.LoginAsync(tokenType = TokenType.Bot, token = botConfig.BotToken)

            do! client.StartAsync()


            do! Task.Delay(-1)

            return Ok "Success"
        with error ->
            return Error(BotStartError(error.ToString()))
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously


let handleExit result =
    match result with
    | Ok _ -> 0
    | Error(BotStartError exc) ->
        printfn $"Exception: {exc}"
        1
    | Error err ->
        printfn $"Error: {err.ToString()}"
        1

[<EntryPoint>]
let main args =
    loadBotConfig () |> Result.bind createAndStartClient |> handleExit
