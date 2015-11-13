using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void runTest()
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
                //int[] sCompression = StatisticalScores.compressionScores(subset);
                int[] sRuns = StatisticalScores.runsScores(subset);
                int[] sExcursion = StatisticalScores.excursionScores(subset);
                int[] sDirectionalRuns = StatisticalScores.directionalRunsScores(subset);
                int[] sCovariance = StatisticalScores.covarianceScores(subset);
                int[] sCollision = StatisticalScores.collisionScores(subset);

                //probably it is not needed

                //sCompressionList.Add(sCompression);
                //sRunsList.Add(sRuns);
                //sExcursionList.Add(sExcursion);
                //sDirectionalRunsList.Add(sDirectionalRuns);
                //sCovarianceList.Add(sCovariance);
                //sCollisionList.Add(sCollision);

                //shuffling

                //Shuffled subsets scores lists

                List<int[]> shufCompressionList = new List<int[]>(subsetNumber);
                List<int[]> shufRunsList = new List<int[]>(1000);
                List<int[]> shufExcursionList = new List<int[]>(1000);
                List<int[]> shufDirectionalRunsList = new List<int[]>(1000);
                List<int[]> shufCovarianceList = new List<int[]>(1000);
                List<int[]> shufCollisionList = new List<int[]>(1000);
                for (int counter = 0; counter < 1000; counter++)
                {
                    shuffle<int>(subset);
                    //shufCompressionList.Add(StatisticalScores.compressionScores(subset));
                    shufRunsList.Add(StatisticalScores.runsScores(subset));
                    shufExcursionList.Add(StatisticalScores.excursionScores(subset));
                    shufDirectionalRunsList.Add(StatisticalScores.directionalRunsScores(subset));
                    shufCovarianceList.Add(StatisticalScores.covarianceScores(subset));
                    shufCollisionList.Add(StatisticalScores.collisionScores(subset));
                }

                //int[] ranksCompression = getRanks(shufCompressionList, sCompresion);
                int[] ranksRuns = getRanks(shufRunsList, sRuns);
                int[] ranksExcursion = getRanks(shufExcursionList, sExcursion);
                int[] ranksDirectionalRuns = getRanks(shufDirectionalRunsList, sDirectionalRuns);
                int[] ranksCovariance = getRanks(shufCovarianceList, sCovariance);
                int[] ranksCollision = getRanks(shufCollisionList, sCollision);

                //ranksCompressionList.Add(ranksCompression);
                ranksRunsList.Add(ranksRuns);
                ranksExcursionList.Add(ranksExcursion);
                ranksDirectionalRunsList.Add(ranksDirectionalRuns);
                ranksCovarianceList.Add(ranksCovariance);
                ranksCollisionList.Add(ranksCollision);

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("=========== new dataset ===========");
                printScores(ranksRuns, "runs");
                printScores(ranksExcursion, "excurs");
                printScores(ranksDirectionalRuns, "dir runs");
                printScores(ranksCovariance, "covar");
                printScores(ranksCollision, "collision");

                //for testing
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine("new subset");
                //Console.ResetColor();

                //printScores(sCompression, "compression");      
                //printScores(sRuns, "runs");
                //printScores(sExcursion, "excursion");
                //printScores(sDirectionalRuns, "dir runs");
                //printScores(sCovariance, "covar");
                //printScores(sCollision, "collision");

            }

            //from list to comapare
            //foreach (int[] vector in sRunsList)
            //{
            //    Console.ForegroundColor = ConsoleColor.Yellow;
            //    Console.WriteLine("new subset");
            //    Console.ResetColor();
            //    printScores(vector, "runs");
            //}
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
                int rank = -1;
                if (medianValue > score)
                {
                    rank = tmpList.FindLastIndex(500, delegate (int scr) { return scr <= score; });
                }
                if (medianValue == score)
                    rank = 500;
                if (medianValue < score)
                {
                    rank = tmpList.FindIndex(500, delegate (int scr) { return scr >= score; });
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
            int[] ds = new int[1000];
            Random r = new Random();
            for (int i = 0; i < ds.Length; i++)
            {
                ds[i] = r.Next(255);
            }

            ShufflingTest st = new ShufflingTest(ds);
            st.runTest();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
