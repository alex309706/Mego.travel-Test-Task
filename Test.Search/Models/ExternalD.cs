using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class ExternalD : IRequestable, IExternable
    {
        public string SearchSystemName => "External Searching System D";
    }
}
