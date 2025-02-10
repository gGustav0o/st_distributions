using ScottPlot;

namespace st_distributions.Distributions
{
    class Uniform : Distribution
    {
        public Uniform(double lower, double upper, int size)
        {
            NumDistribution = new(lower, upper);
            Data = NumDistribution.Samples().Take(size).ToArray();
        }
        public override MathNet.Numerics.Distributions.ContinuousUniform NumDistribution { get; }
        public override double[] GetXs(double step) =>
            Generate.Range(NumDistribution.LowerBound * 1.1, NumDistribution.UpperBound * 1.1, step);
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
