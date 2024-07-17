module JimBroBot.Converters

open JimBroBot.DomainTypes
open JimBroBot.Data

let stringToExerciseType str =
    match str with
    | "set" -> SetBased
    | "speed" -> SpeedBased
    | "time" -> TimeBased
    | _ -> failwith "Unexpected exercise type"

let databaseExerciseToDomainExerciseInfo exercise =
    let exerciseType = stringToExerciseType exercise.Type

    { Name = exercise.Name
      Type = exerciseType }

let loadDataForUser (connectionString: string, discordId: string) =
    task {
        let! botUser = readBotUser connectionString discordId
        let! botUserExercises = readExercisesForBotUser connectionString botUser.Id

        let exerciseInfos = botUserExercises |> Seq.map databaseExerciseToDomainExerciseInfo

        // TODO: Figure out way to load and combine exercise log
        return
            { Id = discordId
              Exercises = exerciseInfos
              ExerciseLog = Seq.empty }
    }
