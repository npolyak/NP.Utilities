using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class TileActionAttribute : Attribute
    {
        public string[] DependsOnProps { get; }
        public string[] DependentResults { get; }

        public TileActionAttribute
        (
            string[] dependsOnProps,
            params string[] dependentResults    
        )
        {
            DependsOnProps = dependsOnProps;
            DependentResults = dependentResults;
        }
    }
}
