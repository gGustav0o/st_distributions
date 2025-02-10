using ScottPlot;

namespace st_distributions.Distributions
{
    class Poisson : Distribution
    {
        public Poisson(double lambda, int size)
        {
            NumDistribution = new(lambda);
            Data = NumDistribution.Samples().Take(size).Select(x => (double)x).ToArray();
        }
        public override MathNet.Numerics.Distributions.Poisson NumDistribution { get; }
        public override double[] GetXs(double step) =>
            Enumerable.Range(0, ((int)NumDistribution.Lambda * 3)).Select(x => (double)x).ToArray();
        public override double[] GetYs(double[] x)
        {
            double[] ys = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                ys[i] = NumDistribution.Probability(i);
            }
            return ys;
        }
    }
}
