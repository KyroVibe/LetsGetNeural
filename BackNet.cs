using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LetsGetNeural
{

    [Serializable]
    public class BackNet
    {
        public NetSettings settings { get; private set; }

        public Layer[] layers;

        public double Error {
            get {
                double[] e = layers[layers.Length - 1].error;
                double cost = 0;
                Array.ForEach(e, x => cost += Math.Pow(x, 2));
                return cost;
            }
        }

        public BackNet(int[] s)
        {
            settings = s;
            layers = new Layer[s.Length - 1];
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(s[i], s[i + 1]);
            }
        }

        public double[] Feed(double[] input)
        {
            layers[0].Feed(input);
            for (int i = 1; i < layers.Length; i++)
            {
                layers[i].Feed(layers[i - 1].output);
            }
            return layers[layers.Length - 1].output;
        }

        public void Train(double[] expected)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                if (i == 0)
                {
                    layers[(layers.Length - 1) - i].TrainOutput(expected);
                }
                else
                {
                    layers[(layers.Length - 1) - i].TrainHidden(layers[(layers.Length) - i].gamma, layers[(layers.Length) - i].weights);
                }

                layers[(layers.Length - 1) - i].UpdateWeights();
            }
        }

        public void Save(string path)
        {
            BinaryFormatter form = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Create);
            form.Serialize(fs, this);
            fs.Flush();
            fs.Close();
        }

        public static NeuralNetwork Load(string path)
        {
            BinaryFormatter form = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);
            return (NeuralNetwork)form.Deserialize(fs);
        }

    }

    public class Layer
    {

        public double[,] weights, weightsDelta;

        // Keep track of for training
        public double[] input, output, gamma, error;

        private Random rand;

        public Layer(int inputLen, int outputLen)
        {
            weights = new double[inputLen, outputLen];
            weightsDelta = new double[inputLen, outputLen];
            input = new double[inputLen];
            output = new double[outputLen];
            error = new double[outputLen];
            gamma = new double[outputLen];

            rand = new Random(DateTime.Now.Millisecond);

            InitWeights();
        }

        public void InitWeights()
        {
            for (int x = 0; x < input.Length; x++)
            {
                for (int y = 0; y < output.Length; y++)
                {
                    weights[x, y] = rand.NextDouble() - 0.5;
                }
            }
        }

        public double TanhDer(double val)
        {
            return 1 - (val * val);
        }

        public double[] Feed(double[] i)
        {
            input = i;
            for (int x = 0; x < output.Length; x++)
            {
                output[x] = 0;
                for (int y = 0; y < input.Length; y++)
                {
                    output[x] += weights[y, x] * input[y];
                }
                output[x] = Math.Tanh(output[x]);
            }
            return output;
        }

        public void TrainOutput(double[] expected)
        {
            for (int i = 0; i < expected.Length; i++)
            {
                error[i] = output[i] - expected[i];
                gamma[i] = error[i] * TanhDer(output[i]);
            }

            for (int x = 0; x < input.Length; x++)
            {
                for (int y = 0; y < output.Length; y++)
                {
                    weightsDelta[x, y] = gamma[y] * input[x];
                }
            }
        }

        public void TrainHidden(double[] g, double[,] w)
        {
            for (int x = 0; x < output.Length; x++)
            {
                gamma[x] = 0;

                for (int y = 0; y < g.Length; y++)
                {
                    gamma[x] += g[y] * w[x, y];
                }

                gamma[x] *= TanhDer(output[x]);
            }

            for (int x = 0; x < input.Length; x++)
            {
                for (int y = 0; y < output.Length; y++)
                {
                    weightsDelta[x, y] = gamma[y] * input[x];
                }
            }
        }

        public void UpdateWeights()
        {
            for (int x = 0; x < input.Length; x++)
            {
                for (int y = 0; y < output.Length; y++)
                {
                    weights[x, y] -= weightsDelta[x, y] * 0.015;// 0.033;
                }
            }
        }

    }

}