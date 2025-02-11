using ScottPlot;
using ScottPlot.Statistics;
using MathNet.Numerics.Distributions;

namespace st_distributions.Distributions
{
    class Cauchy : Distribution
    {
        public Cauchy(double[] data, double location, double scale) : base(data)
        {
            Location = location;
            Scale = scale;
        }
        public override double[] GetXs(double step) =>
            Generate.Range(Location - 10 * Scale, Location + 10 * Scale, step);
        public override double[] GetYs(double[] x, Histogram hist)
        {
            double maxHistY = hist.GetProbability().Max();
            MathNet.Numerics.Distributions.Cauchy distr = new(Location, Scale);

            double maxDensity = distr.Density(Location);     // Пик плотности PDF в центре
            double scaleFactor = maxHistY / maxDensity;      // Коэффициент масштабирования

            double[] ys = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                ys[i] = distr.Density(x[i]) * scaleFactor;
            }
            return ys;
        }
        public double Location { get; }
        public double Scale { get; }
    }
}
