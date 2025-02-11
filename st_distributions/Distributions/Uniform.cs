using MathNet.Numerics.Distributions;
using ScottPlot;
using ScottPlot.Statistics;

namespace st_distributions.Distributions
{
    class Uniform : Distribution
    {
        public Uniform(double[] data, double left, double right) : base(data)
        {
            Left = left;
            Right = right;
        }
        public override double[] GetXs(double step) =>
            Generate.Range(Left * 1.1, Right * 1.1);
        public override double[] GetYs(double[] x, Histogram hist)
        {
            double binWidth = hist.FirstBinSize;
            ContinuousUniform distr = new(Left, Right);
            double[] ys = new double[x.Length];
            for(int i = 0; i < x.Length; i++)
            {
                ys[i] = distr.Density(x[i]) * binWidth;
            }
            return ys;
        }
        public double Left { get;}
        public double Right { get;}
    }
}
