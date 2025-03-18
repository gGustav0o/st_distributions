
using MathNet.Numerics.Statistics;
using st_distributions.Distributions;
using Normal = st_distributions.Distributions.Normal;
using Cauchy = st_distributions.Distributions.Cauchy;
using Poisson = st_distributions.Distributions.Poisson;
using System.IO;

namespace st_distributions
{
    public class StatisticsManager
    {
        private static readonly int[] SampleSizes = [10, 50, 100, 1000];
        private const int Iterations = 1000;
        public static readonly string OutputFolder = "Results";
        public static Dictionary<string, ReportDistributionInfo> RunAnalysis()
        {
            Dictionary<string, ReportDistributionInfo> infoDict = [];
            if (Directory.Exists(OutputFolder))
            {
                Directory.Delete(OutputFolder, true);
            }
            Directory.CreateDirectory(OutputFolder);

            foreach (var size in SampleSizes)
            {
                Console.WriteLine($"Process samples of size {size}...");

                Distribution[] datasets = [
                    new Normal(0, 1, size),
                    new Cauchy(0.0, 1.0, size),
                    new Poisson(10.0, size),
                    new Uniform(-Math.Sqrt(3.0), Math.Sqrt(3.0), size),
                ];

                for (int i = 0; i < datasets.Length; i++)
                {
                    string distr = datasets[i].GetType().Name;
                    string dir = $"{OutputFolder}/{distr}";
                    string filename = $"{dir}/{distr}_{size}.png";

                    if (!infoDict.ContainsKey(distr))
                    {
                        infoDict[distr] = new()
                        {
                            Name = distr,
                            LatexFormula = datasets[i].LatexFormula
                        };
                    }
                    infoDict[distr].Samples.Add(new Tuple<int, string>(size, filename));
                    
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    
                    Plotter.PlotHistogram(datasets[i], filename, size);
                    Console.WriteLine($"Сохранен график: {filename}");
                    string sampleFilename = $"{dir}/{distr}_{size}_data.txt";
                    SaveSamplesToFile(datasets[i].Data, sampleFilename);
                }

                ComputeStatistics(datasets, Iterations, OutputFolder);
            }

            Console.WriteLine("Все расчёты завершены!");
            return infoDict;
        }
        private static void SaveSamplesToFile(double[] data, string filename)
        {
            using (StreamWriter writer = new(filename))
            {
                writer.WriteLine("Value");
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
                    double variance = Statistics.Variance(sample.Data);
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