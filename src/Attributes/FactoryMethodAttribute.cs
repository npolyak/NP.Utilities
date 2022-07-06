using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FactoryMethodAttribute : MappingBaseAttribute
    {
        public FactoryMethodAttribute(bool isSingleton = false, object partKey = null) :
            base(isSingleton, partKey)
        {

        }

        public FactoryMethodAttribute(Type typeToResolve, bool isSingleton = false, object partKey = null) :
            base(typeToResolve, isSingleton, partKey)
        {

        }
    }
}
