using System.Runtime.CompilerServices;

namespace Superpower.Util
{
    static class ArrayEnumerable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Cons<T>(T first, T[] rest)
        {
            var all = new T[rest.Length + 1];
            all[0] = first;
            for (var i = 0; i < rest.Length; ++i)
                all[i + 1] = rest[i];
            return all;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Concat<T>(T[] first, T[] rest)
        {
            var all = new T[first.Length + rest.Length];
            var i = 0;
            for (; i < first.Length; ++i)
                all[i] = first[i];
            for (var j = 0; j < rest.Length; ++i, ++j)
                all[i] = rest[j];
            return all;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Append<T>(T[] first, T last)
        {
            var all = new T[first.Length + 1];
            for (var i = 0; i < first.Length; ++i)
                all[i] = first[i];
            all[first.Length] = last;
            return all;
        }
    }
}
