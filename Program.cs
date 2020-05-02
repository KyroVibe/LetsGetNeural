using System;
using System.Collections.Generic;
using System.Collections;

namespace LetsGetNeural
{
  class Program
  {
    public static int[] SETTINGS = new int[]{ 64, 3, 3, 64 };

    static void Main(string[] args)
    {
      NeuralNetwork[] n = new NeuralNetwork[1000];
      for (int i = 0; i < n.Length; i++) {
        n[i] = new NeuralNetwork(SETTINGS);
      }

      var data = GenTestData(500);
      double c = n.AvgCost(data);
      Console.WriteLine(c);
    }

    static (double[] input, double[] output)[] GenTestData(int count) {
      Random rand = new Random(DateTime.Now.Millisecond);
      List<(double[] input, double[] output)> testData = new List<(double[] input, double[] output)>();
      for (int i = 0; i < count; i++) {
        double a = rand.Next(-count, count);
        double b = Math.Pow(a, 2);
        testData.Add((toBitArray(a), toBitArray(b)));
      }
      return testData.ToArray();
    }

    static double[] toBitArray(double val) {
      BitArray b = new BitArray(BitConverter.GetBytes(val));
      double[] a = new double[64];
      for (int i = 0; i < b.Count; i++) {
        a[i] = b[i] ? 1 : 0;
      }
      return a;
      // int[] bits = b.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();
    }
  }
}
