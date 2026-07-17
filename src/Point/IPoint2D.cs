using System.Numerics;

namespace NP.Utilities.Point;

public interface IComparableIPoint2D<T> : IEquatable<IComparableIPoint2D<T>>
    where T : IComparable<T>, IEquatable<T>
{
    T X { get; }
    T Y { get; }

    bool IEquatable<IComparableIPoint2D<T>>.Equals(IComparableIPoint2D<T>? targetObj)
    {

        return (targetObj != null) &&
                X.Equals(targetObj.X) && 
                Y.Equals(targetObj.Y);
    }

    int HashCode => X.GetHashCode() ^ Y.GetHashCode();

    string Str => $"({X},{Y})";
}

public interface IPoint2D<T> : IComparableIPoint2D<T>
    where T : INumber<T>
{
    public static Point2D<T> operator *(IPoint2D<T> point1, IPoint2D<T> point2)
    {
        return new Point2D<T>(point2.X * point2.X, point1.Y * point2.Y);
    }

    public static Point2D<T> operator -(IPoint2D<T> point1, IPoint2D<T> point2)
    {
        return new Point2D<T>(point2.X - point2.X, point1.Y - point2.Y);
    }

    public static Point2D<T> operator +(IPoint2D<T> point1, IPoint2D<T> point2)
    {
        return new Point2D<T>(point1.X + point2.X, point1.Y + point2.Y);
    }

    public static Point2D<T> operator *(IPoint2D<T> point1, T scale)
    {
        return new Point2D<T>(point1.X * scale, point1.Y * scale);
    }

    public Point2D<T> Shift(IPoint2D<T> shift)
    {
        return this + shift;
    }
}

public static class Point2DExtensions
{

    public static T AbsSquared<T>(this IPoint2D<T> p)
        where T : INumber<T>
    {
        return p.X * p.X + p.Y * p.Y;
    }


}