using System;
using System.Collections.Generic;
using System.Text;

namespace NP.Utilities.BasicInterfaces
{
    public interface ISuspendable
    {
        void Suspend();

        void Reset(bool resetItems = true);
    }
}
