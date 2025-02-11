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
        public Poisson(double[] data, double lambda) : base(data)
        {
            Lambda = lambda;
        }
        public override double[] GetXs(double step)
        {
            return Enumerable.Range(0, (int)Lambda * 3).Select(x => (double)x).ToArray();
        }
        public override double[] GetYs(double[] x, Histogram hist)
        {
            double binWidth = hist.FirstBinSize;
            MathNet.Numerics.Distributions.Poisson distr = new(Lambda);
            double[] ys = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                ys[i] = distr.Probability((int)x[i]) * binWidth;
            }
            return ys;
        }
        public double Lambda { get; }
    }
}
