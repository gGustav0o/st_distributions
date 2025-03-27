using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st_distributions.Lab3
{
    public static class Manager
    {
        public static void Run()
        {
            var sampleSizes = new[] { 20, 60, 100 };
            var rhos = new[] { 0.0, 0.5, 0.9 };

            foreach (int n in sampleSizes)
            {
                foreach (double rho in rhos)
                {
                    Console.WriteLine($"--- Sample size: {n}, ρ = {rho} ---");

                    var result = CorrelationAnalysis.Analyze(n, rho, repetitions: 1000);

                    PrintResults(result, n, rho, false);
                }
            }

            foreach (int n in sampleSizes)
            {
                Console.WriteLine($"--- Sample size: {n}, Mixture distribution ---");

                var result = CorrelationAnalysis.Analyze(n, rho: 0, repetitions: 1000, useMixture: true);

                PrintResults(result, n, rho: 0, true);
            }
        }
        static void PrintResults(CorrelationResult result, int sampleSize, double rho, bool isMixture)
        {
            var CsvPath = "lab3results.csv";
            bool fileExists = File.Exists(CsvPath);
            using var writer = new StreamWriter(CsvPath, append: true);

            if (!fileExists)
            {
                writer.WriteLine("SampleSize,Rho,Type,Metric,Mean,MeanSquared,Variance");
            }

            string type = isMixture ? "Mixture" : "Normal";
            string rhoStr = isMixture ? "" : rho.ToString("0.0", CultureInfo.InvariantCulture);

            void WriteLine(string metric, CorrelationStats stats)
            {
                writer.WriteLine($"{sampleSize},{rhoStr},{type},{metric}," +
                    $"{stats.Mean.ToString(CultureInfo.InvariantCulture)}," +
                    $"{stats.MeanSquared.ToString(CultureInfo.InvariantCulture)}," +
                    $"{stats.Variance.ToString(CultureInfo.InvariantCulture)}");
            }

            WriteLine("Pearson", result.Pearson);
            WriteLine("Spearman", result.Spearman);
            WriteLine("R2", result.RSquared);
        }
    }
}
