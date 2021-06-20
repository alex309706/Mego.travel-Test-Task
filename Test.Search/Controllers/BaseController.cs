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
        IStorage<Metric> MetricStorage;
        public BaseController(IStorage<Metric> inputRealisationOfStorage)
        {
            MetricStorage = inputRealisationOfStorage;
        }

        [Route("/api/[controller]/Search")]
        [HttpGet]
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

            return await Task.Run(async()=>
            {
                //запрос к системе A
                Task<Metric> MakeRequestToSystemA =  Task.Run(() => MakeMetric(A,randomMin,randomMax,token));
                //запись метрики
                Task continuationTaskToWriteMetricForSystemA = MakeRequestToSystemA.ContinueWith((prevTask) => MetricStorage.Create(MakeRequestToSystemA.Result));

                //запрос к системе B
                Task<Metric> MakeRequestToSystemB = Task.Run(()=> MakeMetric(B, randomMin, randomMax, token));
                //запись метрики
                Task continuationTaskToWriteMetricForSystemB = MakeRequestToSystemB.ContinueWith((prevTask) => MetricStorage.Create(MakeRequestToSystemB.Result));

                //запрос к системе C
                Task<Metric> MakeRequestToSystemC = Task.Run(() => MakeMetric(C, randomMin, randomMax, token));
                //запись метрики
                Task continuationTaskToWriteMetricForSystemC = MakeRequestToSystemC.ContinueWith((prevTask) => 
                {
                    Metric resultFromSearchingSystemC = MakeRequestToSystemC.Result;
                    MetricStorage.Create(resultFromSearchingSystemC);
                    if(resultFromSearchingSystemC.Result=="OK")
                    {
                        //запрос к системе D
                        Task<Metric> MakeRequestToSystemD = Task.Factory.StartNew(() => MakeMetric(D, randomMin, randomMax, token));
                        //запись метрики
                        Task continuationTaskToWriteMetricForSystemD = MakeRequestToSystemD.ContinueWith((prevTask) => MetricStorage.Create(MakeRequestToSystemD.Result));
                    }
                });
                await Task.WhenAll(new [] { MakeRequestToSystemA, MakeRequestToSystemB,MakeRequestToSystemC });
                return MetricStorage;
            });
        }

        private Metric MakeMetric(IRequestable SearchingSystem, int randomMin, int randomMax,CancellationToken token)
        {
            Metric newMetric = new Metric();
            newMetric.RequestableSystem = SearchingSystem;
            newMetric.NameOfSearchingSystem = SearchingSystem.SearchingSystemName;
            newMetric.Result = SearchingSystem.Request(randomMin, randomMax, token);
            newMetric.TimeSpentToRequest = SearchingSystem.RequestTime;
            return newMetric;
        }

        [Route("/api/[controller]/Metrics")]
        [HttpGet]
        public IEnumerable<Report> Metrics()
        {
            //группировка по секундам осуществляется путем преобразования мс в сек (/1000) и округления до ближайшего целого с помощью Math.Round
            //2 приведения типа...Скорее всего,можно улучшить
            var Reports = MetricStorage.GroupBy(metric => (int)Math.Round((double)metric.TimeSpentToRequest/1000))
                .Select(group => new Report
                {
                    TimeSpentToRequest = group.Key, 
                    CountOfRequests= group.Count(),
                    ReportCollection = group.Select(metric=>metric)
                });
            return Reports;
        }
    }
}
