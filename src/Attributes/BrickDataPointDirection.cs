namespace NP.Utilities.Attributes
{
    public enum BrickDataPointDirection
    {
        Target,
        Source,
        SourceAndTarget
    }

    public static class DataPointExtensions
    {
        public static bool IsSource(this BrickDataPointDirection dataPointDirection)
        {
            return dataPointDirection == BrickDataPointDirection.Source || 
                   dataPointDirection == BrickDataPointDirection.SourceAndTarget;
        }

        public static bool IsTarget(this BrickDataPointDirection dataPointDirection)
        {
            return (dataPointDirection == BrickDataPointDirection.Target) ||
                   (dataPointDirection == BrickDataPointDirection.SourceAndTarget);
        }

        public static BrickDataPointDirection Invert(this BrickDataPointDirection dataPointDirection)
        {
            return dataPointDirection switch
            {
                BrickDataPointDirection.Target => BrickDataPointDirection.Source,
                BrickDataPointDirection.Source => BrickDataPointDirection.Target,
                _ => BrickDataPointDirection.SourceAndTarget
            };
        }
    }
}
