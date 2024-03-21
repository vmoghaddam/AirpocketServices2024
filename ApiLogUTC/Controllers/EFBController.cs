using ApiLogUTC.Models;
using ApiLogUTC.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiLogUTC.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EFBController : ApiController
    {

        [Route("api/efb/dr/save")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostDR(DSPReleaseViewModel DSPRelease)
        {
            var _context = new ppa_entities();

            var appleg = await _context.AppLegs.FirstOrDefaultAsync(q => q.FlightId == DSPRelease.FlightId);
            var appcrewflight = await _context.AppCrewFlights.Where(q => q.FlightId == appleg.FlightId && q.CrewId == appleg.PICId).FirstOrDefaultAsync();
            var fdpitems = await _context.FDPItems.Where(q => q.FDPId == appcrewflight.FDPId).ToListAsync();
            var fltIds = fdpitems.Select(q => q.FlightId).ToList();

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

            }


            var saveResult = await _context.SaveAsync();


            return Ok(new DataResponse() { IsSuccess = true, Data = saveResult });



            // return new DataResponse() { IsSuccess = false };
        }


        [Route("api/efb/dr/{flightId}")]
        public async Task<IHttpActionResult> GetDRByFlightId(int flightId)
        {
            var _context = new ppa_entities();
            var entity = await _context.ViewEFBDSPReleases.FirstOrDefaultAsync(q => q.FlightId == flightId);
            return Ok(new DataResponse()
            {
                Data = entity,
                IsSuccess = true

            });
        }

        public class DataResponse
        {
            public bool IsSuccess { get; set; }
            public object Data { get; set; }
            public List<string> Errors { get; set; }
        }

        public class DSPReleaseViewModel
        {
            public int? FlightId { get; set; }
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
            public string User { get; set; }
        }


    }
}
