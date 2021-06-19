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
        public async Task <IEnumerable<Metric>> Search(int wait, int randomMin, int randomMax)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            Task TaskToCancelRequest = Task.Run(() =>
            {
                int waitTimeToSeconds = wait * 1000;
                Thread.Sleep(waitTimeToSeconds);
                source.Cancel();
            });

            List<Metric> MetricData = new List<Metric>();

            return await Task.Run(async()=>
            {
                //запрос к системе A
                Task<Metric> MakeRequestToSystemA = Task.Run(() => MakeMetric(A,randomMin,randomMax,token));
                //запись метрики
                Task continuationTaskToWriteMetricForSystemA = MakeRequestToSystemA.ContinueWith((prevTask) => MetricData.Add(MakeRequestToSystemA.Result));

                //запрос к системе B
                Task<Metric> MakeRequestToSystemB = Task.Run(()=> MakeMetric(B, randomMin, randomMax, token));
                //запись метрики
                Task continuationTaskToWriteMetricForSystemB = MakeRequestToSystemB.ContinueWith((prevTask) => MetricData.Add(MakeRequestToSystemB.Result));

                //запрос к системе C
                Task<Metric> MakeRequestToSystemC = Task.Run(() => MakeMetric(C, randomMin, randomMax, token));
                //запись метрики
                Task continuationTaskToWriteMetricForSystemC = MakeRequestToSystemC.ContinueWith((prevTask) => 
                {
                    Metric resultFromSearchingSystemC = MakeRequestToSystemC.Result;
                    MetricData.Add(resultFromSearchingSystemC);
                    if(resultFromSearchingSystemC.Result=="OK")
                    {
                        //запрос к системе D
                        Task<Metric> MakeRequestToSystemD = Task.Run(() => MakeMetric(D, randomMin, randomMax, token));
                        //запись метрики
                        Task continuationTaskToWriteMetricForSystemD = MakeRequestToSystemD.ContinueWith((prevTask) => MetricData.Add(MakeRequestToSystemD.Result));
                    }
                });

                await Task.WhenAll(new [] { MakeRequestToSystemA, MakeRequestToSystemB,MakeRequestToSystemC });
                return MetricData;
            });
        }
        private Metric MakeMetric(IRequestable SearchingSystem, int randomMin, int randomMax,CancellationToken token)
        {
            Metric newMetric = new Metric();
            newMetric.RequestableSystem = SearchingSystem;
            newMetric.NameOfSearchingSystem = A.SearchingSystemName;
            newMetric.Result = A.Request(randomMin, randomMax, token);
            newMetric.TimeSpentToRequest = A.RequestTime;
            return newMetric;
        }
    }
}
