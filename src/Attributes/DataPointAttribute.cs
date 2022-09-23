using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DataPointAttribute : Attribute
    {
        public DataPointDirection Direction { get; }

        public string? TriggersActionName { get; }

        public string? ChangedByActionName { get; }

        public DataPointAttribute
        (
            DataPointDirection direction, 
            string triggersActionName = null,
            string changedByActionName = null)
        {
            this.Direction = direction;
            TriggersActionName = triggersActionName;
            ChangedByActionName = changedByActionName;
        }
    }
}
