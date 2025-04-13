using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.Optimization;
using st_distributions.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st_distributions.Lab4
{
    class RegressionComparison
    {
        private const double Step = 0.2;
        private const int Count = 20;

        public static void Run()
        {
            var normal = new Normal(mean: 0, stddev: 1, size: Count);

            double[] x = Enumerable.Range(0, Count).Select(i => -1.8 + i * Step).ToArray();
            double[] noise = normal.Data;
            double[] y = x.Select((xi, i) => 2 + 2 * xi + noise[i]).ToArray();

            Analyze(x, y, "Невозмущённая выборка");

            double[] y_perturbed = (double[])y.Clone();
            y_perturbed[0] += 10;
            y_perturbed[^1] -= 10;

            Analyze(x, y_perturbed, "Возмущённая выборка");
        }

        private static void Analyze(double[] x, double[] y, string title)
        {
            Console.WriteLine($"\n=== {title} ===");

            var (a_ols, b_ols) = OrdinaryLeastSquares(x, y);
            Console.WriteLine($"МНК: a = {a_ols:F4}, b = {b_ols:F4}");

            var (a_l1, b_l1) = LeastAbsoluteDeviations(x, y);
            Console.WriteLine($"МНМ: a = {a_l1:F4}, b = {b_l1:F4}");
        }

        private static (double a, double b) OrdinaryLeastSquares(double[] x, double[] y)
        {
            int n = x.Length;
            var matrixX = Matrix<double>.Build.Dense(n, 2, (i, j) => j == 0 ? 1.0 : x[i]);
            var vectorY = Vector<double>.Build.DenseOfArray(y);

            var xt = matrixX.Transpose();
            var xtx = xt * matrixX;
            var xty = xt * vectorY;
            var beta = xtx.Solve(xty);

            return (beta[0], beta[1]);
        }

        private static (double a, double b) LeastAbsoluteDeviations(double[] x, double[] y)
        {
            var objective = ObjectiveFunction.Value(v =>
            {
                double a = v[0], b = v[1];
                return x.Select((xi, i) => Math.Abs(y[i] - a - b * xi)).Sum();
            });

            var initialGuess = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0.0 });
            var result = NelderMeadSimplex.Minimum(objective, initialGuess, 1e-6, 1000);

            return (result.MinimizingPoint[0], result.MinimizingPoint[1]);
        }

    }
}
