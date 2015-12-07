using System;
using System.Collections;

namespace EntropySourceTesting
{
    class MainClass
    {
        public static void Main()
        {
            Console.Title = "Entropy source shuffling test";
            
            ////////////////////////////////////Demonstration for integer samples

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
            ShufflingTest.extendedResult(result, "Shuffling test");

            ////////////////////////////////////Demonstration for binary samples
            Console.WriteLine("Working... (binary)");
            int[] ds2 = new int[10000];
            for (int i = 0; i < ds2.Length; i++)
            {
                ds2[i] = r.Next();
            }

            BitArray bs = new BitArray(ds);
            ShufflingTestBinary stBin = new ShufflingTestBinary(bs);
            bool resultBin = stBin.runTest();
            Console.WriteLine();
            ShufflingTest.extendedResult(resultBin, "Shuffling test");


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
