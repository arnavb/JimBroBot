module JimBroBot.Data

open System

open Npgsql.FSharp

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

// TODO: Replace temporary connection string with proper environment setup
let connection =
    "Host=localhost;Port=5432;Username=jimbrobotdb;\
                        Password=jimbrobotdbpassword;Database=jimbrobotdb"

let readBotUser (connectionString: string) (discordId: string) =
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM bot_user WHERE discord_id = @discord_id"
    |> Sql.parameters [ "discord_id", Sql.string discordId ]
    |> Sql.executeRowAsync (fun read ->
        { Id = read.int "id"
          DiscordId = read.string "discord_id" })

let readExercisesForBotUser (connectionString: string) (botUserId: int) =
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM exercise WHERE bot_user_id = @bot_user_id"
    |> Sql.parameters [ "bot_user_id", Sql.int botUserId ]
    |> Sql.executeAsync (fun read ->
        { Id = read.int "id"
          Name = read.string "name"
          Type = read.string "type"
          BotUserId = read.int "bot_user_id" })

let readExerciseLogEntriesForBotUser (connectionString: string) (botUserId: int) =
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM exercise_log WHERE bot_user_id = @bot_user_id"
    |> Sql.parameters [ "bot_user_id", Sql.int botUserId ]
    |> Sql.executeAsync (fun read ->
        { Id = read.int "id"
          ExerciseId = read.int "exercise_id"
          LogDate = read.dateOnly "log_date" })
