module JimBroBot.Data

open System

type BotUser = { Id: uint32; DiscordId: string }

type Exercise =
    { Id: uint32
      Name: string
      Type: string
      BotUserId: uint32 }

type ExerciseLog =
    { Id: uint32
      ExerciseId: uint32
      LogDate: DateOnly }

type AllSetDetail = { Id: uint32 }

type SetExerciseDetail =
    { Id: uint32
      Reps: uint32
      Weight: double
      IsWarmup: bool
      SequenceOrder: uint32 }

type SpeedExerciseDetail =
    { Id: uint32
      DurationSec: uint32
      DistanceMiles: double }

type TimeExerciseDetail = { Id: uint32; DurationSec: uint32 }
