using ApiGRH.Models;
using ApiGRH.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiGRH.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GRHController : ApiController
    {
    }
}
