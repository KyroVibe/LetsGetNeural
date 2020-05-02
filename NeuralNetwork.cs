using System;
using System.Collections.Generic;

namespace LetsGetNeural {
  public class NeuralNetwork {

    public double[][] nodes { get; private set; }
    public double[][][] weights { get; private set; }
    public double LastTestedCost { get; private set; }
    public Random rand { get; private set; }

    public NeuralNetwork(int[] nodeLayerInfo) {

      rand = new Random(DateTime.Now.Millisecond);

      nodes = new double[nodeLayerInfo.Length][];
      weights = new double[nodeLayerInfo.Length - 1][][];
      for (int l = 0; l < nodes.Length; l++) {
        int bias = 0;
        if (l != 0 && l != nodes.Length - 1) bias++;
        nodes[l] = new double[nodeLayerInfo[l] + bias];
        if (l < nodeLayerInfo.Length - 1)
        {
          weights[l] = new double[nodes[l].Length][];
          for (int i = 0; i < weights[l].Length; i++) {
            weights[l][i] = new double[nodeLayerInfo[l + 1]];
          }
        }
      }

      InitNodes();
      InitWeights();
    }

    public void InitNodes() {
      for (int x = 0; x < nodes.Length; x++) {
        for (int y = 0; y < nodes[x].Length; y++) {
          nodes[x][y] = 0.0;
        }
        nodes[x][nodes[x].Length - 1] = rand.NextDouble() - 0.5;
      }
    }

    public void InitWeights() {
      for (int x = 0; x < weights.Length; x++) {
        // Console.WriteLine("Layer");
        for (int y = 0; y < weights[x].Length; y++) {
          // Console.WriteLine(weights[x][y].Length);
          for (int z = 0; z < weights[x][y].Length; z++) {
            weights[x][y][z] = rand.NextDouble() - 0.5;
          }
        }
      }
    }

    private double[] Iterate(double[] layer, double[][] weight) {
      double[] newLayer = new double[weight[0].Length];
      for (int x = 0; x < newLayer.Length; x++) {
        newLayer[x] = 0;
        for (int y = 0; y < layer.Length; y++) {
          newLayer[x] = newLayer[x] + (layer[y] * weight[y][x]);
        }
        newLayer[x] = Math.Tanh(newLayer[x]);
      }
      return newLayer;
    }

    public double[] Iterate(double[] input) {
      for (int i = 0; i < weights.Length; i++) {
        input = Iterate(input, weights[i]);
      }
      return input;
    }

    public double Cost(double[] input, double[] output) {
      double[] tO = Iterate(input);
      for (int i = 0; i < tO.Length; i++) {
        tO[i] = Math.Pow(output[i] - tO[i], 2);
      }
      double a = 0;
      Array.ForEach(tO, x => a += x);
      return a;
    }

    public double AvgCost((double[] input, double[] output)[] data) {
      List<double> costs = new List<double>();
      for (int i = 0; i < data.Length; i++) {
        costs.Add(Cost(data[i].input, data[i].output));
      }
      double a = 0;
      Array.ForEach(costs.ToArray(), x => a += x);
      return a / costs.Count;
    }
  }
}