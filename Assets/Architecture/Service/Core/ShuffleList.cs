
/*
Description: Utility class used to shuffle a List
*/

using System.Collections.Generic;
using Random = System.Random;

namespace Service.Core.Utilities
{
    public class ShuffleList
    {
        public static void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}