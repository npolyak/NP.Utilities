using System;

namespace NP.Utilities
{
    public static class ExceptionUtils
    {
        public static void Throw(this string str) => throw new Exception(str);

        public static void ThrowProgError(this string str) => throw new ProgrammingError(str);


        public static void ThrowIfFalse(this bool b, string msg)
        {
            if (!b)
            {
                msg.ThrowProgError();
            }
        }

        public static void ThrowIfNull(this object nonNullObj, string msg)
        {
            (nonNullObj != null).ThrowIfFalse(msg);
        }
    }
}
