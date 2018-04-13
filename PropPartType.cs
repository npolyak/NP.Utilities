using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities
{
    public enum PropPartType
    {
        InOut,
        In, // always assigned from outside of the class
        Out // always assigned from inside the class
    }
}
