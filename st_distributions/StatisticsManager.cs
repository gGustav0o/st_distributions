
using MathNet.Numerics.Statistics;
using st_distributions.Distributions;
using Normal = st_distributions.Distributions.Normal;
using Cauchy = st_distributions.Distributions.Cauchy;
using Poisson = st_distributions.Distributions.Poisson;
using System.IO;
using CSharpMath;
using MathNet.Numerics.LinearAlgebra.Solvers;
using System.Data;
using static OpenTK.Graphics.OpenGL.GL;

namespace st_distributions
{
    public class StatisticsManager
    {
        private static readonly int Iterations = 1_000;
        public static readonly string OutputFolder = "Results";
        public static Dictionary<string, ReportDistributionInfo> RunAnalysis()
        {
            Dictionary<string, ReportDistributionInfo> infoDict = [];
            if (Directory.Exists(OutputFolder))
            {
                Directory.Delete(OutputFolder, true);
            }
            Directory.CreateDirectory(OutputFolder);

            int[] SampleSizes = [10, 50, 1000];
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
            }

            Dictionary<string, Dictionary<int, Dictionary<string, List<double>>>> statistics = [];

            SampleSizes = [10, 100, 1000];
            for (var i = 0; i < Iterations; i++)
            {
                foreach (var size in SampleSizes)
                {

                    Distribution[] datasets = [
                        new Normal(0, 1, size),
                        new Cauchy(0.0, 1.0, size),
                        new Poisson(10.0, size),
                        new Uniform(-Math.Sqrt(3.0), Math.Sqrt(3.0), size),
                    ];


                    foreach (var set in datasets)
                    {
                        double mean = set.Data.Average();
                        double variance = Statistics.Variance(set.Data);
                        double median = GetMedian(set.Data);
                        double zQ = GetQuartileMean(set.Data);

                        if(!statistics.ContainsKey(set.GetType().Name))
                        {
                            statistics.Add(set.GetType().Name, []);
                        }
                        if (!statistics[set.GetType().Name].ContainsKey(size))
                        {
                            statistics[set.GetType().Name].Add(size, []);
                        }
                        if (!statistics[set.GetType().Name][size].ContainsKey(nameof(mean)))
                        {
                            statistics[set.GetType().Name][size].Add(nameof(mean), []);
                        }
                        if (!statistics[set.GetType().Name][size].ContainsKey(nameof(variance)))
                        {
                            statistics[set.GetType().Name][size].Add(nameof(variance), []);
                        }
                        if (!statistics[set.GetType().Name][size].ContainsKey(nameof(median)))
                        {
                            statistics[set.GetType().Name][size].Add(nameof(median), []);
                        }
                        if (!statistics[set.GetType().Name][size].ContainsKey(nameof(zQ)))
                        {
                            statistics[set.GetType().Name][size].Add(nameof(zQ), []);
                        }
                        statistics[set.GetType().Name][size][nameof(mean)].Add(mean);
                        statistics[set.GetType().Name][size][nameof(variance)].Add(variance);
                        statistics[set.GetType().Name][size][nameof(median)].Add(median);
                        statistics[set.GetType().Name][size][nameof(zQ)].Add(zQ);
                    }
                }
            }
            foreach (var set in statistics)
            {
                Console.WriteLine(set.Key);
            }

            SaveTables(statistics, $"{OutputFolder}/Statistics");
            
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

        private static void SaveTables(Dictionary<string, Dictionary<int, Dictionary<string, List<double>>>> statistics, string folder)
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);

            foreach (var key in statistics.Keys)
            {
                var dataset = statistics[key];
                string resultFile = $"{folder}/Statistics_{key}.csv";
                using (StreamWriter writer = new(resultFile))
                {
                    string titlesLine = ";" + string.Join(";", dataset.Keys);
                    writer.WriteLine(titlesLine);

                    List<string> lines = [];

                    foreach (var size in dataset.Keys)
                    {

                        Dictionary<string, double> e_st = [];
                        Dictionary<string, double> d_st = [];

                        var set = dataset[size];
                        var variables = set.Keys.ToArray();
                        foreach(var v in variables)
                        {
                            e_st.Add(v, set[v].Average());
                            d_st.Add(v, SqMean(set[v]));
                        }

                        if(lines.IsEmpty())
                        {
                            lines = Enumerable.Repeat("", e_st.Count + d_st.Count).ToList();
                        }

                        for(int i = 0; i < e_st.Count; i++)
                        {
                            if (lines[i].IsEmpty()) { lines[i] = $"E({variables[i]});"; }
                            lines[i] += $"{e_st[variables[i]]};";
                        }
                        for (int i = e_st.Count; i < lines.Count; i++)
                        {
                            int j = i - e_st.Count;
                            if (lines[i].IsEmpty()) { lines[i] = $"D({variables[j]});"; }
                            lines[i] += $"{d_st[variables[j]]};";
                        }
                    }

                    foreach (var l in lines)
                    {
                        writer.WriteLine(l);
                    }
                }
            }

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

        private static double SqMean(IEnumerable<double> data)
            => data.Select(v => v * v).Average() - data.Average() * data.Average();
    }

}