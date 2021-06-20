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
            int waitTimeToMilliseconds = wait * 1000;
            //задача на прекращение ожидания
            Task TaskToCancelWaitingRequests = Task.Run(() =>
            {
                Thread.Sleep(waitTimeToMilliseconds);
                source.Cancel();
            });

            return await Task.Run(() =>
            {
                Task<RequestState> MakeRequestToSystemA = Task.Run(async() =>
                {
                    RequestState requestState = new RequestState();
                    requestState.System = A;
                    requestState.Status = RequestState.RequestStatus.Initialized;
                    //метрика на начало выполнения запроса
                    Metric InitialStateOfMetric = MakeMetric(requestState);
                    MetricsFromCurrentRequest.Add(InitialStateOfMetric);

                    //подсчет времени выполнения
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    requestState.Result = await GetRequestResult(A, randomMin, randomMax);
                    sw.Stop();
                    //приведение к секундам
                    if (token.IsCancellationRequested!=true)
                    {
                        requestState.ExecutionTime = (int)sw.ElapsedMilliseconds / 1000;
                        requestState.Status = RequestState.RequestStatus.Completed;
                    }
                    else
                    {
                        requestState.ExecutionTime = waitTimeToMilliseconds;
                        Metric metricTochange = MetricsFromCurrentRequest.FirstOrDefault(metric => metric.MetricID == InitialStateOfMetric.MetricID);
                        metricTochange.TimeSpentToRequest = requestState.ExecutionTime;
                    }
                    return requestState;
                },token);
                if(MakeRequestToSystemA.Result.Status==RequestState.RequestStatus.Completed)
                {
                    Task continuationTaskToWriteMetricForSystemA = MakeRequestToSystemA.ContinueWith((prevTask) => MetricsFromCurrentRequest.Add(MakeMetric(MakeRequestToSystemA.Result)));
                }

              



                //Task<string> MakeRequestToSystemB = Task.Run(() => GetRequestResult(B, randomMin, randomMax), token);
                //Task<string> MakeRequestToSystemC = Task.Run(() => GetRequestResult(C, randomMin, randomMax), token);
                //Task continuationTaskToWriteMetricForSystemA = MakeRequestToSystemA.ContinueWith((prevTask) => MetricStorage.Create(MakeMetric(A, MakeRequestToSystemA.Result)));
                //Task continuationTaskToWriteMetricForSystemB = MakeRequestToSystemB.ContinueWith((prevTask) => MetricStorage.Create(MakeMetric(B, MakeRequestToSystemB.Result)));
                //Task continuationTaskToWriteMetricForSystemC = MakeRequestToSystemC.ContinueWith((prevTask) => MetricStorage.Create(MakeMetric(C, MakeRequestToSystemC.Result)));


                //ждем выполнения запросов
                Task.WaitAll(new[] { MakeRequestToSystemA });
                //Task.WaitAll(new[] { MakeRequestToSystemA, MakeRequestToSystemB, MakeRequestToSystemC });
                foreach (var metricFromCurrentRequest in MetricsFromCurrentRequest)
                {
                    MetricStorage.Add(metricFromCurrentRequest);
                }
               
                return MetricsFromCurrentRequest;
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
        private Task<string> GetRequestResult(IRequestable SearchingSystem, int randomMin, int randomMax)
        {
            return Task.Run(()=>SearchingSystem.Request(randomMin, randomMax));
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
