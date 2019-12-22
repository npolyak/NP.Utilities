using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class CompositeConstructorAttribute : Attribute
    {
    }
}
