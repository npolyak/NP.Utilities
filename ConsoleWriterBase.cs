using NP.Utilities.BasicInterfaces;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace NP.Utilities
{
    public abstract class ConsoleWriterBase : TextWriter
    {
        public override Encoding Encoding =>
            Encoding.ASCII;

        SynchronizationContext _syncContext = null;

        public ConsoleWriterBase()
        {
            _syncContext = SynchronizationContext.Current;

            Console.SetOut(this);
        }

        void PerformInSyncContext<T>(T val, Action<T> operation)
        {
            if (_syncContext != null)
            {
                _syncContext.Send((state) => operation(val), null);
            }
            else
            {
                operation(val);
            }
        }

        protected abstract void WriteString(string str);

        public override void Write(char value)
        {
            PerformInSyncContext<char>
            (
                value,
                (val) => WriteString("" + val)
            );
        }

        public override void Write(string value)
        {
            PerformInSyncContext<string>
            (
                value,
                (val) => WriteString(val)
            );
        }
    }
}
