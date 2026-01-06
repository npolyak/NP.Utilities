using System;

namespace NP.Utilities.Attributes
{
    [AttributeUsage
        (   
            AttributeTargets.Property | 
            AttributeTargets.Field, 
            AllowMultiple = false, 
            Inherited = true
        )
    ]
    public class TileDataPointAttribute : Attribute
    {
        public TileDataPointDirection Direction { get; init; }

        public string? TriggersActionName { get; init; } = null;

        public string? ChangedByActionName { get; init; } = null;


    }
}
