using System;
using System.Collections.Generic;
using System.Threading;

namespace LetsGetNeural
{

    public class BackNetTrainer
    {

        public NetSettings Settings { get; private set; }
        public TrainingData DataSet { get; private set; }

        public double AdjustMod = 0.033;

        private BackNet net;

        public double Tested = 0;
        public double avgErr = 0;

        public BackNetTrainer(NetSettings s, TrainingData ds)
        {
            Settings = s;
            DataSet = ds;
        }

        public BackNet Train(int count)
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    string s = Console.ReadLine();
                    if (s.Equals("p"))
                    {
                        Console.WriteLine("Tested: " + (Tested / 1000000.0) + " M\nAvg. Error: " + avgErr);
                    }
                    else if (s.Equals("s"))
                    {
                        Console.WriteLine("Saved Network with AvgErr: " + avgErr);
                        net.Save("Net.nn");
                    } else if (s.Equals("e")) {
                        double[] error = net.layers[net.layers.Length - 1].error;
                        int a = 0;
                        for (int i = 0; i < error.Length; i++) {
                            if (Math.Abs(error[i]) > 0.4) {
                                Console.WriteLine("Node '" + i + "' failed check: " + error[i]);
                                a++;
                            }
                        }
                        Console.WriteLine("===[ Summary ]===");
                        Console.WriteLine("Nodes failed check: " + a);
                        Console.WriteLine("Cost: " + net.Error);
                    }
                    else
                    {
                        AdjustMod = double.Parse(s);
                    }
                }
            });
            t.Start();
            net = new BackNet(Settings);
            
            List<double> errs = new List<double>();
            for (int x = 0; x < count; x++)
            {
                errs.Clear();
                for (int y = 0; y < DataSet.Length; y++)
                {
                    net.Feed(DataSet[y].input);
                    net.Train(DataSet[y].output, AdjustMod);
                    errs.Add(net.Error);
                    // Console.WriteLine(net.Error);
                }

                avgErr = 0;
                errs.ForEach(v => avgErr += v);
                avgErr /= errs.Count;

                Tested = DataSet.Length * (x + 1);
                if (Tested % 50000 == 0) Console.WriteLine("Tested: " + (Tested / 1000000.0) + " M\nAvg. Error: " + avgErr);
            }
            return net;
        }

    }

}