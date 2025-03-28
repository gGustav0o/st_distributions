﻿using ScottPlot.Statistics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

namespace st_distributions.Distributions
{
    public abstract class Distribution
    {
        public Distribution(int size, IUnivariateDistribution distr, string latexFormula)
        {
            Size = size;
            _distr = distr;
            LatexFormula = latexFormula;
        }
        public double[] Data { get; protected set; }
        public abstract double[] GetXs(double step);
        public abstract double[] GetYs(double[] x, Histogram hist);
        protected readonly IUnivariateDistribution _distr;
        public int Size {  get; private set; }
        public double StdDev() => _distr.StdDev;
        public string LatexFormula {  get; private set; }
        protected abstract double Scale(Histogram hist);
        protected static readonly SystemRandomSource Rand = new();
    }
}
