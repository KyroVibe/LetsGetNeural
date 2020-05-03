using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

namespace LetsGetNeural
{
    class Program
    {
        public static int[] SETTINGS = new int[] { sizeof(double) * 8, 25, 25, sizeof(double) * 8 };

        static void Main(string[] args)
        {
            Console.WriteLine("5000 points of data");
            var data = GenTestData(5000);
            BackNetTrainer trainer = new BackNetTrainer(SETTINGS, data);
            Console.WriteLine("5000 cycles");
            Console.WriteLine("25 M tests");
            BackNet net = trainer.Train(5000);
            Console.WriteLine("Saving Net");
            net.Save("Net.nn");
            Console.ReadKey();
        }

        static void Evolution() {
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
        }

        static (double[] input, double[] output)[] GenTestData(int count)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            List<(double[] input, double[] output)> testData = new List<(double[] input, double[] output)>();
            for (int i = 0; i < count; i++)
            {
                double a = 9.99E+100 * ((rand.NextDouble() * 2) - 1);
                double b = Math.Pow(a, 2);
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
