using ScottPlot;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using ScottPlot.Statistics;

namespace st_distributions.Distributions
{
    class Normal : Distribution
    {
        public Normal(double mean, double stddev, int size)
            : base(size, new MathNet.Numerics.Distributions.Normal(mean: mean, stddev: stddev))
        {
            Data = MathNet.Numerics.Distributions.Normal.Samples(mean: mean, stddev: stddev).Take(size).ToArray();
        }
        public override double[] GetXs(double step)
             => Generate.Range(Distr().Mean - 4 * Distr().StdDev, Distr().Mean + 4 * Distr().StdDev, step);
        public override double[] GetYs(double[] x, Histogram hist)
        {
            double[] ys = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                ys[i] = Distr().Density(x[i]) * Scale(hist);
            }
            return ys;
        }
        private MathNet.Numerics.Distributions.Normal Distr() => (MathNet.Numerics.Distributions.Normal)_distr;
        protected override double Scale(Histogram hist) => hist.FirstBinSize;
    }
}
