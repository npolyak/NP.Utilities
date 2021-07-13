// (c) Nick Polyak 2018 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
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
