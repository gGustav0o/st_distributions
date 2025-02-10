using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;


namespace st_distributions
{
    class Plotter
    {
        public static void PlotHistogram(double[] data, string filename, string title, int size)
        {
            var plt = new ScottPlot.Plot(600, 400);

            var hist = new ScottPlot.Statistics.Histogram(data, binCount: 20);
            plt.AddBar(hist.counts, hist.binCenters);

            plt.Title($"{title} (n={size})");
            plt.XLabel("Значения");
            plt.YLabel("Частота");

            Directory.CreateDirectory("Results");
            plt.SaveFig($"Results/{filename}");
        }
    }
}
