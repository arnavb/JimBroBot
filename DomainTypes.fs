module JimBroBot.DomainTypes

type BotConfig =
    { BotToken: string
      TestGuildId: uint64 option }

type BotError =
    | NoEnvBotToken
    | BotStartError of string

type ExerciseInfo = { UserId: string; Name: string }

type SetBasedDetails =
    { Warmup: bool; Reps: int; Weight: int }

type SpeedBasedDetails = { Duration: int; Distance: int }

type TimeBasedDetails = { Duration: int }

type ExerciseLogDetails =
    | SpeedDetails of SpeedBasedDetails
    | TimeDetails of TimeBasedDetails
    | AllSetDetails of SetBasedDetails seq

type ExerciseLog =
    { Info: ExerciseInfo
      LogDetails: ExerciseLogDetails
      Notes: string }
