module JimBroBot.User

open JimBroBot.DomainTypes

let createUser id =
    { Id = id
      Exercises = Seq.empty
      ExerciseLog = Seq.empty }

let addExercise user exercise =
    { user with
        Exercises = Seq.append exercise user.Exercises }

let logExercise user exercise =
    { user with
        ExerciseLog = Seq.append exercise user.ExerciseLog }

let undoExercise user =
    if user |> _.ExerciseLog |> Seq.isEmpty then
        user
    else
        { user with
            ExerciseLog = Seq.skip 1 user.ExerciseLog }
