using System;
using System.Collections.Generic;
using System.Text;

namespace LunarConstruct
{
    public static class Extensions
    {
        public static T GetRandom<T>(this List<T> list, Xoroshiro128Plus rng = null)
        {
            if (list.Count == 0)
            {
                return default(T);
            }
            if (rng == null)
            {
                return list[UnityEngine.Random.RandomRangeInt(0, list.Count)];
            }
            else
            {
                return list[rng.RangeInt(0, list.Count)];
            }
        }

        public static T GetRandom<T>(this T[] array)
        {
            int index = UnityEngine.Random.Range(0, array.Length);
            return array[index];
        }
    }
}