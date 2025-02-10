using ScottPlot;

namespace st_distributions.Distributions
{
    public abstract class Distribution
    {
        public Distribution(double[] data)
        {
            Data = data;
        }
        public double[] Data { get; set; }
        public abstract double[] GetXs(double step);
        public abstract double[] GetYs(double[] x, double scale = 1);
        public double StdDev() =>
            Math.Sqrt(
                Data.Select(x => x * x).Average()
                - Math.Pow(Data.Average(), 2.0)
            );
    }
}
