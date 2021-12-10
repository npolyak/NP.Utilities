using System;

namespace NP.Utilities
{
    public class KeyedDisposable<TKey> : IDisposable
    {
        public TKey Key { get; set; }

        public IDisposable Disposable { get; set; }

        public void Dispose()
        {
            Disposable?.Dispose();
            Disposable = null;
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyedDisposable<TKey> keyedDisposable)
                return this.Key.ObjEquals(keyedDisposable.Key);

            return false;
        }

        public override int GetHashCode()
        {
            return Key?.GetHashCode() ?? 0;
        }
    }
}
