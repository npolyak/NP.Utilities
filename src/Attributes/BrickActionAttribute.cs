using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class BrickActionAttribute : Attribute
    {
        public string? Name { get; private set; }

        public string[] DependentResults { get; private set; }

        public BrickActionAttribute
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
