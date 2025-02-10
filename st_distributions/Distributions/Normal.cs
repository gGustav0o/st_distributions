using ScottPlot;

namespace st_distributions.Distributions
{
    class Normal : Distribution
    {
        public Normal(double mean, double stdDev, int size)
        {
            NumDistribution = new(mean, stdDev);
            Data = NumDistribution.Samples().Take(size).ToArray();
        }
        public override MathNet.Numerics.Distributions.Normal NumDistribution { get; }
        public override double[] GetXs(double step) =>
            Generate.Range(NumDistribution.Mean - 4 * NumDistribution.StdDev, NumDistribution.Mean + 4 * NumDistribution.StdDev, step);
        public override double[] GetYs(double[] x)
        {
            double[] ys = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                ys[i] = NumDistribution.Density(i);
            }
            return ys;
        }
    }
}
