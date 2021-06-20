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
            List<Metric> MetricsFromCurrentRequest = new List<Metric>();
            //токен для прекращения ожидания результатов запроса
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            
            //задача на прекращение ожидания
            Task TaskToCancelWaitingRequests = Task.Run(() =>
            {
                int waitTimeToSeconds = wait * 1000;
                Thread.Sleep(waitTimeToSeconds);
                source.Cancel();
            });

            return await Task.Run(()=>
            {
                Task<string> MakeRequestToSystemA = Task.Run(() => GetRequestResult(A,randomMin,randomMax),token);
                Task<string> MakeRequestToSystemB = Task.Run(() => GetRequestResult(B, randomMin, randomMax), token);
                Task<string> MakeRequestToSystemC = Task.Run(() => GetRequestResult(C, randomMin, randomMax), token);
                Task continuationTaskToWriteMetricForSystemA = MakeRequestToSystemA.ContinueWith((prevTask) => MetricStorage.Create(MakeMetric(A, MakeRequestToSystemA.Result)));
                Task continuationTaskToWriteMetricForSystemB = MakeRequestToSystemB.ContinueWith((prevTask) => MetricStorage.Create(MakeMetric(B, MakeRequestToSystemB.Result)));
                Task continuationTaskToWriteMetricForSystemC = MakeRequestToSystemC.ContinueWith((prevTask)=> MetricStorage.Create(MakeMetric(C, MakeRequestToSystemC.Result)));


                ////запись метрики
                //Task continuationTaskToWriteMetricForSystemA = MakeRequestToSystemA.ContinueWith((prevTask) => MetricStorage.Create(MakeRequestToSystemA.Result));

                ////запрос к системе B
                //Task<Metric> MakeRequestToSystemB = Task.Run(()=> MakeMetric(B, randomMin, randomMax), token);
                ////запись метрики
                //Task continuationTaskToWriteMetricForSystemB = MakeRequestToSystemB.ContinueWith((prevTask) => MetricStorage.Create(MakeRequestToSystemB.Result));

                ////запрос к системе C
                //Task<Metric> MakeRequestToSystemC = Task.Run(() => MakeMetric(C, randomMin, randomMax),token);
                ////запись метрики
                //Task continuationTaskToWriteMetricForSystemC = MakeRequestToSystemC.ContinueWith((prevTask) => 
                //{
                //    //получение результата из запроса к системе C, чтобы проверить, нужно ли делать запрос к системе D
                //    Metric resultFromSearchingSystemC = MakeRequestToSystemC.Result;
                //    MetricStorage.Create(resultFromSearchingSystemC);
                //    //по условию задания
                //    if(resultFromSearchingSystemC.Result=="OK")
                //    {
                //        //запрос к системе D
                //        Task<Metric> MakeRequestToSystemD = Task.Factory.StartNew(() => MakeMetric(D, randomMin, randomMax));
                //        //запись метрики
                //        Task continuationTaskToWriteMetricForSystemD = MakeRequestToSystemD.ContinueWith((prevTask) => MetricStorage.Create(MakeRequestToSystemD.Result));
                //        MakeRequestToSystemD.Wait();
                //    }
                //});
                //ждем выполнения запросов
                Task.WaitAll(new [] { MakeRequestToSystemA, MakeRequestToSystemB,MakeRequestToSystemC});
                return MetricStorage;
            },token);
        }

        //создание метрики
        private Metric MakeMetric(IRequestable SearchingSystem,string result)
        {
            Metric newMetric = new Metric();
            newMetric.NameOfSearchingSystem = SearchingSystem.SearchingSystemName;
            newMetric.Result = result;
            newMetric.TimeSpentToRequest = SearchingSystem.RequestTime;
            return newMetric;
        }
        private string GetRequestResult(IRequestable SearchingSystem, int randomMin,int randomMax)
        {
          return SearchingSystem.Request(randomMin, randomMax);
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
