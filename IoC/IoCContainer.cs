using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.IoC
{
    public interface IIocContainer
    {
        void Register<T>(Func<T> getter);
        bool Unregister<T>();

        T Get<T>();

        //event EventHandler<ResolveGetEventArgs> ResolveGet;
    }
}
