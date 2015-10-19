#r "System.Data.dll"
#r "System.Data.Linq.dll"
#r "System.Xml.Linq"
#r "FSharp.Data.TypeProviders.dll"
#r @"..\packages\FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"

open FSharp.Data
open Microsoft.FSharp.Data.TypeProviders

type DestSql = SqlDataConnection<"Data Source=localhost; Initial Catalog=NWIND; Integrated Security=True;">

let notifyOperations msg =
    printfn "%s" msg
    printfn "E-mail operations with this error."
    printfn "Or maybe submit to an SNS topic."

try
    let customerValues = [
        "CompanyName", "Test Name"
    ]
    let result = Http.RequestString("http://www.slatner.com/api/AddCustomer", body = FormValues customerValues)

    use db = DestSql.GetDataContext()
    query {
        for row in db.Customers do
        where (row.CompanyName = "Test Name")
        select row
    } |> Seq.exactlyOne |> ignore

with
    | :? System.ArgumentException as ex ->
        notifyOperations "Didn't find new customer"
    | :? System.Net.WebException as ex -> 
        notifyOperations ex.Message