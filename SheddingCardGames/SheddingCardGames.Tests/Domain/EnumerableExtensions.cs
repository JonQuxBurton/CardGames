using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Tests.Domain
{
    public static class EnumerableExtensions
    {
        public static TSource LastSkip<TSource>(this IEnumerable<TSource> source, int numberFromLast)
        {
            return source.Reverse().Skip(numberFromLast).Take(1).FirstOrDefault();
        }
    }
}