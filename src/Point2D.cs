// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
global using Point2D = NP.Utilities.Point2D<double>;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;


namespace NP.Utilities
{
    public class Point2D<T>
        where T : IComparable<T>
    {
        [XmlAttribute]
        public T X { get; }

        [XmlAttribute]
        public T Y { get; }

        public Point2D()
        {

        }

        public Point2D<T> Copy()
        {
            return new Point2D<T>(X, Y);
        }

        public Point2D(T x, T y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            Point2D<T> p = obj as Point2D<T>;

            if (p == null)
                return false;

            return (this.X.ObjEquals(p.X)) && (this.Y.ObjEquals(p.Y));
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    //[TypeConverter(typeof(Point2DTypeConverter))]
    //public class Point2D : Point2D<double>
    //{
    //}


    public static class Point2DHelper
    {
        public static Point2D SetFromStr(string str)
        {
            string[] split =
                str.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Count() != 2)
                throw new Exception($"Format Error: Cannot parse string '{str}' as Point2D");


            return new Point2D(double.Parse(split[0]), double.Parse(split[1]));
        }

        public static Point2D ParseToPoint2D(this string str)
        {
            Point2D p = SetFromStr(str);

            return p;
        }
    }

    public static class IntPoint2D
    {

    }

    public class BoolPoint2D : Point2D<bool>
    {
        public BoolPoint2D()
        {

        }

        public BoolPoint2D(bool x, bool y) : base(x, y)
        {

        }

        public bool Any => X || Y;

        public bool All => X && Y;
    }

    public static class Point2DUtils
    {
        public static Point2D<T> ToNonNegative<T>(this Point2D<T> point)
            where T: INumber<T>
        {
            return new Point2D<T>(point.X.NonNegative(), point.Y.NonNegative());
        }

        public static Point2D<T> Transform<T>(this Point2D<T> pt1, Point2D<T> pt2, Func<T, T, T> transformFn)
            where T: INumber<T>
        {
            return new Point2D<T>(transformFn(pt1.X, pt2.X), transformFn(pt1.Y, pt2.Y));
        }

        public static Point2D<T> Min<T>(this Point2D<T> pt1, Point2D<T> pt2)
            where T : INumber<T> 
            =>
            pt1.Transform(pt2, T.Min);


        public static Point2D<T> Max<T>(this Point2D<T> pt1, Point2D<T> pt2) 
            where T : INumber<T> 
            =>
            pt1.Transform(pt2, T.Max);

        public static Point2D<T> Plus<T>(this Point2D<T> pt1, Point2D<T> pt2)
            where T : INumber<T> 
            =>
            pt1.Transform(pt2, (num1, num2) => num1 + num2);

        public static Point2D<T> Minus<T>(this Point2D<T> pt1, Point2D<T> pt2)
            where T: INumber<T>
            =>
            pt1.Transform(pt2, (num1, num2) => num1 - num2);

        public static Point2D<T> Times<TPoint, T>(this TPoint point, T scale)
            where TPoint : Point2D<T>
            where T : INumber<T>
        {
            return new Point2D<T>(point.X * scale, point.Y * scale);
        }

        public static Point2D<T> ToAbs<T>(this Point2D<T> pt)
            where T : INumber<T>
        {
            return new Point2D<T>(T.Abs(pt.X), T.Abs(pt.Y));
        }


        public static T AbsSquared<T>(this Point2D<T> p)
            where T : INumber<T>
        {
            return p.X * p.X + p.Y * p.Y;
        }

        public static Point2D<T> ScaleX<T>(this Point2D<T> p, T scale)
            where T : INumber<T>
        {
            return new Point2D<T>(p.X * scale, p.Y);
        }

        public static Point2D<T> ScaleY<T>(this Point2D<T> p, T scale) 
            where T : INumber<T>
        {
            return new Point2D<T>(p.X, p.Y * scale);
        }

        public static Point2D<T> AddX<T>(this Point2D<T> p, T add)
            where T : INumber<T>
        {
            return new Point2D<T>(p.X + add, p.Y);
        }

        public static Point2D<T> AddY<T>(this Point2D<T> p, T add)
            where T : INumber<T>
        {
            return new Point2D<T>(p.X, p.Y + add);
        }

        private static BoolPoint2D ComparePoints<T>(this Point2D<T> p1, Point2D<T> p2, Func<T, T, bool> compareFn)
            where T : IComparable<T>
        {
            return new BoolPoint2D(compareFn(p1.X, p2.X), compareFn(p1.Y, p2.Y));
        }

        public static BoolPoint2D Less<T>(this Point2D<T> p1, Point2D<T> p2)
            where T : IComparable<T>
        {
            return p1.ComparePoints(p2, (d1, d2) => d1.CompareTo(d2) < 0);
        }

        public static BoolPoint2D LessOrEqual<T>(this Point2D<T> p1, Point2D<T> p2)
            where T : IComparable<T>
        {
            return p1.ComparePoints(p2, (d1, d2) => d1.CompareTo(d2) <= 0);
        }


        public static BoolPoint2D Greater<T>(this Point2D<T> p1, Point2D<T> p2)
            where T : IComparable<T>
        {
            return p1.ComparePoints(p2, (d1, d2) => d1.CompareTo(d2) > 0);
        }

        public static BoolPoint2D GreaterOrEqual<T>(this Point2D<T> p1, Point2D<T> p2)
            where T : IComparable<T>
        {
            return p1.ComparePoints(p2, (d1, d2) => d1.CompareTo(d2) >= 0);
        }

        private static T? RelativePosition<T>(this T start, T end, T position)
            where T : notnull, INumber<T>
        {
            if (position < start || position > end || start == end)
                return default(T?);

            return (position - start) / (end - start);
        }

        private static (Side1D, double) RelativeDistanceToNearestSide(this double start, double end, double position) 
        {
            double? relativeDistance = start.RelativePosition(end, position);

            if (relativeDistance == null)
            {
                return (Side1D.Center, -1);
            }

            if (relativeDistance < 0.5)
            {
                return (Side1D.Start, relativeDistance.Value);
            }
            else
            {
                return (Side1D.End, 1 - relativeDistance.Value);
            }
        }

        public static (Side1D, double) GetNearestSideAndRelativePosition(this Segment1D<double> startEnd, double position)
        {
            return startEnd.Start.RelativeDistanceToNearestSide(startEnd.End, position);
        }

        public static T LocationWithinBoundaries<T>(this Segment1D<T> segment, T position)
            where T : notnull, INumber<T>
        {
            if (position < segment.Start)
                position = segment.Start;
            else if (position > segment.End)
                position = segment.End;

            return position;
        }
    }
}
