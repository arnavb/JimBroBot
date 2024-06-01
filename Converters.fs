module JimBroBot.Converters

open JimBroBot.DomainTypes

let stringToExerciseType str =
    match str with
    | "set" -> SetBased
    | "speed" -> SpeedBased
    | "time" -> TimeBased
    | _ -> failwith "Unexpected exercise type"
