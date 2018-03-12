using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultImplAttribute : Attribute
    {
        public Type ImplType { get; }

        public DefaultImplAttribute(Type implType)
        {
            this.ImplType = implType;
        }
    }
}
