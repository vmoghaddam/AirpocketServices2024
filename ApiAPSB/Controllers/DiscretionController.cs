using ApiAPSB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data.Entity;
using Microsoft.Ajax.Utilities;

namespace ApiAPSB.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DiscretionController : ApiController
    {

        public class DataResponse
        {
            public bool IsSuccess { get; set; }
            public dynamic Data { get; set; }
        }

        public class fdp_item_dto
        {

            public int flight_id { get; set; }
            public DateTime? off_block { get; set; }
            public DateTime? off_block_local { get; set; }
            public DateTime? off_block_actual { get; set; }
            public DateTime? off_block_actual_local { get; set; }
            public DateTime? on_block { get; set; }
            public DateTime? on_block_actual { get; set; }
            public DateTime? on_block_local { get; set; }
            public DateTime? on_block_actual_local { get; set; }
            public string from_airport { get; set; }
            public string to_airport { get; set; }

        }

        public class fdp_dto
        {
            public int? Id { get; set; }
            public int? CrewId { get; set; }
            public DateTime? ReportingTime { get; set; }
            public DateTime? DelayedReportingTime { get; set; }
            public DateTime? RevisedDelayedReportingTime { get; set; }
            public DateTime? FirstNotification { get; set; }
            public DateTime? NextNotification { get; set; }
            public int? DelayAmount { get; set; }
            public int? BoxId { get; set; }
            public int? JobGroupId { get; set; }
            public bool IsTemplate { get; set; }
            public int? DutyType { get; set; }
            public DateTime? DateContact { get; set; }
            public int? FDPId { get; set; }
            public DateTime? DateStart { get; set; }
            public DateTime? DateEnd { get; set; }
            public int? CityId { get; set; }
            public int? TemplateId { get; set; }
            public DateTime? FDPReportingTime { get; set; }

            public int? FirstFlightId { get; set; }
            public int? LastFlightId { get; set; }
            public int? UPD { get; set; }
            public bool IsMain { get; set; }
            public string Key { get; set; }
            public bool CP { get; set; }
            public int? CustomerId { get; set; }
            public string Remark { get; set; }
            public int? LocationId { get; set; }
            public DateTime? InitStart { get; set; }
            public DateTime? InitEnd { get; set; }
            public DateTime? InitRestTo { get; set; }
            public string InitFlts { get; set; }
            public string InitRoute { get; set; }
            public string InitFromIATA { get; set; }
            public string InitToIATA { get; set; }
            public string InitNo { get; set; }
            public string InitKey { get; set; }
            public int? InitHomeBase { get; set; }
            public string InitRank { get; set; }
            public int? InitIndex { get; set; }
            public string InitGroup { get; set; }
            public string InitScheduleName { get; set; }
            public string InitFlights { get; set; }
            public string Remark2 { get; set; }
            public string CanceledNo { get; set; }
            public string CanceledRoute { get; set; }
            public int? Extension { get; set; }
            public double Split { get; set; }
            public DateTime? DateConfirmed { get; set; }
            public string ConfirmedBy { get; set; }
            public string UserName { get; set; }
            public Nullable<decimal> MaxFDP { get; set; }
            public int? BL { get; set; }
            public int? FX { get; set; }
            public DateTime? ActualStart { get; set; }
            public DateTime? ActualEnd { get; set; }
            public DateTime? ActualRestTo { get; set; }
            public bool IsOver { get; set; }
            public DateTime? STD { get; set; }
            public DateTime? STA { get; set; }
            public bool OutOfHomeBase { get; set; }
            public string InitPosition { get; set; }
            public DateTime? PLNEnd { get; set; }
            public DateTime? PLNRest { get; set; }
            public string PosFrom { get; set; }
            public string PosTo { get; set; }
            public DateTime? PosDep { get; set; }
            public DateTime? PosArr { get; set; }
            public string PosAirline { get; set; }
            public int? PosFDPId { get; set; }
            public string PosRemark { get; set; }
            public string PosTicketUrl { get; set; }
            public DateTime? DutyEndDateLocal { get; set; }
            public string InitTo { get; set; }

            public DateTime? DSUTC { get; set; }
            public DateTime? DSLocal { get; set; }
            public DateTime? DSActualUTC { get; set; }
            public DateTime? DSActualLocal { get; set; }
            public string DSPlace { get; set; }
            public DateTime? DEUTC { get; set; }
            public DateTime? DELocal { get; set; }
            public DateTime? DEActualUTC { get; set; }
            public DateTime? DEActualLocal { get; set; }
            public string DEPlace { get; set; }

            public DateTime? FDPUTC { get; set; }
            public DateTime? FDPLocal { get; set; }
            public DateTime? FDPActualUTC { get; set; }
            public DateTime? FDPActualLocal { get; set; }
            public string FDPPlace { get; set; }

            public TimeSpan? PlannedDuty { get; set; }
            public TimeSpan? ActualDuty { get; set; }
            public decimal? CommanderDiscretion { get; set; }
            public List<fdp_item_dto> fdp_items { get; set; }
        }

        public class CLJLData
        {
            public int? CrewId { get; set; }
            public bool? IsPositioning { get; set; }
            public int? PositionId { get; set; }
            public string Position { get; set; }
            public string Name { get; set; }
            public int? GroupId { get; set; }
            public string JobGroup { get; set; }
            public string JobGroupCode { get; set; }
            public int? GroupOrder { get; set; }
            public int IsCockpit { get; set; }
            public string PassportNo { get; set; }

            public List<string> Legs { get; set; }
            public string LegsStr { get; set; }

            public int? FlightId { get; set; }
            public string PID { get; set; }

            public string Mobile { get; set; }
            public string Address { get; set; }
            public string NID { get; set; }


            //public string ATLNO { get; set; }
            //public DateTime Date { get; set; }
            //public int ScheduledTime { get; set; }
            //public int STD { get; set; }
            //public int MaxFDP { get; set; }
            //public DateTime ReportingTime { get; set; }
            //public DateTime EndFDP { get; set; }
            //public int FDP { get; set; }

            public double? TotalFlightTime { get; set; }
            public double? TotalBlockTime { get; set; }
            //public int TotalNight { get; set; }
            //public DateTime Night { get; set; }


        }



        [Route("api/get/fdp/{fdp_id}")]
        [HttpGet]
        public async Task<DataResponse> GetFDP(int fdp_id)
        {
            using (dbEntities context = new dbEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                try
                {
                    var result = new fdp_dto();
                    var fdp = await context.FDPs.FirstOrDefaultAsync(q => q.Id == fdp_id);
                    var query = await (from item in context.FDPItems
                                       where item.FDPId == fdp_id
                                       join appleg in context.AppLegs on item.FlightId equals appleg.ID
                                       select new fdp_item_dto
                                       {
                                           flight_id = appleg.ID,
                                           off_block = appleg.STD,
                                           off_block_actual = appleg.BlockOff,
                                           on_block = appleg.STA,
                                           on_block_actual = appleg.BlockOn,
                                           off_block_local = appleg.STDLocal,
                                           off_block_actual_local = appleg.BlockOff,
                                           on_block_local = appleg.STALocal,
                                           on_block_actual_local = appleg.BlockOn,
                                           from_airport = appleg.FromAirportIATA,
                                           to_airport = appleg.ToAirportIATA
                                       }).ToListAsync();

                    result.Id = fdp.Id;
                    result.CrewId = fdp.CrewId;
                    result.ReportingTime = fdp.ReportingTime;
                    result.DelayedReportingTime = fdp.DelayedReportingTime;
                    result.RevisedDelayedReportingTime = fdp.RevisedDelayedReportingTime;
                    result.FirstNotification = fdp.FirstNotification;
                    result.NextNotification = fdp.NextNotification;
                    result.DelayAmount = fdp.DelayAmount;
                    result.BoxId = fdp.BoxId;
                    result.JobGroupId = fdp.JobGroupId;
                    result.IsTemplate = fdp.IsTemplate;
                    result.DutyType = fdp.DutyType;
                    result.DateContact = fdp.DateContact;
                    result.FDPId = fdp.FDPId;
                    result.DateStart = fdp.DateStart;
                    result.DateEnd = fdp.DateEnd;
                    result.CityId = fdp.CityId;
                    result.TemplateId = fdp.TemplateId;
                    result.FDPReportingTime = fdp.FDPReportingTime;

                    result.FirstFlightId = fdp.FirstFlightId;
                    result.LastFlightId = fdp.LastFlightId;
                    result.UPD = fdp.UPD;
                    result.IsMain = fdp.IsMain.HasValue ? fdp.IsMain.Value : false; // Handle nullable bool
                    result.Key = fdp.Key;
                    result.CP = fdp.CP.HasValue ? fdp.CP.Value : false; // Handle nullable bool
                    result.CustomerId = fdp.CustomerId;
                    result.Remark = fdp.Remark;
                    result.LocationId = fdp.LocationId;
                    result.InitStart = fdp.InitStart;
                    result.InitEnd = fdp.InitEnd;
                    result.InitRestTo = fdp.InitRestTo;
                    result.InitFlts = fdp.InitFlts;
                    result.InitRoute = fdp.InitRoute;
                    result.InitFromIATA = fdp.InitFromIATA;
                    result.InitToIATA = fdp.InitToIATA;
                    result.InitNo = fdp.InitNo;
                    result.InitKey = fdp.InitKey;
                    result.InitHomeBase = fdp.InitHomeBase;
                    result.InitRank = fdp.InitRank;
                    result.InitIndex = fdp.InitIndex;
                    result.InitGroup = fdp.InitGroup;
                    result.InitScheduleName = fdp.InitScheduleName;
                    result.InitFlights = fdp.InitFlights;
                    result.Remark2 = fdp.Remark2;
                    result.CanceledNo = fdp.CanceledNo;
                    result.CanceledRoute = fdp.CanceledRoute;
                    result.Extension = fdp.Extension;
                    result.Split = fdp.Split.HasValue ? fdp.Split.Value : 0.0; // Handle nullable double
                    result.DateConfirmed = fdp.DateConfirmed;
                    result.ConfirmedBy = fdp.ConfirmedBy;
                    result.UserName = fdp.UserName;
                    result.MaxFDP = fdp.MaxFDP;
                    result.BL = fdp.BL;
                    result.FX = fdp.FX;
                    result.ActualStart = fdp.ActualStart;
                    result.ActualEnd = fdp.ActualEnd;
                    result.ActualRestTo = fdp.ActualRestTo;
                    result.IsOver = fdp.IsOver.HasValue ? fdp.IsOver.Value : false; // Handle nullable bool
                    result.STD = fdp.STD;
                    result.STA = fdp.STA;
                    result.OutOfHomeBase = fdp.OutOfHomeBase.HasValue ? fdp.OutOfHomeBase.Value : false; // Handle nullable bool
                    result.InitPosition = fdp.InitPosition;
                    result.PLNEnd = fdp.PLNEnd;
                    result.PLNRest = fdp.PLNRest;
                    result.PosFrom = fdp.PosFrom;
                    result.PosTo = fdp.PosTo;
                    result.PosDep = fdp.PosDep;
                    result.PosArr = fdp.PosArr;
                    result.PosAirline = fdp.PosAirline;
                    result.PosFDPId = fdp.PosFDPId;
                    result.PosRemark = fdp.PosRemark;
                    result.PosTicketUrl = fdp.PosTicketUrl;
                    result.DutyEndDateLocal = fdp.DutyEndDateLocal;
                    result.InitTo = fdp.InitTo;
                    result.DSUTC = query[0].off_block.HasValue ? query[0].off_block.Value.AddMinutes(-60) : (DateTime?)null;
                    result.DSLocal = query[0].off_block_local.HasValue ? query[0].off_block_local.Value.AddMinutes(-60) : (DateTime?)null;
                    result.DSActualUTC = query[0].off_block_actual.HasValue ? query[0].off_block_actual.Value.AddMinutes(-60) : (DateTime?)null;
                    result.DSActualLocal = query[0].off_block_actual_local.HasValue ? query[0].off_block_actual_local.Value.AddMinutes(-60) : (DateTime?)null;
                    result.DSPlace = query[0].from_airport;
                    result.FDPUTC = query[query.Count - 1].on_block;
                    result.FDPLocal = query[query.Count - 1].on_block_local;
                    result.FDPActualUTC = query[query.Count - 1].on_block_actual;
                    result.FDPActualLocal = query[query.Count - 1].on_block_actual_local;
                    result.FDPPlace = query[query.Count - 1].to_airport;
                    result.DEUTC = query[query.Count - 1].on_block.HasValue ? query[0].on_block.Value.AddMinutes(-60) : (DateTime?)null;
                    result.DELocal = query[query.Count - 1].on_block_local.HasValue ? query[0].on_block_local.Value.AddMinutes(30) : (DateTime?)null;
                    result.DEActualUTC = query[query.Count - 1].on_block_actual.HasValue ? query[0].on_block_actual.Value.AddMinutes(30) : (DateTime?)null;
                    result.DEActualLocal = query[query.Count - 1].on_block_actual_local.HasValue ? query[0].on_block_actual_local.Value.AddMinutes(30) : (DateTime?)null;
                    result.DEPlace = query[query.Count - 1].to_airport;
                    result.PlannedDuty = (result.DEUTC.HasValue && result.DSUTC.HasValue) ? (TimeSpan?)(result.DEUTC.Value - result.DSUTC.Value.AddMinutes(-60)) : null;
                    result.ActualDuty = (query[query.Count - 1].on_block_actual.HasValue && query[0].off_block_actual.HasValue) ? (TimeSpan?)(query[query.Count - 1].on_block_actual.Value - query[0].off_block_actual.Value.AddMinutes(-60)) : null;
                    result.CommanderDiscretion = result.ActualDuty.HasValue ? (decimal?)result.ActualDuty.Value.TotalMinutes - result.MaxFDP : null;
                    result.fdp_items = query;

                    return new DataResponse()
                    {
                        IsSuccess = true,
                        Data = result
                    };
                }
                catch (Exception ex)
                {
                    return new DataResponse() { IsSuccess = false, Data = ex };
                }
            }
        }


        [Route("api/get/fdp/crew/{fid}")]
        [AcceptVerbs("GET")]
        public async Task<DataResponse> GetCrewList(int fid)
        {
            dbEntities context = new dbEntities();

            try
            {
                var appleg = context.AppLegs.FirstOrDefault(q => q.FlightId == fid);
                var pid = appleg.PICId;
                var appFlight = context.AppCrewFlightJLs.Where(q => q.FlightId == fid && q.CrewId == pid).FirstOrDefault();
                var crewlegs = context.AppCrewFlightJLs.Where(q => q.FDPId == appFlight.FDPId).ToList();
                var clegs = crewlegs.Select(q => (int)q.FlightId).ToList();
                var legs = context.AppLegJLs.Where(q => clegs.Contains(q.FlightId)).OrderBy(q => q.STD).ToList();
                var fids = legs.Select(q => (Nullable<int>)q.FlightId).ToList();
                var _crews2 = (from x in context.ViewFlightCrewNews
                               where fids.Contains(x.FlightId)
                               orderby x.IsPositioning, x.GroupOrder
                               select new CLJLData()
                               {
                                   CrewId = x.CrewId,
                                   IsPositioning = x.IsPositioning,
                                   PositionId = x.PositionId,
                                   Position = x.Position,
                                   Name = x.Name,
                                   GroupId = x.GroupId,
                                   JobGroup = x.JobGroup,
                                   JobGroupCode = x.JobGroupCode,
                                   GroupOrder = x.GroupOrder,
                                   IsCockpit = x.IsCockpit,
                                   FlightId = x.FlightId,
                               }).ToList();

                var _gcrews = (from x in _crews2
                               group x by new
                               {
                                   x.CrewId,
                                   x.IsPositioning,
                                   x.PositionId,
                                   x.Position,
                                   x.Name,
                                   x.GroupId,
                                   x.JobGroup,
                                   x.JobGroupCode,
                                   x.GroupOrder,
                                   x.IsCockpit,
                               } into grp
                               select grp).ToList();

                var query = (from x in _gcrews
                             let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                             select new CLJLData()
                             {
                                 CrewId = x.Key.CrewId,
                                 IsPositioning = x.Key.IsPositioning,
                                 PositionId = x.Key.PositionId,
                                 Position = x.Key.Position,
                                 Name = x.Key.Name,
                                 GroupId = x.Key.GroupId,
                                 JobGroup = x.Key.JobGroup,
                                 JobGroupCode = x.Key.JobGroupCode,
                                 GroupOrder = x.Key.GroupOrder,
                                 IsCockpit = x.Key.IsCockpit,
                                 Legs = legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList(),
                                 LegsStr = string.Join("-", legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList()),
                                 TotalBlockTime = legs.Where(q => xfids.Contains((int)q.FlightId)).Sum(q => q.BlockOff != null && q.BlockOn != null
                                      ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes
                                      : (Nullable<double>)null),
                             }).ToList();

                return new DataResponse { IsSuccess = true, Data = query };
            }
            catch (Exception ex)
            {
                return new DataResponse { IsSuccess = false, Data = ex };
            }
        }

        public class discretion_dto
        {
            public int id { get; set; }
            public int fdp_id { get; set; }
            public string commander_report { get; set; }
            public DateTime? pic_date_sign { get; set; }
            public DateTime? date_create { get; set; }
            public List<form_discretion_item> items { get; set; }
        }

        public class discretion_dto2
        {
            public int id { get; set; }
            public int fdp_id { get; set; }
            public string commander_report { get; set; }
            public DateTime? pic_date_sign { get; set; }
            public DateTime? date_create { get; set; }
            public List<discretion_item_dto> items { get; set; }
        }

        public class discretion_item_dto
        {
            public int id { get; set; }
            public int form_id { get; set; }
            public int item_id { get; set; }
            public string remark { get; set; }

        }

        public class item_dto
        {

            public int item_id { get; set; }
            public string title { get; set; }

        }


        [Route("api/get/discertion/items")]
        [AcceptVerbs("GET")]
        public async Task<DataResponse> GetDiscertionItems()
        {
            dbEntities context = new dbEntities();
            List<item_dto> items = new List<item_dto>();

            var result = context.Options.Where(q => q.ParentId == 300064).ToList();
            foreach (var x in result)
            {
                item_dto item = new item_dto();
                item.item_id = x.Id;
                item.title = x.Title;
                items.Add(item);
            }

            return new DataResponse()
            {
                IsSuccess = true,
                Data = items
            };
        }




        [Route("api/get/discertion/{fdp_id}")]
        [AcceptVerbs("GET")]
        public async Task<DataResponse> GetDiscertion(int fdp_id)
        {

            dbEntities context = new dbEntities();

            var entity = new discretion_dto2();
            List<form_discretion_item> items = new List<form_discretion_item>();
            entity.items = new List<discretion_item_dto>();
            var result = await context.form_discretion.FirstOrDefaultAsync(q => q.fdp_id == fdp_id);
            if (result != null)
            {
                items = context.form_discretion_item.Where(q => q.form_id == result.id).ToList();

                entity.id = result.id;
                entity.fdp_id = result.fdp_id;
                entity.commander_report = result.commander_report;
                entity.pic_date_sign = result.pic_sign_date;
                entity.date_create = result.date_create;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var x = new discretion_item_dto();
                        x.id = item.id;
                        x.form_id = item.form_id;
                        x.remark = item.remark;
                        x.item_id = item.item_id;

                        entity.items.Add(x);
                    }
                }
            }
            return new DataResponse()
            {
                IsSuccess = true,
                Data = entity
            };
        }


        [Route("api/save/discretion")]
        [AcceptVerbs("POST")]
        public async Task<DataResponse> SaveDiscretion(discretion_dto dto)
        {
            try
            {
               

                dbEntities context = new dbEntities();
               
                var itemsToDelete = new List<form_discretion_item>();
                form_discretion entity = await context.form_discretion.FirstOrDefaultAsync(q => q.id == dto.id);
                if (entity != null)
                {
                    itemsToDelete = context.form_discretion_item
       .Where(item => item.form_id == entity.id)
       .ToList();
                }
                 if (entity == null)
                {
                    entity = new form_discretion();
                    context.form_discretion.Add(entity);
                }

                entity.fdp_id = dto.fdp_id;
                entity.commander_report = dto.commander_report;
                entity.date_create = DateTime.Now;
                entity.pic_sign_date = DateTime.Now;
                if (dto.items != null)
                {
                    if (itemsToDelete != null)
                        context.form_discretion_item.RemoveRange(itemsToDelete);
                    foreach (var item in dto.items)
                    {
                        entity.form_discretion_item.Add(item);
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = entity.id,
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " INNER: " + ex.InnerException.Message;
                return new DataResponse() { IsSuccess = false, Data = msg };
            }
        }

    }
}