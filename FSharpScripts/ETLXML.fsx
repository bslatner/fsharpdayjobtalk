#r "System.Data.dll"
#r "System.Data.Linq.dll"
#r "System.Xml.Linq"
#r "FSharp.Data.TypeProviders.dll"
#r @"..\packages\FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"

open FSharp.Data
open Microsoft.FSharp.Data.TypeProviders

type SourceXml = XmlProvider<"http://w1.weather.gov/xml/current_obs/index.xml">
type DestSql = SqlDataConnection<"Data Source=localhost; Initial Catalog=Test; Integrated Security=True;">

let importStations() =
    let getDestination (source : SourceXml.Station) =
        let dest = new DestSql.ServiceTypes.WeatherStation()
        dest.StationID <- source.StationId
        dest.State <- source.State
        dest.Name <- source.StationName
        dest.Latitude <- float(source.Latitude)
        dest.Longitude <- float(source.Longitude)
        dest.XmlFeedUrl <- source.XmlUrl
        dest

    let sourceDB = SourceXml.Load("http://w1.weather.gov/xml/current_obs/index.xml")
    use destDB = DestSql.GetDataContext()

    let copyToDestination station =
        destDB.WeatherStation.InsertOnSubmit station
        printfn "Importing station %s" station.Name

    sourceDB.Stations
    |> Seq.map getDestination
    |> Seq.iter copyToDestination

    destDB.DataContext.SubmitChanges()

printfn "Starting import"
importStations()
printfn "Done"