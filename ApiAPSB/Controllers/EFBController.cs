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
    public class EFBController : ApiController
    {

        public class EFBASRViewModel
        {
            // Existing properties...
            public int FlightId { get; set; }
            public int? EventTypeId { get; set; }
            public string OccurrenceDate { get; set; }
            public bool? IsDay { get; set; }
            public string SQUAWK { get; set; }
            public decimal? FuelJettisoned { get; set; }
            public decimal? Altitude { get; set; }
            public double? Speed { get; set; }
            public double? MachNo { get; set; }
            public decimal? ACWeight { get; set; }
            public string TechLogPageNO { get; set; }
            public string TechLogItemNO { get; set; }
            public int? FlightPhaseId { get; set; }
            public string LOCAirport { get; set; }
            public string LOCStand { get; set; }
            public string LOCRunway { get; set; }
            public string LOCGEOLongtitude { get; set; }
            public string LOCGEOAltitude { get; set; }
            public string LOCTaxiway { get; set; }
            public string LOCEnRoute { get; set; }
            public string LOCAPP { get; set; }
            public int? METId { get; set; }
            public string ActualWX { get; set; }
            public int? SigxWXId { get; set; }
            public int? RunwayConditionId { get; set; }
            public string ACConfigAP { get; set; }
            public string ACConfigATHR { get; set; }
            public string ACConfigGear { get; set; }
            public string ACConfigFlap { get; set; }
            public string ACConfigSlat { get; set; }
            public string ACConfigSpoilers { get; set; }
            public string ACConfigAPU { get; set; }
            public string ACConfigENG { get; set; }
            public string Summary { get; set; }
            public string Result { get; set; }
            public string OthersInfo { get; set; }
            public int? AATRiskId { get; set; }
            public bool? AATIsActionTaken { get; set; }
            public string AATReportedToATC { get; set; }
            public string AATATCInstruction { get; set; }
            public string AATFrequency { get; set; }
            public decimal? AATHeading { get; set; }
            public string AATClearedAltitude { get; set; }
            public string AATMinVerticalSep { get; set; }
            public string AATMinHorizontalSep { get; set; }
            public int? AATTCASAlertId { get; set; }
            public string AATTypeRA { get; set; }
            public bool? AATIsRAFollowed { get; set; }
            public string AATVerticalDeviation { get; set; }
            public string AATOtherACType { get; set; }
            public string AATMarkingColour { get; set; }
            public string AATCallSign { get; set; }
            public string AATLighting { get; set; }
            public string ATTRelativePos { get; set; }
            public int? AirspaceId { get; set; }
            public int? AtcId { get; set; }
            public string MetAirportATIS { get; set; }
            public decimal? WTHeading { get; set; }
            public int? WTTurningId { get; set; }
            public int? WTGlideSlopePosId { get; set; }
            public int? WTExtendedCenterlinePosId { get; set; }
            public int? WTAttitudeChangeId { get; set; }
            public decimal? WTAttitudeChangeDeg { get; set; }
            public bool? WTIsBuffet { get; set; }
            public bool? WTIsStickShaker { get; set; }
            public string WTSuspect { get; set; }
            public string WTDescribeVA { get; set; }
            public string WTPrecedingAC { get; set; }
            public bool? WTIsAware { get; set; }
            public int? WTIsAP { get; set; }
            public int? WTIsSpeed { get; set; }
            public int? WTIsBuffetExp { get; set; }
            public int? WTIsStall { get; set; }
            public int? WTIsAltitude { get; set; }
            public int? WTIsAttitude { get; set; }
            public bool? WTStall { get; set; }
            public bool? WTAPDisconnected { get; set; }
            public bool? WTSpeed { get; set; }
            public string BSBirdType { get; set; }
            public int? BSNrSeenId { get; set; }
            public int? BSNrStruckId { get; set; }
            public int? BSTimeId { get; set; }
            public DateTime? PICDate { get; set; }
            public int? DayNightStatusId { get; set; }
            public int? IncidentTypeId { get; set; }
            public string BSImpactDec { get; set; }
            public bool? IsSecurityEvent { get; set; }
            public bool? IsAirproxATC { get; set; }
            public bool? IsTCASRA { get; set; }
            public bool? IsWakeTur { get; set; }
            public bool? IsBirdStrike { get; set; }
            public bool? IsOthers { get; set; }
            public int? SigxWXTypeId { get; set; }
            public double? BSHeading { get; set; }
            public int? BSTurningId { get; set; }
            public string User { get; set; }
            public int? OPSStatusId { get; set; }
            public int? OPSStaffStatusId { get; set; }
            public int Id { get; set; }
            public string JLSignedBy { get; set; }
            public DateTime? JLDatePICApproved { get; set; }
            public int? PICId { get; set; }
            public string PIC { get; set; }
            public string OPSRemark { get; set; }
            public DateTime? OPSRemarkDate { get; set; }
            public string OPSUser { get; set; }
            public int? OPSId { get; set; }
            public string OPSStaffRemark { get; set; }
            public DateTime? OPSStaffDateVisit { get; set; }
            public DateTime? OPSStaffConfirmDate { get; set; }
            public int? OPSStaffId { get; set; }
            public DateTime? OPSStaffRemarkDate { get; set; }
            public int? CFITAirapaceId { get; set; }
            public int? CFITATCId { get; set; }
            public string CFITFrequency { get; set; }
            public string CFITHeading { get; set; }
            public string CFITALT { get; set; }
            public string CFITGPWS { get; set; }
            public int? MetSurfaceId { get; set; }
            public int? MetSurfaceConditionId { get; set; }
            public int? MetFlightRuleId { get; set; }

            // Missing properties to add
            public string DateUpdate { get; set; }
            public int? AATXAbove { get; set; }
            public int? AATYAbove { get; set; }
            public int? AATXAstern { get; set; }
            public int? AATYAstern { get; set; }
            public int? AATHorizontalPlane { get; set; }
            public DateTime? OPSConfirmDate { get; set; }
            public string OPSStaffUser { get; set; }
            public int? EGPWSTypeId { get; set; }
            public string EGPWSAuralAlert { get; set; }
            public string EGPWSActionTaken { get; set; }
            public int? CFITIncidentId { get; set; }
            public int? AATIncidentId { get; set; }
            public string AATRelativeALT { get; set; }
            public int? BSSkyCondition { get; set; }
            public int? BSRandom { get; set; }
            public int? BSWindShield { get; set; }
            public int? BSENG { get; set; }
            public int? BSWing { get; set; }
            public int? BSFuselage { get; set; }
            public int? BSLDG { get; set; }
            public int? BSTail { get; set; }
            public int? BSLights { get; set; }
            public int? BSOther { get; set; }
            public int? BSEffectFlt { get; set; }
        }

        [HttpPost]
        [Route("api/asr/save")]

        public async Task<DataResponse> SaveEFBASR(EFBASRViewModel EFBASR)
        {

            var _context = new dbEntities();
            //var dt = new DateTime(600 + 400 + 1000 + 2 + 2 + 17 + 3, 1, 1);
            //dt = dt.AddMonths(6 + 5);
            //if(DateTime.Now > dt)
            //{
            //    return new DataResponse() { IsSuccess = true, Data = null };
            //}



            try
            {
                var entity = await _context.EFBASRs.FirstOrDefaultAsync(q => q.FlightId == EFBASR.FlightId);

                if (entity == null)
                {
                    entity = new EFBASR();
                    _context.EFBASRs.Add(entity);
                }

                entity.User = EFBASR.User;
                entity.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm");

                entity.FlightId = EFBASR.FlightId;
                entity.EventTypeId = EFBASR.EventTypeId;
                entity.OccurrenceDate = DateTime.Now; //DateTime.Parse(EFBASR.OccurrenceDate);//Helper.ConvertToDate(EFBASR.OccurrenceDate);// DateTime.Parse(EFBASR.OccurrenceDate);
                entity.IsDay = EFBASR.IsDay;
                entity.SQUAWK = EFBASR.SQUAWK;
                entity.FuelJettisoned = EFBASR.FuelJettisoned;
                entity.Altitude = EFBASR.Altitude;
                //entity.SpeedMACHNO = EFBASR.SpeedMACHNO;
                entity.MachNo = EFBASR.MachNo;
                entity.Speed = EFBASR.Speed;
                entity.ACWeight = EFBASR.ACWeight;
                entity.TechLogPageNO = EFBASR.TechLogPageNO;
                entity.TechLogItemNO = EFBASR.TechLogItemNO;
                entity.FlightPhaseId = EFBASR.FlightPhaseId;
                entity.LOCAirport = EFBASR.LOCAirport;
                entity.LOCStand = EFBASR.LOCStand;
                entity.LOCRunway = EFBASR.LOCRunway;
                //entity.LOCGEOLongitude = EFBASR.LOCGEOLongitude;
                entity.LOCGEOAltitude = EFBASR.LOCGEOAltitude;
                entity.LOCGEOLongtitude = EFBASR.LOCGEOLongtitude;
                entity.METId = EFBASR.METId;
                entity.ActualWX = EFBASR.ActualWX;
                entity.SigxWXId = EFBASR.SigxWXId;
                entity.RunwayConditionId = EFBASR.RunwayConditionId;
                entity.ACConfigAP = EFBASR.ACConfigAP;
                entity.ACConfigATHR = EFBASR.ACConfigATHR;
                entity.ACConfigGear = EFBASR.ACConfigGear;
                entity.ACConfigFlap = EFBASR.ACConfigFlap;
                entity.ACConfigSlat = EFBASR.ACConfigSlat;
                entity.ACConfigSpoilers = EFBASR.ACConfigSpoilers;
                entity.Summary = EFBASR.Summary;
                entity.Result = EFBASR.Result;
                entity.OthersInfo = EFBASR.OthersInfo;
                entity.AATRiskId = EFBASR.AATRiskId;
                entity.AATIsActionTaken = EFBASR.AATIsActionTaken;
                entity.AATReportedToATC = EFBASR.AATReportedToATC;
                entity.AATATCInstruction = EFBASR.AATATCInstruction;
                entity.AATFrequency = EFBASR.AATFrequency;
                entity.AATHeading = EFBASR.AATHeading;
                entity.AATClearedAltitude = EFBASR.AATClearedAltitude;
                entity.AATMinVerticalSep = EFBASR.AATMinVerticalSep;
                entity.AATMinHorizontalSep = EFBASR.AATMinHorizontalSep;
                entity.AATTCASAlertId = EFBASR.AATTCASAlertId;
                entity.AATTypeRA = EFBASR.AATTypeRA;
                entity.AATIsRAFollowed = EFBASR.AATIsRAFollowed;
                entity.AATVerticalDeviation = EFBASR.AATVerticalDeviation;
                entity.AATOtherACType = EFBASR.AATOtherACType;
                entity.AATMarkingColour = EFBASR.AATMarkingColour;
                entity.AATCallSign = EFBASR.AATCallSign;
                entity.AATLighting = EFBASR.AATLighting;
                entity.WTHeading = EFBASR.WTHeading;
                entity.WTTurningId = EFBASR.WTTurningId;
                entity.WTGlideSlopePosId = EFBASR.WTGlideSlopePosId;
                entity.WTExtendedCenterlinePosId = EFBASR.WTExtendedCenterlinePosId;
                entity.WTAttitudeChangeId = EFBASR.WTAttitudeChangeId;
                entity.WTAttitudeChangeDeg = EFBASR.WTAttitudeChangeDeg;
                entity.WTIsBuffet = EFBASR.WTIsBuffet;
                entity.WTIsStickShaker = EFBASR.WTIsStickShaker;
                entity.WTSuspect = EFBASR.WTSuspect;
                entity.WTDescribeVA = EFBASR.WTDescribeVA;
                entity.WTPrecedingAC = EFBASR.WTPrecedingAC;
                entity.WTIsAware = EFBASR.WTIsAware;
                entity.BSBirdType = EFBASR.BSBirdType;
                entity.BSNrSeenId = EFBASR.BSNrSeenId;
                entity.BSNrStruckId = EFBASR.BSNrStruckId;
                entity.BSTimeId = EFBASR.BSTimeId;
                entity.PICDate = EFBASR.PICDate;

                entity.DayNightStatusId = EFBASR.DayNightStatusId;
                entity.IncidentTypeId = EFBASR.IncidentTypeId;
                entity.AATXAbove = EFBASR.AATXAbove;
                entity.AATYAbove = EFBASR.AATYAbove;
                entity.AATXAstern = EFBASR.AATXAstern;
                entity.AATYAstern = EFBASR.AATYAstern;
                entity.AATHorizontalPlane = EFBASR.AATHorizontalPlane;
                entity.BSImpactDec = EFBASR.BSImpactDec;
                entity.BSHeading = EFBASR.BSHeading;
                entity.IsSecurityEvent = EFBASR.IsSecurityEvent;
                entity.IsAirproxATC = EFBASR.IsAirproxATC;
                entity.IsTCASRA = EFBASR.IsTCASRA;
                entity.IsWakeTur = EFBASR.IsWakeTur;
                entity.IsBirdStrike = EFBASR.IsBirdStrike;
                entity.IsOthers = EFBASR.IsOthers;
                entity.SigxWXTypeId = EFBASR.SigxWXTypeId;
                entity.BSTurningId = EFBASR.BSTurningId;

                entity.OPSStatusId = EFBASR.OPSStatusId;
                entity.OPSStaffStatusId = EFBASR.OPSStaffStatusId;

                // Add missing fields
                entity.JLSignedBy = EFBASR.JLSignedBy;
                entity.JLDatePICApproved = EFBASR.JLDatePICApproved;
                entity.PICId = EFBASR.PICId;
                entity.PIC = EFBASR.PIC;
                entity.OPSRemark = EFBASR.OPSRemark;
                entity.OPSRemarkDate = EFBASR.OPSRemarkDate;
                entity.OPSId = EFBASR.OPSId;
                entity.OPSConfirmDate = EFBASR.OPSConfirmDate;
                entity.OPSStaffRemark = EFBASR.OPSStaffRemark;
                entity.OPSStaffDateVisit = EFBASR.OPSStaffDateVisit;
                entity.OPSStaffConfirmDate = EFBASR.OPSStaffConfirmDate;
                entity.OPSStaffId = EFBASR.OPSStaffId;
                entity.OPSStaffRemarkDate = EFBASR.OPSStaffRemarkDate;
                entity.OPSUser = EFBASR.OPSUser;
                entity.OPSStaffUser = EFBASR.OPSStaffUser;
                entity.LOCTaxiway = EFBASR.LOCTaxiway;
                entity.LOCEnRoute = EFBASR.LOCEnRoute;
                entity.LOCAPP = EFBASR.LOCAPP;
                entity.ACConfigAPU = EFBASR.ACConfigAPU;
                entity.ACConfigENG = EFBASR.ACConfigENG;
                entity.EGPWSTypeId = EFBASR.EGPWSTypeId;
                entity.EGPWSAuralAlert = EFBASR.EGPWSAuralAlert;
                entity.EGPWSActionTaken = EFBASR.EGPWSActionTaken;
                entity.CFITAirapaceId = EFBASR.CFITAirapaceId;
                entity.CFITATCId = EFBASR.CFITATCId;
                entity.CFITFrequency = EFBASR.CFITFrequency;
                entity.CFITHeading = EFBASR.CFITHeading;
                entity.CFITALT = EFBASR.CFITALT;
                entity.CFITIncidentId = EFBASR.CFITIncidentId;
                entity.CFITGPWS = EFBASR.CFITGPWS;
                entity.AATIncidentId = EFBASR.AATIncidentId;
                entity.AATRelativeALT = EFBASR.AATRelativeALT;
                entity.ATTRelativePos = EFBASR.ATTRelativePos;
                entity.BSSkyCondition = EFBASR.BSSkyCondition;
                entity.BSRandom = EFBASR.BSRandom;
                entity.BSWindShield = EFBASR.BSWindShield;
                entity.BSENG = EFBASR.BSENG;
                entity.BSWing = EFBASR.BSWing;
                entity.BSFuselage = EFBASR.BSFuselage;
                entity.BSLDG = EFBASR.BSLDG;
                entity.BSTail = EFBASR.BSTail;
                entity.BSLights = EFBASR.BSLights;
                entity.BSOther = EFBASR.BSOther;
                entity.BSEffectFlt = EFBASR.BSEffectFlt;
                entity.WTAPDisconnected = EFBASR.WTAPDisconnected;
                entity.WTSpeed = EFBASR.WTSpeed;
                entity.WTStall = EFBASR.WTStall;
                entity.MetSurfaceId = EFBASR.MetSurfaceId;
                entity.MetSurfaceConditionId = EFBASR.MetSurfaceConditionId;
                entity.MetFlightRuleId = EFBASR.MetFlightRuleId;
                entity.WTIsStall = EFBASR.WTIsStall;
                entity.WTIsBuffetExp = EFBASR.WTIsBuffetExp;
                entity.WTIsSpeed = EFBASR.WTIsSpeed;
                entity.WTIsAP = EFBASR.WTIsAP;
                entity.WTIsAltitude = EFBASR.WTIsAltitude;
                entity.WTIsAttitude = EFBASR.WTIsAttitude;
                entity.AirspaceId = EFBASR.AirspaceId;
                entity.AtcId = EFBASR.AtcId;
                entity.MetAirportATIS = EFBASR.MetAirportATIS;

                var saveResult = await _context.SaveChangesAsync();
                ViewEFBASR view_efb = await _context.ViewEFBASRs.FirstOrDefaultAsync(q => q.Id == entity.Id);
                return new DataResponse() { IsSuccess = true, Data = view_efb };
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
        [Route("api/asr/flights")]
        public async Task<DataResponse> GetEFBASRsByFlightIds(List<int> ids)
        {
            var _context = new dbEntities();
            var entity = await _context.EFBASRs.Where(q => ids.Contains(q.FlightId)).ToListAsync();
            return new DataResponse()
            {
                Data = entity,
                IsSuccess = true

            };
        }

        [Route("api/asr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetASRByFlight(int fltid)
        {

            // GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Re‌​ferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var context = new dbEntities();
            var result = context.ViewEFBASRs.FirstOrDefault(q => q.FlightId == fltid);
            return Ok(new { IsSuccess = true, Data = result, Errors = "null", Messages = "null" });
        }

        [Route("api/asr/flight/view/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetASRByFlightView(int fltid)
        {


            var context = new dbEntities();
            var result = context.ViewEFBASRs.FirstOrDefault(q => q.FlightId == fltid);
            return Ok(result);
        }

        //        [HttpGet]
        //        [Route("api/voyage/flight/{flightId}")]
        //        public IHttpActionResult GetVoyageReportByFlightId(int flightId)
        //        {
        //            var context = new dbEntities();
        //            var result = context.EFBVoyageReports.FirstOrDefault(q => q.FlightId == flightId);
        //            //result.EFBFlightIrregularities = context.EFBFlightIrregularities.Where(q => q.VoyageReportId == result.Id).ToList();
        //            // result.EFBReasons = context.EFBReasons.Where(q => q.VoyageReportId == result.Id).ToList();
        //            var _vr = new
        //            {
        //                result.Id,
        //                result.FlightId,
        //                result.Route,
        //                result.RestReduction,
        //                result.DutyExtention,
        //                result.Report,
        //                result.DatePICSignature,
        //                result.ActionedById,
        //                result.DateActioned,
        //                result.DateConfirmed,
        //                result.DepDelay,
        //                result.DateUpdate,
        //                result.User,
        //                result.JLSignedBy,
        //                result.JLDatePICApproved,
        //                result.PICId,
        //                result.PIC,
        //                result.OPSRemark,
        //                result.OPSRemarkDate,
        //                result.OPSId,
        //                result.OPSConfirmDate,
        //                result.OPSStaffRemark,
        //                result.OPSStaffDateVisit,
        //                result.OPSStaffConfirmDate,
        //                result.OPSStaffId,
        //                result.OPSStaffRemarkDate,
        //                result.OPSUser,
        //                result.OPSStaffUser,
        //                result.OPSStatusId,
        //                result.OPSStaffStatusId,
        //                EFBFlightIrregularities = context.EFBFlightIrregularities.Where(q => q.VoyageReportId == result.Id).Select(q => new { q.Id, q.IrrId }).ToList(),
        //                EFBReasons = context.EFBReasons.Where(q => q.VoyageReportId == result.Id).Select(q => new { q.Id, q.ReasonId }).ToList()

        //            };
        //            object output = new
        //            {
        //                Data = _vr,
        //                IsSuccess = true
        //            };
        //            var str = JsonConvert.SerializeObject(output, Formatting.Indented,
        //new JsonSerializerSettings
        //{
        //    PreserveReferencesHandling = PreserveReferencesHandling.Objects
        //});

        //            return Ok(str);
        //        }

        [Route("api/vr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVRByFlight(int fltid)
        {


            var context = new dbEntities();

            var result = context.EFBVoyageReports.FirstOrDefault(q => q.FlightId == fltid);
            if (result == null)
                return Ok(result);
            result.EFBReasons = context.EFBReasons.Where(q => q.VoyageReportId == result.Id).ToList();
            result.EFBFlightIrregularities = context.EFBFlightIrregularities.Where(q => q.VoyageReportId == result.Id).ToList();
            return Ok(result);
        }

        [Route("api/vr/remark/manager/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostVrRemarkManager(dynamic dto)
        {
            string user = Convert.ToString(dto.user);
            string remark = Convert.ToString(dto.remark);
            int status = Convert.ToInt32(dto.status);
            int id = Convert.ToInt32(dto.id);

            var context = new dbEntities();

            var vr = context.EFBVoyageReports.FirstOrDefault(q => q.Id == id);
            if (vr.OPSStatusId == null)
                vr.OPSStatusId = 0;
            if (vr.OPSRemark == null)
                vr.OPSRemark = "";
            if (vr != null && (vr.OPSStaffStatusId == null || vr.OPSStaffStatusId == 0))
            {
                //if (string.IsNullOrEmpty(remark.Trim().Replace(" ","")))
                //{
                //    vr.OPSRemark = "";
                //    vr.OPSRemarkDate = null;
                //    vr.OPSUser = "";
                //    vr.OPSStatusId = 0;

                //    vr.OPSStaffConfirmDate = null;
                //    vr.OPSStaffDateVisit = null;
                //    vr.OPSStaffRemark = null;
                //    vr.OPSStaffRemarkDate = null;
                //    vr.OPSStaffUser = null;
                //    vr.OPSStaffStatusId = 0;

                //    context.SaveChanges();

                //}
                //else
                {
                    var isnew = vr.OPSRemark.Trim().Replace(" ", "") != remark.Trim().Replace(" ", "") || vr.OPSStatusId != status || vr.OPSUser != user;
                    if (isnew)
                    {
                        vr.OPSRemark = remark;
                        vr.OPSUser = user;
                        vr.OPSRemarkDate = DateTime.Now;
                        vr.OPSStatusId = status;

                        // vr.OPSStaffConfirmDate = null;
                        // vr.OPSStaffDateVisit = null;
                        // vr.OPSStaffRemark = null;
                        // vr.OPSStaffRemarkDate = null;
                        // vr.OPSStaffUser = null;
                        vr.OPSStaffStatusId = 0;


                        context.SaveChanges();

                    }
                }
            }




            return Ok(true);
        }

        public class EFBVoyageReportViewModel
        {

            public int Id { get; set; }
            public int? FlightId { get; set; }
            public string Route { get; set; }
            public int? RestReduction { get; set; }
            public int? DutyExtention { get; set; }
            public string Report { get; set; }
            public string DatePICSignature { get; set; }
            public int? ActionedById { get; set; }
            public string DateActioned { get; set; }
            public string DateConfirmed { get; set; }
            public int? DepDelay { get; set; }
            public List<int> Irregularities { get; set; }
            public List<int> Reasons { get; set; }
            public List<int> DutyDisorders { get; set; }
            public string User { get; set; }
            public bool? AttForm_ASR { get; set; }
            public bool? AttForm_CSR { get; set; }
            public bool? AttForm_CR { get; set; }
            public bool? AttForm_Other { get; set; }
            public bool? IsForInformation { get; set; }
            public bool? IsActionRequired { get; set; }
            public bool? AttForm_ACCIDET { get; set; }

            public string OtherForm { get; set; }
            public string ActionTaken { get; set; }
        }

        public class EFBVoyageReportDTO
        {
            public int Id { get; set; }
            public int? FlightId { get; set; }
            public string Route { get; set; }
            public int? RestReduction { get; set; }
            public int? DutyExtention { get; set; }
            public string Report { get; set; }
            public DateTime? DatePICSignature { get; set; }
            public int? ActionedById { get; set; }
            public DateTime? DateActioned { get; set; }
            public DateTime? DateConfirmed { get; set; }
            public int? DepDelay { get; set; }
            public string DateUpdate { get; set; }
            public string User { get; set; }
            public string JLSignedBy { get; set; }
            public DateTime? JLDatePICApproved { get; set; }
            public int? PICId { get; set; }
            public string PIC { get; set; }
            public string OPSRemark { get; set; }
            public DateTime? OPSRemarkDate { get; set; }
            public int? OPSId { get; set; }
            public DateTime? OPSConfirmDate { get; set; }
            public string OPSStaffRemark { get; set; }
            public DateTime? OPSStaffDateVisit { get; set; }
            public DateTime? OPSStaffConfirmDate { get; set; }
            public int? OPSStaffId { get; set; }
            public DateTime? OPSStaffRemarkDate { get; set; }
            public string OPSUser { get; set; }
            public string OPSStaffUser { get; set; }
            public int? OPSStatusId { get; set; }
            public int? OPSStaffStatusId { get; set; }
            public DateTime? DateSign { get; set; }
            public int? ReporterId { get; set; }
            public string Status { get; set; }
            public int? FormNo { get; set; }
            public DateTime? DateStatus { get; set; }
            public int? StatusEmployeeId { get; set; }
            public DateTime? DateOccurrence { get; set; }
            public string Result { get; set; }
            public bool? AttForm_ASR { get; set; }
            public bool? AttForm_CSR { get; set; }
            public bool? AttForm_CR { get; set; }
            public bool? AttForm_Other { get; set; }
            public string ActionTaken { get; set; }
            public bool? IsForInformation { get; set; }
            public bool? IsActionRequired { get; set; }
            public string OtherForm { get; set; }
            public bool? AttForm_ACCIDET { get; set; }

            // Flattened navigation properties (as IDs or lists)
            public List<EFBFlightIrregularityDto> EFBFlightIrregularities { get; set; }
            public List<EFBReasonDto> EFBReasons { get; set; }
            public List<EFBDutyDisorderDto> EFBDutyDisorders { get; set; }
        }

        public class EFBFlightIrregularityDto
        {
            public int Id { get; set; }
            public int? VoyageReportId { get; set; }
            public int? IrrId { get; set; }

        }

        public class EFBReasonDto
        {
            public int Id { get; set; }
            public int? VoyageReportId { get; set; }
            public int? ReasonId { get; set; }

        }

        public class EFBDutyDisorderDto
        {
            public int Id { get; set; }
            public int? VoyageReportId { get; set; }
            public int? DisorderId { get; set; }

        }

        [HttpPost]
        [Route("api/voyage/save")]
        public async Task<DataResponse> SaveEFBVoyageReport(EFBVoyageReportViewModel EFBVoyageReport)
        {
            try
            {
                var _context = new dbEntities();
                _context.Configuration.LazyLoadingEnabled = false;
                var entity = await _context.EFBVoyageReports.FirstOrDefaultAsync(q => q.FlightId == EFBVoyageReport.FlightId);

                if (entity == null)
                {
                    entity = new EFBVoyageReport();
                    _context.EFBVoyageReports.Add(entity);
                }
                entity.User = EFBVoyageReport.User;
                entity.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                entity.FlightId = EFBVoyageReport.FlightId;
                entity.Route = EFBVoyageReport.Route;
                entity.RestReduction = EFBVoyageReport.RestReduction;
                entity.DutyExtention = EFBVoyageReport.DutyExtention;
                entity.DepDelay = EFBVoyageReport.DepDelay;
                entity.Report = EFBVoyageReport.Report;
                if (EFBVoyageReport.DatePICSignature != null)
                    entity.DatePICSignature = DateTime.Parse(EFBVoyageReport.DatePICSignature);
                else
                    entity.DatePICSignature = null;
                if (EFBVoyageReport.DateActioned != null)
                    entity.DateActioned = DateTime.Parse(EFBVoyageReport.DateActioned);
                else
                    entity.DatePICSignature = null;
                entity.DateActioned = null;
                entity.DateConfirmed = null;


                entity.AttForm_ASR = EFBVoyageReport.AttForm_ASR;
                entity.AttForm_CSR = EFBVoyageReport.AttForm_CSR;
                entity.AttForm_CR = EFBVoyageReport.AttForm_CR;
                entity.AttForm_Other = EFBVoyageReport.AttForm_Other;
                entity.IsForInformation = EFBVoyageReport.IsForInformation;
                entity.IsActionRequired = EFBVoyageReport.IsActionRequired;
                entity.AttForm_ACCIDET = EFBVoyageReport.AttForm_ACCIDET;

                entity.OtherForm = EFBVoyageReport.OtherForm;
                entity.ActionTaken = EFBVoyageReport.ActionTaken;






                var exist = await _context.EFBFlightIrregularities.Where(q => q.VoyageReportId == entity.Id).ToListAsync();
                _context.EFBFlightIrregularities.RemoveRange(exist);

                if (EFBVoyageReport.Irregularities != null)
                {
                    foreach (int x in EFBVoyageReport.Irregularities)
                    {
                        entity.EFBFlightIrregularities.Add(new EFBFlightIrregularity()
                        {
                            EFBVoyageReport = entity,
                            IrrId = x
                        });

                    }
                }

                var existReason = await _context.EFBReasons.Where(q => q.VoyageReportId == entity.Id).ToListAsync();
                _context.EFBReasons.RemoveRange(existReason);

                if (EFBVoyageReport.Reasons != null)
                {
                    foreach (int x in EFBVoyageReport.Reasons)
                    {
                        entity.EFBReasons.Add(new EFBReason()
                        {
                            EFBVoyageReport = entity,
                            ReasonId = x
                        });

                    }
                }


                var existDutyDisorders = await _context.EFBDutyDisorders.Where(q => q.VoyageReportId == entity.Id).ToListAsync();
                _context.EFBDutyDisorders.RemoveRange(existDutyDisorders);

                if (EFBVoyageReport.DutyDisorders != null)
                {
                    foreach (int x in EFBVoyageReport.DutyDisorders)
                    {
                        entity.EFBDutyDisorders.Add(new EFBDutyDisorder()
                        {
                            //VoyageReportId=entity.Id,
                            EFBVoyageReport = entity,
                            DisorderId = x
                        });

                    }
                }

                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {

                    return new DataResponse
                    {
                        IsSuccess = true,
                        Data = new EFBVoyageReportDTO
                        {
                            Id = entity.Id,
                            FlightId = entity.FlightId,
                            Route = entity.Route,
                            RestReduction = entity.RestReduction,
                            DutyExtention = entity.DutyExtention,
                            DepDelay = entity.DepDelay,
                            Report = entity.Report,
                            DatePICSignature = entity.DatePICSignature,
                            DateActioned = entity.DateActioned,
                            DateConfirmed = entity.DateConfirmed,
                            DateUpdate = entity.DateUpdate,
                            User = entity.User,
                            JLSignedBy = entity.JLSignedBy,
                            JLDatePICApproved = entity.JLDatePICApproved,
                            PICId = entity.PICId,
                            PIC = entity.PIC,
                            OPSRemark = entity.OPSRemark,
                            OPSRemarkDate = entity.OPSRemarkDate,
                            OPSId = entity.OPSId,
                            OPSConfirmDate = entity.OPSConfirmDate,
                            OPSStaffRemark = entity.OPSStaffRemark,
                            OPSStaffDateVisit = entity.OPSStaffDateVisit,
                            OPSStaffConfirmDate = entity.OPSStaffConfirmDate,
                            OPSStaffId = entity.OPSStaffId,
                            OPSStaffRemarkDate = entity.OPSStaffRemarkDate,
                            OPSUser = entity.OPSUser,
                            OPSStaffUser = entity.OPSStaffUser,
                            OPSStatusId = entity.OPSStatusId,
                            OPSStaffStatusId = entity.OPSStaffStatusId,
                            DateSign = entity.DateSign,
                            ReporterId = entity.ReporterId,
                            Status = entity.Status,
                            FormNo = entity.FormNo,
                            DateStatus = entity.DateStatus,
                            StatusEmployeeId = entity.StatusEmployeeId,
                            DateOccurrence = entity.DateOccurrence,
                            Result = entity.Result,
                            AttForm_ASR = entity.AttForm_ASR,
                            AttForm_CSR = entity.AttForm_CSR,
                            AttForm_CR = entity.AttForm_CR,
                            AttForm_Other = entity.AttForm_Other,
                            ActionTaken = entity.ActionTaken,
                            IsForInformation = entity.IsForInformation,
                            IsActionRequired = entity.IsActionRequired,
                            OtherForm = entity.OtherForm,
                            AttForm_ACCIDET = entity.AttForm_ACCIDET,
                            EFBFlightIrregularities = (from e in exist select new EFBFlightIrregularityDto { VoyageReportId = e.VoyageReportId, IrrId = e.IrrId, Id = e.Id}).ToList(),
                            EFBReasons = (from e in existReason select new EFBReasonDto{ VoyageReportId = e.VoyageReportId, ReasonId = e.ReasonId, Id = e.Id }).ToList(),
                            EFBDutyDisorders = (from e in existDutyDisorders select new EFBDutyDisorderDto { VoyageReportId = e.VoyageReportId, DisorderId = e.DisorderId, Id = e.Id }).ToList(),
                        }
                    };

                }

                else
                    return new DataResponse() { IsSuccess = false, Errors = new List<string>() { saveResult.ToString() }, Data = saveResult };


            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse() { IsSuccess = false, Errors = new List<string>() { msg } };
            }




        }

        [HttpPost]
        [Route("api/vr/flights")]
        public async Task<DataResponse> GetEFBVRsByFlightIds(List<int> ids)
        {
            var _context = new dbEntities();
            var _ids = ids.Select(q => (Nullable<int>)q).ToList();
            var entity = await _context.EFBVoyageReports.Where(q => _ids.Contains(q.FlightId)).ToListAsync();
            return new DataResponse()
            {
                Data = entity,
                IsSuccess = true

            };
        }

        [HttpGet]
        [Route("api/voyage/flight/{flightId}")]
        public async Task<DataResponse> GetEFBVoyageReportByFlightId(int flightId)
        {
            try
            {
                var _context = new dbEntities();
                _context.Configuration.LazyLoadingEnabled = false;
                _context.Configuration.ProxyCreationEnabled = false;

                var voyage = await _context.EFBVoyageReports.SingleOrDefaultAsync(q => q.FlightId == flightId);
                if (voyage != null)
                {
                    var exist = await _context.EFBFlightIrregularities.Where(q => q.VoyageReportId == voyage.Id).ToListAsync();
                    

                    var existReason = await _context.EFBReasons.Where(q => q.VoyageReportId == voyage.Id).ToListAsync();
                   


                    var existDutyDisorders = await _context.EFBDutyDisorders.Where(q => q.VoyageReportId == voyage.Id).ToListAsync();
                    
                    var Irregularities = await _context.EFBFlightIrregularities.Where(q => q.VoyageReportId == voyage.Id).ToListAsync();
                    var Reasons = await _context.EFBReasons.Where(q => q.VoyageReportId == voyage.Id).ToListAsync();
                    var DutyDisorders = await _context.EFBDutyDisorders.Where(q => q.VoyageReportId == voyage.Id).ToListAsync();
                    //var irregularity = await _context.EFBFlightIrregularities.Where(q => q.VoyageReportId == voyage.Id).Select(q => q.IrrId).ToListAsync();
                    // var reason = await _context.EFBReasons.Where(q => q.VoyageReportId == voyage.Id).Select(q => q.ReasonId).ToListAsync();
                    return new DataResponse()
                    {
                        //Data = new
                        //{
                        //    voyage,
                        //    irregularity,
                        //    reason
                        //},
                        Data = new EFBVoyageReportDTO
                        {
                            Id = voyage.Id,
                            FlightId = voyage.FlightId,
                            Route = voyage.Route,
                            RestReduction = voyage.RestReduction,
                            DutyExtention = voyage.DutyExtention,
                            DepDelay = voyage.DepDelay,
                            Report = voyage.Report,
                            DatePICSignature = voyage.DatePICSignature,
                            DateActioned = voyage.DateActioned,
                            DateConfirmed = voyage.DateConfirmed,
                            DateUpdate = voyage.DateUpdate,
                            User = voyage.User,
                            JLSignedBy = voyage.JLSignedBy,
                            JLDatePICApproved = voyage.JLDatePICApproved,
                            PICId = voyage.PICId,
                            PIC = voyage.PIC,
                            OPSRemark = voyage.OPSRemark,
                            OPSRemarkDate = voyage.OPSRemarkDate,
                            OPSId = voyage.OPSId,
                            OPSConfirmDate = voyage.OPSConfirmDate,
                            OPSStaffRemark = voyage.OPSStaffRemark,
                            OPSStaffDateVisit = voyage.OPSStaffDateVisit,
                            OPSStaffConfirmDate = voyage.OPSStaffConfirmDate,
                            OPSStaffId = voyage.OPSStaffId,
                            OPSStaffRemarkDate = voyage.OPSStaffRemarkDate,
                            OPSUser = voyage.OPSUser,
                            OPSStaffUser = voyage.OPSStaffUser,
                            OPSStatusId = voyage.OPSStatusId,
                            OPSStaffStatusId = voyage.OPSStaffStatusId,
                            DateSign = voyage.DateSign,
                            ReporterId = voyage.ReporterId,
                            Status = voyage.Status,
                            FormNo = voyage.FormNo,
                            DateStatus = voyage.DateStatus,
                            StatusEmployeeId = voyage.StatusEmployeeId,
                            DateOccurrence = voyage.DateOccurrence,
                            Result = voyage.Result,
                            AttForm_ASR = voyage.AttForm_ASR,
                            AttForm_CSR = voyage.AttForm_CSR,
                            AttForm_CR = voyage.AttForm_CR,
                            AttForm_Other = voyage.AttForm_Other,
                            ActionTaken = voyage.ActionTaken,
                            IsForInformation = voyage.IsForInformation,
                            IsActionRequired = voyage.IsActionRequired,
                            OtherForm = voyage.OtherForm,
                            AttForm_ACCIDET = voyage.AttForm_ACCIDET,
                            EFBFlightIrregularities = (from e in exist select new EFBFlightIrregularityDto { VoyageReportId = e.VoyageReportId, IrrId = e.IrrId, Id = e.Id }).ToList(),
                            EFBReasons = (from e in existReason select new EFBReasonDto { VoyageReportId = e.VoyageReportId, ReasonId = e.ReasonId, Id = e.Id }).ToList(),
                            EFBDutyDisorders = (from e in existDutyDisorders select new EFBDutyDisorderDto { VoyageReportId = e.VoyageReportId, DisorderId = e.DisorderId, Id = e.Id }).ToList(),

                        },
                        IsSuccess = true

                    };
                }

               
                else
                {
                    return new DataResponse()
                    {
                        Data = null,
                        IsSuccess = false
                    };
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "    INNER:" + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }

        }

        public class response_asr
        {
            public int Id { get; set; }
            public int FlightId { get; set; }
            public DateTime FlightDate { get; set; }
            public string FlightNumber { get; set; }
            public string Route { get; set; }
            public string Register { get; set; }
            public string PIC { get; set; }
            public string P1Name { get; set; }
            public string IPName { get; set; }
            public string SIC { get; set; }
            public string P2Name { get; set; }
            public string Summary { get; set; }
            public int? P1Id { get; set; }
            public int? PICId { get; set; }
            public int? IPId { get; set; }
        }

        [Route("api/qa/notify/asr/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNotifyASR(int id)
        {
            try
            {
                var context = new dbEntities();
                List<qa_notification_history> _result = new List<qa_notification_history>();
                var url = "https://ava.skybag.app/apiapsb/api/asr/view/abs/" + id;
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    var str = webClient.DownloadString(url);
                    response_asr asr = JsonConvert.DeserializeObject<response_asr>(str);
                    var pic = context.ViewProfiles.Where(q => q.Id == asr.PICId).FirstOrDefault();

                    //var pic_msg1 = "ضمن سپاس از ارسال گزارش، پس از بررسی و اقدامات انجام شده، حصول نتیجه در صورت لزوم به شما ابلاغ خواهد شد. با تشکر، مدیریت ایمنی و تضمین کیفیت هواپیمایی وارش";





                    List<string> prts = new List<string>();
                    prts.Add("New ASR Notification");
                    //prts.Add("Dear ");
                    prts.Add("Dear " + asr.PIC);
                    prts.Add("Please click on the below link to see details.");
                    prts.Add("https://ava.report.airpocket.app/frmreportview.aspx?type=17&fid=" + asr.FlightId);
                    prts.Add("Date: " + asr.FlightDate.ToString("yyyy-MM-dd"));
                    prts.Add("Route: " + asr.Route);
                    prts.Add("Register: " + asr.Register);
                    prts.Add("PIC: " + asr.PIC);
                    prts.Add("FO: " + asr.P2Name);
                    prts.Add("FP: " + asr.SIC);
                    prts.Add("Event Summary:");
                    prts.Add(asr.Summary);


                    var text = String.Join("\n", prts);
                    List<qa_notification_history> nots = new List<qa_notification_history>();

                    var not_receivers = context.qa_notification_receiver.Where(q => q.is_active == true).ToList();

                    MelliPayamac m1 = new MelliPayamac();
                    foreach (var rec in not_receivers)
                    {
                        List<string> prts2 = new List<string>();
                        prts2.Add("New ASR Notification");
                        prts2.Add("Dear " + rec.rec_name);
                        prts2.Add("Please click on the below link to see details.");

                        prts2.Add("https://ava.report.airpocket.app/frmreportview.aspx?type=17&fid=" + asr.FlightId);
                        prts2.Add("Date: " + asr.FlightDate.ToString("yyyy-MM-dd"));
                        prts2.Add("Route: " + asr.Route);
                        prts2.Add("Register: EP-" + asr.Register);
                        prts2.Add("PIC: " + asr.PIC);
                        prts2.Add("FO: " + asr.P2Name);
                        prts2.Add("FP: " + asr.SIC);
                        prts2.Add("Event Summary:");
                        prts2.Add(asr.Summary);

                        var text2 = String.Join("\n", prts2);



                        //List<string> mail_parts = new List<string>();

                        //mail_parts.Add("<b>" + "New ASR Notification" + "</b><br/>");
                        //mail_parts.Add("<b>" + "Dear " + rec.rec_name + "</b><br/>");
                        //mail_parts.Add("Please click on the below link to see details.");

                        //mail_parts.Add("https://ava.report.airpocket.app/frmreportview.aspx?type=17&fid=" + asr.FlightId + "<br/>");
                        //mail_parts.Add("Date: " + "<b>" + asr.FlightDate.ToString("yyyy-MM-dd") + "</b><br/>");
                        //mail_parts.Add("Route: " + "<b>" + asr.Route + "</b><br/>");
                        //mail_parts.Add("Register: " + "<b>" + "EP-" + asr.Register + "</b><br/>");
                        //mail_parts.Add("PIC: " + "<b>" + asr.PIC + "</b><br/>");
                        //mail_parts.Add("FO: " + "<b>" + asr.P2Name + "</b><br/>");
                        //mail_parts.Add("FP: " + "<b>" + asr.SIC + "</b><br/>");
                        //mail_parts.Add("<b>" + "Event Summary:" + "</b><br/>");
                        //mail_parts.Add(asr.Summary);
                        //mail_parts.Add("<br/>");
                        //mail_parts.Add("<br/>");
                        //mail_parts.Add("Sent by AIRPOCKET" + "<br/>");
                        //mail_parts.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));


                        //var email_body = String.Join("\n", mail_parts);
                        //if (!string.IsNullOrEmpty(rec.email))
                        //{
                        //    MailHelper mail_helper = new MailHelper();
                        //    mail_helper.SendMailByAirpocket(rec.email, rec.rec_name, "ASR NOTIFICATION (FLT NO " + asr.FlightNumber + ", ROUTE " + asr.Route + ", DATE: " + asr.FlightDate.ToString("yyyy-MM-dd") + ")", email_body);

                        //}
                        //if (!string.IsNullOrEmpty(rec.email2))
                        //{
                        //    MailHelper mail_helper = new MailHelper();
                        //    mail_helper.SendMailByAirpocket(rec.email2, rec.rec_name, "ASR NOTIFICATION (FLT NO " + asr.FlightNumber + ", ROUTE " + asr.Route + ", DATE: " + asr.FlightDate.ToString("yyyy-MM-dd") + ")", email_body);

                        //}


                        var not_history = new qa_notification_history()
                        {
                            date_send = DateTime.Now,
                            entity_id = id,
                            entity_type = 8,
                            message_text = text2,
                            message_type = 1,
                            rec_id = rec.rec_id,
                            rec_mobile = rec.mobile,
                            rec_name = rec.rec_name,
                            counter = 0,

                        };

                        var smsResult1 = m1.send(not_history.rec_mobile, null, text2)[0];
                        not_history.ref_id = smsResult1.ToString();
                        _result.Add(not_history);
                        System.Threading.Thread.Sleep(2000);

                    }
                    var not_history_pic = new qa_notification_history()
                    {
                        date_send = DateTime.Now,
                        entity_id = id,
                        entity_type = 8,
                        //message_text = pic_msg1,
                        message_text = text,
                        message_type = 2,
                        rec_id = asr.PICId,
                        rec_mobile = pic.Mobile,
                        rec_name = pic.Name,
                        counter = 0,
                    };
                    var not_history_pic2 = new qa_notification_history()
                    {
                        date_send = DateTime.Now,
                        entity_id = id,
                        entity_type = 8,
                        message_text = text,
                        message_type = 1,
                        rec_id = asr.PICId,
                        rec_mobile = pic.Mobile,
                        rec_name = pic.Name,
                        counter = 0,
                    };

                    MelliPayamac m1_pic = new MelliPayamac();
                    var m1_pic_result = m1_pic.send(not_history_pic.rec_mobile,null ,not_history_pic.message_text)[0];
                    not_history_pic.ref_id = m1_pic_result.ToString();

                    MelliPayamac m_pic = new MelliPayamac();
                    var m_pic_result = m_pic.send(not_history_pic2.rec_mobile,null, not_history_pic2.message_text)[0];
                    not_history_pic2.ref_id = m_pic_result.ToString();

                    _result.Add(not_history_pic);
                    _result.Add(not_history_pic2);

                    System.Threading.Thread.Sleep(20000);
                    //foreach (var x in _result)
                    //{
                    //    MelliPayamac m_status = new MelliPayamac();
                    //    x.status = m_status.send(Convert.ToInt64(x.ref_id));

                    //    context.qa_notification_history.Add(x);
                    //}
                    
                    context.SaveChanges();
                }

                return Ok(_result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "      " + ex.InnerException.Message;
                return Ok(msg);
            }
        }



        [HttpGet]
        [Route("api/voyage/{Id}")]
        public async Task<DataResponse> GetEFBVoyageReportById(int Id)
        {
            var _context = new dbEntities();
            var voyage = await _context.ViewEFBVoyageReportsAlls.SingleOrDefaultAsync(q => q.Id == Id);
            if (voyage != null)
            {
                var irregularity = await _context.EFBFlightIrregularities.Where(q => q.VoyageReportId == voyage.Id).ToListAsync();
                var reason = await _context.EFBReasons.Where(q => q.VoyageReportId == voyage.Id).ToListAsync();
                return new DataResponse()
                {
                    Data = new
                    {
                        voyage,
                        irregularity,
                        reason
                    },
                    IsSuccess = true

                };
            }
            else
            {
                return new DataResponse()
                {
                    Data = voyage,
                    IsSuccess = false
                };
            }
        }


        [Route("api/vr/remark/staff/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostVrRemarkStaff(dynamic dto)
        {
            string user = Convert.ToString(dto.user);
            string remark = Convert.ToString(dto.remark);
            int status = Convert.ToInt32(dto.status);
            int id = Convert.ToInt32(dto.id);

            var context = new dbEntities();

            var vr = context.EFBVoyageReports.FirstOrDefault(q => q.Id == id);
            if (vr.OPSStaffStatusId == null)
                vr.OPSStaffStatusId = 0;
            if (vr.OPSStaffRemark == null)
                vr.OPSStaffRemark = "";
            if (vr != null)
            {

                var isnew = vr.OPSStaffRemark.Trim().Replace(" ", "") != remark.Trim().Replace(" ", "") || vr.OPSStaffStatusId != status || vr.OPSStaffUser != user;
                if (isnew)
                {
                    vr.OPSStaffRemark = remark;
                    vr.OPSStaffUser = user;
                    vr.OPSStaffRemarkDate = DateTime.Now;
                    vr.OPSStaffStatusId = status;




                    context.SaveChanges();

                }

            }




            return Ok(true);
        }

        [HttpPost]
        [Route("api/save/followup")]
        public async Task<DataResponse> SaveFollowUp(dynamic dto)
        {

            try
            {
                //int Id = dto.Id;
                //var entity = context.QAFollowingUps.SingleOrDefault(q => q.Id == Id);
                //if (entity == null)
                //{
                //    entity = new QAFollowingUp();
                //    context.QAFollowingUps.Add(entity);
                //}
                var context = new dbEntities();
                int Type = dto.Type;
                var respEmployee = context.QAResponsibilties.Where(q => q.Type == Type && q.IsResponsible == true).ToList();

                foreach (var x in respEmployee)
                {

                    var entity = new QAFollowingUp();
                    context.QAFollowingUps.Add(entity);

                    entity.EntityId = dto.EntityId;
                    entity.DateStatus = DateTime.Now;
                    entity.Type = dto.Type;
                    entity.ReferredId = x.ReceiverEmployeeId;
                    entity.ReferrerId = null;
                    entity.ReviewResult = 2;
                    entity.Priority = dto.Priority;
                    entity.DeadLine = dto.DeadLine;
                };


                context.SaveChanges();
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    Data = ex,
                    IsSuccess = false
                };
            }
        }
        [Route("api/qa/feedback/first/{no}/{eid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetQAFeedbackFirst(string no, string eid)
        {
            var _no = no;

            if (eid != "-1" && _no == "-1")
            {
                try
                {
                    var _eid = Convert.ToInt32(eid);
                    var context = new dbEntities();
                    _no = context.ViewProfiles.Where(q => q.Id == _eid).FirstOrDefault().Mobile;
                }
                catch (Exception ex)
                {

                }

            }
            var msg = "ضمن سپاس از ارسال گزارش، پس از بررسی و اقدامات انجام شده، حصول نتیجه در صورت لزوم به شما ابلاغ خواهد شد. با تشکر، مدیریت ایمنی و تضمین کیفیت هواپیمایی آوا";
            MelliPayamac m = new MelliPayamac();
            var smsResult = m.send(_no, null, msg)[0];
            var refids = new List<Int64>() { smsResult };
            System.Threading.Thread.Sleep(5000);
            //var status = m.getStatus(refids);

            return Ok(new { refids, _no, no, eid });
        }


    }
}