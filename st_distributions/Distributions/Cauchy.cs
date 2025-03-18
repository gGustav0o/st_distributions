using ScottPlot;
using ScottPlot.Statistics;
using MathNet.Numerics.Distributions;

namespace st_distributions.Distributions
{
    class Cauchy : Distribution
    {
        public Cauchy(double location, double scale, int size)
            : base
            (
                  size
                  , new MathNet.Numerics.Distributions.Cauchy(location, scale)
                  , @"f(x) = \frac{1}{\pi \gamma \left[1+\left(\frac{x-x_0}{\gamma}\right)^2\right]}"
            )
        {
            Data = MathNet.Numerics.Distributions.Cauchy.Samples(Rand, location, scale).Take(size).ToArray();
        }
        public override double[] GetXs(double step) =>
            Generate.Range(Distr().Location - 10 * Distr().Scale, Distr().Location + 10 * Distr().Scale, step);
        public override double[] GetYs(double[] x, Histogram hist)
        {
            double[] ys = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                ys[i] = Distr().Density(x[i]) * Scale(hist);
            }
            return ys;
        }

        protected override double Scale(Histogram hist)
            => hist.GetProbability().Max() / Distr().Density(Distr().Location);

        private MathNet.Numerics.Distributions.Cauchy Distr() => (MathNet.Numerics.Distributions.Cauchy)_distr;
    }
}
