////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Threading.Tasks;
////using System.Web;
////using System.Web.Http;
////using System.Web.Http.Cors;
////using ApiFDM.Models;
////using ApiFDM.Objects;

////namespace ApiFDM.Controllers
////{
////    [EnableCors(origins: "*", headers: "*", methods: "*")]
////    public class QAController : ApiController
////    {

////        dbEntities context = new dbEntities();

////        [HttpGet]
////        [Route("api/get/csr/flightphase")]
////        public async Task<DataResponse> CSRFlightPhase()
////        {

////            //var result = context.QAOptions.Where(q => q.ParentId == 7);
////            var query = from x in context.QAOptions
////                        where x.ParentId == 7
////                        select new
////                        {
////                            Title = x.Title,
////                            Id = x.Id
////                        };
////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/csr/eventtitle")]
////        public async Task<DataResponse> CSREventTitle()
////        {
////            var query = from x in context.QAOptions
////                        where x.ParentId == 16
////                        select new
////                        {
////                            Title = x.Title,
////                            Id = x.Id
////                        };
////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/gia/dmgby")]
////        public async Task<DataResponse> GIADamageBy()
////        {
////            var query = from x in context.QAOptions
////                        where x.ParentId == 68
////                        select new
////                        {
////                            Title = x.Title,
////                            Id = x.Id
////                        };
////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/gia/lighting")]
////        public async Task<DataResponse> GetGAILighting()
////        {
////            var query = from x in context.QAOptions
////                        where x.ParentId == 62
////                        select new
////                        {
////                            Title = x.Title,
////                            Id = x.Id
////                        };
////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/gia/surface")]
////        public async Task<DataResponse> GetGIASurface()
////        {
////            var query = from x in context.QAOptions
////                        where x.ParentId == 54
////                        select new
////                        {
////                            Title = x.Title,
////                            Id = x.Id
////                        };

////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/gia/weather")]
////        public async Task<DataResponse> GetGIAWeather()
////        {
////            var query = from x in context.QAOptions
////                        where x.ParentId == 48
////                        select new
////                        {
////                            Title = x.Title,
////                            Id = x.Id
////                        };
////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/mor/compnspec")]
////        public async Task<DataResponse> GetMORComponentSpec()
////        {
////            var query = from x in context.QAOptions
////                        where x.ParentId == 43
////                        select new
////                        {
////                            x.Id,
////                            x.Title
////                        };
////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/flightinformation/{flightId}")]
////        public async Task<DataResponse> GetFlightInformation(int flightId)
////        {

////            var result = context.AppLegs.SingleOrDefault(q => q.FlightId == flightId);
////            return new DataResponse()
////            {
////                Data = result,
////                IsSuccess = true
////            };
////        }


////        [HttpGet]
////        [Route("api/get/csr/{FlightId}")]
////        public async Task<DataResponse> GetCSRByFlightId(int FlightId)
////        {
////            var query = from x in context.ViewQACSRs
////                        where x.FlightId == FlightId
////                        select x;

////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/csr/byid/{Id}")]
////        public async Task<DataResponse> GetCSRById(int Id)
////        {
////            var query = from x in context.ViewQACSRs
////                        where x.FlightId == Id
////                        select x;

////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }


////        [HttpGet]
////        [Route("api/get/mor/{FlightId}")]
////        public async Task<DataResponse> GetMORByFlightId(int FlightId)
////        {
////            var query = from x in context.ViewQAMORs
////                        where x.FlightId == FlightId
////                        select x;

////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/mor/byid/{Id}")]
////        public async Task<DataResponse> GetMORById(int Id)
////        {
////            var query = from x in context.ViewQAMORs
////                        where x.FlightId == Id
////                        select x;

////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/gia/{FlightId}")]
////        public async Task<DataResponse> GetGIAByFlightId(int FlightId)
////        {
////            var query = from x in context.ViewQAGrounds
////                        where x.FlightId == FlightId
////                        select x;

////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/gia/byid/{Id}")]
////        public async Task<DataResponse> GetGIAById(int Id)
////        {
////            var query = from x in context.ViewQAGrounds
////                        where x.FlightId == Id
////                        select x;

////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

////        [HttpGet]
////        [Route("api/get/vhr/{Id}")]
////        public async Task<DataResponse> GetVHRById(int Id)
////        {
////            var query = from x in context.ViewQAHazards
////                        where x.Id == Id
////                        select x;

////            return new DataResponse()
////            {
////                Data = query,
////                IsSuccess = true
////            };
////        }

       

////        [HttpPost]
////        [Route("api/save/csr")]
////        public async Task<DataResponse> SaveQACSR(dynamic dto)
////        {

////            try
////            {
////                int Id = dto.Id;
////                var entity = context.QACSRs.SingleOrDefault(q => q.Id == Id);
////                if (entity == null)
////                {
////                    entity = new QACSR();
////                }

////                entity.FlightPhaseId = dto.FlightPhaseId;
////                entity.FlightId = dto.FlightId;
////                entity.Describtion = dto.description;
////                entity.EventLocation = dto.Location;
////                entity.ReportFiledBy = dto.ReportFieldby;
////                entity.OccurrenceDateTime = dto.OccurrenceDate;
////                entity.Recommendation = dto.Recommendation;
////                entity.WeatherCondition = dto.WeatherCondition;
////                entity.Name = dto.Name;
////                entity.BOX = dto.box;
////                entity.RefNumber = dto.RefNumber;
////                foreach (var x in dto.EventTitleIds)
////                {
////                    entity.QACSREvents.Add(new QACSREvent()
////                    {
////                        EventTitleId = x
////                    });
////                };

////                context.QACSRs.Add(entity);
////                context.SaveChanges();

////                return new DataResponse()
////                {
////                    Data = entity,
////                    IsSuccess = true
////                };
////            }
////            catch (Exception ex)
////            {
////                return new DataResponse()
////                {
////                    Data = ex.InnerException,
////                    IsSuccess = false
////                };
////            }
////        }

////        [HttpPost]
////        [Route("api/save/mor")]
////        public async Task<DataResponse> SaveMOR(dynamic dto)
////        {
////            try
////            {

////                int Id = dto.Id;
////                var entity = context.QAMORs.SingleOrDefault(q => q.Id == Id);
////                if (entity == null)
////                    entity = new QAMOR();

////                entity.OccurrenceDateTime = dto.OccurrenceDateTime;
////                entity.ComponentSpecificationId = dto.ComponentSpecificationId;
////                entity.ATLNo = dto.ATLNo;
////                entity.Reference = dto.Reference;
////                entity.FlightId = dto.FlightId;
////                //utcTime;
////                entity.StationId = dto.StationId;
////                entity.EventDescription = dto.EventDescription;
////                entity.ActionTakenDescription = dto.ActionTakenDescription;
////                entity.Name = dto.Name;
////                entity.CAALicenceNo = dto.CAALicenceNo;
////                entity.SerialNumber = dto.SerialNumber;
////                entity.PartNumber = dto.PartNumber;
////                context.SaveChanges();
////                return new DataResponse()
////                {
////                    Data = entity,
////                    IsSuccess = true
////                };

////            }
////            catch (Exception ex)
////            {
////                return new DataResponse()
////                {
////                    Data = ex.InnerException,
////                    IsSuccess = false
////                };
////            }

////        }

////        [HttpPost]
////        [Route("api/save/vhr")]
////        public async Task<DataResponse> SaveVHR(dynamic dto)
////        {

////            try
////            {
////                int Id = dto.Id;
////                var entity = context.QAHazards.SingleOrDefault(q => q.Id == Id);
////                if (entity == null)
////                    entity = new QAHazard();

////                entity.Name = dto.Name; 
////                entity.Email = dto.Email;
////                entity.TelNumber = dto.TelNumber;
////                entity.ReportDate = dto.ReportDate;
////                entity.AffectedArea = dto.AffectedArea;
////                entity.HazardDate = dto.HazardDate;
////                entity.HazardDescription = dto.HazardDescription;
////                entity.RecommendedAction = dto.RecommendedAction;

////                context.SaveChanges();
////                return new DataResponse()
////                {
////                    Data = entity,
////                    IsSuccess = false
////                };

////            }
////            catch (Exception ex)
////            {
////                return new DataResponse()
////                {
////                    Data = ex.InnerException,
////                    IsSuccess = false
////                };
////            }


////        }

////        [HttpPost]
////        [Route("api/save/gia")]
////        public async Task<DataResponse> SaveGIA(dynamic dto)
////        {

////            try
////            {
////                int Id = dto.Id;
////                var entity = context.QAGroundIADs.SingleOrDefault(q => q.Id == Id);
////                if (entity == null)
////                    entity = new QAGroundIAD();

////                entity.Airport = dto.Airport;
////                entity.AirportId = dto.AirportId;
////                entity.Area = dto.Area;
////                entity.ContributoryFactors = dto.ContributoryFactors;
////                entity.CorrectiveActionTaken = dto.CorrectiveActionTaken;
////                entity.DamageById = dto.DamageById;
////                entity.DamageDate = dto.DamageDate;
////                entity.DamageDetails = dto.DamageDetails;
////                //entity.DateSigne
////                entity.EmploeeId = entity.EmploeeId;
////                entity.CorrectiveActionTaken = dto.CorrectiveActionTaken;
////                entity.EmployeesNonFatalityNr = dto.EmployeesNonFatalityNr;
////                entity.Event = dto.Event;
////                entity.FlightId = dto.FlightId;
////                //entity.FlightInformation
////                entity.OperationPhase = dto.OperationPhase;
////                entity.OthersFatalityNr = dto.OthersFatalityNr;
////                entity.OthersNonFatalityNr = dto.OthersNonFatalityNr;
////                entity.OtherSuggestions = dto.OtherSuggestions;
////                entity.PassengersFatalityNr = dto.PassengersFatalityNr;
////                entity.PassengersNonFatalityNr = dto.PassengersNonFatalityNr;
////                entity.PersonnelCompany1 = dto.PersonnelCompany1;
////                entity.PersonnelJobTitle1 = dto.PersonnelJobTitle1;
////                entity.PersonnelLicense1 = dto.PersonnelLicense1;
////                entity.PersonnelName1 = dto.PersonnelName1;
////                entity.PersonnelStaffNr1 = dto.PersonnelStaffNr1;
////                entity.PersonnelCompany2 = dto.PersonnelCompany2;
////                entity.PersonnelJobTitle2 = dto.PersonnelJobTitle2;
////                entity.PersonnelLicense2 = dto.PersonnelLicense2;
////                entity.PersonnelName2 = dto.PersonnelName2;
////                entity.PersonnelStaffNr2 = dto.PersonnelStaffNr2;
////                entity.PersonnelCompany3 = dto.PersonnelCompany3;
////                entity.PersonnelJobTitle3 = dto.PersonnelJobTitle3;
////                entity.PersonnelLicense3 = dto.PersonnelLicense3;
////                entity.PersonnelName3 = dto.PersonnelName3;
////                entity.PersonnelStaffNr3 = dto.PersonnelStaffNr3;
////                entity.ScheduledGroundTime = dto.ScheduledGroundTime;
////                //entity.Title
////                entity.VEAge = dto.VEAge;
////                entity.VEArea = dto.VEArea;
////                entity.VEBrakesCon = dto.VEBrakesCon;
////                entity.VEFieldofVisionCon = dto.VEFieldofVisionCon;
////                entity.VEFromDrivingPoCon = dto.VEFromDrivingPoCon;
////                entity.VELastOverhaul = dto.VELastOverhaul;
////                entity.VELightsCon = dto.VELightsCon;
////                entity.VEOwner = dto.VEOwner;
////                entity.VEProtectionCon = dto.VEProtectionCon;
////                entity.VERemarks = dto.VERemarks;
////                entity.VESerialFleetNr = dto.VESerialFleetNr;
////                entity.VEStabilizersCon = dto.VEStabilizersCon;
////                entity.VESteeringCon = dto.VESteeringCon;
////                entity.VETowHitchCon = dto.VETowHitchCon;
////                entity.VEType = dto.VEType;
////                entity.VETyresCon = dto.VETyresCon;
////                entity.VEWarningDevicesCon = dto.VEWarningDevicesCon;
////                entity.VEWipersCon = dto.VEWipersCon;
////                entity.WXLightingId = dto.WXLightingId;
////                entity.WXSurfaceId = dto.WXSurfaceId;
////                entity.WXTemperature = dto.WXTemperature;
////                entity.WXVisibility = dto.WXVisibility;
////                entity.WXWeatherId = dto.WXWeatherId;
////                entity.WXWind = dto.WXWind;
////                context.SaveChanges();
////                return new DataResponse()
////                {
////                    Data = entity,
////                    IsSuccess = true
////                };

////            }
////            catch (Exception ex)
////            {
////                return new DataResponse()
////                {
////                    Data = ex.InnerException,
////                    IsSuccess = false
////                };
////            }
////        }
////    }
////}