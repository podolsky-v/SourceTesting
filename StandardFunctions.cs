using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntropySourceTesting
{
    /// <summary>
    /// introduces standard statistical functions
    /// </summary>
    public static class StandardFunctions
    {
        private static Type[] numericTypes = {
                                  typeof(int),
                                  typeof(long),
                                  typeof(double)
                              };

        #region statistical functions for datasets with integer values
        /// <summary>
        /// calculates average value
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>average value</returns>
        public static double average(IEnumerable<int> dataset) {
            return expectation(dataset, x => x);
        }

        /// <summary>
        /// calculates observed expectation of a function
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static double expectation(IEnumerable<int> dataset, Func<int, int> function) {
            long integerPart = 0;
            long remainder = 0;
            int counter = 0;
            foreach(int value in dataset) {
                ++counter;
                long result = function(value);
                long tempRemainder = (result - integerPart + remainder) % counter;
                integerPart += (result - integerPart + remainder) / counter;
                remainder = tempRemainder;
            }
            return Utilities.convert(integerPart, remainder, counter);
        }

        /// <summary>
        /// calculates n-th order central moment
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="order">moment's order</param>
        /// <returns>n-th order central moment</returns>
        public static double centralMoment(IEnumerable<int> dataset, byte order) {
            return expectation(dataset, x => Utilities.pow(x, order));
        }

        /// <summary>
        /// calculates variance
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>variance</returns>
        public static double variance(IEnumerable<int> dataset) {
            return variance(dataset, average(dataset));
        }

        /// <summary>
        /// calculates variance knowing the mean value
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="mean">mean value</param>
        /// <returns>variance</returns>
        public static double variance(IEnumerable<int> dataset, double mean) {
            int length = dataset.Count();
            return (expectation(dataset, x => x * x) - mean * mean) * length / (length - 1);
        }         

        /// <summary>
        /// calculates standard deviation
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>standard deviation</returns>
        public static double standardDeviation(IEnumerable<int> dataset) {
            return standardDeviation(dataset, average(dataset));
        }

        /// <summary>
        /// calculates standard deviation knowing the mean value
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="mean">mean value</param>
        /// <returns>standard deviation</returns>
        public static double standardDeviation(IEnumerable<int> dataset, double mean) {
            return Math.Sqrt(variance(dataset, mean));
        }

        /// <summary>
        /// estimates confidence interval
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>lower and upper bounds of the confidence interval</returns>
        public static double[] confidenceInterval(IEnumerable<int> dataset) {
            double mean = StandardFunctions.average(dataset);
            double deviation = StandardFunctions.standardDeviation(dataset, mean);
            deviation = 1.96 * deviation / Math.Sqrt(dataset.Count());
            return new double[] { mean - deviation, mean + deviation };
        }
        #endregion

        #region statistical functions for datasets with floating point values
        public static double expectation(IEnumerable<double> dataset, 
                Func<double, double> function) {
            double expectationValue = .0;
            foreach(int value in dataset) {
                expectationValue += function(value);
            }
            return expectationValue / dataset.Count();
        }

        public static double average(IEnumerable<double> dataset) {
            return dataset.Average();
        }

        public static double variance(IEnumerable<dynamic> dataset, double mean) {
            return average((dynamic)dataset);
        }
        #endregion

        /// <summary>
        /// calculates accurate average value rounded down to integer
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>average value rounded down to integer</returns>
        public static int integerAverage(IEnumerable<int> dataset) {
            int remainder;
            return integerAverage(dataset, out remainder);
        }

        /// <summary>
        /// calculates accurate average value rounded down to integer and remainder
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="remainder"></param>
        /// <returns>average value rounded down to integer</returns>
        public static int integerAverage(IEnumerable<int> dataset, out int remainder) {            
            return integerExpectation(dataset, x => x, out remainder);
        }

        public static int integerExpectation(IEnumerable<int> dataset, 
                Func<int, int> function, out int remainder) {
            int integerPart = 0;
            remainder = 0;
            int counter = 0;
            foreach(int value in dataset) {
                ++counter;
                int result = function(value);
                int tempRemainder = (result - integerPart + remainder) % counter;
                integerPart += (result - integerPart + remainder) / counter;
                remainder = tempRemainder;
            }
            return integerPart;
        }

        public static int integerExpectation(IEnumerable<int> dataset, 
                Func<int, int> function) {
            int remainder;
            return integerExpectation(dataset, function, out remainder);
        }

        /// <summary>
        /// calculates integer bounds of median
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>median rounded down and up to integers</returns>
        public static int[] integerMedian(int[] dataset) {
            int[] sorteddataset = (int[])dataset.Clone();
            Array.Sort(sorteddataset);
            int[] median = new int[2];
            int centralPosition = sorteddataset.Length / 2;            
            ///////////////////////////////////////////////////////////////////////////////////////
            int currentPosition = centralPosition;
            while(sorteddataset[currentPosition] == sorteddataset[sorteddataset.Length - 1] 
                    && currentPosition > 0) {
                --currentPosition;
            }
            median[0] = sorteddataset[currentPosition];
            ///////////////////////////////////////////////////////////////////////////////////////
            currentPosition = centralPosition + (sorteddataset.Length - 1) % 2;
            while(sorteddataset[currentPosition] == sorteddataset[0] 
                    && currentPosition < sorteddataset.Length - 1) {
                ++currentPosition;
            }
            median[1] = sorteddataset[currentPosition];
            return median;
        }

        /// <summary>
        /// searches the next collision in the dataset starting from the defined position
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="startPosition"></param>
        /// <returns>position of the next collision</returns>
        public static int nextCollision(int[] dataset, int startPosition) {
            var values = new HashSet<int>();
            int currentPosition = startPosition;
            while(currentPosition < dataset.Length) {
                if(values.Contains(dataset[currentPosition])){
                    //the next collision is found
                    return currentPosition;
                } else {
                    values.Add(dataset[currentPosition]);
                }
                ++currentPosition;
            }
            //no collision is found
            return -1;
        }

        public static int[] allCollisions(int[] dataset) {
            var collisions = new List<int>();
            int currentPosition = 0;
            int nextPosition;
            while((nextPosition = nextCollision(dataset, currentPosition)) != -1) {
                //number of datasets before the collision including the collision
                collisions.Add(nextPosition - currentPosition + 1);
                currentPosition = nextPosition + 1;
            }
            return collisions.ToArray();
        }
    }
}
