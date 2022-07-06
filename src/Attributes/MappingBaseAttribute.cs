using System;

namespace NP.Utilities.Attributes
{
    public class MappingBaseAttribute : Attribute
    {
        public bool IsSingleton { get; set; } = false;

        public Type TypeToResolve { get; set; }

        public object PartKey { get; } = null;

        public MappingBaseAttribute(bool isSingleton = false, object partKey = null)
        {
            IsSingleton = isSingleton;
            PartKey = partKey;
        }

        public MappingBaseAttribute(Type typeToResolve, bool isSingleton = false, object partKey = null) :
            this(isSingleton, partKey)
        {
            TypeToResolve = typeToResolve;
        }
    }
}
