using ApiAPSB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ApiAPSB.Controllers
{
    public class EFBController : ApiController
    {

        [HttpPost]
        [Route("api/asr/save")]
        public async Task<DataResponse> SaveEFBASR(ViewEFBASR EFBASR)
        {
            var _context = new dbEntities();
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
                entity.OccurrenceDate = EFBASR.OccurrenceDate;
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

                var saveResult = await _context.SaveChangesAsync();
                return new DataResponse() { IsSuccess = true, Data = entity };
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
    }
}