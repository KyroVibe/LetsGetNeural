using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

namespace LetsGetNeural
{
    class Program
    {
        public static int[] SETTINGS = new int[] { sizeof(double) * 8, 3, 3, 3, sizeof(double) * 8 };

        static void Main(string[] args)
        {
            var data = GenTestData(500);

            NetTrainer nt = new NetTrainer(1000, SETTINGS, data);
            nt.Train();

            Console.WriteLine("Testing...");

            bool stop = false;

            while (!stop)
            {
                Console.Write(">");
                string a = Console.ReadLine();

                switch (a)
                {
                    case "r":
                        (double best, double worst, int gen) res = nt.stats;
                        int k = nt.kept;
                        Console.WriteLine("\nBest: " + res.best + "\nWorst: " + res.worst + "\nGeneration: " + res.gen + "\nKept after purge: " + k);
                        break;
                    case "s":
                        nt.Halt = true;
                        stop = true;
                        break;
                }
            }

            Console.WriteLine("Waiting for trainer to stop...");
            while (nt.trainingThread.IsAlive) { Thread.Sleep(100); }

            /*
            NeuralNetwork[] n = new NeuralNetwork[1000];
            for (int i = 0; i < n.Length; i++) {
              n[i] = new NeuralNetwork(SETTINGS);
            }

            Array.ForEach(n, x => x.AvgCost(data));
            Array.Sort(n, new NetComparer());

            foreach (NeuralNetwork ne in n) {
              Console.WriteLine(ne.LastTestedCost);
            }
            */

            // var data = GenTestData(500);
            // double c = n.AvgCost(data);
            // Console.WriteLine(c);
        }

        static (double[] input, double[] output)[] GenTestData(int count)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            List<(double[] input, double[] output)> testData = new List<(double[] input, double[] output)>();
            for (int i = 0; i < count; i++)
            {
                double a = rand.Next(-100, 100);
                double b = (2 * a) + 3;
                testData.Add((toBitArray(a), toBitArray(b)));
            }
            return testData.ToArray();
        }

        static double[] toBitArray(double val)
        {
            BitArray b = new BitArray(BitConverter.GetBytes(val));
            double[] a = new double[sizeof(double) * 8];
            for (int i = 0; i < b.Count; i++)
            {
                a[i] = b[i] ? 1 : 0;
            }
            return a;
            // int[] bits = b.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();
        }
    }
}
