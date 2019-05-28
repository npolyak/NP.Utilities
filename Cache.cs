using System;
using System.Collections.Generic;

namespace NP.Utilities
{
    public class Cache<TIn, TOut>
    {
        public Dictionary<TIn, TOut> _cache = new Dictionary<TIn, TOut>();

        public Func<TIn, TOut> FactoryMethod { get; }

        public Cache(Func<TIn, TOut> factoryMethod)
        {
            FactoryMethod = factoryMethod;
        }

        public TOut Get(TIn input)
        {
            if (!_cache.TryGetValue(input, out TOut val))
            {
                val = FactoryMethod(input);

                _cache[input] = val;
            }

            return val;
        }
    }
}
