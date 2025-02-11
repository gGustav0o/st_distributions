using ScottPlot;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using ScottPlot.Statistics;

namespace st_distributions.Distributions
{
    class Normal : Distribution
    {
        public Normal(double[] data, double mean, double stddev) : base(data)
        {
            Mean = mean;
            Stddev = stddev;
        }
        public override double[] GetXs(double step)
        {
            double mean = Data.Average();
            double stdDev = StdDev();
            return Generate.Range(mean - 4 * stdDev, mean + 4 * stdDev, step);
        }
        public override double[] GetYs(double[] x, Histogram hist)
        {
            double binWidth = hist.FirstBinSize;
            MathNet.Numerics.Distributions.Normal distr = new(Mean, Stddev);
            double[] ys = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                ys[i] = distr.Density(x[i]) * binWidth;
            }
            return ys;
        }
        public double Mean { get; }
        public double Stddev { get; }
    }
}
