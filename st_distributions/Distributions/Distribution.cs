using MathNet.Numerics.Distributions;
using ScottPlot;

namespace st_distributions.Distributions
{
    public abstract class Distribution
    {
        public double[] Data;
        public abstract IUnivariateDistribution NumDistribution { get; }
        public abstract double[] GetXs(double step);
        public abstract double[] GetYs(double[] x, double scale = 1);
    }
}
