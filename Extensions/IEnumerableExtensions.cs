namespace AOC_2022.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T[][] Transpose<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
            enumerable.SelectMany(x => x.Select((c, i) => Tuple.Create(c, i)))
                .GroupBy(x => x.Item2)
                .OrderBy(x => x.Key)
                .Select(x => x.Select(y => y.Item1).ToArray())
                .ToArray();

        public static T[][] ReverseElements<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
            enumerable.Select(x => x.Reverse().ToArray()).ToArray();

        public static void AddSorted<T>(this List<T> list, T item) where T : IComparable<T>
        {
            var index = list.BinarySearch(item);
            if (index < 0) list.Insert(~index, item);
            else list.Insert(index, item);
        }

        public static IEnumerable<T> ConcatSingle<T>(this IEnumerable<T> collection, T item)
            => collection.Concat(new[] { item });
    }
}
