module JimBroBot.DomainTypes

type BotError =
    | NoEnvBotToken
    | BotStartError of string
