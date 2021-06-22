using System.Collections.Generic;
using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class MetricsReport:IReportable<Metric>
    {
        public int TimeSpentToRequest { get; set; }
        public int CountOfRequests { get; set; }
        public IEnumerable<Metric> ReportCollection{ get; set; }
    }

    public class MetricWithCountOfRequests
    {
        public IEnumerable<Counter> CountOfRequestsToEachSearchingSystem { get; set; }
        public IEnumerable<MetricsReport> MetricsReports { get; set; }

    }
    public class Counter
    {
        public int CountOfRequests { get; set; }
        public string NameOfSearchingSystemSystem { get; set; }
    }
}
