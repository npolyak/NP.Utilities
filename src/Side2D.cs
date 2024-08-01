using static NP.Utilities.Side2D;

namespace NP.Utilities;

public enum Side2D
{
    Center,
    Left,
    Top,
    Right,
    Bottom
}

public static class Side2DUtils
{
    public static bool IsX(this Side2D side) => side is Left or Right;

    public static bool IsY(this Side2D side) => side is Top or Bottom;

    public static bool IsStart(this Side2D side) => side is Left or Top;

    public static Side2D GetSide2D(this Side1D side, bool horizontalOrVertical)
    {
        return side switch
        {
            Side1D.Start => horizontalOrVertical ? Left : Top,
            Side1D.End => horizontalOrVertical ? Right : Bottom,
            _ => Center
        };
    }
}
