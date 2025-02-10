using ScottPlot;

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
        public override double[] GetYs(double[] x, double scale = 1)
        {
            return new double[0];
        }
        public double Left { get;}
        public double Right { get;}
    }
}
