using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace LetsGetNeural
{

    public class NetTrainer
    {

        public bool Halt = false;
        public bool Purging = false;

        public int NetworkCount { get; private set; }
        public TrainingData DataSet { get; private set; }
        public NetSettings Settings { get; private set; }
        private List<NeuralNetwork> networks;
        private Random rand;

        public Thread trainingThread { get; private set; }

        public (double best, double worst, int gen) stats { get; private set; }
        public int kept { get; private set; }

        public NetTrainer(int nc, NetSettings s, TrainingData ds)
        {
            Settings = s;
            DataSet = ds;

            networks = new List<NeuralNetwork>();
            for (int i = 0; i < nc; i++)
            {
                networks.Add(new NeuralNetwork(s));
            }

            rand = new Random(DateTime.Now.Millisecond);

            stats = (100, 100, 0);
            kept = nc;

            NetworkCount = nc;
        }

        public void Train()
        {
            trainingThread = new Thread(() =>
            {
                while (!Halt)
                {
                    // Test and sort networks
                    networks.ForEach(x => x.AvgCost(DataSet));
                    networks.Sort(new NetComparer());

                    // Store results
                    stats = (networks[0].LastTestedCost, networks[networks.Count - 1].LastTestedCost, stats.gen + 1);

                    // Kill shitty ones
                    Purging = true;
                    NeuralNetwork[] temp = networks.ToArray();
                    for (int i = 0; i < temp.Length; i++)
                    {
                        int a = temp.Length - i;
                        /* double check = (a * 2) / temp.Length;
                        double r = rand.NextDouble();
                        if (r < 0.3) r = 0.3;
                        if (check < r)
                        {
                            networks.Remove(temp[i]);
                        }*/
                        if (i > temp.Length / 2)
                        {
                            networks.Remove(temp[i]);
                        }
                    }
                    kept = networks.Count;
                    // Repopulate
                    while (networks.Count < NetworkCount)
                    {
                        // int g = (int)((networks.Count - 1) * Math.Pow(rand.NextDouble(), 2));
                        NeuralNetwork a = networks[(int)((networks.Count - 1) * (1 - Math.Pow(rand.NextDouble(), 2)))];
                        NeuralNetwork b = networks[(int)((networks.Count - 1) * (1 - Math.Pow(rand.NextDouble(), 2)))];
                        NeuralNetwork child = NeuralNetwork.Breed(a, b);
                        networks.Add(child);
                    }
                    networks.ForEach(x => x.Mutate(0.05));
                    Purging = false;

                    // Console.WriteLine("\nBest: " + stats.best + "\nWorst: " + stats.worst
                    //     + "\nGeneration: " + stats.gen + "\nKept after purge: " + kept + "\nTotal: " + networks.Count);
                    //Thread.Sleep(500);
                }

            });
            trainingThread.Start();
        }

    }

}