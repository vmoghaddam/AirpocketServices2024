using ApiCRM.Models;
using ApiCRM.ViewModels;
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

namespace ApiCRM.Controllers
{
    public class NotificationController : ApiController
    {
        [Route("api/sms/mp/send")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostMPSMS(sms_obj dto)
        {
            ppa_entities context = new ppa_entities();
            MelliPayamak sender = new MelliPayamak();
            var result=sender.send(dto);
            var status = "نامشخص";
            return Ok(new
            {
                result,
                status

            });
        }
    }
}
