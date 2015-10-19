using System;
using System.Linq;

namespace CSharpLibrary
{
    public class FSharpConsumer
    {
        public int CallFSharpModuleFunction()
        {
            return TestModule.Multiply(2, 3);
        }

        public TestModule.Shape CreateSquare()
        {
            return TestModule.Shape.NewSquare(1.0);
        }

        public double ComputeArea(TestModule.Shape shape)
        {
            return TestModule.Area(shape);
        }

        public bool GetIsSquare(TestModule.Shape shape)
        {
            return shape.IsSquare;
        }

        public double GetSquareSize(TestModule.Shape shape)
        {
            return ((TestModule.Shape.Square) shape).Item;
        }

        public Tuple<int, int> MakeTuple(int x, int y)
        {
            var t = TestModule.Tuplefy(x, y);
            return new Tuple<int, int>(t.Item1 + 1, t.Item2 + 1);
        }

        public int AddInNamespace()
        {
            return Test.FSharpLibrary.MathModule.Add(2, 3);
        }

        public double ClassAndInterfaceDemo(double x1, double y1, double x2, double y2)
        {
            var p1 = Test.FSharpLibrary.MathModule.GetPointAsInterface(x1, y1);
            var p2 = Test.FSharpLibrary.MathModule.GetPointAsClass(x2, y2);
            var p2Interface = (Test.FSharpLibrary.MathModule.IPoint)p2;

            Console.WriteLine("p1 = ({0},{1})", p1.X, p1.Y);
            Console.WriteLine("p2 = ({0},{1})", p2Interface.X, p2Interface.Y);

            return Test.FSharpLibrary.MathModule.GetLengthOfLine(p1, p2);
        }

        public static void ListsAndSequencesAndArraysOhMy()
        {
            var arr = Test.FSharpLibrary.MathModule.CreatePointArray(10);
            var seq = Test.FSharpLibrary.MathModule.CreatePointSequence(10);
            var lst = Test.FSharpLibrary.MathModule.CreatePointList(10);

            var index = 0;
            foreach (var pt in arr)
            {
                Console.WriteLine("p{0} = ({1},{2})", index++, pt.X, pt.Y);
            }

            index = 0;
            foreach (var pt in seq)
            {
                Console.WriteLine("p{0} = ({1},{2})", index++, pt.X, pt.Y);
            }

            index = 0;
            foreach (var pt in lst)
            {
                Console.WriteLine("p{0} = ({1},{2})", index++, pt.X, pt.Y);
            }
        }
    }
}
