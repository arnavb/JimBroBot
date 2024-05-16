module JimBroBot.Program

open dotenv.net
open dotenv.net.Utilities
open Discord
open Discord.WebSocket
open System.Threading.Tasks

open JimBroBot.DomainTypes

let loadBotToken () =
    DotEnv.Load(new DotEnvOptions(probeForEnv = true, ignoreExceptions = false))

    let success, botToken = EnvReader.TryGetStringValue("JIM_BRO_BOT_TOKEN")

    match success with
    | true -> Ok botToken
    | false -> Error NoEnvBotToken


let log message =
    printfn $"{message.ToString()}"
    Task.CompletedTask


let createAndStartClient (botToken: string) =
    let client = new DiscordSocketClient()
    client.add_Log log

    async {
        try
            do!
                client.LoginAsync(tokenType = TokenType.Bot, token = botToken)
                |> Async.AwaitTask

            do! client.StartAsync() |> Async.AwaitTask

            do! Task.Delay(-1) |> Async.AwaitTask

            return Ok "Success"
        with error ->
            return Error(BotStartError(error.ToString()))
    }
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
    loadBotToken () |> Result.bind createAndStartClient |> handleExit
