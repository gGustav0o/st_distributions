using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st_distributions.Lab3
{
    public record CorrelationStats(
        double Mean,
        double MeanSquared,
        double Variance
    );

    public record CorrelationResult(
        CorrelationStats Pearson,
        CorrelationStats Spearman,
        CorrelationStats RSquared
    );

    public static class CorrelationAnalysis
    {
        public static CorrelationResult Analyze(
            int sampleSize,
            double rho,
            int repetitions = 1000,
            bool useMixture = false)
        {
            var pearsons = new List<double>(repetitions);
            var spearmans = new List<double>(repetitions);
            var rsquareds = new List<double>(repetitions);

            double[]? exampleX = null;
            double[]? exampleY = null;

            for (int i = 0; i < repetitions; i++)
            {
                double[] x, y;

                if (useMixture)
                    (x, y) = BivariateNormalGenerator.GenerateMixture(sampleSize);
                else
                    (x, y) = BivariateNormalGenerator.Generate(sampleSize, rho);

                if (i == 0)
                {
                    exampleX = x;
                    exampleY = y;
                }

                double p = Correlation.Pearson(x, y);
                double s = Correlation.Spearman(x, y);
                double r2 = p * p;

                pearsons.Add(p);
                spearmans.Add(s);
                rsquareds.Add(r2);
            }

            string name = useMixture
                ? $"mixture_n{sampleSize}"
                : $"normal_n{sampleSize}_rho{rho.ToString("0.0").Replace(",", ".")}";

            ScatterPlotWithEllipse.PlotWithEllipse(exampleX, exampleY, name, $"{name}.png");

            return new CorrelationResult(
                Pearson: ComputeStats(pearsons),
                Spearman: ComputeStats(spearmans),
                RSquared: ComputeStats(rsquareds)
            );
        }

        private static CorrelationStats ComputeStats(List<double> values)
        {
            double mean = values.Mean();
            double meanSquared = values.Select(v => v * v).Mean();
            double variance = values.Variance();
            return new CorrelationStats(mean, meanSquared, variance);
        }

    }
}
