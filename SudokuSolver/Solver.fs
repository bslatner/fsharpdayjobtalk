module Solver

open System

// Represents one square on the game board and the possible solutions to it.
type Square =
    {
        Value : int option
        Possibilities : Set<int>
    }

// Represents the whole game board
type GameBoard = Square[,]

// The context of solving a particular square
type SolverContext =
    {
        Board : GameBoard
        Row : int
        Col : int
        Square : Square
        SquareRow : Square[]
        SquareRowValues : int[]
        SquareCol : Square[]
        SquareColValues : int[]
        SquareRegionHomeRow : int
        SquareRegionHomeCol : int
        SquareRegion : Square[,]
        SquareRegionValues : int[]
    }

// A function that restricts the values of a square. Returns a list
// of values NOT POSSIBLE for the square.
type Restriction = SolverContext -> Set<int>

let restrictSameRow context =
    context.SquareRowValues |> Set.ofSeq

let restrictSameCol context =
    context.SquareColValues |> Set.ofSeq

let restrictSameRegion context =
    context.SquareRegionValues |> Set.ofSeq

let standardRestrictions = [| restrictSameRow; restrictSameCol; restrictSameRegion|]

// Parse a game board.
//
// Expected input is 9 strings consisting of 9 numeric characters or a space.
// Spaces represent unsolved squares.
let ParseGame (lines : string[]) : GameBoard =

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
let SolveGame (restrictions : Restriction[]) (board : GameBoard) =

    let flatten (A:'a[,]) = A |> Seq.cast<'a> |> Array.ofSeq

    let subtractSet set1 set2 = Set.difference set2 set1

    let getRegion (board' : GameBoard) homeRow homeCol =
        board'.[homeRow..homeRow+2,homeCol..homeCol+2]

    let getIsGameSolved : bool =
        let mutable allSolved = true

        let check square =
            allSolved <- allSolved && (square.Value <> None)

        board |> Array2D.iter(check)
        allSolved

    let allPossibilities = [|1..9|] |> Set.ofArray

    let getPossibilities restrictions =
        Set.difference allPossibilities restrictions

    let getValueFromPossibilities (possibilities : Set<int>) =
        match possibilities |> Set.count with 
        | 1 -> possibilities |> Set.minElement |> Some
        | _ -> None

    let setSquare context possibilities =
        let row = context.Row
        let col = context.Col
        let value = getValueFromPossibilities possibilities
        context.Board.[row,col] <- { context.Board.[row,col] with Value = value; Possibilities = possibilities }

    // Solve a square by simple elimination of restricted values. If the set
    // of possibilities comes down to one value, that's the value for the square.
    let solveSquareSimple context =
        restrictions 
        |> Seq.map (fun r -> r context) 
        |> Set.unionMany
        |> getPossibilities
        |> setSquare context

    let getRepeatingSubsets (others : seq<Set<int>>) setLength =
        others
        // possibilities must be exactly as long as the length we're looking for
        |> Seq.filter(fun o -> o.Count = setLength)
        // count repetitions
        |> Seq.countBy id
        // filter out repetitions that don't match the length we're looking for
        |> Seq.filter(fun (_, count) -> count = setLength)
        // discard the counter
        |> Seq.map(fun (posSet, _) -> posSet)
        // return the union of all repeating subsets
        |> Set.unionMany

    // Solve a square by removing repeating subsets from possibilities. 
    //
    // For example, if a square has possibilities 1-3-9 and two other squares in the 
    // same row both have possibilities 1 and 9, then we can eliminate 1 and 9 from
    // the possibilities for the square.
    let solveSquareByRemovingRepeatingSubsets setLength context =
        let row = context.Row
        let col = context.Col
        let rowPossibilities = [| for c in 0..8 do if c <> col then yield context.Board.[row, c].Possibilities |]
        let colPossibilities = [| for r in 0..8 do if r <> row then yield context.Board.[r, col].Possibilities |]
        let regPossibilities = [|
                for r in 0..2 do
                    for c in 0..2 do
                        if not (r = row && c = col) then
                            yield context.SquareRegion.[r,c].Possibilities
        |]

        let newPossibilities = 
            context.Square.Possibilities
            |> subtractSet (getRepeatingSubsets rowPossibilities setLength)
            |> subtractSet (getRepeatingSubsets colPossibilities setLength)
            |> subtractSet (getRepeatingSubsets regPossibilities setLength)
        setSquare context newPossibilities

    let solveBoard solvers  =
        for row in 0..8 do
            let rowArray = board.[row,0..8]
            let rowValues = rowArray |> Array.choose (fun sq -> sq.Value)
            let regionHomeRow = (row / 3) * 3
            for col in 0..8 do
                if board.[row,col].Value.IsNone then
                    let colArray = board.[0..8,col]
                    let colValues = colArray |> Array.choose (fun sq -> sq.Value)
                    let regionHomeCol = (col / 3) * 3
                    let region = getRegion board regionHomeRow regionHomeCol

                    let runSolver s =
                        if board.[row,col].Value.IsNone then
                            let context =
                                {
                                    Board = board
                                    Row = row
                                    Col = col
                                    Square = board.[row,col]
                                    SquareRow = rowArray
                                    SquareRowValues = rowValues
                                    SquareCol = colArray
                                    SquareColValues = colValues
                                    SquareRegionHomeRow = regionHomeRow
                                    SquareRegionHomeCol = regionHomeCol
                                    SquareRegion = region
                                    SquareRegionValues = region |> flatten |> Array.choose (fun sq -> sq.Value)
                                }
                            s context

                    solvers |> Seq.iter runSolver

    let mutable numIterations = 0
    while not getIsGameSolved && numIterations < 100 do
        numIterations <- numIterations + 1
        solveBoard [|
                    solveSquareSimple
                    solveSquareByRemovingRepeatingSubsets 2
                    solveSquareByRemovingRepeatingSubsets 3
                   |]

    board

// Solve a game using standard restrictions.
let SolveGameStandard = SolveGame standardRestrictions