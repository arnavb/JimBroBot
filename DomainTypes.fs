module JimBroBot.DomainTypes

type BotConfig =
    { BotToken: string
      TestGuildId: uint64 option }

type BotError =
    | NoEnvBotToken
    | BotStartError of string
