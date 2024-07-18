module JimBroBot.Data

open JimBroBot.Converters
open JimBroBot.DomainTypes
open JimBroBot.RawDataTypes

open Npgsql.FSharp


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


let loadDataForUser (connectionString: string) (discordId: string) =
    task {
        try
            let! botUser = readBotUser connectionString discordId
            let! botUserExercises = readExercisesForBotUser connectionString botUser.Id

            let exerciseInfos = botUserExercises |> Seq.map databaseExerciseToDomainExerciseInfo

            // TODO: Figure out way to load and combine exercise log
            return
                Ok
                    { Id = discordId
                      Exercises = exerciseInfos
                      ExerciseLog = Seq.empty }
        with NoResultsException(_) ->
            return Error NoUserFound
    }
