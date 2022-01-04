using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Tests
{
    public static class EnumerableExtensions
    {
        public static TSource FirstSkip<TSource>(this IEnumerable<TSource> source, int numberFromFirst)
        {
            return source.Skip(numberFromFirst).Take(1).FirstOrDefault();
        }

        public static TSource LastSkip<TSource>(this IEnumerable<TSource> source, int numberFromLast)
        {
            return source.Reverse().Skip(numberFromLast).Take(1).FirstOrDefault();
        }
    }
}