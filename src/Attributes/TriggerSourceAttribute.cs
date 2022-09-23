using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class TriggerSourceAttribute : Attribute
    {
    }
}
