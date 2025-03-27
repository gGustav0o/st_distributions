using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st_distributions.Lab3
{
    public static class CorrelationMetrics
    {
        public static (double pearson, double spearman, double r2) Compute(double[] x, double[] y)
        {
            double pearson = Correlation.Pearson(x, y);
            double spearman = Correlation.Spearman(x, y);
            double r2 = pearson * pearson;

            return (pearson, spearman, r2);
        }
    }
}
