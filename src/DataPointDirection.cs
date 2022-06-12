namespace NP.Utilities
{
    public enum DataPointDirection
    {
        Target,
        Source,
        SourceAndTarget
    }

    public static class DataPointExtensions
    {
        public static bool IsSource(this DataPointDirection dataPointDirection)
        {
            return dataPointDirection == DataPointDirection.Source || 
                   dataPointDirection == DataPointDirection.SourceAndTarget;
        }

        public static bool IsTarget(this DataPointDirection dataPointDirection)
        {
            return (dataPointDirection == DataPointDirection.Target) ||
                   (dataPointDirection == DataPointDirection.SourceAndTarget);
        }

        public static DataPointDirection Invert(this DataPointDirection dataPointDirection)
        {
            return dataPointDirection switch
            {
                DataPointDirection.Target => DataPointDirection.Source,
                DataPointDirection.Source => DataPointDirection.Target,
                _ => DataPointDirection.SourceAndTarget
            };
        }
    }
}
