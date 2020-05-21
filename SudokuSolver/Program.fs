open System
open Solver

let displayValue x =
    match x with
    | Some(v) -> sprintf "%i" v
    | _ -> " "

[<EntryPoint>]
let main argv = 
    let game = [|
        "     6   "
        "4 7 19   "
        "  62  7  "
        "9  8 26  "
        "7 5    9 "
        " 62 514  "
        " 48   237"
        "27 4    5"
        "5 1    6 "
    |]

    let parsed = ParseGame game
    SolveGameStandard parsed |> ignore

    for row in 0..8 do
        for col in 0..8 do
            let v = displayValue parsed.[row,col].Value
            Console.Write(v)
        Console.WriteLine()

    0