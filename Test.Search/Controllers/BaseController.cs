using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        ExternalB B = new ExternalB();
        ExternalC C = new ExternalC();
        ExternalD D = new ExternalD();

    //    [Route("/api/[controller]/Search")]
    //    public string Search(int wait, int randomMin, int randomMax)
    //    {
    //        //TimeSpan requestTimeForASystem;
    //        //A.Request(randomMin,randomMax, out requestTimeForASystem);
    //        //return $"RequestTime: {requestTimeForASystem}  Wait : { wait} RandomMin:{randomMin} RandomMax : {randomMax}";
    //    }
    //    public string Index()
    //    {
    //        return "Index";
    //    }
    }
}
