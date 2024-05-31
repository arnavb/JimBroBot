module JimBroBot.Program

open dotenv.net
open dotenv.net.Utilities
open Discord
open Discord.Interactions
open Discord.WebSocket
open System.Threading.Tasks
open System

open JimBroBot.DomainTypes
open JimBroBot.UserCommands

let commands =
    [ "add", (addExerciseBuilder, addExerciseBuilder)
      "log", (logExerciseBuilder, logExerciseBuilder) ]

let loadBotConfig () =
    DotEnv.Load(DotEnvOptions(probeForEnv = true, ignoreExceptions = false))

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

let buildCommands (client: DiscordSocketClient) =
    commands
    |> List.map (fun (name, (commandBuilder, _)) -> name |> commandBuilder)
    |> List.map _.Build()

let log (message: LogMessage) =
    printfn $"{message.ToString()}"
    Task.CompletedTask

let ready (client: DiscordSocketClient) testGuildId () =
    task {
        let! _ =
            buildCommands client
            |> List.map (fun builtCommand ->
                client.Rest.CreateGuildCommand(builtCommand, testGuildId) |> Async.AwaitTask)
            |> Async.Parallel

        printfn "Bot is ready!"
    }
    :> Task

let interactionCreated client (interactionService: InteractionService) message =
    let ctx = SocketInteractionContext(client, message)

    task { do! interactionService.ExecuteCommandAsync(ctx, null) :> Task } :> Task


let createAndStartClient botConfig =
    use client =
        new DiscordSocketClient(DiscordSocketConfig(GatewayIntents = GatewayIntents.GuildMessageReactions))

    use interactionService = new InteractionService(client.Rest)

    // Register event handlers
    client.add_Log log
    client.add_Ready (ready client (Option.defaultValue 3UL botConfig.TestGuildId))
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
