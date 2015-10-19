module GrayCode

let BinaryToGray16 (num : uint16) =
    (num >>> 1) ^^^ num

let GrayToBinary16 (num : uint16) =
    let mutable temp = num ^^^ (num >>> 8)
    temp <- temp ^^^ (temp >>> 4)
    temp <- temp ^^^ (temp >>> 2)
    temp ^^^ (temp >>> 1)

let BinaryToGray32 (num : uint32) =
    (num >>> 1) ^^^ num

let GrayToBinary32 (num : uint32) =
    let mutable temp = num ^^^ (num >>> 16)
    temp <- temp ^^^ (temp >>> 8)
    temp <- temp ^^^ (temp >>> 4)
    temp <- temp ^^^ (temp >>> 2)
    temp ^^^ (temp >>> 1)

let BinaryToGray64(num : uint64) =
    (num >>> 1) ^^^ num

let GrayToBinary64(num : uint64) =
    let mutable temp = num ^^^ (num >>> 32)
    temp <- temp ^^^ (temp >>> 16)
    temp <- temp ^^^ (temp >>> 8)
    temp <- temp ^^^ (temp >>> 4)
    temp <- temp ^^^ (temp >>> 2)
    temp ^^^ (temp >>> 1)
