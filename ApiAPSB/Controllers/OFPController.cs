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
using ApiAPSB.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using System.Reflection;

namespace ApiAPSB.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OFPController : ApiController
    {
        [HttpPost]
        [Route("api/ofp/flights")]
        public async Task<IHttpActionResult> PostGetOFPsByFlightIds(SimpleDto dto)
        {
            var context = new Models.dbEntities();

            var ofps = await context.view_ofpb_root.Where(q => dto.ids.Contains(q.FlightID)).ToListAsync();
            var ofp_ids = ofps.Select(q => (Nullable<int>)q.Id).ToList();
            var nav_logs = await context.view_ofpb_navlog.Where(q => ofp_ids.Contains(q.RootId)).OrderBy(q => q.RootId).ThenBy(q => q.NavType).ThenBy(q => q.Id).ToListAsync();
            var wts = await context.view_ofpb_wt.Where(q => ofp_ids.Contains(q.OFPId)).OrderBy(q => q.OFPId).ThenBy(q => q.Type).ThenBy(q => q.Id).ToListAsync();

            var ofp_grps = (from x in ofps
                            group x by new { x.FlightID, ofp_id = x.Id } into grp
                            select new ofpb()
                            {
                                flight_id = grp.Key.FlightID,
                                ofp_id = grp.Key.ofp_id,
                                root = grp.First(),
                                main_route = nav_logs.Where(q => q.RootId == grp.Key.ofp_id && q.NavType == "MAIN").ToList(),
                                alt1_route = nav_logs.Where(q => q.RootId == grp.Key.ofp_id && q.NavType == "ALT1").ToList(),
                                alt2_route = nav_logs.Where(q => q.RootId == grp.Key.ofp_id && q.NavType == "ALT2").ToList(),
                                main_wts = wts.Where(q => q.OFPId == grp.Key.ofp_id && q.Type == "MAIN").ToList(),
                                alt1_wts = wts.Where(q => q.OFPId == grp.Key.ofp_id && q.Type == "ALT1").ToList(),
                                alt2_wts = wts.Where(q => q.OFPId == grp.Key.ofp_id && q.Type == "ALT2").ToList(),
                                to_wts = wts.Where(q => q.OFPId == grp.Key.ofp_id && q.Type == "ALTTO").ToList(),
                                routes = nav_logs.Where(q => q.RootId == grp.Key.ofp_id).ToList(),
                                wts = wts.Where(q => q.OFPId == grp.Key.ofp_id).ToList(),

                            }).ToList();


            return Ok(new DataResponse()
            {
                Data = ofp_grps,
                Errors = null,
                IsSuccess = true,
            });


        }

        [HttpGet]
        [Route("api/ofp/flight/{flightId}")]
        public async Task<IHttpActionResult> GetOFPByFlight(int flightId)
        {
            var context = new Models.dbEntities();

            var ofp = await context.view_ofpb_root.Where(q => q.FlightID == flightId).FirstOrDefaultAsync();

            if (ofp == null)
            {
                return Ok(new DataResponse()
                {
                    Data = new ofpb() { ofp_id = -1, flight_id = flightId },
                    Errors = null,
                    IsSuccess = true,
                });
            }

            var nav_logs = await context.view_ofpb_navlog.Where(q => q.RootId == ofp.Id).OrderBy(q => q.RootId).ThenBy(q => q.NavType).ThenBy(q => q.Id).ToListAsync();
            var wts = await context.view_ofpb_wt.Where(q => q.OFPId == ofp.Id).OrderBy(q => q.OFPId).ThenBy(q => q.Type).ThenBy(q => q.Id).ToListAsync();



            var ofp_obj = new ofpb()
            {
                ofp_id = ofp.Id,
                flight_id = flightId,
                root = ofp,
                main_route = nav_logs.Where(q => q.NavType == "MAIN").ToList(),
                alt1_route = nav_logs.Where(q => q.NavType == "ALT1").ToList(),
                alt2_route = nav_logs.Where(q => q.NavType == "ALT2").ToList(),
                main_wts = wts.Where(q => q.Type == "MAIN").ToList(),
                alt1_wts = wts.Where(q => q.Type == "ALT1").ToList(),
                alt2_wts = wts.Where(q => q.Type == "ALT2").ToList(),
                to_wts = wts.Where(q => q.Type == "ALTTO").ToList(),
                routes = nav_logs.ToList(),
                wts = wts.ToList(),
            };


            return Ok(new DataResponse()
            {
                Data = ofp_obj,
                Errors = null,
                IsSuccess = true,
            });


        }
        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        [HttpPost]
        [Route("api/ofp/prop/save/test")]
        public async Task<DataResponse> SaveOFPPropTest(/*int ofpId, string propName, string propValue, string user*/dynamic dto)
        {
            try
            {
                int ofpId = Convert.ToInt32(dto.OFPId);
                string propName = Convert.ToString(dto.PropName);
                string propValue = Convert.ToString(dto.PropValue);
                string user = Convert.ToString(dto.User);

                var property = propName.Split('-')[2];

                var tbl_part = propName.Split('-')[1];
                string table = "";
                switch (tbl_part)
                {
                    case "root":
                        table = "OFPB_Root";
                        break;
                    default:
                        break;
                }



                if (table == "OFPB_Root")
                {
                    //var ofp_root = await context.OFPB_Root.FirstOrDefaultAsync(q => q.Id == ofpId);
                    var ofp_root = new OFPB_Root();
                    if (ofp_root != null)
                    {
                        Type type = ofp_root.GetType();

                        PropertyInfo propinfo = type.GetProperty(property);
                        if (string.IsNullOrEmpty(propValue))
                        {
                            propinfo.SetValue(ofp_root, null, null);
                        }
                        else
                        {
                            var val = ChangeType(propValue, propinfo.PropertyType);

                            propinfo.SetValue(ofp_root, val, null);
                        }


                    }
                }


                return new DataResponse() { IsSuccess = true };

            }
            catch (Exception ex)
            {
                return new DataResponse() { IsSuccess = false };
            }




        }

        [HttpPost]
        [Route("api/ofp/prop/save")]
        public async Task<DataResponse> SaveOFPProp(/*int ofpId, string propName, string propValue, string user*/dynamic dto)
        {
            try
            {
                int ofpId = Convert.ToInt32(dto.OFPId);
                string propName = Convert.ToString(dto.PropName);
                string propValue = Convert.ToString(dto.PropValue);
                string user = Convert.ToString(dto.User);
                var context = new Models.dbEntities();
                var prop = await context.OFPB_Prop.Where(q => q.OFPId == ofpId && q.PropName == propName).FirstOrDefaultAsync();
                //prop-root-crew_cockpit
                var property = propName.Split('-')[2];

                var tbl_part = propName.Split('-')[1];
                string table = "";
                switch (tbl_part)
                {
                    case "root":
                        table = "OFPB_Root";
                        break;
                    default:
                        break;
                }
                if (prop == null)
                {
                    prop = new OFPB_Prop()
                    {
                        OFPId = ofpId,

                        PropName = propName,
                        TableName = table,
                        PropertyName = property,


                    };
                    context.OFPB_Prop.Add(prop);
                }
                prop.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                if (!string.IsNullOrEmpty(propValue.Trim().Replace(" ", "")))
                    prop.PropValue = propValue;
                prop.User = user;


                if (table == "OFPB_Root")
                {
                    var ofp_root = await context.OFPB_Root.FirstOrDefaultAsync(q => q.Id == ofpId);
                    if (ofp_root != null)
                    {
                        Type type = ofp_root.GetType();

                        PropertyInfo propinfo = type.GetProperty(property);
                        if (string.IsNullOrEmpty(propValue))
                        {
                            propinfo.SetValue(ofp_root, null, null);
                        }
                        else
                        {
                            var val = ChangeType(propValue, propinfo.PropertyType);

                            propinfo.SetValue(ofp_root, val, null);
                        }
                    }
                }


                var saveResult = await context.SaveChangesAsync();
                return new DataResponse() { IsSuccess = true, Data = new { prop.Id, prop.OFPId, prop.PropName, prop.PropValue, prop.DateUpdate, prop.User, prop.PropType } };

            }
            catch (Exception ex)
            {
                return new DataResponse() { IsSuccess = false };
            }




        }


        [HttpPost]
        [Route("api/ofp/update")]
        public async Task<DataResponse> UpdateOFP(/*int ofpId, string propName, string propValue, string user*/dynamic dto)
        {
            try
            {
                int ofpId = Convert.ToInt32(dto.OFPId);
                string propName = Convert.ToString(dto.PropName);
                string propValue = Convert.ToString(dto.PropValue);
                string user = Convert.ToString(dto.User);
                var context = new Models.dbEntities();
                
                var property = propName.Split('-')[2];

                var tbl_part = propName.Split('-')[1];
                string table = "";
                switch (tbl_part)
                {
                    case "root":
                        table = "OFPB_Root";
                        break;
                    default:
                        break;
                }

                
                var    prop = new OFPB_Prop()
                    {
                        OFPId = ofpId,

                        PropName = propName,
                        TableName = table,
                        PropertyName = property,


                    };
                  
                
                prop.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                if (!string.IsNullOrEmpty(propValue.Trim().Replace(" ", "")))
                    prop.PropValue = propValue;
                prop.User = user;
                context.OFPB_Prop.Add(prop);

                if (table == "OFPB_Root")
                {
                    var ofp_root = await context.OFPB_Root.FirstOrDefaultAsync(q => q.Id == ofpId);
                    if (ofp_root != null)
                    {
                        Type type = ofp_root.GetType();

                        PropertyInfo propinfo = type.GetProperty(property);
                        if (string.IsNullOrEmpty(propValue))
                        {
                            propinfo.SetValue(ofp_root, null, null);
                        }
                        else
                        {
                            var val = ChangeType(propValue, propinfo.PropertyType);

                            propinfo.SetValue(ofp_root, val, null);
                        }
                        ofp_root.date_update = Convert.ToInt64(prop.DateUpdate);
                        ofp_root.user_update= user;
                    }
                    var saveResult = await context.SaveChangesAsync();
                    return new DataResponse() { IsSuccess = true, Data =new { ofp_root.date_update } };

                }

                return new DataResponse() { IsSuccess = false, Messages=new List<string>() { "table not found" } };


            }
            catch (Exception ex)
            {
                return new DataResponse() { IsSuccess = false };
            }




        }



    }


}

public class ofpb
{
    public int? flight_id { get; set; }
    public int? ofp_id { get; set; }
    public view_ofpb_root root { get; set; }
    public List<view_ofpb_navlog> main_route { get; set; }
    public List<view_ofpb_navlog> alt1_route { get; set; }
    public List<view_ofpb_navlog> alt2_route { get; set; }
    public List<view_ofpb_wt> main_wts { get; set; }
    public List<view_ofpb_wt> alt1_wts { get; set; }
    public List<view_ofpb_wt> alt2_wts { get; set; }
    public List<view_ofpb_wt> to_wts { get; set; }
    public List<view_ofpb_navlog> routes { get; set; }
    public List<view_ofpb_wt> wts { get; set; }

}
public class DataResponse
{
    public bool IsSuccess { get; set; }
    public object Data { get; set; }
    public List<string> Errors { get; set; }
    public List<string> Messages { get; set; }
}

public class SimpleDto
{
    public List<int?> ids { get; set; }
    public string type { get; set; }
}

