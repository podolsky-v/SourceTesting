﻿using System;
using System.Collections;

namespace EntropySourceTesting
{
    /// <summary>
    /// introduces useful simple utilities
    /// </summary>
    public static class Utilities
    {
        public static double convert(long integerPart, long remainder, long denominator) {
            return integerPart + (double) remainder / denominator;
        }

        /// <summary>
        /// Returns the specified number raised to the specified power
        /// </summary>
        /// <param name="x">number to be raised to a power</param>
        /// <param name="y">number that specifies the power</param>
        /// <returns>The specified number raised to the specified power</returns>
        public static int pow(int x, byte y) {
            int result = 1;
            while(y > 0) {
                if(y % 2 == 1) {
                    result *= x;
                }
                x *= x;
                y >>= 1;
            }
            return result;
        }

        /// <summary>
        /// Returns the specified number raised to the specified power
        /// </summary>
        /// <param name="x">number to be raised to a power</param>
        /// <param name="y">number that specifies the power</param>
        /// <returns>The specified number raised to the specified power</returns>
        public static double pow(double x, byte y) {
            return Math.Pow(x, y);
        }

        public static int ones(BitArray sample)
        {
            int ones = 0;
            for (int i = 0; i < sample.Count; ++i)
            {
                if (sample[i] == true)
                    ones++;
            }
            return ones;
        }
        public static decimal bitAverage(BitArray sample)
        {
            return (decimal)ones(sample) / sample.Count;
        }
    }
}
