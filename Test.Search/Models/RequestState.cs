using Test.Search.Interfaces;

namespace Test.Search.Models
{

    //для отслеживания состояния запроса
    class RequestState
    {
        public enum RequestStatus
        {
            Initialized = 0,
            Running = 1,
            Completed = 2,
        };
        public int? ExecutionTime { get; set; }
        public IRequestable System { get; set; }
        public RequestStatus Status { get; set; }
        public string Result { get; set; }
    }
}
