
using MathNet.Numerics.Distributions;
using ScottPlot;
using ScottPlot.Plottables;
using System.Collections.Generic;

namespace st_distributions
{
    public static class Plotter
    {
        public static void PlotHistogram(IUnivariateDistribution distribution, string filename, int size)
        {
            var plt = new ScottPlot.Plot();

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

            var hist = ScottPlot.Statistics.Histogram.WithBinCount(barsCount, distribution.Data);
            var barPlot = plt.Add.Bars(hist.Bins, hist.GetProbability());

            foreach (var bar in barPlot.Bars)
            {
                bar.Size = hist.FirstBinSize;
                bar.LineWidth = 0;
                bar.FillStyle.AntiAlias = false;
                bar.FillColor = Colors.C0.Lighten(.3);
            }

            double[] xs = distribution.GetXs(0.01);
            double[] ys = distribution.GetYs(xs, 1.0);

            var curve = plt.Add.ScatterLine(xs, ys);
            curve.LineWidth = 2;
            curve.LineColor = Colors.Black;
            curve.LinePattern = LinePattern.DenselyDashed;

            plt.Axes.Margins(bottom: 0);
            plt.SavePng($"{filename}", 800, 500);
        }
    }
}
