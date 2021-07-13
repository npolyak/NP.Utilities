// (c) Nick Polyak 2018 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
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
