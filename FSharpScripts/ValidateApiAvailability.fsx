﻿#r @"..\packages\scripts\fsharp.data\lib\net45\FSharp.Data.dll"

open FSharp.Data

let notifyOperations msg =
    printfn "%s" msg
    printfn "E-mail operations with this error."
    printfn "Or maybe submit to an SNS topic."

try
    Http.RequestString "http://www.slatner.com/api/Ping"
with
    | :? System.Net.WebException as ex -> 
        notifyOperations ex.Message; ""