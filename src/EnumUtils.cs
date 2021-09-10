
using System;
using System.Linq;

namespace NP.Utilities
{
    public static class EnumUtils
    {
        public static TOut Cast<TIn, TOut>(this TIn val)
            where TOut : struct, Enum
            where TIn : struct, Enum
        {
            return Enum.GetValues<TOut>().First(v => v.ToString() == val.ToString());
        }
    }
}
