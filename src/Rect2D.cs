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
global using Rect2D = NP.Utilities.Rect2D<double>;
using System.Numerics;

using static NP.Utilities.Side2D;

namespace NP.Utilities
{
    public class Rect2D<T>
        where T : INumber<T>
    {
        public Point2D<T> StartPoint { get; } = new Point2D<T>();
        public Point2D<T> EndPoint { get; } = new Point2D<T>();

        public T Width =>
            EndPoint.X - StartPoint.X;

        public T Height =>
            EndPoint.Y - StartPoint.Y;

        public Rect2D()
        {

        }

        public Rect2D(Point2D<T> startPoint, Point2D<T> endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }


        public override string ToString()
        {
            return $"{{{StartPoint}, {EndPoint}}}";
        }

        public static Point2D Parse(string s)
        {
            return null;
        }
    }

    public static class Rect2DUtils
    {
        public static (Rect2D<T> rect, Point2D<T> rectPt1, Point2D<T> rectPt2) ToRectAndPointsWithin<T>(this Point2D<T> pt1, Point2D<T> pt2)
            where T: INumber<T>
        {
            if ((pt1 == null) || (pt2 == null))
                return (null, null, null);

            Point2D<T> startPoint = pt1.Min(pt2);
            Point2D<T> endPoint = pt1.Max(pt2);

            Rect2D<T> rect = new Rect2D<T>(startPoint, endPoint);

            return (rect, pt1.Minus(startPoint), pt2.Minus(startPoint));
        }

        public static bool ContainsPoint<T>(this Rect2D<T> rect, Point2D<T> pt)
            where T: INumber<T>
        {
            return pt.GreaterOrEqual(rect.StartPoint).All &&
                pt.LessOrEqual(rect.EndPoint).All;
        }

        public static Point2D<T> GetSize<T>(this Rect2D<T> rect, T scale)
            where T : INumber<T>
        {
            return new Point2D<T>(rect.Width * scale, rect.Height * scale);
        }

        public static Point2D<T> GetSize<T>(this Rect2D<T> rect)
            where T : INumber<T>
        {
            return rect.GetSize(T.One);
        }

        public static Rect2D<T> ScaleWidth<T>(this Rect2D<T> rect, T scaleFactor, bool  fromStart = true)
            where T : INumber<T>
        {
            T width = rect.GetSize().ScaleX(scaleFactor).X;

            if (fromStart)
            {
                Point2D<T> endPoint = rect.StartPoint.AddX(width);

                return new Rect2D<T>(rect.StartPoint, rect.EndPoint);
            }
            else // from end
            {
                Point2D<T> startPoint = rect.EndPoint.AddX(-width);

                return new Rect2D<T>(startPoint, rect.EndPoint);
            }
        }

        public static Rect2D<T> ScaleHeight<T>(this Rect2D<T> rect, T scaleFactor, bool fromStart = true)
            where T : INumber<T>
        {
            T height = rect.GetSize().ScaleY(scaleFactor).Y;

            if (fromStart)
            {
                Point2D<T> endPoint = rect.StartPoint.AddY(height);

                return new Rect2D<T>(rect.StartPoint, endPoint);
            }
            else // from end
            {
                Point2D<T> startPoint = rect.EndPoint.AddY(-height);

                return new Rect2D<T>(startPoint, rect.EndPoint);
            }
        }


        public static Rect2D<T> ScaleToSide<T>(this Rect2D<T> rect, T scale, Side2D sideToScaleTo)
            where T : INumber<T>
        {
            if (sideToScaleTo == Center)
                return rect;

            bool fromStart = sideToScaleTo.IsStart();

            return sideToScaleTo.IsX() ? rect.ScaleWidth(scale, fromStart) : rect.ScaleHeight(scale, fromStart);
        }
    }
}
