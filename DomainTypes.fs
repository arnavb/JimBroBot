module DomainTypes

type BotError =
    | NoEnvBotToken
    | BotStartError of string
