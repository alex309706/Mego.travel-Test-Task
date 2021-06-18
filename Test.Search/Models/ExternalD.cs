using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class ExternalD : IRequestable
    {
        public string SearchingSystemName => "External Searching System D";

        public long RequestTime { get; set; }
    }
}
