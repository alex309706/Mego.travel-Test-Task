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
        public async Task <IEnumerable<Metrics>> Search(int wait, int randomMin, int randomMax)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            Task TaskToCancelRequest = Task.Run(() =>
            {
                int waitTimeToSeconds = wait * 1000;
                Thread.Sleep(waitTimeToSeconds);
                source.Cancel();
            });

            List<Metrics> MetricsData = new List<Metrics>();

            return await Task.Run(async()=>
            {
                //запрос к системе A
                Task<Metrics> MakeRequestToSystemA = Task.Run(() => MakeMetrics(A,randomMin,randomMax,token));
                //запись метрики
                Task continuationTaskToWriteMetricsForSystemA = MakeRequestToSystemA.ContinueWith((prevTask) => MetricsData.Add(MakeRequestToSystemA.Result));

                //запрос к системе B
                Task<Metrics> MakeRequestToSystemB = Task.Run(()=> MakeMetrics(B, randomMin, randomMax, token));
                //запись метрики
                Task continuationTaskToWriteMetricsForSystemB = MakeRequestToSystemB.ContinueWith((prevTask) => MetricsData.Add(MakeRequestToSystemB.Result));

                Task<Metrics> MakeRequestToSystemC = Task.Run(() => MakeMetrics(C, randomMin, randomMax, token));
                //запись метрики
                Task continuationTaskToWriteMetricsForSystemC = MakeRequestToSystemC.ContinueWith((prevTask) => 
                {
                    Metrics resultFromSearchingSystemC = MakeRequestToSystemC.Result;
                    MetricsData.Add(resultFromSearchingSystemC);
                    if(resultFromSearchingSystemC.Result=="OK")
                    {
                        //запрос к системе D
                        Task<Metrics> MakeRequestToSystemD = Task.Run(() => MakeMetrics(D, randomMin, randomMax, token));
                        //запись метрики
                        Task continuationTaskToWriteMetricsForSystemD = MakeRequestToSystemD.ContinueWith((prevTask) => MetricsData.Add(MakeRequestToSystemD.Result));
                    }
                });

                await Task.WhenAll(new [] { MakeRequestToSystemA, MakeRequestToSystemB,MakeRequestToSystemC });
                return MetricsData;
            });
        }
        private Metrics MakeMetrics(IRequestable SearchingSystem, int randomMin, int randomMax,CancellationToken token)
        {
            Metrics newMetrics = new Metrics();
            newMetrics.RequestableSystem = SearchingSystem;
            newMetrics.NameOfSearchingSystem = A.SearchingSystemName;
            newMetrics.Result = A.Request(randomMin, randomMax, token);
            newMetrics.TimeSpentToRequest = A.RequestTime;
            return newMetrics;
        }
    }
}
