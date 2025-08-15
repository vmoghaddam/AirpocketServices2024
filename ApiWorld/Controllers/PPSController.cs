using ApiWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiWorld.Controllers
{
    public class PPSController : ApiController
    {
        [Route("api/pps/test")]
        public async Task<IHttpActionResult> GetPPSTest()
        {
            try
            {
               pps.EfbService pps_ins=new pps.EfbService();
               var session_id= pps_ins.GetSessionID("ArmeniaAirways", "J8V14HNHK", "AMWINTEGRATION", "i$qn719e");
                var flts = pps_ins.GetSTDFlightList(session_id, new DateTime(2025, 7, 27), new DateTime(2025, 7, 28));
                pps.Flight flt_info = pps_ins.GetFlight(session_id, flts.Items[0].ID, true, true, true, true, "kg");
                var atc = pps_ins.GetATC(session_id, flts.Items[0].ID);
                var apts = pps_ins.GetFlightAirports(session_id, flts.Items[0].ID);
                var wx = pps_ins.GetWX(session_id, flts.Items[0].ID,false);
                var a1 = pps_ins.GetArinc633FlightLog(session_id, flts.Items[0].ID, "kg");
                var flt_id = flts.Items[0].ID;
                //var a2 = pps_ins.GetArinc633WBACommon(session_id, flt_id, "kg");
                //var a3 = pps_ins.GetArinc633FlightPlanAtcIcao(session_id, flt_id);
                //var a4 = pps_ins.GetEff_FullPackage(session_id, flt_id, "kg");
                var a5 = pps_ins.GetFlightDocumentsMeta(session_id, flt_id);
                var a6 = pps_ins.GetRSChart(session_id, flt_id, true, true, true, true, true, true, true);
                var a7 = pps_ins.GetAirport(session_id, "OIMS");
                return Ok(session_id);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }

        }
    }
}
