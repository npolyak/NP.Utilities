using System;
using System.Collections.Generic;

namespace NP.Utilities
{
    public class KeyedDisposables<TKey> : IDisposable where TKey : notnull
    {
        private Dictionary<TKey, IDisposable> _dict = new Dictionary<TKey, IDisposable>();

        public void Add(TKey key, IDisposable disposable)
        {
            if (_dict.ContainsKey(key))
            {
                throw new ProgrammingError($"Key '{key}' is already in the KeyedDisposablesContainer");
            }

            _dict[key] = disposable;
        }

        public void Remove(TKey key)
        {
            if (_dict.Remove(key, out IDisposable? d))
            {
                d?.Dispose();
            }
        }

        public void Clear()
        {
            _dict.Keys.DoForEach(k => this.Remove(k));
        }


        public void Dispose()
        {
            Clear();
        }
    }
}
