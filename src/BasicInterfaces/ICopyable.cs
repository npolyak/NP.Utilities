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
