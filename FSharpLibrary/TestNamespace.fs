namespace Test.FSharpLibrary

type OutsideModuleType =
    {
        Value1 : string
        Value2 : string
    }

module MathModule =

    let Add x y = x + y

    type IPoint =
        abstract member X : float
        abstract member Y : float

    type ConcretePoint(x, y) =
        interface IPoint with
            member __.X = x
            member __.Y = y

    let GetPointAsInterface x y = ConcretePoint(x, y) :> IPoint

    let GetPointAsClass x y = ConcretePoint(x, y)

    let GetLengthOfLine (p1 : IPoint) (p2 : IPoint) =
        sqrt (abs((p2.X - p1.X) ** 2.0 - (p2.Y - p1.Y) ** 2.0))

    let CreatePointArray len =
        [| for i in 1..len -> ConcretePoint(float i, float 1) :> IPoint |]

    let CreatePointSequence len =
        seq { for i in 1..len -> ConcretePoint(float i, float 1) :> IPoint }

    let CreatePointList len =
        [ for i in 1..len -> ConcretePoint(float i, float 1) :> IPoint ]
