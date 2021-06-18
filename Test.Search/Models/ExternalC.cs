using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class ExternalC : IRequestable
    {

        public string SearchingSystemName => "External Searching System C";

        public long RequestTime { get; set; }
    }
}
