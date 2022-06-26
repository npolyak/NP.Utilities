using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ActionAttribute : Attribute
    {
        public string Name { get; private set; }

        public ActionAttribute(string name = null)
        {
            Name = name;
        }
    }
}
