using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task<IEnumerable<Metric>> Search(int wait, int randomMin, int randomMax)
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

            return await Task.Run(() =>
            {
                Task<string> MakeRequestToSystemA = Task.Run(() => GetRequestResult(A, randomMin, randomMax), token);
                Task<string> MakeRequestToSystemB = Task.Run(() => GetRequestResult(B, randomMin, randomMax), token);
                Task<string> MakeRequestToSystemC = Task.Run(() => GetRequestResult(C, randomMin, randomMax), token);
                Task continuationTaskToWriteMetricForSystemA = MakeRequestToSystemA.ContinueWith((prevTask) => MetricStorage.Create(MakeMetric(A, MakeRequestToSystemA.Result)));
                Task continuationTaskToWriteMetricForSystemB = MakeRequestToSystemB.ContinueWith((prevTask) => MetricStorage.Create(MakeMetric(B, MakeRequestToSystemB.Result)));
                Task continuationTaskToWriteMetricForSystemC = MakeRequestToSystemC.ContinueWith((prevTask) => MetricStorage.Create(MakeMetric(C, MakeRequestToSystemC.Result)));


                //ждем выполнения запросов
                Task.WaitAll(new[] { MakeRequestToSystemA, MakeRequestToSystemB, MakeRequestToSystemC });
                return MetricStorage;
            });
        }

        //создание метрики
        private Metric MakeMetric(RequestState requestState)
        {

            Metric newMetric = new Metric();
            newMetric.NameOfSearchingSystem = requestState.System.SearchingSystemName;
            newMetric.Result = requestState.Status != RequestState.RequestStatus.Completed ? "TIMEOUT" : requestState.Result;
            newMetric.TimeSpentToRequest = requestState.ExecutionTime;
            return newMetric;
        }
        private async Task<string> GetRequestResult(IRequestable SearchingSystem, int randomMin, int randomMax)
        {
            //для подсчета времени выполнения запроса
            Stopwatch stopwatchToGetSpentTimeForRequest = new Stopwatch();
            try
            {
                stopwatchToGetSpentTimeForRequest.Start();
                return await Task.Run(() => SearchingSystem.Request(randomMin, randomMax));

            }
            finally
            {
                stopwatchToGetSpentTimeForRequest.Stop();
                //RequestTime = stopwatchToGetSpentTimeForRequest.ElapsedMilliseconds;
            }
          
        }

        [Route("/api/[controller]/Metrics")]
        [HttpGet]
        public IEnumerable<Report> Metrics()
        {
            //группировка по секундам осуществляется путем преобразования мс в сек (/1000) и округления до ближайшего целого с помощью Math.Round
            //2 приведения типа...Скорее всего,можно улучшить
            var Reports = MetricStorage.GroupBy(metric => (int)Math.Round((double)metric.TimeSpentToRequest / 1000))
                .Select(group => new Report
                {
                    TimeSpentToRequest = group.Key,
                    CountOfRequests = group.Count(),
                    ReportCollection = group.Select(metric => metric)
                });
            return Reports;
        }
    }
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
