using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class ExternalC : IRequestable, IExternable
    {
        public string SearchSystemName => "External Searching System C";
    }
}
