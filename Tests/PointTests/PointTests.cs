using NP.Utilities.Point;

namespace PointTests;

public static class PointTests
{
    [Fact]
    public static void FitRectIntoBoundsTest()
    {
        Rect2D<double> boundsRect = new Rect2D<double>(0, 0, 100, 50);

        Rect2D<double> dragRect = new Rect2D<double>(0, 0, 20, 20);

        Point2D<double> shift = new Point2D<double>(100, -20);

        (var dragRectStart, var dragDelta) = boundsRect.FitRectangleToRectangle(dragRect.Shift(shift));

        var realDragRectStart = new Point2D<double>(80, 0);
        Assert.Equal(realDragRectStart, dragRectStart);

        var realDragDelta = new Point2D<double>(-20, 20);
        Assert.Equal(realDragDelta, dragDelta);

    }
}
