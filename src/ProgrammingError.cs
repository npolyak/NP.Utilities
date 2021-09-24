using System;

namespace NP.Utilities
{
    public class ProgrammingError : Exception
    {
        public ProgrammingError(string msg, params object[] args) : base("Programming ERROR: " + string.Format(msg, args))
        {
        }
    }
}
