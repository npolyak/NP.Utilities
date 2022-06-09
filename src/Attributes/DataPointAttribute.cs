using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DataPointAttribute : Attribute
    {
        public object Key { get; set; }
        public DataPointDirection Direction { get; }

        public DataPointAttribute(DataPointDirection direction)
        {
            this.Direction = direction;
        }
    }
}
