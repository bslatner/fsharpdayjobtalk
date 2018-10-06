module Solver

open System

// Represents one square on the game board and the possible solutions to it.
type Square =
    {
        Value : int option
        Possibilities : Set<int>
    }

// Parse a game board.
//
// Expected input is 9 strings consisting of 9 numeric characters or a space.
// Spaces represent unsolved squares.
let ParseGame (lines : string[]) : Square[,] =

    let parseNumber (ch : char) : Square =
        match ch with
        | ' ' -> { Value = None; Possibilities = Set.empty<int> }
        | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' -> { Value = Some(Int32.Parse(string ch)); Possibilities = Set.empty<int> }
        | _ -> failwith "Invalid character"

    let parseLine (line : string) : Square[] =
        line |> Seq.map parseNumber |> Seq.toArray

    let parsed = lines |> Array.map parseLine

    if parsed.Length <> 9 then failwith "Game does not have 9 rows"
    parsed |> Array.iteri(fun idx item -> if parsed.[idx].Length <> 9 then failwith (sprintf "Row %i does not have 9 columns" (idx + 1)))

    Array2D.init 9 9 (fun row col -> parsed.[row].[col])

// Solve a sudoku game.
let SolveGame (game : Square[,]) =

    let getRegion row col : Square[,] =
        let firstRow = (row / 3) * 3
        let firstCol = (col / 3) * 3
        game.[firstRow..firstRow+2,firstCol..firstCol+2]

    let flatten (A:'a[,]) = A |> Seq.cast<'a>

    let getValuesInColumn col =
        game.[0..8,col] |> Array.choose(fun sq -> sq.Value) |> Set.ofArray

    let getValuesInRow row =
        game.[row,0..8] |> Array.choose(fun sq -> sq.Value) |> Set.ofArray

    let getValuesInRegion row col =
        let region = getRegion row col
        let flattened = Array.ofSeq (flatten region)
        flattened 
            |> Array.choose(fun sq -> sq.Value) 
            |> Set.ofArray

    let difference set1 set2 =
        Set.difference set2 set1

    // Return possibilities by simple elimination. Eliminate numbers
    // in the same row, same column, and same region.
    let findPossibilitiesSimple row col =
        match game.[row,col].Value with
        | Some(int) -> Set.empty<int>
        | _ -> 
            Set<int> [1..9] 
            |> difference(getValuesInRow row)
            |> difference(getValuesInColumn col)
            |> difference(getValuesInRegion row col)

    // Get the possibilities for other squares in the same row.
    let getOtherPossibilitiesInRow row col =
        seq { for c in 0..8 do if c <> col then yield game.[row, c].Possibilities }

    // Get the possibilities for other squares in the same column
    let getOtherPossibilitiesInColumn row col =
        seq { for r in 0..8 do if r <> row then yield game.[r, col].Possibilities }

    // Get the possibilities for other squares in the same region.
    let getOtherPossibilitiesInRegion row col =
        let region = getRegion row col
        let regionRow = row % 3
        let regionCol = col % 3

        seq {
            for r in 0..2 do
                for c in 0..2 do
                    if not (r = regionRow && c = regionCol) then
                        yield region.[r,c].Possibilities
        }

    // Return possibilities for a square, removing possibility subsets for other squares
    // that appear in the same row, column, or region.
    //
    // For example, if the a square has possibilities, 1-3-9 and two other squares
    // in the same row both have possibilities 1 and 9, then we can eliminate 1 and 9 from
    // the possibilities for the square.
    let removeRepeatingSubsets row col setLength =
        let possibilities = game.[row,col].Possibilities

        let getRepeatingSubsets (others : seq<Set<int>>) =
            others
            // possibilities must be exactly as long as the length we're looking for
            |> Seq.filter(fun o -> o.Count = setLength)
            // count repetitions
            |> Seq.countBy id
            // filter out repetitions that don't match the length we're looking for
            |> Seq.filter(fun (posSet, count) -> count = setLength)
            // discard the counter
            |> Seq.map(fun (posSet, count) -> posSet)
            // return the union of all repeating subsets
            |> Set.unionMany

        let rowPossibilities = getOtherPossibilitiesInRow row col
        let colPossibilities = getOtherPossibilitiesInColumn row col
        let regPossibilities = getOtherPossibilitiesInRegion row col

        let newPossibilities =
            possibilities
            |> difference(getRepeatingSubsets rowPossibilities)
            |> difference(getRepeatingSubsets colPossibilities)
            |> difference(getRepeatingSubsets regPossibilities)

        newPossibilities
            
    let removePossibility row col p =
        let sq = game.[row,col]
        let newPossibilities = sq.Possibilities |> difference p
        game.[row,col] <- { sq with Possibilities = newPossibilities }

    let solveSquare row col answer =
        game.[row,col] <- { Value = Some(answer); Possibilities = Set.empty<int> }

        let answerSet = set [answer]
        for r in 0..8 do
            if r <> row then removePossibility r col answerSet
        for c in 0..8 do
            if c <> col then removePossibility row c answerSet
        let firstRegionRow = (row / 3) * 3
        let firstRegionCol = (col / 3) * 3
        for r in firstRegionRow..(firstRegionRow + 2) do
            for c in firstRegionCol..(firstRegionCol + 2) do
                if r <> row && col <> col then removePossibility r c answerSet

    let tryToSolve row col =
        let square = game.[row,col]
        if Set.count square.Possibilities = 1 then
            let answer = Set.minElement square.Possibilities
            solveSquare row col answer

    let getIsGameSolved : bool =
        let mutable allSolved = true

        let check square =
            allSolved <- allSolved && (square.Value <> None)

        game |> Array2D.iter(check)
        allSolved

    let doWorkUnsolved f =
        let doWork row col =
            f row col
            tryToSolve row col

        for row in 0..8 do
            for col in 0..8 do if game.[row,col].Value = None then doWork row col

    let mutable numIterations = 0
    while not getIsGameSolved && numIterations < 100 do
        numIterations <- numIterations + 1

        doWorkUnsolved (fun row col -> 
            game.[row,col] <- { game.[row,col] with Possibilities = findPossibilitiesSimple row col }
        )
        doWorkUnsolved (fun row col ->
            game.[row,col] <- { game.[row,col] with Possibilities = removeRepeatingSubsets row col 2 }
            game.[row,col] <- { game.[row,col] with Possibilities = removeRepeatingSubsets row col 3 }
        )

    game