using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Attributes
{
    public class InAttribute : PartAttribute
    {
        public InAttribute() : base(PropPartType.In)
        {
        }
    }

    public class OutAttribute : PartAttribute
    {
        public OutAttribute() : base(PropPartType.Out)
        {
        }
    }
}
