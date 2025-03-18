using ScottPlot;
using ScottPlot.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st_distributions.Distributions
{
    class Poisson : Distribution
    {
        public Poisson(double lambda, int size)
            : base
            (
                  size
                  , new MathNet.Numerics.Distributions.Poisson(lambda)
                  , @"P(X = k) = \frac{\lambda^k e^{-\lambda}}{k!}"
            )
        {
            Data = MathNet.Numerics.Distributions.Poisson.Samples(Rand, 10).Take(size).Select(x => (double)x).ToArray();
        }
        public override double[] GetXs(double step)
            => Enumerable.Range(0, (int)Distr().Lambda * 3).Select(x => (double)x).ToArray();
        public override double[] GetYs(double[] x, Histogram hist)
        {
            double[] ys = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                ys[i] = Distr().Probability((int)x[i]) * Scale(hist);
            }
            return ys;
        }
        private MathNet.Numerics.Distributions.Poisson Distr() => (MathNet.Numerics.Distributions.Poisson)_distr;
        protected override double Scale(Histogram hist) => hist.FirstBinSize;
    }
}
