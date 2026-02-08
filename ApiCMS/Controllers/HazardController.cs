using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using ApiCMS.Models;

namespace ApiCMS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HazardController : ApiController
    {

        public class DataResponse
        {
            public bool IsSuccess { get; set; }
            public object Data { get; set; }
            public object DataExtra { get; set; }
            public List<string> Errors { get; set; }

        }


        public class hazard_dto
        {
            public int id { get; set; }
            public int hazard_type_id { get; set; }
            public string code { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public System.DateTime identified_date { get; set; }
            public Nullable<System.DateTime> last_review_date { get; set; }
            public Nullable<System.DateTime> next_review_due_date { get; set; }
            public bool is_active { get; set; }
            public string risk_owner { get; set; }
            public string responsible_department { get; set; }
            public string initial_severity_code { get; set; }
            public string initial_likelihood_code { get; set; }
            public string initial_risk_level_code { get; set; }
            public string residual_severity_code { get; set; }
            public string residual_likelihood_code { get; set; }
            public string residual_risk_level_code { get; set; }
            public string risk_acceptance_status { get; set; }
            public Nullable<System.DateTime> risk_acceptance_date { get; set; }
            public string risk_accepted_by { get; set; }
            public string risk_matrix_version { get; set; }
            public string created_by { get; set; }
            public System.DateTime created_at { get; set; }
            public string updated_by { get; set; }
            public Nullable<System.DateTime> updated_at { get; set; }
            public byte[] row_version { get; set; }
            public int hazard_status_id { get; set; }
            public Nullable<int> initial_severity_id { get; set; }
            public Nullable<int> initial_likelihood_id { get; set; }
            public Nullable<int> initial_risk_level_id { get; set; }
            public Nullable<int> residual_severity_id { get; set; }
            public Nullable<int> residual_likelihood_id { get; set; }
            public Nullable<int> residual_risk_level_id { get; set; }
            public Nullable<int> risk_matrix_id { get; set; }
        }


        [HttpPost]
        [Route("api/cms/save/hazard")]
        public async Task<DataResponse> SaveHazard(hazard_dto dto)
        {
            try
            {
                var context = new ppa_entities();
                var entity = context.cms2_hazard.FirstOrDefault(q => q.id == dto.id);
                if (entity == null)
                {
                    entity = new cms2_hazard();
                    context.cms2_hazard.Add(entity);
                }

                entity.id = dto.id;
                entity.hazard_type_id = dto.hazard_type_id;
                entity.code = dto.code;
                entity.title = dto.title;
                entity.description = dto.description;
                entity.identified_date = dto.identified_date;
                entity.last_review_date = dto.last_review_date;
                entity.next_review_due_date = dto.next_review_due_date;
                entity.is_active = dto.is_active;
                entity.risk_owner = dto.risk_owner;
                entity.responsible_department = dto.responsible_department;
                entity.initial_severity_code = dto.initial_severity_code;
                entity.initial_likelihood_code = dto.initial_likelihood_code;
                entity.initial_risk_level_code = dto.initial_risk_level_code;
                entity.residual_severity_code = dto.residual_severity_code;
                entity.residual_likelihood_code = dto.residual_likelihood_code;
                entity.residual_risk_level_code = dto.residual_risk_level_code;
                entity.risk_acceptance_status = dto.risk_acceptance_status;
                entity.risk_acceptance_date = dto.risk_acceptance_date;
                entity.risk_accepted_by = dto.risk_accepted_by;
                entity.risk_matrix_version = dto.risk_matrix_version;
                entity.created_by = dto.created_by;
                entity.created_at = dto.created_at;
                entity.updated_by = dto.updated_by;
                entity.updated_at = dto.updated_at;
                entity.row_version = dto.row_version;
                entity.hazard_status_id = dto.hazard_status_id;
                entity.initial_severity_id = dto.initial_severity_id;
                entity.initial_likelihood_id = dto.initial_likelihood_id;
                entity.initial_risk_level_id = dto.initial_risk_level_id;
                entity.residual_severity_id = dto.residual_severity_id;
                entity.residual_likelihood_id = dto.residual_likelihood_id;
                entity.residual_risk_level_id = dto.residual_risk_level_id;
                entity.risk_matrix_id = dto.risk_matrix_id;

                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = dto
                };

            }
            catch (Exception ex)
            {
                return new DataResponse
                {
                    IsSuccess = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }


        [HttpGet]
        [Route("api/cms/get/hazard/{id}")]
        public async Task<DataResponse> GetHazard(int id)
        {
            try
            {
                var context = new ppa_entities();
                var entity = context.cms2_hazard.Select(q => 
                new {
                    q.id,
                q.hazard_type_id,
                q.code,
                q.title,
                q.description,
                q.identified_date,
                q.last_review_date,
                q.next_review_due_date,
                q.is_active,
                q.risk_owner,
                q.responsible_department,
                q.initial_severity_code,
                q.initial_likelihood_code,
                q.initial_risk_level_code,
                q.residual_severity_code,
                q.residual_likelihood_code,
                q.residual_risk_level_code,
                q.risk_acceptance_status,
                q.risk_acceptance_date,
                q.risk_accepted_by,
                q.risk_matrix_version,
                q.created_by,
                q.created_at,
                q.updated_by,
                q.updated_at,
                q.row_version,
                q.hazard_status_id,
                q.initial_severity_id,
                q.initial_likelihood_id,
                q.initial_risk_level_id,
                q.residual_severity_id,
                q.residual_likelihood_id,
                q.residual_risk_level_id,
                q.risk_matrix_id,
            }).FirstOrDefault();



                return new DataResponse
                {
                    IsSuccess = true,
                    Data = entity
                };

            }
            catch (Exception ex)
            {
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = ex
                };
            }
        }


        [HttpGet]
        [Route("api/cms/get/hazard/list")]
        public async Task<DataResponse> GetHazardList()
        {
            try
            {
                var context = new ppa_entities();
                var entity = context.cms2_hazard.Select(q => new {q.id, q.title, q.description, q.initial_risk_level_id, q.residual_risk_level_id, q.hazard_status_id, q.code, q.risk_owner });



                return new DataResponse
                {
                    IsSuccess = true,
                    Data = entity
                };

            }
            catch (Exception ex)
            {
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = ex
                };
            }
        }
    }
}