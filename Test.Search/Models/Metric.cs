using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class Metric : IMetric
    {
        public int MetricID { get; set; }
        public string NameOfSearchingSystem { get; set; }
        public string Result { get; set; }
        public int? TimeSpentToRequest { get; set; }
    }
}
