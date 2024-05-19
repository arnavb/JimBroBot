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
