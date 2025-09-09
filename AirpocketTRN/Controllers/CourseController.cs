using AirpocketTRN.Models;
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
using AirpocketTRN.Services;
using System.Web.Http.OData;
using AirpocketTRN.ViewModels;
using System.Web;
using static AirpocketTRN.Services.CourseService;

namespace AirpocketTRN.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CourseController : ApiController
    {
        CourseService courseService = null;

        public CourseController()
        {
            courseService = new CourseService();
        }


        [Route("api/notam")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetNotam()
        {
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://notams.aim.faa.gov/notamSearch/search");

                request.Headers.Add("Accept", "application/json, text/plain, */*");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("Pragma", "no-cache");
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Site", "same-origin");

                string body = "searchType=0&designatorsForLocation=OIII,OIMM%2COIMM&designatorForAccountable=&latDegrees=&latMinutes=0&latSeconds=0&longDegrees=&longMinutes=0&longSeconds=0&radius=10&sortColumns=5+false&sortDirection=true&designatorForNotamNumberSearch=&notamNumber=&radiusSearchOnDesignator=false&radiusSearchDesignator=&latitudeDirection=N&longitudeDirection=W&freeFormText=&flightPathText=&flightPathDivertAirfields=&flightPathBuffer=4&flightPathIncludeNavaids=true&flightPathIncludeArtcc=false&flightPathIncludeTfr=true&flightPathIncludeRegulatory=false&flightPathResultsType=All+NOTAMs&archiveDate=&archiveDesignator=&offset=0&notamsOnly=false&filters=&minRunwayLength=&minRunwayWidth=&runwaySurfaceTypes=&predefinedAbraka=&predefinedDabra=&flightPathAddlBuffer=&recaptchaToken=YOUR_RECAPTCHA_TOKEN";

                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                // Console.WriteLine(responseBody);
                var jsn=JsonConvert.DeserializeObject<NotamResponse>(responseBody);
                return Ok(jsn);
            }
        }

        [Route("api/trn/stat/coursepeople")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTrnStatCoursePeople(string df, string dt, int? ct, int? status, int? cstatus, string cls, int? pid,int? inst1,int? inst2,int? rank,int? active,string grp,string dep)
        {
            var fp = df.Split('-').Select(q=>Convert.ToInt32(q)).ToList();
            var ft = dt.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var _df = new DateTime(fp[0], fp[1], fp[2]);
            var _dt = new DateTime(ft[0], ft[1], ft[2]);
            var result = await courseService.GetTrnStatCoursePeople(_df,_dt,ct,status,cstatus,cls,pid,inst1,inst2,rank,active,grp,dep);

            return Ok(result);
        }

        string build_exception_message(Exception ex)
        {
            var msg = ex.Message;
            if (ex.InnerException != null)
                msg += " INNER: " + ex.InnerException.Message;
            return msg;
        }

        [Route("api/course/types")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseTypes()
        {
            try
            {
                var result = await courseService.GetCourseTypes();

                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(build_exception_message(ex));
            }
           
        }
        //GetCourseTypeGroupsProfile
        [Route("api/profile/course/types")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseTypeGroupsProfile()
        {
            try
            {
                var result = await courseService.GetCourseTypeGroupsProfile();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(build_exception_message(ex));
            }

        }
        //GetCourseTypeJobGroups
        [Route("api/course/type/groups/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseTypeJobGroups(int cid)
        {
            var result = await courseService.GetCourseTypeJobGroups(cid);

            return Ok(result);
        }
         
       [Route("api/course/type/subject/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseTypeSubject(int cid)
        {
            var result = await courseService.GetCourseTypeSubject(cid);

            return Ok(result);
        }

        [Route("api/course/type/groups/group/{gid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseTypeJobGroupsByGrouup(int gid)
        {
            var result = await courseService.GetCourseTypeJobGroupsByGroup(gid);

            return Ok(result);
        }


        [Route("api/certificate/types")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificateTypes()
        {
            var result = await courseService.GetCertificateTypes();

            return Ok(result);
        }

        [Route("api/certificate/types/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<CertificateType> GetPeople()
        {

            return courseService.GetCertificateTypesQuery();
        }

        [Route("api/course/types/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewCourseType> GetcourseTypesQuery()
        {

            return courseService.GetCourseTypesQuery();
        }

       

        [Route("api/jobgroup")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewJobGroup> GetViewJobGroupQuery()
        {

            return courseService.GetViewJobGroupQuery();
        }

        [Route("api/course/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewCourseNew> GetCourseQuery()
        {
           // if (DateTime.Now >= new DateTime(2022, 6, 28))
             //   return (new List<ViewCourseNew>()).AsQueryable();
            return courseService.GetCourseQuery();
        }
        [Route("api/course/bytype/{tid}/{sid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursesByType(int tid,int sid)
        {
            var result = await courseService.GetCoursesByType(tid,sid);

            return Ok(result);
        }
        [Route("api/course/bytype/outside/{tid}/{sid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursesByTypeOutSide(int tid, int sid)
        {
            var result = await courseService.GetCoursesByTypeOutSide(tid, sid);

            return Ok(result);
        }
        [Route("api/course/types/subjects/{id}")]
        [EnableQuery]
        // [Authorize]
        public async Task<IHttpActionResult> GetcourseTypesSubjects(int id)
        {
            var result = await courseService.GetCourseTypeSubjects(id);
            return Ok(result);
        }
        [Route("api/certificates/history/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificatesHistory(int pid)
        {
            var result = await courseService.GetCertificatesHistory(pid);

            return Ok(result.Data);
        }

        [Route("api/trainingcard/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTrainingCard(int pid)
        {
            var result = await courseService.GetTrainingCard(pid);

            return Ok(result.Data);
        }
        [Route("api/courses/passed/history/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursesPassedHistory(int pid)
        {
            var result = await courseService.GetCoursesPassedHistory(pid);

            return Ok(result.Data);
        }



        [Route("api/groups/main")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetMainGroups()
        {
            var result = await courseService.GetMainJobGroups();

            return Ok(result.Data);
        }

        [Route("api/employees/{root}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetEmployees(string root)
        {
            var result = await courseService.GetEmployees(root);

            return Ok(result.Data);
        }

        [Route("api/expiring/ct/")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetExpiringCT()
        {
            var result = await courseService.GetGRPCTExpiring();

            return Ok(result.Data);
        }
        [Route("api/expiring/ct/main/group/{main}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetExpiringCTByMainGroup(string main)
        {
            var result = await courseService.GetGRPCTExpiringByMainGroup(main);

            return Ok(result.Data);
        }

        [Route("api/expiring/ct/main/{type}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetExpiringCTMain(int type)
        {
            var result = await courseService.GetGRPCTExpiringMainGroups(type);

            return Ok(result.Data);
        }
        [Route("api/expiring/ct/group/{type}/{group}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetExpiringCTGroup(int type,string group)
        {
            var result = await courseService.GetGRPCTExpiringGroups(group, type);

            return Ok(result.Data);
        }

        [Route("api/groups/main/expiring/{main}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetGroupsMainExpiring(string main)
        {
            var result = await courseService.GetGRPMainGroupsExpiring(main);

            return Ok(result.Data);
        }
        [Route("api/groups/expiring/{main}/{group}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetGroupsExpiring(string main,string group)
        {
            var result = await courseService.GetGRPGroupsExpiring(main, group);

            return Ok(result.Data);
        }

        [Route("api/groups/expiring/course/types/{main}/{group}/{type}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetGroupsExpiringCourseTypes(string main, string group,int type)
        {
            var result = await courseService.GetGRPGroupsExpiringCourseTypes(main, group,type);

            return Ok(result.Data);
        }


        [Route("api/trn/monitoring/expiring")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetMonitoringExpiringGroups()
        {
            var result = await courseService.GetMonitoringExpiringGroups();

            return Ok(result.Data);
        }
        [Route("api/trn/monitoring/expiring/type")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetMonitoringExpiringCertificateTypes()
        {
            var result = await courseService.GetMonitoringExpiringCertificateTypes();

            return Ok(result.Data);
        }

        [Route("api/trn/monitoring/expiring/main")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetMonitoringExpiringMain( )
        {
            var result = await courseService.GetMonitoringExpiringMain();

            return Ok(result.Data);
        }
        [Route("api/trn/monitoring/expiring/{parent_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetMonitoringExpiringMainByParent(int parent_id)
        {
            var result = await courseService.GetMonitoringExpiringMainByParent(parent_id);

            return Ok(result.Data);
        }

        [Route("api/trn/schedule/{year}/{month}/{mng_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTrainingSchedule(int year,int month,int mng_id)
        {
            var result = await courseService.GetTrainingSchedule(  year,  month,mng_id);

            return Ok(result.Data);
        }

        [Route("api/trn/schedule/year/{year}/{mng_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTrainingScheduleYear(int year,int mng_id )
        {
            var result = await courseService.GetTrainingSchedule(year,mng_id );

            return Ok(result.Data);
        }

        //GetTrainingExpiredCertificateTypes
        [Route("api/trn/schedule/year/type/{year}/{type}/{mng_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTrainingExpiredCertificateTypes(int year, int type,int mng_id)
        {
            var result = await courseService.GetTrainingExpiredCertificateTypes(year, type,mng_id);

            return Ok(result.Data);
        }

        [Route("api/trn/schedule/year/month/type/{year}/{month}/{type}/{mng_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTrainingExpiredCertificateTypes(int year,int month, int type, int mng_id)
        {
            var result = await courseService.GetTrainingExpiredCertificateTypes(year,month, type, mng_id);

            return Ok(result.Data);
        }



        [Route("api/trn/schedule/year/person/{year}/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTrainingExpiredCertificateTypesByPerson(int year, int pid)
        {
            var result = await courseService.GetTrainingExpiredCertificateTypesByPerson(year, pid);

            return Ok(result.Data);
        }

        [Route("api/trn/schedule/year/month/person/{year}/{month}/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTrainingExpiredCertificateTypesByPerson(int year, int month, int pid)
        {
            var result = await courseService.GetTrainingExpiredCertificateTypesByPerson(year, month, pid);

            return Ok(result.Data);
        }


        [Route("api/trn/dashboard/report/type")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetRptCourseType(DateTime df,DateTime dt)
        {
            var result = await courseService.GetRptCourseType(df, dt);

            return Ok(result.Data);
        }
        [Route("api/trn/dashboard/report/group/{ct}/{jg}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetRptCourseJobGroup(DateTime df, DateTime dt,int ct,string jg)
        {
            var result = await courseService.GetRptCourseJobGroup(df, dt,ct,jg);

            return Ok(result.Data);
        }

        [Route("api/trn/dashboard/report/person/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetRptCoursePerson(DateTime df, DateTime dt, int pid)
        {
            var result = await courseService.GetRptCoursePerson(df, dt, pid);

            return Ok(result.Data);
        }

        [Route("api/trn/dashboard/report/summary")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetRptSummary(DateTime df, DateTime dt )
        {
            var result = await courseService.GetRptSummary(df, dt );

            return Ok(result.Data);
        }



        [Route("api/profiles/abs/{grp}")]

        public async Task<IHttpActionResult> GetProfilesByCustomerId(  string grp)
        {
            var result = await courseService.GetProfilesAbs(grp);

            return Ok(result.Data);

        }
        //GetCertificatesHistory
        [Route("api/course/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourse (int cid)
        {
            var result = await courseService.GetCourse (cid);

            return Ok(result);
        }
        [Route("api/course/view/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseView(int cid)
        {
            var result = await courseService.GetCourseView(cid);

            return Ok(result);
        }

        [Route("api/course/view/object/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseViewObject(int cid)
        {
            var result = await courseService.GetCourseViewObject(cid);

            return Ok(result);
        }

        [Route("api/course/sessions/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseSessions(int cid)
        {
            var result = await courseService.GetCourseSessions(cid);

            return Ok(result);
        }
        //SyncSessionsToRoster
        [Route("api/course/sessions/sync")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSessionssync(dynamic dto)
        {
            int cid = Convert.ToInt32(dto.id);
            // var result = await courseService.SyncSessionsToRoster(cid);
            var result = await courseService.SyncSessionsToRosterByDate(cid);

            return Ok(result);
        }

        [Route("api/course/sessions/sync/get/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseSessionssync(int id)
        {
             
            var result = await courseService.SyncSessionsToRosterByDate(id);

            return Ok(result);
        }

        [Route("api/course/sessions/sync/teacher/get/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseSessionssyncTeachers(int id)
        {

            var result = await courseService.SyncSessionsToRosterTeachers(id);

            return Ok(result);
        }
        //GetCoursePeopleAndSessions
        [Route("api/course/peoplesessions/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursePeopleSessions(int cid)
        {
            var result = await courseService.GetCoursePeopleAndSessions(cid);

            return Ok(result);
        }


       



        [Route("api/course/person/date")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursePeopleSessionsByDate(DateTime dt,int pid)
        {
            var result = await courseService.GetCoursePeopleAndSessionsByDate(dt,pid);

            return Ok(result);
        }

        [Route("api/course/attendance/{cid}/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseAttendance(int cid,int pid)
        {
            var result = await courseService.GetCourseAttendance(cid,pid);

            return Ok(result);
        }


        [Route("api/course/attendance/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostAttendanceSave(Attendance att)
        {

            var result = await courseService.SaveCourseAttendance(att);

            return Ok(result);
        }

        [Route("api/course/attendance/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostAttendanceDelete(dynamic dto)
        {
            int id = Convert.ToInt32(dto.Id);
            var result = await courseService.DeleteCourseAttendance(id);

            return Ok(result);
        }



        //GetPersonCourses
        [Route("api/person/courses/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetPersonCourses(int pid)
        {
            var result = await courseService.GetPersonCourses(pid);

            return Ok(result);
        }

        [Route("api/person/courses/mandatory/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetPersonMandatoryCourses(int pid)
        {
            var result = await courseService.GetPersonMandatoryCourses(pid);

            return Ok(result);
        }

        [Route("api/courses/mandatory/people/{type}/{group}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetPersonMandatoryCourses(int type,int group)
        {
            //var result = await courseService.GetPersonMandatoryCoursesByType(type,group);
            var result = await courseService.GetCertificateHistoryByTypeGroup(type, group);

            return Ok(result);
        }

        [Route("api/courses/mandatory/people/type/{type}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetPersonMandatoryCoursesByType(int type)
        {
            //var result = await courseService.GetPersonMandatoryCoursesByType(type,group);
            var result = await courseService.GetCertificateHistoryByType(type);

            return Ok(result);
        }

        //GetCertificateHistory_Expiring
        [Route("api/expiring")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificateHistory_Expiring( )
        {
            //var result = await courseService.GetPersonMandatoryCoursesByType(type,group);
            var result = await courseService.GetCertificateHistory_Expiring( );

            return Ok(result);
        }
        [Route("api/expiring/people/{type}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificateHistory_Expiring_People(string type)
        {
            //var result = await courseService.GetPersonMandatoryCoursesByType(type,group);
            var result = await courseService.GetCertificateHistory_Expiring_People(type);

            return Ok(result);
        }


        [Route("api/expiring/new")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetStat()
        {
            //var result = await courseService.GetPersonMandatoryCoursesByType(type,group);
            var result = await courseService.GetStat();

            return Ok(result);
        }


        [Route("api/course/people/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursePeople(int cid)
        {
            var result = await courseService.GetCoursePeople(cid);

            return Ok(result);
        }
        //GetCoursePeopleNames
        [Route("api/course/people/names/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursePeopleNames(int cid)
        {
            var result = await courseService.GetCoursePeopleNames(cid);

            return Ok(result);
        }

        [Route("api/course/types/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseType(ViewModels.CourseTypeViewModel dto)
        {
            var result = await courseService.SaveCourseType(dto);

            return Ok(result);
        }
        [Route("api/course/type/notapplicable/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseTypeApplicable( CourseService.course_type_notapplicable_viewmodel  dto)
        {
            var result = await courseService.SaveCourseTypeNotApplicable(dto);

            return Ok(result);
        }
        [Route("api/course/type/notapplicable/{ctid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseTypeNotApplicable(int ctid)
        {
            var result = await courseService.GetCourseTypeNotApplicable(ctid);

            return Ok(result);
        }

        [Route("api/course/type/group/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseTypeGroup(dynamic dto)
        {
            int tid = Convert.ToInt32(dto.tid);
            int gid = Convert.ToInt32(dto.gid);
            int man = Convert.ToInt32(dto.man);
            int sel = Convert.ToInt32(dto.sel);
            var result = await courseService.SaveCourseTypeJobGroup(tid, gid, man, sel);

            return Ok(result);
        }

        [Route("api/course/types/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseTypeDelete(dynamic dto)
        {
            int id = Convert.ToInt32(dto.Id);
            var result = await courseService.DeleteCourseType(id);

            return Ok(result);
        }

        [Route("api/course/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourse(ViewModels.CourseViewModel dto)
        {
            var result = await courseService.SaveCourse(dto);

            return Ok(result);
        }
        [Route("api/course/save/score")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseScore(exam_score dto)
        {
            var result = await courseService.SaveExamScore(dto);

            return Ok(result);
        }
        [Route("api/course/documents/{id}")]

        public async Task<IHttpActionResult> GetCourseDocs(int id)
        {

            var result = await courseService.GetCourseDocs(id);

            return Ok(result);
        }
        [Route("api/course/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseDelete(dynamic dto)
        {
            int id = Convert.ToInt32(dto.Id);
            var result = await courseService.DeleteCourse(id);

            return Ok(result);
        }
        //DeleteCoursePeople
        [Route("api/course/people/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostDeleteCoursePeople(dynamic dto)
        {
            int pid = Convert.ToInt32(dto.pid);
            int cid = Convert.ToInt32(dto.cid);
            var result = await courseService.DeleteCoursePeople(pid,cid);

            return Ok(result);
        }

        [Route("api/certificate/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCertificate(ViewModels.CertificateViewModel dto)
        {
            var result = await courseService.SaveCertificate(dto);

            return Ok(result);
        }
        [Route("api/certificate/atlas/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCertificateAtlas(ViewModels.CertificateViewModel dto)
        {
            var result = await courseService.SaveCertificateAtlas(dto);

            return Ok(result);
        }

        [Route("api/course/people/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCoursePeople(dynamic dto)
        {
            var result = await courseService.SaveCoursePeople(dto);

            return Ok(result);
        }
        //CopyCoursePeople
        [Route("api/course/people/copy")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCoursePeopleCopy(dynamic dto)
        {
            var result = await courseService.CopyCoursePeople(dto);

            return Ok(result);
        }
        //SaveCourseSessionPresence
        [Route("api/course/session/pres/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSessionPresence(dynamic dto)
        {
            var result = await courseService.SaveCourseSessionPresence(dto);

            return Ok(result);
        }

        [Route("api/course/session/pres/save/all")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSessionPresenceAll(cspg_dto dto)
        {
            var result = await courseService.SaveCourseSessionPresenceAll(dto);

            return Ok(result);
        }

        [Route("api/course/exam/result/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseExamResult(dto_exam_result dto)
        {
            var result = await courseService.UpdateExamResult(dto);

            return Ok(result);
        }

        [Route("api/course/exam/sign")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseExamSign(dto_exam_sign dto)
        {
            var result = await courseService.UpdateExamSign(dto);

            return Ok(result);
        }

        [Route("api/course/sign/director")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSignDirector(dto_exam_sign dto)
        {
            var result = await courseService.UpdateCourseSignDirector(dto);

            return Ok(result);
        }
        [Route("api/course/sign/staff")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSignStaff(dto_exam_sign dto)
        {
            var result = await courseService.UpdateCourseSignStaff(dto);

            return Ok(result);
        }
        [Route("api/course/sign/ops")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSignOps(dto_exam_sign dto)
        {
            var result = await courseService.UpdateCourseSignOPS(dto);

            return Ok(result);
        }

        [Route("api/course/sign/")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSign(dto_exam_sign dto)
        {
            var result = await courseService.UpdateCourseSign(dto);

            return Ok(result);
        }

        //UpdateCoursePeopleSignDirector
        [Route("api/cp/sign/director")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCPSignDirector(dto_exam_sign dto)
        {
            var result = await courseService.UpdateCoursePeopleSignDirector(dto);

            return Ok(result);
        }
        [Route("api/cp/sign/staff")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCPSignStaff(dto_exam_sign dto)
        {
            var result = await courseService.UpdateCoursePeopleSignStaff(dto);

            return Ok(result);
        }

        [Route("api/cp/sign/")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCPSignIns(dto_exam_sign dto)
        {
            var result = await courseService.UpdateCoursePeopleSign(dto);

            return Ok(result);
        }

        [Route("api/cp/sign/ops")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCPSignOPS(dto_exam_sign dto)
        {
            var result = await courseService.UpdateCoursePeopleSignOPS(dto);

            return Ok(result);
        }

        [Route("api/course/syllabus/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSyllabus(dynamic dto)
        {
            var result = await courseService.SaveSyllabus(dto);

            return Ok(result);
        }

        //UpdateCoursePeopleStatus(CoursePeopleStatusViewModel dto)
        [Route("api/course/people/status/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostUpdateCoursePeopleStatus(CoursePeopleStatusViewModel dto)
        {
            var result = await courseService.UpdateCoursePeopleStatus(dto);

            return Ok(result);
        }

        [Route("api/course/people/status/all/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostUpdateCoursePeopleStatusAll(CoursePeopleStatusViewModel dto)
        {
            var result = await courseService.UpdateCoursePeopleStatusAll(dto);

            return Ok(result);
        }

        [Route("api/course/people/status/atlas/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostUpdateCoursePeopleStatusAtlas(CoursePeopleStatusViewModel dto)
        {
            var result = await courseService.UpdateCoursePeopleStatusAtlas(dto);

            return Ok(result);
        }

        [Route("api/crew/certificates/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetEmployeeCertificates(int id)
        {
            var result = await courseService.GetEmployeeCertificates(id);

            return Ok(result.Data);
        }


        [Route("api/teacher/courses/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTeacherCourses(int id)
        {
            var result = await courseService.GetTeacherCourses(id);

            return Ok(result.Data);
        }

        [Route("api/instructor/courses/active/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetInstructorActiveCourses(int id)
        {
            var result = await courseService.GetTeacherActiveCourses(id);

            return Ok(result.Data);
        }
        [Route("api/instructor/courses/archived/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetTeacherArchivedCourses(int id)
        {
            var result = await courseService.GetTeacherArchivedCourses(id);

            return Ok(result.Data);
        }


        [Route("api/director/courses/active/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetDirectorActiveCourses(int id)
        {
            var result = await courseService.GetDirectorActiveCourses(id);

            return Ok(result.Data);
        }

        [Route("api/director/courses/archived/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetDirectorArchivedCourses(int id)
        {
            var result = await courseService.GetDirectorArchivedCourses(id);

            return Ok(result.Data);
        }

        [Route("api/exam/student/answer")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostExamStudentAnswer(dto_exam_student_answer dto)
        {
            var result = await courseService.UpdateExamStudentAnswer(dto);

            return Ok(result.Data);
        }

        [Route("api/sms/test")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetSMSTest()
        {
            //var result = await courseService.GetPersonMandatoryCoursesByType(type, group);
            Magfa m = new Magfa();
            var res = m.enqueue(1, "09124449584", "Hi")[0];

            return Ok(res);
        }
        [Route("api/course/notify/{id}/{recs}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetNotifyCourse(int id,string recs)
        {
            var result = await courseService.NotifyCoursePeople(id,recs);

            return Ok(result.Data);
        }

        [Route("api/course/notify/teacher/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetNotifyTeacherCourse(int id)
        {
            var result = await courseService.NotifyCourseTeachers(id);

            return Ok(result.Data);
        }


        [Route("api/certificate/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificate(int id)
        {
            var result = await courseService.GetCertificate(id);

            return Ok(result.Data);
        }

        [Route("api/certificate/flykish/all")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificateFlyKish()
        {
            var result = await courseService.GetCertificateAll();

            return Ok(result.Data);
        }

        [Route("api/certificate/course/all/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificateCourseAll(int id)
        {
            var result = await courseService.GetCertificateCourseAll(id);

            return Ok(result.Data);
        }

        [Route("api/certificate/people/all/{ids}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificatePeopleAll(string ids)
        {
            var result = await courseService.GetCertificatePeopleAll(ids);

            return Ok(result.Data);
        }

        [Route("api/upload/certificate/{id}/{pid}/{tid}/{cid}")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Upload(int id,int pid,int tid,int cid )
        {
            IHttpActionResult outPut = Ok(200);
             
            FLYEntities context = new FLYEntities();
            ViewCoursePeople cp = null;
            if (id != -1)
                cp = await context.ViewCoursePeoples.Where(q => q.Id == id).FirstOrDefaultAsync();
            else
                cp = await context.ViewCoursePeoples.Where(q => q.PersonId==pid && q.CourseId==cid).FirstOrDefaultAsync();

            var entity = await context.CoursePeoples.Where(q => q.Id == cp.Id).FirstOrDefaultAsync();
            string key = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            string folder = ConfigurationManager.AppSettings["files_certificates"];
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var date = DateTime.Now;
                    var ext = System.IO.Path.GetExtension(postedFile.FileName);
                    //key = date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + ext;
                    if (cp.CertificateTypeId != null)
                    {
                        key = "FPC-" + cp.PersonId.ToString() + "-" + cp.CertificateTypeId.ToString() + ext;
                        var filePath =folder+key;
                        postedFile.SaveAs(filePath);
                    }
                    key = "FPC-" + cp.Id + ext;
                    var filePath2 = folder + key;
                    postedFile.SaveAs(filePath2);
                    entity.ImgUrl = key;
                    await context.SaveChangesAsync();
                    outPut =Ok( key);
                }
                

            }
            else
            {
                outPut =Ok( "-1");
            }
            return outPut;
        }


        [Route("api/upload/pf")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> UploadPF()
        {
            IHttpActionResult outPut = Ok(200);

            
            string key = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            string folder = ConfigurationManager.AppSettings["files_certificates"];
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var date = DateTime.Now;
                    var ext = System.IO.Path.GetExtension(postedFile.FileName);
                    key = date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + ext;
                   
                    key = "PF-" + key;
                    var filePath2 = folder + key;
                    postedFile.SaveAs(filePath2);
                    
                    outPut = Ok(key);
                }


            }
            else
            {
                outPut = Ok("-1");
            }
            return outPut;
        }
        // FLYEntities context = new FLYEntities();
        //[Route("api/manager/groups/{mng_id}")]
        //[AcceptVerbs("GET")]
        //public async Task<IHttpActionResult> GetManagerGroups(int mng_id)
        //{
        //    FLYEntities context = new FLYEntities();
        //    List<string> result = new List<string>();
        //    if (mng_id != -1)
        //    {
        //        var mng = await context.Managers.FirstOrDefaultAsync(q => q.EmployeeId == mng_id);
        //        result = await context.ManagerGroups.Where(q => q.ManagerId == mng_id).Select(q => q.ProfileGroup).ToListAsync();

                
        //    }

        //    return Ok(result);
        //}

        [Route("api/certificate/url/{pid}/{tid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificateUrl(int pid,int tid)
        {
            var result = await courseService.GetCertificateUrl(pid, tid);

            return Ok(result);
        }

        [Route("api/document/url/{pid}/{tid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetDocumentUrl(int pid, int tid)
        {
            var result = await courseService.GetPersonDocumentFile(pid, tid);

            return Ok(result);
        }

        [Route("api/certificate/obj/{pid}/{tid}/{str}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificateObj(int pid, int tid,string str)
        {
            var result = await courseService.GetPersonCertificateDocument(pid, tid,str);

            return Ok(result);
        }


        [Route("api/person/doc/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEmployeeDoc(dynamic dto)
        {
            int personId = Convert.ToInt32(dto.personId);
            string type = Convert.ToString(dto.type);
            string url = Convert.ToString(dto.url);
            var result = await courseService.SavePersonDoc(personId, type, url);

            return Ok(result);
        }

        [Route("api/course/pf/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseFP(dynamic dto)
        {
            int courseId = Convert.ToInt32(dto.courseId);
            
            string url = Convert.ToString(dto.url);
            var result = await courseService.SaveCourseFP(courseId, url);

            return Ok(result);
        }


        [Route("api/groups/manager/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostGroupsManager(dynamic dto)
        {
            int managerId = Convert.ToInt32(dto.managerId);

            string ids = Convert.ToString(dto.ids);
            var _ids = ids.Split('_').Select(q => Convert.ToInt32(q)).ToList();
            var result = await courseService.SaveGroupsManager(managerId, _ids);

            return Ok(result);
        }


        [Route("api/course/remaining/{dd}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseRemaining(int dd)
        {
            var result = await courseService.GetCourseRemaining(dd);

            return Ok(result.Data);
        }

        [Route("api/course/remaining/notify/{dd}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseRemainingNotify(int dd)
        {
            var result = await courseService.NotifyCourseRemaining(dd);

            return Ok(result.Data);
        }
        [Route("api/course/remaining/email/{dd}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseRemainingEmail(int dd)
        {
            var result = await courseService.EmailCourseRemaining(dd);

            return Ok(result.Data);
        }

        [Route("api/mail/{port}/{ssl}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult TESTEMAIL(int port, int ssl)
        {


            var helper = new MailHelper();
            var result = helper.SendTest("v.moghaddam59@gmail.com","TEST", "TEST", port, ssl);

            return Ok(result);
        }



        [Route("api/course/ids")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseIds()
        {
            

            FLYEntities context = new FLYEntities();
            var ids = await context.Courses.OrderByDescending(q => q.Id).Select(q => q.Id).ToListAsync();
            return Ok(ids);
        }


        [Route("api/trn/manager/groups/{mng_id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetManagerGroups(  int mng_id)
        {
            var result = await courseService.GetManagerGroups(  mng_id);

            return Ok(result.Data);
        }






    }
}
