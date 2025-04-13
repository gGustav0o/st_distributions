using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using st_distributions.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st_distributions.Lab5
{
    public class NormalityAnalyzer
    {
        private const int NormalSampleSize = 100;
        private const int UniformSampleSize = 20;
        private const double Alpha = 0.05;

        public void Run()
        {
            AnalyzeNormalSample();
            AnalyzeUniformSample();
        }

        private void AnalyzeNormalSample()
        {
            var normal = new Distributions.Normal(0, 1, NormalSampleSize);
            var data = normal.Data;

            double muHat = data.Mean();
            double sigmaHat = data.StandardDeviation();

            Console.WriteLine($"[Normal] Estimated mu = {muHat:F4}, sigma = {sigmaHat:F4}");

            AnalyzeWithChiSquared(data, muHat, sigmaHat, NormalSampleSize, "Normal");
        }

        private void AnalyzeUniformSample()
        {
            var uniform = new Uniform(-2, 2, UniformSampleSize);
            var data = uniform.Data;

            double muHat = data.Mean();
            double sigmaHat = data.StandardDeviation();

            Console.WriteLine($"\n[Uniform] Estimated mu = {muHat:F4}, sigma = {sigmaHat:F4}");

            AnalyzeWithChiSquared(data, muHat, sigmaHat, UniformSampleSize, "Uniform");
        }

        private static void AnalyzeWithChiSquared(double[] data, double muHat, double sigmaHat, int size, string label)
        {
            int binsCount = (int)Math.Ceiling(Math.Sqrt(size));
            var histogram = new MathNet.Numerics.Statistics.Histogram(data, binsCount);
            double[] observed = Enumerable.Range(0, histogram.BucketCount)
                .Select(i => (double)histogram[i].Count)
                .ToArray();

            double[] binEdges = Enumerable.Range(0, histogram.BucketCount)
                .Select(i => histogram[i].LowerBound)
                .Append(histogram[histogram.BucketCount - 1].UpperBound)
                .ToArray();

            var normalDistr = new MathNet.Numerics.Distributions.Normal(muHat, sigmaHat);
            double[] expected = new double[observed.Length];

            for (int i = 0; i < observed.Length; i++)
            {
                double cdfLeft = i == 0 ? 0 : normalDistr.CumulativeDistribution(binEdges[i]);
                double cdfRight = normalDistr.CumulativeDistribution(binEdges[i + 1]);
                expected[i] = size * (cdfRight - cdfLeft);
            }

            double chi2 = observed
                .Zip(expected, (o, e) => e > 0 ? Math.Pow(o - e, 2) / e : 0)
                .Sum();

            int degreesOfFreedom = binsCount - 1 - 2;
            double criticalValue = ChiSquared.InvCDF(degreesOfFreedom, 1 - Alpha);

            Console.WriteLine($"\n[{label}] HI^2 = {chi2:F4}, критическое значение = {criticalValue:F4}");
            Console.WriteLine(chi2 < criticalValue
                ? $"[{label}] Нормальность не отвергается."
                : $"[{label}] Нормальность отвергается.");

            Console.WriteLine($"\n[{label}] Таблица HI^2:");
            Console.WriteLine("Interval\tObserved\tExpected");
            for (int i = 0; i < observed.Length; i++)
            {
                Console.WriteLine($"{binEdges[i]:F4}–{binEdges[i + 1]:F4}\t{observed[i]}\t\t{expected[i]:F4}");
            }
        }
    }
}
