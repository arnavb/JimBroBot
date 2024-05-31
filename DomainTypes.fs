module JimBroBot.DomainTypes

type BotConfig =
    { BotToken: string
      TestGuildId: uint64 option }

type BotError =
    | NoEnvBotToken
    | BotStartError of string
    | MissingCommandHandler

type ExerciseType =
    | SetBased
    | SpeedBased
    | TimeBased

type ExerciseInfo = { Name: string; Type: ExerciseType }

type SetDetails =
    { Warmup: bool; Reps: int; Weight: int }

type SpeedDetails =
    { Duration: int; DistanceMiles: double }

type TimeDetails = { Duration: int }

type ExerciseLogDetails =
    | SpeedDetails of SpeedDetails
    | TimeDetails of TimeDetails
    | AllSetDetails of SetDetails seq

type ExerciseLogItem =
    { Info: ExerciseInfo
      Details: ExerciseLogDetails }

type User =
    { Id: uint64
      Exercises: ExerciseInfo seq
      ExerciseLog: ExerciseLogItem seq }

let stringToExerciseType str =
    match str with
    | "set" -> SetBased
    | "speed" -> SpeedBased
    | "time" -> TimeBased
    | _ -> failwith "Unexpected exercise type"
