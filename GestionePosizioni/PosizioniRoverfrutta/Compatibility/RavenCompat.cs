using System.Collections.Generic;

namespace Raven.Abstractions.Extensions
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }
    }
}

namespace Raven.Client.Document
{
    public enum ConsistencyOptions
    {
        AlwaysWaitForNonStaleResultsAsOfLastWrite
    }
}
