using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Attributes
{
    // ExternalToClass means that it is not produced by the class
    // it is produced externally to the class and then set to the property.
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ExternalToClassAttribute : Attribute
    {
    }
}
