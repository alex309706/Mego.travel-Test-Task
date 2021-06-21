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
        //Создать api-метод GET Search
        //Данный метод вызывает параллельно запросы в системах A, B, C.
        //Если система C вернула результат OK, тогда отправляем запрос еще и в систему D.
        //URL-параметры : Search? wait = X & randomMin = Y & randomMax = Z
        //Х – время, в течение которого метод Search ждет ответы от всех внешних систем.Если система не ответила вовремя, мы не ждем ее результат.
        //Общая длительность выполнения Search не должна превышать X более чем на 100 мс.
        //Y – минимальное значения диапазона Random(). Передается в метод Request системы.
        //Z – максимальное значение диапазона Random().Передается в метод Requestсистемы.
        //Метод Search должен вернуть: Список всех систем, с данными по каждой системе: название, результат выполнения запроса OK/ERROR/TIMEOUT,
        //сколько это заняло времени в миллисекундах
        [Route("/api/[controller]/Search")]
        [HttpGet]
        public async Task<IEnumerable<Metric>> Search(int wait, int randomMin, int randomMax)
        {
            //список состояний запроса, на основании которых будем создавать метрики по текущему запросу
            List<RequestState> RequestsStates = new List<RequestState>();
            //список метрик из текущего запроса
            List<Metric> MetricsFromCurrentRequest = new List<Metric>();
            //токен для прекращения ожидания результатов запроса...Пока не разобрался как его правильно использовать.
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            int waitTimeToMilliseconds = wait * 1000;
            //задача на прекращение ожидания
            Task TaskToCancelWaitingRequests = Task.Run(() =>
            {
                Thread.Sleep(waitTimeToMilliseconds);
                source.Cancel();
            });

            //Получилось громоздко, но я пока не знаю как это отрефакторить
            await Task.Run(() =>
            {
                Task<RequestState> MakeRequestToSystemA = Task.Run(async () =>
                {
                    RequestState requestState = new RequestState();
                    requestState.System = A;
                    requestState.Status = RequestState.RequestStatus.Initialized;
                    requestState.ExecutionTime = waitTimeToMilliseconds;
                    RequestsStates.Add(requestState);
                    //подсчет времени выполнения
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    requestState.Result = await GetRequestResult(A, randomMin, randomMax);
                    sw.Stop();
                    //приведение к секундам
                    if (token.IsCancellationRequested != true)
                    {
                        requestState.ExecutionTime = (int)sw.ElapsedMilliseconds;
                        requestState.Status = RequestState.RequestStatus.Completed;
                    }
                    else
                    {
                        requestState.ExecutionTime = waitTimeToMilliseconds;
                        RequestState requestStateToChange = RequestsStates.FirstOrDefault(requestState => requestState.System == A);
                        requestStateToChange.ExecutionTime = requestState.ExecutionTime;
                    }
                    return requestState;
                }, token);
                Task<RequestState> MakeRequestToSystemB = Task.Run(async () =>
                {
                    RequestState requestState = new RequestState();
                    requestState.System = B;
                    requestState.Status = RequestState.RequestStatus.Initialized;
                    requestState.ExecutionTime = waitTimeToMilliseconds;
                    RequestsStates.Add(requestState);
                    //подсчет времени выполнения
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    requestState.Result = await GetRequestResult(B, randomMin, randomMax);
                    sw.Stop();
                    //приведение к секундам
                    if (token.IsCancellationRequested != true)
                    {
                        requestState.ExecutionTime = (int)sw.ElapsedMilliseconds;
                        requestState.Status = RequestState.RequestStatus.Completed;
                    }
                    else
                    {
                        requestState.ExecutionTime = waitTimeToMilliseconds;
                        RequestState requestStateToChange = RequestsStates.FirstOrDefault(requestState => requestState.System == B);
                        requestStateToChange.ExecutionTime = requestState.ExecutionTime;
                    }
                    return requestState;
                }, token);
                Task<RequestState> MakeRequestToSystemC = Task.Run(async () =>
                {
                    RequestState requestState = new RequestState();
                    requestState.System = C;
                    requestState.Status = RequestState.RequestStatus.Initialized;
                    requestState.ExecutionTime = waitTimeToMilliseconds;
                    RequestsStates.Add(requestState);
                    //подсчет времени выполнения
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    requestState.Result = await GetRequestResult(C, randomMin, randomMax);
                    sw.Stop();
                    //приведение к секундам
                    if (token.IsCancellationRequested != true)
                    {
                        requestState.ExecutionTime = (int)sw.ElapsedMilliseconds;
                        requestState.Status = RequestState.RequestStatus.Completed;
                    }
                    else
                    {
                        requestState.ExecutionTime = waitTimeToMilliseconds;
                        RequestState requestStateToChange = RequestsStates.FirstOrDefault(requestState => requestState.System == C);
                        requestStateToChange.ExecutionTime = requestState.ExecutionTime;
                    }
                    return requestState;
                }, token);
                if (MakeRequestToSystemC.Result.Result == "OK")
                {
                    Task<RequestState> MakeRequestToSystemD = Task.Run(async () =>
                    {
                        RequestState requestState = new RequestState();
                        requestState.System = D;
                        requestState.Status = RequestState.RequestStatus.Initialized;
                        requestState.ExecutionTime = waitTimeToMilliseconds;
                        RequestsStates.Add(requestState);
                        //подсчет времени выполнения
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        requestState.Result = await GetRequestResult(D, randomMin, randomMax);
                        sw.Stop();
                        //приведение к секундам
                        if (token.IsCancellationRequested != true)
                        {
                            requestState.ExecutionTime = (int)sw.ElapsedMilliseconds;
                            requestState.Status = RequestState.RequestStatus.Completed;
                        }
                        else
                        {
                            requestState.ExecutionTime = waitTimeToMilliseconds;
                            RequestState requestStateToChange = RequestsStates.FirstOrDefault(requestState => requestState.System == D);
                            requestStateToChange.ExecutionTime = requestState.ExecutionTime;
                        }
                        return requestState;
                    }, token);
                    //костыль для возможности записать результат для системы D
                    MakeRequestToSystemD.Wait();
                }
                //ждем выполнения запросов
                Task.WaitAll(MakeRequestToSystemA, MakeRequestToSystemB, MakeRequestToSystemC);
                
            });

            //заполнение метрик
            return await Task.Run(() =>
            {
                foreach (var requestState in RequestsStates)
                {
                    MetricsFromCurrentRequest.Add(MakeMetric(requestState));
                }
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
            newMetric.TimeSpentToRequest = requestState.ExecutionTime == null ? 0 : requestState.ExecutionTime;
            return newMetric;
        }
        //задача на получение ответа от системы
        private Task<string> GetRequestResult(IRequestable SearchingSystem, int randomMin, int randomMax)
        {
            return Task.Run(() => SearchingSystem.Request(randomMin, randomMax));
        }


        //Создать api-метод GET Metrics, который должен выводить отчет: 
        // Посекундная группировка(1 сек, 2 сек, ...) и названию системы, количество запросов внутри этой группы.
        //Возвращать ответ в любом формате.
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
}
