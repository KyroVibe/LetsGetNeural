using System;
using System.Collections.Generic;
using System.Text;

namespace LetsGetNeural.Data
{
    public interface IDataPoint
    {
        int[] settings { get; }
        double[] input { get; }
        double[] output { get; }

        void Generate();
    }
}
