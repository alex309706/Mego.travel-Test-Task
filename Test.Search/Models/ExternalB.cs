using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class ExternalB : IRequestable, IExternable
    {
        public string SearchSystemName => "External Searching System B";
    }
}
