using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class ExternalA : IRequestable
    {
        public string SearchingSystemName => "External Searching System A";

        public long RequestTime { get; set; }
    }
}
