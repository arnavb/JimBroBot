module JimBroBot.Converters

open JimBroBot.DomainTypes
open JimBroBot.RawDataTypes

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
