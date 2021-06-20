using System.Threading;

namespace Test.Search.Interfaces
{
    public interface IRequestable
    {
        public string SearchingSystemName { get;}
        public long RequestTime { get; set;}
        public string Request(int minimalExecutionTime, int maximalExecutionTime, CancellationToken token);
       
    }
}
