using ApiAPSB.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;

namespace ApiAPSB.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FomrsController : ApiController
    {
        [Route("api/forms/onboard/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOnboard(int fltid)
        {

            // GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Re‌​ferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var context = new dbEntities();

            var items = context.view_frm_onboard_item.OrderBy(q => q.category_index).ThenBy(q => q.order_index).ToList();
            var items_filled = context.view_frm_onboard_status.Where(q => q.flight_id == fltid).ToList();
            foreach (var x in items_filled)
            {
                var item = items.FirstOrDefault(q => q.id == x.item_id);
                if (item != null)
                    item.status = x.status;
            }

            var result = (from x in items
                          group x by new { x.category, x.category_index } into grp
                          select new
                          {
                              grp.Key.category,
                              grp.Key.category_index,
                              items = grp.OrderBy(q => q.order_index).ToList()

                          }).ToList(); ;



            return Ok(new { IsSuccess = true, Data = result, Errors = "null", Messages = "null" });
        }


        public class item_dto
        {
            public int flight_id { get; set; }
            public int item_id { get; set; }
            public string status { get; set; }
            public int? crew_id { get; set; }
        }
        [HttpPost]
        [Route("api/forms/onboard/save")]

        public async Task<DataResponse> SaveOnboard(item_dto dto)
        {

            var _context = new dbEntities();




            try
            {
                var entity = await _context.frm_onboard_status.FirstOrDefaultAsync(q => q.flight_id == dto.flight_id && q.item_id == dto.item_id);

                if (entity == null)
                {
                    entity = new frm_onboard_status();
                    _context.frm_onboard_status.Add(entity);
                }

                entity.flight_id = dto.flight_id;
                entity.item_id = dto.item_id;
                entity.crew_id = dto.crew_id;
                entity.date_status = DateTime.Now;
                entity.status = dto.status;



                var saveResult = await _context.SaveChangesAsync();
                // ViewEFBASR view_efb = await _context.ViewEFBASRs.FirstOrDefaultAsync(q => q.Id == entity.Id);
                return new DataResponse() { IsSuccess = true, Data = dto };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex
                };
            }
        }


        [HttpPost]
        [Route("api/forms/onboard/status/all")]

        public async Task<DataResponse> SaveOnboardAl(item_dto dto)
        {

            var _context = new dbEntities();




            try
            {
                var _items = _context.frm_onboard_status.Where(q => q.flight_id == dto.flight_id).ToList();
                _context.frm_onboard_status.RemoveRange(_items);

                var items = _context.view_frm_onboard_item.OrderBy(q => q.category_index).ThenBy(q => q.order_index).ToList();
                foreach (var x in items)
                {
                    _context.frm_onboard_status.Add(new frm_onboard_status()
                    {
                        date_status = DateTime.Now,
                        crew_id = dto.crew_id,
                        status = dto.status,
                        item_id = x.id,
                        flight_id = dto.flight_id,

                    });
                }






                var saveResult = await _context.SaveChangesAsync();
                // ViewEFBASR view_efb = await _context.ViewEFBASRs.FirstOrDefaultAsync(q => q.Id == entity.Id);
                return new DataResponse() { IsSuccess = true, Data = dto };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex
                };
            }
        }



    }
}
