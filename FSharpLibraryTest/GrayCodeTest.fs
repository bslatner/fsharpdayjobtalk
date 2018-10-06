module GrayCodeTest

open Expecto
open GrayCode

[<Tests>]
let tests = 
    testList "Gray Code Tests" [
        testCase "Round trips yield original result (16-bit)" <| fun _ ->
            printfn "Hi!"
            let test (num : uint16) =
                let result =
                    BinaryToGray16 num 
                    |> GrayToBinary16
                Expect.equal result num "should be equal"

            test 0x0001us
            test 0xDEADus
            test 0xBEEFus
            test 0xFFFFus

        testCase "Round trips yield original result (32-bit)" <| fun _ ->
            let test (num : uint32) =
                let result =
                    BinaryToGray32 num 
                    |> GrayToBinary32
                Expect.equal result num "should be equal"

            test 0x00000001ul
            test 0xDEADBEEFul
            test 0xBEEFDEADul
            test 0xFFFFFFFFul

        testCase "Round trips yield original result (64-bit)" <| fun _ ->
            let test (num : uint64) =
                let result =
                    BinaryToGray64 num 
                    |> GrayToBinary64
                Expect.equal result num "should be equal"

            test 0x0000000000000001UL
            test 0xDEADBEEFFEEDBEEFUL
            test 0xFFFFFFFFFFFFFFFFUL
    ]