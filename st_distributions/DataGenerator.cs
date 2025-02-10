using MathNet.Numerics.Distributions;

namespace st_distributions
{
    class DataGenerator
    {
        private static Random rand = new Random();

        public static double[] GenerateNormalSample(int size) =>
            Normal.Samples(rand, 0, 1).Take(size).ToArray();

        public static double[] GenerateCauchySample(int size) =>
            Cauchy.Samples(rand, 0, 1).Take(size).ToArray();

        public static double[] GeneratePoissonSample(int size) =>
            Poisson.Samples(rand, 10).Take(size).Select(x => (double)x).ToArray();

        public static double[] GenerateUniformSample(int size) =>
            ContinuousUniform.Samples(rand, -Math.Sqrt(3), Math.Sqrt(3)).Take(size).ToArray();
    }
}
