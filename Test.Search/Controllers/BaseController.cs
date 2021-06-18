using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Test.Search.Interfaces;
using Test.Search.Models;

namespace Test.Search.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        IRequestable A = new ExternalA();
        IRequestable B = new ExternalB();
        IRequestable C = new ExternalC();
        IRequestable D = new ExternalD();

        [Route("/api/[controller]/Search")]
        public async Task <string> Search(int wait, int randomMin, int randomMax)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            Task TaskToCancelRequest = Task.Run(() =>
            {
                int waitTimeToSeconds = wait * 1000;
                Thread.Sleep(waitTimeToSeconds);
                source.Cancel();
            });

            Metrics MetricsA = new Metrics();
            MetricsA.RequestableSystem = A;
            MetricsA.NameOfSearchingSystem = A.SearchingSystemName;
            MetricsA.Result = await A.Request(randomMin, randomMax, token);
            MetricsA.TimeSpentToRequest = A.RequestTime;

            Metrics MetricsB = new Metrics();
            MetricsB.RequestableSystem = B;
            MetricsB.NameOfSearchingSystem = A.SearchingSystemName;
            MetricsB.Result = await A.Request(randomMin, randomMax, token);
            MetricsB.TimeSpentToRequest = A.RequestTime;

            Metrics MetricsC = new Metrics();
            MetricsC.RequestableSystem = C;
            MetricsC.NameOfSearchingSystem = A.SearchingSystemName;
            MetricsC.Result = await A.Request(randomMin, randomMax, token);
            MetricsC.TimeSpentToRequest = A.RequestTime;

            string result = $"Result: {MetricsA.Result} Name Of Searching System: {MetricsA.NameOfSearchingSystem} Request Time : {MetricsA.TimeSpentToRequest} Wait : { wait} RandomMin:{randomMin} RandomMax : {randomMax}\n";
            result += $"Result: {MetricsB.Result} Name Of Searching System: {MetricsB.NameOfSearchingSystem} Request Time : {MetricsB.TimeSpentToRequest} Wait : { wait} RandomMin:{randomMin} RandomMax : {randomMax}\n";
            result += $"Result: {MetricsC.Result} Name Of Searching System: {MetricsC.NameOfSearchingSystem} Request Time : {MetricsC.TimeSpentToRequest} Wait : { wait} RandomMin:{randomMin} RandomMax : {randomMax}";
            return result;
        }
    }
}
