using AirpocketTRN.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AirpocketTRN.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TrainingController : ApiController
    {
        TrainingService trainingService = null;

        public TrainingController()
        {
            trainingService = new TrainingService();
        }


        [Route("api/trn/get/crm/assessment/{flightId}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_crm_assessment(int flightId)
        {
            var result = await trainingService.get_trn_crm_assessment(flightId);
            return Ok(result);
        }

        [Route("api/trn/save/crm/assessment")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> get_crm_assessment(dynamic dto)
        {
            var result = await trainingService.save_trn_crm_assessment(dto);
            return Ok(result);
        }

        [Route("api/trn/get/efb/assessment/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_efb_assessment(int flight_id)
        {
            var result = await trainingService.get_trn_efb_assessment(flight_id);
            return Ok(result);
        }

        [Route("api/trn/save/efb/assessment")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> save_efb_assessment(dynamic dto)
        {
            var result = await trainingService.save_trn_efb_assessment(dto);
            return Ok(result);
        }


        [Route("api/trn/get/fstd/crm/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_fstd_crm(int flight_id)
        {
            var result = await trainingService.get_trn_fstd_crm(flight_id);
            return Ok(result);
        }

        [Route("api/trn/save/fstd/crm")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> save_fstd_crm(dynamic dto)
        {
            var result = await trainingService.save_trn_fstd_crm(dto);
            return Ok(result);
        }

        [Route("api/trn/get/grt/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_trn_grt(int flight_id)
        {
            var result = await trainingService.get_trn_grt(flight_id);
            return Ok(result);
        }

        [Route("api/trn/save/grt")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> save_trn_grt(dynamic dto)
        {
            var result = await trainingService.save_trn_grt(dto);
            return Ok(result);
        }


        [Route("api/trn/get/instructor/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_trn_instructor(int flight_id)
        {
            var result = await trainingService.get_trn_instructor(flight_id);
            return Ok(result);
        }

        [Route("api/trn/save/instructor")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> save_trn_instructor(dynamic dto)
        {
            var result = await trainingService.save_trn_instructor(dto);
            return Ok(result);
        }


        [Route("api/trn/get/line/check/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_trn_line_check(int flight_id)
        {
            var result = await trainingService.get_trn_line_check(flight_id);
            return Ok(result);
        }

        [Route("api/trn/save/line/check")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> save_trn_line_check(dynamic dto)
        {
            var result = await trainingService.save_trn_line_check(dto);
            return Ok(result);
        }

        [Route("api/trn/get/line/crm/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_trn_line_crm(int flight_id)
        {
            var result = await trainingService.get_trn_line_crm(flight_id);
            return Ok(result);
        }

        [Route("api/trn/save/line/crm")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> save_trn_line_crm(dynamic dto)
        {
            var result = await trainingService.save_trn_line_crm(dto);
            return Ok(result);
        }




        [Route("api/trn/get/zftt/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_trn_zftt(int flight_id)
        {
            var result = await trainingService.get_trn_zftt(flight_id);
            return Ok(result);
        }

        [Route("api/trn/save/zftt")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> save_trn_zft(dynamic dto)
        {
            var result = await trainingService.save_trn_zftt(dto);
            return Ok(result);
        }

        [Route("api/trn/get/fstd/{flight_id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_trn_fstd(int flight_id)
        {
            var result = await trainingService.get_trn_fstd(flight_id);
            return Ok(result);
        }

        [Route("api/trn/save/fstd")]
        [AcceptVerbs("Post")]

        public async Task<IHttpActionResult> save_trn_fstd(dynamic dto)
        {
            var result = await trainingService.save_trn_fstd(dto);
            return Ok(result);
        }
    }
}