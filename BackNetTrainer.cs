using System;
using System.Collections.Generic;

namespace LetsGetNeural
{

    public class BackNetTrainer
    {

        public NetSettings Settings { get; private set; }
        public TrainingData DataSet { get; private set; }

        public BackNetTrainer(NetSettings s, TrainingData ds)
        {
            Settings = s;
            DataSet = ds;
        }

        public BackNet Train(int count)
        {
            BackNet net = new BackNet(Settings);
            double Tested = 0;
            double avgErr = 0;
            List<double> errs = new List<double>();
            for (int x = 0; x < count; x++)
            {
                errs.Clear();
                for (int y = 0; y < DataSet.Length; y++)
                {
                    net.Feed(DataSet[y].input);
                    net.Train(DataSet[y].output);
                    errs.Add(net.Error);
                    // Console.WriteLine(net.Error);
                }

                avgErr = 0;
                errs.ForEach(v => avgErr += v);
                avgErr /= errs.Count;

                Tested = DataSet.Length * (x + 1);
                if (Tested % 100000 == 0) Console.WriteLine("Tested: " + (Tested / 1000000.0) + " M\nAvg. Error: " + avgErr);
            }
            return net;
        }

    }

}