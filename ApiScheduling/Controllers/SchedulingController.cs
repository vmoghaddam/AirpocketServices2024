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
using ApiScheduling.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using Spire.Xls;
using ApiScheduling.ViewModel;
using System.Diagnostics;
using System.Globalization;

namespace ApiScheduling.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SchedulingController : ApiController
    {



        ///// QA
        ///// VIEW MODELS
        //public class dto_qa_risk_assessment
        //{


        //    public int id { get; set; }
        //    public Nullable<int> form_type { get; set; }
        //    public Nullable<int> form_id { get; set; }
        //    public Nullable<System.DateTime> id_date { get; set; }
        //    public string id_department { get; set; }
        //    public string id_risk_register_number { get; set; }
        //    public string id_hazard_description { get; set; }
        //    public string id_hazard_consequence { get; set; }
        //    public Nullable<int> initial_prob_level { get; set; }
        //    public string initial_severity_level { get; set; }
        //    public string initial_index { get; set; }
        //    public Nullable<int> final_prob_level { get; set; }
        //    public string final_severity_level { get; set; }
        //    public string final_index { get; set; }
        //    public string approval_relevant_department { get; set; }
        //    public Nullable<int> approval_relevant_department_id { get; set; }
        //    public Nullable<System.DateTime> approval_relevant_department_date { get; set; }
        //    public string approval_qa { get; set; }
        //    public Nullable<int> approval_qa_id { get; set; }
        //    public Nullable<System.DateTime> approval_qa_date { get; set; }
        //    public string om_form_no { get; set; }
        //    public string om_form_date { get; set; }
        //    public string om_form_rev { get; set; }
        //    public string om_form_issue { get; set; }
        //    public string remark { get; set; }
        //    public string id_source { get; set; }
        //    public Nullable<bool> has_relevant { get; set; }

        //    public string initial_acceptability { get; set; }
        //    public string initital_description { get; set; }
        //    public string initial_responsible_manager { get; set; }
        //    public Nullable<System.DateTime> initial_responsible_sign { get; set; }
        //    public string initial_qa_approval { get; set; }
        //    public Nullable<System.DateTime> initial_qa_sign { get; set; }
        //    public string final_acceptability { get; set; }



        //    public List<dto_qa_consequence> consequences { get; set; }

        //    public List<dto_qa_corrective_action> actions { get; set; }

        //    public List<dto_qa_monitor> monitors { get; set; }

        //    public List<dto_qa_risk_acceptability> acceptabilities { get; set; }

        //    public List<dto_qa_root_cause> root_causes { get; set; }

        //}
        //public class dto_qa_consequence
        //{
        //    public int id { get; set; }
        //    public Nullable<int> risk_id { get; set; }
        //    public string title { get; set; }


        //}
        //public class dto_qa_corrective_action
        //{
        //    public int id { get; set; }
        //    public Nullable<int> risk_id { get; set; }
        //    public string action { get; set; }
        //    public string responsible_staff { get; set; }
        //    public Nullable<int> responsible_staff_id { get; set; }
        //    public Nullable<System.DateTime> time_limit_date { get; set; }
        //    public string time_limit_remark { get; set; }
        //    public Nullable<System.DateTime> date { get; set; }
        //    public string qa_approval { get; set; }
        //    public Nullable<int> qa_approval_id { get; set; }
        //    public Nullable<System.DateTime> qa_approval_date { get; set; }


        //}
        //public class dto_qa_monitor
        //{
        //    public int id { get; set; }
        //    public Nullable<int> risk_id { get; set; }
        //    public Nullable<System.DateTime> date_last_updated { get; set; }
        //    public string cpmment { get; set; }


        //}
        //public class dto_qa_risk_acceptability
        //{
        //    public int id { get; set; }
        //    public Nullable<int> risk_id { get; set; }
        //    public string risk_type { get; set; }
        //    public Nullable<bool> is_control_action { get; set; }
        //    public Nullable<bool> is_no_control_action { get; set; }
        //    public Nullable<bool> is_urgent_control_action { get; set; }
        //    public Nullable<bool> is_stop_operation { get; set; }
        //    public string responsible_manager { get; set; }
        //    public Nullable<int> responsible_manager_id { get; set; }
        //    public Nullable<System.DateTime> responsible_manager_date { get; set; }
        //    public Nullable<System.DateTime> responsible_manager_qa_date { get; set; }
        //    public Nullable<int> responsible_manager_qa_id { get; set; }
        //    public Nullable<int> level_id { get; set; }
        //    public string level_remark { get; set; }



        //}
        //public class dto_qa_root_cause
        //{
        //    public int id { get; set; }
        //    public Nullable<int> risk_id { get; set; }
        //    public string root_cause { get; set; }


        //}
        ///// 
        //[Route("api/sch/qa/register/save")]
        //[AcceptVerbs("POST")]
        //public async Task<IHttpActionResult> PostRegisterSave(dto_qa_risk_assessment dto)
        //{
        //    var context = new Models.dbEntities();

        //    var entity = context.qa_risk_assessment.FirstOrDefault(q => q.id == dto.id);
        //    if (entity == null)
        //    {
        //        entity = new qa_risk_assessment();
        //        context.qa_risk_assessment.Add(entity);
        //    }

        //    // entity.id = dto.id;


        //    entity.form_type = dto.form_type;
        //    entity.form_id = dto.form_id;

        //    entity.id_date = dto.id_date;
        //    entity.id_department = dto.id_department;
        //    entity.id_risk_register_number = dto.id_risk_register_number;
        //    entity.id_hazard_description = dto.id_hazard_description;
        //    entity.id_hazard_consequence = dto.id_hazard_consequence;
        //    entity.initial_prob_level = dto.initial_prob_level;
        //    entity.initial_severity_level = dto.initial_severity_level;
        //    entity.initial_index = dto.initial_index;
        //    entity.final_prob_level = dto.final_prob_level;
        //    entity.final_severity_level = dto.final_severity_level;
        //    entity.final_index = dto.final_index;
        //    entity.approval_relevant_department = dto.approval_relevant_department;
        //    entity.approval_relevant_department_id = dto.approval_relevant_department_id;
        //    entity.approval_relevant_department_date = dto.approval_relevant_department_date;
        //    entity.approval_qa = dto.approval_qa;
        //    entity.approval_qa_id = dto.approval_qa_id;
        //    entity.approval_qa_date = dto.approval_qa_date;
        //    entity.om_form_no = dto.om_form_no;
        //    entity.om_form_date = dto.om_form_date;
        //    entity.om_form_rev = dto.om_form_rev;
        //    entity.om_form_issue = dto.om_form_issue;
        //    entity.remark = dto.remark;
        //    entity.id_source = dto.id_source;
        //    entity.has_relevant = dto.has_relevant;

        //    entity.initial_acceptability = dto.initial_acceptability;
        //    entity.initital_description = dto.initital_description;
        //    entity.initial_responsible_manager = dto.initial_responsible_manager;
        //    entity.initial_responsible_sign = dto.initial_responsible_sign;
        //    entity.initial_qa_approval = dto.initial_qa_approval;
        //    entity.initial_qa_sign = dto.initial_qa_sign;
        //    entity.final_acceptability = dto.final_acceptability;

        //    if (entity.id > 0)
        //    {
        //        var _cons = context.qa_consequence.Where(q => q.risk_id == entity.id).ToList();
        //        context.qa_consequence.RemoveRange(_cons);

        //        var _causes = context.qa_root_cause.Where(q => q.risk_id == entity.id).ToList();
        //        context.qa_root_cause.RemoveRange(_causes);

        //        var _action = context.qa_corrective_action.Where(q => q.risk_id == entity.id).ToList();
        //        context.qa_corrective_action.RemoveRange(_action);

        //       // var _acc = context.qa_risk_acceptability.Where(q => q.risk_id == entity.id).ToList();
        //       // context.qa_risk_acceptability.RemoveRange(_acc);

        //        var _mon = context.qa_monitor.Where(q => q.risk_id == entity.id).ToList();
        //        context.qa_monitor.RemoveRange(_mon);

        //    }

        //    foreach (var q in dto.consequences)
        //    {
        //        entity.qa_consequence.Add(new qa_consequence()
        //        {
        //            title = q.title
        //        });
        //    }
        //    foreach (var q in dto.root_causes)
        //        entity.qa_root_cause.Add(new qa_root_cause()
        //        {
        //            root_cause = q.root_cause
        //        });

        //    foreach (var q in dto.actions)
        //    {
        //        entity.qa_corrective_action.Add(new qa_corrective_action()
        //        {
        //            action = q.action,
        //            date = q.date,
        //            qa_approval = q.qa_approval,
        //            qa_approval_date = q.qa_approval_date,
        //            qa_approval_id = q.qa_approval_id,
        //            responsible_staff = q.responsible_staff,
        //            responsible_staff_id = q.responsible_staff_id,
        //            time_limit_date = q.time_limit_date,
        //            time_limit_remark = q.time_limit_remark,

        //        });
        //    }

        //    //foreach (var q in dto.acceptabilities)
        //    //{
        //    //    entity.qa_risk_acceptability.Add(new qa_risk_acceptability()
        //    //    {
        //    //        is_control_action = q.is_control_action,
        //    //        is_no_control_action = q.is_no_control_action,
        //    //        is_stop_operation = q.is_stop_operation,
        //    //        is_urgent_control_action = q.is_urgent_control_action,
        //    //        level_id = q.level_id,
        //    //        level_remark = q.level_remark,
        //    //        responsible_manager = q.responsible_manager,
        //    //        responsible_manager_date = q.responsible_manager_date,
        //    //        responsible_manager_id = q.responsible_manager_id,
        //    //        responsible_manager_qa_date = q.responsible_manager_qa_date,
        //    //        responsible_manager_qa_id = q.responsible_manager_qa_id,
        //    //        risk_type = q.risk_type,


        //    //    });
        //    //}

        //    foreach (var q in dto.monitors)
        //    {
        //        entity.qa_monitor.Add(new qa_monitor()
        //        {
        //            cpmment = q.cpmment,
        //            date_last_updated = q.date_last_updated,

        //        });
        //    }

        //    context.SaveChanges();

        //    return Ok(entity.id);

        //    // return new DataResponse() { IsSuccess = false };
        //}

        //[Route("api/sch/qa/hazard/logs")]
        //[AcceptVerbs("GET")]
        //public async Task<IHttpActionResult> GetQaHL( )
        //{
        //    var context = new Models.dbEntities();
        //    var result = await context.ViewQaHazardLogs.OrderByDescending(q => q.id).ToListAsync();
        //    return Ok(result);
        //}
        //[Route("api/sch/qa/register/{fid}/{type}")]
        //[AcceptVerbs("GET")]
        //public async Task<IHttpActionResult> GetQaRegister(int fid, int type)
        //{
        //    var context = new Models.dbEntities();

        //    var entity = context.qa_risk_assessment.FirstOrDefault(q => q.form_id == fid && q.form_type == type);
        //    if (entity == null)
        //        return Ok(new dto_qa_risk_assessment()
        //        {
        //            id = -1,
        //            actions=new List<dto_qa_corrective_action>(),
        //             monitors=new List<dto_qa_monitor>(),
        //              acceptabilities=new List<dto_qa_risk_acceptability>(),
        //               root_causes=new List<dto_qa_root_cause>(),
        //                consequences=new List<dto_qa_consequence>(),

        //        });
        //    var dto = new dto_qa_risk_assessment();
        //    dto.id = entity.id;
        //    dto.form_type = entity.form_type;
        //    dto.form_id = entity.form_id;
        //    dto.id_date = entity.id_date;
        //    dto.id_department = entity.id_department;
        //    dto.id_risk_register_number = entity.id_risk_register_number;
        //    dto.id_hazard_description = entity.id_hazard_description;
        //    dto.id_hazard_consequence = entity.id_hazard_consequence;
        //    dto.initial_prob_level = entity.initial_prob_level;
        //    dto.initial_severity_level = entity.initial_severity_level;
        //    dto.initial_index = entity.initial_index;
        //    dto.final_prob_level = entity.final_prob_level;
        //    dto.final_severity_level = entity.final_severity_level;
        //    dto.final_index = entity.final_index;
        //    dto.approval_relevant_department = entity.approval_relevant_department;
        //    dto.approval_relevant_department_id = entity.approval_relevant_department_id;
        //    dto.approval_relevant_department_date = entity.approval_relevant_department_date;
        //    dto.approval_qa = entity.approval_qa;
        //    dto.approval_qa_id = entity.approval_qa_id;
        //    dto.approval_qa_date = entity.approval_qa_date;
        //    dto.om_form_no = entity.om_form_no;
        //    dto.om_form_date = entity.om_form_date;
        //    dto.om_form_rev = entity.om_form_rev;
        //    dto.om_form_issue = entity.om_form_issue;
        //    dto.remark = entity.remark;
        //    dto.id_source = entity.id_source;
        //    dto.has_relevant = entity.has_relevant;

        //    dto.initial_acceptability = entity.initial_acceptability;
        //    dto.initital_description = entity.initital_description;
        //    dto.initial_responsible_manager = entity.initial_responsible_manager;
        //    dto.initial_responsible_sign = entity.initial_responsible_sign;
        //    dto.initial_qa_approval = entity.initial_qa_approval;
        //    dto.initial_qa_sign = entity.initial_qa_sign;
        //    dto.final_acceptability = entity.final_acceptability;


        //    var cons = context.qa_consequence.Where(q => q.risk_id == entity.id).ToList();
        //    dto.consequences = new List<dto_qa_consequence>();
        //    foreach (var q in cons)
        //        dto.consequences.Add(new dto_qa_consequence()
        //        {
        //            id = q.id,
        //            risk_id = q.risk_id,
        //            title = q.title
        //        });
        //    var actions = context.qa_corrective_action.Where(q => q.risk_id == entity.id).ToList();
        //    dto.actions = new List<dto_qa_corrective_action>();
        //    foreach (var q in actions)
        //        dto.actions.Add(new dto_qa_corrective_action()
        //        {
        //            action = q.action,
        //            date = q.date,
        //            id = q.id,
        //            qa_approval = q.qa_approval,
        //            qa_approval_date = q.qa_approval_date,
        //            qa_approval_id = q.qa_approval_id,
        //            responsible_staff = q.responsible_staff,
        //            responsible_staff_id = q.responsible_staff_id,
        //            risk_id = q.risk_id,
        //            time_limit_date = q.time_limit_date,
        //            time_limit_remark = q.time_limit_remark,
        //        });

        //    dto.root_causes = new List<dto_qa_root_cause>();
        //    var root = context.qa_root_cause.Where(q => q.risk_id == entity.id).ToList();
        //    foreach (var x in root)
        //        dto.root_causes.Add(new dto_qa_root_cause()
        //        {
        //            id = x.id,
        //            risk_id = x.risk_id,
        //            root_cause = x.root_cause,
        //        });

        //    //dto.acceptabilities = new List<dto_qa_risk_acceptability>();
        //    //var acc = context.qa_risk_acceptability.Where(q => q.risk_id == entity.id).ToList();
        //    //foreach (var q in acc)
        //    //{
        //    //    dto.acceptabilities.Add(new dto_qa_risk_acceptability()
        //    //    {
        //    //        id = q.id,
        //    //        is_control_action = q.is_control_action,
        //    //        is_no_control_action = q.is_no_control_action,
        //    //        is_stop_operation = q.is_stop_operation,
        //    //        is_urgent_control_action = q.is_urgent_control_action,
        //    //        level_id = q.level_id,
        //    //        level_remark = q.level_remark,
        //    //        responsible_manager = q.responsible_manager,
        //    //        responsible_manager_date = q.responsible_manager_date,
        //    //        responsible_manager_id = q.responsible_manager_id,
        //    //        responsible_manager_qa_date = q.responsible_manager_qa_date,
        //    //        responsible_manager_qa_id = q.responsible_manager_qa_id,
        //    //        risk_id = q.risk_id,
        //    //        risk_type = q.risk_type

        //    //    });
        //    //}

        //    dto.monitors = new List<dto_qa_monitor>();
        //    var monitor = context.qa_monitor.Where(q => q.risk_id == entity.id).ToList();
        //    foreach (var q in monitor)
        //        dto.monitors.Add(new dto_qa_monitor()
        //        {
        //            cpmment = q.cpmment,
        //            date_last_updated = q.date_last_updated,
        //            id = q.id,
        //            risk_id = q.risk_id
        //        });

        //    return Ok(dto);

        //    // return new DataResponse() { IsSuccess = false };
        //}
        ///// 

        [Route("api/test")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTEST()
        {
            var _context = new Models.dbEntities();

            var result = _context.FDPs.Take(10).ToList();
            return Ok(result);

            // return new DataResponse() { IsSuccess = false };
        }

        [Route("api/sch/crew/valid/")]

        //nookp
        public IHttpActionResult GetValidCrewForRoster(DateTime dt)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            try
            {
                var context = new Models.dbEntities();
                var query = context.ViewCrewValidFTLs.ToList();



                // return result.OrderBy(q => q.STD);
                return Ok(query);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                return Ok(msg);
            }


        }

        public int getGroupOrderIndex(string grp, string pos)
        {
            switch (grp)
            {
                case "IP":
                case "TRE":
                case "TRI":
                    return 1;
                case "P1":
                    return 2;
                case "P2":
                    return 3;
                case "ISCCM":
                case "CCE":
                case "CCI":
                    return 4;
                case "SCCM":
                case "SCC":
                    return 5;
                default:
                    return 1000;
            }
        }
        [Route("api/roster/fdps/{crewId}/{year}/{month}")]
        public IHttpActionResult GetCrewDuties(int crewId, int year, int month)
        {
            var context = new Models.dbEntities();
            var query = from x in context.ViewCrewDuties
                        where x.DateStartYear == year && x.DateStartMonth == month && x.CrewId == crewId
                        select x;

            var result = query.OrderBy(q => q.DateStartLocal).ToList();
            return Ok(result);

        }
        [Route("api/pp/fdps/{crewId}/{year}/{month}")]
        public IHttpActionResult GetPPCrewDuties(int crewId, int year, int month)
        {
            var context = new Models.dbEntities();
            var query = from x in context.ViewCrewDutyPulsePockets
                        where x.DateStartYear == year && x.DateStartMonth == month && x.CrewId == crewId
                        select x;

            var result = query.OrderBy(q => q.DateStartLocal).ToList();
            return Ok(result);

        }

        [Route("api/pp/duty/{fdpid}/{type}")]
        public IHttpActionResult GetPPCrewDuties(int fdpid, int type)
        {
            var context = new Models.dbEntities();
            var is_crew_visible = Convert.ToInt32(ConfigurationManager.AppSettings["is_crew_visible"]);
           
            switch (type)
            {
                case 1165:
                    var fdp = context.FDPs.FirstOrDefault(q => q.Id == fdpid);
                    var crew = context.ViewEmployeeLights.Where(q => q.Id == fdp.CrewId).FirstOrDefault();
                    var show_crew = is_crew_visible == 1 || crew.Id==4811 || (  crew.JobGroup != "CCM"  );
                    var flts = context.AppCrewFlights.Where(q => q.FDPId == fdpid).OrderBy(q => q.STD).ToList();
                    var flt_ids = flts.Select(q => q.FlightId).ToList();
                    var crews =!show_crew?null: context.AppCrewFlights.Where(q => flt_ids.Contains(q.FlightId)).Select(q => new { q.Name, q.Position, q.IsPositioning, q.GroupOrder,q.FlightId }).Distinct().OrderBy(q => q.GroupOrder).ToList();
                    return Ok(new { type, flts,crews });

                case 5000:
                    var sessions = context.ViewCourseFDPs.Where(q => q.FDPId == fdpid).OrderBy(q => q.SessionStart).ToList();
                    return Ok(new { type, sessions });
                    break;
                default:
                    var duty = context.ViewCrewDutyTimeLineNews.Where(q => q.Id == fdpid).FirstOrDefault();
                    return Ok(new { type, duty });
                    break;
            }

            

        }
        [Route("api/pp/duty/date/{cid}")]
        public IHttpActionResult GetPPCrewDutiesByDate(int cid,DateTime df,DateTime dt)
        {
            var context = new Models.dbEntities();
            df = df.Date;
            dt = dt.Date.AddDays(1);
            List<object> result = new List<object>();

            var duties = context.ViewCrewDutyTimeLineNews.Where(q => q.CrewId == cid && q.DateStartLocal >= df && q.DateStartLocal < dt).ToList();
            var crew = context.ViewEmployeeLights.Where(q => q.Id == cid).FirstOrDefault();
            var is_crew_visible = Convert.ToInt32(ConfigurationManager.AppSettings["is_crew_visible"]);
            var show_crew = is_crew_visible==1 || cid==4811 || (   crew.JobGroup != "CCM" );
            foreach (var x in duties)
            {
                if (x.DutyType == 1165)
                {
                    var flts = context.AppCrewFlights.Where(q => q.FDPId == x.Id).OrderBy(q => q.STD).ToList();
                    var flt_ids = flts.Select(q => q.FlightId).ToList();
                    var crews =!show_crew? null: context.AppCrewFlights.Where(q => flt_ids.Contains(q.FlightId)).Select(q => new { q.Name, q.Position, q.IsPositioning, q.GroupOrder, q.FlightId }).Distinct().OrderBy(q => q.GroupOrder).ToList();
                    result.Add(new { type = x.DutyType, flts, crews, duty = x ,date=x.DutyDateLocal});

                }
                else if (x.DutyType == 5000)
                {
                    var sessions = context.ViewCourseFDPs.Where(q => q.FDPId == x.Id).OrderBy(q => q.SessionStart).ToList();
                    result.Add(new { type = x.DutyType, sessions, duty = x, date = x.DutyDateLocal });
                }
                else
                {
                    result.Add(new { type = x.DutyType, duty = x, date = x.DutyDateLocal });
                }
            }

            return Ok(result);



        }
        //05-21
        [Route("api/fdps/flight/{ids}")]

        //nookp
        public IHttpActionResult GetFDPsByFlight(string ids)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;

            var context = new Models.dbEntities();
            var fltIds = ids.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
            var query = from x in context.FDPItems
                        join y in context.FDPs on x.FDPId equals y.Id
                        where y.CrewId != null && fltIds.Contains(x.FlightId)
                        orderby y.InitStart
                        select y;
            var result = query.ToList();
            var grps = (from x in result
                        group x by new { x.InitFlts, x.InitRoute } into grp
                        select new
                        {
                            flts = grp.Key.InitFlts.Replace("*1", "(DH)"),
                            route = grp.Key.InitRoute,
                            item = grp.Select(q => new
                            {
                                q.ActualEnd,
                                q.ActualRestTo,
                                q.ActualStart,
                                q.InitScheduleName,
                                q.CrewId,
                                q.InitKey,
                                q.InitNo,
                                q.InitPosition,
                                q.InitHomeBase,
                                q.InitFlights,
                                q.InitFlts,
                                q.InitFromIATA,
                                q.InitToIATA,
                                q.InitRoute,
                                q.InitStart,
                                q.InitEnd,
                                q.InitRestTo,
                                q.OutOfHomeBase,
                                q.InitGroup,
                                q.InitRank,
                                FDPId = q.Id
                            }).Distinct().OrderBy(q => getGroupOrderIndex(q.InitGroup, q.InitPosition)).ToList()
                        }).OrderBy(q => q.flts).ToList();

            return Ok(grps);

        }


        [Route("api/sch/crew/valid/gant/")]

        //nookp
        public IHttpActionResult GetValidCrewForGantt(string rank = "-1")
        {
            try
            {
                //nooz
                //this.context.Database.CommandTimeout = 160;
                /*
                 * switch ($scope.rank) {



                case 'ISCCM,SCCM':
                    _code = 6;
                    break;
                case 'ISCCM':
                    _code = 7;
                    break;
                case 'SCCM':
                    _code = 8;
                    break;
                case 'CCM':
                    _code = 9;
                    break;api/sch/crew/valid/gant

                default:
                    break;
            }
                 */
                var context = new Models.dbEntities();
                var _query = from x in context.ViewCrewValidFTLs select x;
                if (rank != "-1")
                {
                    switch (rank)
                    {
                        case "1":
                            _query = _query.Where(q => q.JobGroup == "TRI" || q.JobGroup == "TRE" || q.JobGroup == "LTC" || q.JobGroup == "P1");
                            break;
                        case "10":
                            _query = _query.Where(q => q.JobGroup == "TRI" || q.JobGroup == "TRE" || q.JobGroup == "LTC" || q.JobGroup == "P1" || q.JobGroup == "P2" || q.JobGroup == "FE");
                            break;
                        case "2":
                            _query = _query.Where(q => q.JobGroup == "P1");
                            break;
                        case "3":
                            _query = _query.Where(q => q.JobGroup == "P2");
                            break;
                        case "4":
                            _query = _query.Where(q => q.JobGroup == "TRE");
                            break;
                        case "5":
                            _query = _query.Where(q => q.JobGroup == "TRI");
                            break;
                        case "6":
                            _query = _query.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "SCC" || q.JobGroup == "CCI" || q.JobGroup == "CCE");
                            break;
                        case "11":
                            _query = _query.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM" || q.JobGroup == "SCC" || q.JobGroup == "CCI"
                            || q.JobGroup == "CCE" || q.JobGroup == "CC");
                            break;
                        case "7":
                            _query = _query.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "CCI" || q.JobGroup == "CCE");
                            break;
                        case "8":
                            _query = _query.Where(q => q.JobGroup == "SCCM" || q.JobGroup == "SCC");
                            break;
                        case "9":
                            _query = _query.Where(q => q.JobGroup == "CCM" || q.JobGroup == "CC");
                            break;
                        case "12":
                            _query = _query.Where(q => q.JobGroup == "FE");
                            break;
                        default:
                            break;
                    }
                }

                var query = _query.ToList();
                var resources = (from x in query
                                 group x by new { x.Id, x.ScheduleName, x.JobGroup, x.GroupOrder, x.BaseAirportId } into grp
                                 select new
                                 {
                                     CrewId = grp.Key.Id,
                                     id = grp.Key.Id,
                                     grp.Key.BaseAirportId,

                                     grp.Key.ScheduleName,
                                     text = "(" + grp.Key.JobGroup + ")" + grp.Key.ScheduleName,
                                     grp.Key.JobGroup,
                                     grp.Key.GroupOrder,
                                     item = grp.FirstOrDefault()
                                 }).OrderBy(q => q.GroupOrder).ThenBy(q => q.ScheduleName).ToList();



                // return result.OrderBy(q => q.STD);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }


        }


        [Route("api/sch/crew/duties/gant/")]

        //nookp
        public IHttpActionResult GetValidCrewForGantt(DateTime df, DateTime dt, string rank = "-1")
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);
            df = df.AddMinutes(-utcdiff);
            dt = dt.AddDays(1);
            var context = new Models.dbEntities();
            var _query = from x in context.ViewCrewDutyTimeLineNews select x;
            if (rank != "-1")
            {
                switch (rank)
                {
                    case "1":
                        _query = _query.Where(q => q.JobGroup == "TRI" || q.JobGroup == "TRE" || q.JobGroup == "LTC" || q.JobGroup == "P1");
                        break;
                    case "2":
                        _query = _query.Where(q => q.JobGroup == "P1");
                        break;
                    case "3":
                        _query = _query.Where(q => q.JobGroup == "P2");
                        break;
                    case "4":
                        _query = _query.Where(q => q.JobGroup == "TRE");
                        break;
                    case "5":
                        _query = _query.Where(q => q.JobGroup == "TRI");
                        break;
                    case "6":
                        _query = _query.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCI");
                        break;
                    case "7":
                        _query = _query.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "CCI");
                        break;
                    case "8":
                        _query = _query.Where(q => q.JobGroup == "SCCM");
                        break;
                    case "9":
                        _query = _query.Where(q => q.JobGroup == "CCM" || q.JobGroup == "CCE");
                        break;
                    default:
                        break;
                }
            }

            var query = _query.Where(q => q.DateStart >= df && q.DateStart < dt).ToList();
            var duties = (from x in query
                          group x by new { x.CrewId } into grp
                          select new
                          {
                              grp.Key.CrewId,
                              Items = grp.ToList(),
                          }).ToList();




            // return result.OrderBy(q => q.STD);
            return Ok(duties);

        }


        [Route("api/sch/crew/duties/gant/crew")]

        //nookp
        public IHttpActionResult GetValidCrewForGantt(DateTime df, DateTime dt, int crewid)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);
            df = df.AddMinutes(-utcdiff);
            dt = dt.AddDays(1);
            var context = new Models.dbEntities();
            var _query = from x in context.ViewCrewDutyTimeLineNews where x.CrewId == crewid select x;


            var query = _query.Where(q => q.DateStart >= df && q.DateStart < dt).ToList();
            var duties = (from x in query
                          group x by new { x.CrewId } into grp
                          select new
                          {
                              grp.Key.CrewId,
                              Items = grp.ToList(),
                          }).ToList();




            // return result.OrderBy(q => q.STD);
            return Ok(duties);

        }

        [Route("api/sch/flts/gant/")]

        //nookp
        public IHttpActionResult GetSCHFltsForGantt(DateTime df, DateTime dt)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            dt = dt.AddDays(1);
            var context = new Models.dbEntities();
            var query = context.SchFlights.Where(q => q.STDLocal >= df && q.STDLocal < dt).ToList();
            var result = (from x in query
                          group x by new { x.ACType, x.ACTypeId, x.Register, x.RegisterID } into grp
                          select new
                          {
                              grp.Key.ACType,
                              grp.Key.ACTypeId,
                              grp.Key.Register,
                              grp.Key.RegisterID,
                              Flights = grp.OrderBy(q => q.STD).ToList()
                          }).ToList();

            return Ok(new { result, flights = query });

        }


        [Route("api/sch/flts/date/")]

        //nookp
        public IHttpActionResult GetSCHFlts(DateTime df)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var dt = df.AddDays(1);
            var context = new Models.dbEntities();
            var query = context.SchFlights.Where(q => q.STDLocal >= df && q.STDLocal <= dt).OrderBy(q => q.FlightNumber).ToList();
            //var result = (from x in query
            //              group x by new { x.ACType, x.ACTypeId, x.Register, x.RegisterID } into grp
            //              select new
            //              {
            //                  grp.Key.ACType,
            //                  grp.Key.ACTypeId,
            //                  grp.Key.Register,
            //                  grp.Key.RegisterID,
            //                  Flights = grp.OrderBy(q => q.STD).ToList()
            //              }).ToList();

            return Ok(query);

        }

        [Route("api/sch/flight/crews/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightCrews(int id)
        {
            try
            {
                var _context = new Models.dbEntities();
                var result = _context.XFlightCrews.Where(q => q.FlightId == id).OrderBy(q => q.IsPositioning).ThenBy(q => q.GroupOrder).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER:" + ex.InnerException.Message;
                return Ok(new List<Models.XFlightCrew> { new Models.XFlightCrew() { Name = msg } });
            }

        }

        [Route("api/sch/roster/fdps/")]

        //nookp
        public async Task<IHttpActionResult> GetCrewDuties(DateTime df, DateTime dt)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            try
            {
                var context = new Models.dbEntities();
                df = df.Date;
                dt = dt.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                var _start = df.ToUniversalTime();
                var _end = dt.ToUniversalTime();
                var fdps = await context.FDPs.Where(q => q.DutyType == 1165 && q.CrewId != null && q.InitStart >= _start && q.InitStart <= _end
                 && !string.IsNullOrEmpty(q.Key)
                ).ToListAsync();
                var _ids = fdps.Select(q => q.Id).ToList();
                //  var viewfdps=await this.context.ViewFDPRests.Where(q => _ids.Contains(q.Id)).ToListAsync();
                var result = new List<RosterFDPDto>();
                foreach (var x in fdps)
                {
                    //   var rfdp = viewfdps.FirstOrDefault(q => q.Id == x.Id);
                    var item = new RosterFDPDto()
                    {
                        Id = x.Id,
                        crewId = (int)x.CrewId,
                        flts = x.InitFlts,
                        from = Convert.ToInt32(x.InitFromIATA),
                        group = x.InitGroup,
                        homeBase = (int)x.InitHomeBase,
                        index = (int)x.InitIndex,
                        key = x.Key.Replace("*0", ""),
                        no = x.InitNo,
                        rank = x.InitRank,
                        route = x.InitRoute,
                        scheduleName = x.InitScheduleName,
                        to = Convert.ToInt32(x.InitToIATA),
                        flights = x.InitFlights.Split('*').ToList(),

                    };
                    // if (rfdp!=null)
                    //  {
                    //     item.IsSplitDuty = rfdp.ExtendedBySplitDuty > 0;
                    //     item.SplitValue = rfdp.DelayAmount;
                    // }
                    item.ids = new List<RosterFDPId>();
                    foreach (var f in item.flights)
                    {
                        var prts = f.Split('_').ToList();
                        item.ids.Add(new RosterFDPId() { id = Convert.ToInt32(prts[0]), dh = Convert.ToInt32(prts[1]) });
                    }
                    result.Add(item);

                }

                return Ok(result);

            }
            catch(Exception ex)
            {
                 var msg = ex.Message;
                 if (ex.InnerException != null)
                     msg += "     " + ex.InnerException.Message;
                return Ok(msg);
            }
        }


        [Route("api/sch/fdp/index/")]
        [AcceptVerbs("GET")]
        //nookp
        public async Task<IHttpActionResult> GetFDPIndex(string key, string pos)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;

            var context = new Models.dbEntities();
            var fdp = await context.FDPs.Where(q => q.Key == key && q.InitRank == pos && q.IsTemplate == false).OrderByDescending(q => q.InitIndex).FirstOrDefaultAsync();
            if (fdp == null)
                return Ok(1);
            else

                return Ok(fdp.InitIndex + 1);


        }


        [Route("api/fdp/log")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetFDPLog(DateTime dt, DateTime df)
        {
            var _context = new Models.dbEntities();
            var _dt = dt.Date.AddDays(1);
            var _df = df.Date;

            var query = from x in _context.ViewFDPLogs
                        where x.DateAction >= _df && x.DateAction <= _dt
                        select x;

            var result = await query.OrderBy(q => q.DateAction).ToListAsync();
            return Ok(result);

            // return new DataResponse() { IsSuccess = false };
        }


        [Route("api/event/group/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> SaveEventGroup(eventDto dto)
        {
            var ctx = new Models.dbEntities();
            var df = toDate(dto.df, dto.tf);
            var dt = toDate(dto.dt, dto.tt);
            df = df.AddMinutes(-210);
            dt = dt.AddMinutes(-210);
            foreach (var id in dto.ids)
            {
                var duty = new FDP();
                duty.DateStart = df;
                duty.DateEnd = dt;
                duty.CityId = null;
                duty.CrewId = id;
                duty.DutyType = dto.type;
                duty.GUID = Guid.NewGuid();
                duty.IsTemplate = false;
                duty.Remark = dto.remark != null ? Convert.ToString(dto.remark) : "";
                duty.UPD = 1;
                duty.InitStart = duty.DateStart;
                duty.InitEnd = duty.DateEnd;
                duty.InitRestTo = duty.DateEnd;
                ctx.FDPs.Add(duty);


            }

            //  var duty = new FDP();
            //  DateTime _date = Convert.ToDateTime(dto.DateStart);
            // _date = _date.Date;



            // var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003, 300010 };
            // duty.InitRestTo = rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;


            await ctx.SaveChangesAsync();

            return Ok(true);
        }

        private string FormatTwoDigits(Int32 i)
        {
            string functionReturnValue = null;
            if (10 > i)
            {
                functionReturnValue = "0" + i.ToString();
            }
            else
            {
                functionReturnValue = i.ToString();
            }
            return functionReturnValue;
        }

        public string GetDutyTypeTitle(int t)
        {
            switch (t)
            {
                case 1165: return "FDP";
                case 1166: return "Day Off";
                case 1167: return "STBY";
                case 1168: return "STBY";
                case 1169: return "Vacation";
                case 1170: return "Reserve ";
                case 5000: return "Training";
                case 5001: return "Office";
                case 10000: return "RERRP/DayOff";
                case 10001: return "RERRP2/DayOff";
                case 100000: return "Ground";
                case 100001: return "Meeting";
                case 100002: return "Sick";
                case 100003: return "Simulator";
                case 100004: return "Expired Licence";
                case 100005: return "Expired Medical";
                case 100006: return "Expired Passport";
                case 100007: return "No Flight";
                case 100008: return "Requested Off";
                case 100009: return "Refuse";
                case 100020: return "Canceled(Rescheduling)";
                case 100021: return "Canceled(Flight cancellation)";
                case 100022: return "Canceled(Change of A/C Type)";
                case 100023: return "Canceled(FTL)";
                case 100024: return "Canceled(Not using Split Duty)";
                case 100025: return "Mission";
                case 300014: return "Briefing";
                default:
                    return "Unknown";
            }
        }
        //internal async Task<object> RemoveItemsFromFDPByRegisterChange(string crews, string flts)
        //{
        //    var crewIds = crews.Split('*').Select(q => Convert.ToInt32(q)).Distinct().ToList();
        //    foreach (var cid in crewIds)
        //    {
        //        var result = await RemoveItemsFromFDP(flts, cid, 3, "", 1, 0);
        //    }
        //    return true;
        //}
        internal List<RosterFDPDto> getRosterFDPDtos(List<FDP> fdps)
        {

            var result = new List<RosterFDPDto>();
            foreach (var x in fdps)
            {
                var item = new RosterFDPDto()
                {
                    Id = x.Id,
                    crewId = (int)x.CrewId,
                    flts = x.InitFlts,
                    from = Convert.ToInt32(x.InitFromIATA),
                    group = x.InitGroup,
                    homeBase = (int)x.InitHomeBase,
                    index = (int)x.InitIndex,
                    key = x.Key.Replace("*0", ""),
                    no = x.InitNo,
                    rank = x.InitRank,
                    route = x.InitRoute,
                    scheduleName = x.InitScheduleName,
                    to = Convert.ToInt32(x.InitToIATA),
                    flights = x.InitFlights.Split('*').ToList(),



                };
                item.ids = new List<RosterFDPId>();
                foreach (var f in item.flights)
                {
                    var prts = f.Split('_').ToList();
                    item.ids.Add(new RosterFDPId() { id = Convert.ToInt32(prts[0]), dh = Convert.ToInt32(prts[1]) });
                }
                result.Add(item);

            }
            return result;
        }
        // internal async Task<CustomActionResult> RemoveItemsFromFDP(string strItems, int crewId, int reason, string remark, int notify, int noflight, string username = "")
        [Route("api/flights/off")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostFlightsOff(dynamic dto)
        {
            var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);
            double default_reporting = Convert.ToInt32(ConfigurationManager.AppSettings["reporting"]);
            var post_flight = Convert.ToInt32(ConfigurationManager.AppSettings["post_flight"]);
            var context = new Models.dbEntities();
            //var _fdpItemIds = strItems.Split('*').Select(q => Convert.ToInt32(q)).Distinct().ToList();
            //var _fdpIds = strfdps.Split('*').Select(q => Convert.ToInt32(q)).Distinct().ToList();
            string strItems = Convert.ToString(dto.flights);
            int crewId = Convert.ToInt32(dto.crewId);
            int reason = Convert.ToInt32(dto.reason);
            string remark = Convert.ToString(dto.remark);
            string remark2 = Convert.ToString(dto.remark2);
            int notify = Convert.ToInt32(dto.notify);
            int noflight = Convert.ToInt32(dto.noflight);
            string username = Convert.ToString(dto.UserName);
            // var result = await unitOfWork.FlightRepository.RemoveItemsFromFDP(strFlights, crewId, reason, remark, notify, noflight, username);

            var flightIds = strItems.Split('*').Select(q => (Nullable<int>)Convert.ToInt32(q)).Distinct().ToList();
            var _fdpItemIds = await context.ViewFDPItem2.Where(q => q.CrewId == crewId && flightIds.Contains(q.FlightId)).OrderBy(q => q.STD).Select(q => q.Id).ToListAsync();
            var allRemovedItems = await context.FDPItems.Where(q => _fdpItemIds.Contains(q.Id)).ToListAsync();
            var _fdpIds = allRemovedItems.Select(q => q.FDPId).ToList();
            var fdps = await context.FDPs.Where(q => _fdpIds.Contains(q.Id)).ToListAsync();
            var fdpItems = await context.FDPItems.Where(q => _fdpIds.Contains(q.FDPId)).ToListAsync();


            var allFlightIds = fdpItems.Select(q => q.FlightId).ToList();
            var allFlights = await context.SchFlights.Where(q => allFlightIds.Contains(q.ID)).OrderBy(q => q.STD).ToListAsync();
            var crews = await context.ViewEmployeeLights.Where(q => q.Id == crewId).ToListAsync();
            var allRemovedFlights = allFlights.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).ToList();
            FDP offFDP = null;
            string offSMS = string.Empty;
            List<string> sms = new List<string>();
            List<string> nos = new List<string>();
            List<CrewPickupSM> csms = new List<CrewPickupSM>();
            if (reason != -1)
            {


                offFDP = new FDP()
                {
                    CrewId = crewId,
                    DateStart = allRemovedFlights.First().STD,
                    DateEnd = allRemovedFlights.Last().STA,
                    InitStart = allRemovedFlights.First().STD,
                    InitEnd = allRemovedFlights.Last().STA,

                    InitRestTo = allRemovedFlights.Last().STA,
                    InitKey = allRemovedFlights.First().ID.ToString(),
                    DutyType = 0,
                    GUID = Guid.NewGuid(),
                    IsTemplate = false,
                    Remark = remark,
                    Remark2 = remark2,
                    UPD = 1,
                    UserName = username


                };
                offFDP.CanceledNo = string.Join(",", allRemovedFlights.Select(q => q.FlightNumber));
                offFDP.CanceledRoute = string.Join(",", allRemovedFlights.Select(q => q.FromAirportIATA)) + "," + allRemovedFlights.Last().ToAirportIATA;
                offFDP.DutyType = 100020;
                offFDP.Remark2 = remark2;
                //switch (reason)
                //{
                //    case 1:
                //        offFDP.DutyType = 100009;
                //        offFDP.Remark2 = "Refused by Crew";
                //        break;
                //    case 5:
                //        offFDP.DutyType = 100020;
                //        offFDP.Remark2 = "Cenceled due to Rescheduling";
                //        break;
                //    case 2:
                //        offFDP.DutyType = 100021;
                //        offFDP.Remark2 = "Cenceled due to Flight(s) Cancellation";
                //        break;
                //    case 3:
                //        offFDP.DutyType = 100022;
                //        offFDP.Remark2 = "Cenceled due to Change of A/C Type";
                //        break;
                //    case 4:
                //        offFDP.DutyType = 100023;
                //        offFDP.Remark2 = "Cenceled due to Flight/Duty Limitations";
                //        break;
                //    case 6:
                //        offFDP.DutyType = 100024;
                //        offFDP.Remark2 = "Cenceled due to Not using Split Duty";
                //        break;


                //    case 7:
                //        offFDP.DutyType = 200000;
                //        offFDP.Remark2 = "Refused-Not Home";
                //        break;
                //    case 8:
                //        offFDP.DutyType = 200001;
                //        offFDP.Remark2 = "Refused-Family Problem";
                //        break;
                //    case 9:
                //        offFDP.DutyType = 200002;
                //        offFDP.Remark2 = "Canceled - Training";
                //        break;
                //    case 10:
                //        offFDP.DutyType = 200003;
                //        offFDP.Remark2 = "Ground - Operation";
                //        break;
                //    case 11:
                //        offFDP.DutyType = 200004;
                //        offFDP.Remark2 = "Ground - Expired License";
                //        break;
                //    case 12:
                //        offFDP.DutyType = 200005;
                //        offFDP.Remark2 = "Ground - Medical";
                //        break;
                //    default:
                //        break;
                //}
                foreach (var x in allRemovedFlights)
                {
                    var _ofdpitem = fdpItems.FirstOrDefault(q => q.FlightId == x.ID);
                    string _oremark = string.Empty;
                    if (_ofdpitem != null)
                    {
                        var _ofdp = fdps.Where(q => q.Id == _ofdpitem.FDPId).FirstOrDefault();
                        if (_ofdp != null)
                            _oremark = _ofdp.InitRank;
                    }
                    offFDP.OffItems.Add(new OffItem() { FDP = offFDP, FlightId = x.ID, Remark = _oremark });
                }

                context.FDPs.Add(offFDP);



                var strs = new List<string>();
                strs.Add(ConfigurationManager.AppSettings["airline"] + " Airlines");
                strs.Add("Dear " + crews.FirstOrDefault(q => q.Id == crewId).Name + ", ");
                strs.Add("Canceling Notification");
                var day = ((DateTime)allRemovedFlights.First().STDLocal).Date;
                var dayStr = day.ToString("ddd") + " " + day.Year + "-" + day.Month + "-" + day.Day;
                strs.Add(dayStr);
                strs.Add(offFDP.CanceledNo);
                strs.Add(offFDP.CanceledRoute);
                strs.Add(offFDP.Remark2);
                strs.Add(remark);
                strs.Add("Date sent: " + DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd HH:mm"));
                strs.Add("Crew Scheduling Department");
                offSMS = String.Join("\n", strs);
                sms.Add(offSMS);
                nos.Add(crews.FirstOrDefault(q => q.Id == crewId).Mobile);

                var csm = new CrewPickupSM()
                {
                    CrewId = (int)crewId,
                    DateSent = DateTime.Now,
                    DateStatus = DateTime.Now,
                    FlightId = -1,
                    Message = offSMS,
                    Pickup = null,
                    RefId = "",
                    Status = "",
                    Type = offFDP.DutyType,
                    FDP = offFDP,
                    DutyType = offFDP.Remark2,
                    DutyDate = ((DateTime)offFDP.InitStart).ToLocalTime().Date,
                    Flts = offFDP.CanceledNo,
                    Routes = offFDP.CanceledRoute
                };
                csms.Add(csm);
                if (notify == 1)
                    context.CrewPickupSMS.Add(csm);
            }

            /////////////////////////
            //var fdps = await this.context.FDPs.Where(q => _fdpIds.Contains(q.Id)).ToListAsync();
            //var fdpIds = fdps.Select(q => q.Id).ToList();
            //var crewIds = fdps.Select(q => q.CrewId).ToList();
            //var fdpItems = await this.context.FDPItems.Where(q => fdpIds.Contains(q.FDPId)).ToListAsync();
            //var allFlightIds = fdpItems.Select(q => q.FlightId).ToList();
            //var allFlights = await this.context.ViewLegTimes.Where(q => allFlightIds.Contains(q.ID)).ToListAsync();
            //var crews = await this.context.ViewEmployeeLights.Where(q => crewIds.Contains(q.Id)).ToListAsync();
            //var allRemovedItems = fdpItems.Where(q => _fdpItemIds.Contains(q.Id)).ToList();
            //////////////////////////
            //////////////////////////
            ///////////////////

            foreach (var x in allRemovedItems)
            {
                var xfdp = fdps.FirstOrDefault(q => q.Id == x.FDPId);
                var xcrew = crews.FirstOrDefault(q => q.Id == xfdp.CrewId);
                var xleg = allFlights.FirstOrDefault(q => q.ID == x.FlightId);


            }

            var updatedIds = new List<int>();
            var updated = new List<FDP>();
            var removed = new List<int>();

            Dictionary<int, List<SchFlight>> fdp_flight_col = new Dictionary<int, List<SchFlight>>();
            Dictionary<int, List<FDPItem>> fdp_item_col = new Dictionary<int, List<FDPItem>>();
            //  List<FDP> deleted = new List<FDP>();
            foreach (var fdp in fdps)
            {
                fdp.Split = 0;
                var allitems = fdpItems.Where(q => q.FDPId == fdp.Id).ToList();
                var removedItems = allitems.Where(q => _fdpItemIds.Contains(q.Id)).ToList();
                var remainItems = allitems.Where(q => !_fdpItemIds.Contains(q.Id)).ToList();
                var remainFlightIds = remainItems.Select(q => q.FlightId).ToList();
                if (allitems.Count == removedItems.Count)
                {
                    removed.Add(fdp.Id);
                    context.FDPItems.RemoveRange(removedItems);

                    context.FDPs.Remove(fdp);
                }
                else
                {
                    //Update FDP
                    //doog
                    context.FDPItems.RemoveRange(removedItems);
                    var items = (from x in remainItems
                                 join y in allFlights on x.FlightId equals y.ID
                                 orderby y.STD
                                 select new { fi = x, flt = y }).ToList();
                    fdp_flight_col.Add(fdp.Id, items.Select(q => q.flt).OrderBy(q => q.STD).ToList());
                    fdp_item_col.Add(fdp.Id, items.Select(q => q.fi).ToList());
                    fdp.FirstFlightId = items.First().flt.ID;
                    fdp.LastFlightId = items.Last().flt.ID;
                    fdp.InitStart = ((DateTime)items.First().flt.STD).AddMinutes(-default_reporting);
                    fdp.InitEnd = ((DateTime)items.Last().flt.STA).AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"]));

                    fdp.DateStart = ((DateTime)items.First().flt.STD).AddMinutes(-default_reporting);
                    fdp.DateEnd = ((DateTime)items.Last().flt.STA).AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"]));

                    var rst = 12;
                    if (fdp.InitHomeBase != null && fdp.InitHomeBase != items.Last().flt.ToAirportId)
                        rst = 10;
                    fdp.InitRestTo = ((DateTime)items.Last().flt.STA).AddMinutes(post_flight).AddHours(rst);
                    fdp.InitFlts = string.Join(",", items.Select(q => q.flt).Select(q => q.FlightNumber).ToList());
                    fdp.InitRoute = string.Join(",", items.Select(q => q.flt).Select(q => q.FromAirportIATA).ToList());
                    fdp.InitRoute += "," + items.Last().flt.ToAirportIATA;
                    fdp.InitFromIATA = items.First().flt.FromAirportIATA.ToString();
                    fdp.InitToIATA = items.Last().flt.ToAirportIATA.ToString();
                    fdp.InitNo = string.Join("-", items.Select(q => q.flt).Select(q => q.FlightNumber).ToList());
                    fdp.InitKey = string.Join("-", items.Select(q => q.flt).Select(q => q.ID).ToList());
                    fdp.InitFlights = string.Join("*", items.Select(q => q.flt.ID + "_" + (q.fi.IsPositioning == true ? "1" : "0") + "_" + ((DateTime)q.flt.STDLocal).ToString("yyyyMMddHHmm")
                      + "_" + ((DateTime)q.flt.STALocal).ToString("yyyyMMddHHmm")
                      + "_" + q.flt.FlightNumber + "_" + q.flt.FromAirportIATA + "_" + q.flt.ToAirportIATA).ToList()
                    );

                    var keyParts = new List<string>();
                    keyParts.Add(items[0].flt.ID + "*" + (items[0].fi.IsPositioning == true ? "1" : "0"));
                    var breakGreaterThan10Hours = string.Empty;
                    for (int i = 1; i < items.Count; i++)
                    {

                        keyParts.Add(items[i].flt.ID + "*" + (items[i].fi.IsPositioning == true ? "1" : "0"));
                        var dt = (DateTime)items[i].flt.STD - (DateTime)items[i - 1].flt.STA;
                        var minuts = dt.TotalMinutes;
                        // – (0:30 + 0:15 + 0:45)
                        var brk = minuts - double.Parse(ConfigurationManager.AppSettings["post_flight"]) - 60; //30:travel time, post flight duty:15, pre flight duty:30
                        if (brk >= 600)
                        {
                            //var tfi = tflights.FirstOrDefault(q => q.ID == flights[i].ID);
                            // var tfi1 = tflights.FirstOrDefault(q => q.ID == flights[i - 1].ID);
                            breakGreaterThan10Hours = "The break is greater than 10 hours.";
                        }
                        else
                        if (brk >= 180)
                        {
                            var xfdpitem = allitems.FirstOrDefault(q => q.Id == items[i].fi.Id);
                            xfdpitem.SplitDuty = true;
                            var pair = allitems.FirstOrDefault(q => q.Id == items[i - 1].fi.Id);
                            pair.SplitDuty = true;
                            xfdpitem.SplitDutyPairId = pair.FlightId;
                            fdp.Split += 0.5 * (brk);

                        }

                    }
                    fdp.UPD = fdp.UPD == null ? 1 : ((int)fdp.UPD) + 1;
                    fdp.Key = string.Join("_", keyParts);
                    fdp.UserName = username;
                    //var flights = allFlights.Where(q => remainFlightIds.Contains(q.ID)).OrderBy(q=>q.STD).ToList();
                    updatedIds.Add(fdp.Id);
                    updated.Add(fdp);

                }
            }





            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;

            ////////////////////////////
            //doog2
            foreach (var did in removed)
            {
                context.Database.ExecuteSqlCommand("Delete from TableDutyFDP where FDPId=" + did);
                context.Database.ExecuteSqlCommand("Delete from TableFlightFDP where FDPId=" + did);
            }

            foreach (var uf in updated)
            {
                context.Database.ExecuteSqlCommand("Delete from TableDutyFDP where FDPId=" + uf.Id);
                context.Database.ExecuteSqlCommand("Delete from TableFlightFDP where FDPId=" + uf.Id);

                var flts = fdp_flight_col.FirstOrDefault(q => q.Key == uf.Id).Value;
                var fitems = fdp_item_col.FirstOrDefault(q => q.Key == uf.Id).Value;

                AddToCumDuty(uf);
                AddToCumFlight(uf, flts, fitems);

            }
            /////////////////////////////

            var fdpsIds = fdps.Select(q => q.Id).ToList();
            var maxfdps = await context.HelperMaxFDPs.Where(q => fdpsIds.Contains(q.Id)).ToListAsync();
            var fdpExtras = await context.FDPExtras.Where(q => fdpsIds.Contains(q.FDPId)).ToListAsync();
            context.FDPExtras.RemoveRange(fdpExtras);
            foreach (var x in maxfdps)
            {
                context.FDPExtras.Add(new FDPExtra()
                {
                    FDPId = x.Id,
                    MaxFDP = Convert.ToDecimal(x.MaxFDPExtended),
                });
            }
            saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;
            //if (notify == 1)
            //{
            //    Magfa m = new Magfa();
            //    int c = 0;
            //    foreach (var x in sms)
            //    {
            //        var txt = sms[c];
            //        var no = nos[c];

            //        var smsResult = m.enqueue(1, no, txt)[0];
            //        c++;

            //    }
            //}

            //updated = await this.context.ViewFDPKeys.Where(q => updatedIds.Contains(q.Id)).ToListAsync();

            var result = new
            {
                removed,
                updatedId = updated.Select(q => q.Id).ToList(),
                updated = getRosterFDPDtos(updated)
            };

            return new CustomActionResult(HttpStatusCode.OK, result);
        }

        [Route("api/roster/fdp/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> DeleteFDP(dynamic dto)
        {
            var context = new Models.dbEntities();
            int fdpId = Convert.ToInt32(dto.Id);
            var fdp = await context.FDPs.FirstOrDefaultAsync(q => q.Id == fdpId);
            string username = Convert.ToString(dto.username);
            if (fdp.DutyType==5000 || fdp.DutyType == 100003)
            {
                if (string.IsNullOrEmpty(username))
                    return Ok(-1);

                if ( username.ToLower() != "trn.moradi" && username.ToLower()!= "F.OMIDVAR")
                    return Ok(-1);


            }
            double total = 0;
            if (!string.IsNullOrEmpty(fdp.InitFlights))
            {
                var parts = fdp.InitFlights.Split('*').ToList();

                foreach (var x in parts)
                {
                    var _std = x.Split('_')[2];
                    var _sta = x.Split('_')[3];
                    var std = DateTime.ParseExact(_std, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                    var sta = DateTime.ParseExact(_sta, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                    total += (sta - std).TotalMinutes;
                }
                //this.context.Database.ExecuteSqlCommand("update employee set flightsum=isnull(flightsum,0)-" + total + ",FlightEarly=isnull(FlightEarly,0)+" + early + ",FlightLate=isnull(FlightLate,0)+" + late + "  where id=" + dto.crewId);
                context.Database.ExecuteSqlCommand("update employee set flightsum=isnull(flightsum,0)-" + total + "  where id=" + fdp.CrewId);

            }
            var related = await context.FDPs.Where(q => q.FDPId == fdpId).FirstOrDefaultAsync();
            if (related != null)
            {
                related.FDPId = null;
                related.FDPReportingTime = null;
                related.UPD = related.UPD != null ? related.UPD + 1 : 1;
            }

            if (fdp.FDPId != null)
            {
                var stby = await context.FDPs.FirstOrDefaultAsync(q => q.Id == fdp.FDPId);
                if (stby != null)
                {
                    stby.FDPId = null;
                    stby.FDPReportingTime = null;
                    stby.UPD = stby.UPD == null ? 1 : ((int)stby.UPD) + 1;
                    stby.InitEnd = stby.PLNEnd;
                    stby.InitRestTo = stby.PLNRest;
                    stby.DateEnd = stby.PLNEnd;
                    if (stby.DutyType != 1170)
                        AddToCumDuty(stby, context);
                }
            }


            var templateId = fdp.TemplateId;
            context.FDPs.Remove(fdp);
            context.Database.ExecuteSqlCommand("Delete from TableDutyFDP where FDPId=" + fdp.Id);
            context.Database.ExecuteSqlCommand("Delete from TableFlightFDP where FDPId=" + fdp.Id);

            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;


            return Ok(total);
        }


        [Route("api/update/fdp/flight/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> UpdateFDPByFlight(int id)
        {   //02-05

            //*********************
            //UPDATE FOR ALL DH
            //***********************
            double default_reporting = Convert.ToInt32(ConfigurationManager.AppSettings["reporting"]);
            var context = new Models.dbEntities();
            var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);
            var fdps = await (from x in context.FDPItems
                              join y in context.FDPs on x.FDPId equals y.Id
                              where x.FlightId == id && y.CrewId != null
                              select y).ToListAsync();
            var fdpIds = fdps.Select(q => q.Id).ToList();
            var fdpitems = await context.FDPItems.Where(q => fdpIds.Contains(q.FDPId)).ToListAsync();
            var fltIds = fdpitems.Select(q => q.FlightId).ToList();
            var flts = await context.SchFlights.Where(q => fltIds.Contains(q.ID)).ToListAsync();

            foreach (var fdp in fdps)
            {
                var items = fdpitems.Where(q => q.FDPId == fdp.Id).ToList();
                var notSectorIds = items.Where(q => q.IsPositioning == true || q.IsSector == false || q.IsOff == true).Select(q => q.FlightId).ToList();
                var flt_ids = items.Select(q => q.FlightId).ToList();
                var sectors = items.Where(q => q.IsPositioning != true && q.IsSector != false && q.IsOff != true).Count();
                var flights = flts.Where(q => flt_ids.Contains(q.ID)).OrderBy(q => q.ChocksOut).ToList();
                var reporting = fdp.ReportingTime == null ? fdp.DateStart : fdp.ReportingTime;
                if (fdp.STD == flights.First().STD)
                {
                    //the reporting time does not change

                    reporting = ((DateTime)reporting).AddMinutes(utcdiff);


                }
                else
                {
                    //reporting time changes
                    reporting = ((DateTime)flights.First().ChocksOut).AddMinutes(-default_reporting);
                    fdp.ReportingTime = reporting;
                    fdp.DateStart = reporting;
                    fdp.InitStart = reporting;

                    reporting = ((DateTime)reporting).AddMinutes(utcdiff);
                }


                var maxFdp = GetMaxFDP2((DateTime)reporting, sectors);
                fdp.MaxFDP = maxFdp;
                var fdp_duration = ((DateTime)flights.Last().ChocksIn - (DateTime)reporting).TotalMinutes;
                if (fdp_duration > maxFdp)
                    fdp.IsOver = true;
                //initflts 5824,5825
                //initroute THR,KIH,THR
                var rts = flights.Select(q => q.FromAirportIATA).ToList();
                rts.Add(flights.Last().ToAirportIATA);
                fdp.InitRoute = string.Join(",", rts);
                //initfromiata
                //inittoiata
                fdp.InitFromIATA = flights.First().FromAirportId.ToString();
                fdp.InitToIATA = flights.Last().ToAirportId.ToString();
                //initno 5824_5825
                fdp.InitNo = string.Join("_", flights.Select(q => q.FlightNumber).ToList());
                fdp.InitFlights = string.Join("*", RosterFDPDto.getFlightsStrs(flights, items.Where(q => q.IsPositioning == true).Select(q => q.FlightId).ToList()));
                fdp.DateEnd = ((DateTime)flights.Last().ChocksIn).AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"]));
                fdp.InitEnd = ((DateTime)flights.Last().ChocksIn).AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"]));
                var rst = fdp.InitHomeBase != flights.Last().ToAirportId ? 10 : 12;
                fdp.InitRestTo = ((DateTime)flights.Last().ChocksIn).AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"])).AddHours(rst);
                //initflights 540302_0_202302051740_202302051930_5824_THR_KIH*540303_0_202302052030_202302052215_5825_KIH_THR



                fdp.STD = flights.First().STD;
                fdp.STA = flights.Last().STA;



                AddToCumDuty(fdp, context);
                AddToCumFlight(fdp, flights.Where(q => !notSectorIds.Contains(q.ID)).OrderBy(q => q.ChocksOut).ToList(), items, context);

            }


            // context.Database.ExecuteSqlCommand("Delete from TableFlightFDP where FDPId=" + fdp.Id);

            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;


            return Ok(true);
        }

        [Route("api/duty/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> AddDuty(dynamic dto)
        {

            //1166 dayoff
            //1169 vac 
            //300008 dty
            //100007 no flt
            //100008 req off
            //above items: datestart date 12:00 utc ex: 2023-02-01 12:00
            var context = new Models.dbEntities();
            var extendedRERRP = 0;
            var timeline = 0;

            if (dto.EXTRERRP != null)
                extendedRERRP = Convert.ToInt32(dto.EXTRERRP);
            if (dto.TIMELINE != null)
                timeline = Convert.ToInt32(dto.TIMELINE);
            var duty = new FDP();
            DateTime _date = Convert.ToDateTime(dto.DateStart);
            _date = _date.Date;
            duty.DateStart = Convert.ToDateTime(dto.DateStart);
            duty.DateEnd = Convert.ToDateTime(dto.DateEnd);
            duty.CityId = Convert.ToInt32(dto.CityId) == -1 ? Convert.ToInt32(dto.CityId) : null;
            duty.CrewId = Convert.ToInt32(dto.CrewId);
            duty.DutyType = Convert.ToInt32(dto.DutyType);
            duty.GUID = Guid.NewGuid();
            duty.IsTemplate = false;
            duty.Remark = dto.Remark != null ? Convert.ToString(dto.Remark) : "";
            duty.UPD = 1;

            duty.InitStart = duty.DateStart;
            duty.InitEnd = duty.DateEnd;
            var rest_after_duty = Convert.ToInt32(ConfigurationManager.AppSettings["rest_after_duty"]);
            if (rest_after_duty == 1)
            {
                var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003, 300010, 300014 };
                duty.InitRestTo = rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;
            }
            else
            {
                var rest = new List<int>() { 1167, 1168, 1170 };
                duty.InitRestTo = rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;

            }
            int rerrp_check = Convert.ToInt32(ConfigurationManager.AppSettings["rerrp_check"]);
            if (rerrp_check == 1 && (new List<int>() { 1167, 1168 }).Contains(duty.DutyType))
            {
                var _dtdate = ((DateTime)duty.InitStart).Date;
                var _rerrp = await context.AppFTLs.Where(q => q.CrewId == duty.CrewId && q.CDate == _dtdate && q.RERRP > 0).FirstOrDefaultAsync();
                if (_rerrp == null && (dto.IsAdmin == null || dto.IsAdmin == 0))
                {
                    return new CustomActionResult(HttpStatusCode.OK, new
                    {
                        Code = 308,
                        message = "RERRP Error. "

                    });
                }
            }



            var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);
            var d24s = new List<int>() {   100007, 100008, 100002 };
            if (d24s.IndexOf(duty.DutyType) != -1)
            {
                var _start = ((DateTime)duty.InitStart);//.Date;//.AddMinutes(-utcdiff);
                var _end = _start.AddHours(24);
                duty.DateStart = _start.AddMinutes(1);
                duty.DateEnd = _end.AddMinutes(-1);
                duty.InitStart = duty.DateStart;
                duty.InitEnd = duty.DateEnd;
                duty.InitRestTo = duty.DateEnd;
            }
            if (duty.DutyType == 10000 || duty.DutyType == 10001)
            {
                var rerrp_calculate = Convert.ToInt32(ConfigurationManager.AppSettings["rerrp_calculate"]);
                if (rerrp_calculate == 1)
                {

                    var _start = ((DateTime)duty.InitStart).AddMinutes(utcdiff);
                    DateTime _end;
                    if (extendedRERRP == 0)
                    {
                        if (_start.Hour >= 18)
                        {
                            _end = _start.AddMinutes(36 * 60);
                        }
                        else
                        {
                            _end = _start.AddMinutes(36 * 60);
                            var _lnr = 18 * 60 - (_start.Hour * 60 + _start.Minute);
                            _end = _end.AddMinutes(_lnr);
                        }
                    }
                    else
                    {
                        duty.Remark += "(EXTENDED)";
                        _end = _start.AddMinutes(48 * 60);
                        var _lnr = 24 * 60 - (_start.Hour * 60 + _start.Minute);
                        _end = _end.AddMinutes(_lnr);
                    }
                    duty.DateStart = _start.AddMinutes(-utcdiff);
                    if (duty.DateEnd > _end.AddMinutes(-utcdiff))
                        _end =(DateTime) duty.DateEnd;
                    else
                        _end = _end.AddMinutes(-utcdiff);

                    duty.DateEnd = _end;  
                    duty.InitStart = duty.DateStart;
                    duty.InitEnd = duty.DateEnd;
                    duty.InitRestTo = duty.DateEnd;
                }
                else
                {
                    duty.InitStart = duty.DateStart;
                    duty.InitEnd = duty.DateEnd;
                    duty.InitRestTo = duty.DateEnd;
                }




            }
            if (duty.DutyType == 300050)
            {
                duty.PosAirline = Convert.ToString(dto.airline);
                duty.PosRemark = Convert.ToString(dto.ticket_no);
                duty.PosFrom = Convert.ToString(dto.apt_from);
                duty.PosTo = Convert.ToString(dto.apt_to);



                duty.InitStart = duty.DateStart;
                duty.InitEnd = duty.DateEnd;
                duty.InitRestTo = duty.DateEnd;

                duty.PosDep = duty.DateStart;
                duty.PosArr = duty.DateEnd;



            }
            //  var _bl = Convert.ToInt32(dto.BL);
            //if (_bl != 0)
            //{
            //    duty.TableBlockTimes.Add(new TableBlockTime()
            //    {
            //        BlockTime = _bl,
            //        CDate = _date,
            //        CrewId = duty.CrewId,


            //    });
            //    duty.BL = _bl;
            //}
            //var _fx = Convert.ToInt32(dto.FX);
            //if (_fx != 0)
            //{
            //    duty.FX = _fx;
            //}
            //1166: Dayoff 100003:sim 5000:trn 5001:ofc 10025:mission 300009:rest
            var _interupted = await context.FDPs.FirstOrDefaultAsync(q =>

                                         q.Id != duty.Id && q.CrewId == duty.CrewId
                                         && (

                                               (duty.InitStart >= q.InitStart && duty.InitRestTo <= q.InitRestTo)
                                               || (q.InitStart >= duty.InitStart && q.InitRestTo <= duty.InitRestTo)
                                               || (q.InitStart >= duty.InitStart && q.InitStart < duty.InitRestTo)
                                               || (q.InitRestTo > duty.InitStart && q.InitRestTo <= duty.InitRestTo)
                                             )
                                          );
            var _interupted_norest = await context.FDPs.FirstOrDefaultAsync(q =>

                                            q.Id != duty.Id && q.CrewId == duty.CrewId
                                            && (

                                                  (duty.InitStart >= q.InitStart && duty.InitRestTo <= q.InitEnd)
                                                  || (q.InitStart >= duty.InitStart && q.InitEnd <= duty.InitRestTo)
                                                  || (q.InitStart >= duty.InitStart && q.InitStart < duty.InitRestTo)
                                                  || (q.InitEnd > duty.InitStart && q.InitEnd <= duty.InitRestTo)
                                                )
                                             );
            //var _interupted_for_training = await context.FDPs.FirstOrDefaultAsync(q =>

            //                               q.Id != duty.Id && q.CrewId == duty.CrewId
            //                               && (

            //                                     (duty.InitStart >= q.InitStart && duty.InitRestTo <= q.InitEnd)
            //                                     || (q.InitStart >= duty.InitStart && q.InitEnd <= duty.InitRestTo)
            //                                     || (q.InitStart >= duty.InitStart && q.InitStart < duty.InitRestTo)
            //                                     || (q.InitEnd > duty.InitStart && q.InitEnd <= duty.InitRestTo)
            //                                   )
            //                                );

            switch (duty.DutyType)
            {
                case 100000: //ground

                case 100004: //exp lic
                case 100005: //exp med
                case 100006: //exp pass
                case 100007: //no flt
                    if (_interupted_norest != null &&
                        (_interupted_norest.DutyType == 1165
                        || _interupted_norest.DutyType == 1167
                        || _interupted_norest.DutyType == 1170
                        || _interupted_norest.DutyType == 1168
                        || _interupted_norest.DutyType == 300010))//other airline stby
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 406,
                            message = "Interruption Error." + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                        });
                    break;
                case 100002: //sick
                case 100008: //req off
                case 1166:   //day off
                case 1169:   //vacation
                    if (_interupted_norest != null &&
                       (_interupted_norest.DutyType == 1165
                       || _interupted_norest.DutyType == 1167
                       || _interupted_norest.DutyType == 1170
                       || _interupted_norest.DutyType == 1168
                       || _interupted_norest.DutyType == 300010 //ostby
                       || _interupted_norest.DutyType == 5000
                       || _interupted_norest.DutyType == 5001
                         || _interupted_norest.DutyType == 300014
                       || _interupted_norest.DutyType == 100001 //meeting
                        || _interupted_norest.DutyType == 100025 //mission

                       ))//other airline stby
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 406,
                            message = "Interruption Error." + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                        });
                    break;
                case 5000://trn
                case 5001: //office
                    if (_interupted_norest != null &&
                        (_interupted_norest.DutyType == 1165
                       // || _interupted_norest.DutyType == 1167
                        //|| _interupted_norest.DutyType == 1170
                       // || _interupted_norest.DutyType == 1168
                       // || _interupted_norest.DutyType == 300010)
                        )
                        )//other airline stby
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 406,
                            message = "Interruption Error." + (_interupted_norest.InitStart == null ? "" : ((DateTime)_interupted_norest.InitStart).ToString("yyyy-MM-dd") + " " + _interupted_norest.InitFlts + " " + _interupted_norest.InitRoute)

                        });
                    break;
                   
                case 300014:
                case 100001: //meeting
                    if (_interupted != null &&
                       (_interupted.DutyType == 1165
                       || _interupted.DutyType == 1167
                       || _interupted.DutyType == 1170
                       || _interupted.DutyType == 1168
                       || _interupted.DutyType == 300010 //ostby
                       || _interupted.DutyType == 5000
                        || _interupted.DutyType == 300014
                       || _interupted.DutyType == 5001
                       || _interupted.DutyType == 100001 //meeting
                        || _interupted.DutyType == 100025 //mission
                        || _interupted.DutyType == 100002 //sick
                        || _interupted.DutyType == 100008  //req off
                        || _interupted.DutyType == 1166 //day off
                        || _interupted.DutyType == 1169  //vacation
                        || _interupted.DutyType == 10000  //rerrp
                        || _interupted.DutyType == 10001  //rerrp2
                        || _interupted_norest.DutyType == 300010 //other airline stby
                       ))
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 406,
                            message = "Interruption Error." + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                        });
                    break;
                case 300010://other airline stby
                case 100025://mission
                    var types = new List<int>() { 1165, 1167, 1168, 1170, 5000, 5001, 300014, 100001, 100025, 100002, 100008, 1166, 1169, 10000, 10001 };
                    if (_interupted != null && types.IndexOf(_interupted.DutyType) != -1)
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 406,
                            message = "Interruption Error." + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                        });
                    break;
                case 100003://sim
                    var types2 = new List<int>() { 100003, 1165, 1167, 1168, 1170, 5000, 5001, 300014, 1166, 1169, 10000, 10001, 100001, 300010, 100025, 100008 };
                    if (_interupted_norest != null && types2.IndexOf(_interupted_norest.DutyType) != -1)
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 406,
                            message = "Interruption Error." + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                        });
                    break;
                case 300008://duty
                    var types3 = new List<int>() { 1166, 1169, 10000, 10001, 100002, 100008 };
                    if (_interupted_norest != null && types3.IndexOf(_interupted_norest.DutyType) != -1)
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 406,
                            message = "Interruption Error." + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                        });
                    break;
                case 300009://rest
                    var types4 = new List<int>() { 1165, 1167, 1168, 1170, 100003, 5000, 5001, 300014, 100025, 300008, 100001, 300010 };
                    if (_interupted_norest != null && types4.IndexOf(_interupted_norest.DutyType) != -1)
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 406,
                            message = "Interruption Error." + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                        });
                    break;
                case 10000://rerrp
                case 10001:
                    var types5 = new List<int>() { 1165, 1167, 1168, 1170, 100003, 5000, 5001, 300014, 100025, 300008, 100001, 300010 };
                    if (_interupted_norest != null && types5.IndexOf(_interupted_norest.DutyType) != -1)
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 406,
                            message = "Interruption Error." + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                        });
                    break;
                default:
                    break;
            }



            context.FDPs.Add(duty);
            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;
            AddToCumDuty(duty);

            if (duty.DutyType == 10000 || duty.DutyType == 10001)
            {
                var d1 = (DateTime)duty.DateStart;
                var de = ((DateTime)duty.DateEnd);
                var d2 = (new DateTime(de.Year, de.Month, de.Day, 0, 0, 0)).AddDays(6);
                var sd1 = d1.ToString("yyyy-MM-dd");
                var sd2 = d2.ToString("yyyy-MM-dd");
                context.Database.ExecuteSqlCommand("update ftlsummary set RERRP=isnull(rerrp,0)+1 where CDate>='" + sd1 + "'  and cdate<='" + sd2 + "' and crewid=" + duty.CrewId);
            }


            if (timeline == 0)
            {
                var result = await context.ViewCrewDuties.FirstOrDefaultAsync(q => q.Id == duty.Id);
                return new CustomActionResult(HttpStatusCode.OK, result);
            }
            else
            {
                var result = await context.ViewCrewDutyTimeLineNews.FirstOrDefaultAsync(q => q.Id == duty.Id);
                return new CustomActionResult(HttpStatusCode.OK, result);
            }

        }


        public class rerrp_check_dto
        {
            public int crew_id { get; set; }
            public DateTime dt { get; set; }
        }
        [Route("api/roster/rerrp/check")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> GetRerrpCheck(rerrp_check_dto dto)
        {
            var context = new Models.dbEntities();
            int rerrp_check = Convert.ToInt32(ConfigurationManager.AppSettings["rerrp_check"]);
            if (rerrp_check == 1)
            {
                var _dtdate = dto.dt.Date;
                var _rerrp = await context.AppFTLs.Where(q => q.CrewId == dto.crew_id && q.CDate == _dtdate && q.RERRP > 0).FirstOrDefaultAsync();
                if (_rerrp == null)
                {
                    return new CustomActionResult(HttpStatusCode.OK, new
                    {
                        Code = 308,
                        message = "RERRP Error. "

                    });
                }
                else return new CustomActionResult(HttpStatusCode.OK, new
                {
                    Code = 0,
                    message = "RERRP Error. "

                });
            }
            else
                return new CustomActionResult(HttpStatusCode.OK, new
                {
                    Code = 0,
                    message = "RERRP Error. "

                });
        }

        [Route("api/roster/stby/save")]
        [AcceptVerbs("POST")]
        //goh
        public async Task<IHttpActionResult> PostRosterSTBYSave(dynamic dto)
        {
            var type = Convert.ToInt32(dto.type);
            var context = new Models.dbEntities();
            var sbam_start = 0 * 60;
            var sbam_durattion = 11 * 60 + 59;
            var sbpm_start = 12 * 60;
            var sbpm_duration = 11 * 60 + 59;
            var res_start = 4 * 60;
            var res_duration = 17 * 60 + 59;

            var sbc_start = 6 * 60;
            var sbc_duration = 11 * 60 + 59;


            var _from = string.Empty;
            var _end = string.Empty;
            if (dto.date_from != null)
            {
                _from = Convert.ToString(dto.date_from);
            }
            if (dto.date_end != null)
            {
                _end = Convert.ToString(dto.date_end);
            }
            DateTime day = (Convert.ToDateTime(dto.date));
            //lool
            var start = day;
            var end = day.AddHours(12);
            // var start = day.AddMinutes(sbam_start);
            // var end = start.AddMinutes(sbam_durattion);
            var _from_date = new DateTime();
            if (string.IsNullOrEmpty(_from))
            {
                switch (type)
                {
                    case 1167:
                        _from = day.AddMinutes(sbam_start).ToString("yyyyMMddHHmm");

                        _end = day.AddMinutes(sbam_start).AddMinutes(sbam_durattion).ToString("yyyyMMddHHmm");
                        break;
                    case 1168:
                        _from = day.AddMinutes(sbpm_start).ToString("yyyyMMddHHmm");
                        _end = day.AddMinutes(sbpm_start).AddMinutes(sbam_durattion).ToString("yyyyMMddHHmm");
                        break;
                    case 1170:
                        _from = day.AddMinutes(res_start).ToString("yyyyMMddHHmm");
                        _end = day.AddMinutes(res_start).AddMinutes(res_duration).ToString("yyyyMMddHHmm");
                        break;
                    case 300013:
                        _from = day.AddMinutes(sbc_start).ToString("yyyyMMddHHmm");
                        _end = day.AddMinutes(sbc_start).AddMinutes(sbc_duration).ToString("yyyyMMddHHmm");
                        break;
                    default:
                        break;
                }
            }




            int crewId = Convert.ToInt32(dto.crewId);

            var isgantt = dto.isgantt != null ? Convert.ToInt32(dto.isgantt) : 0;


            int rerrp_check = Convert.ToInt32(ConfigurationManager.AppSettings["rerrp_check"]);
            if (rerrp_check == 1 && (new List<int>() { 1167, 1168 }).Contains(type))
            {
                var _dtdate = ((DateTime)day).Date;
                var _rerrp = await context.AppFTLs.Where(q => q.CrewId == crewId && q.CDate == _dtdate && q.RERRP > 0).FirstOrDefaultAsync();
                if (_rerrp == null && (dto.IsAdmin == null || dto.IsAdmin == 0))
                {
                    return new CustomActionResult(HttpStatusCode.OK, new
                    {
                        Code = 308,
                        message = "RERRP Error. "

                    });
                }
            }

            //if (type == 1167)
            //{
            //    start = day.AddHours(12);
            //    end = start.AddHours(12);
            //}
            //if (type == 1170)
            //{
            //    end = start.AddHours(23).AddMinutes(59).AddSeconds(59);
            //}

            //caspian

            if (type == 1168)
            {

                start = toDateTime(_from).AddMinutes(-210);
                end = toDateTime(_end).AddMinutes(-210);
            }
            if (type == 1167)
            {

                // start = day.AddMinutes(sbpm_start);
                // end = start.AddMinutes(sbpm_duration);
                start = toDateTime(_from).AddMinutes(-210);
                end = toDateTime(_end).AddMinutes(-210);
            }
            if (type == 300013)
            {

                //start = day.AddMinutes(sbc_start);
                // end = start.AddMinutes(sbc_duration);
                start = toDateTime(_from).AddMinutes(-210);
                end = toDateTime(_end).AddMinutes(-210);
            }
            if (type == 1170)
            {

                //start = day.AddMinutes(res_start);
                //end = start.AddMinutes(res_duration);
                start = toDateTime(_from).AddMinutes(-210);
                end = toDateTime(_end).AddMinutes(-210);

            }

            var duty = new FDP();
            duty.DateStart = start;
            duty.DateEnd = end;
            var duration = (end - start).TotalMinutes;
            duty.CityId = Convert.ToInt32(dto.CityId); //Convert.ToInt32(dto.CityId) == -1 ? Convert.ToInt32(dto.CityId) : null;
            var homeBase = Convert.ToInt32(dto.HomeBase);
            var outOfBase = false;
            if (duty.CityId != -1 && homeBase != duty.CityId)
            {
                outOfBase = true;
            }
            var rest = duration >= 720 ? duration : (outOfBase ? 600 : 720);
            duty.CrewId = crewId;
            duty.DutyType = type;
            duty.GUID = Guid.NewGuid();
            duty.IsTemplate = false;
            //duty.Remark = dto.Remark != null ? Convert.ToString(dto.Remark) : "";
            duty.UPD = 1;

            duty.InitStart = duty.DateStart;
            duty.InitEnd = duty.DateEnd;
            // var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003 };
            //duty.InitRestTo = rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;
            if (duty.DutyType == 1167 || duty.DutyType == 1168 || duty.DutyType == 300013)
                duty.InitRestTo = ((DateTime)duty.InitEnd).AddMinutes(rest);
            else
                duty.InitRestTo = duty.DateEnd;
            //porn
            var exc = new List<int>() { 100009, 100020, 100021, 100022, 100023, 1170,5000 };
            var check = new List<int>() { 1165, 1166, 1167, 1168, 300013, 1169,  5001, 100000, 100002, 100003, 100004, 100005, 100006,100007, 100008, 100025, 300008, 300009, 300010, 300014 };
            // var _interupted = await this.context.FDPs.FirstOrDefaultAsync(q => !exc.Contains(q.DutyType) && q.CrewId == duty.CrewId
            // && (
            //       (duty.InitStart >= q.InitStart && duty.InitStart <= q.InitRestTo)
            //    || (duty.InitEnd >= q.InitStart && duty.InitEnd <= q.InitRestTo)
            //    || (q.InitStart >= duty.InitStart && q.InitRestTo <= duty.InitRestTo)
            //   )300051
            //);
            FDP _interupted = null;
            if (duty.DutyType == 1170)
            {
                _interupted = await context.FDPs.FirstOrDefaultAsync(q =>
                                         check.Contains(q.DutyType) &&
                                         q.Id != duty.Id && q.CrewId == duty.CrewId
                                         && (

                                               (duty.InitStart >= q.InitStart && duty.InitRestTo <= q.InitEnd)
                                               || (q.InitStart >= duty.InitStart && q.InitEnd <= duty.InitRestTo)
                                               || (q.InitStart >= duty.InitStart && q.InitStart < duty.InitRestTo)
                                                || (q.InitEnd > duty.InitStart && q.InitEnd <= duty.InitRestTo)
                                             )
                                          );
            }
            else
            {
                _interupted = await context.FDPs.FirstOrDefaultAsync(q =>
                                                         check.Contains(q.DutyType) &&
                                                         q.Id != duty.Id && q.CrewId == duty.CrewId
                                                         && (

                                                               (duty.InitStart >= q.InitStart && duty.InitRestTo <= q.InitRestTo)
                                                               || (q.InitStart >= duty.InitStart && q.InitRestTo <= duty.InitRestTo)
                                                               || (q.InitStart >= duty.InitStart && q.InitStart < duty.InitRestTo)
                                                               || (q.InitRestTo > duty.InitStart && q.InitRestTo <= duty.InitRestTo)
                                                             )
                                                          );
            }
       
            if (_interupted !=null && _interupted.DutyType != 1165)
            {
                if (_interupted.InitStart >= duty.InitEnd)
                    _interupted = null;
            }

            if (_interupted != null)
            {
                //Rest/Interruption Error
                return new CustomActionResult(HttpStatusCode.OK, new
                {
                    Code = 406,
                    message = "Rest/Interruption Error." + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                });
                //return new CustomActionResult(HttpStatusCode.NotAcceptable, _interupted);
            }


            context.FDPs.Add(duty);
            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;
            AddToCumDuty(duty);

            //2020-11-22 noreg

            if (isgantt == 0)
            {
                //var view = await context.ViewCrewDutyNoRegs.FirstOrDefaultAsync(q => q.Id == duty.Id);
                // return new CustomActionResult(HttpStatusCode.OK, view);
                var result = await context.ViewCrewDuties.FirstOrDefaultAsync(q => q.Id == duty.Id);
                return new CustomActionResult(HttpStatusCode.OK, result);
            }
            else
            {
                var view = await context.ViewCrewDutyTimeLineNews.FirstOrDefaultAsync(q => q.Id == duty.Id);
                return new CustomActionResult(HttpStatusCode.OK, view);
            }


        }

        [Route("api/stby/ceased/stat/{isExtended}/{stbyId}/{firstLeg}/{duty}/{maxfdp}")]
        public async Task<IHttpActionResult> GetSTBYActivationStat(int isExtended, int stbyId, int firstLeg, int duty, int maxfdp)
        {
            double default_reporting = Convert.ToInt32(ConfigurationManager.AppSettings["reporting"]);
            var context = new Models.dbEntities();
            var stby = await context.ViewFDPRests.FirstOrDefaultAsync(q => q.Id == stbyId);
            var leg = await context.ViewLegTimes.FirstOrDefaultAsync(q => q.ID == firstLeg);
            var reporting = ((DateTime)leg.STDLocal).AddMinutes(-default_reporting);
            string sqlQuery = "SELECT [dbo].[getFDPReductionBySTBY] ({0},{1},{2},{3})";
            Object[] parameters = { -1, stby.DateStartLocal, reporting, isExtended };
            double reduction = context.Database.SqlQuery<double>(sqlQuery, parameters).FirstOrDefault();

            var reducedMaxFDP = maxfdp - reduction;
            var maxFDPError = duty > reducedMaxFDP;

            var stbyDuration = (reporting - (DateTime)stby.DateStartLocal).TotalMinutes;
            var stbyFDPDuration = stbyDuration + duty;
            var durationError = stbyFDPDuration > 18 * 60;

            return Ok(new
            {
                reduction,
                reducedMaxFDP,
                stbyDuration,
                stbyFDPDuration,
                maxFDPError,
                durationError

            });




        }

        internal async Task<CustomActionResult> ActivateStandby(int crewId, int stbyId, string fids, int rank, int index, string ranks, int isgantt)
        {
            //doolnazs
            var context = new Models.dbEntities();
            var _fids = fids.Split('*').Select(q => Convert.ToInt32(q)).ToList();
            var flights = await context.ViewLegTimes.Where(q => _fids.Contains(q.ID)).OrderBy(q => q.ChocksOut).ToListAsync();

            var flightIds = flights.Select(q => (Nullable<int>)q.ID).ToList();
            var allFdpItems = await (from x in context.FDPItems
                                     join y in context.FDPs on x.FDPId equals y.Id
                                     where flightIds.Contains(x.FlightId) && y.CrewId != null && x.IsSector && (x.IsOff == null || x.IsOff == false)
                                     select x).ToListAsync();
            if (index == -1)
            {
                var _fpdItemX = allFdpItems.Where(q => q.FlightId == flightIds.First() && q.PositionId == rank).OrderByDescending(q => q.RosterPositionId).FirstOrDefault();
                index = 1;
                if (_fpdItemX != null)
                {
                    index = _fpdItemX.RosterPositionId == null ? 1 : (int)_fpdItemX.RosterPositionId + 1;
                }
            }


            var stby = await context.FDPs.FirstOrDefaultAsync(q => q.Id == stbyId);
            // keyParts.Add(items[0].flt.ID + "*" + (items[0].fi.IsPositioning == true ? "1" : "0"));
            var keyParts = flights.Select(q => q.ID + "*0").ToList();
            var crew = await context.ViewEmployeeLights.Where(q => q.Id == crewId).FirstOrDefaultAsync();
            double default_reporting = Convert.ToInt32(ConfigurationManager.AppSettings["reporting"]);
            var fdp = new FDP()
            {
                IsTemplate = false,
                DutyType = 1165,
                CrewId = crewId,
                GUID = Guid.NewGuid(),
                JobGroupId = rank,
                FirstFlightId = flights.First().ID,
                LastFlightId = flights.Last().ID,
                Key = string.Join("_", keyParts),
                FDPId = stbyId,
                IsOver = false,
                STD = flights.First().STD,
                STA = flights.Last().STA,
                OutOfHomeBase = flights.Last().ToAirport != crew.BaseAirportId,
                // InitPosition = String.Join("_", dto.items.Select(q => q.flightId + "*" + RosterFDPDto.getRank(q.pos)).ToList()),
                InitPosition = String.Join("_", flights.Select(q => q.ID + "*" + rank).ToList()),



            };
            /////////////////////////////////
            fdp.InitScheduleName = crew.ScheduleName;
            fdp.InitHomeBase = crew.BaseAirportId;
            fdp.InitIndex = index;
            fdp.InitGroup = crew.JobGroup;
            //switch (crew.JobGroup)
            //{
            //    case "TRE":
            //    case "TRI":
            //    case "LTC":
            //        fdp.InitRank = "IP";
            //        break;
            //    case "ISCCM":
            //        fdp.InitRank = "ISCCM";
            //        break;
            //    case "SCCM":
            //        fdp.InitRank = "SCCM";
            //        break;
            //    case "CCM":
            //        fdp.InitRank = "CCM";
            //        break;
            //    case "P1":
            //        fdp.InitRank = "P1";
            //        break;
            //    case "P2":
            //        fdp.InitRank = "P2";
            //        break;
            //    default:
            //        break;
            //}
            fdp.InitRank = RosterFDPDto.getRankStr(rank);
            fdp.InitStart = ((DateTime)flights.First().ChocksOut).AddMinutes(-default_reporting);
            fdp.InitEnd = ((DateTime)flights.Last().ChocksIn).AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"]));
            fdp.DateStart = ((DateTime)flights.First().ChocksOut).AddMinutes(-default_reporting);
            fdp.DateEnd = ((DateTime)flights.Last().ChocksIn).AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"]));
            var rst = 12;
            if (fdp.OutOfHomeBase == false)
                rst = 10;

            fdp.InitRestTo = ((DateTime)flights.Last().ChocksIn).AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"])).AddHours(rst);
            fdp.InitFlts = string.Join(",", flights.Select(q => q.FlightNumber).ToList());
            fdp.InitRoute = string.Join(",", flights.Select(q => q.FromAirportIATA).ToList());
            fdp.InitRoute += "," + flights.Last().ToAirportIATA;
            fdp.InitFromIATA = flights.First().FromAirport.ToString();
            fdp.InitToIATA = flights.Last().ToAirport.ToString();
            fdp.InitNo = string.Join("-", flights.Select(q => q.FlightNumber).ToList());
            fdp.InitKey = string.Join("-", flights.Select(q => q.ID).ToList());
            fdp.InitFlights = string.Join("*", flights.Select(q => q.ID + "_" + ("0") + "_" + ((DateTime)q.STDLocal).ToString("yyyyMMddHHmm")
              + "_" + ((DateTime)q.STALocal).ToString("yyyyMMddHHmm")
              + "_" + q.FlightNumber + "_" + q.FromAirportIATA + "_" + q.ToAirportIATA).ToList()
            );
            fdp.Split = 0;
            fdp.ReportingTime = ((DateTime)flights.First().STD).AddMinutes(-default_reporting);
            fdp.ActualEnd = fdp.InitEnd;
            fdp.ActualStart = fdp.InitStart;
            fdp.ActualRestTo = fdp.InitRestTo;
            //////////////////////////////////
            var temp = new FDP()
            {
                IsTemplate = true,
                DutyType = 1165,
                IsMain = true,
                GUID = Guid.NewGuid(),

                FirstFlightId = flights.First().ID,
                LastFlightId = flights.Last().ID,
                Key = string.Join("_", keyParts),
                Split = 0,

            };
            context.FDPs.Add(temp);

            int _cr = 0;
            List<FDPItem> _fdpitems = new List<FDPItem>();
            foreach (var x in flights)
            {
                //var _fpdItem = allFdpItems.Where(q => q.FlightId == x.ID && q.PositionId == rank).OrderByDescending(q => q.RosterPositionId).FirstOrDefault();
                //var rosterPositionId = 1;
                //if (_fpdItem != null)
                //{
                //    rosterPositionId = _fpdItem.RosterPositionId == null ? 1 : (int)_fpdItem.RosterPositionId + 1;
                //}
                //cici
                var _rnk = string.IsNullOrEmpty(ranks) ? rank : RosterFDPDto.getRank(ranks.Split('_')[_cr]);
                var _fdp_item = new FDPItem()
                {
                    FlightId = x.ID,
                    IsPositioning = false,
                    IsSector = true,
                    PositionId = _rnk, //rank,
                    RosterPositionId = index,

                };
                fdp.FDPItems.Add(_fdp_item);
                temp.FDPItems.Add(new FDPItem()
                {
                    FlightId = x.ID,
                    IsPositioning = false,
                    IsSector = true,


                });
                _fdpitems.Add(_fdp_item);
                _cr++;
            }

            var breakGreaterThan10Hours = string.Empty;
            if (flights.Count > 1)
            {
                for (int i = 1; i < flights.Count; i++)
                {
                    var dt = (DateTime)flights[i].ChocksOut - (DateTime)flights[i - 1].ChocksIn;
                    var minuts = dt.TotalMinutes;
                    // – (0:30 + 0:15 + 0:45)
                    var brk = minuts - double.Parse(ConfigurationManager.AppSettings["post_flight"]) - 60; //30:travel time, post flight duty:15, pre flight duty:30
                    if (brk >= 600)
                    {
                        //var tfi = tflights.FirstOrDefault(q => q.ID == flights[i].ID);
                        // var tfi1 = tflights.FirstOrDefault(q => q.ID == flights[i - 1].ID);
                        breakGreaterThan10Hours = "The break is greater than 10 hours.";
                    }

                    else
                    if (brk >= 180)
                    {
                        var fdpitem = fdp.FDPItems.FirstOrDefault(q => q.FlightId == flights[i].ID);
                        fdpitem.SplitDuty = true;
                        var pair = fdp.FDPItems.FirstOrDefault(q => q.FlightId == flights[i - 1].ID);
                        pair.SplitDuty = true;
                        fdpitem.SplitDutyPairId = pair.FlightId;
                        fdp.Split += 0.5 * (brk);
                        ////////////////////////////////////////////////////
                        var fdpitemTemp = temp.FDPItems.FirstOrDefault(q => q.FlightId == flights[i].ID);
                        fdpitemTemp.SplitDuty = true;
                        var pairTemp = temp.FDPItems.FirstOrDefault(q => q.FlightId == flights[i - 1].ID);
                        pairTemp.SplitDuty = true;
                        fdpitemTemp.SplitDutyPairId = pair.FlightId;
                        temp.Split += 0.5 * (brk);
                        //////////////////////////////////

                    }
                }
            }



            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;
            fdp.TemplateId = temp.Id;
            stby.FDP2 = fdp;
            stby.FDPReportingTime = ((DateTime)flights.First().ChocksOut).AddMinutes(-default_reporting);
            stby.UPD = stby.UPD == null ? 1 : ((int)stby.UPD) + 1;
            stby.PLNEnd = stby.InitEnd;
            stby.PLNRest = stby.InitRestTo;
            stby.InitEnd = ((DateTime)fdp.InitStart).AddMinutes(-1);
            stby.DateEnd = ((DateTime)fdp.InitStart).AddMinutes(-1);
            stby.InitRestTo = stby.InitEnd;
            context.FDPs.Add(fdp);
            saveResult = await context.SaveAsync();
            AddToCumDuty(stby, context);

            AddToCumDuty(fdp, context);
            // AddToCumFlight(fdp, fdp.FDPItems, context);
            AddToCumFlightByLEGTIME(fdp, flights, _fdpitems, context);
            saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;
            var vfdp = await context.ViewFDPRests.FirstOrDefaultAsync(q => q.Id == fdp.Id);

            if (isgantt == 1)
            {
                var gres = await context.ViewCrewDutyTimeLineNews.FirstOrDefaultAsync(q => q.Id == fdp.Id);
                return new CustomActionResult(HttpStatusCode.OK, gres);
            }
            else
                return new CustomActionResult(HttpStatusCode.OK, vfdp);






        }
        [Route("api/stby/activate")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostStbyActivate(dynamic dto)
        {
            //internal async Task<CustomActionResult> ActivateStandby(int crewId, int stbyId, string fids, int rank)
            if (dto == null)
                return Exceptions.getNullException(ModelState);

            int crewId = Convert.ToInt32(dto.crewId);
            int stbyId = Convert.ToInt32(dto.stbyId);
            string fids = Convert.ToString(dto.fids);
            int rank = Convert.ToInt32(dto.rank);
            string rank2 = Convert.ToString(dto.rank2);
            int index = -1;
            int isgantt = -1;
            if (dto.index != null)
            {
                index = Convert.ToInt32(dto.index);
            }

            if (dto.isgantt != null)
            {
                isgantt = Convert.ToInt32(dto.isgantt);
            }

            var result = await ActivateStandby(crewId, stbyId, fids, rank, index, rank2, isgantt);


            return result;

        }



        [Route("api/stby/activate/test/{crewId}/{fids}/{index}/{rank}/{stbyId}")]

        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetStbyActivate(int crewId, string fids, int index, int rank, int stbyId)
        {
            //internal async Task<CustomActionResult> ActivateStandby(int crewId, int stbyId, string fids, int rank)
            //   if (dto == null)
            //      return Exceptions.getNullException(ModelState);

            // int crewId = Convert.ToInt32(dto.crewId);
            // int stbyId = Convert.ToInt32(dto.stbyId);
            // string fids = Convert.ToString(dto.fids);
            // int rank = Convert.ToInt32(dto.rank);
            string rank2 = ""; //Convert.ToString(dto.rank2);

            int isgantt = -1;
            //if (dto.index != null)
            //{
            //    index = Convert.ToInt32(dto.index);
            //}

            //if (dto.isgantt != null)
            //{
            //    isgantt = Convert.ToInt32(dto.isgantt);
            //}

            var result = await ActivateStandby(crewId, stbyId, fids, rank, index, rank2, isgantt);


            return result;

        }


        [Route("api/fdp/stat/{ids}/{dh}")]

        public async Task<IHttpActionResult> GetFDPStat(string ids, int dh)
        {
            var _ids = ids.Split('_').Select(q => Convert.ToInt32(q)).ToList();
            var result = await GetMaxFDPStats(_ids, dh);

            return Ok(result);

        }



        [Route("api/roster/fdp/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> SaveFDP(RosterFDPDto dto)
        {
            //rest_after_post_flight
            int rest_after_post_flight = Convert.ToInt32(ConfigurationManager.AppSettings["rest_after_post_flight"]);
            int rerrp_check = Convert.ToInt32(ConfigurationManager.AppSettings["rerrp_check"]);
            int post_flight = Convert.ToInt32(ConfigurationManager.AppSettings["post_flight"]);
            int _ln = 1;
            if (dto.Id == -100)
            {
                dto.ids = new List<RosterFDPId>() { new RosterFDPId() { dh = 0, id = 539799 }, new RosterFDPId() { dh = 0, id = 539928 } };
                dto.extension = 0;
                dto.IsAdmin = 0;
                dto.UserName = "demo";
                dto.crewId = 4484;
                dto.from = 141866;
                dto.group = "TRE";
                dto.homeBase = 135502;
                dto.index = 1;
                dto.key = "539799_539928";
                dto.maxFDP = 780;
                dto.no = "909_910";
                dto.rank = "rank";
                dto.scheduleName = "scheduleName";
                dto.split = false;
                dto.to = 141866;


            }
            _ln = 2;
            double default_reporting = Convert.ToInt32(ConfigurationManager.AppSettings["reporting"]);
            _ln = 3;
            var context = new Models.dbEntities();
            try
            {
                var _x_fltids = dto.ids.Select(q => q.id).ToList();
                _ln = 4;
                var _x_flights = context.SchFlights.Where(q => _x_fltids.Contains(q.ID)).OrderBy(q => q.STD).ToList();
                _ln = 41;
                dto.IsAdmin = dto.IsAdmin == null ? 0 : (int)dto.IsAdmin;
                _ln = 42;
                Stopwatch timer = new Stopwatch();
                timer.Start();
                dto.items = RosterFDPDto.getItemsX(_x_flights, dto.ids); //RosterFDPDto.getItems(dto.flights);
                _ln = 43;
                dto.no = string.Join("_", _x_flights.Select(q => q.FlightNumber).ToList());
                _ln = 44;
                dto.key = string.Join("_", _x_flights.Select(q => q.ID.ToString()).ToList());
                _ln = 45;
                dto.from = (int)_x_flights.First().FromAirportId;
                _ln = 46;
                dto.to = (int)_x_flights.Last().ToAirportId;
                _ln = 47;
                dto.flights = RosterFDPDto.getFlightsStrs(_x_flights, dto.ids);

                _ln = 5;

                bool alldh = dto.items.Where(q => q.dh == 0).Count() == 0;
                var fdpDuty = dto.getDuty(default_reporting);
                var fdpFlight = dto.getFlight();
                var stdday = dto.items[0].std.Date;
                //var dutyFlight = await this.context.ViewDayDutyFlights.FirstOrDefaultAsync(q => q.Date == stdday);
                //magu210

                _ln = 6;

                var _d1 = stdday;
                var _d2 = stdday.AddDays(-6);
                var _df1 = stdday;
                var _df2 = stdday.AddDays(-27);
                var ncrewid = (Nullable<int>)dto.crewId;

                var ftlDateFrom = stdday.Date;
                var ftlDate7 = stdday.AddDays(6);
                var ftlDate14 = stdday.AddDays(13);
                var ftlDate28 = stdday.AddDays(27);
                var ftlDate12M = stdday.AddMonths(11);

                var ftlYearFrom = new DateTime(stdday.Year, 1, 1);
                var ftlYearTo = (new DateTime(stdday.Year + 1, 1, 1)).AddDays(-1);
                _ln = 7;
                if (rerrp_check == 1)
                {
                    var _rerrp = await context.AppFTLs.Where(q => q.CrewId == ncrewid && q.CDate == ftlDateFrom && q.RERRP > 0).FirstOrDefaultAsync();
                    if (_rerrp == null && (dto.IsAdmin == null || dto.IsAdmin == 0))
                    {
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 308,
                            message = "RERRP Error. "

                        });
                    }
                }

                _ln = 8;
                //var _d7 = await this.context.TableDutyFDPs.Where(q => q.CrewId == ncrewid && q.CDate >= _d2 && q.CDate <= _d1).Select(q => q.DurationLocal).SumAsync();
                var _d7 = await context.AppFTLs.Where(q => q.CrewId == ncrewid && q.CDate >= ftlDateFrom && q.CDate <= ftlDate7 && q.Duty7 > 60 * 60 - fdpDuty).FirstOrDefaultAsync();
                var _d14 = await context.AppFTLs.Where(q => q.CrewId == ncrewid && q.CDate >= ftlDateFrom && q.CDate <= ftlDate14 && q.Duty14 > 110 * 60 - fdpDuty).FirstOrDefaultAsync();
                var _d28 = await context.AppFTLs.Where(q => q.CrewId == ncrewid && q.CDate >= ftlDateFrom && q.CDate <= ftlDate28 && q.Duty28 > 190 * 60 - fdpDuty).FirstOrDefaultAsync();

                var _f28 = await context.AppFTLs.Where(q => q.CrewId == ncrewid && q.CDate >= ftlDateFrom && q.CDate <= ftlDate28 && q.Flight28 > 100 * 60 - fdpFlight).FirstOrDefaultAsync();
                var _fcy = await context.AppFTLs.Where(q => q.CrewId == ncrewid && q.CDate == stdday && q.FlightCYear > 900 * 60 - fdpFlight).FirstOrDefaultAsync();
                var _fy = await context.AppFTLs.Where(q => q.CrewId == ncrewid && q.CDate >= ftlDateFrom && q.CDate <= ftlDate12M && q.FlightYear > 1000 * 60 - fdpFlight).FirstOrDefaultAsync();

                double flight28 = 0;


                if (_d7 != null && (dto.IsAdmin == null || dto.IsAdmin == 0))
                {
                    return new CustomActionResult(HttpStatusCode.OK, new
                    {
                        Code = 307,
                        message = "Flight Time/Duty Limitaion Error. "
                        + "7-Day Duty: "
                        + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_d7.Duty7 + fdpDuty))) / 60)
                        + ":" + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_d7.Duty7 + fdpDuty))) % 60)
                        + " on " + _d7.CDate.ToString("yyy-MMM-dd")
                    });
                }
                if (_d14 != null && (dto.IsAdmin == null || dto.IsAdmin == 0))
                {
                    return new CustomActionResult(HttpStatusCode.OK, new
                    {
                        Code = 314,
                        message = "Flight Time/Duty Limitaion Error. "
                        + "14-Day Duty: "
                        + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_d14.Duty14 + fdpDuty))) / 60)
                        + ":" + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_d14.Duty14 + fdpDuty))) % 60)
                        + " on " + _d14.CDate.ToString("yyy-MMM-dd")
                    });
                }
                if (_d28 != null && (dto.IsAdmin == null || dto.IsAdmin == 0))
                {

                    return new CustomActionResult(HttpStatusCode.OK, new
                    {
                        Code = 328,
                        message = "Flight Time/Duty Limitaion Error. "
                        + "28-Day Duty: "
                        + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_d28.Duty28 + fdpDuty))) / 60)
                        + ":" + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_d28.Duty28 + fdpDuty))) % 60)
                        + " on " + _d28.CDate.ToString("yyy-MMM-dd")
                    });
                }

                if (_f28 != null && (dto.IsAdmin == null || dto.IsAdmin == 0))
                {
                    return new CustomActionResult(HttpStatusCode.OK, new
                    {
                        Code = 428,
                        message = "Flight Time/Duty Limitaion Error. "
                        + "28-Day Flight: "
                        + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_f28.Flight28 + fdpFlight))) / 60)
                        + ":" + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_f28.Flight28 + fdpFlight))) % 60)
                        + " on " + _f28.CDate.ToString("yyy-MMM-dd")
                    });
                }

                if (_fy != null && (dto.IsAdmin == null || dto.IsAdmin == 0))
                {
                    return new CustomActionResult(HttpStatusCode.OK, new
                    {
                        Code = 412,
                        message = "Flight Time/Duty Limitaion Error. "
                        + "12-Month Flight: "
                        + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_fy.FlightYear + fdpFlight))) / 60)
                        + ":" + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_fy.FlightYear + fdpFlight))) % 60)
                        + " on " + _fy.CDate.ToString("yyy-MMM-dd")
                    });
                }

                if (_fcy != null && (dto.IsAdmin == null || dto.IsAdmin == 0))
                {
                    return new CustomActionResult(HttpStatusCode.OK, new
                    {
                        Code = 413,
                        message = "Flight Time/Duty Limitaion Error. "
                        + "Current-Year Flight: "
                        + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_fcy.FlightCYear + fdpFlight))) / 60)
                        + ":" + FormatTwoDigits(Convert.ToInt32(Math.Floor(Convert.ToDecimal(_fcy.FlightCYear + fdpFlight))) % 60)
                        + " on " + _fcy.CDate.ToString("yyy-MMM-dd")
                    });
                }
                //if (flight28 + fdpFlight > 100 * 60 && (dto.IsAdmin == null || dto.IsAdmin == 0))
                //{
                //    return new CustomActionResult(HttpStatusCode.NotAcceptable, new
                //    {
                //        message = "Flight Time/Duty Limitaion Error. "
                //       + "28-Day Flights: " + FormatTwoDigits(Convert.ToInt32(Math.Floor(flight28 + fdpFlight)) / 60) + ":" + FormatTwoDigits(Convert.ToInt32(Math.Floor(flight28 + fdpFlight)) % 60)
                //    });
                //}
                ////////////////////////////////////


                //(from x in this.context.ViewDayDutyFlights
                // where x.Date >= dateStart && x.Date <= dateEnd && crewIds.Contains(x.CrewId)
                // select x).ToList();



                dto.flts = string.Join(",", dto.items.Select(q => q.no + (q.dh == 1 ? "(dh)" : "")).ToList());

                List<string> _rts = new List<string>();
                int _rt_i = 0;
                foreach (var x in dto.items)
                {
                   
                   // if (x != dto.items.Last())
                    {

                        if (_rt_i > 0 && dto.items[_rt_i - 1].dh == 1)
                        {
                            _rts.Add(x.from + "(dh)");
                        }
                        else
                        {
                            //_rts.Add((x.pos.ToLower().Contains("obs") ? "(obs)" : "")+x.from); 
                            if (_rt_i > 0 && dto.items[_rt_i - 1].pos.ToLower().Contains("obs"))
                                _rts.Add("(obs)"+x.from );
                            else
                                _rts.Add(x.from);
                        }

                    }
                    //else
                    //{
                    //    _rts.Add(x.from);
                    //    if (x.dh != 1)
                    //        _rts.Add(x.to);
                    //    else
                    //        _rts.Add(x.to + "(dh)");
                    //}
                    if (x == dto.items.Last())
                    {
                        if (x.dh != 1)
                        { 
                            _rts.Add(  (x.pos.ToLower().Contains("obs")?"(obs)":"")+x.to); 
                        }
                        else
                            _rts.Add(x.to + "(dh)");
                    }
                    _rt_i++;
                }
                
               // var rts = dto.items.Select(q => q.from).ToList();
                //rts.Add(dto.items.Last().to);
                dto.route = string.Join(",", _rts);
                // if (dto.items.Count= 1)


                var keyParts = dto.items.Select(q => q.flightId + (q.dh == 1 ? "*1" : "*0")).ToList();
                var rst = dto.to == dto.homeBase ? 12 : 10;

                // if (dto.extension != null && dto.extension > 0)
                //     rst += 4;
                //bini

                var post_flight_rest = rest_after_post_flight == 1 ? post_flight : 0;


                var fdp = new FDP()
                {
                    IsTemplate = false,
                    DutyType = 1165,
                    CrewId = dto.crewId,
                    GUID = Guid.NewGuid(),
                    JobGroupId = RosterFDPDto.getRank(dto.rank),
                    FirstFlightId = dto.items.First().flightId,
                    LastFlightId = dto.items.Last().flightId,
                    Key = string.Join("_", keyParts),
                    InitFlts = dto.flts,
                    InitRoute = dto.route,
                    InitFromIATA = dto.from.ToString(),
                    InitToIATA = dto.to.ToString(),
                    InitStart = !alldh ? dto.items.First().offblock.AddMinutes(-default_reporting) : dto.items.First().offblock,
                    InitEnd = !alldh ? dto.items.Last().onblock.AddMinutes(post_flight) : dto.items.Last().onblock,
                    DateStart = !alldh ? dto.items.First().offblock.AddMinutes(-default_reporting) : dto.items.First().offblock,
                    DateEnd = !alldh ? dto.items.Last().onblock.AddMinutes(post_flight) : dto.items.Last().onblock,

                    InitRestTo = !alldh ? dto.items.Last().onblock.AddMinutes(post_flight_rest).AddHours(rst) : dto.items.Last().onblock,

                    InitFlights = string.Join("*", dto.flights),
                    InitGroup = dto.group,
                    InitHomeBase = dto.homeBase,
                    InitIndex = dto.index,
                    InitKey = dto.key,
                    InitNo = dto.no,
                    InitRank = dto.rank,
                    InitPosition = String.Join("_", dto.items.Select(q => q.flightId + "*" + RosterFDPDto.getRank(q.pos)).ToList()),
                    InitScheduleName = dto.scheduleName,
                    Extension = dto.extension,
                    Split = 0,
                    UserName = dto.UserName,
                    MaxFDP = dto.maxFDP,
                    ReportingTime = !alldh ? dto.items.First().offblock.AddMinutes(-default_reporting) : dto.items.First().offblock,
                    IsOver = false,
                    STD = dto.items.First().std,
                    STA = dto.items.Last().sta,
                    OutOfHomeBase = dto.to != dto.homeBase,



                };
                fdp.ActualEnd = fdp.InitEnd;
                fdp.ActualStart = fdp.InitStart;
                fdp.ActualRestTo = fdp.InitRestTo;
                fdp.FDPExtras.Add(new FDPExtra() { MaxFDP = dto.maxFDP });
                //Check Extension //////////////////
                if (fdp.Extension != null && fdp.Extension > 0)
                {
                    var exd1 = (DateTime)fdp.InitStart;
                    var exd0 = exd1.AddHours(-168);
                    var extcnt = await context.ExtensionHistories.Where(q => q.CrewId == fdp.CrewId && (q.DutyStart >= exd0 && q.DutyStart <= exd1)).CountAsync();
                    if (extcnt >= 2)
                    {
                        return new CustomActionResult(HttpStatusCode.OK, new
                        {
                            Code = 110,
                            message = "You can't extend maximum daily FDP due to the crew extended fdps in 7 consecutive days."

                        });
                    }
                }

                


                //4-11
                //Check interuption/////////////////
                var exc = new List<int>() { 
           //1166
		   //, 1169
		    /*10000, 10001, 100000, 100002, 100004, 100005, 100006,*/
               // 100007
				//, 100008
				 100024
				//, 100025
				, 100009
                , 100020
                , 100021
                , 100022
                , 100023
                , 200000
                , 200001
                , 200002
                , 200003
                , 200004
                , 200005
                ,300000
                ,300001
                ,300002
                ,300003
                ,300004
                ,300005
                ,300006
                ,300007
                //,300008

                //,300010
                ,300011
                ,300012
               
               // ,300014
               ,300051

                ,1167
                ,1168
                ,1170
                ,1169
                ,300013
               // ,5001
                };
                var stbys = new List<int>() { 1167, 1168, 1170, 300013 };
                //if (!alldh)
                FDP _interupted = null;
                {
                    //FDP _interupted = null;

                    _interupted = await context.FDPs.FirstOrDefaultAsync(q =>
                                                !exc.Contains(q.DutyType) &&
                                                q.Id != fdp.Id && q.CrewId == fdp.CrewId
                                                && (
                                                      //(fdp.InitStart >= q.InitStart && fdp.InitStart < q.InitRestTo)
                                                      // || (fdp.InitEnd >= q.InitStart && fdp.InitEnd <= q.InitRestTo)
                                                      // || (q.InitStart >= fdp.InitStart && q.InitStart < fdp.InitRestTo)
                                                      // || (q.InitRestTo > fdp.InitStart && q.InitRestTo <= fdp.InitRestTo)
                                                      (fdp.InitStart >= q.InitStart && fdp.InitRestTo <= q.InitRestTo)
                                                      || (q.InitStart >= fdp.InitStart && q.InitRestTo <= fdp.InitRestTo)
                                                      || (q.InitStart >= fdp.InitStart && q.InitStart < fdp.InitRestTo)
                                                      || (q.InitRestTo > fdp.InitStart && q.InitRestTo <= fdp.InitRestTo)
                                                    )
                                                );
                    if (_interupted == null)
                    {
                        _interupted = await context.FDPs.FirstOrDefaultAsync(q => stbys.Contains(q.DutyType) && q.Id != fdp.Id && q.CrewId == fdp.CrewId
                          && (
                               ((q.InitStart > fdp.InitStart) && ((fdp.InitEnd > q.InitStart && fdp.InitEnd < q.InitRestTo) || (fdp.InitRestTo > q.InitStart && fdp.InitRestTo < q.InitRestTo)))
                               ||
                               ((q.InitRestTo > fdp.InitStart && q.InitRestTo < fdp.InitRestTo) && !(fdp.InitStart >= q.InitStart && fdp.InitStart < q.InitEnd))

                          )


                        );

                    }
                    bool _activeq = false;
                    if (_interupted == null)
                    {
                        _interupted = await context.FDPs.FirstOrDefaultAsync(q => stbys.Contains(q.DutyType) && q.Id != fdp.Id && q.CrewId == fdp.CrewId
                          && (fdp.InitStart >= q.InitStart && fdp.InitStart < q.InitEnd)

                        );
                        _activeq = _interupted != null;

                    }
                    //WHAT IS THIS?
                    bool _interuptedAcceptable = dto.DeletedFDPId == null ? true : dto.DeletedFDPId != _interupted.Id;

                    if (_interupted != null && _interuptedAcceptable /* && (dto.IsAdmin == null || dto.IsAdmin == 0)*/)
                    {
                        //Rest/Interruption Error
                        //if ((dto.IsAdmin == null || dto.IsAdmin == 0)
                        //    && (_activeq && _interupted.DutyType != 1167 && _interupted.DutyType != 1168 && _interupted.DutyType != 1170)
                        //    || !(dto.items.First().std >= _interupted.DateStart && dto.items.First().std <= _interupted.DateEnd))
                        if ((dto.IsAdmin == null || dto.IsAdmin == 0)
                           && (_interupted.DutyType != 1167 && _interupted.DutyType != 1168 && _interupted.DutyType != 1170)
                          )
                        {
                            //if (false)
                            bool sendError = false;
                            switch (_interupted.DutyType)
                            {
                                case 5000:
                                    if ((fdp.InitStart >= _interupted.InitStart && fdp.InitStart <= _interupted.InitEnd)
                                           || (fdp.InitEnd >= _interupted.InitStart && fdp.InitEnd <= _interupted.InitEnd)
                                           || (_interupted.InitStart >= fdp.InitStart && _interupted.InitStart <= fdp.InitEnd)
                                           || (_interupted.InitEnd >= fdp.InitStart && _interupted.InitEnd <= fdp.InitEnd)
                                           )
                                        sendError = true;

                                    break;
                                case 10000:
                                case 100000:
                                case 100002:
                                case 100004:
                                case 100005:
                                case 100006:
                                    if ((new List<int>() { 10000, 10001, 1169, 100000, 100002, 100004, 100005, 100006, 100007, 100008, 100009, 100020, 100021, 100022, 100023, 100024, 200000, 200001, 200002, 200003, 200004, 200005 }).IndexOf(fdp.DutyType) == -1)
                                    {
                                        if ((fdp.InitStart >= _interupted.InitStart && fdp.InitStart <= _interupted.InitEnd)
                                            || (fdp.InitEnd >= _interupted.InitStart && fdp.InitEnd <= _interupted.InitEnd)
                                            || (_interupted.InitStart >= fdp.InitStart && _interupted.InitStart <= fdp.InitEnd)
                                            || (_interupted.InitEnd >= fdp.InitStart && _interupted.InitEnd <= fdp.InitEnd)
                                            )
                                            sendError = true;
                                    }
                                    break;
                                case 1165:
                                    if (alldh)
                                    {
                                        try
                                        {
                                            var intStart = ((DateTime)_interupted.InitStart).AddMinutes(60);
                                            var intEnd = ((DateTime)_interupted.InitEnd).AddMinutes(-post_flight);
                                            if (fdp.InitStart > intStart && fdp.InitStart < intEnd)
                                                sendError = true;
                                            else if (fdp.InitEnd > intStart && fdp.InitEnd < intEnd)
                                                sendError = true;
                                            else if (fdp.InitStart == intStart && fdp.InitEnd == intEnd)
                                                sendError = true;
                                            else if (intStart >= fdp.InitStart && intStart <= fdp.InitEnd)
                                                sendError = true;
                                            else if (intEnd >= fdp.InitStart && intEnd <= fdp.InitEnd)
                                                sendError = true;
                                            else
                                                sendError = false;

                                        }
                                        catch (Exception exxx)
                                        {
                                            sendError = false;
                                        }

                                    }
                                    else
                                    {
                                        if (_interupted.InitNo.Contains("*1"))
                                        {
                                            var nodh = await context.FDPItems.Where(q => q.FDPId == _interupted.Id && q.IsPositioning != true).CountAsync();
                                            if (nodh > 0)
                                                sendError = true;
                                            else
                                            {
                                                var fdpStart = dto.items.First().std;
                                                var fdpEnd = dto.items.Last().sta;
                                                if (_interupted.InitStart >= fdpStart && _interupted.InitStart <= fdpEnd)
                                                    sendError = true;
                                                else if (_interupted.InitEnd >= fdpStart && _interupted.InitEnd <= fdpEnd)
                                                    sendError = true;
                                                else if (fdpStart >= _interupted.InitStart && fdpStart <= _interupted.InitEnd)
                                                    sendError = true;
                                                else if (fdpEnd >= _interupted.InitStart && fdpEnd <= _interupted.InitEnd)
                                                    sendError = true;
                                                else
                                                    sendError = false;
                                            }
                                        }
                                        else
                                            sendError = true;
                                    }

                                    break;
                                case 1169:
                                case 100008:
                                    if (_interupted.InitStart< fdp.InitEnd)
                                        sendError = true;
                                    break;

                                default:
                                    sendError = true;
                                    break;
                            }
                            if (sendError && (dto.IsAdmin == null || dto.IsAdmin == 0))
                                return new CustomActionResult(HttpStatusCode.OK, new
                                {
                                    Code = 406,
                                    message = "Rest/Interruption Error(" + _interupted.Id + "). " + (GetDutyTypeTitle(_interupted.DutyType)) + "   " + (_interupted.InitStart == null ? "" : ((DateTime)_interupted.InitStart).ToString("yyyy-MM-dd") + " " + _interupted.InitFlts + " " + _interupted.InitRoute)

                                });
                        }
                        else
                        {
                            if (_interupted.DutyType == 1167 || _interupted.DutyType == 1168)
                                return new CustomActionResult(HttpStatusCode.OK, new { Code = 501, data = /*_interupted*/new { Id = _interupted.Id } });
                            else if (_interupted.DutyType == 5000)
                                return new CustomActionResult(HttpStatusCode.OK, new
                                {
                                    Code = 406,
                                    message = "Interruption Error (TRAINING)"

                                });
                            else
                            {
                                //2025-04-12
                                var _strt = ((DateTime)fdp.InitStart).AddMinutes(default_reporting);
                                var rdif = Math.Abs((DateTime.UtcNow - _strt).TotalMinutes);
                                if (rdif < 10 * 60 && !alldh)
                                    return new CustomActionResult(HttpStatusCode.OK, new { Code = 304, data =  /*_interupted*/new { Id = _interupted.Id } });
                            }
                        }
                        //return new CustomActionResult(HttpStatusCode.NotAcceptable, _interupted);
                    }
                }

                ///////////////////////////////
                if (dto.DeletedFDPId != null)
                {
                    //await DeleteFDP((int)dto.DeletedFDPId);
                    var dfdp = await context.FDPs.FirstOrDefaultAsync(q => q.Id == dto.DeletedFDPId);
                    double total = 0;
                    if (!string.IsNullOrEmpty(dfdp.InitFlights))
                    {
                        var parts = dfdp.InitFlights.Split('*').ToList();

                        foreach (var x in parts)
                        {
                            var _std = x.Split('_')[2];
                            var _sta = x.Split('_')[3];
                            var std = DateTime.ParseExact(_std, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                            var sta = DateTime.ParseExact(_sta, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                            total += (sta - std).TotalMinutes;
                        }
                        //this.context.Database.ExecuteSqlCommand("update employee set flightsum=isnull(flightsum,0)-" + total + ",FlightEarly=isnull(FlightEarly,0)+" + early + ",FlightLate=isnull(FlightLate,0)+" + late + "  where id=" + dto.crewId);
                        context.Database.ExecuteSqlCommand("update employee set flightsum=isnull(flightsum,0)-" + total + "  where id=" + dfdp.CrewId);

                    }
                    var related = await context.FDPs.Where(q => q.FDPId == dto.DeletedFDPId).FirstOrDefaultAsync();
                    if (related != null)
                    {
                        related.FDPId = null;
                        related.FDPReportingTime = null;
                        related.UPD = related.UPD != null ? related.UPD + 1 : 1;
                    }

                    if (dfdp.FDPId != null)
                    {
                        var stby = await context.FDPs.FirstOrDefaultAsync(q => q.Id == dfdp.FDPId);
                        if (stby != null)
                        {
                            stby.FDPId = null;
                            stby.FDPReportingTime = null;
                            stby.UPD = stby.UPD == null ? 1 : ((int)stby.UPD) + 1;
                        }
                    }


                    var templateId = dfdp.TemplateId;
                    context.FDPs.Remove(dfdp);


                }
                ////////////////////////////
                bool saveTemp = false;
                var tkey = string.Join("_", keyParts);
                var temp = await context.FDPs.FirstOrDefaultAsync(q => q.IsTemplate && q.Key == tkey);
                if (temp == null)
                {
                    saveTemp = true;
                    temp = new FDP()
                    {
                        IsTemplate = true,
                        DutyType = 1165,
                        IsMain = true,
                        GUID = Guid.NewGuid(),

                        FirstFlightId = dto.items.First().flightId,
                        LastFlightId = dto.items.Last().flightId,
                        Key = string.Join("_", keyParts),
                        InitFlts = dto.flts,
                        InitRoute = dto.route,
                        InitFromIATA = dto.from.ToString(),
                        InitToIATA = dto.to.ToString(),
                        InitStart = dto.items.First().offblock.AddMinutes(default_reporting),
                        InitEnd = dto.items.Last().onblock.AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"])),
                        Split = 0,
                        UserName = dto.UserName,
                        MaxFDP = dto.maxFDP,
                        IsOver = false,

                    };
                    temp.FDPExtras.Add(new FDPExtra() { MaxFDP = dto.maxFDP });
                    context.FDPs.Add(temp);
                }

                double flightSum = 0;
                var firststd = ((DateTime)dto.items.First().offblock).ToLocalTime();
                var laststa = ((DateTime)dto.items.Last().onblock).ToLocalTime();
                int early = firststd.Hour <= 6 ? 1 : 0;
                int late = laststa.Hour >= 22 ? 1 : 0;
                foreach (var x in dto.items)
                {
                    flightSum += ((DateTime)x.onblock - (DateTime)x.offblock).TotalMinutes;
                    fdp.FDPItems.Add(new FDPItem()
                    {
                        FlightId = x.flightId,
                        IsPositioning = x.dh == 1,
                        IsSector = x.dh != 1,
                        PositionId = RosterFDPDto.getRank(x.pos), //RosterFDPDto.getRank(dto.rank),
                        RosterPositionId = dto.index,

                    });
                    if (saveTemp)
                        temp.FDPItems.Add(new FDPItem()
                        {
                            FlightId = x.flightId,
                            IsPositioning = x.dh == 1,
                            IsSector = x.dh != 1,


                        });

                }

                var breakGreaterThan10Hours = string.Empty;
                if (dto.items.Count > 1 && dto.split)
                {
                    for (int i = 1; i < dto.items.Count; i++)
                    {
                        var dt = (DateTime)dto.items[i].offblock - (DateTime)dto.items[i - 1].onblock;
                        var minuts = dt.TotalMinutes;
                        // – (0:30 + 0:15 + 0:45)
                        var brk = minuts - double.Parse(ConfigurationManager.AppSettings["post_flight"]) - 60; //30:travel time, post flight duty:15, pre flight duty:30
                        if (brk >= 600)
                        {
                            //var tfi = tflights.FirstOrDefault(q => q.ID == flights[i].ID);
                            // var tfi1 = tflights.FirstOrDefault(q => q.ID == flights[i - 1].ID);
                            breakGreaterThan10Hours = "The break is greater than 10 hours.";
                        }

                        else
                        if (brk >= 180)
                        {
                            var fdpitem = fdp.FDPItems.FirstOrDefault(q => q.FlightId == dto.items[i].flightId);
                            fdpitem.SplitDuty = true;
                            var pair = fdp.FDPItems.FirstOrDefault(q => q.FlightId == dto.items[i - 1].flightId);
                            pair.SplitDuty = true;
                            fdpitem.SplitDutyPairId = pair.FlightId;
                            fdp.Split += 0.5 * (brk);
                            ////////////////////////////////////////////////////
                            if (saveTemp)
                            {
                                var fdpitemTemp = temp.FDPItems.FirstOrDefault(q => q.FlightId == dto.items[i].flightId);
                                fdpitemTemp.SplitDuty = true;
                                var pairTemp = temp.FDPItems.FirstOrDefault(q => q.FlightId == dto.items[i - 1].flightId);
                                pairTemp.SplitDuty = true;
                                fdpitemTemp.SplitDutyPairId = pair.FlightId;
                                temp.Split += 0.5 * (brk);
                            }


                            //////////////////////////////////

                        }
                    }
                }

                var saveResult = new CustomActionResult(HttpStatusCode.OK, null);

                if (saveTemp)
                {
                    saveResult = await context.SaveAsync();
                }

                if (saveResult.Code != HttpStatusCode.OK)
                    return saveResult;
                fdp.TemplateId = temp.Id;

                //????? stby.UPD = stby.UPD == null ? 1 : ((int)stby.UPD) + 1;
                context.FDPs.Add(fdp);
                if (fdp.Extension != null && fdp.Extension > 0)
                {
                    fdp.ExtensionHistories.Add(new ExtensionHistory()
                    {
                        CrewId = (int)fdp.CrewId,
                        DutyStart = fdp.InitStart,
                        Extension = fdp.Extension,
                        Sectors = dto.items.Count,
                    });
                }

                context.Database.ExecuteSqlCommand("update employee set flightsum=isnull(flightsum,0)+" + flightSum + ",FlightEarly=isnull(FlightEarly,0)+" + early + ",FlightLate=isnull(FlightLate,0)+" + late + "  where id=" + dto.crewId);

                //2023-05-16
                if (_interupted != null && _interupted.DutyType == 1170)
                {
                    _interupted.FDP2 = fdp;
                    _interupted.FDPReportingTime = fdp.InitStart;
                    _interupted.UPD = _interupted.UPD == null ? 1 : ((int)_interupted.UPD) + 1;
                    _interupted.PLNEnd = _interupted.InitEnd;
                    _interupted.PLNRest = _interupted.InitRestTo;
                    _interupted.InitEnd = ((DateTime)fdp.InitStart).AddMinutes(-1);
                    _interupted.DateEnd = ((DateTime)fdp.InitStart).AddMinutes(-1);
                    _interupted.InitRestTo = _interupted.InitEnd;

                    fdp.FDPId = _interupted.Id;

                }
                saveResult = await context.SaveAsync();
                if (saveResult.Code != HttpStatusCode.OK)
                    return saveResult;
                // var vfdp = await this.context.ViewFDPRests.FirstOrDefaultAsync(q => q.Id == fdp.Id);
                //  return new CustomActionResult(HttpStatusCode.OK, vfdp);

                AddToCumDuty(fdp);
                AddToCumFlight(fdp, dto.items);

                timer.Stop();
                var _ms = timer.Elapsed;
                var fdp_result = new FDPResult()
                {
                    BL = fdp.BL,
                    BoxId = fdp.BoxId,
                    CanceledNo = fdp.CanceledNo,
                    CanceledRoute = fdp.CanceledRoute,
                    CityId = fdp.CityId,
                    ConfirmedBy = fdp.ConfirmedBy,
                    CP = fdp.CP,
                    CrewId = fdp.CrewId,
                    CustomerId = fdp.CustomerId,
                    DateConfirmed = fdp.DateConfirmed,
                    DateContact = fdp.DateContact,
                    DateEnd = fdp.DateEnd,
                    DateStart = fdp.DateStart,
                    DelayAmount = fdp.DelayAmount,
                    DelayedReportingTime = fdp.DelayedReportingTime,
                    DutyType = fdp.DutyType,
                    Extension = fdp.Extension,
                    FDPId = fdp.FDPId,
                    FDPReportingTime = fdp.FDPReportingTime,
                    FirstFlightId = fdp.FirstFlightId,
                    FirstNotification = fdp.FirstNotification,
                    FX = fdp.FX,
                    GUID = fdp.GUID,
                    Id = fdp.Id,
                    InitEnd = fdp.InitEnd,
                    InitFlights = fdp.InitFlights,
                    InitFlts = fdp.InitFlts,
                    InitFromIATA = fdp.InitFromIATA,
                    InitGroup = fdp.InitGroup,
                    InitHomeBase = fdp.InitHomeBase,
                    InitIndex = fdp.InitIndex,
                    InitKey = fdp.InitKey,
                    InitNo = fdp.InitNo,
                    InitRank = fdp.InitRank,
                    InitRestTo = fdp.InitRestTo,
                    InitRoute = fdp.InitRoute,
                    InitScheduleName = fdp.InitScheduleName,
                    InitStart = fdp.InitStart,
                    InitToIATA = fdp.InitToIATA,
                    IsMain = fdp.IsMain,
                    IsTemplate = fdp.IsTemplate,
                    JobGroupId = fdp.JobGroupId,
                    Key = fdp.Key,
                    LastFlightId = fdp.LastFlightId,
                    LocationId = fdp.LocationId,
                    MaxFDP = fdp.MaxFDP,
                    NextNotification = fdp.NextNotification,
                    Remark = fdp.Remark,
                    Remark2 = fdp.Remark2,
                    ReportingTime = fdp.ReportingTime,
                    RevisedDelayedReportingTime = fdp.RevisedDelayedReportingTime,
                    Split = fdp.Split,
                    TemplateId = fdp.TemplateId,
                    UPD = fdp.UPD,
                    UserName = fdp.UserName,
                };
                if (dto.IsGantt == 1)
                {
                    var gres = await context.ViewCrewDutyTimeLineNews.FirstOrDefaultAsync(q => q.Id == fdp.Id);
                    return new CustomActionResult(HttpStatusCode.OK, gres);
                }
                else
                    return new CustomActionResult(HttpStatusCode.OK, fdp_result);


            }
            catch (Exception ex)
            {


                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();


                int iiii = 0;
                var msg = line + "   " + _ln + "   " + ex.Message;
                if (ex.InnerException != null)
                    msg += " IINER:" + ex.InnerException.Message;
                return new CustomActionResult(HttpStatusCode.OK, msg);
            }

            return Ok(true);
        }


        public class FTLCrewIds
        {
            public DateTime CDate { get; set; }
            public List<int> CrewIds { get; set; }
        }
        [Route("api/ftl/abs/crews/")]
        [AcceptVerbs("POST")]
        //nookp
        public IHttpActionResult GetAppFTLsABSByCrewIds(FTLCrewIds dto)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var date = dto.CDate.Date;
            var context = new Models.dbEntities();

            var query = from x in context.AppFTLAbs
                        where x.CDate == date && dto.CrewIds.Contains(x.CrewId)
                        select x;

            var result = query.OrderByDescending(q => q.Duty7).ThenByDescending(q => q.Duty14).ThenByDescending(q => q.Duty28).ToList();
            var yy = date.Year;
            var mm = date.Month;
            var ratios = context.FTLFlightTimeRatioMonthlies.Where(q => q.Year == yy && q.Month == mm && dto.CrewIds.Contains(q.CrewId)).ToList();
            foreach (var r in ratios)
            {
                var c = result.FirstOrDefault(q => q.CrewId == r.CrewId);
                if (c != null)
                {
                    c.Ratio = Math.Round(Convert.ToDouble(r.Ratio));
                    c.CNT = r.CNT;
                    c.ScheduledFlightTime = r.ScheduledFlightTime;

                }
            }

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }

        public class appAll
        {
            public DateTime? Start { get; set; }
            public DateTime? End { get; set; }
            public int? Type { get; set; }
            public string TypeStr { get; set; }

            public int? Total { get; set; }
        }
        [Route("api/scheduling/crew/calndar/{id}")]
        [AcceptVerbs("GET")]
        //nookp
        public IHttpActionResult GetCrewCalndar(int id)
        {

            var context = new Models.dbEntities();

            try
            {
                context.Database.CommandTimeout = 160;


                var query = (
                    from x in context.ViewFlightCrewXes

                    where x.CrewId == id //&& x.DateConfirmed != null
                    group x by new { x.STDDay, x.STDDayEnd } into grp
                    select new appAll() { Start = grp.Key.STDDay, End = grp.Key.STDDayEnd, Total = grp.Count(), Type = 1165 }

                    ).ToList();

                var dayOffs = context.FDPs.Where(q => q.CrewId == id && (q.DutyType == 10000 || q.DutyType == 10001 || q.DutyType == 100008 || q.DutyType == 300009) && q.DateConfirmed != null).ToList();
                var ds = dayOffs.Select(q => new appAll()
                {
                    Start = ((DateTime)q.DateStart).AddMinutes(210),
                    End = ((DateTime)q.DateEnd).AddMinutes(210),
                    Total = 0,
                    Type = 10000,

                }).ToList();

                var allowedTypes = new List<int>() {
                    1167,
                    1168,
                    5001,
                    5000,
                    300008,
                    1170,
                    1169,
                    100002,
                    100001,
                    100003,
                    100004,
                    100005,
                    100006 
                //rerrp
                ,10000
                //rerrp2
                ,10001
                //ground
                ,100000
                //meeting
                ,100001
                //req off
                ,100008
                //refuse
                ,100009
                //duty
                ,300008
                //rest
                ,300009
                //oa stby
                ,300010
                //stby c
                ,300013
                //off
                ,1166

                };
                //var others = this.context.FDPs.Where(q => q.CrewId == id && (q.DutyType != 1165 && q.DutyType != 10001 && q.DutyType != 10000)).ToList();
                var others = context.FDPs.Where(q => q.CrewId == id && allowedTypes.Contains(q.DutyType) && q.DateConfirmed != null).ToList();
                var ds2 = others.Select(q => new appAll()
                {
                    Start = ((DateTime)q.DateStart).AddMinutes(210),
                    End = ((DateTime)q.DateEnd).AddMinutes(210),
                    Total = 0,
                    Type = q.DutyType,

                }).ToList();
                foreach (var x in ds2)
                {
                    switch (x.Type)
                    {






                        case 100003:
                            x.TypeStr = "SIM";
                            break;
                        case 100004:
                        case 100005:
                        case 100006:
                            x.TypeStr = "EXP";
                            break;
                        //rerrp

                        case 10000:
                            x.TypeStr = "RERRP";
                            break;
                        //rerrp2

                        case 10001:
                            x.TypeStr = "RERRP";
                            break;


                        //ground

                        case 100000:
                            x.TypeStr = "GRND";
                            break;

                        //meeting
                        case 100001:
                            x.TypeStr = "MTG";
                            break;
                        //req off
                        case 100008:
                        case 1166:
                            x.TypeStr = "OFF";
                            break;

                        case 100009:
                            x.TypeStr = "REF";
                            break;

                        case 100002:
                            x.TypeStr = "SICK";
                            break;
                        case 1167:
                            x.TypeStr = "STBP";
                            break;
                        case 300010:
                            x.TypeStr = "STBO";
                            break;
                        case 1168:
                            x.TypeStr = "STBA";
                            break;
                        case 300013:
                            x.TypeStr = "STB";
                            break;
                        case 5001:
                            x.TypeStr = "OFC";
                            break;
                        case 5000:
                            x.TypeStr = "TRN";
                            break;
                        case 300008:
                            x.TypeStr = "DTY";
                            break;
                        case 300009:
                            x.TypeStr = "RST";
                            break;
                        case 1169:
                            x.TypeStr = "VAC";
                            break;
                        case 1170:
                            x.TypeStr = "RES";
                            break;

                        default:
                            x.TypeStr = "DTY";
                            break;
                    }
                }

                query = query
                    //.Concat(ds)
                    .Concat(ds2).ToList();

                return Ok(query);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return Ok(msg);
            }



        }



        DateTime toDate(string dt, string t)
        {
            var year = Convert.ToInt32(dt.Substring(0, 4));
            var month = Convert.ToInt32(dt.Substring(4, 2));
            var day = Convert.ToInt32(dt.Substring(6, 2));
            var hour = Convert.ToInt32(t.Substring(0, 2));
            var minute = Convert.ToInt32(t.Substring(2, 2));

            return new DateTime(year, month, day, hour, minute, 0);
        }

        DateTime toDateTime(string dt)
        {
            var year = Convert.ToInt32(dt.Substring(0, 4));
            var month = Convert.ToInt32(dt.Substring(4, 2));
            var day = Convert.ToInt32(dt.Substring(6, 2));
            var hour = Convert.ToInt32(dt.Substring(8, 2));
            var minute = Convert.ToInt32(dt.Substring(10, 2));

            return new DateTime(year, month, day, hour, minute, 0);
        }
        private void AddToCumFlight(FDP fdp, List<RosterFDPDtoItem> items)
        {
            try
            {

                var context = new Models.dbEntities();
                context.Database.ExecuteSqlCommand("Delete from TableFlightFDP where FDPId=" + fdp.Id);
                var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);
                var fdpItems = fdp.FDPItems.ToList();
                foreach (var fdp_item in fdpItems)
                {
                    var item = items.FirstOrDefault(q => q.flightId == fdp_item.FlightId);
                    var offblock = ((DateTime)item.offblock).AddMinutes(utcdiff);
                    var onblock = ((DateTime)item.onblock).AddMinutes(utcdiff);
                    if (offblock.Date == onblock.Date)
                    {
                        var duration = (fdp_item.IsOff == true || fdp_item.IsPositioning == true || fdp_item.IsSector == false) ? 0 : (onblock - offblock).TotalMinutes;
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = offblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration,
                            DurationLocal = duration,
                            DutyEnd = item.onblock,
                            DutyEndLocal = onblock,
                            DutyStart = item.offblock,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = fdp_item.FlightId,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                    }
                    else
                    {
                        var duration1 = (fdp_item.IsOff == true || fdp_item.IsPositioning == true || fdp_item.IsSector == false)
                            ? 0 : (onblock.Date - offblock).TotalMinutes;
                        var duration2 = (fdp_item.IsOff == true || fdp_item.IsPositioning == true || fdp_item.IsSector == false)
                            ? 0 : (onblock - onblock.Date).TotalMinutes;
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = offblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration1,
                            DurationLocal = duration1,
                            DutyEnd = item.onblock,
                            DutyEndLocal = onblock,
                            DutyStart = item.offblock,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = fdp_item.FlightId,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = onblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration2,
                            DurationLocal = duration2,
                            DutyEnd = item.onblock,
                            DutyEndLocal = onblock,
                            DutyStart = item.offblock,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = fdp_item.FlightId,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });


                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {

            }


        }

        private void AddToCumFlight(FDP fdp, List<SchFlight> flights, List<FDPItem> fdpitems, Models.dbEntities context = null)
        {
            try
            {
                //02-05
                bool do_save = context == null;
                if (context == null)
                    context = new Models.dbEntities();
                context.Database.ExecuteSqlCommand("Delete from TableFlightFDP where FDPId=" + fdp.Id);
                var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);
                // var fdpItems = fdp.FDPItems.ToList();
                foreach (var flight in flights)
                {
                    var fdp_item = fdpitems.FirstOrDefault(q => q.FlightId == flight.ID);
                    var offblock = ((DateTime)flight.ChocksOut).AddMinutes(utcdiff);
                    var onblock = ((DateTime)flight.ChocksIn).AddMinutes(utcdiff);
                    if (offblock.Date == onblock.Date)
                    {
                        var duration = (onblock - offblock).TotalMinutes;
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = offblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration,
                            DurationLocal = duration,
                            DutyEnd = flight.ChocksIn,
                            DutyEndLocal = onblock,
                            DutyStart = flight.ChocksOut,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = flight.ID,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                    }
                    else
                    {
                        var duration1 = (onblock.Date - offblock).TotalMinutes;
                        var duration2 = (onblock - onblock.Date).TotalMinutes;
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = offblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration1,
                            DurationLocal = duration1,
                            DutyEnd = flight.ChocksIn,
                            DutyEndLocal = onblock,
                            DutyStart = flight.ChocksOut,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = flight.ID,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = onblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration2,
                            DurationLocal = duration2,
                            DutyEnd = flight.ChocksIn,
                            DutyEndLocal = onblock,
                            DutyStart = flight.ChocksOut,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = flight.ID,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });


                    }
                }
                if (do_save)
                    context.SaveChanges();
            }
            catch (Exception ex)
            {

            }


        }

        private void AddToCumFlightByLEGTIME(FDP fdp, List<ViewLegTime> flights, List<FDPItem> fdpitems, Models.dbEntities context = null)
        {
            try
            {
                //02-05
                bool do_save = context == null;
                if (context == null)
                    context = new Models.dbEntities();
                context.Database.ExecuteSqlCommand("Delete from TableFlightFDP where FDPId=" + fdp.Id);
                var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);
                // var fdpItems = fdp.FDPItems.ToList();
                foreach (var flight in flights)
                {
                    var fdp_item = fdpitems.FirstOrDefault(q => q.FlightId == flight.ID);
                    var offblock = ((DateTime)flight.ChocksOut).AddMinutes(utcdiff);
                    var onblock = ((DateTime)flight.ChocksIn).AddMinutes(utcdiff);
                    if (offblock.Date == onblock.Date)
                    {
                        var duration = (onblock - offblock).TotalMinutes;
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = offblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration,
                            DurationLocal = duration,
                            DutyEnd = flight.ChocksIn,
                            DutyEndLocal = onblock,
                            DutyStart = flight.ChocksOut,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = flight.ID,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                    }
                    else
                    {
                        var duration1 = (onblock.Date - offblock).TotalMinutes;
                        var duration2 = (onblock - onblock.Date).TotalMinutes;
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = offblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration1,
                            DurationLocal = duration1,
                            DutyEnd = flight.ChocksIn,
                            DutyEndLocal = onblock,
                            DutyStart = flight.ChocksOut,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = flight.ID,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                        context.TableFlightFDPs.Add(new TableFlightFDP()
                        {
                            CDate = onblock.Date,
                            CrewId = fdp.CrewId,
                            Duration = duration2,
                            DurationLocal = duration2,
                            DutyEnd = flight.ChocksIn,
                            DutyEndLocal = onblock,
                            DutyStart = flight.ChocksOut,
                            DutyStartLocal = offblock,
                            FDPItemId = fdp_item.Id,
                            FlightId = flight.ID,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });


                    }
                }
                if (do_save)
                    context.SaveChanges();
            }
            catch (Exception ex)
            {

            }


        }



        //this method is planningController too
        void AddToCumDuty(FDP fdp, Models.dbEntities context = null)
        {
            try
            {
                bool do_save = context == null;
                if (context == null)
                    context = new Models.dbEntities();
                context.Database.ExecuteSqlCommand("Delete from TableDutyFDP where FDPId=" + fdp.Id);
                var utcdiff = Convert.ToInt32(ConfigurationManager.AppSettings["utcdiff"]);

                if (fdp.DutyType == 1165)
                {
                    var startLocal = ((DateTime)fdp.DateStart).AddMinutes(utcdiff);
                    var endLocal = ((DateTime)fdp.DateEnd).AddMinutes(utcdiff);
                    var startDate = startLocal.Date;
                    var endDate = endLocal.Date;
                    if (startDate == endDate)
                    {
                        var duration = (endLocal - startLocal).TotalMinutes;
                        context.TableDutyFDPs.Add(new TableDutyFDP()
                        {
                            CDate = startDate,
                            CrewId = fdp.CrewId,
                            Duration = duration,
                            DurationLocal = duration,
                            DutyEnd = fdp.DateEnd,
                            DutyEndLocal = endLocal,
                            DutyStart = fdp.DateStart,
                            DutyStartLocal = fdp.DateStart,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });

                    }
                    else
                    {
                        var duration1 = (endDate - startLocal).TotalMinutes;
                        var duration2 = (endLocal - endDate).TotalMinutes;
                        context.TableDutyFDPs.Add(new TableDutyFDP()
                        {
                            CDate = startDate,
                            CrewId = fdp.CrewId,
                            Duration = duration1,
                            DurationLocal = duration1,
                            DutyEnd = fdp.DateEnd,
                            DutyEndLocal = endLocal,
                            DutyStart = fdp.DateStart,
                            DutyStartLocal = fdp.DateStart,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                        context.TableDutyFDPs.Add(new TableDutyFDP()
                        {
                            CDate = endDate,
                            CrewId = fdp.CrewId,
                            Duration = duration2,
                            DurationLocal = duration2,
                            DutyEnd = fdp.DateEnd,
                            DutyEndLocal = endLocal,
                            DutyStart = fdp.DateStart,
                            DutyStartLocal = fdp.DateStart,
                            FDPId = fdp.Id,
                            GUID = Guid.NewGuid()
                        });
                    }




                }
                else
                {
                    double ext = 0;
                    double coef = 0;
                    switch (fdp.DutyType)
                    {
                        case 5000:
                        case 5001:
                        case 300014:
                        case 100001: //meeting
                            coef = 1;
                            break;
                        case 100025:
                        case 100003:
                        case 1167:
                        case 1168:
                        case 300013:
                            coef = 0.25;
                            ext = 1;
                            break;





                        default:
                            coef = 0;
                            break;
                    }
                    var startLocal = ((DateTime)fdp.DateStart).AddMinutes(utcdiff);
                    var endLocal = ((DateTime)fdp.DateEnd).AddMinutes(utcdiff);
                    var startDate = startLocal.Date;
                    var endDate = endLocal.Date;
                    var duration = ((endLocal - startLocal).TotalMinutes + ext) * coef;
                    context.TableDutyFDPs.Add(new TableDutyFDP()
                    {
                        CDate = startDate,
                        CrewId = fdp.CrewId,
                        Duration = duration,
                        DurationLocal = duration,
                        DutyEnd = fdp.DateEnd,
                        DutyEndLocal = endLocal,
                        DutyStart = fdp.DateStart,
                        DutyStartLocal = fdp.DateStart,
                        FDPId = fdp.Id,
                        GUID = Guid.NewGuid()
                    });



                }
                if (do_save)
                    context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }


        static List<FDPMaxDaily> MaxTable = null;
        List<FDPMaxDaily> getMaxFDPTable()
        {
            if (MaxTable == null)
            {
                var context = new Models.dbEntities();
                MaxTable = context.FDPMaxDailies.ToList();
            }
            return MaxTable;
        }

        static List<Extension> ExtensionTable = null;
        List<Extension> getExtensionTable()
        {
            if (ExtensionTable == null)
            {
                var context = new Models.dbEntities();
                ExtensionTable = context.Extensions.ToList();
            }
            return ExtensionTable;
        }



        public int GetMaxFDP2(DateTime reporting, int sectors)
        {

            var MaxFDPTable = getMaxFDPTable();
            var maxfdp = getMaxFDP(reporting, sectors, MaxFDPTable);
            return maxfdp;
        }
        //magu2
        public async Task<dynamic> GetMaxFDPStats(List<int> ids, int? dh = null)
        {
            var context = new Models.dbEntities();
            var MaxFDPTable = getMaxFDPTable(); //await this.context.FDPMaxDailies.ToListAsync();
            //var flights = await this.context.ViewFlightABS.Where(q => ids.Contains(q.ID)).OrderBy(q => q.STDDay).ThenBy(q => q.STD).ThenBy(q => q.Register).ToListAsync();
            //2022-01-23
            var flights = await context.ViewLegTimes.Where(q => ids.Contains(q.ID)).OrderBy(q => q.STDDay).ThenBy(q => q.ChocksOut).ThenBy(q => q.Register).ToListAsync();
         //   var flights2 = flights.OrderBy(q => q.STDDay).ThenBy(q => q.ChocksOut).ThenBy(q => q.Register).ToList();
            foreach (var f in flights)
            {

                if (f.ChocksIn == null)
                    f.ChocksIn = f.STA;
                if (f.ChocksOut == null)
                    f.ChocksOut = f.STD;
            }
            //old 
            //var flights = await this.context.ViewLegTimes.Where(q => ids.Contains(q.ID)).OrderBy(q => q.STDDay).ThenBy(q => q.STD).ThenBy(q => q.Register).ToListAsync();
            MaxFDPStats stat = new MaxFDPStats();
            var rp = Convert.ToInt32(ConfigurationManager.AppSettings["reporting"]);
            stat.ReportingTime = ((DateTime)flights.First().DepartureLocal).AddMinutes(-1 * rp);
            stat.Sectors = ids.Count - (dh == null ? 0 : (int)dh);
            stat.RestFrom = ((DateTime)flights.Last().ArrivalLocal).AddMinutes(double.Parse(ConfigurationManager.AppSettings["post_flight"]));
            var endDate = ((DateTime)flights.Last().ArrivalLocal);

            var _start = stat.ReportingTime.Hour + stat.ReportingTime.Minute * 1.0 / 60;
            var _end = endDate.Hour + endDate.Minute * 1.0 / 60;
            if (stat.ReportingTime.Day != endDate.Day)
                _start = 0;
            double wocl = 0;
            if (_start >= 2 && _start <= 6)
                wocl = Math.Min(6, _end) - _start;
            else if (_end >= 2 && _end <= 6)
                wocl = Math.Min(6, _end) - 2;
            else if (2 > _start && 2 < _end && 6 > _start && 6 < _end)
                wocl = 4;


            stat.RestTo = stat.RestFrom.AddHours(12);
            stat.MaxFDP = getMaxFDP(((DateTime)flights.First().DepartureLocal).AddMinutes(-rp), stat.Sectors, MaxFDPTable);
            stat.WOCL = wocl * 60;
            stat.Extended = 0;
            stat.AllowedExtension = 0;
            bool allowExtension = true;
            stat.WOCLError = 0;
            if (flights.Count >= 5)
                allowExtension = false;
            else if (wocl > 0 && wocl <= 2 && flights.Count > 4)
            {
                allowExtension = false;
                stat.WOCLError = 1;
            }
            else if (wocl > 2 && flights.Count > 2)
            {
                allowExtension = false;
                stat.WOCLError = 1;
            }

            if (wocl > 0 && wocl <= 2 && flights.Count > 4)
            {

                stat.WOCLError = 1;
            }
            else if (wocl > 2 && flights.Count > 2)
            {

                stat.WOCLError = 1;
            }
            //allowExtension = true;
            if (allowExtension)
                for (int i = 1; i < flights.Count; i++)
                {
                    var dt = (DateTime)flights[i].ChocksOut - (DateTime)flights[i - 1].ChocksIn;
                    var minuts = dt.TotalMinutes;

                    var brk = minuts - double.Parse(ConfigurationManager.AppSettings["post_flight"]) - 60;

                    if (brk >= 180 /*&& brk < 600*/)
                    {
                        stat.Extended = 0.5 * brk;
                        break;
                    }
                }

            stat.Flight = flights.Select(q => ((DateTime)q.ChocksIn - (DateTime)q.ChocksOut).TotalMinutes).Sum();
            stat.Duration = ((DateTime)flights.Last().ArrivalLocal - stat.ReportingTime).TotalMinutes;
            if (stat.Duration > stat.MaxFDP && stat.Extended == 0)
            {
                var extTable = getExtensionTable();
                var extend = getExtension(((DateTime)flights.First().DepartureLocal).AddMinutes(-rp), stat.Sectors, extTable);
                if (extend >= stat.Duration)
                    stat.AllowedExtension = getExtension(((DateTime)flights.First().DepartureLocal).AddMinutes(-rp), stat.Sectors, extTable);


            }
            stat.MaxFDPExtended = stat.MaxFDP + stat.Extended;

            stat.Duty = stat.Duration + double.Parse(ConfigurationManager.AppSettings["post_flight"]);


            return stat;
        }
        public int getMaxFDP(DateTime start, int sectors, List<FDPMaxDaily> rows)
        {
            var _start = new DateTime(1900, 1, 1, start.Hour, start.Minute, start.Second);
            var row = rows.FirstOrDefault(q => q.Sectors == sectors && _start >= q.DutyStart && _start <= q.DutyEnd);
            if (row != null)
                return (int)row.MaxFDP;
            else
                return 13 * 60;
        }
        public int getExtension(DateTime start, int sectors, List<Extension> rows)
        {
            var _start = new DateTime(1900, 1, 1, start.Hour, start.Minute, start.Second);
            var row = rows.FirstOrDefault(q => q.Sectors == sectors && _start >= q.DutyStart && _start <= q.DutyEnd);
            if (row != null)
                return (int)row.MaxFDP;
            else
                return 0;
        }


    }

    public class MaxFDPStats
    {
        public int Sectors { get; set; }
        public DateTime ReportingTime { get; set; }

        public DateTime RestTo { get; set; }

        public double MaxFDP { get; set; }
        public double WOCL { get; set; }
        public double WOCLError { get; set; }

        public double Extended { get; set; }

        public double MaxFDPExtended { get; set; }

        public double Flight { get; set; }

        public DateTime RestFrom { get; set; }

        public double Duration { get; set; }
        public double Duty { get; set; }

        public bool IsOver
        {
            get
            {
                return Duration > MaxFDPExtended;
            }
        }

        public int AllowedExtension { get; set; }
    }

    public class eventDto
    {
        public List<int> ids { get; set; }
        public int type { get; set; }
        public string df { get; set; }
        public string dt { get; set; }
        public string tf { get; set; }
        public string tt { get; set; }
        public string remark { get; set; }
    }
    public class FDPResult
    {
        public int Id { get; set; }
        public Nullable<int> CrewId { get; set; }
        public Nullable<System.DateTime> ReportingTime { get; set; }
        public Nullable<System.DateTime> DelayedReportingTime { get; set; }
        public Nullable<System.DateTime> RevisedDelayedReportingTime { get; set; }
        public Nullable<System.DateTime> FirstNotification { get; set; }
        public Nullable<System.DateTime> NextNotification { get; set; }
        public Nullable<int> DelayAmount { get; set; }
        public Nullable<int> BoxId { get; set; }
        public Nullable<int> JobGroupId { get; set; }
        public bool IsTemplate { get; set; }
        public int DutyType { get; set; }
        public Nullable<System.DateTime> DateContact { get; set; }
        public Nullable<int> FDPId { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> TemplateId { get; set; }
        public Nullable<System.DateTime> FDPReportingTime { get; set; }
        public Nullable<System.Guid> GUID { get; set; }
        public Nullable<int> FirstFlightId { get; set; }
        public Nullable<int> LastFlightId { get; set; }
        public Nullable<int> UPD { get; set; }
        public Nullable<bool> IsMain { get; set; }
        public string Key { get; set; }
        public Nullable<bool> CP { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string Remark { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<System.DateTime> InitStart { get; set; }
        public Nullable<System.DateTime> InitEnd { get; set; }
        public Nullable<System.DateTime> InitRestTo { get; set; }
        public string InitFlts { get; set; }
        public string InitRoute { get; set; }
        public string InitFromIATA { get; set; }
        public string InitToIATA { get; set; }
        public string InitNo { get; set; }
        public string InitKey { get; set; }
        public Nullable<int> InitHomeBase { get; set; }
        public string InitRank { get; set; }
        public Nullable<int> InitIndex { get; set; }
        public string InitGroup { get; set; }
        public string InitScheduleName { get; set; }
        public string InitFlights { get; set; }
        public string Remark2 { get; set; }
        public string CanceledNo { get; set; }
        public string CanceledRoute { get; set; }
        public Nullable<int> Extension { get; set; }
        public Nullable<double> Split { get; set; }
        public Nullable<System.DateTime> DateConfirmed { get; set; }
        public string ConfirmedBy { get; set; }
        public string UserName { get; set; }
        public Nullable<decimal> MaxFDP { get; set; }
        public Nullable<int> BL { get; set; }
        public Nullable<int> FX { get; set; }


    }
}
