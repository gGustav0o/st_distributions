using st_distributions;

class Program
{
    static void Main()
    {
        //var results = st_distributions.StatisticsManager.RunAnalysis();
        //ReportGenerator.GeneratePdf(results);

        //StatisticsManager.RunAnalysisLab2();
        //ReportGenerator.GeneratePdfLab2();

        //st_distributions.Lab3.Manager.Run();

        //st_distributions.Lab4.RegressionComparison.Run();
        var analyzer = new st_distributions.Lab5.NormalityAnalyzer();
        analyzer.Run();
    }

}
