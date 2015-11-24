using System;
using System.Collections.Generic;

namespace EntropySourceTesting
{

    public class ShufflingTest
    {
        private static Random randGen = new Random();

        public static readonly int subsetNumber = 10;

        private int subsetLength;

        private int[][] subsets;

        ShufflingTest(int[] dataset)
        {
            subsetLength = dataset.Length / subsetNumber;
            subsets = new int[subsetNumber][];
            int shift = 0;
            for (int currentSubset = 0; currentSubset < subsetNumber; ++currentSubset)
            {
                subsets[currentSubset] = new int[subsetLength];
                for (int position = 0; position < subsetLength; ++position)
                {
                    subsets[currentSubset][position] = dataset[shift + position];
                }
                shift += subsetLength;
            }
        }

        /// <summary>
        /// shuffles the array
        /// </summary>
        /// <typeparam name="T">Array element type</typeparam>
        /// <param name="array">Array to shuffle</param>
        public static void shuffle<T>(T[] array)
        {
            for (int currentPosition = array.Length - 1; currentPosition >= 0; --currentPosition)
            {
                // NextDouble returns a random number between 0 and 1
                int newPosition = (int)(randGen.NextDouble() * (currentPosition + 1));
                T tempElement = array[newPosition];
                array[newPosition] = array[currentPosition];
                array[currentPosition] = tempElement;
            }
        }

        public bool runTest()
        {
            //Lists of computed ranks for each data subset for each test

            List<int[]> ranksCompressionList = new List<int[]>(subsetNumber);
            List<int[]> ranksRunsList = new List<int[]>(subsetNumber);
            List<int[]> ranksExcursionList = new List<int[]>(subsetNumber);
            List<int[]> ranksDirectionalRunsList = new List<int[]>(subsetNumber);
            List<int[]> ranksCovarianceList = new List<int[]>(subsetNumber);
            List<int[]> ranksCollisionList = new List<int[]>(subsetNumber);

            foreach (int[] subset in subsets)
            {
                int[] sCompression = StatisticalScores.compressionScores(subset);
                int[] sRuns = StatisticalScores.runsScores(subset);
                int[] sExcursion = StatisticalScores.excursionScores(subset);
                int[] sDirectionalRuns = StatisticalScores.directionalRunsScores(subset);
                int[] sCovariance = StatisticalScores.covarianceScores(subset);
                int[] sCollision = StatisticalScores.collisionScores(subset);
                                
                //shuffling

                //Shuffled subsets scores lists

                List<int[]> shufCompressionList = new List<int[]>(1000);
                List<int[]> shufRunsList = new List<int[]>(1000);
                List<int[]> shufExcursionList = new List<int[]>(1000);
                List<int[]> shufDirectionalRunsList = new List<int[]>(1000);
                List<int[]> shufCovarianceList = new List<int[]>(1000);
                List<int[]> shufCollisionList = new List<int[]>(1000);
                for (int counter = 0; counter < 1000; counter++)
                {
                    shuffle<int>(subset);
                    shufCompressionList.Add(StatisticalScores.compressionScores(subset));
                    shufRunsList.Add(StatisticalScores.runsScores(subset));
                    shufExcursionList.Add(StatisticalScores.excursionScores(subset));
                    shufDirectionalRunsList.Add(StatisticalScores.directionalRunsScores(subset));
                    shufCovarianceList.Add(StatisticalScores.covarianceScores(subset));
                    shufCollisionList.Add(StatisticalScores.collisionScores(subset));
                }

                int[] ranksCompression = getRanks(shufCompressionList, sCompression);
                int[] ranksRuns = getRanks(shufRunsList, sRuns);
                int[] ranksExcursion = getRanks(shufExcursionList, sExcursion);
                int[] ranksDirectionalRuns = getRanks(shufDirectionalRunsList, sDirectionalRuns);
                int[] ranksCovariance = getRanks(shufCovarianceList, sCovariance);
                int[] ranksCollision = getRanks(shufCollisionList, sCollision);

                //print ranks (optional)
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("========== new dataset ==========");
                Console.ResetColor();
                printScores(ranksCompression, "Ranks for Compression test");                
                printScores(ranksRuns, "Ranks for Runs test");
                printScores(ranksExcursion, "Ranks for Excursion test");
                printScores(ranksDirectionalRuns, "Ranks for DirectionalRuns test");
                printScores(ranksCovariance, "Ranks for Covariance test");
                printScores(ranksCollision, "Ranks for Collision test");

                ranksCompressionList.Add(ranksCompression);
                ranksRunsList.Add(ranksRuns);
                ranksExcursionList.Add(ranksExcursion);
                ranksDirectionalRunsList.Add(ranksDirectionalRuns);
                ranksCovarianceList.Add(ranksCovariance);
                ranksCollisionList.Add(ranksCollision);                
            }

            //results interpretation
            //bool compression = testResult(ranksCompressionList);
            //bool runs = testResult(ranksRunsList);
            //bool excursion = testResult(ranksExcursionList);
            //bool directional = testResult(ranksDirectionalRunsList);
            //bool covariance = testResult(ranksCovarianceList);
            //bool collision = testResult(ranksCollisionList);

            //results with names
            bool compression = testResult(ranksCompressionList, "Compression");
            bool runs = testResult(ranksRunsList, "Runs");
            bool excursion = testResult(ranksExcursionList, "Excursion");
            bool directional = testResult(ranksDirectionalRunsList, "Directional runs");
            bool covariance = testResult(ranksCovarianceList, "Covariance");
            bool collision = testResult(ranksCollisionList, "Collsion");

            //comment this section if extended resuls is out of need
            #region extended results
            extendedResult(compression, "Compression");
            extendedResult(runs, "Over/Under Runs");
            extendedResult(excursion, "Excursion");
            extendedResult(directional, "Directional runs");
            extendedResult(covariance, "Covariance");
            extendedResult(collision, "Collision");
            #endregion

            return (compression && runs && excursion && directional && covariance && collision);
        }

        private static void extendedResult(bool test, string testname)
        {
            if (test)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(testname+" test has passed");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(testname + " test has failed");
                Console.ResetColor();
            }
        }

        private static bool testResult(List<int[]> ranksList)
        {
            int count = 0;
            foreach (int[] ranks in ranksList)
            {
                //printScores(ranks, "test ranks");
                if (testFailed(ranks))
                    count++;
            }
            Console.WriteLine("Number of failed subsets in executed test " + count);
            if (count >= 8)
            {
                //Console.WriteLine("test failed");
                return false;
            }
            else
            {
                //Console.WriteLine("test passed");
                return true;
            }
        }
        private static bool testResult(List<int[]> ranksList, string message)
        {
            int count = 0;
            foreach (int[] ranks in ranksList)
            {
                //printScores(ranks, "test ranks");
                if (testFailed(ranks))
                    count++;
            }
            Console.WriteLine("Number of failed subsets in "+message+" test " + count);
            if (count >= 8)
            {
                //Console.WriteLine("test failed");
                return false;
            }
            else
            {
                //Console.WriteLine("test passed");
                return true;
            }
        }

        private static bool testFailed(int[] ranks)
        {
            return Array.TrueForAll<int>(ranks, delegate (int rank) { return (rank <= 50 || rank >= 950); });
        }

        private static int[] getRanks(List<int[]> shufScoresList, int[] sTest)
        {
            int[] ranks = new int[shufScoresList[0].Length];
            for (int j = 0; j < shufScoresList[0].Length; j++)
            {
                List<int> tmpList = new List<int>();
                foreach (int[] result in shufScoresList)
                {
                    tmpList.Add(result[j]);
                }
                tmpList.Sort();
                int medianValue = tmpList[500];
                int score = sTest[j];
                int rank = -2;
                if (medianValue > score)
                {
                    rank = tmpList.FindLastIndex(500, delegate (int scr) { return scr <= score; });
                    if (rank == -1)
                        rank = 0;
                }
                if (medianValue == score)
                    rank = 500;
                if (medianValue < score)
                {
                    rank = tmpList.FindIndex(500, delegate (int scr) { return scr >= score; });
                    if (rank == -1)
                        rank = 1000;
                }
                ranks[j] = rank;
            }
            return ranks;
        }
       
        private static void printScores(int[] scores, string testname)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(testname);
            Console.ResetColor();
            foreach (int i in scores)
                Console.Write(i + " ");
            Console.WriteLine();
        }

        public static void Main()
        {
            //main part

            Console.Title = "Entropy source shuffling test";
            Console.WriteLine("Working...");
            int[] ds = new int[10000];
            Random r = new Random();
            for (int i = 0; i < ds.Length; i++)
            {
                ds[i] = r.Next(255);
            }
            ShufflingTest st = new ShufflingTest(ds);
            bool result = st.runTest();
            Console.WriteLine();
            extendedResult(result, "Shuffling test");

            //test
            //int[] ds = new int[10000];
            //Random r = new Random();
            //for (int i = 0; i < ds.Length; i++)
            //{
            //    ds[i] = r.Next(255);
            //}
            //StatisticalScores.compressionScores(ds);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
