module GrayCodeTest

open NUnit.Framework
open FsUnit
open GrayCode

[<TestCase(0x0001us)>]
[<TestCase(0xDEADus)>]
[<TestCase(0xBEEFus)>]
[<TestCase(0xFFFFus)>]
let ``Round trips yield original result (16-bit)`` (num : uint16) =
    BinaryToGray16 num 
    |> GrayToBinary16
    |> should equal num

[<TestCase(0x00000001ul)>]
[<TestCase(0xDEADBEEFul)>]
[<TestCase(0xBEEFDEADul)>]
[<TestCase(0xFFFFFFFFul)>]
let ``Round trips yield original result (32-bit)`` (num : uint32) =
    BinaryToGray32 num 
    |> GrayToBinary32
    |> should equal num

[<TestCase(0x0000000000000001UL)>]
[<TestCase(0xDEADBEEFFEEDBEEFUL)>]
[<TestCase(0xFFFFFFFFFFFFFFFFUL)>]
let ``Round trips yield original result (64-bit)`` (num : uint64) =
    BinaryToGray64 num 
    |> GrayToBinary64
    |> should equal num

