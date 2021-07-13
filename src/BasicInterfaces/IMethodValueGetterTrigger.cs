using System;

namespace NP.Utilities.BasicInterfaces
{
    public interface IMethodValueGetterTrigger
    {
        event Func<object[], object> GetValue;
    }
}
