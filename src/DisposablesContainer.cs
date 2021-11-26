using System;
using System.Collections.Generic;

namespace NP.Utilities
{
    public class DisposablesContainer : IDisposable
    {
        private List<IDisposable> _disposables = new List<IDisposable>();

        public DisposablesContainer(params IDisposable[] disposables)
        {
            _disposables.AddRange(disposables);
        }

        public void Dispose()
        {
            _disposables.DoForEach(d => d.Dispose());
                
            _disposables.Clear();
        }
    }
}
