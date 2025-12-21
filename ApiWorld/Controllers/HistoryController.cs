using ApiWorld.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;


namespace ApiWorld.Controllers
{
    public class HistoryController : ApiController
    {



        [Route("api/faranegar/send/request")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveOFPProp(dynamic dto)
        {
            ppa_entities context = new ppa_entities();
            
            var prop = new ofp_prop_history()
            {
                OFPId = dto.ofpId,
                PropName = dto.propName,
                PropValue = dto.propValue,
                PropType = dto.propType,
                IP = dto.ip,
                HardWare = dto.hardware,
                ISP = dto.isp,
                User = dto.user,
                DateCreate = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                Remark = dto.remark,
                Identifier = dto.identifier

        };
            context.ofp_prop_history.Add(prop);

          
            if (!string.IsNullOrEmpty(dto.propValue.Trim().Replace(" ", "")))
                prop.PropValue = dto.propValue;
          
            await context.SaveAsync();

            return Ok(true);
        }
    }
}