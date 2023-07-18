using System.Collections.Generic;

namespace GameFramework
{
    public static class CollectionUtils
    {
        public static T Pop<T>(this IList<T> list)
        {
            if (list.Count <= 0)
            {
                return default;
            }

            int lastIndex = list.Count - 1;
            T item = list[lastIndex];
            list.RemoveAt(lastIndex);
            return item;
        }

        public static void AddRange<T>(this ICollection<T> collection, ICollection<T> targets)
        {
            foreach (var target in targets)
            {
                collection.Add(target);
            }
        }
    }
}