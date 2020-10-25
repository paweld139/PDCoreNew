﻿using PDCore.Utils;
using System;

namespace PDCore.Helpers.Calculation
{
    public static class RandomNumberGenerator
    {
        //private static readonly Lazy<Random> random = new Lazy<Random>();
        //public static Random Random => random.Value;
        private static readonly Random random;

        /// <summary>
        /// Modyfikator dostępu jest niedozwolony dla statycznych konstruktorów
        /// </summary>
        static RandomNumberGenerator()
        {
            random = new Random();
        }

        public static int[] Next(int from, int to, int instances)
        {
            if (instances <= 0)
                throw new ArgumentOutOfRangeException(ReflectionUtils.GetNameOf(() => instances), instances, "Wartość \"od\" musi być większa od zera");

            if (from > to)
                throw new ArgumentOutOfRangeException(ReflectionUtils.GetNameOf(() => from), from, "Wartość \"od\" nie może być większa od wartości \"do\"");

            int[] result = new int[instances];

            for (int i = 0; i < instances; i++)
            {
                result[i] = random.Next(from, to + 1);
            }

            return result;

            //return Enumerable.Range(0, instances).ToArray(x => random.Next(from, to + 1));
        }
    }
}
