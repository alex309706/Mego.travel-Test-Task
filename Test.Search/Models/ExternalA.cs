using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class ExternalA : IRequestable, IExternable
    {
        public string SearchSystemName => "External Searching System A";
    }
}
