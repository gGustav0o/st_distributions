
namespace st_distributions
{
    public static class Plotter
    {
        public static void PlotHistogram(double[] data, string filename, string title, int size)
        {
            var plt = new ScottPlot.Plot();

            var histogram = ScottPlot.Statistics.Histogram.WithBinCount(20, data);

            plt.Add.Bars(histogram.Bins, histogram.Counts);

            plt.Title($"{title} (n={size})");
            plt.XLabel("Значения");
            plt.YLabel("Частота");

            Directory.CreateDirectory("Results");

            plt.SavePng($"Results/{filename}", 600, 400);
        }
    }
}
