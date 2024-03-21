using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;


using System.Web.Http.Description;

using System.Data.Entity.Validation;

using System.Web.Http.ModelBinding;

using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using System.IO;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;
using Formatting = Newtonsoft.Json.Formatting;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using ApiScheduling.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using Spire.Xls;
using ApiScheduling.ViewModel;
using System.Diagnostics;
using System.Globalization;

namespace ApiScheduling.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RecencyController : ApiController
    {
        [Route("api/cockpit")]
        public IHttpActionResult GetCockpit()
        {
            var context = new Models.dbEntities();
            var query = from x in context.ViewEmployeeLights
                        where x.InActive==false && ( x.JobGroup=="TRE" || x.JobGroup == "TRI" || x.JobGroup == "P1" || x.JobGroup == "P2")
                        orderby x.JobGroup, x.LastName, x.FirstName 
                        select x;

            var result = query.ToList();
            return Ok(result);

        }
        [Route("api/recency/crew/all")]
        public IHttpActionResult GetRecencyCrewAll()
        {
            var context = new Models.dbEntities();
            var query = from x in context.view_recency_route
                        where x.Rank==1 && (x.JobGroup == "TRE" || x.JobGroup == "TRI" || x.JobGroup == "P1" || x.JobGroup == "P2")
                        orderby x.JobGroup,x.LastName,x.FirstName,x.RemainExpire descending
                        select x;

            var result = query.ToList();
            return Ok(result);

        }
        [Route("api/recency/crew/{id}")]
        public IHttpActionResult GetRecencyCrew(int id)
        {
            var context = new Models.dbEntities();
            var query = from x in context.view_recency_route
                        where x.crew_id==id && x.Rank==1
                        orderby  x.RemainExpire descending
                        select x;

            var result = query.ToList();
            return Ok(result);

        }

        [Route("api/recency/route/{route}")]
        public IHttpActionResult GetRecencyRoute(string route)
        {
            var context = new Models.dbEntities();
            var query = from x in context.view_recency_route
                        where x.RouteIATA==route && x.Rank == 1 && (x.JobGroup == "TRE" || x.JobGroup == "TRI" || x.JobGroup == "P1" || x.JobGroup == "P2")
                        orderby  x.RemainExpire descending,x.LastName,x.FirstName
                        select x;

            var result = query.ToList();
            return Ok(result);

        }

        [Route("api/recency/routes/")]
        public IHttpActionResult GetRoutes()
        {
            var context = new Models.dbEntities();
            var query = from x in context.view_route_count
                       
                        orderby x.FltCount descending, x.FromIATA,x.ToIATA
                        select x;

            var result = query.ToList();
            return Ok(result);

        }





    }
}
