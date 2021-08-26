// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

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
