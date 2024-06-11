open System.Reflection

open DbUp
open Microsoft.Data.Sqlite

[<EntryPoint>]
let main args =
    let connectionString = "Data Source=JimBroBot.db"

    use connection = new SqliteConnection(connectionString)
    use _ = new SQLite.Helpers.SharedConnection(connection)

    let upgrader =
        DeployChanges.To
            .SQLiteDatabase(connection.ConnectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build()

    let result = upgrader.PerformUpgrade()

    match result.Successful, result.Error with
    | (false, err) ->
        printfn $"ERROR: ${err}"
        1
    | (true, _) ->
        printfn "Migrations successful"
        0
