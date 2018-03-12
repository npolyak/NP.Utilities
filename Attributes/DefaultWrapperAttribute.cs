using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultWrapperAttribute : Attribute
    {
        public Type WrapperType { get; }
        public DefaultWrapperAttribute(Type wrapperType)
        {
            this.WrapperType = wrapperType;
        }
    }
}
