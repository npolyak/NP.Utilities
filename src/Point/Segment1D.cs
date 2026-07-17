using System.Numerics;

namespace NP.Utilities.Point;

public record Segment1D<T>(T Start, T End)
    where T : notnull, INumber<T>
{
    public T Length => End - Start;
}

public static class SegmentUtils
{
    public static Segment1D<T> CreateByLength<T>(T start, T length)
        where T : notnull, INumber<T>
    {
        return new Segment1D<T>(start, start + length);
    }

    public static bool Contains<T>(this Segment1D<T> segment, T position)
        where T : notnull, INumber<T>
    {
        return position >= segment.Start && position <= segment.End;
    }

    public static (T startDistance, T endDistance) DistancesToPosition<T>(this Segment1D<T> segment, T position)
        where T : notnull, INumber<T>
    {
        return (T.Abs(position - segment.Start), T.Abs(position - segment.End));
    }

    public static (Side1D side, T distance) ClosestSideAndDistance<T>(this Segment1D<T> segment, T position)
        where T : notnull, INumber<T>
    {
        (T startDistance, T endDistance) = segment.DistancesToPosition(position);

        return (startDistance < endDistance) ? (Side1D.Start, startDistance) : (Side1D.End, endDistance);
    }

    public static Segment1D<T> Shift<T>(this Segment1D<T> segment, T shift) 
        where T : notnull, INumber<T>
    {
        return new Segment1D<T>(segment.Start - shift, segment.End - shift);
    }

    public static Segment1D<T> ShiftStartToPosition<T>(this Segment1D<T> segment, T positionToShiftTo)
        where T : notnull, INumber<T>
    { 
        T delta = segment.Start - positionToShiftTo;

        // positionToShiftTo is the new start, so we need to adjust the end by the same delta
        return new Segment1D<T>(positionToShiftTo, segment.End + delta);
    }

    public static Segment1D<T> ShiftEndToPosition<T>(this Segment1D<T> segment, T positionToShiftTo)
        where T : notnull, INumber<T>
    {
        T delta = segment.End - positionToShiftTo;
        // positionToShiftTo is the new end, so we need to adjust the start by the same delta
        return new Segment1D<T>(segment.Start + delta, positionToShiftTo);
    }
}
