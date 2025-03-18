using MathNet.Numerics.Distributions;
using ScottPlot;
using ScottPlot.Statistics;

namespace st_distributions.Distributions
{
    class Uniform : Distribution
    {
        public Uniform(double lower, double upper, int size)
            : base
            (
                  size
                  , new MathNet.Numerics.Distributions.ContinuousUniform(lower, upper)
                  , @"f(x) = \frac{1}{b-a}, \; a \leq x \leq b"
            )
        {
            Data = MathNet.Numerics.Distributions.ContinuousUniform.Samples(lower, upper).Take(size).ToArray();
        }
        public override double[] GetXs(double step)
        {
            var xs = Generate.Range(Distr().LowerBound, Distr().UpperBound, step).ToList();
            return xs.ToArray();
        }
        public override double[] GetYs(double[] x, Histogram hist)
        {
            double[] ys = new double[x.Length];
            for(int i = 0; i < x.Length; i++)
            {
                ys[i] = Distr().Density(x[i]) * Scale(hist);
            }
            return ys;
        }
        protected override double Scale(Histogram hist) => hist.FirstBinSize;
        private MathNet.Numerics.Distributions.ContinuousUniform Distr() => (MathNet.Numerics.Distributions.ContinuousUniform)_distr;
    }
}
