using System;
using System.IO;
using System.Linq;

namespace st_distributions
{
    public class StatisticsManager
    {
        private static Random rand = new Random();

        public void RunAnalysis()
        {
            int[] sampleSizes = { 10, 50, 1000 };
            int iterations = 1000;

            string outputFolder = "Results";
            Directory.CreateDirectory(outputFolder);

            foreach (var size in sampleSizes)
            {
                Console.WriteLine($"Обрабатываем выборки размером {size}...");

                double[][] datasets = {
                DataGenerator.GenerateNormalSample(size),
                DataGenerator.GenerateCauchySample(size),
                DataGenerator.GeneratePoissonSample(size),
                DataGenerator.GenerateUniformSample(size)
            };

                string[] names = { "Normal", "Cauchy", "Poisson", "Uniform" };

                for (int i = 0; i < datasets.Length; i++)
                {
                    string filename = $"{names[i]}_{size}.png";
                    Plotter.PlotHistogram(datasets[i], filename, names[i], size);
                    Console.WriteLine($"Сохранен график: {filename}");
                }

                ComputeStatistics(datasets, names, size, iterations, outputFolder);
            }

            Console.WriteLine("Все расчёты завершены!");
        }

        private static void ComputeStatistics(double[][] datasets, string[] names, int size, int iterations, string outputFolder)
        {
            string resultFile = $"{outputFolder}/Statistics_{size}.csv";
            using (StreamWriter writer = new StreamWriter(resultFile))
            {
                writer.WriteLine("Распределение,Среднее,Дисперсия,Медиана,zQ");

                for (int i = 0; i < datasets.Length; i++)
                {
                    double meanSum = 0, varianceSum = 0, medianSum = 0, zQSum = 0;

                    for (int j = 0; j < iterations; j++)
                    {
                        double[] sample = datasets[i].OrderBy(x => rand.Next()).Take(size).ToArray();

                        double mean = sample.Average();
                        double variance = sample.Select(x => x * x).Average() - mean * mean;
                        double median = GetMedian(sample);
                        double zQ = GetQuartileMean(sample);

                        meanSum += mean;
                        varianceSum += variance;
                        medianSum += median;
                        zQSum += zQ;
                    }

                    writer.WriteLine($"{names[i]},{meanSum / iterations},{varianceSum / iterations},{medianSum / iterations},{zQSum / iterations}");
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