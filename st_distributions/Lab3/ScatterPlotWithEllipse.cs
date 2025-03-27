using ScottPlot;
using MathNet.Numerics.LinearAlgebra;

public static class ScatterPlotWithEllipse
{
    public static void PlotWithEllipse(double[] x, double[] y, string title, string fileName = "outputLab3.png")
    {
        var plt = new Plot();
        plt.Title(title);
        plt.XLabel("X");
        plt.YLabel("Y");

        var sp = plt.Add.Scatter(x, y);
        AddConfidenceEllipse(plt, x, y);

        plt.SavePng(fileName, 600, 800);
    }

    private static void AddConfidenceEllipse(ScottPlot.Plot plt, double[] x, double[] y, double confidence = 0.95)
    {
        int n = x.Length;

        double mx = x.Average();
        double my = y.Average();

        var dx = x.Select(xi => xi - mx).ToArray();
        var dy = y.Select(yi => yi - my).ToArray();

        double sxx = dx.Zip(dx, (a, b) => a * b).Sum() / (n - 1);
        double syy = dy.Zip(dy, (a, b) => a * b).Sum() / (n - 1);
        double sxy = dx.Zip(dy, (a, b) => a * b).Sum() / (n - 1);

        var cov = Matrix<double>.Build.DenseOfArray(new double[,]
        {
            { sxx, sxy },
            { sxy, syy }
        });

        var evd = cov.Evd();
        var eigenValues = evd.D.Diagonal().ToArray();
        var eigenVectors = evd.EigenVectors;

        double chi2 = 5.991; // χ²(2, 0.95)
        double a = Math.Sqrt(eigenValues[0] * chi2);
        double b = Math.Sqrt(eigenValues[1] * chi2);

        double angle = Math.Atan2(eigenVectors[1, 0], eigenVectors[0, 0]);

        int segments = 100;
        double[] xs = new double[segments];
        double[] ys = new double[segments];
        for (int i = 0; i < segments; i++)
        {
            double t = 2 * Math.PI * i / segments;
            double ex = a * Math.Cos(t);
            double ey = b * Math.Sin(t);

            xs[i] = mx + ex * Math.Cos(angle) - ey * Math.Sin(angle);
            ys[i] = my + ex * Math.Sin(angle) + ey * Math.Cos(angle);
        }

        plt.Add.Scatter(xs, ys);
    }
}
