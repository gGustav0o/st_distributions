using st_distributions;

class Program
{
    static void Main()
    {
        var results = st_distributions.StatisticsManager.RunAnalysis();
        ReportGenerator.GeneratePdf(results);
    }
}
