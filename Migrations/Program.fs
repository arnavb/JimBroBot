open System.Reflection

open DbUp

[<EntryPoint>]
let main args =
    // TODO: Replace temporary connection string with proper environment setup
    let connection =
        "Host=localhost;Port=5432;Username=jimbrobotdb;\
                            Password=jimbrobotdbpassword;Database=jimbrobotdb"

    EnsureDatabase.For.PostgresqlDatabase(connection)

    let upgrader =
        DeployChanges.To
            .PostgresqlDatabase(connection)
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
