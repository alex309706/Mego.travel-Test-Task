using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class ExternalB : IRequestable
    {
        public string SearchingSystemName => "External Searching System B";

        public long RequestTime { get; set; }
    }
}
