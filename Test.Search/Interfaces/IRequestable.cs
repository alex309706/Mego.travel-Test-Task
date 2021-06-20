using System.Threading;

namespace Test.Search.Interfaces
{
    public interface IRequestable
    {
        public string SearchingSystemName { get;}
        public string Request(int minimalExecutionTimeInSeconds, int maximalExecutionTimeInSeconds);
       
    }
}
