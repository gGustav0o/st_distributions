using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.Statistics;
using st_distributions.Distributions;

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

            Histogram hist = Histogram.WithBinCount(barsCount, distribution.Data);
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
            plt.SavePng($"{filename}", 800, 500);
        }
    }
}
