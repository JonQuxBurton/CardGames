using System;
using System.Collections.Generic;

namespace CardGamesDomain
{
    public class RandomShuffler : IShuffler
    {
        private readonly Random random;

        public RandomShuffler()
        {
            random = new Random();
        }

        public IEnumerable<T> Shuffle<T>(IEnumerable<T> list)
        {
            var listCopy = new List<T>(list);

            var n = listCopy.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                var value = listCopy[k];
                listCopy[k] = listCopy[n];
                listCopy[n] = value;
            }


            return listCopy;
        }
    }
}