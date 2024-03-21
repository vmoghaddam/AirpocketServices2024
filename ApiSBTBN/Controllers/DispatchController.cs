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
using ApiSBTBN.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;


namespace ApiSBTBN.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DispatchController : ApiController
    {
        string toHHMM(int? mm)
        {
            if (mm == null || mm <= 0)
                return "00:00";
            // TimeSpan ts = TimeSpan.FromMinutes(Convert.ToDouble(mm));
            // var result = ts.Hours.ToString().PadLeft(2, '0') + ":" + ts.Minutes.ToString().PadLeft(2, '0');
            var hh = mm / 60;
            var min = mm % 60;
            var result = hh.ToString().PadLeft(2, '0') + ":" + min.ToString().PadLeft(2, '0');
            return result;

        }
        bool isValidString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            str = str.Replace(" ", "");
            return !string.IsNullOrEmpty(str);
        }
        [Route("api/tolnd/group/save")]
        [AcceptVerbs("POST")]
        public IHttpActionResult SaveTOLNDGroup(List<TOLNDCardViewModel> objs)
        {
            try
            {
                var result = new List<TOLNDCard>();
                var _context = new Models.dbEntities();
                if (objs == null)
                    return Ok(new DataResponse()
                    {
                        IsSuccess = false,
                        Errors = new List<string>() { "objs is null" }

                    });
                //var flihgtIds = objs.Select(q => q.FlightId).ToList();
                foreach (var dto in objs)
                {

                    var entity = _context.TOLNDCards.FirstOrDefault(q => q.FlightId == dto.FlightId && q.Type == dto.Type);
                    if (entity == null)
                    {
                        entity = new TOLNDCard();
                        _context.TOLNDCards.Add(entity);
                    }


                    entity.User = dto.User;
                    entity.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm");

                    entity.FlightId = dto.FlightId;

                    if (isValidString(dto.Information))
                        entity.Information = dto.Information;

                    if (isValidString(dto.RW))
                        entity.RW = dto.RW;

                    if (isValidString(dto.TL))
                        entity.TL = dto.TL;

                    if (isValidString(dto.FE))
                        entity.FE = dto.FE;

                    if (isValidString(dto.Wind))
                        entity.Wind = dto.Wind;

                    if (isValidString(dto.Visibility))
                        entity.Visibility = dto.Visibility;

                    if (isValidString(dto.Cloud))
                        entity.Cloud = dto.Cloud;

                    if (isValidString(dto.Temp))
                        entity.Temp = dto.Temp;

                    if (isValidString(dto.QNH))
                        entity.QNH = dto.QNH;

                    if (isValidString(dto.DewP))
                        entity.DewP = dto.DewP;

                    if (isValidString(dto.WXCondition))
                        entity.WXCondition = dto.WXCondition;

                    if (isValidString(dto.STAR))
                        entity.STAR = dto.STAR;

                    if (isValidString(dto.APP))
                        entity.APP = dto.APP;

                    if (isValidString(dto.MAS))
                        entity.MAS = dto.MAS;

                    if (isValidString(dto.ActLandingWeight))
                        entity.ActLandingWeight = dto.ActLandingWeight;

                    if (isValidString(dto.Flap))
                        entity.Flap = dto.Flap;

                    if (isValidString(dto.StabTrim))
                        entity.StabTrim = dto.StabTrim;

                    if (isValidString(dto.Verf))
                        entity.Verf = dto.Verf;

                    if (isValidString(dto.FuelToAlternate))
                        entity.FuelToAlternate = dto.FuelToAlternate;

                    if (isValidString(dto.TA))
                        entity.TA = dto.TA;

                    if (isValidString(dto.ZFW))
                        entity.ZFW = dto.ZFW;

                    if (isValidString(dto.TOFuel))
                        entity.TOFuel = dto.TOFuel;

                    if (isValidString(dto.TOWeight))
                        entity.TOWeight = dto.TOWeight;

                    if (isValidString(dto.CG))
                        entity.CG = dto.CG;

                    if (isValidString(dto.V1))
                        entity.V1 = dto.V1;

                    if (isValidString(dto.Vr))
                        entity.Vr = dto.Vr;

                    if (isValidString(dto.V2))
                        entity.V2 = dto.V2;

                    if (isValidString(dto.Type))
                        entity.Type = dto.Type;

                    if (isValidString(dto.LDA))
                        entity.LDA = dto.LDA;

                    if (isValidString(dto.CTime))
                        entity.CTime = dto.CTime;

                    if (isValidString(dto.AC))
                        entity.AC = dto.AC;
                    if (isValidString(dto.AI))
                        entity.AI = dto.AI;
                    if (isValidString(dto.NERP))
                        entity.NERP = dto.NERP;
                    if (isValidString(dto.MERP))
                        entity.MERP = dto.MERP;
                    if (isValidString(dto.ATEMP))
                        entity.ATEMP = dto.ATEMP;
                    if (isValidString(dto.FERP))
                        entity.FERP = dto.FERP;
                    if (isValidString(dto.RWINUSE))
                        entity.RWINUSE = dto.RWINUSE;
                    if (isValidString(dto.VGA))
                        entity.VGA = dto.VGA;
                    if (isValidString(dto.VFLAP))
                        entity.VFLAP = dto.VFLAP;
                    if (isValidString(dto.VSLAT))
                        entity.VSLAT = dto.VSLAT;
                    if (isValidString(dto.VCLEAN))
                        entity.VCLEAN = dto.VCLEAN;

                    result.Add(entity);
                }

                _context.SaveChanges();

                return Ok(new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                });


            }
            catch (Exception ex)
            {
                return Ok(new DataResponse()
                {
                    IsSuccess = false,
                    Errors = new List<string>() {
                  ex.Message+" Inner "+(ex.InnerException==null?"":ex.InnerException.Message)
                 }
                });
            }






        }

        DateTime ParseDate2(string str)
        {
            var _y = Convert.ToInt32(str.Substring(0, 4));
            var _m = Convert.ToInt32(str.Substring(4, 2));
            var _d = Convert.ToInt32(str.Substring(6, 2));
            var _h = Convert.ToInt32(str.Substring(8, 2));
            var _mm = Convert.ToInt32(str.Substring(10, 2));
            var _s = Convert.ToInt32(str.Substring(12, 2));

            var dt = new DateTime(_y, _m, _d, _h, _mm, _s);

            return dt;
        }

        [Route("api/efb/value/save")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEFBValue(EFBValues dto)
        {
            var _context = new Models.dbEntities();

            foreach (var rec in dto.Values)
            {
                var dbrec = new EFBValue();
                dbrec.UserName = rec.UserName;
                dbrec.FieldName = rec.FieldName;
                dbrec.FieldValue = rec.FieldValue;
                dbrec.TableName = rec.TableName;
                dbrec.FlightId = rec.FlightId;
                dbrec.DateCreate = ParseDate2(rec.DateCreate);

                _context.EFBValues.Add(dbrec);
            }
            var saveResult = await _context.SaveChangesAsync();

            return Ok(new DataResponse() { IsSuccess = true });



            // return new DataResponse() { IsSuccess = false };
        }

        [Route("api/efb/action/save")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEFBValue(EFBActionObj dto)
        {
            var _context = new Models.dbEntities();

            var action = new Models.EFBAction()
            {
                ActionName = dto.ActionName,

                DateCreate = DateTime.Now,
                UserName = dto.UserName,
                NewValue = dto.NewValue,
                OldValue = dto.OldValue
            };
            _context.EFBActions.Add(action);
            var saveResult = await _context.SaveChangesAsync();

            return Ok(new DataResponse() { IsSuccess = true });



            // return new DataResponse() { IsSuccess = false };
        }



    }


    public class DataResponse
    {
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
        public List<string> Errors { get; set; }
    }
    public class EFBActionObj
    {
        public string UserName { get; set; }
        public string ActionName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
    public class EFBValueObj
    {
        public int id { get; set; }
        public int FlightId { get; set; }
        public string DateCreate { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string TableName { get; set; }
        public string UserName { get; set; }


    }
    public class EFBValues
    {
        public List<EFBValueObj> Values { get; set; }
    }
    //public class DataResponse
    //{
    //    public bool IsSuccess { get; set; }
    //    public object Data { get; set; }
    //    public List<string> Errors { get; set; }
    //}

    public class TOLNDCardViewModel
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string Information { get; set; }
        public string RW { get; set; }
        public string TL { get; set; }
        public string FE { get; set; }
        public string Wind { get; set; }
        public string Visibility { get; set; }
        public string Cloud { get; set; }
        public string Temp { get; set; }
        public string QNH { get; set; }
        public string DewP { get; set; }
        public string WXCondition { get; set; }
        public string STAR { get; set; }
        public string APP { get; set; }
        public string MAS { get; set; }
        public string ActLandingWeight { get; set; }
        public string Flap { get; set; }
        public string StabTrim { get; set; }
        public string Verf { get; set; }
        public string FuelToAlternate { get; set; }
        public string TA { get; set; }
        public string LDA { get; set; }
        public string ZFW { get; set; }
        public string TOFuel { get; set; }
        public string TOWeight { get; set; }
        public string CG { get; set; }
        public string V1 { get; set; }
        public string Vr { get; set; }
        public string V2 { get; set; }
        public string Type { get; set; }
        public string DateUpdate { get; set; }
        public string User { get; set; }
        public string CTime { get; set; }
        public string AC { get; set; }
        public string AI { get; set; }
        public string NERP { get; set; }
        public string MERP { get; set; }
        public string ATEMP { get; set; }
        public string FERP { get; set; }
        public string RWINUSE { get; set; }
        public string VGA { get; set; }
        public string VFLAP { get; set; }
        public string VSLAT { get; set; }
        public string VCLEAN { get; set; }
    }
}
