using System;

namespace NP.Utilities
{
    public static class UniqueNumberGenerator
    {
        // just do not call it more often than once per second...
        public static long Generate()
        {
            return (long) DateTime.UtcNow.Subtract(new DateTime(2020,1,1)).TotalSeconds;
        }
    }
}
