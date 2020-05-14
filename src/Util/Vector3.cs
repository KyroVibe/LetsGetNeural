using System;
using System.Collections.Generic;

namespace LetsGetNeural.Util {

    public struct Vector3 {

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 a, double b) => new Vector3(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator *(double a, Vector3 b) => new Vector3(b.X * a, b.Y * a, b.Z * a);
        public static Vector3 operator /(Vector3 a, double b) => new Vector3(a.X / b, a.Y / b, a.Z / b);

        public Vector3(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }

        public double[] ToBits() {
            List<double> bitArray = new List<double>();
            Array.ForEach(Utility.toBits(X), x => bitArray.Add(x));
            Array.ForEach(Utility.toBits(Y), x => bitArray.Add(x));
            Array.ForEach(Utility.toBits(Z), x => bitArray.Add(x));
            return bitArray.ToArray();
        }

    }

}