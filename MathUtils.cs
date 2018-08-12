namespace NP.Utilities
{
    public static class MathUtils
    {
        public static double NonNegative(this double d)
        {
            if (d < 0)
                d = 0;

            return d;
        }

        // returns the value needed the add to d so that it would fit the Interval
        // if d is within the interval, 0 is returned.
        public static double BoundaryUpdate(this double d, double lowerBound, double upperBound)
        {
            if ((lowerBound != double.NaN) && (d < lowerBound))
            {
                return lowerBound - d;
            }
            if ((upperBound != double.NaN) && (d > upperBound))
            {
                return upperBound - d;
            }

            return 0;
        }
    }
}
