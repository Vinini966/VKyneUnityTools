using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;

namespace VkyneTools.Extensions
{
    static class EnumerableExtensions
    {
        static public T GetRandom<T>(this IEnumerable<T> list)
        {
            return list.ElementAt(Random.Range(0, list.Count()));
        }

        static public int GetRandomIndex<T>(this IEnumerable<T> list)
        {
            return Random.Range(0, list.Count());
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }

            foreach (T item in list)
            {
                action(item);
            }
        }

    }
}
