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

           
            
            return $"Result: {MetricsA.Result} Name Of Searching System: {MetricsA.NameOfSearchingSystem} Request Time : {MetricsA.TimeSpentToRequest} Wait : { wait} RandomMin:{randomMin} RandomMax : {randomMax}";
        }

    }
}
