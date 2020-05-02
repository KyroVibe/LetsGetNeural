using System;
using System.Collections.Generic;

namespace LetsGetNeural
{
    public class NeuralNetwork
    {
        public int[] Settings { get; private set; }
        public double[][] nodes { get; set; }
        public double[][][] weights { get; set; }
        public double[] biases { get; set; }
        public double LastTestedCost { get; private set; }
        public Random rand { get; private set; }

        public NeuralNetwork(int[] nodeLayerInfo)
        {

            Settings = nodeLayerInfo;

            rand = new Random(DateTime.Now.Millisecond);

            nodes = new double[nodeLayerInfo.Length][];
            weights = new double[nodeLayerInfo.Length - 1][][];
            biases = new double[nodeLayerInfo.Length - 1];
            for (int l = 0; l < nodes.Length; l++)
            {
                nodes[l] = new double[nodeLayerInfo[l]];
                if (l < nodeLayerInfo.Length - 1)
                {
                    weights[l] = new double[nodes[l].Length][];
                    for (int i = 0; i < weights[l].Length; i++)
                    {
                        weights[l][i] = new double[nodeLayerInfo[l + 1]];
                    }
                }
            }

            InitNodes();
            InitWeightsBiases();
        }

        // Deep Copy Constructor
        public NeuralNetwork(NeuralNetwork n)
        {
            Settings = (int[])n.Settings.Clone();
            nodes = (double[][])n.nodes.Clone();
            weights = (double[][][])n.weights.Clone();
            biases = (double[])n.biases.Clone();
            rand = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
        }

        #region Setup

        public void InitNodes()
        {
            for (int x = 0; x < nodes.Length; x++)
            {
                for (int y = 0; y < nodes[x].Length; y++)
                {
                    nodes[x][y] = 0.0;
                }
                nodes[x][nodes[x].Length - 1] = rand.NextDouble() - 0.5;
            }
        }

        public void InitWeightsBiases()
        {
            for (int x = 0; x < weights.Length; x++)
            {
                biases[x] = (rand.NextDouble() * 2) - 1;
                              // Console.WriteLine("Layer");
                for (int y = 0; y < weights[x].Length; y++)
                {
                    // Console.WriteLine(weights[x][y].Length);
                    for (int z = 0; z < weights[x][y].Length; z++)
                    {
                        weights[x][y][z] = rand.NextDouble() - 0.5;
                    }
                }
            }
        }

        #endregion

        #region Test and Evaluate

        private double[] Iterate(double[] layer, double[][] weight, double bias)
        {
            double[] newLayer = new double[weight[0].Length];
            for (int x = 0; x < newLayer.Length; x++)
            {
                newLayer[x] = 0;
                for (int y = 0; y < layer.Length; y++)
                {
                    newLayer[x] = newLayer[x] + (layer[y] * weight[y][x]);
                }
                newLayer[x] = Math.Tanh(newLayer[x] + bias);
            }
            return newLayer;
        }

        public double[] Iterate(double[] input)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                input = Iterate(input, weights[i], biases[i]);
            }
            return input;
        }

        public double Cost(double[] input, double[] output)
        {
            double[] tO = Iterate(input);
            for (int i = 0; i < tO.Length; i++)
            {
                tO[i] = Math.Pow(output[i] - tO[i], 2);
            }
            double a = 0;
            Array.ForEach(tO, x => a += x);
            return a;
        }

        public double AvgCost((double[] input, double[] output)[] data)
        {
            List<double> costs = new List<double>();
            for (int i = 0; i < data.Length; i++)
            {
                costs.Add(Cost(data[i].input, data[i].output));
            }
            double a = 0;
            Array.ForEach(costs.ToArray(), x => a += x);
            LastTestedCost = a / costs.Count;
            return a / costs.Count;
        }

        #endregion

        #region Reproduction

        public static NeuralNetwork Breed(NeuralNetwork a, NeuralNetwork b)
        {
            Random rand = new Random(DateTime.Now.Second);
            NeuralNetwork n = new NeuralNetwork(a);
            for (int x = 0; x < n.nodes.Length; x++)
            {
                if (x < n.nodes.Length - 1)
                {
                    if (rand.NextDouble() > 0.5) n.biases[x] = b.biases[x];
                }
                for (int y = 0; y < n.nodes[x].Length; y++)
                {
                    if (rand.NextDouble() > 0.5) n.nodes[x][y] = b.nodes[x][y];
                    if (x < n.nodes.Length - 1)
                    {
                        for (int z = 0; z < n.weights[x][y].Length; z++)
                        {
                            if (rand.NextDouble() > 0.5) n.weights[x][y][z] = b.weights[x][y][z];
                        }
                    }
                }
            }
            return n;
        }

        public void Mutate(double chance)
        {
            for (int x = 0; x < weights.Length; x++)
            {
                if (rand.NextDouble() <= chance) biases[x] = (rand.NextDouble() * 2) - 1;
                for (int y = 0; y < nodes[x].Length; y++)
                {
                    for (int z = 0; z < weights[x][y].Length; z++)
                    {
                        if (rand.NextDouble() <= chance) weights[x][y][z] = rand.NextDouble() - 0.5;
                    }
                }
            }
        }

        #endregion
    }

    public class NetComparer : IComparer<NeuralNetwork>
    {
        public int Compare(NeuralNetwork a, NeuralNetwork b)
        {
            if (a.LastTestedCost == b.LastTestedCost)
            {
                return 0;
            }
            else if (a.LastTestedCost < b.LastTestedCost)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}