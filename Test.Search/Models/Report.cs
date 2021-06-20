using System.Collections.Generic;
using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class Report:IReportable<Metric>
    {
        public int TimeSpentToRequest { get; set; }
        public int CountOfRequests { get; set; }
        public IEnumerable<Metric> ReportCollection{ get; set; }


    }
}
