using System.Collections.Generic;
using System.Diagnostics;

namespace PDS_Exam
{
    internal class Program
    {
        public static Random random = new Random();
        public static int amount = 10_000;
        static readonly object locker = new object();
        public static int[] nthreads = { 2, 5 }; //number of threads
        public const int n = 300;
        public static int biggerN = 0;

        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();


            Console.WriteLine("START");

            //generate random array
            int[] array = new int[amount];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = random.Next(1, amount);
            }


            foreach (var t in nthreads)
            {
                biggerN = 0;
                stopwatch.Reset();
                stopwatch.Start();
                var sybarrays = GenerateSubLists(t, array);
                ThreadsWorkBarcodes(t, sybarrays);
                stopwatch.Stop();
                Console.WriteLine($"BIGGER N: {biggerN}\n");
                Console.WriteLine($"Time to process with {t} threads: {stopwatch.ElapsedMilliseconds} miliseconds\n");
                Console.WriteLine("---------------------------------------");
            }

            Console.WriteLine($"FINISHED");
            Console.Read();
        }
        static List<int>[] GenerateSubLists(int numOfthreads, int[] array)
        {
            List<int>[] subLists = new List<int>[numOfthreads];
            var splitFactor = Math.Ceiling(Convert.ToDouble(array.Length) / Convert.ToDouble(numOfthreads));
            for (int i = 0; i < numOfthreads; i++) {
                subLists[i] = new List<int>();
            }

            //add elements until the length of sublist is equal to splitFactor

            for (int i = 0; i < array.Length; i++)
            {
                int j = Convert.ToInt32(Math.Floor(Convert.ToDouble(i) / Convert.ToDouble(splitFactor)));
                var value = array[i];
                subLists[j].Add(value);
            }


            return subLists;

        }





        static void ThreadsWorkBarcodes(int numOfthreads, List<int>[] array)
        {
            //start threads
            var threads = new List<Thread>();

            for (int i = 0; i < numOfthreads; i++)
            {
                var subarray = array[i].ToArray();
                var thread = new Thread(() => FindBiggerN(subarray));
                thread.Start();
                threads.Add(thread);
            }

            //stop threads
            foreach (var thread in threads)
            {
                thread.Join();
            }


        }
        static void FindBiggerN(int[] array)
        {
            lock (locker)
            {
                if (biggerN == 0)
                {
                    for (var i = 0; i < array.Length; i++)
                    {
                        if (array[i] > n)
                        {
                            biggerN = array[i];
                            Console.WriteLine($"Bigger N for thread id {Thread.CurrentThread.ManagedThreadId}: {biggerN}");
                            break;
                        }


                    }
                }
            }      

        }
    }
}