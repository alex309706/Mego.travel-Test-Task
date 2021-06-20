using System.Collections.Generic;

namespace Test.Search.Interfaces
{
    interface IReportable<T>
    {
        public int TimeSpentToRequest { get; set; }
        public int CountOfRequests { get; set; }
        public IEnumerable<T> ReportCollection { get; set; }
    }
}
