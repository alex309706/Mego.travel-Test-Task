using System;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Search.Interfaces
{
    interface IRequestable
    {
        public async Task<string> Request (int minimalExecutionTime, int maximumExecutiontime,CancellationToken token)
        {

            if (token.IsCancellationRequested)
                return "TIMEOUT";

            return await Task.Run(() => SyncRequest(minimalExecutionTime, maximumExecutiontime,token));
        }

        public string SyncRequest(int minimalExecutionTime, int maximumExecutiontime, CancellationToken token)
        {

            Random rnd = new Random();
            //для генерации случайного времени выполнения запроса
            int executionTime = rnd.Next(minimalExecutionTime, maximumExecutiontime);

            int executionTimeToSeconds = executionTime * 1000;
            Thread.Sleep(executionTimeToSeconds);

            //проверка ожидания данных от запроса
            if (token.IsCancellationRequested)
            {
                return "TIMEOUT";
            }
            int flagToResult = rnd.Next(0, 1);

            if (flagToResult > 0)
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
