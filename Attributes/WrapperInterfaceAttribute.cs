using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class WrapperInterfaceAttribute : Attribute
    {
        public Type TypeToImplement { get; }
        public WrapperInterfaceAttribute(Type typeToImplement)
        {
            this.TypeToImplement = typeToImplement;
        }
    }
}
 