using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class Metric : IMetric
    {
        public int MetricID { get; set; }
        public IRequestable RequestableSystem { get; set; }
        public string NameOfSearchingSystem { get; set; }
        public string Result { get; set; }
        public long TimeSpentToRequest { get; set; }
    }
}
