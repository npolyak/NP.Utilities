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
using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace NP.Utilities
{
    public class Point2D<T>
        where T : IComparable<T>
    {
        [XmlAttribute]
        public T X { get; set; } = default;

        [XmlAttribute]
        public T Y { get; set; } = default;

        public Point2D()
        {

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

    [TypeConverter(typeof(Point2DTypeConverter))]
    public class Point2D : Point2D<double>
    {
        public Point2D()
        {

        }

        public Point2D(double x, double y) : base(x, y)
        {
            X = x;
            Y = y;
        }

        public Point2D(string str)
        {
            this.SetFromStr(str);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public double AbsSquared()
        {
            return X * X + Y * Y;
        }
    }


    public static class Point2DHelper
    {
        public static void SetFromStr(this Point2D p, string str)
        {
            string[] split =
                str.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Count() != 2)
                throw new Exception($"Format Error: Cannot parse string '{str}' as Point2D");


            p.X = double.Parse(split[0]);
            p.Y = double.Parse(split[1]);
        }

        public static Point2D ParseToPoint2D(this string str)
        {
            Point2D p = new Point2D();

            SetFromStr(p, str);

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
        public static Point2D ToNonNegative(this Point2D point)
        {
            return new Point2D(point.X.NonNegative(), point.Y.NonNegative());
        }

        public static Point2D Transform(this Point2D pt1, Point2D pt2, Func<double, double, double> transformFn)
        {
            return new Point2D(transformFn(pt1.X, pt2.X), transformFn(pt1.Y, pt2.Y));
        }

        public static Point2D Min(this Point2D pt1, Point2D pt2) =>
            pt1.Transform(pt2, Math.Min);


        public static Point2D Max(this Point2D pt1, Point2D pt2) =>
            pt1.Transform(pt2, Math.Max);

        public static Point2D Plus(this Point2D pt1, Point2D pt2) =>
            pt1.Transform(pt2, (num1, num2) => num1 + num2);

        public static Point2D Minus(this Point2D pt1, Point2D pt2) =>
            pt1.Transform(pt2, (num1, num2) => num1 - num2);

        public static Point2D ToAbs(this Point2D pt)
        {
            return new Point2D(Math.Abs(pt.X), Math.Abs(pt.Y));
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
    }
}
