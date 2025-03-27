using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st_distributions.Lab3
{
    public static class BivariateNormalGenerator
    {

        public static (double[], double[]) GenerateMixture(int n)
        {
            int n1 = (int)(n * 0.9);
            int n2 = n - n1;

            var part1 = Generate(n1, 0.9);
            var part2 = Generate(n2, -0.9, 10, 10);

            double[] x = [.. part1.Item1, .. part2.Item1];
            double[] y = [.. part1.Item2, .. part2.Item2];

            return (x, y);
        }

        public static (double[], double[]) Generate(int n, double rho, double mx = 0, double my = 0)
        {
            var normal = new Normal(0, 1);
            double[] x = new double[n];
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                double u = normal.Sample();
                double v = normal.Sample();

                x[i] = mx + u;
                y[i] = my + rho * u + Math.Sqrt(1 - rho * rho) * v;
            }

            return (x, y);
        }
    }
}
