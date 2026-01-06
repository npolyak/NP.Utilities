using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class TileActionAttribute : Attribute
    {
        public string? Name { get; private set; }

        public string[] DependentResults { get; private set; }

        public TileActionAttribute
        (
            string? name = null,
            params string[] dependentResults    
        )
        {
            Name = name;
            DependentResults = dependentResults;
        }
    }
}
