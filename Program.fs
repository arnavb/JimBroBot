open dotenv.net
open dotenv.net.Utilities
open Discord
open Discord.WebSocket
open System.Threading.Tasks

type BotError =
    | NoEnvBotToken
    | BotStartError of string

let loadBotToken () =
    DotEnv.Load(new DotEnvOptions(probeForEnv = true, ignoreExceptions = false))

    let success, botToken = EnvReader.TryGetStringValue("JIM_BRO_BOT_TOKEN")

    match success with
    | true -> Ok botToken
    | false -> Error NoEnvBotToken


let log message =
    printfn "%s" (message.ToString())
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

            return Ok ""
        with error ->
            return Error(BotStartError(error.ToString()))
    }
    |> Async.RunSynchronously


let handleExit result =
    match result with
    | Ok _ -> 0
    | Error(BotStartError exc) ->
        printfn "Exception: %s" exc
        1
    | Error err ->
        printfn "Error: %s" (err.ToString())
        1

let tryParse (input: string) =
    match System.Int32.TryParse input with
    | true, v -> Ok v
    | false, _ -> Error "couldn't parse"


[<EntryPoint>]
let main args =
    loadBotToken () |> Result.bind createAndStartClient |> handleExit
