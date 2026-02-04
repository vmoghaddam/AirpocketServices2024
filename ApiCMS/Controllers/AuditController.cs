using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using ApiCMS.Models;

namespace ApiCMS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuditController : ApiController
    {

        public class DataResponse
        {
            public bool IsSuccess { get; set; }
            public object Data { get; set; }
            public object DataExtra { get; set; }
            public List<string> Errors { get; set; }

        }


        [HttpGet]
        [Route("api/cms/get/audit/{id}")]

        public async Task<DataResponse> GetAuditForm(int id)
        {
            try
            {
                var context = new ppa_entities();
                var entity = context.cms2_audit.FirstOrDefault(q => q.id == id);

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = entity
                };

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


        public class audit_dto
        {
            public int id { get; set; }
            public string code { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public int auditee_id { get; set; }
            public int type_id { get; set; }
            public int location_id { get; set; }
            public string scope { get; set; }
            public string objective { get; set; }
            public string audit_team { get; set; }
            public string lead_auditor { get; set; }
            public Nullable<System.DateTime> audit_date { get; set; }
            public Nullable<System.DateTime> audit_close_date { get; set; }
            public string standards_refrences { get; set; }
            public int created_by { get; set; }
            public System.DateTime created_at { get; set; }
            public Nullable<int> updated_by { get; set; }
            public Nullable<System.DateTime> updated_at { get; set; }
        }


        [HttpPost]
        [Route("api/cms/save/audit")]

        public async Task<DataResponse> GetAuditForm(audit_dto dto)
        {
            try
            {
                var context = new ppa_entities();
                var entity = await context.cms2_audit.FirstOrDefaultAsync(q => q.id == dto.id);
                if (entity == null)
                {
                    entity = new cms2_audit();
                    context.cms2_audit.Add(entity);
                }

                entity.code = dto.code;
                entity.title = dto.title;
                entity.description = dto.description;
                entity.auditee_id = dto.auditee_id;
                entity.type_id = dto.type_id;
                entity.location_id = dto.location_id;
                entity.scope = dto.scope;
                entity.objective = dto.objective;
                entity.audit_date = dto.audit_date;
                entity.audit_close_date = dto.audit_close_date;
                entity.standards_refrences = dto.standards_refrences;
                entity.created_by = dto.created_by;
                entity.created_at = dto.created_at;
                entity.updated_by = dto.updated_by;
                entity.updated_at = dto.updated_at;

                await context.SaveChangesAsync();

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = null
                };

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

    }
}