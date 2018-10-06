#r "System.Data.dll"
#r "System.Data.Linq.dll"
#r "System.Xml.Linq"
#r @"..\packages\scripts\fsharp.data\lib\net45\FSharp.Data.dll"
#r @"..\packages\scripts\SqlProvider\lib\net451\FSharp.Data.SqlProvider.dll"

open FSharp.Data
open FSharp.Data.Sql

[<Literal>]
let ConnectionString = "Data Source=localhost; Initial Catalog=NWIND; Integrated Security=True;"

type DestSql = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString>

let notifyOperations msg =
    printfn "%s" msg
    printfn "E-mail operations with this error."
    printfn "Or maybe submit to an SNS topic."

try
    let customerValues = [
        "CompanyName", "Test Name"
    ]
    let result = Http.RequestString("http://www.slatner.com/api/AddCustomer", body = FormValues customerValues)

    let db = DestSql.GetDataContext()
    query {
        for row in db.Dbo.Customers do
        where (row.CompanyName = "Test Name")
        select row
    } |> Seq.exactlyOne |> ignore

with
    | :? System.ArgumentException as ex ->
        notifyOperations "Didn't find new customer"
    | :? System.Net.WebException as ex -> 
        notifyOperations ex.Message