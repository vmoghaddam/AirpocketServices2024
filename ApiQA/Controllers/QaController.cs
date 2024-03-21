using ApiQA.Models;
using ApiQA.ViewModels;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Net.Http.Headers;

namespace ApiQA.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QaController : ApiController
    {



        //[Route("api/efb/dr/{flightId}")]
        //public async Task<IHttpActionResult> GetDRByFlightId(int flightId)
        //{
        //    var _context = new ppa_entities();

        //}

        ppa_entities context = new ppa_entities();


        [HttpGet]
        [Route("api/get/station")]
        public async Task<DataResponse> GetStation()
        {
            try
            {
                var entity = from x in context.Airports
                             where x.Country == "DOM"
                             select new
                             {
                                 x.Id,
                                 x.IATA
                             };
                var result = entity.ToList();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            };
        }


        [HttpGet]
        [Route("api/creator/history/{cid}")]
        public async Task<DataResponse> CreatorHistory(int cid)
        {

            //var result = context.QAOptions.Where(q => q.ParentId == 7);
            var query = (from x in context.ViewQaReportsByCreators
                         where x.CreatorId == cid
                         orderby x.DateCreation descending, x.Status
                         select x).ToList();
            var keys = query.Select(q => q.EntityId + "_" + q.type).ToList();
            var feedbacks = context.ViewQaFeedbacks.Where(q => keys.Contains(q.FormKey)).ToList();
            return new DataResponse()
            {
                Data = new { query, feedbacks },
                IsSuccess = true
            };
        }

        [HttpPost]
        [Route("api/qa/save/feedback")]
        public async Task<DataResponse> SaveFeedBack(dynamic dto)
        {
            try
            {
                var entity = new QAFeedback();
                context.QAFeedbacks.Add(entity);

                entity.FormId = dto.EntityId;
                entity.FormType = dto.Type;
                entity.DateCreate = DateTime.Now;
                entity.CreatorId = -1;
                entity.Feedback = dto.Feedback;

                context.SaveChanges();

                return new DataResponse() { Data = entity, IsSuccess = true };

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = true
                };
            }
        }

        [HttpGet]
        [Route("api/qa/delete/feedback/{id}")]
        public async Task<DataResponse> DeleteFeedBack(int id)
        {
            try
            {
                var entity = context.QAFeedbacks.Single(q => q.Id == id);
                context.QAFeedbacks.Remove(entity);

                context.SaveChanges();

                return new DataResponse() { Data = entity, IsSuccess = true };

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = true
                };
            }
        }

        [HttpGet]
        [Route("api/qa/get/feedback/{entityid}/{type}")]
        public async Task<DataResponse> GetFeedBack(int entityid, int type)
        {
            try
            {
                var entity = context.ViewQaFeedbacks.Where(q => q.FormId == entityid && q.FormType == type).OrderByDescending(q => q.DateCreate).ToList();
                return new DataResponse()
                {
                    Data = entity,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = true
                };
            }


        }

        [HttpGet]
        [Route("api/get/csr/flightphase")]
        public async Task<DataResponse> CSRFlightPhase()
        {

            //var result = context.QAOptions.Where(q => q.ParentId == 7);
            var query = from x in context.QAOptions
                        where x.ParentId == 7
                        select new
                        {
                            Title = x.Title,
                            Id = x.Id
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/csr/eventtitle")]
        public async Task<DataResponse> CSREventTitle()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 16
                        select new
                        {
                            Title = x.Title,
                            Id = x.Id
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/csr/reporter")]
        public async Task<DataResponse> CSRReporter()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 138
                        select new
                        {
                            Title = x.Title,
                            Id = x.Id
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/gia/dmgby")]
        public async Task<DataResponse> GIADamageBy()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 68
                        select new
                        {
                            Title = x.Title,
                            Id = x.Id
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/gia/lighting")]
        public async Task<DataResponse> GetGAILighting()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 62
                        select new
                        {
                            Title = x.Title,
                            Id = x.Id
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/gia/surface")]
        public async Task<DataResponse> GetGIASurface()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 54
                        select new
                        {
                            Title = x.Title,
                            Id = x.Id
                        };

            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/gia/weather")]
        public async Task<DataResponse> GetGIAWeather()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 48
                        select new
                        {
                            Title = x.Title,
                            Id = x.Id
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/mor/compnspec")]
        public async Task<DataResponse> GetMORComponentSpec()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 43
                        select new
                        {
                            x.Id,
                            x.Title
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/flightinformation/{flightId}")]
        public async Task<DataResponse> GetFlightInformation(int flightId)
        {

            var result = context.AppLegs.SingleOrDefault(q => q.FlightId == flightId);
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }


        [HttpGet]
        [Route("api/get/csr/{FlightId}/{EmployeeId}")]
        public async Task<DataResponse> GetCSRByFlightId(int FlightId, int EmployeeId)
        {
            //var result = context.ViewQACSRs.SingleOrDefault(q => q.FlightId == FlightId);
            try
            {
                var result = context.QACSRGet(EmployeeId, FlightId).Single();
                var csrEvent = context.ViewQACSREvents.Where(q => q.QACSRId == result.Id).ToList();
                return new DataResponse()
                {
                    Data = new { result = result, CSREvent = csrEvent },
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = true
                };
            }

        }

        [HttpGet]
        [Route("api/get/csr/byid/{Id}")]
        public async Task<DataResponse> GetCSRById(int Id)
        {
            try
            {
                var result = context.ViewQACSRs.SingleOrDefault(q => q.Id == Id);
                var csrEvent = context.ViewQACSREvents.Where(q => q.QACSRId == result.Id).ToList();
                return new DataResponse()
                {
                    Data = new { result = result, CSREvent = csrEvent },
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


        [HttpGet]
        [Route("api/get/mor/{employeeId}/{flightId}")]
        public async Task<DataResponse> GetMORByFlightId(int employeeId, int flightId)
        {
            try
            {
                var result = context.QAMaintenanceGet(employeeId, flightId).Single();
                return new DataResponse()
                {
                    Data = result,
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

        [HttpGet]
        [Route("api/get/mor/byid/{Id}")]
        public async Task<DataResponse> GetMORById(int Id)
        {
            var result = context.ViewQAMaintenances.SingleOrDefault(q => q.Id == Id);
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/gia/{employeeId}/{flightId}")]
        public async Task<DataResponse> GetGIAByFlightId(int employeeId, int flightId)
        {
            var result = context.QAGroundGet(employeeId, flightId).Single();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/gia/byid/{Id}")]
        public async Task<DataResponse> GetGIAById(int Id)
        {
            var result = context.ViewQAGrounds.SingleOrDefault(q => q.Id == Id);
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/vhr/{Id}")]
        public async Task<DataResponse> GetVHRById(int Id)
        {
            try
            {
                var result = context.ViewQAHazards.SingleOrDefault(q => q.Id == Id);
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }

        DateTime ConvertToDateTime(string str)
        {
            var prts = str.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var dt = new DateTime(prts[0], prts[1], prts[2], prts[3], prts[4], 0);
            return dt;
        }

        TimeSpan ConvertToTime(string str)
        {
            var prts = str.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var dt = new TimeSpan(prts[0], prts[1], 0);
            return dt;
        }

        TimeSpan ConvertToTimeSpan(string str)
        {
            var parts = str.Split(':').Select(q => Convert.ToInt32(q)).ToList();
            var ts = new TimeSpan(parts[0], parts[1], 0);
            return ts;
        }


        [HttpPost]
        [Route("api/save/csr")]
        public async Task<DataResponse> SaveQACSR(dynamic dto)
        {

            try
            {
                int Id = dto.Id;
                string Date = dto.DateOccurrenceStr.ToString();
                var entity = context.QACSRs.SingleOrDefault(q => q.Id == Id);
                if (entity == null)
                {
                    entity = new QACSR();
                    context.QACSRs.Add(entity);
                }
                else
                {
                    var exist_events = context.QACSREvents.Where(q => q.QACSRId == Id).ToList();
                    context.QACSREvents.RemoveRange(exist_events);
                }



                entity.FlightPhaseId = dto.FlightPhaseId;
                entity.FlightId = dto.FlightId;
                entity.Describtion = dto.Describtion;
                entity.EventLocation = dto.EventLocation;
                entity.ReportFiledBy = dto.ReportFiledBy;
                entity.DateOccurrence = ConvertToDateTime(Date); //DateTime.Parse(Date) ;
                entity.Recommendation = dto.Recommendation;
                entity.WeatherCondition = dto.WeatherCondition;
                entity.Name = dto.Name;
                entity.BOX = dto.BOX;
                entity.RefNumber = dto.RefNumber;
                entity.FollowUp = dto.FollowUp;
                entity.Recived = dto.Recived;
                entity.EmployeeId = dto.EmployeeId;
                entity.EventTitleRemark = dto.EventTitleRemark;
                entity.Status = dto.Status;
                entity.StatusEmployeeId = dto.StatusEmployeeId;
                entity.DateStatus = dto.DateStatus;
                entity.ReporterId = dto.ReporterId;
                entity.DelayReason = dto.DelayReason;
                entity.Delay = dto.Delay;
                //entity.DateSign = dto.DateSign;
                if (dto.Signed != null)
                    entity.DateSign = DateTime.Now;
                entity.DateCreation = Id == -1 ? DateTime.Now : entity.DateCreation;
                foreach (var x in dto.EventTitleIds)
                {
                    entity.QACSREvents.Add(new QACSREvent()
                    {
                        EventTitleId = x
                    });
                };

                context.SaveChanges();

                if (dto.files != null)
                {
                    foreach (var f in dto.files)
                    {
                        int AttachmentId = f.AttachmentId;
                        var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                        if (file == null)
                        {
                            file = new QAAttachment();
                            context.QAAttachments.Add(file);
                        }
                        file.EntityId = entity.Id;
                        file.EmployeeId = dto.EmployeeId;
                        file.URL = HttpContext.Current.Server.MapPath("~/upload/" + f.FileName);
                        file.Lable = f.FileName;
                        file.DateAttach = DateTime.Now;
                        file.AttachmentType = f.FileType;
                        file.Description = f.Description;
                        file.Type = 0;
                    }

                }

                context.SaveChanges();
                dto.Id = entity.Id;
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }

        [HttpPost]
        [Route("api/save/mor")]
        public async Task<DataResponse> SaveMOR(dynamic dto)
        {
            try
            {

                int Id = dto.Id;
                string Date = dto.DateOccurrenceStr.ToString();
                var entity = context.QAMaintenances.SingleOrDefault(q => q.Id == Id);
                if (entity == null)
                {
                    entity = new QAMaintenance();
                    context.QAMaintenances.Add(entity);
                }

                entity.DateOccurrence = ConvertToDateTime(Date);
                entity.ComponentSpecificationId = dto.ComponentSpecificationId;
                entity.ATLNo = dto.ATLNo;
                entity.TaskNo = dto.TaskNo;
                entity.Reference = dto.Reference;
                entity.FlightId = dto.FlightId;
                entity.EmployeeId = dto.EmployeeId;
                entity.StationId = dto.StationId;
                entity.EventDescription = dto.EventDescription;
                entity.ActionTakenDescription = dto.ActionTakenDescription;
                entity.Name = dto.EmployeeName;
                entity.CAALicenceNo = dto.CAALicenceNo;
                entity.AuthorizationNo = dto.AuthorizationNo;
                entity.SerialNumber = dto.SerialNumber;
                entity.PartNumber = dto.PartNumber;
                entity.Status = dto.Status;
                entity.StatusEmployeeId = dto.StatusEmployeeId;
                entity.DateStatus = dto.DateStatus;
                entity.DelayReason = dto.DelayReason;
                entity.Delay = dto.Delay;
                //entity.DateSign = dto.DateSign;
                if (dto.Signed != null)
                    entity.DateSign = DateTime.Now;
                entity.DateCreation = Id == -1 ? DateTime.Now : entity.DateCreation;

                context.SaveChanges();

                if (dto.files != null)
                {
                    foreach (var f in dto.files)
                    {

                        int AttachmentId = f.AttachmentId;
                        var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                        if (file == null)
                        {
                            file = new QAAttachment();
                            context.QAAttachments.Add(file);
                        }
                        file.EntityId = entity.Id;
                        file.EmployeeId = dto.EmployeeId;
                        file.URL = HttpContext.Current.Server.MapPath("~/upload/" + f.FileName);
                        file.Lable = f.FileName;
                        file.DateAttach = DateTime.Now;
                        file.AttachmentType = f.FileType;
                        file.Description = f.Description;
                        file.Type = 3;
                    }

                }


                context.SaveChanges();
                dto.Id = entity.Id;
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
                    Data = ex.InnerException,
                    IsSuccess = false
                };
            }

        }

        [HttpPost]
        [Route("api/save/cyber")]
        public async Task<DataResponse> SaveQACyber(dynamic dto)
        {

            try
            {
                int Id = dto.Id;
                string Date = dto.DateOccurrenceStr.ToString();
                var entity = context.QACybers.SingleOrDefault(q => q.Id == Id);
                if (entity == null)
                {
                    entity = new QACyber();
                    context.QACybers.Add(entity);
                }



                entity.AccessDescription = dto.AccessDescription;
                entity.AccessId = dto.AccessId;
                entity.AccessMeasure = dto.AccessMeasure;
                entity.AttackDescriptipn = dto.AttackDescriptipn;
                entity.BreachedDescription = dto.BreachedDescription;
                entity.ContactInfo = dto.ContactInfo;
                entity.DateIncident = dto.DateIncident;
                entity.ImpactDescription = dto.ImpactDescription;
                entity.IncidentDescription = dto.IncidentDescription;
                entity.IncidentId = dto.IncidentId;
                entity.MethodDescription = dto.MethodDescription;
                entity.MethodId = dto.MethodId;
                entity.Other = dto.Other;
                entity.Result = dto.Result;
                entity.FlightId = dto.FlightId;
                entity.JobTitle = dto.JobTitle;
                entity.DateOccurrence = ConvertToDateTime(Date); //DateTime.Parse(Date) ;
                entity.Name = dto.EmployeeName;
                entity.Email = dto.Email;
                entity.Mobile = dto.Mobile;
                entity.EmployeeId = dto.EmployeeId;
                entity.Status = dto.Status;
                entity.StatusEmployeeId = dto.StatusEmployeeId;
                entity.DateStatus = dto.DateStatus;
                entity.DelayReason = dto.DelayReason;
                entity.Delay = dto.Delay;
                if (dto.Signed != null)
                    entity.DateSign = DateTime.Now;
                entity.DateCreation = Id == -1 ? DateTime.Now : entity.DateCreation;

                context.SaveChanges();

                if (dto.files != null)
                {
                    foreach (var f in dto.files)
                    {
                        int AttachmentId = f.AttachmentId;
                        var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                        if (file == null)
                        {
                            file = new QAAttachment();
                            context.QAAttachments.Add(file);
                        }
                        file.EntityId = entity.Id;
                        file.EmployeeId = dto.EmployeeId;
                        file.URL = HttpContext.Current.Server.MapPath("~/upload/" + f.FileName);
                        file.Lable = f.FileName;
                        file.DateAttach = DateTime.Now;
                        file.AttachmentType = f.FileType;
                        file.Description = f.Description;
                        file.Type = 7;
                    }

                }

                context.SaveChanges();
                dto.Id = entity.Id;
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }

        [HttpGet]
        [Route("api/get/cyber/{employeeId}/{flightId}")]
        public async Task<DataResponse> GetCyberByFlightId(int employeeId, int flightId)
        {
            try
            {
                var result = context.QACyberGet(employeeId, flightId).Single();

                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = true
                };
            }
        }

        [HttpGet]
        [Route("api/get/cyber/byid/{Id}")]
        public async Task<DataResponse> GetCyberById(int Id)
        {
            var result = context.ViewQACybers.SingleOrDefault(q => q.Id == Id);
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/cyber/incident")]
        public async Task<DataResponse> GetCyberIncident()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 120
                        select new
                        {
                            x.Id,
                            x.Title
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }


        [HttpGet]
        [Route("api/get/cyber/accessibility")]
        public async Task<DataResponse> GetCyberAccessibility()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 128
                        select new
                        {
                            x.Id,
                            x.Title
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/cyber/method")]
        public async Task<DataResponse> GetCyberMethod()
        {
            var query = from x in context.QAOptions
                        where x.ParentId == 133
                        select new
                        {
                            x.Id,
                            x.Title
                        };
            return new DataResponse()
            {
                Data = query,
                IsSuccess = true
            };
        }


        [HttpPost]
        [Route("api/save/vhr")]
        public async Task<DataResponse> SaveVHR(dynamic dto)
        {

            try
            {
                int Id = dto.Id;
                string Data = dto.DateOccurrenceStr.ToString();
                var entity = context.QAHazards.SingleOrDefault(q => q.Id == Id);
                if (entity == null)
                {
                    entity = new QAHazard();
                    context.QAHazards.Add(entity);
                }
                entity.Name = dto.EmployeeName;
                entity.Email = dto.Email;
                entity.TelNumber = dto.TelNumber;
                entity.DateOccurrence = dto.DateOccurrence;
                entity.AffectedArea = dto.AffectedArea;
                entity.DateReport = dto.DateReport;
                entity.HazardDescription = dto.HazardDescription;
                entity.RecommendedAction = dto.RecommendedAction;
                entity.EmployeeId = dto.EmployeeId;
                entity.Status = dto.Status;
                entity.StatusEmployeeId = dto.StatusEmployeeId;
                entity.DateStatus = dto.DateStatus;
                entity.RelatedDepartment = dto.RelatedDepartment;
                entity.DelayReason = dto.DelayReason;
                entity.Delay = dto.Delay;
                if (dto.Signed != null)
                    entity.DateSign = DateTime.Now;
                entity.DateCreation = Id == -1 ? DateTime.Now : entity.DateCreation;

                context.SaveChanges();

                if (dto.files != null)
                {
                    foreach (var f in dto.files)
                    {

                        int AttachmentId = f.AttachmentId;
                        var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                        if (file == null)
                        {
                            file = new QAAttachment();
                            context.QAAttachments.Add(file);
                        }
                        file.EntityId = entity.Id;
                        file.EmployeeId = dto.EmployeeId;
                        file.URL = HttpContext.Current.Server.MapPath("~/upload/" + f.FileName);
                        file.Lable = f.FileName;
                        file.DateAttach = DateTime.Now;
                        file.AttachmentType = f.FileType;
                        file.Description = f.Description;
                        file.Type = 2;
                    }

                }

                context.SaveChanges();
                dto.Id = entity.Id;
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = true
                };

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }



        }

        [HttpPost]
        [Route("api/save/gia")]
        public async Task<DataResponse> SaveGIA(dynamic dto)
        {

            try
            {
                int Id = dto.Id;
                string Date = dto.DateOccurrenceStr.ToString();
                string FlightDelay = dto.FlightDelay.ToString();
                string ScheduleGroundTime = dto.ScheduledGroundTime.ToString();
                var entity = context.QAGroundIADs.SingleOrDefault(q => q.Id == Id);
                if (entity == null)
                {
                    entity = new QAGroundIAD();
                    context.QAGroundIADs.Add(entity);
                }
                entity.Airport = dto.Airport;
                entity.AirportId = dto.AirportId;
                entity.Title = dto.Title;
                entity.Area = dto.Area;
                entity.ContributoryFactors = dto.ContributoryFactors;
                entity.CorrectiveActionTaken = dto.CorrectiveActionTaken;
                entity.DamageById = dto.DamageById;
                entity.DateOccurrence = ConvertToDateTime(Date);
                entity.DamageDetails = dto.DamageDetails;
                entity.EmployeeId = dto.EmployeeId;
                entity.CorrectiveActionTaken = dto.CorrectiveActionTaken;
                entity.EmployeesNonFatalityNr = dto.EmployeesNonFatalityNr;
                entity.EmployeesFatalityNr = dto.EmployeesFatalityNr;
                entity.Event = dto.Event;
                entity.FlightId = dto.FlightId;
                entity.OperationPhase = dto.OperationPhase;
                entity.OthersFatalityNr = dto.OthersFatalityNr;
                entity.OthersNonFatalityNr = dto.OthersNonFatalityNr;
                entity.OtherSuggestions = dto.OtherSuggestions;
                entity.PassengersFatalityNr = dto.PassengersFatalityNr;
                entity.PassengersNonFatalityNr = dto.PassengersNonFatalityNr;
                entity.PersonnelCompany1 = dto.PersonnelCompany1;
                entity.PersonnelJobTitle1 = dto.PersonnelJobTitle1;
                entity.PersonnelLicense1 = dto.PersonnelLicense1;
                entity.PersonnelName1 = dto.PersonnelName1;
                entity.PersonnelStaffNr1 = dto.PersonnelStaffNr1;
                entity.PersonnelCompany2 = dto.PersonnelCompany2;
                entity.PersonnelJobTitle2 = dto.PersonnelJobTitle2;
                entity.PersonnelLicense2 = dto.PersonnelLicense2;
                entity.PersonnelName2 = dto.PersonnelName2;
                entity.PersonnelStaffNr2 = dto.PersonnelStaffNr2;
                entity.PersonnelCompany3 = dto.PersonnelCompany3;
                entity.PersonnelJobTitle3 = dto.PersonnelJobTitle3;
                entity.PersonnelLicense3 = dto.PersonnelLicense3;
                entity.PersonnelName3 = dto.PersonnelName3;
                entity.PersonnelStaffNr3 = dto.PersonnelStaffNr3;
                entity.ScheduledGroundTime = ConvertToTimeSpan(ScheduleGroundTime);
                entity.FlightDelay = ConvertToTimeSpan(FlightDelay);
                entity.VEAge = dto.VEAge;
                entity.VEArea = dto.VEArea;
                entity.VEBrakesCon = dto.VEBrakesCon;
                entity.VEFieldofVisionCon = dto.VEFieldofVisionCon;
                entity.VEFromDrivingPoCon = dto.VEFromDrivingPoCon;
                entity.VELastOverhaul = dto.VELastOverhaul;
                entity.VELightsCon = dto.VELightsCon;
                entity.VEOwner = dto.VEOwner;
                entity.VEProtectionCon = dto.VEProtectionCon;
                entity.VERemarks = dto.VERemarks;
                entity.VESerialFleetNr = dto.VESerialFleetNr;
                entity.VEStabilizersCon = dto.VEStabilizersCon;
                entity.VESteeringCon = dto.VESteeringCon;
                entity.VETowHitchCon = dto.VETowHitchCon;
                entity.VEType = dto.VEType;
                entity.VETyresCon = dto.VETyresCon;
                entity.VEWarningDevicesCon = dto.VEWarningDevicesCon;
                entity.VEWipersCon = dto.VEWipersCon;
                entity.WXLightingId = dto.WXLightingId;
                entity.WXSurfaceId = dto.WXSurfaceId;
                entity.WXTemperature = dto.WXTemperature;
                entity.WXVisibilityM = dto.WXVisibilityM;
                entity.WXVisibilityKM = dto.WXVisibilityKM;
                entity.WXWeatherId = dto.WXWeatherId;
                entity.WXWind = dto.WXWind;
                entity.DamageRemark = dto.DamageRemark;
                entity.Status = dto.Status;
                entity.StatusEmployeeId = dto.StatusEmployeeId;
                entity.DateStatus = dto.DateStatus;
                entity.DateCreation = Id == -1 ? DateTime.Now : entity.DateCreation;
                entity.DelayReason = dto.DelayReason;
                entity.Delay = dto.Delay;
                //entity.DateSign = dto.DateSign;
                if (dto.Signed != null)
                    entity.DateSign = DateTime.Now;

                context.SaveChanges();

                if (dto.files != null)
                {
                    foreach (var f in dto.files)
                    {

                        int AttachmentId = f.AttachmentId;
                        var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                        if (file == null)
                        {
                            file = new QAAttachment();
                            context.QAAttachments.Add(file);
                        }
                        file.EntityId = entity.Id;
                        file.EmployeeId = dto.EmployeeId;
                        file.URL = HttpContext.Current.Server.MapPath("~/upload/" + f.FileName);
                        file.Lable = f.FileName;
                        file.DateAttach = DateTime.Now;
                        file.AttachmentType = f.FileType;
                        file.Description = f.Description;
                        file.Type = 1;
                    }

                }

                context.SaveChanges();
                dto.Id = entity.Id;
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = true
                };

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }


        [HttpGet]
        [Route("api/get/chr/reason")]
        public async Task<DataResponse> GetCHRReason()
        {
            var result = from x in context.QAOptions
                         where x.ParentId == 78
                         select new
                         {
                             Title = x.Title,
                             Id = x.Id
                         };

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/chr/{employeeId}/{flightId}")]
        public async Task<DataResponse> GetCHRByFlightId(int employeeId, int flightId)
        {
            try
            {
                var result = context.QACateringGet(employeeId, flightId).Single();
                return new DataResponse()
                {
                    Data = result,
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

        [HttpGet]
        [Route("api/get/chr/byid/{id}")]
        public async Task<DataResponse> GetCHRById(int id)
        {
            var result = context.ViewQACaterings.SingleOrDefault(q => q.Id == id);
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpPost]
        [Route("api/save/chr")]
        public async Task<DataResponse> SaveCHR(dynamic dto)
        {
            try
            {

                int Id = dto.Id;
                string Date = dto.DateOccurrenceStr.ToString();
                var entity = context.QACaterings.SingleOrDefault(q => q.Id == Id);
                if (entity == null)
                {
                    entity = new QACatering();
                    context.QACaterings.Add(entity);
                }


                entity.DateReport = dto.DateReport;
                entity.DateOccurrence = ConvertToDateTime(Date);
                entity.Description = dto.Description;
                entity.Equipment = dto.Equipment;
                entity.EmployeeId = dto.EmployeeId;
                entity.InjuryDescription = dto.InjuryDescription;
                entity.InjeryOccurring = dto.InjeryOccurring;
                entity.Place = dto.Place;
                entity.PreventiveActions = dto.PreventiveActions;
                entity.ReasonDescription = dto.ReasonDescription;
                entity.ReasonId = dto.ReasonId;
                entity.SaftyEquipmentType = dto.SaftyEquipmentType;
                entity.SaftyEquipmentUseage = dto.SaftyEquipmentUseage;
                entity.Transporter = dto.Transporter;
                entity.Trolley = dto.Trolley;
                entity.TrolleyEquipmentTransporterDecs = dto.TrolleyEquipmentTransporterDecs;
                entity.WorkBreak = dto.WorkBreak;
                entity.WorkBreakPeriod = dto.WorkBreakPeriod;
                entity.FlightId = dto.FlightId;
                entity.Status = dto.Status;
                entity.StatusEmployeeId = dto.StatusEmployeeId;
                entity.DateStatus = dto.DateStatus;
                entity.DelayReason = dto.DelayReason;
                entity.Delay = dto.Delay;
                //entity.DateSign = dto.DateSign;
                if (dto.Signed != null)
                    entity.DateSign = DateTime.Now;
                entity.DateCreation = Id == -1 ? DateTime.Now : entity.DateCreation;
                entity.Name = dto.EmployeeName;
                entity.Email = dto.Email;
                entity.TelNumber = dto.TelNumber;

                context.SaveChanges();

                if (dto.files != null)
                {
                    foreach (var f in dto.files)
                    {

                        int AttachmentId = f.AttachmentId;
                        var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                        if (file == null)
                        {
                            file = new QAAttachment();
                            context.QAAttachments.Add(file);
                        }
                        file.EntityId = entity.Id;
                        file.EmployeeId = dto.EmployeeId;
                        file.URL = HttpContext.Current.Server.MapPath("~/upload/" + f.FileName);
                        file.Lable = f.FileName;
                        file.DateAttach = DateTime.Now;
                        file.AttachmentType = f.FileType;
                        file.Description = f.Description;
                        file.Type = 4;
                    }

                }


                var saveChanges = await context.SaveChangesAsync();
                dto.Id = entity.Id;
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }



        [HttpGet]
        [Route("api/get/shr/reason")]
        public async Task<DataResponse> GetSHRReason()
        {
            var result = from x in context.QAOptions
                         where x.ParentId == 92
                         select new
                         {
                             Title = x.Title,
                             Id = x.Id
                         };

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/shr/{employeeId}/{flightId}")]
        public async Task<DataResponse> GetSHRByFlightId(int employeeId, int flightId)
        {
            var result = context.QASecurityGet(employeeId, flightId).Single();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/shr/byid/{id}")]
        public async Task<DataResponse> GetSHRById(int id)
        {
            var result = context.ViewQASecurities.SingleOrDefault(q => q.Id == id);
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpPost]
        [Route("api/save/shr")]
        public async Task<DataResponse> SaveSHR(dynamic dto)
        {
            try
            {

                int Id = dto.Id;
                string Date = dto.DateOccurrenceStr.ToString();
                var entity = context.QASecurities.SingleOrDefault(q => q.Id == Id);
                if (entity == null)
                {
                    entity = new QASecurity();
                    context.QASecurities.Add(entity);
                }

                entity.DateReport = dto.DateReport;
                entity.DateSign = dto.DateSign;
                entity.DateOccurrence = ConvertToDateTime(Date);
                entity.Description = dto.Description;
                entity.Camera = dto.Camera;
                entity.CarryingBox = dto.CarryingBox;
                entity.InjuryDescription = dto.InjuryDescription;
                entity.InjuryOccuring = dto.InjuryOccuring;
                entity.Place = dto.Place;
                entity.PreventiveActions = dto.PreventiveActions;
                entity.ReasonDescription = dto.ReasonDescription;
                entity.ReasonId = dto.ReasonId;
                entity.EquipmentDescription = dto.EquipmentDescription;
                entity.WorkBreak = dto.WorkBreak;
                entity.WorkBreakPeriod = dto.WorkBreakPeriod;
                entity.FlightId = dto.FlightId;
                entity.Comail = dto.Comail;
                entity.HandRocket = dto.HandRocket;
                entity.InjuryOccuring = dto.InjuryOccuring;
                entity.Other = dto.Other;
                entity.EmployeeId = dto.EmployeeId;
                entity.Status = dto.Status;
                entity.StatusEmployeeId = dto.StatusEmployeeId;
                entity.DateStatus = dto.DateStatus;
                entity.DelayReason = dto.DelayReason;
                entity.Delay = dto.Delay;
                //entity.DateSign = dto.DateSign;
                if (dto.Signed != null)
                    entity.DateSign = DateTime.Now;
                entity.DateCreation = Id == -1 ? DateTime.Now : entity.DateCreation;
                entity.Email = dto.Email;
                entity.Name = dto.EmployeeName;
                entity.TelNumber = dto.TelNumber;

                context.SaveChanges();

                if (dto.files != null)
                {
                    foreach (var f in dto.files)
                    {

                        int AttachmentId = f.AttachmentId;
                        var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                        if (file == null)
                        {
                            file = new QAAttachment();
                            context.QAAttachments.Add(file);
                        }
                        file.EntityId = entity.Id;
                        file.EmployeeId = dto.EmployeeId;
                        file.URL = HttpContext.Current.Server.MapPath("~/upload/" + f.FileName);
                        file.Lable = f.FileName;
                        file.DateAttach = DateTime.Now;
                        file.AttachmentType = f.FileType;
                        file.Description = f.Description;
                        file.Type = 5;
                    }

                }


                var saveChanges = await context.SaveChangesAsync();
                dto.Id = entity.Id;
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
                    Data = ex.InnerException,
                    IsSuccess = false
                };
            }
        }







        [HttpGet]
        [Route("api/get/opcatagory")]
        public async Task<DataResponse> GetDispatchOPCatagory()
        {
            var result = from x in context.QAOptions
                         where x.ParentId == 104
                         select new
                         {
                             Title = x.Title,
                             Id = x.Id
                         };

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/discatagory")]
        public async Task<DataResponse> GetDispatchDISCatagory()
        {
            var result = from x in context.QAOptions
                         where x.ParentId == 112
                         select new
                         {
                             Title = x.Title,
                             Id = x.Id
                         };

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }


        [HttpGet]
        [Route("api/get/dhr/{employeeId}/{flightId}")]
        public async Task<DataResponse> GetDHRByFlightId(int employeeId, int flightId)
        {
            try
            {
                var result = context.QADispatchGet(employeeId, flightId).Single();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse() { Data = ex, IsSuccess = false };

            }
        }

        [HttpGet]
        [Route("api/get/dhr/byid/{id}")]
        public async Task<DataResponse> GetDHRById(int id)
        {
            var result = context.ViewQADispatches.SingleOrDefault(q => q.Id == id);
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true
            };
        }

        [HttpPost]
        [Route("api/save/dhr")]
        public async Task<DataResponse> SaveDHR(dynamic dto)
        {
            try
            {

                int Id = dto.Id;
                string Date = dto.DateOccurrenceStr.ToString();
                var entity = context.QADispatches.SingleOrDefault(q => q.Id == Id);
                if (entity == null)
                {
                    entity = new QADispatch();
                    context.QADispatches.Add(entity);
                }

                entity.DateReport = dto.DateReport;
                entity.DISActionResult = dto.DISActionResult;
                entity.JobPosition = dto.JobPosition;
                entity.DisCatagoryId = dto.DisCatagoryId;
                entity.OpCatagoryId = dto.OpCatagoryId;
                entity.DateOccurrence = ConvertToDateTime(Date);
                entity.DISLocation = dto.DISLocation;
                entity.DISTimeDuration = dto.DISTimeDuration;
                entity.FlightId = dto.FlightId;
                entity.OPLocation = dto.OPLocation;
                entity.OPReportedBy = dto.OPReportedBy;
                entity.OPSummary = dto.OPSummary;
                entity.OPTimeReceived = dto.OPTimeReceived;
                entity.Remarks = dto.Remarks;
                entity.Type = dto.Type;
                entity.EmployeeId = dto.EmployeeId;
                entity.Status = dto.Status;
                entity.StatusEmployeeId = dto.StatusEmployeeId;
                entity.DateStatus = dto.DateStatus;
                entity.ReporterName = dto.ReporterName;
                entity.ReporterPosition = dto.ReporterPosition;
                entity.ACChanged = dto.ACChanged;
                if (dto.ACChangedTimeStr != "Invalid date")
                    entity.ACChangedTime = ConvertToTimeSpan(dto.ACChangedTimeStr.ToString());
                entity.CrewChanged = dto.CrewChanged;
                if (dto.CrewChangedTimeStr != "Invalid date")
                    entity.CrewChangedTime = ConvertToTimeSpan(dto.CrewChangedTimeStr.ToString());
                entity.FlightCancelled = dto.FlightCancelled;
                if (dto.FlightCancelledTimeStr != "Invalid date")
                    entity.FlightCancelledTime = ConvertToTimeSpan(dto.FlightCancelledTimeStr.ToString());
                entity.FlightPerFormed = dto.FlightPerFormed;
                if (dto.FlightPerFormedTimeStr != "Invalid date")
                    entity.FlightPerFormedTime = ConvertToTimeSpan(dto.FlightPerFormedTimeStr.ToString());
                entity.PIC = dto.PIC;
                entity.DelayReason = dto.DelayReason;
                entity.Delay = dto.Delay;
                if (dto.Signed != null)
                    entity.DateSign = DateTime.Now;
                entity.DateCreation = Id == -1 ? DateTime.Now : entity.DateCreation;
                entity.Name = dto.EmployeeName;
                entity.Email = dto.Email;
                entity.TelNumber = dto.TelNumber;

                context.SaveChanges();

                if (dto.files != null)
                {
                    foreach (var f in dto.files)
                    {

                        int AttachmentId = f.AttachmentId;
                        var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                        if (file == null)
                        {
                            file = new QAAttachment();
                            context.QAAttachments.Add(file);
                        }
                        file.EntityId = entity.Id;
                        file.EmployeeId = dto.EmployeeId;
                        file.URL = HttpContext.Current.Server.MapPath("~/upload/" + f.FileName);
                        file.Lable = f.FileName;
                        file.DateAttach = DateTime.Now;
                        file.AttachmentType = f.FileType;
                        file.Description = f.Description;
                        file.Type = 6;
                    }

                }

                var saveChanges = await context.SaveChangesAsync();
                dto.Id = entity.Id;
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
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

        [HttpGet]
        [Route("api/get/qa/{employeeId}")]
        public async Task<DataResponse> GetQAByEmployee(int employeeId)
        {

            try
            {

                var query = from e in context.ViewQAByEmployeeCounts
                            where e.EmployeeId == employeeId
                            group e by new { e.TypeTitle, e.type } into grp
                            select new
                            {
                                grp.Key.TypeTitle,
                                grp.Key.type,
                                NewCount = grp.Sum(q => q.NewCount),
                                OpenCount = grp.Sum(q => q.InProgressCount),
                                DeterminedCount = grp.Sum(q => q.ClosedCount)
                            };

                var result = query.ToList();
                //var result = new
                //{
                //    Type = ds.Single(q => q.Type),
                //    Status = ds.Count(q => q.Status),
                //};
                return new DataResponse()
                {
                    Data = result,
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


        //[HttpPost]
        //[Route("api/get/qa/status")]
        //public async Task<DataResponse> GetQAStatus(dynamic dto)
        //{
        //    try
        //    {
        //        dynamic result = null;
        //        DateTime df = dto.dt_from;
        //        DateTime dt = dto.dt_to;
        //        int employeeId = dto.employeeId;
        //        int type = dto.type;
        //        //var result = new { };
        //        //var result = new { Confirmed, };
        //        switch (type)
        //        {
        //            case 0:
        //                var q0 = (from byEmp in context.ViewQABYEmployees
        //                          join cabin in context.ViewQACSRs on byEmp.EntityId equals cabin.Id
        //                          where byEmp.EmployeeId == employeeId && cabin.OccurrenceDateTime >= df && cabin.OccurrenceDateTime <= dt && byEmp.Type == 0
        //                          select new
        //                          {
        //                              Category = byEmp.Category,
        //                              Date = cabin.dateStatus,
        //                              EmployeeName = cabin.EmployeeName,
        //                              EmployeeId = byEmp.EmployeeId,
        //                              Id = cabin.Id,
        //                              Status = byEmp.DeterminedType,
        //                              StatusTitle = byEmp.DeterminedTitle,
        //                              Type = byEmp.Type
        //                          }).ToList();

        //                var ds = (from x in q0
        //                          join refer in context.ViewQAReferringCounts on new { Id = (int?)x.Id, EmployeeId = (int?)x.EmployeeId } equals new { Id = (int?)refer.EntityId, EmployeeId = (int?)refer.EmployeeId } into referGroup
        //                          from matching in referGroup.DefaultIfEmpty()
        //                          select new
        //                          {
        //                              Category = x.Category,
        //                              Date = x.Date,
        //                              EmployeeName = x.EmployeeName,
        //                              Id = x.Id,
        //                              Status = x.Status,
        //                              StatusTitle = x.StatusTitle,
        //                              ReferCount = (matching == null) ? 0 : matching.ReferCount,
        //                              DeterminedCount = (matching == null) ? 0 : matching.DeterminedCount

        //                          }).ToList();
        //                ;
        //                result = new
        //                {
        //                    New = ds.Where(q => q.Category == "New"),
        //                    Determined = ds.Where(q => q.Category == "Determined"),
        //                    Open = ds.Where(q => q.Category == "Open"),
        //                };


        //                break;

        //            case 1:
        //                var q1 = (from byEmp in context.ViewQABYEmployees
        //                          join ground in context.ViewQAGrounds on byEmp.EntityId equals ground.Id
        //                          where byEmp.EmployeeId == employeeId && ground.DamageDate >= df && ground.DamageDate <= dt && byEmp.Type == 1
        //                          select new
        //                          {
        //                              Category = byEmp.Category,
        //                              Date = ground.DamageDate,
        //                              EmployeeName = ground.EmployeeName,
        //                              EmployeeId = byEmp.EmployeeId,
        //                              Id = ground.Id,
        //                              Status = byEmp.DeterminedType,
        //                              StatusTitle = byEmp.DeterminedTitle,
        //                              Type = byEmp.Type
        //                          }).ToList();
        //                //var ds1 = q1.ToList();

        //                var ds1 = (from x in q1
        //                           join refer in context.ViewQAReferringCounts on new { Id = (int?)x.Id, EmployeeId = (int?)x.EmployeeId } equals new { Id = (int?)refer.EntityId, EmployeeId = (int?)refer.EmployeeId } into referGroup
        //                           from matching in referGroup.DefaultIfEmpty()
        //                           select new
        //                           {
        //                               Category = x.Category,
        //                               Date = x.Date,
        //                               EmployeeName = x.EmployeeName,
        //                               Id = x.Id,
        //                               Status = x.Status,
        //                               StatusTitle = x.StatusTitle,
        //                               ReferCount = (matching == null) ? 0 : matching.ReferCount,
        //                               DeterminedCount = (matching == null) ? 0 : matching.DeterminedCount

        //                           }).ToList();


        //                result = new
        //                {
        //                    New = ds1.Where(q => q.Category == "New"),
        //                    Determined = ds1.Where(q => q.Category == "Determined"),
        //                    Open = ds1.Where(q => q.Category == "Open"),
        //                };
        //                break;
        //            case 2:
        //                var q2 = (from byEmp in context.ViewQABYEmployees
        //                          join hazard in context.ViewQAHazards on byEmp.EntityId equals hazard.Id
        //                          where byEmp.EmployeeId == employeeId && hazard.HazardDate >= df && hazard.HazardDate <= dt && byEmp.Type == 2
        //                          select new
        //                          {
        //                              Category = byEmp.Category,
        //                              Date = hazard.dateStatus,
        //                              EmployeeName = hazard.EmployeeName,
        //                              EmployeeId = byEmp.EmployeeId,
        //                              Id = hazard.Id,
        //                              Status = byEmp.DeterminedType,
        //                              StatusTitle = byEmp.DeterminedTitle,
        //                              Type = byEmp.Type
        //                          }).ToList();
        //                var ds2 = (from x in q2
        //                                     join refer in context.ViewQAReferringCounts on new { Id = (int?)x.Id, EmployeeId = (int?)x.EmployeeId } equals new { Id = (int?)refer.EntityId, EmployeeId = (int?)refer.EmployeeId } into referGroup
        //                                     from matching in referGroup.DefaultIfEmpty()
        //                                     select new
        //                                     {
        //                                         Category = x.Category,
        //                                         Date = x.Date,
        //                                         EmployeeName = x.EmployeeName,
        //                                         Id = x.Id,
        //                                         Status = x.Status,
        //                                         StatusTitle = x.StatusTitle,
        //                                         ReferCount = (matching == null) ? 0 : matching.ReferCount,
        //                                         DeterminedCount = (matching == null) ? 0 : matching.DeterminedCount

        //                                     }).ToList();

        //                result = new
        //                {
        //                    New = ds2.Where(q => q.Category == "New"),
        //                    Determined = ds2.Where(q => q.Category == "Determined"),
        //                    Open = ds2.Where(q => q.Category == "Open"),
        //                };
        //                break;
        //            case 3:
        //                var q3 = (from byEmp in context.ViewQABYEmployees
        //                         join maintenance in context.ViewQAMaintenances on byEmp.EntityId equals maintenance.Id
        //                         where byEmp.EmployeeId == employeeId && maintenance.OccurrenceDateTime >= df && maintenance.OccurrenceDateTime <= dt && byEmp.Type == 3
        //                         select new
        //                         {
        //                             Category = byEmp.Category,
        //                             Date = maintenance.dateStatus,
        //                             EmployeeName = maintenance.EmployeeName,
        //                             EmployeeId = byEmp.EmployeeId,
        //                             Id = maintenance.Id,
        //                             Status = byEmp.DeterminedType,
        //                             StatusTitle = byEmp.DeterminedTitle,
        //                             Type = byEmp.Type
        //                         }).ToList();
        //                var ds3 = q3.ToList();
        //                result = new
        //                {
        //                    New = ds3.Where(q => q.Category == "New"),
        //                    Determined = ds3.Where(q => q.Category == "Determined"),
        //                    Open = ds3.Where(q => q.Category == "Open"),
        //                };
        //                break;
        //            case 4:
        //                var q4 = (from byEmp in context.ViewQABYEmployees
        //                         join catering in context.ViewQACaterings on byEmp.EntityId equals catering.Id
        //                         where byEmp.EmployeeId == employeeId && catering.DateHazard >= df && catering.DateHazard <= dt && byEmp.Type == 4
        //                         select new
        //                         {
        //                             Category = byEmp.Category,
        //                             Date = catering.dateStatus,
        //                             EmployeeName = catering.EmployeeName,
        //                             EmployeeId = byEmp.EmployeeId,
        //                             Id = catering.Id,
        //                             Status = byEmp.DeterminedType,
        //                             StatusTitle = byEmp.DeterminedTitle,
        //                             Type = byEmp.Type
        //                         }).ToList();
        //                var ds4 = (from x in q4
        //                           join refer in context.ViewQAReferringCounts on new { Id = (int?)x.Id, EmployeeId = (int?)x.EmployeeId } equals new { Id = (int?)refer.EntityId, EmployeeId = (int?)refer.EmployeeId } into referGroup
        //                           from matching in referGroup.DefaultIfEmpty()
        //                           select new
        //                           {
        //                               Category = x.Category,
        //                               Date = x.Date,
        //                               EmployeeName = x.EmployeeName,
        //                               Id = x.Id,
        //                               Status = x.Status,
        //                               StatusTitle = x.StatusTitle,
        //                               ReferCount = (matching == null) ? 0 : matching.ReferCount,
        //                               DeterminedCount = (matching == null) ? 0 : matching.DeterminedCount

        //                           }).ToList();

        //                result = new
        //                {
        //                    New = ds4.Where(q => q.Category == "New"),
        //                    Determined = ds4.Where(q => q.Category == "Determined"),
        //                    Open = ds4.Where(q => q.Category == "Open"),
        //                };
        //                break;
        //            case 5:
        //                var q5 = (from byEmp in context.ViewQABYEmployees
        //                         join security in context.ViewQASecurities on byEmp.EntityId equals security.Id
        //                         where byEmp.EmployeeId == employeeId && security.DateReport >= df && security.DateReport <= dt && byEmp.Type == 5
        //                         select new
        //                         {
        //                             Category = byEmp.Category,
        //                             Date = security.dateStatus,
        //                             EmployeeName = security.EmployeeName,
        //                             EmployeeId = byEmp.EmployeeId,
        //                             Id = security.Id,
        //                             Status = byEmp.DeterminedType,
        //                             StatusTitle = byEmp.DeterminedTitle,
        //                             Type = byEmp.Type
        //                         }).ToList();
        //                var ds5 = (from x in q5
        //                           join refer in context.ViewQAReferringCounts on new { Id = (int?)x.Id, EmployeeId = (int?)x.EmployeeId, Type = x.Type  } equals new { Id = (int?)refer.EntityId, EmployeeId = (int?)refer.EmployeeId, Type = refer.Type } into referGroup

        //                           from matching in referGroup.DefaultIfEmpty()
        //                           select new
        //                           {
        //                               Category = x.Category,
        //                               Date = x.Date,
        //                               EmployeeName = x.EmployeeName,
        //                               Id = x.Id,
        //                               Status = x.Status,
        //                               StatusTitle = x.StatusTitle,
        //                               ReferCount = (matching == null) ? 0 : matching.ReferCount,
        //                               DeterminedCount = (matching == null) ? 0 : matching.DeterminedCount

        //                           }).ToList();

        //                result = new
        //                {
        //                    New = ds5.Where(q => q.Category == "New"),
        //                    Determined = ds5.Where(q => q.Category == "Determined"),
        //                    Open = ds5.Where(q => q.Category == "Open"),
        //                };
        //                break;

        //            case 6:
        //                var q6 = (from byEmp in context.ViewQABYEmployees
        //                         join dispatch in context.ViewQADispatches on byEmp.EntityId equals dispatch.Id
        //                         where byEmp.EmployeeId == employeeId && dispatch.DateReport >= df && dispatch.DateReport <= dt && byEmp.Type == 6
        //                         select new
        //                         {
        //                             Category = byEmp.Category,
        //                             Date = dispatch.dateStatus,
        //                             EmployeeName = dispatch.EmployeeName,
        //                             EmployeeId = byEmp.EmployeeId,
        //                             Id = dispatch.Id,
        //                             Status = byEmp.DeterminedType,
        //                             StatusTitle = byEmp.DeterminedTitle,
        //                             Type = byEmp.Type
        //                         }).ToList();
        //                var ds6 = (from x in q6
        //                           join refer in context.ViewQAReferringCounts on new { Id = (int?)x.Id, EmployeeId = (int?)x.EmployeeId } equals new { Id = (int?)refer.EntityId, EmployeeId = (int?)refer.EmployeeId } into referGroup
        //                           from matching in referGroup.DefaultIfEmpty()
        //                           select new
        //                           {
        //                               Category = x.Category,
        //                               Date = x.Date,
        //                               EmployeeName = x.EmployeeName,
        //                               Id = x.Id,
        //                               Status = x.Status,
        //                               StatusTitle = x.StatusTitle,
        //                               ReferCount = (matching == null) ? 0 : matching.ReferCount,
        //                               DeterminedCount = (matching == null) ? 0 : matching.DeterminedCount

        //                           }).ToList();

        //                result = new
        //                {
        //                    New = ds6.Where(q => q.Category == "New"),
        //                    Determined = ds6.Where(q => q.Category == "Determined"),
        //                    Open = ds6.Where(q => q.Category == "Open"),
        //                };
        //                break;
        //        }



        //        //var entity = context.ViewQABYEmployees.Where(q => q.EmploeeId == employeeId).ToList();


        //        return new DataResponse()
        //        {
        //            Data = result,
        //            IsSuccess = true
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse()
        //        {
        //            Data = ex,
        //            IsSuccess = false
        //        };
        //    }
        //}

        [HttpPost]
        [Route("api/get/qa/status")]
        public async Task<DataResponse> GetQAStatus(dynamic dto)
        {
            try
            {
                var df = ((DateTime)dto.dt_from).Date;
                var dt = ((DateTime)dto.dt_to).Date.AddDays(1);
                var ds = context.QAGetEntities((int?)dto.employeeId, (int?)dto.type, df, dt).ToList().OrderBy(q =>q.DateSign).ThenBy(q => q.DeadLine);
                var result = new
                {
                    New = ds.Where(q => q.Category == "New"),
                    Determined = ds.Where(q => q.Category == "Closed"),
                    Open = ds.Where(q => q.Category == "InProgress"),
                };

                return new DataResponse()
                {
                    Data = result,
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


        ////[HttpGet]
        ////[Route("api/qa/confirm/report/{employeeId}/{type}/{entityId}/{status}")]
        ////public async Task<DataResponse> ConfirmReport(int employeeId, int type, int entityId, int status)
        ////{
        ////    try
        ////    {
        ////        var entity = new QAFollowingUp();
        ////        context.QAFollowingUps.Add(entity);
        ////        entity.Type = type;
        ////        entity.ReferredId = employeeId;
        ////        entity.ReferrerId = employeeId;
        ////        entity.Status = status;
        ////        entity.DateStatus = DateTime.Now;
        ////        entity.EntityId = entityId;
        ////        context.SaveChanges();

        ////        return new DataResponse()
        ////        {
        ////            Data = entity,
        ////            IsSuccess = true
        ////        };
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return new DataResponse()
        ////        {
        ////            Data = ex,
        ////            IsSuccess = false
        ////        };
        ////    }
        ////}

        [HttpGet]
        [Route("api/get/followup/{entityId}/{type}")]
        public async Task<DataResponse> FollowUpReport(int entityId, int type)
        {
            try
            {
                var entity = context.ViewQAFollowingUps.Where(q => q.EntityId == entityId && q.Type == type).ToList();
                return new DataResponse()
                {
                    Data = entity,
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


        [HttpGet]
        [Route("api/get/qa/catering/report/{ymf}/{ymt}")]
        public async Task<DataResponse> GetCateringReport(int ymf, int ymt)
        {
            var dateCount = (from x in context.ViewQADashCaterings
                             where x.YearMonth >= ymf && x.YearMonth <= ymt
                             group x by new { x.YearMonth } into grp
                             select new
                             {
                                 YearMonth = grp.Key.YearMonth,
                                 Count = grp.Sum(q => q.Count),
                             }).ToList();

            var registerCount = (from x in context.ViewQADashCaterings
                                 where x.YearMonth >= ymf && x.YearMonth <= ymt
                                 group x by new { x.Register } into grp
                                 select new
                                 {
                                     Register = grp.Key.Register,
                                     Count = grp.Sum(q => q.Count),
                                 }).ToList();

            var routeCount = (from x in context.ViewQADashCaterings
                              where x.YearMonth >= ymf && x.YearMonth <= ymt
                              group x by new { x.route } into grp
                              select new
                              {
                                  Route = grp.Key.route,
                                  Count = grp.Sum(q => q.Count),
                              }).ToList();


            var reasonTitle = (from x in context.ViewQaDashSecurities
                               where x.YearMonth >= ymf && x.YearMonth <= ymt
                               group x by new { x.ReasonTitle } into grp
                               select new
                               {
                                   ReasonTitle = grp.Key.ReasonTitle,
                                   Count = grp.Sum(q => q.Count),
                               }).ToList();


            var totalCount = dateCount.Sum(q => q.Count);
            return new DataResponse()
            {
                Data = new { DateCount = dateCount, RegisterCount = registerCount, TotalCount = totalCount, ReasonTitle = reasonTitle, RouteCount = routeCount },
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/qa/ground/report/{ymf}/{ymt}")]
        public async Task<DataResponse> GetGroundReport(int ymf, int ymt)
        {
            var dateCount = (from x in context.ViewQaDashGrounds
                             where x.YearMonth >= ymf && x.YearMonth <= ymt
                             group x by new { x.YearMonth } into grp
                             select new
                             {
                                 YearMonth = grp.Key.YearMonth,
                                 Count = grp.Sum(q => q.Count),
                                 InjuredCount = grp.Sum(q => q.InjuredCount),
                                 FatalityCount = grp.Sum(q => q.FatalityCount)
                             }).ToList();

            var registerCount = (from x in context.ViewQaDashGrounds
                                 where x.YearMonth >= ymf && x.YearMonth <= ymt
                                 group x by new { x.Register } into grp
                                 select new
                                 {
                                     Register = grp.Key.Register,
                                     Count = grp.Sum(q => q.Count),
                                     InjuredCount = grp.Sum(q => q.InjuredCount),
                                     FatalityCount = grp.Sum(q => q.FatalityCount)
                                 }).ToList();

            var routeCount = (from x in context.ViewQaDashGrounds
                              where x.YearMonth >= ymf && x.YearMonth <= ymt
                              group x by new { x.route } into grp
                              select new
                              {
                                  Route = grp.Key.route,
                                  Count = grp.Sum(q => q.Count),
                                  InjuredCount = grp.Sum(q => q.InjuredCount),
                                  FatalityCount = grp.Sum(q => q.FatalityCount)
                              }).ToList();

            var damageByCount = (from x in context.ViewQaDashGrounds
                                 where x.YearMonth >= ymf && x.YearMonth <= ymt
                                 group x by new { x.DamageBy } into grp
                                 select new
                                 {
                                     DamageBy = grp.Key.DamageBy,
                                     Count = grp.Sum(q => q.Count),
                                     InjuredCount = grp.Sum(q => q.InjuredCount),
                                     FatalityCount = grp.Sum(q => q.FatalityCount)
                                 }).ToList();



            var totalCount = dateCount.Sum(q => q.Count);
            return new DataResponse()
            {
                Data = new { DateCount = dateCount, RegisterCount = registerCount, RouteCount = routeCount, TotalCount = totalCount, DamageByCount = damageByCount },
                IsSuccess = true
            };
        }


        [HttpGet]
        [Route("api/get/qa/security/report/{ymf}/{ymt}")]
        public async Task<DataResponse> GetSecurityReport(int ymf, int ymt)
        {
            var dateCount = (from x in context.ViewQaDashSecurities
                             where x.YearMonth >= ymf && x.YearMonth <= ymt
                             group x by new { x.YearMonth } into grp
                             select new
                             {
                                 YearMonth = grp.Key.YearMonth,
                                 Count = grp.Sum(q => q.Count),
                             }).ToList();

            var registerCount = (from x in context.ViewQaDashSecurities
                                 where x.YearMonth >= ymf && x.YearMonth <= ymt
                                 group x by new { x.Register } into grp
                                 select new
                                 {
                                     Register = grp.Key.Register,
                                     Count = grp.Sum(q => q.Count),
                                 }).ToList();

            var routeCount = (from x in context.ViewQaDashSecurities
                              where x.YearMonth >= ymf && x.YearMonth <= ymt
                              group x by new { x.route } into grp
                              select new
                              {
                                  Route = grp.Key.route,
                                  Count = grp.Sum(q => q.Count),
                              }).ToList();

            var reasonTitle = (from x in context.ViewQaDashSecurities
                               where x.YearMonth >= ymf && x.YearMonth <= ymt
                               group x by new { x.ReasonTitle } into grp
                               select new
                               {
                                   ReasonTitle = grp.Key.ReasonTitle,
                                   Count = grp.Sum(q => q.Count),
                               }).ToList();


            var totalCount = dateCount.Sum(q => q.Count);
            return new DataResponse()
            {
                Data = new { DateCount = dateCount, RegisterCount = registerCount, RouteCount = routeCount, TotalCount = totalCount, ReasonTitle = reasonTitle },
                IsSuccess = true
            };
        }


        [HttpGet]
        [Route("api/get/qa/csr/report/{ymf}/{ymt}")]
        public async Task<DataResponse> GetCSRReport(int ymf, int ymt)
        {
            var dateCount = (from x in context.ViewQaDashCSRs
                             where x.YearMonth >= ymf && x.YearMonth <= ymt
                             group x by new { x.YearMonth } into grp
                             select new
                             {
                                 YearMonth = grp.Key.YearMonth,
                                 Count = grp.Sum(q => q.Count),
                             }).ToList();

            var registerCount = (from x in context.ViewQaDashCSRs
                                 where x.YearMonth >= ymf && x.YearMonth <= ymt
                                 group x by new { x.Register } into grp
                                 select new
                                 {
                                     Register = grp.Key.Register,
                                     Count = grp.Sum(q => q.Count),
                                 }).ToList();

            var routeCount = (from x in context.ViewQaDashCSRs
                              where x.YearMonth >= ymf && x.YearMonth <= ymt
                              group x by new { x.route } into grp
                              select new
                              {
                                  Route = grp.Key.route,
                                  Count = grp.Sum(q => q.Count),
                              }).ToList();

            var flightPhase = (from x in context.ViewQaDashCSRs
                               where x.YearMonth >= ymf && x.YearMonth <= ymt
                               group x by new { x.FlightPhase } into grp
                               select new
                               {
                                   Route = grp.Key.FlightPhase,
                                   Count = grp.Sum(q => q.Count),
                               }).ToList();


            var totalCount = dateCount.Sum(q => q.Count);
            return new DataResponse()
            {
                Data = new { DateCount = dateCount, RegisterCount = registerCount, RouteCount = routeCount, TotalCount = totalCount, FlightPhase = flightPhase },
                IsSuccess = true
            };
        }


        [HttpGet]
        [Route("api/qa/csr/event/report/{ymf}/{ymt}")]
        public async Task<DataResponse> CSREventReport(int ymf, int ymt)
        {
            var EventCount = (from x in context.ViewQADashCSREvents
                              where x.YearMonth >= ymf && x.YearMonth <= ymt
                              group x by new { x.EventTitle } into grp
                              select new
                              {
                                  EventTitle = grp.Key.EventTitle,
                                  Count = grp.Sum(q => q.Count),
                              }).ToList();

            return new DataResponse()
            {
                Data = EventCount,
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/qa/maintenance/report/{ymf}/{ymt}")]
        public async Task<DataResponse> GetMaintenance(int ymf, int ymt)
        {
            var records = (from x in context.ViewQaDashMaintenances
                           where x.YearMonth >= ymf && x.YearMonth <= ymt
                           select x).ToList();
            var dateCount = (from x in records
                                 // where x.YearMonth >= ymf && x.YearMonth <= ymt
                             group x by new { x.YearMonth } into grp
                             select new
                             {
                                 YearMonth = grp.Key.YearMonth,
                                 Count = grp.Sum(q => q.Count),
                             }).ToList();

            var registerCount = (from x in records
                                     //where x.YearMonth >= ymf && x.YearMonth <= ymt
                                 group x by new { x.Register } into grp
                                 select new
                                 {
                                     Register = grp.Key.Register,
                                     Count = grp.Sum(q => q.Count),
                                 }).ToList();

            var routeCount = (from x in records
                                  // where x.YearMonth >= ymf && x.YearMonth <= ymt
                              group x by new { x.route } into grp
                              select new
                              {
                                  Route = grp.Key.route,
                                  Count = grp.Sum(q => q.Count),
                              }).ToList();

            var components = (from x in records
                              group x by new { x.ComponentSpecificationTitle, x.ComponentSpecificationId } into grp
                              select new
                              {
                                  Component = string.IsNullOrEmpty(grp.Key.ComponentSpecificationTitle) ? "Unknown" : grp.Key.ComponentSpecificationTitle,
                                  ComponentId = grp.Key.ComponentSpecificationId,
                                  Count = grp.Sum(q => q.Count),
                              }).ToList();
            var components_regs = (from x in records
                                   group x by new { x.ComponentSpecificationTitle, x.ComponentSpecificationId, x.Register } into grp
                                   select new
                                   {
                                       Component = string.IsNullOrEmpty(grp.Key.ComponentSpecificationTitle) ? "Unknown" : grp.Key.ComponentSpecificationTitle,
                                       ComponentId = grp.Key.ComponentSpecificationId,
                                       Register = grp.Key.Register,
                                       Count = grp.Sum(q => q.Count),
                                   }).ToList();


            var totalCount = dateCount.Sum(q => q.Count);
            return new DataResponse()
            {
                Data = new { DateCount = dateCount, RegisterCount = registerCount, RouteCount = routeCount, TotalCount = totalCount, components, components_regs },
                IsSuccess = true
            };
        }


        [HttpGet]
        [Route("api/get/qa/voluntary/report/{ymf}/{ymt}")]
        public async Task<DataResponse> GetVoluntaryReport(int ymf, int ymt)
        {
            var dateCount = (from x in context.ViewQaDashHazards
                             where x.YearMonth >= ymf && x.YearMonth <= ymt
                             group x by new { x.YearMonth } into grp
                             select new
                             {
                                 YearMonth = grp.Key.YearMonth,
                                 Count = grp.Sum(q => q.Count),
                             }).ToList();



            var totalCount = dateCount.Sum(q => q.Count);
            return new DataResponse()
            {
                Data = new { DateCount = dateCount, TotalCount = totalCount },
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/qa/cyber/report/{ymf}/{ymt}")]
        public async Task<DataResponse> GetCyberReport(int ymf, int ymt)
        {
            var dateCount = (from x in context.ViewQADashCybers
                             where x.YearMonth >= ymf && x.YearMonth <= ymt
                             group x by new { x.YearMonth } into grp
                             select new
                             {
                                 YearMonth = grp.Key.YearMonth,
                                 Count = grp.Sum(q => q.Count),
                             }).ToList();

            var registerCount = (from x in context.ViewQADashCybers
                                 where x.YearMonth >= ymf && x.YearMonth <= ymt
                                 group x by new { x.Register } into grp
                                 select new
                                 {
                                     Register = grp.Key.Register,
                                     Count = grp.Sum(q => q.Count),
                                 }).ToList();

            var routeCount = (from x in context.ViewQADashCybers
                              where x.YearMonth >= ymf && x.YearMonth <= ymt
                              group x by new { x.route } into grp
                              select new
                              {
                                  Route = grp.Key.route,
                                  Count = grp.Sum(q => q.Count),
                              }).ToList();


            var totalCount = dateCount.Sum(q => q.Count);
            return new DataResponse()
            {
                Data = new { DateCount = dateCount, RegisterCount = registerCount, RouteCount = routeCount, TotalCount = totalCount },
                IsSuccess = true
            };
        }

        [HttpGet]
        [Route("api/get/qa/dispatch/report/{ymf}/{ymt}")]
        public async Task<DataResponse> GetDispatchReport(int ymf, int ymt)
        {
            var dateCount = (from x in context.ViewQADashDispatches
                             where x.YearMonth >= ymf && x.YearMonth <= ymt
                             group x by new { x.YearMonth } into grp
                             select new
                             {
                                 YearMonth = grp.Key.YearMonth,
                                 Count = grp.Sum(q => q.Count),
                             }).ToList();

            var registerCount = (from x in context.ViewQADashDispatches
                                 where x.YearMonth >= ymf && x.YearMonth <= ymt
                                 group x by new { x.Register } into grp
                                 select new
                                 {
                                     Register = grp.Key.Register,
                                     Count = grp.Sum(q => q.Count),
                                 }).ToList();

            var routeCount = (from x in context.ViewQADashDispatches
                              where x.YearMonth >= ymf && x.YearMonth <= ymt
                              group x by new { x.route } into grp
                              select new
                              {
                                  Route = grp.Key.route,
                                  Count = grp.Sum(q => q.Count),
                              }).ToList();


            var totalCount = dateCount.Sum(q => q.Count);
            return new DataResponse()
            {
                Data = new { DateCount = dateCount, RegisterCount = registerCount, RouteCount = routeCount, TotalCount = totalCount },
                IsSuccess = true
            };
        }


        [HttpGet]
        [Route("api/get/qa/employee/{type}/{entityId}/{referrerId}")]
        public async Task<DataResponse> GetQAEmployee(int type, int entityId, int referrerId)
        {
            try
            {
                var query = from x in context.ViewQAResponsibilities
                            where x.Type == type
                            select new
                            {
                                TypeTitle = x.TypeTitle,
                                Name = x.Name,
                                ReceiverEmployeeId = x.ReceiverEmployeeId,
                                JobGroup = x.JobGroup,
                                Mobile = x.Mobile
                            };

                var ds = query.Select(q => q.ReceiverEmployeeId).ToList();
                var ableToRefer = context.ViewQAFollowingUps.Where(q => q.Type == type && q.EntityId == entityId && q.ReferrerId == referrerId && ds.Contains(q.ReferredId)).Select(q => q.ReferredId).ToList();
                var result = query.Where(q => !ableToRefer.Contains(q.ReceiverEmployeeId)).ToList();

                return new DataResponse()
                {
                    Data = result,
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


        public class QADto
        {
            public int EntityId { get; set; }
            public int ReferredId { get; set; }
            public int ReferrerId { get; set; }
            public int Type { get; set; }
            public string Comment { get; set; }
            public int Priority { get; set; }
            public DateTime DeadLine { get; set; }
        }

        [HttpGet]
        [Route("get/ast/entityId/{flightId}")]
        public async Task<DataResponse> GetASREntityId(int flightId)
        {
            try
            {
              var result = context.EFBASRs.SingleOrDefault(q => q.FlightId == flightId).Id;
                return new DataResponse() { Data = result, IsSuccess = true };

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



        [HttpPost]
        [Route("api/qa/referr")]
        public async Task<DataResponse> QAReferr(List<QADto> dto)
        {
            try
            {
                var test = dto;
                var parentId = 0;
               
                foreach (var y in test)
                {
                    parentId = context.ViewQAFollowingUps.Where(q => q.Type == (int?)y.Type && q.EntityId == (int?)y.EntityId && q.ReferredId == (int?)y.ReferrerId).ToList().OrderByDescending(q => q.Id).First().Id;
                }



                foreach (var x in test)
                {

                    var entity = new QAFollowingUp();
                    context.QAFollowingUps.Add(entity);
                    entity.EntityId = x.EntityId;
                    entity.ReferredId = x.ReferredId;
                    entity.ReferrerId = x.ReferrerId;
                    entity.DateStatus = DateTime.Now;
                    entity.Type = x.Type;
                    entity.ReviewResult = 2;
                    entity.Comment = x.Comment;
                    entity.ParentId = parentId;
                    entity.DeadLine = null;
                    entity.Priority = x.Priority;
                };
                context.SaveChanges();

                return new DataResponse() { Data = null, IsSuccess = true };

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


        [HttpPost]
        [Route("api/qa/accept")]
        public async Task<DataResponse> AcceptQA(dynamic dto)
        {
            try
            {
                if (dto.isResponsible == true)
                {

                    var test = DateTime.Now.ToString("yyyy-MM-dd");
                    switch ((int)dto.Type)
                    {
                        case 0:
                            context.Database.ExecuteSqlCommand("update QACSR set Status = 1,DateStatus = '" + test + "', StatusEmployeeId= " + dto.EmployeeId + ",Result = '" + dto.Result + "' where Id=" + dto.Id);
                            break;
                        case 1:
                            context.Database.ExecuteSqlCommand("update QAGroundIAD set Status = 1,DateStatus = '" + test + "', StatusEmployeeId= " + dto.EmployeeId + ",Result = '" + dto.Result + "' where Id=" + dto.Id);
                            break;
                        case 2:
                            context.Database.ExecuteSqlCommand("update QAHazard set Status = 1,DateStatus = '" + test + "', StatusEmployeeId= " + dto.EmployeeId + ",Result = '" + dto.Result + "' where Id=" + dto.Id);
                            break;
                        case 3:
                            context.Database.ExecuteSqlCommand("update QAMaintenance set Status = 1,DateStatus = '" + test + "', StatusEmployeeId= " + dto.EmployeeId + ",Result = '" + dto.Result + "' where Id=" + dto.Id);
                            break;
                        case 4:
                            context.Database.ExecuteSqlCommand("update QACatering set Status = 1,DateStatus = '" + test + "', StatusEmployeeId= " + dto.EmployeeId + ",Result = '" + dto.Result + "' where Id=" + dto.Id);
                            break;
                        case 5:
                            context.Database.ExecuteSqlCommand("update QASecurity set Status = 1,DateStatus = '" + test + "', StatusEmployeeId= " + dto.EmployeeId + ",Result = '" + dto.Result + "' where Id=" + dto.Id);
                            break;
                        case 6:
                            context.Database.ExecuteSqlCommand("update QADispatch set Status = 1,DateStatus = '" + test + "', StatusEmployeeId= " + dto.EmployeeId + ",Result = '" + dto.Result + "' where Id=" + dto.Id);
                            break;
                        case 7:
                            context.Database.ExecuteSqlCommand("update QACyber set Status = 1,DateStatus = '" + test + "', StatusEmployeeId= " + dto.EmployeeId + ",Result = '" + dto.Result + "' where Id=" + dto.Id);
                            break;
                    }
                }

                var entity = new QAFollowingUp();
                context.QAFollowingUps.Add(entity);
                entity.Type = dto.Type;
                entity.EntityId = dto.Id;
                entity.ReferrerId = dto.EmployeeId;
                entity.ReferredId = null;
                entity.ReviewResult = 1;
                entity.DateStatus = DateTime.Now;




                context.SaveChanges();
                return new DataResponse()
                {
                    Data = null,
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


        [HttpPost]
        [Route("api/qa/reject")]
        public async Task<DataResponse> RejectQA(dynamic dto)
        {
            try
            {
                var entity = new QAFollowingUp();
                context.QAFollowingUps.Add(entity);
                entity.Type = dto.Type;
                entity.EntityId = dto.Id;
                entity.ReferrerId = dto.EmployeeId;
                entity.ReviewResult = 0;
                entity.DateStatus = DateTime.Now;

                context.SaveChanges();
                dto.Id = entity.Id;
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





        [HttpGet]
        [Route("api/qa/get/referred/{referreId}/{type}/{entityId}")]
        public async Task<DataResponse> GetReferredList(int referreId, int type, int entityId)
        {

            try
            {
                var data = from x in context.ViewQAFollowingUps
                           join y in context.ViewQABYEmployees on new { EmployeeId = x.ReferredId, Id = x.EntityId, Type = x.Type } equals new { EmployeeId = y.EmployeeId, Id = y.EntityId, Type = y.Type } into grp
                           from matching in grp
                           where x.Type == type && x.EntityId == entityId
                           select new
                           {
                               Id = x.Id,
                               ReferredId = x.ReferredId,
                               ReferrerId = x.ReferrerId,
                               ReferredName = x.ReferredName,
                               ReferrerName = x.ReferrerName,
                               DateStatus = x.DateStatus,
                               Type = x.Type,
                               ReviewResultTitle = matching.ReviewResultTitle,
                               ReviewResult = matching.ReviewResult,
                               Comment = x.Comment,
                               EntityId = x.EntityId,
                               ParentId = x.ParentId,
                           };


                //var data = from x in context.ViewQAFollowingUps
                //           where x.Type == type && x.EntityId == entityId && x.ReferredId != null
                //           select new
                //           {
                //               ReferredId = x.ReferredId,
                //               ReferrerId = x.ReferrerId,
                //               ReferredName = x.ReferredName,
                //               ReferrerName = x.ReferrerName,
                //               DateStatus = x.DateStatus,
                //               Type = x.Type,
                //               ReviewResultTitle = x.ReviewResultTitle,
                //               EntityId = x.EntityId,
                //           };

                var result = data.ToList();

                return new DataResponse()
                {
                    Data = result,
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


        [HttpGet]
        [Route("api/qa/isresponsible/{employeeId}/{type}/{entityId}")]
        public async Task<DataResponse> QAIsResponsible(int employeeId, int type, int entityId)
        {
            try
            {

                var isResponsible = context.ViewQAFollowingUps.Single(q => q.EntityId == entityId && q.ReferredId == employeeId && q.Type == type && q.ReferrerId == 0);
                return new DataResponse()
                {
                    Data = isResponsible,
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


        [HttpGet]
        [Route("api/qa/get/comments/{entityId}/{type}")]
        public async Task<DataResponse> GetComments(int entityId, int type)
        {

            try
            {
                var query = from comments in context.QAComments
                            join files in context.QAAttachments on comments.id equals files.CommentId into commentAttachmnets
                            join employee in context.ViewEmployees on comments.EmployeeId equals employee.Id into employeeComment
                            where comments.EntityId == entityId && comments.Type == type

                            select new
                            {
                                Comment = comments.Comment,
                                CommentId = comments.id,
                                EmployeeId = comments.EmployeeId,
                                EmployeeName = employeeComment.Select(q => q.Name).FirstOrDefault(),
                                DateComment = comments.DateComment,
                                Attachments = commentAttachmnets.ToList()

                            };

                var result = query.ToList();

                return new DataResponse()
                {
                    Data = result,
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

        [HttpGet]
        [Route("api/qa/get/employee/{EmployeeId}")]
        public async Task<DataResponse> GetEmployeeInformation(int EmployeeId)
        {

            try
            {
                var result = context.ViewEmployees.Single(q => q.Id == EmployeeId);
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }

        }


        [HttpPost]
        [Route("api/qa/send/comment")]
        public async Task<DataResponse> SendComment(dynamic dto)
        {
            try
            {
                var entity = new QAComment();
                context.QAComments.Add(entity);
                entity.EntityId = dto.Id;
                entity.Type = dto.Type;
                entity.EmployeeId = dto.EmployeeId;
                entity.Comment = dto.Comment;
                entity.DateComment = DateTime.Now;
                context.SaveChanges();

                if (dto.files != null)
                {
                    foreach (var f in dto.files)
                    {

                        int AttachmentId = f.AttachmentId;
                        var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                        if (file == null)
                        {
                            file = new QAAttachment();
                            context.QAAttachments.Add(file);
                        }
                        file.EntityId = dto.Id;
                        file.EmployeeId = dto.EmployeeId;
                        file.URL = HttpContext.Current.Server.MapPath("~/upload/" + f.FileName);
                        file.Lable = f.FileName;
                        file.DateAttach = DateTime.Now;
                        file.AttachmentType = f.FileType;
                        file.Description = f.Description;
                        file.Type = dto.Type;
                        file.CommentId = entity.id;
                    }

                }
                context.SaveChanges();
                dto.Id = entity.EntityId;
                return new DataResponse()
                {
                    Data = null,
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


        //[HttpGet]
        //[Route("api/comment/{id}/{type}/{employeeId}")]
        //public async Task<DataResponse> qaSaveAttachmentComment(int id, int type, int employeeId)
        //{
        //    try
        //    {
        //        var comment = context.QAComments.Single(q => q.EntityId == id && q.EmployeeId == employeeId && q.Type == type);
        //        var files = context.QAAttachments.Where(q => q.EntityId == id && q.EmployeeId == employeeId && q.Type == type).ToList();

        //        foreach (var x in files)
        //        {
        //            var entity = new QAAttachmentComment();
        //            context.QAAttachmentComments.Add(entity);
        //            entity.CommentId = (int)comment.id;
        //            entity.AttachmentId = (int)x.Id;
        //        };

        //        context.SaveChanges();

        //        return new DataResponse()
        //        {
        //            Data = null,
        //            IsSuccess = true
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += "   Inner: " + ex.InnerException.Message;
        //        return new DataResponse()
        //        {
        //            Data = msg,
        //            IsSuccess = false
        //        };
        //    }

        //}



        [HttpPost]
        [Route("api/qa/uploadfile")]
        public async Task<IHttpActionResult> Upload()
        {
            //var context = new FLYEntities();
            string key = string.Empty;
            var Files = HttpContext.Current.Request.Files;

            var docfiles = new List<string>();
            foreach (string file in Files)
            {
                var postedFile = Files[file];
                key = postedFile.FileName;
                var filePath = HttpContext.Current.Server.MapPath("~/upload/" + key);
                postedFile.SaveAs(filePath);
                docfiles.Add(filePath);
            }

            return Ok(docfiles);
        }

        [HttpPost]
        [Route("api/import/attachment")]
        public async Task<DataResponse> ImportAttachment(dynamic dto)
        {
            try
            {
                int EntityId = dto.EntityId;
                int Type = dto.Type;
                string Lable = dto.Lable;

                var entity = context.QAAttachments.SingleOrDefault(q => q.EntityId == EntityId && q.Type == Type && q.Lable == Lable);
                if (entity == null)
                {
                    entity = new QAAttachment();
                    context.QAAttachments.Add(entity);
                }
                entity.EntityId = dto.EntityId;
                entity.EmployeeId = dto.EmployeeId;
                entity.URL = HttpContext.Current.Server.MapPath("~/upload/" + dto.FileName);
                entity.Lable = dto.FileName;
                entity.DateAttach = DateTime.Now;
                entity.AttachmentType = dto.FileType;
                entity.Description = dto.Description;
                entity.Type = dto.Type;

                context.SaveChanges();
                dto.Id = entity.Id;
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }

        [HttpGet]
        [Route("api/get/imported/file/{entityId}/{employeeId}/{type}")]
        public async Task<DataResponse> GetImportedFile(int entityId, int employeeId, int type)
        {
            try
            {
                var result = context.QAAttachments.Where(q => q.EmployeeId == employeeId && q.EntityId == entityId && q.Type == type && q.CommentId == null).ToList();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }

        [HttpPost]
        [Route("api/delete/attachment")]
        public async Task<DataResponse> DeleteAttachment(dynamic dto)
        {
            try
            {
                int AttachmentId = dto.AttachmentId;

                var filePath = HttpContext.Current.Server.MapPath("~/upload/" + dto.FileName);
                if (File.Exists(filePath))
                    File.Delete(filePath);


                var file = context.QAAttachments.SingleOrDefault(q => q.Id == AttachmentId);
                if (file != null)
                    context.QAAttachments.Remove(file);




                context.SaveChanges();
                return new DataResponse()
                {
                    Data = file,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }

        [HttpGet]
        [Route("api/download/qa/{filename}/{filetype}")]
        public HttpResponseMessage DownloadQAAttachment(string filename, string filetype)
        {
            //string filename = "server.txt";
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(HttpContext.Current.Server.MapPath("~/upload/" + filename + "." + filetype), FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = filename + "." + filetype;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return response;

        }


        [HttpGet]
        [Route("api/qa/form/date/{yearmonth}/{type}")]
        public async Task<DataResponse> GetFormByDate(int yearmonth, int type)
        {
            try
            {
                int year = (int)Math.Truncate(yearmonth / Math.Pow(10, 2));
                int month = yearmonth - (year * 100);
                dynamic result = null;

                switch (type)
                {
                    case 0:
                        result = (from x in context.ViewQACSRs
                                  where x.DateOccurrence.Value.Year == year && x.DateOccurrence.Value.Month == month && x.DateSign != null
                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.EmployeeId,
                                      x.DateOccurrence,
                                  }).ToList();
                        break;
                    case 1:
                        result = (from x in context.ViewQAGrounds
                                  where x.DateOccurrence.Value.Year == year && x.DateOccurrence.Value.Month == month && x.DateSign != null
                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.EmployeeId,
                                      x.DateOccurrence,
                                  }).ToList();
                        break;
                    case 2:
                        result = (from x in context.ViewQAHazards
                                  where x.DateOccurrence.Value.Year == year && x.DateOccurrence.Value.Month == month && x.DateSign != null
                                  select new
                                  {
                                      //x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 3:
                        result = (from x in context.ViewQAMaintenances
                                  where x.DateOccurrence.Value.Year == year && x.DateOccurrence.Value.Month == month && x.DateSign != null
                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 4:
                        result = (from x in context.ViewQACaterings
                                  where x.DateOccurrence.Value.Year == year && x.DateOccurrence.Value.Month == month && x.DateSign != null
                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 5:
                        result = (from x in context.ViewQASecurities
                                  where x.DateOccurrence.Value.Year == year && x.DateOccurrence.Value.Month == month && x.DateSign != null
                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 6:
                        result = (from x in context.ViewQADispatches
                                  where x.DateOccurrence.Value.Year == year && x.DateOccurrence.Value.Month == month && x.DateSign != null
                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 7:
                        result = (from x in context.ViewQACybers
                                  where x.DateOccurrence.Value.Year == year && x.DateOccurrence.Value.Month == month && x.DateSign != null
                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                }




                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }

        [HttpGet]
        [Route("api/qa/form/register/{yf}/{yt}/{mf}/{mt}/{register}/{type}")]
        public async Task<DataResponse> GetFormByRegister(int yf, int mf, int yt, int mt, string register, int type)
        {
            try
            {

                dynamic result = null;

                switch (type)
                {
                    case 0:
                        result = (from x in context.ViewQACSRs
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Register == register && x.DateSign != null
                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 1:
                        result = (from x in context.ViewQAGrounds
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Register == register && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;

                    case 3:
                        result = (from x in context.ViewQAMaintenances
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Register == register && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 4:
                        result = (from x in context.ViewQACaterings
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Register == register && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 5:
                        result = (from x in context.ViewQASecurities
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Register == register && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 6:
                        result = (from x in context.ViewQADispatches
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Register == register && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 7:
                        result = (from x in context.ViewQACybers
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Register == register && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                }




                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
        }



        [HttpGet]
        [Route("api/qa/form/route/{yf}/{yt}/{mf}/{mt}/{route}/{type}")]
        public async Task<DataResponse> GetFormByRoute(int yf, int mf, int yt, int mt, string route, int type)
        {
            try
            {

                dynamic result = null;

                switch (type)
                {
                    case 0:
                        result = (from x in context.ViewQACSRs
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Route == route && x.DateSign != null
                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 1:
                        result = (from x in context.ViewQAGrounds
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Route == route && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;

                    case 3:
                        result = (from x in context.ViewQAMaintenances
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Route == route && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 4:
                        result = (from x in context.ViewQACaterings
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Route == route && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 5:
                        result = (from x in context.ViewQASecurities
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Route == route && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 6:
                        result = (from x in context.ViewQADispatches
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Route == route && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                    case 7:
                        result = (from x in context.ViewQACybers
                                  where (x.DateOccurrence.Value.Year >= yf && x.DateOccurrence.Value.Month >= mf) && (x.DateOccurrence.Value.Year <= yt && x.DateOccurrence.Value.Month <= mt) && x.Route == route && x.DateSign != null

                                  select new
                                  {
                                      x.FlightNumber,
                                      x.Id,
                                      x.EmployeeName,
                                      x.DateOccurrence,
                                      x.EmployeeId,
                                  }).ToList();
                        break;
                }




                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   Inner: " + ex.InnerException.Message;
                return new DataResponse()
                {
                    Data = msg,
                    IsSuccess = false
                };
            }
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
