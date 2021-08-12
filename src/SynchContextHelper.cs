using System;
using System.Threading;

namespace NP.Utilities
{
    public static class SynchContextHelper
    {
        public static void RunWithinContext(this SynchronizationContext context, Action actionToRun)
        {
            if (context == SynchronizationContext.Current || context == null)
            {
                actionToRun?.Invoke();
            }
            else
            {
                context.Send(_ => actionToRun?.Invoke(), null);
            }
        }
    }
}
