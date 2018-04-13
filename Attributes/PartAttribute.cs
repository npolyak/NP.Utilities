using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PartAttribute : Attribute
    {
        PropPartType ThePropPartType { get; }
        public PartAttribute(PropPartType propPartType)
        {
            ThePropPartType = propPartType;
        }
    }
}
