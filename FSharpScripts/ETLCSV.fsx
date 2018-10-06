#r "System.Data.dll"
#r "System.Data.Linq.dll"
#r "FSharp.Data.TypeProviders.dll"
#r @"..\packages\scripts\fsharp.data\lib\net45\FSharp.Data.dll"
#r @"..\packages\scripts\SqlProvider\lib\net451\FSharp.Data.SqlProvider.dll"

open FSharp.Data
open FSharp.Data.Sql

type SourceCsv = CsvProvider<"../data/customers.csv">

[<Literal>]
let ConnectionString = "Data Source=localhost; Initial Catalog=NWIND; Integrated Security=True;"

type DestSql = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString, UseOptionTypes = true>

let importCustomers() =
    let destDB = DestSql.GetDataContext()

    let getDestination (source : SourceCsv.Row) =
        let dest = destDB.Dbo.Customers.Create()
        dest.CustomerId <- source.CustomerID
        dest.CompanyName <- source.CompanyName
        dest.ContactName <- Some source.ContactName
        dest.ContactTitle <- Some source.ContactTitle
        dest.Address <- Some source.Address
        dest.City <- Some source.City
        dest.Region <- Some source.Region
        dest.PostalCode <- Some source.PostalCode
        dest.Country <- Some source.Country
        dest.Phone <- Some source.Phone
        dest.Fax <- Some source.Fax
        dest

    use sourceDB = SourceCsv.Load("../data/customers.csv")

    sourceDB.Rows
    |> Seq.map getDestination
    |> Seq.iter (fun c ->
        printfn "Importing customer %s" c.CustomerId
    )

    destDB.SubmitUpdates()

printfn "Starting import"
importCustomers()
printfn "Done"