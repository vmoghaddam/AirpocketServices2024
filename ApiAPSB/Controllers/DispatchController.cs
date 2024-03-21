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


namespace ApiAPSB.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DispatchController : ApiController
    {
        [Route("api/dr/test/{fltid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetDR(int fltid)
        {
            var x = Environment.GetEnvironmentVariable("cnn_string", EnvironmentVariableTarget.User);
            var _context = new Models.dbEntities();

            var appleg = await _context.XAppLegs.OrderByDescending(q => q.ID).Select(q => q.FlightId).FirstOrDefaultAsync();

            return Ok(appleg);

            // return new DataResponse() { IsSuccess = false };
        }

        [Route("api/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetFlight(int fltid)
        {
            var context = new Models.dbEntities();
            var flight = await context.AppLegOPS.Where(q => q.ID == fltid).FirstOrDefaultAsync();
            var crew = await context.XFlightCrews.Where(q => q.FlightId == fltid).OrderBy(q => q.GroupOrder).ToListAsync();

            var result = new
            {
                flight,
                crew
            };

            return Ok(result);

            // return new DataResponse() { IsSuccess = false };
        }


        [Route("api/applegs/{crtbl}")]

        //nookp
        public IHttpActionResult GetAppLegs(DateTime? df, DateTime? dt, int? ip, int? cpt, int? status, int? asrvr, int crtbl)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var step = "0";
            try
            {
                df = df != null ? ((DateTime)df).Date : DateTime.MinValue.Date;
                dt = dt != null ? ((DateTime)dt).Date : DateTime.MaxValue.Date;
                var context = new Models.dbEntities();
                var query = from x in context.AppLegOPS
                                // where x.FlightStatusID != 1 && x.FlightStatusID != 4
                            select x;
                query = query.Where(q => q.STDDay >= df && q.STDDay <= dt);
                if (crtbl == 1)
                    query = query.Where(q => q.CRTBL == 1);
                if (ip != null)
                    query = query.Where(q => q.IPId == ip);
                if (cpt != null)
                    query = query.Where(q => q.P1Id == cpt);
                if (asrvr != null)
                {
                    if (asrvr == 1)
                        query = query.Where(q => q.MSN == 1);


                }
                if (status != null)
                {

                    List<int?> sts = new List<int?>();
                    switch ((int)status)
                    {
                        case 1:
                            sts.Add(15);
                            sts.Add(3);
                            query = query.Where(q => sts.Contains(q.FlightStatusID));
                            break;
                        case 2:
                            sts.Add(1);
                            query = query.Where(q => sts.Contains(q.FlightStatusID));
                            break;
                        case 3:
                            sts.Add(4);
                            query = query.Where(q => sts.Contains(q.FlightStatusID));
                            break;
                        case 4:
                            sts.Add(20);
                            sts.Add(21);
                            sts.Add(22);
                            sts.Add(4);
                            sts.Add(2);
                            sts.Add(23);
                            sts.Add(24);
                            sts.Add(25);
                            query = query.Where(q => sts.Contains(q.FlightStatusID));

                            break;
                        case 5:
                            break;
                        default:
                            break;
                    }
                }
                step = "1";
                var result = query.OrderBy(q => q.STD).ToList();
                step = "2";
                // return result.OrderBy(q => q.STD);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = step + "    " + ex.Message;
                if (ex.InnerException != null)
                    msg += " INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }


        }




        [Route("api/appleg/ofp/{flightId}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOPF(int flightId)
        {

            try
            {
                var context = new Models.dbEntities();
                var ofp = context.OFPImports.FirstOrDefault(q => q.FlightId == flightId);
                var fuel = context.FlightInformations.Where(q=>q.ID==flightId).Select(q=>new {q.UsedFuel,q.FuelDeparture,q.FuelArrival }).FirstOrDefault();
                decimal? onblock_fuel = 0;
                var arr = fuel.FuelArrival == null ? 0 : fuel.FuelArrival;
                var dep = fuel.FuelDeparture == null ? 0 : fuel.FuelDeparture;
                var used = fuel.UsedFuel == null ? 0 : fuel.UsedFuel;
                //if (fuel.FuelArrival != null && fuel.FuelDeparture != null && fuel.UsedFuel != null)
                //  onblock_fuel = (fuel.FuelArrival??0   + fuel.FuelDeparture??0  ) - fuel.UsedFuel??0  ;
                onblock_fuel = (arr + dep) - used;

                if (ofp == null)
                    return Ok(new { Id = -1 });
                else
                {
                    var props = context.OFPImportProps.Where(q => q.OFPId == ofp.Id).Select(q =>
                      new
                      {
                          q.Id,
                          q.OFPId,
                          q.PropName,
                          q.PropType,
                          q.PropValue,
                          q.User,
                          q.DateUpdate,

                      }
                        ).ToList();
                    return Ok(new
                    {
                        ofp.Id,
                        ofp.FlightId,
                        ofp.TextOutput,
                        ofp.User,
                        ofp.DateCreate,
                        ofp.PIC,
                        ofp.PICId,
                        ofp.JLSignedBy,
                        ofp.JLDatePICApproved,



                        ofp.DOW,
                        ofp.FLL,
                        ofp.MCI,
                        ofp.JAPlan1,
                        ofp.JAPlan2,
                        ofp.JPlan,
                        ofp.JFuel,

                        ofp.JWTDRF,
                        ofp.JCSTBL,
                        ofp.JALDRF,

                        ofp.ETA,
                        ofp.ETD,
                        ofp.THM,
                        ofp.UNT,
                        ofp.CRW,
                        ofp.RTM,
                        ofp.RTA,
                        ofp.RTB,
                        ofp.RTT,
                        ofp.PLD,
                        ofp.EZFW,
                        ofp.ETOW,
                        ofp.ELDW,
                        ofp.ALT1,
                        ofp.ALT2,
                        ofp.TALT1,
                        ofp.TALT2,
                        ofp.FPF,
                        ofp.FPFuel,
                        ofp.FPTripFuel,
                        ofp.FuelALT1,
                        ofp.FuelALT2,
                        ofp.FuelCONT,
                        ofp.FuelFINALRES,
                        ofp.FuelMINTOF,
                        ofp.FuelOFFBLOCK,
                        ofp.FuelTAXI,
                        ofp.FuelTOF,
                        ofp.VDT,
                        ofp.DID,
                        ofp.MAXSHEER,
                        ofp.MINDIVFUEL,
                        ofp.WDCLB,
                        ofp.WDDES,
                        ofp.WDTMP,


                        props,
                        onblock_fuel

                    });
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                return Ok(msg);
            }

        }



        [Route("api/atc/text/get/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetImportATCTextGET(int id)
        {
            try
            {
                var context = new Models.dbEntities();


                var flightObj = context.FlightInformations.FirstOrDefault(q => q.ID == id);


                return Ok(flightObj.ATCPlan);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                return Ok(msg);
            }

        }


        [Route("api/upload/atc/flightplan")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> UploadATCFLIGHPLAN()
        {
            try
            {
                IHttpActionResult outPut = Ok(200);

                string key = string.Empty;
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var date = DateTime.Now;
                        var ext = System.IO.Path.GetExtension(postedFile.FileName);
                        key = "atc-" + date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + ext;

                        var filePath = ConfigurationManager.AppSettings["atc"] + key; //HttpContext.Current.Server.MapPath("~/upload/" + key);
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);
                    }
                    // outPut = (await ImportFlights2(key));
                    // var ctrl = new FlightController();
                    //  outPut = await ctrl.UploadFlights3(key);
                    outPut = Ok(key);

                }
                else
                {
                    return Ok("error");
                }
                return outPut;
            }
            catch (Exception ex)
            {
                return Ok(ex.Message + "   IN    " + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }

        }
        [Route("api/flight/atc/update/{id}/{fn}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetATcUpdate(int id, string fn)
        {
            var context = new Models.dbEntities();
            try
            {
                var flt = context.FlightInformations.FirstOrDefault(q => q.ID == id);
                if (flt != null)
                    flt.ATCPlan = fn.Split('X')[0] + "." + fn.Split('X')[1]; //".pdf";
                context.SaveChanges();
                return Ok("done");
            }
            catch (Exception ex)
            {
                var msg = ex.Message + " IN:" + (ex.InnerException != null ? ex.InnerException.Message : "NO");
                return Ok(msg);
            }

        }


        [Route("api/atc/text/input")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostImportATCTextInput(dynamic dto)
        {
            try
            {
                string user = Convert.ToString(dto.user);
                int fltId = Convert.ToInt32(dto.fltId);
                var context = new Models.dbEntities();

                var flight = context.ViewLegTimes.FirstOrDefault(q => q.ID == fltId);
                var flightObj = context.FlightInformations.FirstOrDefault(q => q.ID == fltId);

                string ftext = Convert.ToString(dto.text);
                flightObj.ATCPlan = ftext;
                context.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                var msg = ex.Message + " IN:" + (ex.InnerException != null ? ex.InnerException.Message : "NO");
                return Ok(msg);
            }

        }


        [Route("api/upload/flight/doc")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> UploadFlightDoc()
        {
            try
            {
                IHttpActionResult outPut = Ok(200);

                string key = string.Empty;
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var date = DateTime.Now;
                        var ext = System.IO.Path.GetExtension(postedFile.FileName);
                        key = "doc-" + date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + ext;

                        var filePath = ConfigurationManager.AppSettings["fltdoc"] + key; //HttpContext.Current.Server.MapPath("~/upload/" + key);
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);
                    }
                    // outPut = (await ImportFlights2(key));
                    // var ctrl = new FlightController();
                    //  outPut = await ctrl.UploadFlights3(key);
                    outPut = Ok(key);

                }
                else
                {
                    return Ok("error");
                }
                return outPut;
            }
            catch (Exception ex)
            {
                return Ok(ex.Message + "   IN    " + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }

        }


        [Route("api/flight/doc/save/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostFlightDoc(FlightDocDto dto)
        {
            var context = new Models.dbEntities();
            try
            {
                var doc = context.FlightDocuments.Where(q => q.FlightId == dto.FlightId && q.DocumentType == dto.DocumentType).FirstOrDefault();
                if (doc == null)
                {
                    doc = new FlightDocument();
                    context.FlightDocuments.Add(doc);
                }
                doc.FlightId = dto.FlightId;
                doc.Remark = dto.Remark;
                doc.DateCreate = DateTime.UtcNow;
                doc.DocumentUrl = dto.DocumentUrl;
                doc.DocumentType = dto.DocumentType;
                var flight = context.FlightInformations.Where(q => q.ID == dto.FlightId).FirstOrDefault();
                switch (dto.DocumentType)
                {
                    case "ddl":
                        flight.CPInstructor = doc.DocumentUrl;
                        break;
                    case "packinglist":
                        flight.CPP1 = doc.DocumentUrl;
                        break;
                    case "spwx":
                        flight.CPP2 = doc.DocumentUrl;
                        break;
                    case "notam":
                        flight.CPISCCM = doc.DocumentUrl;
                        break;
                    case "other":
                        flight.CPSCCM = doc.DocumentUrl;
                        break;

                    default:
                        break;
                }

                context.SaveChanges();
                return Ok(doc);
            }
            catch (Exception ex)
            {
                var msg = ex.Message + " IN:" + (ex.InnerException != null ? ex.InnerException.Message : "NO");
                return Ok(msg);
            }

        }

        [Route("api/flight/doc/visit/{flt}/{type}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetDocVisit(int flt, string type)
        {
            var context = new Models.dbEntities();
            try
            {
                //  var flight = context.FlightInformations.FirstOrDefault(q => q.ID == flt);
                var doc = context.FlightDocuments.Where(q => q.FlightId == flt && q.DocumentType == type).OrderByDescending(q => q.Id).FirstOrDefault();
                if (doc != null)
                    doc.DateVisite = DateTime.Now;
                context.SaveChanges();
                return Ok("done");
            }
            catch (Exception ex)
            {
                var msg = ex.Message + " IN:" + (ex.InnerException != null ? ex.InnerException.Message : "NO");
                return Ok(msg);
            }

        }

        [Route("api/flight/docs/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsDocs(int id)
        {
            var context = new Models.dbEntities();
            try
            {
                var docs = context.FlightDocuments.Where(q => q.FlightId == id).OrderBy(q => q.DateCreate).ToList();
                return Ok(docs);
            }
            catch (Exception ex)
            {
                var msg = ex.Message + " IN:" + (ex.InnerException != null ? ex.InnerException.Message : "NO");
                return Ok(msg);
            }

        }




        [Route("api/flight/doc/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsDoc(int id)
        {
            var context = new Models.dbEntities();
            try
            {
                var docs = context.FlightDocuments.Where(q => q.Id == id).FirstOrDefault();
                return Ok(docs);
            }
            catch (Exception ex)
            {
                var msg = ex.Message + " IN:" + (ex.InnerException != null ? ex.InnerException.Message : "NO");
                return Ok(msg);
            }

        }

        [Route("api/flight/doc/type/{id}/{type}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsDoc(int id, string type)
        {
            var context = new Models.dbEntities();
            try
            {
                var docs = context.FlightDocuments.Where(q => q.FlightId == id && q.DocumentType == type).FirstOrDefault();
                return Ok(docs);
            }
            catch (Exception ex)
            {
                var msg = ex.Message + " IN:" + (ex.InnerException != null ? ex.InnerException.Message : "NO");
                return Ok(msg);
            }

        }

        public class FlightDocDto
        {
            public int Id { get; set; }

            public int FlightId { get; set; }
            public string DocumentType { get; set; }
            public string DocumentUrl { get; set; }
            public string Remark { get; set; }
        }


        [Route("api/dr/test1/{fltid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetDR1(int fltid)
        {
            var _context = new Models.dbEntities();

            var appleg = await _context.XAppLegs.FirstOrDefaultAsync(q => q.FlightId == fltid);
            var appcrewflight = await _context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefaultAsync();
            // var fdpitems = await _context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToListAsync();
            //var fltIds = fdpitems.Select(q => q.FlightId).ToList();

            return Ok(appcrewflight);

            // return new DataResponse() { IsSuccess = false };
        }
        [Route("api/dr/test2/{fltid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetDR2(int fltid)
        {
            var _context = new Models.dbEntities();

            var appleg = await _context.XAppLegs.FirstOrDefaultAsync(q => q.FlightId == fltid);
            var appcrewflight = await _context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefaultAsync();
            var fdpitems = await _context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToListAsync();
            //var fltIds = fdpitems.Select(q => q.FlightId).ToList();

            return Ok(fdpitems);

            // return new DataResponse() { IsSuccess = false };
        }


        [Route("api/efb/dr/save")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostDR(DSPReleaseViewModel DSPRelease)
        {
            var step = "0";
            try
            {
                var _context = new Models.dbEntities();

                var appleg = await _context.XAppLegs.FirstOrDefaultAsync(q => q.FlightId == DSPRelease.FlightId);
                var appcrewflight = await _context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefaultAsync();
                var fdpitems = await _context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToListAsync();
                var fltIds = fdpitems.Select(q => q.FlightId).ToList();
                step = "1";
                var drs = await _context.EFBDSPReleases.Where(q => fltIds.Contains(q.FlightId)).ToListAsync();
                step = "2";
                _context.EFBDSPReleases.RemoveRange(drs);
                await _context.SaveChangesAsync();
                var _res = new List<object>();
                foreach (var flightId in fltIds)
                {
                    var release = await _context.EFBDSPReleases.FirstOrDefaultAsync(q => q.FlightId == DSPRelease.FlightId);
                    if (release == null)
                    {
                        release = new EFBDSPRelease();
                        _context.EFBDSPReleases.Add(release);

                    }

                    release.User = DSPRelease.User;
                    release.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm");


                    release.FlightId = flightId; //DSPRelease.FlightId;
                    release.ActualWXDSP = DSPRelease.ActualWXDSP;
                    release.ActualWXCPT = DSPRelease.ActualWXCPT;
                    release.ActualWXDSPRemark = DSPRelease.ActualWXDSPRemark;
                    release.ActualWXCPTRemark = DSPRelease.ActualWXCPTRemark;
                    release.WXForcastDSP = DSPRelease.WXForcastDSP;
                    release.WXForcastCPT = DSPRelease.WXForcastCPT;
                    release.WXForcastDSPRemark = DSPRelease.WXForcastDSPRemark;
                    release.WXForcastCPTRemark = DSPRelease.WXForcastCPTRemark;
                    release.SigxWXDSP = DSPRelease.SigxWXDSP;
                    release.SigxWXCPT = DSPRelease.SigxWXCPT;
                    release.SigxWXDSPRemark = DSPRelease.SigxWXDSPRemark;
                    release.SigxWXCPTRemark = DSPRelease.SigxWXCPTRemark;
                    release.WindChartDSP = DSPRelease.WindChartDSP;
                    release.WindChartCPT = DSPRelease.WindChartCPT;
                    release.WindChartDSPRemark = DSPRelease.WindChartDSPRemark;
                    release.WindChartCPTRemark = DSPRelease.WindChartCPTRemark;
                    release.NotamDSP = DSPRelease.NotamDSP;
                    release.NotamCPT = DSPRelease.NotamCPT;
                    release.NotamDSPRemark = DSPRelease.NotamDSPRemark;
                    release.NotamCPTRemark = DSPRelease.NotamCPTRemark;
                    release.ComputedFligthPlanDSP = DSPRelease.ComputedFligthPlanDSP;
                    release.ComputedFligthPlanCPT = DSPRelease.ComputedFligthPlanCPT;
                    release.ComputedFligthPlanDSPRemark = DSPRelease.ComputedFligthPlanDSPRemark;
                    release.ComputedFligthPlanCPTRemark = DSPRelease.ComputedFligthPlanCPTRemark;
                    release.ATCFlightPlanDSP = DSPRelease.ATCFlightPlanDSP;
                    release.ATCFlightPlanCPT = DSPRelease.ATCFlightPlanCPT;
                    release.ATCFlightPlanDSPRemark = DSPRelease.ATCFlightPlanDSPRemark;
                    release.ATCFlightPlanCPTRemark = DSPRelease.ATCFlightPlanCPTRemark;
                    release.PermissionsDSP = DSPRelease.PermissionsDSP;
                    release.PermissionsCPT = DSPRelease.PermissionsCPT;
                    release.PermissionsDSPRemark = DSPRelease.PermissionsDSPRemark;
                    release.PermissionsCPTRemark = DSPRelease.PermissionsCPTRemark;
                    release.JeppesenAirwayManualDSP = DSPRelease.JeppesenAirwayManualDSP;
                    release.JeppesenAirwayManualCPT = DSPRelease.JeppesenAirwayManualCPT;
                    release.JeppesenAirwayManualDSPRemark = DSPRelease.JeppesenAirwayManualDSPRemark;
                    release.JeppesenAirwayManualCPTRemark = DSPRelease.JeppesenAirwayManualCPTRemark;
                    release.MinFuelRequiredDSP = DSPRelease.MinFuelRequiredDSP;
                    release.MinFuelRequiredCPT = DSPRelease.MinFuelRequiredCPT;
                    //   release.MinFuelRequiredCFP = DSPRelease.MinFuelRequiredCFP;
                    //  release.MinFuelRequiredPilotReq = DSPRelease.MinFuelRequiredPilotReq;
                    release.GeneralDeclarationDSP = DSPRelease.GeneralDeclarationDSP;
                    release.GeneralDeclarationCPT = DSPRelease.GeneralDeclarationCPT;
                    release.GeneralDeclarationDSPRemark = DSPRelease.GeneralDeclarationDSPRemark;
                    release.GeneralDeclarationCPTRemark = DSPRelease.GeneralDeclarationCPTRemark;
                    release.FlightReportDSP = DSPRelease.FlightReportDSP;
                    release.FlightReportCPT = DSPRelease.FlightReportCPT;
                    release.FlightReportDSPRemark = DSPRelease.FlightReportDSPRemark;
                    release.FlightReportCPTRemark = DSPRelease.FlightReportCPTRemark;
                    release.TOLndCardsDSP = DSPRelease.TOLndCardsDSP;
                    release.TOLndCardsCPT = DSPRelease.TOLndCardsCPT;
                    release.TOLndCardsDSPRemark = DSPRelease.TOLndCardsDSPRemark;
                    release.TOLndCardsCPTRemark = DSPRelease.TOLndCardsCPTRemark;
                    release.LoadSheetDSP = DSPRelease.LoadSheetDSP;
                    release.LoadSheetCPT = DSPRelease.LoadSheetCPT;
                    release.LoadSheetDSPRemark = DSPRelease.LoadSheetDSPRemark;
                    release.LoadSheetCPTRemark = DSPRelease.LoadSheetCPTRemark;
                    release.FlightSafetyReportDSP = DSPRelease.FlightSafetyReportDSP;
                    release.FlightSafetyReportCPT = DSPRelease.FlightSafetyReportCPT;
                    release.FlightSafetyReportDSPRemark = DSPRelease.FlightSafetyReportDSPRemark;
                    release.FlightSafetyReportCPTRemark = DSPRelease.FlightSafetyReportCPTRemark;
                    release.AVSECIncidentReportDSP = DSPRelease.AVSECIncidentReportDSP;
                    release.AVSECIncidentReportCPT = DSPRelease.AVSECIncidentReportCPT;
                    release.AVSECIncidentReportDSPRemark = DSPRelease.AVSECIncidentReportDSPRemark;
                    release.AVSECIncidentReportCPTRemark = DSPRelease.AVSECIncidentReportCPTRemark;
                    release.OperationEngineeringDSP = DSPRelease.OperationEngineeringDSP;
                    release.OperationEngineeringCPT = DSPRelease.OperationEngineeringCPT;
                    release.OperationEngineeringDSPRemark = DSPRelease.OperationEngineeringDSPRemark;
                    release.OperationEngineeringCPTRemark = DSPRelease.OperationEngineeringCPTRemark;
                    release.VoyageReportDSP = DSPRelease.VoyageReportDSP;
                    release.VoyageReportCPT = DSPRelease.VoyageReportCPT;
                    release.VoyageReportDSPRemark = DSPRelease.VoyageReportDSPRemark;
                    release.VoyageReportCPTRemark = DSPRelease.VoyageReportCPTRemark;
                    release.PIFDSP = DSPRelease.PIFDSP;
                    release.PIFCPT = DSPRelease.PIFCPT;
                    release.PIFDSPRemark = DSPRelease.PIFDSPRemark;
                    release.PIFCPTRemark = DSPRelease.PIFCPTRemark;
                    release.GoodDeclarationDSP = DSPRelease.GoodDeclarationDSP;
                    release.GoodDeclarationCPT = DSPRelease.GoodDeclarationCPT;
                    release.GoodDeclarationDSPRemark = DSPRelease.GoodDeclarationDSPRemark;
                    release.GoodDeclarationCPTRemark = DSPRelease.GoodDeclarationCPTRemark;
                    release.IPADDSP = DSPRelease.IPADDSP;
                    release.IPADCPT = DSPRelease.IPADCPT;
                    release.IPADDSPRemark = DSPRelease.IPADDSPRemark;
                    release.IPADCPTRemark = DSPRelease.IPADCPTRemark;
                    release.DateConfirmed = DSPRelease.DateConfirmed;
                    release.DispatcherId = DSPRelease.DispatcherId;
                    release.ATSFlightPlanCMDR = DSPRelease.ATSFlightPlanCMDR;
                    release.ATSFlightPlanFOO = DSPRelease.ATSFlightPlanFOO;
                    release.ATSFlightPlanFOORemark = DSPRelease.ATSFlightPlanFOORemark;
                    release.ATSFlightPlanCMDRRemark = DSPRelease.ATSFlightPlanCMDRRemark;
                    release.VldCMCCMDR = DSPRelease.VldCMCCMDR;
                    release.VldCMCCMDRRemark = DSPRelease.VldCMCCMDRRemark;
                    release.VldCMCFOO = DSPRelease.VldCMCFOO;
                    release.VldCMCFOORemark = DSPRelease.VldCMCFOORemark;
                    release.VldEFBCMDR = DSPRelease.VldEFBCMDR;
                    release.VldEFBCMDRRemark = DSPRelease.VldEFBCMDRRemark;
                    release.VldEFBFOO = DSPRelease.VldEFBFOO;
                    release.VldEFBFOORemark = DSPRelease.VldEFBFOORemark;
                    release.VldFlightCrewCMDR = DSPRelease.VldFlightCrewCMDR;
                    release.VldFlightCrewCMDRRemark = DSPRelease.VldFlightCrewCMDRRemark;
                    release.VldFlightCrewFOO = DSPRelease.VldFlightCrewFOO;
                    release.VldFlightCrewFOORemark = DSPRelease.VldFlightCrewFOORemark;
                    release.VldMedicalCMDR = DSPRelease.VldMedicalCMDR;
                    release.VldMedicalCMDRRemark = DSPRelease.VldMedicalCMDRRemark;
                    release.VldMedicalFOO = DSPRelease.VldMedicalFOO;
                    release.VldMedicalFOORemark = DSPRelease.VldMedicalFOORemark;
                    release.VldPassportCMDR = DSPRelease.VldPassportCMDR;
                    release.VldPassportCMDRRemark = DSPRelease.VldPassportCMDRRemark;
                    release.VldPassportFOO = DSPRelease.VldPassportFOO;
                    release.VldPassportFOORemark = DSPRelease.VldPassportFOORemark;
                    release.VldRampPassCMDR = DSPRelease.VldRampPassCMDR;
                    release.VldRampPassCMDRRemark = DSPRelease.VldRampPassCMDRRemark;
                    release.VldRampPassFOO = DSPRelease.VldRampPassFOO;
                    release.VldRampPassFOORemark = DSPRelease.VldRampPassFOORemark;
                    release.OperationalFlightPlanFOO = DSPRelease.OperationalFlightPlanFOO;
                    release.OperationalFlightPlanFOORemark = DSPRelease.OperationalFlightPlanFOORemark;
                    release.OperationalFlightPlanCMDR = DSPRelease.OperationalFlightPlanCMDR;
                    release.OperationalFlightPlanCMDRRemark = DSPRelease.OperationalFlightPlanCMDRRemark;
                    release.SgnDSPLicNo = DSPRelease.SgnDSPLicNo;
                    release.SgnCPTLicNo = DSPRelease.SgnCPTLicNo;
                    release.JLDSPSignDate = DSPRelease.JLDSPSignDate;
                    release.SGNDSPName = DSPRelease.SGNDSPName;

                    _res.Add(release);
                }


                var saveResult = await _context.SaveChangesAsync();


                return Ok(new DataResponse() { IsSuccess = true, Data = _res });

            }
            catch (Exception ex)
            {
                var msg = step + "   " + ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                return Ok(msg);
            }

            // return new DataResponse() { IsSuccess = false };
        }
        public class SimpleDto
        {
            public List<int> ids { get; set; }
        }
        [AcceptVerbs("POST")]
        [Route("api/drs")]
        public async Task<IHttpActionResult> PostgetDRs(SimpleDto dto)
        {
            var ids = dto.ids.Select(q => (Nullable<int>)q).ToList();
            var _context = new Models.dbEntities();
            var drs = await _context.ViewEFBDSPReleases.Where(q => ids.Contains(q.FlightId)).ToListAsync();
            return Ok(new DataResponse()
            {
                Data = drs,
                IsSuccess = true

            });

        }


        [Route("api/efb/dr/{flightId}")]
        public async Task<IHttpActionResult> GetDRByFlightId(int flightId)
        {
            var _context = new Models.dbEntities();
            var entity = await _context.ViewEFBDSPReleases.FirstOrDefaultAsync(q => q.FlightId == flightId);
            var flight = await _context.FlightInformations.FirstOrDefaultAsync(q => q.ID == flightId);
            if (entity != null)
            {
                entity.MinFuelRequiredCFP = flight.OFPOFFBLOCKFUEL == null ? 0 : (decimal)flight.OFPOFFBLOCKFUEL;//flight.FPFuel == null ? 0 : (decimal)flight.FPFuel + 200;
                entity.MinFuelRequiredPilotReq = flight.FuelPlanned;// flight.FuelPlanned;

            }
            else
            {
                entity = new ViewEFBDSPReleas()
                {
                    Id = -1,
                    MinFuelRequiredCFP = flight.OFPOFFBLOCKFUEL == null ? 0 : (decimal)flight.OFPOFFBLOCKFUEL,
                    MinFuelRequiredPilotReq = flight.FuelPlanned,

                };
            }
            return Ok(new DataResponse()
            {
                Data = entity,
                IsSuccess = true

            });
        }

        DateTime ParseDate(string str)
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
                dbrec.DateCreate = ParseDate(rec.DateCreate);

                _context.EFBValues.Add(dbrec);
            }
            var saveResult = await _context.SaveChangesAsync();

            return Ok(new DataResponse() { IsSuccess = true });



            // return new DataResponse() { IsSuccess = false };
        }

        [Route("api/dr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetDRByFlight(int fltid)
        {
            // return Ok('X');
            try
            {
                var context = new Models.dbEntities();
                var result = context.ViewEFBDSPReleases.FirstOrDefault(q => q.FlightId == fltid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }
        }

        [Route("api/dsp/dr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetDRByFlightDSP(int fltid)
        {


            var context = new Models.dbEntities();
            var result = context.ViewEFBDSPReleases.FirstOrDefault(q => q.FlightId == fltid);
            //.EFBDSPReleases.FirstOrDefault(q => q.FlightId == fltid);
            return Ok(result);
        }
        [Route("api/dr/save")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostDRX(DSPReleaseViewModel DSPRelease)
        {
            var context = new Models.dbEntities();
            var release = context.EFBDSPReleases.FirstOrDefault(q => q.FlightId == DSPRelease.FlightId);
            if (release == null)
            {
                release = new EFBDSPRelease();
                context.EFBDSPReleases.Add(release);

            }

            release.User = DSPRelease.User;
            release.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm");


            release.FlightId = DSPRelease.FlightId;
            release.ActualWXDSP = DSPRelease.ActualWXDSP;
            release.ActualWXCPT = DSPRelease.ActualWXCPT;
            release.ActualWXDSPRemark = DSPRelease.ActualWXDSPRemark;
            release.ActualWXCPTRemark = DSPRelease.ActualWXCPTRemark;
            release.WXForcastDSP = DSPRelease.WXForcastDSP;
            release.WXForcastCPT = DSPRelease.WXForcastCPT;
            release.WXForcastDSPRemark = DSPRelease.WXForcastDSPRemark;
            release.WXForcastCPTRemark = DSPRelease.WXForcastCPTRemark;
            release.SigxWXDSP = DSPRelease.SigxWXDSP;
            release.SigxWXCPT = DSPRelease.SigxWXCPT;
            release.SigxWXDSPRemark = DSPRelease.SigxWXDSPRemark;
            release.SigxWXCPTRemark = DSPRelease.SigxWXCPTRemark;
            release.WindChartDSP = DSPRelease.WindChartDSP;
            release.WindChartCPT = DSPRelease.WindChartCPT;
            release.WindChartDSPRemark = DSPRelease.WindChartDSPRemark;
            release.WindChartCPTRemark = DSPRelease.WindChartCPTRemark;
            release.NotamDSP = DSPRelease.NotamDSP;
            release.NotamCPT = DSPRelease.NotamCPT;
            release.NotamDSPRemark = DSPRelease.NotamDSPRemark;
            release.NotamCPTRemark = DSPRelease.NotamCPTRemark;
            release.ComputedFligthPlanDSP = DSPRelease.ComputedFligthPlanDSP;
            release.ComputedFligthPlanCPT = DSPRelease.ComputedFligthPlanCPT;
            release.ComputedFligthPlanDSPRemark = DSPRelease.ComputedFligthPlanDSPRemark;
            release.ComputedFligthPlanCPTRemark = DSPRelease.ComputedFligthPlanCPTRemark;
            release.ATCFlightPlanDSP = DSPRelease.ATCFlightPlanDSP;
            release.ATCFlightPlanCPT = DSPRelease.ATCFlightPlanCPT;
            release.ATCFlightPlanDSPRemark = DSPRelease.ATCFlightPlanDSPRemark;
            release.ATCFlightPlanCPTRemark = DSPRelease.ATCFlightPlanCPTRemark;
            release.PermissionsDSP = DSPRelease.PermissionsDSP;
            release.PermissionsCPT = DSPRelease.PermissionsCPT;
            release.PermissionsDSPRemark = DSPRelease.PermissionsDSPRemark;
            release.PermissionsCPTRemark = DSPRelease.PermissionsCPTRemark;
            release.JeppesenAirwayManualDSP = DSPRelease.JeppesenAirwayManualDSP;
            release.JeppesenAirwayManualCPT = DSPRelease.JeppesenAirwayManualCPT;
            release.JeppesenAirwayManualDSPRemark = DSPRelease.JeppesenAirwayManualDSPRemark;
            release.JeppesenAirwayManualCPTRemark = DSPRelease.JeppesenAirwayManualCPTRemark;
            release.MinFuelRequiredDSP = DSPRelease.MinFuelRequiredDSP;
            release.MinFuelRequiredCPT = DSPRelease.MinFuelRequiredCPT;
            release.MinFuelRequiredCFP = DSPRelease.MinFuelRequiredCFP;
            release.MinFuelRequiredPilotReq = DSPRelease.MinFuelRequiredPilotReq;
            release.GeneralDeclarationDSP = DSPRelease.GeneralDeclarationDSP;
            release.GeneralDeclarationCPT = DSPRelease.GeneralDeclarationCPT;
            release.GeneralDeclarationDSPRemark = DSPRelease.GeneralDeclarationDSPRemark;
            release.GeneralDeclarationCPTRemark = DSPRelease.GeneralDeclarationCPTRemark;
            release.FlightReportDSP = DSPRelease.FlightReportDSP;
            release.FlightReportCPT = DSPRelease.FlightReportCPT;
            release.FlightReportDSPRemark = DSPRelease.FlightReportDSPRemark;
            release.FlightReportCPTRemark = DSPRelease.FlightReportCPTRemark;
            release.TOLndCardsDSP = DSPRelease.TOLndCardsDSP;
            release.TOLndCardsCPT = DSPRelease.TOLndCardsCPT;
            release.TOLndCardsDSPRemark = DSPRelease.TOLndCardsDSPRemark;
            release.TOLndCardsCPTRemark = DSPRelease.TOLndCardsCPTRemark;
            release.LoadSheetDSP = DSPRelease.LoadSheetDSP;
            release.LoadSheetCPT = DSPRelease.LoadSheetCPT;
            release.LoadSheetDSPRemark = DSPRelease.LoadSheetDSPRemark;
            release.LoadSheetCPTRemark = DSPRelease.LoadSheetCPTRemark;
            release.FlightSafetyReportDSP = DSPRelease.FlightSafetyReportDSP;
            release.FlightSafetyReportCPT = DSPRelease.FlightSafetyReportCPT;
            release.FlightSafetyReportDSPRemark = DSPRelease.FlightSafetyReportDSPRemark;
            release.FlightSafetyReportCPTRemark = DSPRelease.FlightSafetyReportCPTRemark;
            release.AVSECIncidentReportDSP = DSPRelease.AVSECIncidentReportDSP;
            release.AVSECIncidentReportCPT = DSPRelease.AVSECIncidentReportCPT;
            release.AVSECIncidentReportDSPRemark = DSPRelease.AVSECIncidentReportDSPRemark;
            release.AVSECIncidentReportCPTRemark = DSPRelease.AVSECIncidentReportCPTRemark;
            release.OperationEngineeringDSP = DSPRelease.OperationEngineeringDSP;
            release.OperationEngineeringCPT = DSPRelease.OperationEngineeringCPT;
            release.OperationEngineeringDSPRemark = DSPRelease.OperationEngineeringDSPRemark;
            release.OperationEngineeringCPTRemark = DSPRelease.OperationEngineeringCPTRemark;
            release.VoyageReportDSP = DSPRelease.VoyageReportDSP;
            release.VoyageReportCPT = DSPRelease.VoyageReportCPT;
            release.VoyageReportDSPRemark = DSPRelease.VoyageReportDSPRemark;
            release.VoyageReportCPTRemark = DSPRelease.VoyageReportCPTRemark;
            release.PIFDSP = DSPRelease.PIFDSP;
            release.PIFCPT = DSPRelease.PIFCPT;
            release.PIFDSPRemark = DSPRelease.PIFDSPRemark;
            release.PIFCPTRemark = DSPRelease.PIFCPTRemark;
            release.GoodDeclarationDSP = DSPRelease.GoodDeclarationDSP;
            release.GoodDeclarationCPT = DSPRelease.GoodDeclarationCPT;
            release.GoodDeclarationDSPRemark = DSPRelease.GoodDeclarationDSPRemark;
            release.GoodDeclarationCPTRemark = DSPRelease.GoodDeclarationCPTRemark;
            release.IPADDSP = DSPRelease.IPADDSP;
            release.IPADCPT = DSPRelease.IPADCPT;
            release.IPADDSPRemark = DSPRelease.IPADDSPRemark;
            release.IPADCPTRemark = DSPRelease.IPADCPTRemark;
            release.DateConfirmed = DSPRelease.DateConfirmed;
            release.DispatcherId = DSPRelease.DispatcherId;
            context.SaveChanges();
            return Ok(release);


        }

        [Route("api/dsp/dr/save")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostDRDSP(DSPReleaseViewModel2 DSPRelease)
        {
            try
            {
                var context = new Models.dbEntities();

                var appleg = context.XAppLegs.FirstOrDefault(q => q.FlightId == DSPRelease.FlightId);
                var appcrewflight = context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefault();
                var fdpitems = context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToList();
                var fltIds = fdpitems.Select(q => q.FlightId).ToList();

                foreach(var flt_id in fltIds)
                {
                    var release = context.EFBDSPReleases.FirstOrDefault(q => q.FlightId == flt_id);
                    if (release == null)
                    {
                        release = new EFBDSPRelease();
                        context.EFBDSPReleases.Add(release);

                    }

                    // release.User = DSPRelease.User;



                    release.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm");


                    release.FlightId = flt_id;
                    release.ActualWXDSP = DSPRelease.ActualWXDSP;
                    // release.ActualWXCPT = DSPRelease.ActualWXCPT;
                    release.ActualWXDSPRemark = DSPRelease.ActualWXDSPRemark;
                    //release.ActualWXCPTRemark = DSPRelease.ActualWXCPTRemark;
                    release.WXForcastDSP = DSPRelease.WXForcastDSP;
                    //release.WXForcastCPT = DSPRelease.WXForcastCPT;
                    release.WXForcastDSPRemark = DSPRelease.WXForcastDSPRemark;
                    //release.WXForcastCPTRemark = DSPRelease.WXForcastCPTRemark;
                    release.SigxWXDSP = DSPRelease.SigxWXDSP;
                    //release.SigxWXCPT = DSPRelease.SigxWXCPT;
                    release.SigxWXDSPRemark = DSPRelease.SigxWXDSPRemark;
                    //release.SigxWXCPTRemark = DSPRelease.SigxWXCPTRemark;
                    release.WindChartDSP = DSPRelease.WindChartDSP;
                    //release.WindChartCPT = DSPRelease.WindChartCPT;
                    release.WindChartDSPRemark = DSPRelease.WindChartDSPRemark;
                    //release.WindChartCPTRemark = DSPRelease.WindChartCPTRemark;
                    release.NotamDSP = DSPRelease.NotamDSP;
                    //release.NotamCPT = DSPRelease.NotamCPT;
                    release.NotamDSPRemark = DSPRelease.NotamDSPRemark;
                    //release.NotamCPTRemark = DSPRelease.NotamCPTRemark;
                    release.ComputedFligthPlanDSP = DSPRelease.ComputedFligthPlanDSP;
                    //release.ComputedFligthPlanCPT = DSPRelease.ComputedFligthPlanCPT;
                    release.ComputedFligthPlanDSPRemark = DSPRelease.ComputedFligthPlanDSPRemark;
                    //release.ComputedFligthPlanCPTRemark = DSPRelease.ComputedFligthPlanCPTRemark;
                    release.ATCFlightPlanDSP = DSPRelease.ATCFlightPlanDSP;
                    //release.ATCFlightPlanCPT = DSPRelease.ATCFlightPlanCPT;
                    release.ATCFlightPlanDSPRemark = DSPRelease.ATCFlightPlanDSPRemark;
                    //release.ATCFlightPlanCPTRemark = DSPRelease.ATCFlightPlanCPTRemark;
                    release.PermissionsDSP = DSPRelease.PermissionsDSP;
                    //release.PermissionsCPT = DSPRelease.PermissionsCPT;
                    release.PermissionsDSPRemark = DSPRelease.PermissionsDSPRemark;
                    //release.PermissionsCPTRemark = DSPRelease.PermissionsCPTRemark;
                    release.JeppesenAirwayManualDSP = DSPRelease.JeppesenAirwayManualDSP;
                    //release.JeppesenAirwayManualCPT = DSPRelease.JeppesenAirwayManualCPT;
                    release.JeppesenAirwayManualDSPRemark = DSPRelease.JeppesenAirwayManualDSPRemark;
                    //release.JeppesenAirwayManualCPTRemark = DSPRelease.JeppesenAirwayManualCPTRemark;
                    release.MinFuelRequiredDSP = DSPRelease.MinFuelRequiredDSP;
                    //release.MinFuelRequiredCPT = DSPRelease.MinFuelRequiredCPT;
                    //release.MinFuelRequiredCFP = DSPRelease.MinFuelRequiredCFP;
                    //release.MinFuelRequiredPilotReq = DSPRelease.MinFuelRequiredPilotReq;
                    release.GeneralDeclarationDSP = DSPRelease.GeneralDeclarationDSP;
                    //release.GeneralDeclarationCPT = DSPRelease.GeneralDeclarationCPT;
                    release.GeneralDeclarationDSPRemark = DSPRelease.GeneralDeclarationDSPRemark;
                    //release.GeneralDeclarationCPTRemark = DSPRelease.GeneralDeclarationCPTRemark;
                    release.FlightReportDSP = DSPRelease.FlightReportDSP;
                    //release.FlightReportCPT = DSPRelease.FlightReportCPT;
                    release.FlightReportDSPRemark = DSPRelease.FlightReportDSPRemark;
                    //release.FlightReportCPTRemark = DSPRelease.FlightReportCPTRemark;
                    release.TOLndCardsDSP = DSPRelease.TOLndCardsDSP;
                    //release.TOLndCardsCPT = DSPRelease.TOLndCardsCPT;
                    release.TOLndCardsDSPRemark = DSPRelease.TOLndCardsDSPRemark;
                    //release.TOLndCardsCPTRemark = DSPRelease.TOLndCardsCPTRemark;
                    release.LoadSheetDSP = DSPRelease.LoadSheetDSP;
                    //release.LoadSheetCPT = DSPRelease.LoadSheetCPT;
                    release.LoadSheetDSPRemark = DSPRelease.LoadSheetDSPRemark;
                    //release.LoadSheetCPTRemark = DSPRelease.LoadSheetCPTRemark;
                    release.FlightSafetyReportDSP = DSPRelease.FlightSafetyReportDSP;
                    //release.FlightSafetyReportCPT = DSPRelease.FlightSafetyReportCPT;
                    release.FlightSafetyReportDSPRemark = DSPRelease.FlightSafetyReportDSPRemark;
                    //release.FlightSafetyReportCPTRemark = DSPRelease.FlightSafetyReportCPTRemark;
                    release.AVSECIncidentReportDSP = DSPRelease.AVSECIncidentReportDSP;
                    //release.AVSECIncidentReportCPT = DSPRelease.AVSECIncidentReportCPT;
                    release.AVSECIncidentReportDSPRemark = DSPRelease.AVSECIncidentReportDSPRemark;
                    //release.AVSECIncidentReportCPTRemark = DSPRelease.AVSECIncidentReportCPTRemark;
                    release.OperationEngineeringDSP = DSPRelease.OperationEngineeringDSP;
                    //release.OperationEngineeringCPT = DSPRelease.OperationEngineeringCPT;
                    release.OperationEngineeringDSPRemark = DSPRelease.OperationEngineeringDSPRemark;
                    //release.OperationEngineeringCPTRemark = DSPRelease.OperationEngineeringCPTRemark;
                    release.VoyageReportDSP = DSPRelease.VoyageReportDSP;
                    //release.VoyageReportCPT = DSPRelease.VoyageReportCPT;

                    //release.VoyageReportCPTRemark = DSPRelease.VoyageReportCPTRemark;
                    release.PIFDSP = DSPRelease.PIFDSP;
                    //release.PIFCPT = DSPRelease.PIFCPT;
                    release.PIFDSPRemark = DSPRelease.PIFDSPRemark;
                    //release.PIFCPTRemark = DSPRelease.PIFCPTRemark;
                    release.GoodDeclarationDSP = DSPRelease.GoodDeclarationDSP;
                    // release.GoodDeclarationCPT = DSPRelease.GoodDeclarationCPT;
                    release.GoodDeclarationDSPRemark = DSPRelease.GoodDeclarationDSPRemark;
                    //  release.GoodDeclarationCPTRemark = DSPRelease.GoodDeclarationCPTRemark;
                    release.IPADDSP = DSPRelease.IPADDSP;
                    //release.IPADCPT = DSPRelease.IPADCPT;
                    release.IPADDSPRemark = DSPRelease.IPADDSPRemark;
                    //release.IPADCPTRemark = DSPRelease.IPADCPTRemark;



                    //release.DateConfirmed = DateTime.Now; //DSPRelease.DateConfirmed;
                    // release.DispatcherId = DSPRelease.DispatcherId;


                    release.VoyageReportDSPRemark = DSPRelease.User;
                }

              


                context.SaveChanges();
                return Ok(fltIds);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }

        public class dto_sign
        {
            public int flight_id { get; set; }
            public string flight_id_str { get; set; }
            public string lic_no { get; set; }
            public string user_id { get; set; }
        }
        //[Route("api/dsp/dr/sign/new")]
        //[AcceptVerbs("Post")]
        //public IHttpActionResult PostDRDSPSIGNNew(dto_sign dto)
        //{
        //    try
        //    {
        //        var do_lic = Convert.ToInt32(ConfigurationManager.AppSettings["dsp_lic"]);
        //        var context = new Models.dbEntities();

        //        int flight_id = Convert.ToInt32(dto.flight_id);
        //        string lic_no = Convert.ToString(dto.lic_no);
        //        string userid = Convert.ToString(dto.user_id);


        //         ViewEmployee employee = null;
        //        //try
        //        //{
        //            employee = context.ViewEmployees.Where(q => q.UserId == userid || q.PersonId.ToString() == userid).FirstOrDefault();
        //        //}
        //        //catch(Exception ex)
        //        //{
        //        //    employee = context.ViewEmployees.Where(q => q.PersonId.ToString() == userid).FirstOrDefault();
        //        //}
        //        //employee = context.ViewEmployees.Where(q => q.PersonId.ToString() == userid).FirstOrDefault();

        //        //if (employee==null)
        //        //   employee=context.ViewEmployees.Where(q=>q.PersonId==userid).
                 
        //        if (do_lic == 1)
        //        {
        //            if (employee != null)
        //            {
        //                if (!employee.LicenceTitle.ToLower().Contains(lic_no.ToLower()))
        //                {
        //                    return Ok(
        //                        new
        //                        {
        //                            done = false,
        //                            code = 100,
        //                            message = "The license number is wrong."
        //                        }
        //                    );
        //                }
        //            }
        //            else
        //            {
        //                if (lic_no.ToLower() != "lic4806")
        //                {
        //                    return Ok(
        //                        new
        //                        {
        //                            done = false,
        //                            code = 100,
        //                            message = "The license number is wrong."
        //                        }
        //                    );
        //                }
        //            }
        //        }
                


        //        var appleg = context.XAppLegs.FirstOrDefault(q => q.FlightId == flight_id);
        //        if (appleg.PICId==null)
        //        {
        //            return Ok(
        //                    new
        //                    {
        //                        done = false,
        //                        code = 100,
        //                        message = "The Flight Crew not found"
        //                    }
        //                );
        //        }
        //        var appcrewflight = context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefault();
        //        var fdpitems = context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToList();
        //        var fltIds = fdpitems.Select(q => q.FlightId).ToList();

        //        //var ofp_req_fuel = context.FlightInformations.Where(q => fltIds.Contains(q.ID)).OrderBy(q => q.ChocksOut).FirstOrDefault();
        //        //if (ofp_req_fuel != null)
        //        //{
        //        //    if (ofp_req_fuel.FuelPlanned==null || ofp_req_fuel.OFPTOTALFUEL == null)
        //        //    {
        //        //        return Ok(
        //        //            new
        //        //            {
        //        //                done = false,
        //        //                code = 200,
        //        //                message = "The OFP FUEL and REQUESTED FUEL can not be empty."
        //        //            }
        //        //        );
        //        //    }
        //        //}

        //        var drs = context.EFBDSPReleases.Where(q => fltIds.Contains(q.FlightId)).ToList();

        //        var dt = DateTime.UtcNow;
        //        foreach (var dr in drs)
        //        {
        //            dr.JLDSPSignDate = dt;
        //            dr.SgnDSPLicNo = lic_no.ToUpper();
        //            dr.DispatcherId = employee != null ? employee.Id : -1;
        //            dr.SGNDSPName = employee != null ? employee.Name : "Dispatch User";
        //        }



        //        context.SaveChanges();
        //        return Ok(new
        //        {
        //            done = true,
        //            dr_ids = drs.Select(q => q.Id).ToList(),
        //            flt_ids = drs.Select(q => q.FlightId).ToList()
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += "   INNER: " + ex.InnerException.Message;
        //        return Ok(new
        //        {
        //            done = false,
        //            code = 1,
        //            message = msg,
        //        });
        //    }

        //}


        //FLY
        [Route("api/dsp/dr/sign/new")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostDRDSPSIGNNewFly(dto_sign dto)
        {
            try
            {
                var do_lic = Convert.ToInt32(ConfigurationManager.AppSettings["dsp_lic"]);
                var context = new Models.dbEntities();

                int flight_id = Convert.ToInt32(dto.flight_id);
                string lic_no = Convert.ToString(dto.lic_no);
                string userid = Convert.ToString(dto.user_id);


                
                //try
                //{
                var person = context.People.Where(q =>   q.Id.ToString() == userid).FirstOrDefault();
                var employee = context.PersonCustomers.Where(q => q.PersonId == person.Id).FirstOrDefault();
                //}
                //catch(Exception ex)
                //{
                //    employee = context.ViewEmployees.Where(q => q.PersonId.ToString() == userid).FirstOrDefault();
                //}
                //employee = context.ViewEmployees.Where(q => q.PersonId.ToString() == userid).FirstOrDefault();

                //if (employee==null)
                //   employee=context.ViewEmployees.Where(q=>q.PersonId==userid).

                if (do_lic == 1)
                {
                    if (employee != null)
                    {
                        if (!person.LicenceTitle.ToLower().Contains(lic_no.ToLower()))
                        {
                            return Ok(
                                new
                                {
                                    done = false,
                                    code = 100,
                                    message = "The license number is wrong."
                                }
                            );
                        }
                    }
                    else
                    {
                        if (lic_no.ToLower() != "lic4806")
                        {
                            return Ok(
                                new
                                {
                                    done = false,
                                    code = 100,
                                    message = "The license number is wrong."
                                }
                            );
                        }
                    }
                }



                var appleg = context.XAppLegs.FirstOrDefault(q => q.FlightId == flight_id);
                if (appleg.PICId == null)
                {
                    return Ok(
                            new
                            {
                                done = false,
                                code = 100,
                                message = "The Flight Crew not found"
                            }
                        );
                }
                var appcrewflight = context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefault();
                var fdpitems = context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToList();
                var fltIds = fdpitems.Select(q => q.FlightId).ToList();

                //var ofp_req_fuel = context.FlightInformations.Where(q => fltIds.Contains(q.ID)).OrderBy(q => q.ChocksOut).FirstOrDefault();
                //if (ofp_req_fuel != null)
                //{
                //    if (ofp_req_fuel.FuelPlanned==null || ofp_req_fuel.OFPTOTALFUEL == null)
                //    {
                //        return Ok(
                //            new
                //            {
                //                done = false,
                //                code = 200,
                //                message = "The OFP FUEL and REQUESTED FUEL can not be empty."
                //            }
                //        );
                //    }
                //}

                var drs = context.EFBDSPReleases.Where(q => fltIds.Contains(q.FlightId)).ToList();

                var dt = DateTime.UtcNow;
                foreach (var dr in drs)
                {
                    dr.JLDSPSignDate = dt;
                    dr.SgnDSPLicNo = lic_no.ToUpper();
                    dr.DispatcherId = employee != null ? employee.Id : -1;
                    dr.SGNDSPName = employee != null ? person.LastName+" "+person.FirstName : "Dispatch User";
                }



                context.SaveChanges();
                return Ok(new
                {
                    done = true,
                    dr_ids = drs.Select(q => q.Id).ToList(),
                    flt_ids = drs.Select(q => q.FlightId).ToList()
                });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                return Ok(new
                {
                    done = false,
                    code = 1,
                    message = msg,
                });
            }

        }


        [Route("api/sign/ofps/new")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostSIGNOfps (dto_sign dto)
        {
            try
            {
                var context = new Models.dbEntities();

                var fids = Convert.ToString(dto.flight_id_str).Split('_').Select(q =>(Nullable<int>) Convert.ToInt32(q)).ToList();

                
                string lic_no = Convert.ToString(dto.lic_no);
                string userid = Convert.ToString(dto.user_id);



                var employee = context.ViewEmployees.Where(q => q.UserId == userid).FirstOrDefault();
                if (employee != null)
                {
                    if (!employee.NDTNumber.ToLower().Contains(lic_no.ToLower()))
                    {
                         
                        return Ok(new DataResponse() { IsSuccess = false, Messages = new List<string>() { "The license number is wrong." } });
                    }
                }
                else
                {
                    if (lic_no.ToLower() != "lic4806")
                    {
                        return Ok(new DataResponse() { IsSuccess = false, Messages = new List<string>() { "The license number is wrong." } });
                    }
                }

                List<sgn_ofp_result> sgn_result = new List<sgn_ofp_result>();
                var ofps = context.OFPImports.Where(q => fids.Contains(q.FlightId)).ToList();
                foreach(var ofp in ofps)
                {
                    ofp.PIC =employee!=null? employee.Name: lic_no;
                    ofp.PICId = employee.Id;
                    ofp.JLDatePICApproved = DateTime.UtcNow;
                    ofp.JLSignedBy = lic_no;

                    sgn_result.Add(new sgn_ofp_result() {
                     FlightId=(int)ofp.FlightId,
                      Id=ofp.Id,
                       JLDatePICApproved=(DateTime)ofp.JLDatePICApproved,
                        JLSignedBy=ofp.JLSignedBy,
                         PIC=ofp.PIC,
                          PICId=(int)ofp.PICId
                    });
                     
                }

                context.SaveChanges();
                return Ok(new DataResponse() { IsSuccess = true, Data = sgn_result });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                return Ok(new DataResponse() { IsSuccess = false, Messages=new List<string>() {msg } });
            }

        }
        public class sgn_ofp_result
        {
            public int Id { get; set; }
            public int FlightId { get; set; }
            public int PICId { get; set; }
            public string JLSignedBy { get; set; }
            public DateTime JLDatePICApproved { get; set; }
            public string PIC { get; set; }
        }
        //[Route("api/pic/dr/sign/new")]
        //[AcceptVerbs("Post")]
        //public IHttpActionResult PostDRPICSIGNNew(dto_sign dto)
        //{
        //    try
        //    {
        //        var context = new Models.dbEntities();

        //        int flight_id = Convert.ToInt32(dto.flight_id);
        //        string lic_no = Convert.ToString(dto.lic_no);
        //        string userid = Convert.ToString(dto.user_id);



        //        var employee = context.ViewEmployees.Where(q => q.UserId == userid).FirstOrDefault();
        //        if (employee != null)
        //        {
        //            if (!employee.NDTNumber.ToLower().Contains(lic_no.ToLower()))
        //            {
        //                return Ok(
        //                    new
        //                    {
        //                        IsSuccess = false,
        //                        code = 100,
        //                        message = "The license number is wrong."
        //                    }
        //                );
        //            }
        //        }
        //        else
        //        {
        //            if (lic_no.ToLower() != "lic4806")
        //            {
        //                return Ok(
        //                    new
        //                    {
        //                        // done = false,
        //                        IsSuccess = false,
        //                        code = 100,
        //                        message = "The license number is wrong."
        //                    }
        //                );
        //            }
        //        }


        //        var appleg = context.XAppLegs.FirstOrDefault(q => q.FlightId == flight_id);
        //        var appcrewflight = context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefault();
        //        var fdpitems = context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToList();
        //        var fltIds = fdpitems.Select(q => q.FlightId).ToList();

        //        var drs = context.EFBDSPReleases.Where(q => fltIds.Contains(q.FlightId)).ToList();

        //        var dt = DateTime.UtcNow;
        //        foreach (var dr in drs)
        //        {
        //            dr.JLDatePICApproved = dt;
        //            dr.SgnCPTLicNo = lic_no.ToUpper();
        //            dr.PICId = employee != null ? employee.Id : -1;
        //            dr.PIC = employee != null ? employee.Name : "PIC";
        //            dr.JLSignedBy = employee != null ? employee.Name : "PIC";
        //        }



        //        context.SaveChanges();
        //        var rdr = drs.Where(q => q.FlightId == flight_id).FirstOrDefault();

        //        var result =new   { IsSuccess = true, Data = new { rdr.Id, rdr.FlightId, rdr.PICId, rdr.JLSignedBy, rdr.JLDatePICApproved, rdr.PIC,rdr.SgnCPTLicNo } };
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += "   INNER: " + ex.InnerException.Message;
        //        //return Ok(new
        //        //{
        //        //    done = false,
        //        //    code = 1,
        //        //    message = msg,
        //        //});
        //        return Ok(new { IsSuccess = false, message = msg });
        //    }

        //}


        //fly
        [Route("api/pic/dr/sign/new")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostDRPICSIGNNew(dto_sign dto)
        {
            try
            {
                var context = new Models.dbEntities();

                int flight_id = Convert.ToInt32(dto.flight_id);
                string lic_no = Convert.ToString(dto.lic_no);
                string userid = Convert.ToString(dto.user_id);


                var person = context.People.Where(q => q.UserId == userid).FirstOrDefault();
                var employee = context.PersonCustomers.Where(q => q.PersonId == person.Id).FirstOrDefault();
                if (employee != null)
                {
                    if (!person.NDTNumber.ToLower().Contains(lic_no.ToLower()))
                    {
                        return Ok(
                            new
                            {
                                IsSuccess = false,
                                code = 100,
                                message = "The license number is wrong."
                            }
                        );
                    }
                }
                else
                {
                    if (lic_no.ToLower() != "lic4806")
                    {
                        return Ok(
                            new
                            {
                                // done = false,
                                IsSuccess = false,
                                code = 100,
                                message = "The license number is wrong."
                            }
                        );
                    }
                }


                var appleg = context.XAppLegs.FirstOrDefault(q => q.FlightId == flight_id);
                var appcrewflight = context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefault();
                var fdpitems = context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToList();
                var fltIds = fdpitems.Select(q => q.FlightId).ToList();

                var drs = context.EFBDSPReleases.Where(q => fltIds.Contains(q.FlightId)).ToList();

                var dt = DateTime.UtcNow;
                foreach (var dr in drs)
                {
                    dr.JLDatePICApproved = dt;
                    dr.SgnCPTLicNo = lic_no.ToUpper();
                    dr.PICId = employee != null ? employee.Id : -1;
                    dr.PIC = employee != null ? person.LastName+" "+person.FirstName : "PIC";
                    dr.JLSignedBy = employee != null ? person.LastName + " " + person.FirstName : "PIC";
                }



                context.SaveChanges();
                var rdr = drs.Where(q => q.FlightId == flight_id).FirstOrDefault();

                var result = new { IsSuccess = true, Data = new { rdr.Id, rdr.FlightId, rdr.PICId, rdr.JLSignedBy, rdr.JLDatePICApproved, rdr.PIC, rdr.SgnCPTLicNo } };
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                //return Ok(new
                //{
                //    done = false,
                //    code = 1,
                //    message = msg,
                //});
                return Ok(new { IsSuccess = false, message = msg });
            }

        }


        [Route("api/pic/asr/sign/new")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostASRPICSIGNNew(dto_sign dto)
        {
            try
            {
                var context = new Models.dbEntities();

                int flight_id = Convert.ToInt32(dto.flight_id);
                string lic_no = Convert.ToString(dto.lic_no);
                string userid = Convert.ToString(dto.user_id);



                var employee = context.ViewEmployees.Where(q => q.UserId == userid).FirstOrDefault();
                if (employee != null)
                {
                    if (!employee.NDTNumber.ToLower().Contains(lic_no.ToLower()))
                    {
                        return Ok(
                            new
                            {
                                IsSuccess = false,
                                code = 100,
                                message = "The license number is wrong."
                            }
                        );
                    }
                }
                else
                {
                    if (lic_no.ToLower() != "lic4806")
                    {
                        return Ok(
                            new
                            {
                                // done = false,
                                IsSuccess = false,
                                code = 100,
                                message = "The license number is wrong."
                            }
                        );
                    }
                }


                var appleg = context.XAppLegs.FirstOrDefault(q => q.FlightId == flight_id);
                // var appcrewflight = context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefault();
                // var fdpitems = context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToList();
                //  var fltIds = fdpitems.Select(q => q.FlightId).ToList();
                var dt = DateTime.UtcNow;
                var asr = context.EFBASRs.Where(q => q.FlightId==flight_id).FirstOrDefault();
                asr.JLSignedBy = employee != null ? employee.Name : "PIC";
                asr.JLDatePICApproved = dt;
                asr.PICId = employee != null ? employee.Id : -1;
                asr.PIC = employee != null ? employee.Name : "PIC";
                
                



                context.SaveChanges();
                //var rdr = drs.Where(q => q.FlightId == flight_id).FirstOrDefault();

                var result = new { IsSuccess = true, Data = new { asr.Id, asr.FlightId, asr.PICId, asr.JLSignedBy, asr.JLDatePICApproved, asr.PIC  } };
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                //return Ok(new
                //{
                //    done = false,
                //    code = 1,
                //    message = msg,
                //});
                return Ok(new { IsSuccess = false, message = msg });
            }

        }


        [Route("api/pic/vr/sign/new")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostVRPICSIGNNew(dto_sign dto)
        {
            try
            {
                var context = new Models.dbEntities();

                int flight_id = Convert.ToInt32(dto.flight_id);
                string lic_no = Convert.ToString(dto.lic_no);
                string userid = Convert.ToString(dto.user_id);



                var employee = context.ViewEmployees.Where(q => q.UserId == userid).FirstOrDefault();
                if (employee != null)
                {
                    if (!employee.NDTNumber.ToLower().Contains(lic_no.ToLower()))
                    {
                        return Ok(
                            new
                            {
                                IsSuccess = false,
                                code = 100,
                                message = "The license number is wrong."
                            }
                        );
                    }
                }
                else
                {
                    if (lic_no.ToLower() != "lic4806")
                    {
                        return Ok(
                            new
                            {
                                // done = false,
                                IsSuccess = false,
                                code = 100,
                                message = "The license number is wrong."
                            }
                        );
                    }
                }


                var appleg = context.XAppLegs.FirstOrDefault(q => q.FlightId == flight_id);
                // var appcrewflight = context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefault();
                // var fdpitems = context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToList();
                //  var fltIds = fdpitems.Select(q => q.FlightId).ToList();
                var dt = DateTime.UtcNow;
                var asr = context.EFBVoyageReports.Where(q => q.FlightId == flight_id).FirstOrDefault();
                asr.JLSignedBy = employee != null ? employee.Name : "PIC";
                asr.JLDatePICApproved = dt;
                asr.PICId = employee != null ? employee.Id : -1;
                asr.PIC = employee != null ? employee.Name : "PIC";





                context.SaveChanges();
                //var rdr = drs.Where(q => q.FlightId == flight_id).FirstOrDefault();

                var result = new { IsSuccess = true, Data = new { asr.Id, asr.FlightId, asr.PICId, asr.JLSignedBy, asr.JLDatePICApproved, asr.PIC } };
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                //return Ok(new
                //{
                //    done = false,
                //    code = 1,
                //    message = msg,
                //});
                return Ok(new { IsSuccess = false, message = msg });
            }

        }


        [Route("api/dsp/dr/sign")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostDRDSPSIGN(dynamic dto)
        {
            try
            {
                var context = new Models.dbEntities();

                int flightId = Convert.ToInt32(dto.flightId);
                int empId = Convert.ToInt32(dto.user);
                var release = context.EFBDSPReleases.FirstOrDefault(q => q.FlightId == flightId);


                string name = Convert.ToString(dto.picStr);

                release.DispatcherId = empId;
                release.DateConfirmed = DateTime.Now;

                context.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }

        public class _h_prop
        {
            public string name { get; set; }
            public string value { get; set; }
            public int id { get; set; }

        }

        public class _h_error
        {
            public bool toc { get; set; }
            public bool tod { get; set; }
            public bool toc_tod { get; set; }
            public string flight_no { get; set; }
            public string route { get; set; }
            public string other { get; set; }
            public bool rvsm_grnd { get; set; }
            public bool rvsm_prelevel { get; set; }
            public bool rvsm_flight { get; set; }
            public bool takeoff { get; set; }
            public int? id { get; set; }
            public bool has_error
            {
                get
                {
                    return toc || tod || toc_tod || rvsm_flight || rvsm_flight || rvsm_prelevel || takeoff;
                         
                }
            }

        }
        [Route("api/ofps/validate/{fids}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult ValidateOFPs(string fids)
        {
            var rvsm_check = true;
                List<int?> _fids = fids.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();

            var _context = new Models.dbEntities();
            var _msgs = new List<string>();
            try
            {
                var ofpImports =   _context.OFPImports.Where(q => _fids.Contains(q.FlightId)).Select(q => new
                {
                    q.FlightId,
                    q.FlightNo,
                    q.Origin,
                    q.Destination,
                    q.Id
                }).ToList ();

                var flight_ids = ofpImports.Select(q => q.FlightId).ToList();
                var flights =   _context.FlightInformations.Where(q => flight_ids.Contains(q.ID)).ToList ();



                var ofpImportIds = ofpImports.Select(q => q.Id).ToList();

                var ofpProps =    _context.OFPImportProps.Where(q => (q.PropName.Contains("mpln") || q.PropName.Contains("rvsm")) && ofpImportIds.Contains(q.OFPId)).ToList ();

                var groupProps = (from x in ofpProps
                                  group x by new { x.OFPId } into grp
                                  select new
                                  {
                                      grp.Key.OFPId,
                                      ofp = ofpImports.FirstOrDefault(q => q.Id == grp.Key.OFPId),
                                     // flight=flights.FirstOrDefault(q=>q.ID== ofpImports.FirstOrDefault(q => q.Id == grp.Key.OFPId).FlightId),
                                      items = grp.OrderBy(q => q.Id).Select(q => new _h_prop() { name = q.PropName, value = q.PropValue,id=q.Id }).ToList(),

                                  }).ToList();

                List<_h_error> errors = new List<_h_error>();
                foreach (var _flt in groupProps)
                {
                    errors.Add(new _h_error()
                    {
                        flight_no = _flt.ofp.FlightNo,
                         id = _flt.ofp.FlightId,

                    }) ;
                     
                }





                foreach (var x in groupProps)
                {
                    /*var first_point = x.items.ToList().OrderBy(q => q.id).FirstOrDefault();
                    if (first_point != null)
                    {
                        var origin = first_point.name.Substring(0, 18);
                        var takeoff_point = x.items.Where(q => q.name.StartsWith(origin) && q.name.Contains("_rem")).FirstOrDefault();
                        if (string.IsNullOrEmpty(takeoff_point.value))
                        {
                            //errors.Add(new _h_error()
                            //{
                            //    flight_no = x.ofp.FlightNo,
                            //    route = x.ofp.Origin + "-" + x.ofp.Destination,
                            //    takeoff=true,
                            //});
                            var _err = errors.Where(q => q.flight_no == x.ofp.FlightNo).FirstOrDefault();
                            if (_err != null)
                            {
                                _err.takeoff = true;
                            }
                        }
                    }*/
                    var _toc = x.items.Where(q => q.name.Contains("toc_ata")).FirstOrDefault();
                    var _tod = x.items.Where(q => q.name.Contains("tod_ata")).FirstOrDefault();
                    if (string.IsNullOrEmpty(_toc.value) || string.IsNullOrEmpty(_tod.value))
                    {
                        //errors.Add(new _h_error()
                        //{
                        //    flight_no = x.ofp.FlightNo,
                        //    route = x.ofp.Origin + "-" + x.ofp.Destination,
                        //    toc = string.IsNullOrEmpty(_toc.value),
                        //    tod = string.IsNullOrEmpty(_tod.value),
                        //});
                        var _err = errors.Where(q => q.flight_no == x.ofp.FlightNo).FirstOrDefault();
                        if (_err != null)
                        {
                            _err.toc = string.IsNullOrEmpty(_toc.value);
                            _err.tod = string.IsNullOrEmpty(_tod.value);
                        }
                    }
                    else
                    {
                        var _toc_idx = x.items.IndexOf(_toc) + 1;
                        var _tod_idx = x.items.IndexOf(_tod) - 1;
                        var _toc_tod_items = x.items.Skip(_toc_idx).Take(_tod_idx - _toc_idx).Where(q => q.name.Contains("_ata")).ToList();
                        var _toc_time = DateTime.Now.Date.AddHours(Convert.ToInt32(_toc.value.Substring(0, 2))).AddMinutes(Convert.ToInt32(_toc.value.Substring(2, 2)));
                        var _tod_time = DateTime.Now.Date.AddHours(Convert.ToInt32(_tod.value.Substring(0, 2))).AddMinutes(Convert.ToInt32(_tod.value.Substring(2, 2)));

                        // var diff = (_tod_time - _toc_time).TotalMinutes;
                        var _flight_id = ofpImports.FirstOrDefault(q => q.Id == x.OFPId).FlightId;

                        var _flight = flights.FirstOrDefault(q => q.ID == _flight_id);
                        var diff = ((DateTime)_flight.STA - (DateTime)_flight.STD).TotalMinutes;
                        var hrs = (int)Math.Ceiling(diff * 1.0 / 60);
                        var hrs2 = diff * 1.0 / 60;
                        var hrs2_int = Math.Truncate(hrs2);
                        _msgs.Add(diff.ToString());
                        _msgs.Add(hrs2.ToString());
                        _msgs.Add(hrs2_int.ToString());


                        //var filled = _toc_tod_items.Where(q => !string.IsNullOrEmpty(q.value)).Count();
                        var filled = x.items.Where(q => q.name.Contains("_ata") && !string.IsNullOrEmpty(q.value)).Count();
                        _msgs.Add(filled.ToString());

                        if (hrs2 >= 1 && /*filled < hrs - 1*/filled < hrs2_int + 2)
                        {
                            //errors.Add(new _h_error()
                            //{
                            //    flight_no = x.ofp.FlightNo,
                            //    route = x.ofp.Origin + "-" + x.ofp.Destination,
                            //    toc = false,
                            //    tod = false,
                            //    toc_tod = true,
                            //});
                            var _err = errors.Where(q => q.flight_no == x.ofp.FlightNo).FirstOrDefault();
                            if (_err != null)
                            {
                                _err.toc_tod = true;
                                 
                            }
                        }
                    }
                    if (rvsm_check)
                    {
                        var rvsm_grnd_props = x.items.Where(q => q.name.Contains("rvsm_gnd")).ToList();
                        var rvsm_grns_props_null = rvsm_grnd_props.Where(q => string.IsNullOrEmpty(q.value)).Count();
                        if (rvsm_grns_props_null > 0)
                        {
                            //errors.Add(new _h_error()
                            //{
                            //    flight_no = x.ofp.FlightNo,
                            //    route = x.ofp.Origin + "-" + x.ofp.Destination,
                            //    rvsm_grnd = true,

                            //});
                            var _err = errors.Where(q => q.flight_no == x.ofp.FlightNo).FirstOrDefault();
                            if (_err != null)
                            {
                                _err.rvsm_grnd = true;

                            }
                        }


                        //var rvsm_prelevel_props = x.items.Where(q => q.name.Contains("rvsm_flt") && q.name.Contains("4")).ToList();
                        //var rvsm_prelevel_props_null = rvsm_prelevel_props.Where(q => string.IsNullOrEmpty(q.value)).Count();
                        //if (rvsm_prelevel_props_null > 0)
                        //{
                        //    errors.Add(new _h_error()
                        //    {
                        //        flight_no = x.ofp.FlightNo,
                        //        route = x.ofp.Origin + "-" + x.ofp.Destination,
                        //        rvsm_prelevel = true,

                        //    });
                        //}

                        var rvsm_flt_l= x.items.Where(q => q.name.EndsWith("rvsm_flt_l")).FirstOrDefault().value;
                        var rvsm_flt_stby = x.items.Where(q => q.name.EndsWith("rvsm_flt_stby")).FirstOrDefault().value;
                        var rvsm_flt_r = x.items.Where(q => q.name.EndsWith("rvsm_flt_r")).FirstOrDefault().value;
                        var rvsm_flt_time = x.items.Where(q => q.name.EndsWith("rvsm_flt_time")).FirstOrDefault().value;
                        if (string.IsNullOrEmpty(rvsm_flt_l) || string.IsNullOrEmpty(rvsm_flt_stby) || string.IsNullOrEmpty(rvsm_flt_r) || string.IsNullOrEmpty(rvsm_flt_time))
                        {
                            //errors.Add(new _h_error()
                            //{
                            //    flight_no = x.ofp.FlightNo,
                            //    route = x.ofp.Origin + "-" + x.ofp.Destination,
                            //    rvsm_flight = true,

                            //});
                            var _err = errors.Where(q => q.flight_no == x.ofp.FlightNo).FirstOrDefault();
                            if (_err != null)
                            {
                                _err.rvsm_flight = true;

                            }
                        }








                    }
                   



                }



                return Ok( new DataResponse()
                {
                    Data = errors,
                    IsSuccess = true,
                    Messages = _msgs

                });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " INNER:" + ex.InnerException.Message;

                return Ok(new DataResponse()
                {
                    Data = new List<_h_error>() { new _h_error() { other = msg, flight_no = "-", } },
                    IsSuccess = true

                });
            }
        }


        public class DataResponse
        {
            public bool IsSuccess { get; set; }
            public object Data { get; set; }
            public List<string> Errors { get; set; }
            public List<string> Messages { get; set; }
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
        /*  public int? FlightId { get; set; }
            public bool? ActualWXDSP { get; set; }
            public bool? ActualWXCPT { get; set; }
            public string ActualWXDSPRemark { get; set; }
            public string ActualWXCPTRemark { get; set; }
            public bool? WXForcastDSP { get; set; }
            public bool? WXForcastCPT { get; set; }
            public string WXForcastDSPRemark { get; set; }
            public string WXForcastCPTRemark { get; set; }
            public bool? SigxWXDSP { get; set; }
            public bool? SigxWXCPT { get; set; }
            public string SigxWXDSPRemark { get; set; }
            public string SigxWXCPTRemark { get; set; }
            public bool? WindChartDSP { get; set; }
            public bool? WindChartCPT { get; set; }
            public string WindChartDSPRemark { get; set; }
            public string WindChartCPTRemark { get; set; }
            public bool? NotamDSP { get; set; }
            public bool? NotamCPT { get; set; }
            public string NotamDSPRemark { get; set; }
            public string NotamCPTRemark { get; set; }
            public bool? ComputedFligthPlanDSP { get; set; }
            public bool? ComputedFligthPlanCPT { get; set; }
            public string ComputedFligthPlanDSPRemark { get; set; }
            public string ComputedFligthPlanCPTRemark { get; set; }
            public bool? ATCFlightPlanDSP { get; set; }
            public bool? ATCFlightPlanCPT { get; set; }
            public string ATCFlightPlanDSPRemark { get; set; }
            public string ATCFlightPlanCPTRemark { get; set; }
            public bool? PermissionsDSP { get; set; }
            public bool? PermissionsCPT { get; set; }
            public string PermissionsDSPRemark { get; set; }
            public string PermissionsCPTRemark { get; set; }
            public bool? JeppesenAirwayManualDSP { get; set; }
            public bool? JeppesenAirwayManualCPT { get; set; }
            public string JeppesenAirwayManualDSPRemark { get; set; }
            public string JeppesenAirwayManualCPTRemark { get; set; }
            public bool? MinFuelRequiredDSP { get; set; }
            public bool? MinFuelRequiredCPT { get; set; }
            public decimal? MinFuelRequiredCFP { get; set; }
            public decimal? MinFuelRequiredSFP { get; set; }
            public decimal? MinFuelRequiredPilotReq { get; set; }
            public bool? GeneralDeclarationDSP { get; set; }
            public bool? GeneralDeclarationCPT { get; set; }
            public string GeneralDeclarationDSPRemark { get; set; }
            public string GeneralDeclarationCPTRemark { get; set; }
            public bool? FlightReportDSP { get; set; }
            public bool? FlightReportCPT { get; set; }
            public string FlightReportDSPRemark { get; set; }
            public string FlightReportCPTRemark { get; set; }
            public bool? TOLndCardsDSP { get; set; }
            public bool? TOLndCardsCPT { get; set; }
            public string TOLndCardsDSPRemark { get; set; }
            public string TOLndCardsCPTRemark { get; set; }
            public bool? LoadSheetDSP { get; set; }
            public bool? LoadSheetCPT { get; set; }
            public string LoadSheetDSPRemark { get; set; }
            public string LoadSheetCPTRemark { get; set; }
            public bool? FlightSafetyReportDSP { get; set; }
            public bool? FlightSafetyReportCPT { get; set; }
            public string FlightSafetyReportDSPRemark { get; set; }
            public string FlightSafetyReportCPTRemark { get; set; }
            public bool? AVSECIncidentReportDSP { get; set; }
            public bool? AVSECIncidentReportCPT { get; set; }
            public string AVSECIncidentReportDSPRemark { get; set; }
            public string AVSECIncidentReportCPTRemark { get; set; }
            public bool? OperationEngineeringDSP { get; set; }
            public bool? OperationEngineeringCPT { get; set; }
            public string OperationEngineeringDSPRemark { get; set; }
            public string OperationEngineeringCPTRemark { get; set; }
            public bool? VoyageReportDSP { get; set; }
            public bool? VoyageReportCPT { get; set; }
            public string VoyageReportDSPRemark { get; set; }
            public string VoyageReportCPTRemark { get; set; }
            public bool? PIFDSP { get; set; }
            public bool? PIFCPT { get; set; }
            public string PIFDSPRemark { get; set; }
            public string PIFCPTRemark { get; set; }
            public bool? GoodDeclarationDSP { get; set; }
            public bool? GoodDeclarationCPT { get; set; }
            public string GoodDeclarationDSPRemark { get; set; }
            public string GoodDeclarationCPTRemark { get; set; }
            public bool? IPADDSP { get; set; }
            public bool? IPADCPT { get; set; }
            public string IPADDSPRemark { get; set; }
            public string IPADCPTRemark { get; set; }
            public DateTime? DateConfirmed { get; set; }
            public bool? ATSFlightPlanFOO { get; set; }
            public bool? ATSFlightPlanCMDR { get; set; }
            public string ATSFlightPlanFOORemark { get; set; }
            public string ATSFlightPlanCMDRRemark { get; set; }
            public bool? VldEFBFOO { get; set; }
            public bool? VldEFBCMDR { get; set; }
            public string VldEFBFOORemark { get; set; }
            public string VldEFBCMDRRemark { get; set; }
            public bool? VldFlightCrewFOO { get; set; }
            public bool? VldFlightCrewCMDR { get; set; }
            public string VldFlightCrewFOORemark { get; set; }
            public string VldFlightCrewCMDRRemark { get; set; }
            public bool? VldMedicalFOO { get; set; }
            public bool? VldMedicalCMDR { get; set; }
            public string VldMedicalFOORemark { get; set; }
            public string VldMedicalCMDRRemark { get; set; }
            public bool? VldPassportFOO { get; set; }
            public bool? VldPassportCMDR { get; set; }
            public string VldPassportFOORemark { get; set; }
            public string VldPassportCMDRRemark { get; set; }
            public bool? VldCMCFOO { get; set; }
            public bool? VldCMCCMDR { get; set; }
            public string VldCMCFOORemark { get; set; }
            public string VldCMCCMDRRemark { get; set; }
            public bool? VldRampPassFOO { get; set; }
            public bool? VldRampPassCMDR { get; set; }
            public string VldRampPassFOORemark { get; set; }
            public string VldRampPassCMDRRemark { get; set; }
            public bool? OperationalFlightPlanFOO { get; set; }
            public bool? OperationalFlightPlanCMDR { get; set; }
            public string OperationalFlightPlanFOORemark { get; set; }
            public string OperationalFlightPlanCMDRRemark { get; set; }

            public int? DispatcherId { get; set; }
            public int Id { get; set; }
            public string User { get; set; }*/
        public class DSPReleaseViewModel
        {
          
            public Nullable<int> FlightId { get; set; }
            public Nullable<bool> ActualWXDSP { get; set; }
            public Nullable<bool> ActualWXCPT { get; set; }
            public string ActualWXDSPRemark { get; set; }
            public string ActualWXCPTRemark { get; set; }
            public Nullable<bool> WXForcastDSP { get; set; }
            public Nullable<bool> WXForcastCPT { get; set; }
            public string WXForcastDSPRemark { get; set; }
            public string WXForcastCPTRemark { get; set; }
            public Nullable<bool> SigxWXDSP { get; set; }
            public Nullable<bool> SigxWXCPT { get; set; }
            public string SigxWXDSPRemark { get; set; }
            public string SigxWXCPTRemark { get; set; }
            public Nullable<bool> WindChartDSP { get; set; }
            public Nullable<bool> WindChartCPT { get; set; }
            public string WindChartDSPRemark { get; set; }
            public string WindChartCPTRemark { get; set; }
            public Nullable<bool> NotamDSP { get; set; }
            public Nullable<bool> NotamCPT { get; set; }
            public string NotamDSPRemark { get; set; }
            public string NotamCPTRemark { get; set; }
            public Nullable<bool> ComputedFligthPlanDSP { get; set; }
            public Nullable<bool> ComputedFligthPlanCPT { get; set; }
            public string ComputedFligthPlanDSPRemark { get; set; }
            public string ComputedFligthPlanCPTRemark { get; set; }
            public Nullable<bool> ATCFlightPlanDSP { get; set; }
            public Nullable<bool> ATCFlightPlanCPT { get; set; }
            public string ATCFlightPlanDSPRemark { get; set; }
            public string ATCFlightPlanCPTRemark { get; set; }
            public Nullable<bool> PermissionsDSP { get; set; }
            public Nullable<bool> PermissionsCPT { get; set; }
            public string PermissionsDSPRemark { get; set; }
            public string PermissionsCPTRemark { get; set; }
            public Nullable<bool> JeppesenAirwayManualDSP { get; set; }
            public Nullable<bool> JeppesenAirwayManualCPT { get; set; }
            public string JeppesenAirwayManualDSPRemark { get; set; }
            public string JeppesenAirwayManualCPTRemark { get; set; }
            public Nullable<bool> MinFuelRequiredDSP { get; set; }
            public Nullable<bool> MinFuelRequiredCPT { get; set; }
            public Nullable<decimal> MinFuelRequiredCFP { get; set; }
            public Nullable<decimal> MinFuelRequiredPilotReq { get; set; }
            public Nullable<bool> GeneralDeclarationDSP { get; set; }
            public Nullable<bool> GeneralDeclarationCPT { get; set; }
            public string GeneralDeclarationDSPRemark { get; set; }
            public string GeneralDeclarationCPTRemark { get; set; }
            public Nullable<bool> FlightReportDSP { get; set; }
            public Nullable<bool> FlightReportCPT { get; set; }
            public string FlightReportDSPRemark { get; set; }
            public string FlightReportCPTRemark { get; set; }
            public Nullable<bool> TOLndCardsDSP { get; set; }
            public Nullable<bool> TOLndCardsCPT { get; set; }
            public string TOLndCardsDSPRemark { get; set; }
            public string TOLndCardsCPTRemark { get; set; }
            public Nullable<bool> LoadSheetDSP { get; set; }
            public Nullable<bool> LoadSheetCPT { get; set; }
            public string LoadSheetDSPRemark { get; set; }
            public string LoadSheetCPTRemark { get; set; }
            public Nullable<bool> FlightSafetyReportDSP { get; set; }
            public Nullable<bool> FlightSafetyReportCPT { get; set; }
            public string FlightSafetyReportDSPRemark { get; set; }
            public string FlightSafetyReportCPTRemark { get; set; }
            public Nullable<bool> AVSECIncidentReportDSP { get; set; }
            public Nullable<bool> AVSECIncidentReportCPT { get; set; }
            public string AVSECIncidentReportDSPRemark { get; set; }
            public string AVSECIncidentReportCPTRemark { get; set; }
            public Nullable<bool> OperationEngineeringDSP { get; set; }
            public Nullable<bool> OperationEngineeringCPT { get; set; }
            public string OperationEngineeringDSPRemark { get; set; }
            public string OperationEngineeringCPTRemark { get; set; }
            public Nullable<bool> VoyageReportDSP { get; set; }
            public Nullable<bool> VoyageReportCPT { get; set; }
            public string VoyageReportDSPRemark { get; set; }
            public string VoyageReportCPTRemark { get; set; }
            public Nullable<bool> PIFDSP { get; set; }
            public Nullable<bool> PIFCPT { get; set; }
            public string PIFDSPRemark { get; set; }
            public string PIFCPTRemark { get; set; }
            public Nullable<bool> GoodDeclarationDSP { get; set; }
            public Nullable<bool> GoodDeclarationCPT { get; set; }
            public string GoodDeclarationDSPRemark { get; set; }
            public string GoodDeclarationCPTRemark { get; set; }
            public Nullable<bool> IPADDSP { get; set; }
            public Nullable<bool> IPADCPT { get; set; }
            public string IPADDSPRemark { get; set; }
            public string IPADCPTRemark { get; set; }
            public Nullable<System.DateTime> DateConfirmed { get; set; }
            public Nullable<int> DispatcherId { get; set; }
            public int Id { get; set; }
            public string DateUpdate { get; set; }
            public string User { get; set; }
            public string JLSignedBy { get; set; }
            public Nullable<System.DateTime> JLDatePICApproved { get; set; }
            public Nullable<int> PICId { get; set; }
            public string PIC { get; set; }
            public Nullable<bool> OperationalFlightPlanFOO { get; set; }
            public Nullable<bool> OperationalFlightPlanCMDR { get; set; }
            public string OperationalFlightPlanFOORemark { get; set; }
            public string OperationalFlightPlanCMDRRemark { get; set; }
            public Nullable<bool> ATSFlightPlanFOO { get; set; }
            public Nullable<bool> ATSFlightPlanCMDR { get; set; }
            public string ATSFlightPlanFOORemark { get; set; }
            public string ATSFlightPlanCMDRRemark { get; set; }
            public Nullable<bool> VldEFBFOO { get; set; }
            public Nullable<bool> VldEFBCMDR { get; set; }
            public string VldEFBFOORemark { get; set; }
            public string VldEFBCMDRRemark { get; set; }
            public Nullable<bool> VldFlightCrewFOO { get; set; }
            public Nullable<bool> VldFlightCrewCMDR { get; set; }
            public string VldFlightCrewFOORemark { get; set; }
            public string VldFlightCrewCMDRRemark { get; set; }
            public Nullable<bool> VldMedicalFOO { get; set; }
            public Nullable<bool> VldMedicalCMDR { get; set; }
            public string VldMedicalFOORemark { get; set; }
            public string VldMedicalCMDRRemark { get; set; }
            public Nullable<bool> VldPassportFOO { get; set; }
            public Nullable<bool> VldPassportCMDR { get; set; }
            public string VldPassportFOORemark { get; set; }
            public string VldPassportCMDRRemark { get; set; }
            public Nullable<bool> VldCMCFOO { get; set; }
            public Nullable<bool> VldCMCCMDR { get; set; }
            public string VldCMCFOORemark { get; set; }
            public string VldCMCCMDRRemark { get; set; }
            public Nullable<bool> VldRampPassFOO { get; set; }
            public Nullable<bool> VldRampPassCMDR { get; set; }
            public string VldRampPassFOORemark { get; set; }
            public string VldRampPassCMDRRemark { get; set; }
            public string Note { get; set; }
            public string SgnDSPLicNo { get; set; }
            public string SgnCPTLicNo { get; set; }
            public Nullable<System.DateTime> JLDSPSignDate { get; set; }
            public string SGNDSPName { get; set; }
        }

        public class DSPReleaseViewModel2
        {
            public int? FlightId { get; set; }
            public bool? ActualWXDSP { get; set; }
            // public bool? ActualWXCPT { get; set; }
            public string ActualWXDSPRemark { get; set; }
            //public string ActualWXCPTRemark { get; set; }
            public bool? WXForcastDSP { get; set; }
            //public bool? WXForcastCPT { get; set; }
            public string WXForcastDSPRemark { get; set; }
            //public string WXForcastCPTRemark { get; set; }
            public bool? SigxWXDSP { get; set; }
            //public bool? SigxWXCPT { get; set; }
            public string SigxWXDSPRemark { get; set; }
            //public string SigxWXCPTRemark { get; set; }
            public bool? WindChartDSP { get; set; }
            //public bool? WindChartCPT { get; set; }
            public string WindChartDSPRemark { get; set; }
            //public string WindChartCPTRemark { get; set; }
            public bool? NotamDSP { get; set; }
            //public bool? NotamCPT { get; set; }
            public string NotamDSPRemark { get; set; }
            //public string NotamCPTRemark { get; set; }
            public bool? ComputedFligthPlanDSP { get; set; }
            //public bool? ComputedFligthPlanCPT { get; set; }
            public string ComputedFligthPlanDSPRemark { get; set; }
            //public string ComputedFligthPlanCPTRemark { get; set; }
            public bool? ATCFlightPlanDSP { get; set; }
            //public bool? ATCFlightPlanCPT { get; set; }
            public string ATCFlightPlanDSPRemark { get; set; }
            //public string ATCFlightPlanCPTRemark { get; set; }
            public bool? PermissionsDSP { get; set; }
            //public bool? PermissionsCPT { get; set; }
            public string PermissionsDSPRemark { get; set; }
            //public string PermissionsCPTRemark { get; set; }
            public bool? JeppesenAirwayManualDSP { get; set; }
            //public bool? JeppesenAirwayManualCPT { get; set; }
            public string JeppesenAirwayManualDSPRemark { get; set; }
            //public string JeppesenAirwayManualCPTRemark { get; set; }
            public bool? MinFuelRequiredDSP { get; set; }
            //public bool? MinFuelRequiredCPT { get; set; }
            //public decimal? MinFuelRequiredCFP { get; set; }
            //public decimal? MinFuelRequiredSFP { get; set; }
            //public decimal? MinFuelRequiredPilotReq { get; set; }
            public bool? GeneralDeclarationDSP { get; set; }
            //public bool? GeneralDeclarationCPT { get; set; }
            public string GeneralDeclarationDSPRemark { get; set; }
            //public string GeneralDeclarationCPTRemark { get; set; }
            public bool? FlightReportDSP { get; set; }
            //public bool? FlightReportCPT { get; set; }
            public string FlightReportDSPRemark { get; set; }
            //public string FlightReportCPTRemark { get; set; }
            public bool? TOLndCardsDSP { get; set; }
            //public bool? TOLndCardsCPT { get; set; }
            public string TOLndCardsDSPRemark { get; set; }
            //public string TOLndCardsCPTRemark { get; set; }
            public bool? LoadSheetDSP { get; set; }
            //public bool? LoadSheetCPT { get; set; }
            public string LoadSheetDSPRemark { get; set; }
            //public string LoadSheetCPTRemark { get; set; }
            public bool? FlightSafetyReportDSP { get; set; }
            //public bool? FlightSafetyReportCPT { get; set; }
            public string FlightSafetyReportDSPRemark { get; set; }
            //public string FlightSafetyReportCPTRemark { get; set; }
            public bool? AVSECIncidentReportDSP { get; set; }
            //public bool? AVSECIncidentReportCPT { get; set; }
            public string AVSECIncidentReportDSPRemark { get; set; }
            //public string AVSECIncidentReportCPTRemark { get; set; }
            public bool? OperationEngineeringDSP { get; set; }
            //public bool? OperationEngineeringCPT { get; set; }
            public string OperationEngineeringDSPRemark { get; set; }
            //public string OperationEngineeringCPTRemark { get; set; }
            public bool? VoyageReportDSP { get; set; }
            //public bool? VoyageReportCPT { get; set; }
            public string VoyageReportDSPRemark { get; set; }
            //public string VoyageReportCPTRemark { get; set; }
            public bool? PIFDSP { get; set; }
            //public bool? PIFCPT { get; set; }
            public string PIFDSPRemark { get; set; }
            //public string PIFCPTRemark { get; set; }
            public bool? GoodDeclarationDSP { get; set; }
            //public bool? GoodDeclarationCPT { get; set; }
            public string GoodDeclarationDSPRemark { get; set; }
            //public string GoodDeclarationCPTRemark { get; set; }
            public bool? IPADDSP { get; set; }
            //public bool? IPADCPT { get; set; }
            public string IPADDSPRemark { get; set; }
            //public string IPADCPTRemark { get; set; }
            //public DateTime? DateConfirmed { get; set; }
            public int? DispatcherId { get; set; }
            public int Id { get; set; }
            public string User { get; set; }
        }
    }
}
