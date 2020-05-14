using System;
using System.Collections;

namespace LetsGetNeural.Util {

    public class Utility {

        public static double[] toBits(double val)
        {
            BitArray b = new BitArray(BitConverter.GetBytes(val));
            double[] a = new double[sizeof(double) * 8];
            for (int i = 0; i < b.Count; i++)
            {
                a[i] = b[i] ? 1 : 0;
            }
            return a;
        }

    }

}