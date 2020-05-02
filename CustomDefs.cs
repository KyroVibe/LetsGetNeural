using System;

namespace LetsGetNeural {

  public struct TrainingData {

    public static implicit operator (double[] input, double[] output)[](TrainingData d) => d.dataSet;
    public static implicit operator TrainingData((double[] input, double[] output)[] d) => new TrainingData(d);

    (double[] input, double[] output)[] dataSet;

    public TrainingData((double[] input, double[] output)[] ds) {
      dataSet = ds;
    }

  }

  public struct NetSettings {

    public static implicit operator int[](NetSettings ns) => ns.settings;
    public static implicit operator NetSettings(int[] settings) => new NetSettings(settings);

    int[] settings;

    public NetSettings(int[] s) {
      settings = s;
    }

  }

}