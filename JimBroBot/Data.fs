module JimBroBot.Data

open System

type BotUser = { Id: int; DiscordId: string }

type Exercise =
    { Id: int
      Name: string
      Type: string
      BotUserId: int }

type ExerciseLog =
    { Id: int
      ExerciseId: int
      LogDate: DateOnly }

type AllSetDetail = { Id: int }

type SetExerciseDetail =
    { Id: int
      Reps: int
      Weight: double
      IsWarmup: bool
      SequenceOrder: int }

type SpeedExerciseDetail =
    { Id: int
      DurationSec: int
      DistanceMiles: double }

type TimeExerciseDetail = { Id: uint32; DurationSec: uint32 }
