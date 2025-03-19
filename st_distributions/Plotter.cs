
using MathNet.Numerics.Statistics;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.Statistics;
using st_distributions.Distributions;
using System.Data;

namespace st_distributions
{
    public static class Plotter
    {
        public static void PlotHistogram(Distribution distribution, string filename, int size)
        {
            var plt = new Plot();

            int barsCount = 0;
            if(size <= 200)
            {
                barsCount = (int)(1.0 + Math.Log2(size));
            }
            else
            {
                barsCount = (int)(2.0 * Math.Pow(size, 1.0 / 3.0));
            }

            var hist = ScottPlot.Statistics.Histogram.WithBinCount(barsCount, distribution.Data);
            BarPlot barPlot = plt.Add.Bars(hist.Bins, hist.GetProbability());

            foreach (Bar bar in barPlot.Bars)
            {
                bar.Size = hist.FirstBinSize;
                bar.LineWidth = 0;
                bar.FillStyle.AntiAlias = false;
                bar.FillColor = Colors.C0.Lighten(.3);
            }

            double[] xs = distribution.GetXs(.001);
            double[] ys = distribution.GetYs(xs, hist);

            Scatter curve = plt.Add.ScatterLine(xs, ys);
            curve.LineWidth = 2;
            curve.LineColor = Colors.Black;
            curve.LinePattern = LinePattern.DenselyDashed;

            plt.Axes.Margins(bottom: 0);
            plt.SavePng(filename, 800, 500);
        }
        public static void PlotBoxPlot(List<Distribution> data, string outputFile)
        {
            var plt = new Plot();

            plt.Axes.Title.Label.Text = data[0].GetType().Name;

            double[] tickPositions = Generate.Consecutive(data.Count);
            string[] ticks = new string[data.Count];

            for (int i = 0; i < data.Count; i++)
            {

                Console.WriteLine($"Q1: {data[i].Data.Percentile(25)}");
                Console.WriteLine($"Q3: {data[i].Data.Percentile(75)}");
                double minValue = data[i].Data.Min();
                double maxValue = data[i].Data.Max();
                Console.WriteLine($"Min: {minValue}, Max: {maxValue}");

                var population = plt.Add.Population(data[i].Data, x: i);

                population.MarkerAlignment = HorizontalAlignment.Center;
                population.BarAlignment = HorizontalAlignment.Center;

                population.IsVisible = true;
                population.Bar.IsVisible = true;
                population.Box.IsVisible = true;

                //population.Bar.LineWidth = 2;
                //population.Width = 0.5;
                //plt.Add.HorizontalLine(data[i].Data.Median());
                //plt.Add.HorizontalLine(data[i].Data.Average());
                //plt.Axes.Margins(bottom: 0);
                ticks[i] = data[i].Size.ToString();
            }

            plt.Axes.Bottom.SetTicks(tickPositions, ticks);

            plt.SavePng(outputFile, 800, 500);
        }
    }
}
