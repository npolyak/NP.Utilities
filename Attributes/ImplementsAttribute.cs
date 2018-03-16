using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementsAttribute : Attribute
    {
        public Type ImplType { get; }

        public ImplementsAttribute(Type implType)
        {
            this.ImplType = implType;
        }
    }
}
