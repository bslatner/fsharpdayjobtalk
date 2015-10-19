module TestModule

type Shape =
    | Square of float
    | Rectangle of float * float
    | Circle of float

let Multiply x y = x * x

let Area (shape : Shape) =
    match shape with
    | Square x -> x * x
    | Rectangle(h, w) -> h * w
    | Circle(r) -> System.Math.PI * r * r

let Tuplefy x y = (x, y)
