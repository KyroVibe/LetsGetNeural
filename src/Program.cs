using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.IO;
using LetsGetNeural.Data;

namespace LetsGetNeural
{
    class Program
    {
        public static int[] SETTINGS = new int[] { 512, 16, 16, 192 };

        static void Main(string[] args)
        {
            Console.WriteLine("--===[ n30b4rt's Really Stupid Brain ]===--\n");
            Console.Write('>');
            string s = Console.ReadLine().ToLower();
            if (s.Equals("load"))
            {
                Console.WriteLine("Nothing rn sorry");
            }
            else if (s.Equals("physics"))
            {
                PhysicsTraining();
            }
            else if (s.Equals("handwriting"))
            {
                HandWriting();
            }
            else if (s.Equals("evolution"))
            {
                Evolution();
            }
        }

        static void PhysicsTraining()
        {
            List<(double[] input, double[] output)> data = new List<(double[] input, double[] output)>();
            int dataCount = 10;
            int cycles = 50000;
            PhysicsDataPoint pt;
            for (int i = 0; i < dataCount; i++) {
                pt = new PhysicsDataPoint();
                data.Add(pt.ToTuple());
            }
            Console.WriteLine(dataCount + " points of data");
            Console.WriteLine(cycles + " cycles");
            Console.WriteLine(((dataCount * cycles) / 1000000.0) + " M tests");
            BackNetTrainer trainer = new BackNetTrainer(SETTINGS, data.ToArray());
            BackNet net = trainer.Train(cycles);
            Console.WriteLine("Training finished");
            Console.WriteLine("Ending Error: " + net.Error);
            Console.ReadKey();
        }

        static void HandWriting()
        {
            var data = GetTestData("..\\..\\..\\TestingData.txt");
            double a = data.Length;
            double b = 200;
            Console.WriteLine(a + " points of data");
            BackNetTrainer trainer = new BackNetTrainer(SETTINGS, data);
            Console.WriteLine(b + " cycles");
            Console.WriteLine(((a * b) / 1000000.0) + " M tests");
            BackNet net = trainer.Train((int)b);
            Console.WriteLine("Saving Net");
            net.Save("Net.nn");
            double[] error = net.layers[net.layers.Length - 1].error;
            for (int i = 0; i < error.Length; i++)
            {
                Console.WriteLine(i + ": " + error[i]);
            }
            Console.ReadKey();
        }

        static void Evolution()
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
        }

        static (double[] input, double[] output)[] GetTestData(string file)
        {
            List<(double[] input, double[] output)> data = new List<(double[] input, double[] output)>();
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string a = sr.ReadToEnd();
            string[] lines = a.Split("\n");
            string[] cappedLines = new string[10000];
            for (int i = 0; i < cappedLines.Length; i++) cappedLines[i] = lines[i];
            Array.ForEach(cappedLines, x => data.Add(new NumberDat(x).GetTuple()));
            return data.ToArray();
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

    public class NumberDat
    {
        public double[] input { get; private set; }
        public double[] output { get; private set; }

        public NumberDat(string s)
        {
            input = new double[784];
            output = new double[10];

            string[] split = s.Split('@');

            int index = int.Parse(split[0]);
            for (int i = 0; i < output.Length; i++)
            {
                if (i == index) output[i] = 1.0;
                else output[i] = 0.0;
            }
            for (int x = 0; x < input.Length; x++)
            {
                input[x] = double.Parse(split[x + 1]);
            }
        }

        public (double[] input, double[] output) GetTuple()
        {
            return (input, output);
        }
    }
}
