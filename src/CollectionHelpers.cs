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
using System.Diagnostics.CodeAnalysis;

namespace NP.Utilities
{
    public class CompFnEqualityComparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> _compFn;

        public CompFnEqualityComparer(Func<T, T, bool> compFn)
        {
            if (compFn == null)
            {
                compFn = (v1, v2) => v1.ObjEquals(v2);
            }

            _compFn = compFn;
        }

        public bool Equals([AllowNull] T x, [AllowNull] T y)
        {
            return _compFn(x, y);
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.GetHashCodeExtension();
        }
    }

    public class FnComparer<T> : IComparer<T>
    {
        Func<T, T, int> _compFn;

        public int Compare([AllowNull] T x, [AllowNull] T y)
        {
            return _compFn(x, y);
        }

        public FnComparer(Func<T, T, int> compFn)
        {
            _compFn = compFn.GetTrivialComparerIfNull();
        }
    }

    public static class CollectionHelpers
    {
        public static Func<T, T, int> GetTrivialComparerIfNull<T>(this Func<T, T, int> compFn)
        {
            if (compFn == null)
            {
                compFn = (v1, v2) =>
                {
                    var c1 = v1 as IComparable;
                    var c2 = v2 as IComparable;

                    if (c1 == null || c2 == null)
                        return 0;

                    return c1.CompareTo(c2);
                };
            }

            return compFn;
        }

        public static Func<T, T, int> InvertComparer<T>(this Func<T, T, int> compFn)
        {
            return (T v1, T v2) => -compFn(v1, v2);
        }

        public static Func<T, T, int> ChooseAndInvertIfNeeded<T>(this Func<T, T, int> compFn, bool ascendingOrDescending)
        {
            compFn = compFn.GetTrivialComparerIfNull();

            if (!ascendingOrDescending)
            {
                compFn = compFn.InvertComparer();
            }

            return compFn;
        }

        public static Func<T, bool> TrivialIfNull<T>(this Func<T, bool> predicate)
        {
            return predicate ?? (v => true);
        }

        public static CompFnEqualityComparer<T> GetEqComparer<T>(this Func<T, T, bool> comparer)
        {
            return new CompFnEqualityComparer<T>(comparer);
        }

        public static FnComparer<T> GetComparer<T>(this Func<T, T, int> compFn)
        {
            return new FnComparer<T>(compFn);
        }
    }
}
