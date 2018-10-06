#r "System.Data.dll"
#r "System.Data.Linq.dll"
#r "System.Xml.Linq"
#r "FSharp.Data.TypeProviders.dll"
#r @"..\packages\scripts\fsharp.data\lib\net45\FSharp.Data.dll"
#r @"..\packages\scripts\SqlProvider\lib\net451\FSharp.Data.SqlProvider.dll"

open FSharp.Data
open FSharp.Data.Sql

type SourceXml = XmlProvider<"http://w1.weather.gov/xml/current_obs/index.xml">

[<Literal>]
let ConnectionString = "Data Source=localhost; Initial Catalog=Test; Integrated Security=True;"

type DestSql = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString, UseOptionTypes = true>

let importStations() =
    let destDB = DestSql.GetDataContext()

    let getDestination (source : SourceXml.Station) =
        let dest = destDB.Dbo.WeatherStation.Create()
        dest.StationId <- source.StationId
        dest.State <- source.State
        dest.Name <- source.StationName
        dest.Latitude <- float(source.Latitude)
        dest.Longitude <- float(source.Longitude)
        dest.XmlFeedUrl <- source.XmlUrl
        dest

    let sourceDB = SourceXml.Load("http://w1.weather.gov/xml/current_obs/index.xml")

    sourceDB.Stations
    |> Seq.map getDestination
    |> Seq.iter (fun s ->
        printfn "Importing station %s" s.Name
    )

    destDB.SubmitUpdates()

printfn "Starting import"
importStations()
printfn "Done"