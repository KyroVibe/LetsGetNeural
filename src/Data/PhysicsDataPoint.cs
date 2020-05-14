using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LetsGetNeural.Util;

namespace LetsGetNeural.Data
{
    /// <summary>
    /// Input: Mass, Inital Speed, Net Force, Timestep
    /// Output: Position Displacement
    /// </summary>
    public class PhysicsDataPoint : IDataPoint {

        public int[] settings { get => new int[] { 512, 192 }; }
        public double[] input { get; private set; }
        public double[] output { get; private set; }

        private static Random rand;

        /// <summary>
        /// Constructs the PhysicsDataPoint and generates a random data point
        /// </summary>
        public PhysicsDataPoint() {
            output = new double[settings[settings.Length - 1]];
            if (rand == null) rand = new Random(DateTime.Now.Millisecond);

            Generate();
        }

        public void Generate() {
            // Get input
            double mass = (double)rand.Next(50000) / 100.0;
            int numForces = rand.Next(5);
            Vector3 netForce = new Vector3();
            for (int i = 0; i < numForces; i++) {
                netForce += RandomVector(5000, 100.0);
            }
            Vector3 initalSpeed = RandomVector(100, 10.0);
            double timeStep = ((double)rand.Next(1) / 1000.0) + 0.001;
            List<double> inputArray = new List<double>();
            Array.ForEach(Utility.toBits(mass), x => inputArray.Add(x));
            Array.ForEach(netForce.ToBits(), x => inputArray.Add(x));
            Array.ForEach(initalSpeed.ToBits(), x => inputArray.Add(x));
            Array.ForEach(Utility.toBits(timeStep), x => inputArray.Add(x));
            input = inputArray.ToArray();

            // Compute output
            Vector3 acceleration = netForce / mass;
            Vector3 displacement = ((0.5 * acceleration) * (Math.Pow(timeStep, 2))) + (initalSpeed * timeStep);

            // Serial output to bits
            output = displacement.ToBits();
        }

        public (double[] input, double[] output) ToTuple() {
            return (input, output);
        }

        private static Vector3 RandomVector(int a, double b) => new Vector3((double)rand.Next(a) / b, (double)rand.Next(a) / b, (double)rand.Next(a) / b);
    }
}
