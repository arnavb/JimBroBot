module JimBroBot.Program

open dotenv.net
open dotenv.net.Utilities
open Discord
open Discord.WebSocket
open System.Threading.Tasks
open System

open JimBroBot.DomainTypes
open JimBroBot.UserCommands

let commands =
    [ "add", (addExerciseBuilder, addExerciseResponder)
      "log", (logExerciseBuilder, logExerciseResponder)
      "undo", (undoExerciseBuilder, undoExerciseResponder) ]

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

let buildCommands =
    commands
    |> List.map (fun (name, (commandBuilder, _)) -> name |> commandBuilder)
    |> List.map _.Build()

let findAndExecuteCommand (name: string, socketSlashCommand: SocketSlashCommand) =
    task {
        match commands |> List.tryFind (fun elem -> fst elem = name) with
        | Some(_, (_, commandResponder)) ->
            do! commandResponder socketSlashCommand
            return Ok()
        | None -> return Error MissingCommandHandler
    }

let log (message: LogMessage) =
    printfn $"{message.ToString()}"
    Task.CompletedTask

let ready (client: DiscordSocketClient) testGuildId () =
    task {
        let! _ =
            buildCommands
            |> List.map (fun builtCommand ->
                client.Rest.CreateGuildCommand(builtCommand, testGuildId) |> Async.AwaitTask)
            |> Async.Parallel

        printfn "Bot is ready!"
    }
    :> Task

let slashCommandExecuted (command: SocketSlashCommand) =
    task {
        let! commandExecution = findAndExecuteCommand (command.Data.Name, command)

        match commandExecution with
        | Ok _ -> ()
        | Error err -> printfn $"Got error {err}"
    }
    :> Task

let createAndStartClient botConfig =
    use client =
        new DiscordSocketClient(DiscordSocketConfig(GatewayIntents = GatewayIntents.GuildMessageReactions))

    // Register event handlers
    client.add_Log log
    client.add_Ready (ready client (Option.defaultValue 3UL botConfig.TestGuildId))
    client.add_SlashCommandExecuted slashCommandExecuted

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
