using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;

namespace ApiLogDefault.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApiLogDefaultMainController : ApiController
    {
        [Route("api/test")]
        [AcceptVerbs("GET")]
        public IHttpActionResult Test1()
        {


            return Ok("test1");
        }

        [Route("api/flight/crews/{id}")]
        [AcceptVerbs("GET")]
        //[CustomAuthorized]
        public IHttpActionResult GetFlightCrews(int id)
        {
            try
            {
                var reqkey = System.Web.HttpContext.Current.Request.Headers.Get("Reqkey");
                var reqname = System.Web.HttpContext.Current.Request.Headers.Get("Reqname");
                var reqroles = System.Web.HttpContext.Current.Request.Headers.Get("Reqroles");
                if (string.IsNullOrEmpty(reqname))
                {

                    return Ok(new List<Models.XFlightCrew> { new Models.XFlightCrew() { Name = "Refresh by F5" } });
                }
                if (string.IsNullOrEmpty(reqroles))
                {

                    return Ok(new List<Models.XFlightCrew>());
                }
                var roles = reqroles.Split(',').Select(q => q.ToLower().Trim()).ToList();
                if (roles.Count == 0)
                {
                    return Ok(new List<Models.XFlightCrew>());
                }
                var role = roles.Where(q => q == "show flight crew" || q == "show flight crews"
              || q == "crew scheduling"
                || q == "crew scheduler"
            || q == "dispatch"
            || q == "ceo"

                ).ToList();
                if (role.Count == 0)
                {
                    var username = reqname.ToLower();
                    if (username != "khanjani" && username != "kermanshahi" && username != "khanjani")
                        return Ok(new List<Models.XFlightCrew>());
                }

                //   var userData = User.FindFirst(ClaimTypes.UserData).Value;
                var _context = new Models.dbEntities();
                var result = _context.XFlightCrews.Where(q => q.FlightId == id && q.IsConfirmed == true).OrderBy(q => q.IsPositioning).ThenBy(q => q.GroupOrder).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER:" + ex.InnerException.Message;
                return Ok(new List<Models.XFlightCrew> { new Models.XFlightCrew() { Name = msg } });
            }

        }


        

        [Route("api/flight/crews/admin/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightCrewsAdmin(int id)
        {
            try
            {
                var _context = new Models.dbEntities();
                var result = _context.XFlightCrews.Where(q => q.FlightId == id).OrderBy(q => q.IsPositioning).ThenBy(q => q.GroupOrder).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER:" + ex.InnerException.Message;
                return Ok(new List<Models.XFlightCrew> { new Models.XFlightCrew() { Name = msg } });
            }

        }

    }

    public class CustomAuthorized : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {

            //  Microsoft.AspNet.Identity.



        }
    }
}
