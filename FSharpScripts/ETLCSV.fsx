#r "System.Data.dll"
#r "System.Data.Linq.dll"
#r "FSharp.Data.TypeProviders.dll"
#r @"..\packages\FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"

open FSharp.Data
open Microsoft.FSharp.Data.TypeProviders

type SourceCsv = CsvProvider<"../data/customers.csv">
type DestSql = SqlDataConnection<"Data Source=localhost; Initial Catalog=NWIND; Integrated Security=True;">

let importCustomers() =
    let getDestination (source : SourceCsv.Row) =
        let dest = new DestSql.ServiceTypes.Customers()
        dest.CustomerID <- source.CustomerID
        dest.CompanyName <- source.CompanyName
        dest.ContactName <- source.ContactName
        dest.ContactTitle <- source.ContactTitle
        dest.Address <- source.Address
        dest.City <- source.City
        dest.Region <- source.Region
        dest.PostalCode <- source.PostalCode
        dest.Country <- source.Country
        dest.Phone <- source.Phone
        dest.Fax <- source.Fax
        dest

    use sourceDB = SourceCsv.Load("../data/customers.csv")
    use destDB = DestSql.GetDataContext()

    let copyToDestination customer =
        destDB.Customers.InsertOnSubmit customer
        printfn "Importing customer %s" customer.CustomerID

    sourceDB.Rows
    |> Seq.map getDestination
    |> Seq.iter copyToDestination

    destDB.DataContext.SubmitChanges()

printfn "Starting import"
importCustomers()
printfn "Done"