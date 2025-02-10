using MathNet.Numerics.Distributions;
using ScottPlot;

namespace st_distributions.Distributions
{
    class Cauchy : Distribution
    {
        public Cauchy(double location, double scale, int size)
        {
            NumDistribution = new(location, scale);
            Data = NumDistribution.Samples().Take(size).ToArray();
        }
        public override MathNet.Numerics.Distributions.Cauchy NumDistribution { get; }
        public override double[] GetXs(double step) =>
            Generate.Range(NumDistribution.Location - 10 * NumDistribution.Scale, NumDistribution.Location + 10 * NumDistribution.Scale, step);
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
