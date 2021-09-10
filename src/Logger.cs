using System;

namespace NP.Utilities
{
    public static class Logger
    {
        private static ILog Instance { get; set; } = null; 

        public static void SetLog(ILog log)
        {
            if (Instance != null)
            {
                throw new Exception("Programming ERROR: SetLog should be called no more than once.");
            }

            Instance = log;
        }

        public static void Log(LogKind logKind, string component, string msg)
        {
            Instance?.Log(logKind, component, msg);
        }
    }
}
