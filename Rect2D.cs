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
namespace NP.Utilities
{
    public class Rect2D
    {
        public Point2D StartPoint { get; set; } = new Point2D();
        public Point2D EndPoint { get; set; } = new Point2D();

        public double Width =>
            EndPoint.X - StartPoint.X;

        public double Height =>
            EndPoint.Y - StartPoint.Y;

        public Rect2D()
        {

        }

        public Rect2D(Point2D startPoint, Point2D endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }
    }

    public static class Rect2DUtils
    {
        public static (Rect2D rect, Point2D rectPt1, Point2D rectPt2) ToRectAndPointsWithin(this Point2D pt1, Point2D pt2)
        {
            if ((pt1 == null) || (pt2 == null))
                return (null, null, null);

            Point2D startPoint = pt1.Min(pt2);
            Point2D endPoint = pt1.Max(pt2);

            Rect2D rect = new Rect2D(startPoint, endPoint);

            return (rect, pt1.Minus(startPoint), pt2.Minus(startPoint));
        }
    }
}
