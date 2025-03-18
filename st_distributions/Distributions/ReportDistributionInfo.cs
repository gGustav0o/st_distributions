using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st_distributions.Distributions
{
    public class ReportDistributionInfo
    {
        public string Name { get; set; }
        public string LatexFormula { get; set; }
        // <SampleSize, GraphPath>
        public List<Tuple<int, string>> Samples { get; set; } = [];
    }
}
