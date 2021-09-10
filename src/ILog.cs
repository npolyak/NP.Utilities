namespace NP.Utilities
{
    public enum LogKind
    {
        Trace,
        Debug,
        Info, 
        Warning, 
        Error,
        Fatal,
        Off
    }

    public interface ILog
    {
        public void Log(LogKind logKind, string component, string msg);
    }
}
