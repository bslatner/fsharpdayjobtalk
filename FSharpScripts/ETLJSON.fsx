#r "System.Data.dll"
#r "System.Data.Linq.dll"
#r "FSharp.Data.TypeProviders.dll"
#r @"..\packages\scripts\fsharp.data\lib\net45\FSharp.Data.dll"
#r @"..\packages\scripts\SqlProvider\lib\net451\FSharp.Data.SqlProvider.dll"

open FSharp.Data
open FSharp.Data.Sql

type SourceJson = JsonProvider<"https://api.github.com/users/bslatner/repos">

[<Literal>]
let ConnectionString = "Data Source=localhost; Initial Catalog=Test; Integrated Security=True;"

type DestSql = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString, UseOptionTypes = true>

let importRepositories() =
    let destDB = DestSql.GetDataContext()

    let getDestination (source : SourceJson.Root) =
        let dest = destDB.Dbo.Repository.Create()
        dest.Name <- source.Name
        dest.FullName <- source.FullName
        dest.CloneUrl <- source.CloneUrl
        dest

    let sourceDB = SourceJson.Load("https://api.github.com/users/bslatner/repos")

    sourceDB
    |> Seq.map getDestination
    |> Seq.iter (fun r ->
        printfn "Importing repository %s" r.Name
    )

    destDB.SubmitUpdates()

printfn "Starting import"
importRepositories()
printfn "Done"