using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

namespace st_distributions
{
    class DataGenerator
    {
        private static readonly SystemRandomSource rand = new();

        public static double[] GenerateNormalSample(int size) =>
            Normal.Samples(rand, 0, 1).Take(size).ToArray();

        public static double[] GenerateCauchySample(int size, double location, double scale) =>
            Cauchy.Samples(rand, location, scale).Take(size).ToArray();

        public static double[] GeneratePoissonSample(int size) =>
            Poisson.Samples(rand, 10).Take(size).Select(x => (double)x).ToArray();

        public static double[] GenerateUniformSample(int size, double left, double right) =>
            ContinuousUniform.Samples(rand, left, right).Take(size).ToArray();
    }
}
