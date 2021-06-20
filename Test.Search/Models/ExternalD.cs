using System;
using System.Diagnostics;
using System.Threading;
using Test.Search.Interfaces;


namespace Test.Search.Models
{
    public class ExternalD : IRequestable
    {
        public string SearchingSystemName => "External Searching System D";
        public string Request(int minimalExecutionTime, int maximumExecutiontime)
        {

            Random rnd = new Random();
            //для генерации случайного времени выполнения запроса
            int minimalExecutionTimeToSeconds = minimalExecutionTime * 1000;
            int maximumExecutiontimeToSeconds = maximumExecutiontime * 1000;

            int executionTime = rnd.Next(minimalExecutionTimeToSeconds, maximumExecutiontimeToSeconds);

            Thread.Sleep(executionTime);


            int flagToResult = rnd.Next(0, 100);

            if (flagToResult % 2 == 0)
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
