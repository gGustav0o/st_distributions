using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st_distributions.Distributions
{
    class Poisson : Distribution
    {
        public Poisson(double[] data) : base(data) { }
        public override double[] GetXs(double step)
        {
            return new double[0];
        }
        public override double[] GetYs(double[] x, double scale = 1)
        {
            return new double[0];
        }
    }
}
