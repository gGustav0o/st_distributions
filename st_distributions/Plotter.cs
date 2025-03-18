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
            //else
            //{
            //    var sorted = data.OrderBy(x => x).ToArray();
            //    double dQ3 = sorted[(int)(3.0 * size / 4.0)];
            //    double dQ1 = sorted[(int)(size / 4.0)];
            //    double dIqr = dQ3 - dQ1;
            //    double dBinWidth = (int)(2.0 * (dQ3 - dQ1) / Math.Pow(size, 1.0 / 3.0));
            //    barsCount = (int)((sorted.Max() - sorted.Min()) / dBinWidth);
            //}

            Histogram hist = Histogram.WithBinCount(barsCount, distribution.Data);
            BarPlot barPlot = plt.Add.Bars(hist.Bins, hist.GetProbability());

            foreach (Bar bar in barPlot.Bars)
            {
                bar.Size = hist.FirstBinSize;
                bar.LineWidth = 0;
                bar.FillStyle.AntiAlias = false;
                bar.FillColor = Colors.C0.Lighten(.3);
            }

            double[] xs = distribution.GetXs(0.01);
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
