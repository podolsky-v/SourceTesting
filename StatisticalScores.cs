using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpZipLib.BZip2;

namespace EntropySourceTesting
{
    /// <summary>
    /// introduces statistical scores used in IID tests
    /// </summary>
    public static class StatisticalScores
    {
        /// <summary>
        /// calculates compression score
        /// </summary>
        /// <param name="sample">sample to analyse</param>
        /// <returns>length of the compressed string in bytes</returns>
        public static int[] compressionScores(int[] sample)
        {
            string str = string.Join(",", sample);
            //Console.WriteLine(str);
            int res = -1;
            using (MemoryStream inp = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                using (MemoryStream compressed = new MemoryStream())
                {
                    //Console.WriteLine("uncompressed length = " + inp.Length);
                    try
                    {
                        BZip2.Compress(inp, compressed, false, 4096);
                        res = (int)compressed.Length;
                        //Console.WriteLine("compressed length = " + res);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return new int[] { res };
        }

        /// <summary>
        /// calculates Over/Under Runs Score
        /// </summary>
        /// <param name="sample">sample to analyse</param>
        /// <returns>longest run and total number of runs</returns>
        public static int[] runsScores(int[] sample) {
            int[] median = StandardFunctions.integerMedian(sample);
            //false: value is under the median
            //true: value is over the median
            //skipped: value is equal to the median
            List<bool> symbols = new List<bool>();
            for(int i = 0; i < sample.Length; ++i) {
                if(sample[i] < median[1]) {
                    symbols.Add(false);
                } else if(sample[i] > median[0]) {
                    symbols.Add(true);
                }
            }
            //first symbol produces first run of length 1
            int currentRunLength = 1;
            bool currentSymbol = symbols[0];
            int longestRunLength = 1;
            int runsNumber = 1;
            ///////////////////////////////////////////////////////////////////////////////////////
            for(int i = 1; i < symbols.Count; ++i) {
                if(symbols[i] != currentSymbol) {
                    //the next run
                    if(currentRunLength > longestRunLength) {
                        longestRunLength = currentRunLength;
                    }
                    currentRunLength = 1;
                    ++runsNumber;                    
                } else {
                    //the same run
                    ++currentRunLength;
                }
            }
            if(currentRunLength > longestRunLength) {
                longestRunLength = currentRunLength;
            }
            return new int[] { longestRunLength, runsNumber };
        }

        /// <summary>
        /// calculates excursion score
        /// </summary>
        /// <param name="sample">sample to analyse</param>
        /// <returns>maximal deviation from expected value</returns>
        public static int[] excursionScores(int[] sample) {
            int initRemainder;
            int initAverage = StandardFunctions.integerAverage(sample, out initRemainder);
            decimal average = initAverage + initRemainder * 100 / sample.Length;
            decimal excursion = 0;
            decimal maxAbsValue = 0;
            for(int i = 0; i < sample.Length; ++i) {
                excursion += sample[i] - average;
                if(Math.Abs(excursion) > maxAbsValue) {
                    maxAbsValue = Math.Abs(excursion);
                }
            }
            return new int[]{(int)maxAbsValue};
        }

        /// <summary>
        /// calculates directional runs score
        /// </summary>
        /// <param name="sample">sample to analyse</param>
        /// <returns>longest run, total number of runs, total number of 1 or -1 (which greater)</returns>
        public static int[] directionalRunsScores(int[] sample) {
            //creation of derivatives array
            sbyte[] derivatives = new sbyte[sample.Length - 1];
            int numberOfOnes = 0;
            int numberOfMinusOnes = 0;
            for(int i = 0; i < derivatives.Length - 1; ++i) {
                if(sample[i] < sample[i + 1]) {
                    derivatives[i] = 1;
                    ++numberOfOnes;
                } else if(sample[i] > sample[i + 1]) {
                    derivatives[i] = -1;
                    ++numberOfMinusOnes;
                } else {
                    derivatives[i] = 0;
                }
            }
            //skipping leading 0
            int currentPosition = 0;
            while(derivatives[currentPosition] == 0 && currentPosition < derivatives.Length - 1) {
                ++currentPosition;
            }
            if(derivatives[currentPosition] == 0) {
                return new int[] { 0, 0, 0 };
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            int currentValue = derivatives[currentPosition];
            ++currentPosition;
            int runsNumber = 1;
            int currentRunLength = 1;
            int longestRunLength = 1;
            for(; currentPosition < derivatives.Length; ++currentPosition) {
                if(derivatives[currentPosition] == -currentValue) {
                    //the next run (zeros do not break the run)
                    ++runsNumber;
                    if(currentRunLength > longestRunLength) {
                        longestRunLength = currentRunLength;
                    }
                    currentRunLength = 1;
                } else {
                    //the same run
                    ++currentRunLength;
                }
            }
            if(currentRunLength > longestRunLength) {
                longestRunLength = currentRunLength;
            }
            return new int[] { 
                longestRunLength, runsNumber, Math.Max(numberOfOnes, numberOfMinusOnes) 
            };
        }        

        /// <summary>
        /// calculates covariance score
        /// </summary>
        /// <param name="sample">sample to analyse</param>
        /// <returns>covariance estimate</returns>
        public static int[] covarianceScores(int[] sample) {
            int average = StandardFunctions.integerAverage(sample);
            int[] counts = new int[sample.Length - 1];
            for(int i = 0; i < counts.Length; ++i) {
                counts[i] = (sample[i] - average) * (sample[i + 1] - average);
            }
            return new int[] { StandardFunctions.integerAverage(counts) };
        }

        /// <summary>
        /// calculates collision score
        /// </summary>
        /// <param name="sample">sample to analyse</param>
        /// <returns>number of values to collision: minimal, maximal and average</returns>
        public static int[] collisionScores(int[] sample) {
            int[] collisions = StandardFunctions.allCollisions(sample);
            return new int[]{collisions.Min(), collisions.Max(), 
                    StandardFunctions.integerAverage(collisions.ToArray())
            };
        }

        /// <summary>
        /// calculates compression score
        /// </summary>
        /// <param name="sample">sample to analyse</param>
        /// <returns>length of the compressed string in bytes</returns>
        public static int[] compressionScores(BitArray sample)
        {
            char[] symbols = new char[sample.Count];
            for (int i = 0; i < sample.Count; i++)
            {
                symbols[i] = sample[i] ? '1' : '0';
            }
            string str = string.Join(",", symbols);
            //Console.WriteLine(str);
            int res = -1;
            using (MemoryStream inp = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                using (MemoryStream compressed = new MemoryStream())
                {
                    //Console.WriteLine("uncompressed length = " + inp.Length);
                    try
                    {
                        BZip2.Compress(inp, compressed, false, 4096);
                        res = (int)compressed.Length;
                        //Console.WriteLine("compressed length = " + res);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return new int[] { res };
        }

        /// <summary>
        /// calculates Over/Under Runs Score
        /// </summary>
        /// <param name="sample">sample to analyse</param>
        /// <returns>longest run and total number of runs</returns>
        public static int[] runsScores(BitArray sample)
        {            
            //median=0.5
            //false: value is under the median
            //true: value is over the median
            int currentRunLength = 1;
            bool currentSymbol = sample[0];
            int longestRunLength = 1;
            int runsNumber = 1;
            ///////////////////////////////////////////////////////////////////////////////////////
            for (int i = 1; i < sample.Count; ++i)
            {
                if (sample[i] != currentSymbol)
                {
                    //the next run
                    if (currentRunLength > longestRunLength)
                    {
                        longestRunLength = currentRunLength;
                    }
                    currentRunLength = 1;
                    ++runsNumber;
                }
                else
                {
                    //the same run
                    ++currentRunLength;
                }
            }
            if (currentRunLength > longestRunLength)
            {
                longestRunLength = currentRunLength;
            }
            return new int[] { longestRunLength, runsNumber };
        }
    }
}
