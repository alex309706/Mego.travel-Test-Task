using System;
using System.Threading;

namespace Test.Search.Interfaces
{
    interface IRequestable
    {
        public string Request (int minimalExecutionTime, int maximumExecutiontime, out TimeSpan actualExecutionTimeOfRequest)
        {
            DateTime beginOfRequest = DateTime.Now;
            Random rnd = new Random();
            //для генерации случайного времени выполнения запроса
            int executionTime = rnd.Next(minimalExecutionTime,maximumExecutiontime);

            Thread.Sleep(executionTime);

            DateTime endOfRequest = DateTime.Now;
            actualExecutionTimeOfRequest = endOfRequest - beginOfRequest;
            int flagToResult = rnd.Next(0, 1);

            if (flagToResult>0)
            {
                return "OK";
            }
            else
            {
                return "ERROR";
            }

        }
    }
}
