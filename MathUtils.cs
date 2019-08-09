namespace NP.Utilities
{
    public static class MathUtils
    {
        public const int Gig = 1000_000_000;


        public const int Million = 1000_000;

        public const int Thousand = 1000;


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

        public static bool IsDivisableBy(this int i, int divisor)
        {
            return i % divisor == 0;
        }

        public static string IntToStr(this int i, int divisor, string divisorLetter)
        {
            if (!i.IsDivisableBy(divisor))
                return null;

            return string.Format("{0:##,#}", i/divisor) + divisorLetter;
        }
    }
}
