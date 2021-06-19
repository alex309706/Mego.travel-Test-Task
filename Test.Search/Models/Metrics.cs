using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class Metrics : IMetrics
    {
        public int MetricsID { get; set; }
        public IRequestable RequestableSystem { get; set; }
        public string NameOfSearchingSystem { get; set; }
        public string Result { get; set; }
        public long TimeSpentToRequest { get; set; }
    }
}
