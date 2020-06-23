using System;

namespace NP.Utilities.BasicInterfaces
{
    public interface ITrigger
    {
        event Action<object[]> TriggerEvent;
    }
}
