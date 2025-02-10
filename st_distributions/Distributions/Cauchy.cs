using ScottPlot;

namespace st_distributions.Distributions
{
    class Cauchy : Distribution
    {
        public Cauchy(double[] data, double location) : base(data)
        {
            Location = location;
        }
        public override double[] GetXs(double step) =>
            Generate.Range(x0 - 10 * gamma, x0 + 10 * gamma, step);
        public override double[] GetYs(double[] x, double scale = 1)
        {
            return new double[0];
        }
        public double Location { get; }
    }
}
