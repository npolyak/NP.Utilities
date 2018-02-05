using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
    public class EventThisIdxAttribute : Attribute
    {
        public int ThisIdx { get; } = 0;
        public EventThisIdxAttribute(int thisIdx = 0)
        {

        }
    }
}
