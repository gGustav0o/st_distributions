﻿
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using st_distributions.Distributions;

namespace st_distributions
{
    public class StatisticsManager
    {
        private static readonly int[] SampleSizes = [10, 50, 100, 1000];
        private const int Iterations = 1000;
        private static readonly string OutputFolder = "Results";
        public static void RunAnalysis()
        {
            if (Directory.Exists(OutputFolder))
            {
                Directory.Delete(OutputFolder, true);
            }
            Directory.CreateDirectory(OutputFolder);

            foreach (var size in SampleSizes)
            {
                Console.WriteLine($"Process samples of size {size}...");

                SystemRandomSource rand = new();

                Distribution[] datasets = [
                    new Normal(0, 1),
                    new Cauchy(0, 1),
                    new Poisson(10),
                    new ContinuousUniform(-Math.Sqrt(3), Math.Sqrt(3))
                ];

                for (int i = 0; i < datasets.Length; i++)
                {
                    string dir = $"{OutputFolder}/{datasets[i].GetType().Name}";
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    string filename = $"{dir}/{datasets[i].GetType().Name}_{size}.png";
                    Plotter.PlotHistogram(datasets[i], filename, size);
                    Console.WriteLine($"Сохранен график: {filename}");
                    filename = $"{dir}/{datasets[i].GetType().Name}_{size}_data.txt";
                    SaveSamplesToFile(datasets[i].Data, filename);
                }

                ComputeStatistics(datasets, Iterations, OutputFolder);
            }

            Console.WriteLine("Все расчёты завершены!");
        }
        private static void SaveSamplesToFile(double[] data, string filename)
        {
            using (StreamWriter writer = new(filename))
            {
                writer.WriteLine("Value"); // Заголовок
                foreach (var value in data)
                {
                    writer.WriteLine(value);
                }
            }
            Console.WriteLine($"Выборка сохранена: {filename}");
        }

        private static void ComputeStatistics(Distribution[] datasets, int size, string outputFolder)
        {
            string resultFile = $"{outputFolder}/Statistics_{size}.csv";
            using (StreamWriter writer = new(resultFile))
            {
                writer.WriteLine("Distribution;Average;Variance;Median;zQ");

                for (int i = 0; i < datasets.Length; i++)
                {
                    var sample = datasets[i];

                    double mean = sample.Data.Average();
                    double variance = sample.Data.Select(x => x * x).Average() - mean * mean;
                    double median = GetMedian(sample.Data);
                    double zQ = GetQuartileMean(sample.Data);

                    writer.WriteLine($"{sample.GetType().Name};{mean};{variance};{median};{zQ}");
                }
            }
            Console.WriteLine($"Файл со статистикой сохранён: {resultFile}");
        }

        private static double GetMedian(double[] data)
        {
            int n = data.Length;
            var sorted = data.OrderBy(x => x).ToArray();
            return (n % 2 == 0) ? (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0 : sorted[n / 2];
        }

        private static double GetQuartileMean(double[] data)
        {
            int n = data.Length;
            var sorted = data.OrderBy(x => x).ToArray();
            return (sorted[n / 4] + sorted[3 * n / 4]) / 2.0;
        }
    }

}