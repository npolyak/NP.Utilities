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
using System.Linq;
using System.Xml.Serialization;

namespace NP.Utilities
{
    public class Point2D
    {
        [XmlAttribute]
        public double X { get; set; } = 0d;

        [XmlAttribute]
        public double Y { get; set; } = 0d;

        public Point2D()
        {

        }

        public Point2D(string str)
        {
            string[] split =
                str.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Count() != 2)
                return;

            X = double.Parse(split[0]);
            Y = double.Parse(split[1]);
        }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            Point2D p = obj as Point2D;

            if (p == null)
                return false;

            return (this.X == p.X) && (this.Y == p.Y);
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

        public static Point2D Minus(this Point2D pt1, Point2D pt2) =>
            pt1.Transform(pt2, (num1, num2) => num1 - num2);
    }
}
