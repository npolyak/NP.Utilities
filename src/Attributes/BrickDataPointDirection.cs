namespace NP.Utilities.Attributes
{
    public enum TileDataPointDirection
    {
        Target,
        Source,
        SourceAndTarget
    }

    public static class DataPointExtensions
    {
        public static bool IsSource(this TileDataPointDirection dataPointDirection)
        {
            return dataPointDirection == TileDataPointDirection.Source || 
                   dataPointDirection == TileDataPointDirection.SourceAndTarget;
        }

        public static bool IsTarget(this TileDataPointDirection dataPointDirection)
        {
            return (dataPointDirection == TileDataPointDirection.Target) ||
                   (dataPointDirection == TileDataPointDirection.SourceAndTarget);
        }

        public static TileDataPointDirection Invert(this TileDataPointDirection dataPointDirection)
        {
            return dataPointDirection switch
            {
                TileDataPointDirection.Target => TileDataPointDirection.Source,
                TileDataPointDirection.Source => TileDataPointDirection.Target,
                _ => TileDataPointDirection.SourceAndTarget
            };
        }
    }
}
