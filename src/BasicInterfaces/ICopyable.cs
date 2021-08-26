// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using System;
using System.Collections.Generic;

namespace NP.Utilities.BasicInterfaces
{
    public interface ICopyable<T>
        where T : ICopyable<T>
    {
        void CopyFrom(T source);
    }


    public static class ICopyableExtensions
    {
        public static T Copy<T>(this T source, Type type = null)
            where T : ICopyable<T>
        {
            if (type == null)
                type = source.GetType();

            T result = (T)Activator.CreateInstance(type);

            result.CopyFrom(source);

            return result;
        }

        public static void CopyCollection<T>(this IList<T> targetCollection, IEnumerable<T> sourceCollection)
            where T : ICopyable<T>
        {
            targetCollection.Clear();

            if (sourceCollection == null)
                return;

            foreach (T sourceItem in sourceCollection)
            {
                T targetItem = sourceItem.Copy();

                targetCollection.Add(targetItem);
            }
        }
    }
}
