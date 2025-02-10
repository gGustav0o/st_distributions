using ScottPlot;

namespace st_distributions.Distributions
{
    class Normal : Distribution
    {
        public Normal(double[] data) : base(data){}
        public override double[] GetXs(double step)
        {
            double mean = Data.Average();
            double stdDev = StdDev();
            return Generate.Range(mean - 4 * stdDev, mean + 4 * stdDev, step);
        }
        public override double[] GetYs(double[] x, double scale = 1)
        {
            return new double[0];
        }
    }
}
