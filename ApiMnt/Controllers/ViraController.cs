using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.WebSockets;
using ApiMnt.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Tokenizer;



namespace ApiMnt.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ViraController : ApiController
    {

        public string url_auth = "https://lmmcore.online/api/Authenticate";
        public string url_cartable = "https://lmmcore.online/api/LGSRequest/GetRequestCartable?page=1&size=10000";
        public string url_cartable_item = "https://lmmcore.online/api/LGSRequest/GetRequestItemCartable?id=";
        public string url_stock = "https://lmmcore.online/api/LGSStockManagement/GetStockPaper";
        public string url_stock_item = "https://lmmcore.online/api/LGSStockManagement/GetStockPaperItem";
        public string url_request = "https://lmmcore.online/api/LGSRequest/GetRequest/";
        public string url_nis = "https://lmmcore.online/api/LGSNIS/GetById/";
        public string url_nis_cartable = "https://lmmcore.online/api/LGSNIS/GetNISApprovingCartable";
        public string url_paper_item = "https://lmmcore.online/api/LGSStockManagement/GetPaperItemById?id=";


        [Route("api/document/request/sync")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> SyncRequest()
        {
            string token = null;

            ppa_entities context = new ppa_entities();
            vira_document result = new vira_document();
            List<vira_document> request_result = new List<vira_document>();
            List<vira_document_item> items = new List<vira_document_item>();

            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_request = new vira_log() { date_create = DateTime.Now };
            vira_log log_item = new vira_log() { date_create = DateTime.Now };
            vira_log log_sync = new vira_log() { date_create = DateTime.Now };


            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;

                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_request);

                var response_request = await http_post_request(url_cartable, new { }, token, log_request);
                request_result = JsonConvert.DeserializeObject<List<vira_document>>(response_request.result);

                log_request.paper_id = request_result.First().id;
                log_request.paper_no = request_result.First().paper_no;
                log_request.paper_type = "request";

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_request.error_message = msg;
                log_request.paper_type = "request";
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });
            }

            try
            {
                context.vira_log.Add(log_item);

                var response_item = await http_post_request(url_cartable_item + request_result.First().id, null, token, log_item);
                items = JsonConvert.DeserializeObject<List<vira_document_item>>(response_item.result);
                log_item.paper_type = "request";

                foreach (var item in items)
                {
                    context.vira_log.Add(log_item);
                    log_request.paper_id = item.id;
                    log_request.paper_no = item.itemNo.ToString();
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_item.error_message = msg;
                log_item.paper_type = "request";
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });
            }

            try
            {
                context.vira_log.Add(log_sync);

                var document_request = await context.vira_document.Where(q => q.vira_type == "request").ToListAsync();
                var non_synced = request_result.Where(q => !document_request.Any(y => y.vira_id == q.id)).ToList();

                foreach (var request in non_synced)
                {
                    result = request;
                    result.vira_id = request.id;
                    result.vira_type = "request";
                    result.vira_document_item = items.Where(q => q.paperId == request.id).ToList();
                    context.vira_document.Add(result);

                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_sync.error_message = msg;
                await context.SaveAsync();
                return Ok(msg);

            }


            return Ok("Succeed");


        }

        //api/mnt/document/save
        [Route("api/vira/document/save/request")]
        [AcceptVerbs("Post")]
        public async Task<IHttpActionResult> vira_save_request(dynamic dto)
        {
            ppa_entities context = new ppa_entities();
            vira_document result = new vira_document();
            vira_document vira_request = new vira_document();
           
            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_request = new vira_log() { date_create = DateTime.Now };
            vira_log log_request_item = new vira_log() { date_create = DateTime.Now };

            string vira_no = dto.viraNo;
            string token;


            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;
                log_auth.error_message = null;
                log_auth.paper_id = null;

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_request);

                var response_request = await http_post_request(url_cartable, new { requestNo = vira_no }, token, log_request);
                List<vira_document> request_result = JsonConvert.DeserializeObject<List<vira_document>>(response_request.result);

                log_request.paper_type = "request";
                vira_request = request_result.First();

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;

                log_request.error_message = msg;
                log_request.paper_type = "request";
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }
            try
            {
                context.vira_log.Add(log_request_item);

                var response_item = await http_post_request(url_cartable_item + vira_request.id, null, token, log_request_item);
                vira_request.vira_document_item = JsonConvert.DeserializeObject<List<vira_document_item>>(response_item.result);

                log_request_item.paper_type = "request";

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_request_item.error_message = msg;
                log_request_item.paper_type = "request";

                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            result = vira_request;
            result.vira_id = vira_request.id;

            context.vira_document.Add(result);

            await context.SaveChangesAsync();

            return Ok(result.id);

        }

        //api/mnt/document/save
        [Route("api/vira/document/save/do")]
        [AcceptVerbs("Post")]
        public async Task<IHttpActionResult> vira_save_do(dynamic dto)
        {
            ppa_entities context = new ppa_entities();
            vira_document result = new vira_document();
            vira_document vira_do = new vira_document();

            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_stock = new vira_log() { date_create = DateTime.Now };
            vira_log log_stock_item = new vira_log() { date_create = DateTime.Now };

            string vira_no = dto.vira_no;
            string token;

            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;


            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_stock);

                var response_do = await http_post_request(url_stock, new { paper_no = vira_no }, token, log_stock);
                List<vira_document> do_result = JsonConvert.DeserializeObject<List<vira_document>>(response_do.result);

                vira_do = do_result.First();
                log_stock.paper_type = "do";
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_stock.error_message = msg;
                log_stock.paper_type = "do";
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_stock_item);

                var response_item = await http_post_request(url_stock_item, new { paper_id = vira_do.id }, token, log_stock_item);
                vira_do.vira_document_item = JsonConvert.DeserializeObject<List<vira_document_item>>(response_item.result);

                result = vira_do;
                result.vira_id = vira_do.id;
                result.vira_type = "do";

                log_stock_item.paper_type = "do";
                
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_stock_item.error_message = msg;
                log_stock_item.paper_type = "do";
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }
            context.vira_document.Add(result);

            await context.SaveChangesAsync();

            return Ok(result.id);

        }

        //api/mnt/document/save
        [Route("vira/document/save/nis")]
        [AcceptVerbs("Post")]
        public async Task<IHttpActionResult> vira_save_nis(dynamic dto)
        {
            ppa_entities context = new ppa_entities();
            vira_document result = new vira_document();
         
            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_nis = new vira_log() { date_create = DateTime.Now };

            string vira_no = dto.vira_no;
            string token;

            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;


            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }
            try
            {
                context.vira_log.Add(log_nis);
                var nis_response = await http_post_request(url_nis_cartable, new { userId = 19, nisNo = vira_no }, token, log_nis);
                List<vira_document> t = JsonConvert.DeserializeObject<List<vira_document>>(nis_response.result);
                result = t.First();
                result.vira_type = "nis";
                context.vira_document.Add(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_nis.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }
            
            context.vira_document.Add(result);
            
            await context.SaveChangesAsync();

            return Ok(result.id);

        }



        //[Route("api/mnt/document/save")]
        //[AcceptVerbs("Post")]
        //public async Task<IHttpActionResult> SaveDocument(dynamic dto)
        //{

        //    ppa_entities context = new ppa_entities();
        //    vira_document result = new vira_document();
        //    vira_document vira_request = new vira_document();
        //    vira_document vira_do = new vira_document();

        //    vira_log log_auth = new vira_log() { date_create = DateTime.Now };
        //    vira_log log_request = new vira_log() { date_create = DateTime.Now };
        //    vira_log log_request_item = new vira_log() { date_create = DateTime.Now };
        //    vira_log log_stock = new vira_log() { date_create = DateTime.Now };
        //    vira_log log_stock_item = new vira_log() { date_create = DateTime.Now };
        //    vira_log log_nis = new vira_log() { date_create = DateTime.Now };

        //    string viraNo = dto.viraNo;
        //    string type = dto.type;
        //    string token;


        //    if (type == "request")
        //    {


        //        try
        //        {

        //            context.vira_log.Add(log_auth);


        //            var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
        //            token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;
        //            log_auth.error_message = null;
        //            log_auth.paper_id = null;

        //        }
        //        catch (Exception ex)
        //        {
        //            var msg = ex.Message;
        //            if (ex.InnerException != null)
        //                msg += "   " + ex.InnerException.Message;
        //            log_auth.error_message = msg;
        //            await context.SaveAsync();
        //            return Ok(new
        //            {
        //                IsSuccess = false,
        //            });

        //        }

        //        try
        //        {
        //            context.vira_log.Add(log_request);

        //            var response_request = await http_post_request(url_cartable, new { requestNo = viraNo }, token, log_request);
        //            List<vira_document> request_result = JsonConvert.DeserializeObject<List<vira_document>>(response_request.result);

        //            log_request.paper_type = "request";
        //            vira_request = request_result.First();

        //        }
        //        catch (Exception ex)
        //        {
        //            var msg = ex.Message;
        //            if (ex.InnerException != null)
        //                msg += "   " + ex.InnerException.Message;

        //            log_request.error_message = msg;
        //            log_request.paper_type = "request";
        //            await context.SaveAsync();
        //            return Ok(new
        //            {
        //                IsSuccess = false,
        //            });

        //        }
        //        try
        //        {
        //            context.vira_log.Add(log_request_item);

        //            var response_item = await http_post_request(url_cartable_item + vira_request.id, null, token, log_request_item);
        //            vira_request.vira_document_item = JsonConvert.DeserializeObject<List<vira_document_item>>(response_item.result);

        //            log_request_item.paper_type = "request";

        //        }
        //        catch (Exception ex)
        //        {
        //            var msg = ex.Message;
        //            if (ex.InnerException != null)
        //                msg += "   " + ex.InnerException.Message;
        //            log_request_item.error_message = msg;
        //            log_request_item.paper_type = "request";

        //            await context.SaveAsync();
        //            return Ok(new
        //            {
        //                IsSuccess = false,
        //            });

        //        }

        //        result = vira_request;
        //        result.vira_id = vira_request.id;

        //        context.vira_document.Add(result);


        //    }

        //    if (type == "do")
        //    {
        //        try
        //        {

        //            context.vira_log.Add(log_auth);


        //            var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
        //            token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;


        //        }
        //        catch (Exception ex)
        //        {
        //            var msg = ex.Message;
        //            if (ex.InnerException != null)
        //                msg += "   " + ex.InnerException.Message;
        //            log_auth.error_message = msg;
        //            await context.SaveAsync();
        //            return Ok(new
        //            {
        //                IsSuccess = false,
        //            });

        //        }

        //        try
        //        {
        //            context.vira_log.Add(log_stock);

        //            var response_do = await http_post_request(url_stock, new { paper_no = viraNo }, token, log_stock);
        //            List<vira_document> do_result = JsonConvert.DeserializeObject<List<vira_document>>(response_do.result);

        //            vira_do = do_result.First();
        //            log_stock.paper_type = "do";
        //        }
        //        catch (Exception ex)
        //        {
        //            var msg = ex.Message;
        //            if (ex.InnerException != null)
        //                msg += "   " + ex.InnerException.Message;
        //            log_stock.error_message = msg;
        //            log_stock.paper_type = "do";
        //            await context.SaveAsync();
        //            return Ok(new
        //            {
        //                IsSuccess = false,
        //            });

        //        }

        //        try
        //        {
        //            context.vira_log.Add(log_stock_item);

        //            var response_item = await http_post_request(url_stock_item, new { paper_id = vira_do.id }, token, log_stock_item);
        //            vira_do.vira_document_item = JsonConvert.DeserializeObject<List<vira_document_item>>(response_item.result);

        //            result = vira_do;
        //            result.vira_id = vira_do.id;

        //            log_stock_item.paper_type = "do";
        //            //log item

        //        }
        //        catch (Exception ex)
        //        {
        //            var msg = ex.Message;
        //            if (ex.InnerException != null)
        //                msg += "   " + ex.InnerException.Message;
        //            log_stock_item.error_message = msg;
        //            log_stock_item.paper_type = "do";
        //            await context.SaveAsync();
        //            return Ok(new
        //            {
        //                IsSuccess = false,
        //            });

        //        }
        //        context.vira_document.Add(result);
        //    }

        //    if (type == "nis")
        //    {
        //        try
        //        {

        //            context.vira_log.Add(log_auth);


        //            var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
        //            token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;


        //        }
        //        catch (Exception ex)
        //        {
        //            var msg = ex.Message;
        //            if (ex.InnerException != null)
        //                msg += "   " + ex.InnerException.Message;
        //            log_auth.error_message = msg;
        //            await context.SaveAsync();
        //            return Ok(new
        //            {
        //                IsSuccess = false,
        //            });

        //        }
        //        try
        //        {
        //            context.vira_log.Add(log_nis);
        //            var nis_response = await http_post_request(url_nis_cartable, new { userId  = 8, nisNo = dto.viraNo }, token, log_nis);
        //            List<vira_document> t = JsonConvert.DeserializeObject<List<vira_document>>(nis_response.result);
        //            result = t.First();
        //            result.vira_type = "nis";
        //            context.vira_document.Add(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            var msg = ex.Message;
        //            if (ex.InnerException != null)
        //                msg += "   " + ex.InnerException.Message;
        //            log_nis.error_message = msg;
        //            await context.SaveAsync();
        //            return Ok(new
        //            {
        //                IsSuccess = false,
        //            });

        //        }
        //        context.vira_document.Add(result);
        //    }



        //    await context.SaveChangesAsync();

        //    return Ok("Succeed");


        //}

        [Route("api/mnt/document/approve/{id}")]
        [AcceptVerbs("get")]
        public async Task<IHttpActionResult> ApproveDocument(int id)
        {
            ppa_entities context = new ppa_entities();

            vira_log log_approve = new vira_log() { date_create = DateTime.Now };
            context.vira_log.Add(log_approve);

            try
            {
                log_approve.paper_id = id;


                var document = context.vira_document.FirstOrDefault(q => q.vira_id == id);
                document.status = 1;
                document.date_status = DateTime.Now;

                await context.SaveChangesAsync();

                return Ok(document.id);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;

                log_approve.paper_id = id;
                log_approve.error_message = msg;

                context.vira_log.Add(log_approve);
                await context.SaveAsync();
                return Ok(msg);

            }
        }

        [Route("api/mnt/document/delete/{id}")]
        [AcceptVerbs("get")]
        public async Task<IHttpActionResult> DeleteDocument(int id)
        {
            ppa_entities context = new ppa_entities();

            vira_log log_delete = new vira_log() { date_create = DateTime.Now };
            context.vira_log.Add(log_delete);

            try
            {
                log_delete.paper_id = id;

                var document = context.vira_document.FirstOrDefault(q => q.id == id);

                context.vira_document.Remove(document);
                await context.SaveChangesAsync();

                return Ok(document);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;

                log_delete.error_message = msg;

                await context.SaveAsync();


                return Ok(msg);

            }
        }

        [Route("api/mnt/stockpaper")]
        [AcceptVerbs("post")]
        public async Task<IHttpActionResult> MNTStockPaper(dynamic dto)
        {
            ppa_entities context = new ppa_entities();

            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_paper = new vira_log() { date_create = DateTime.Now };
            vira_log log_paper_item = new vira_log() { date_create = DateTime.Now };

            StockPaper vira_stock = new StockPaper();
            string token;

            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;

                log_auth.error_message = null;
                log_auth.paper_id = null;

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }
            try
            {
                context.vira_log.Add(log_paper);

                var stock_paper = await http_post_request(url_stock, dto, token, log_paper);
                List<StockPaper> stock_response = JsonConvert.DeserializeObject<List<StockPaper>>(stock_paper.result);

                vira_stock = stock_response.First();

                log_paper.error_message = null;
                log_paper.paper_id = vira_stock.id;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_paper.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_paper_item);
                var stock_item = await http_post_request(url_stock_item, new { paper_id = vira_stock.id }, token, log_paper_item);
                vira_stock.Items = JsonConvert.DeserializeObject<List<StockItems>>(stock_item.result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_paper_item.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            return Ok(vira_stock);

        }


        [Route("api/mnt/stockitem/{id}")]
        [AcceptVerbs("get")]
        public async Task<IHttpActionResult> StockPaperItems(int id)
        {
            ppa_entities context = new ppa_entities();
            List<StockItems> result = new List<StockItems>();

            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_stock_item = new vira_log() { date_create = DateTime.Now };
            string token;

            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;

                log_auth.error_message = null;
                log_auth.paper_id = null;

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_stock_item);

                var items = await http_post_request(url_stock_item, new { paper_id = id }, token, log_stock_item);
                result = JsonConvert.DeserializeObject<List<StockItems>>(items.result);

                foreach (var item in result)
                {
                    log_stock_item.paper_item_id = item.Id;
                    log_stock_item.paper_no = item.FullNo;
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_stock_item.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }


            return Ok(result);

        }

        [Route("api/mnt/document/paperitem/{id}")]
        [AcceptVerbs("get")]
        public async Task<IHttpActionResult> PaperItemsById(int id)
        {
            ppa_entities context = new ppa_entities();

            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_paper = new vira_log() { date_create = DateTime.Now };

            PaperItem result = new PaperItem();
            string token;

            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;

                log_auth.error_message = null;
                log_auth.paper_id = null;

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                var paper_item = await http_post_request(url_paper_item + id, null, token, log_paper);
                result = JsonConvert.DeserializeObject<PaperItem>(paper_item.result);

                log_paper.error_message = null;
                log_paper.paper_id = result.Id;

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_paper.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }
            return Ok(result);
        }

        [Route("api/vira/get/nis/{id}")]
        [AcceptVerbs("get")]
        public async Task<IHttpActionResult> vira_nis(int id)
        {
            ppa_entities context = new ppa_entities();

            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_nis = new vira_log() { date_create = DateTime.Now };

            NISResponse result = new NISResponse();
            string token;

            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;

                log_auth.error_message = null;
                log_auth.paper_id = null;

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_nis);
                var nis_response = await http_get_request(url_nis + id, token, log_nis);
                result = JsonConvert.DeserializeObject<NISResponse>(nis_response.result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_nis.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }
            return Ok(result);

        }

        [Route("api/vira/get/nis/list")]
        [AcceptVerbs("post")]
        public async Task<IHttpActionResult> vira_get_nis(dynamic dto)
        {
            ppa_entities context = new ppa_entities();

            // Start with the base query
            var query = context.vira_document.AsQueryable();

            // Filter only "nis" type documents
            query = query.Where(q => q.vira_type == "nis");

            // Dynamically apply filters based on the properties in the dto
            if (dto != null)
            {
                var parameter = Expression.Parameter(typeof(vira_document), "q");
                Expression predicate = Expression.Constant(true);

                foreach (var prop in dto.Properties())
                {
                    var propName = prop.Name;
                    var propValue = prop.Value.ToString();

                    var member = Expression.Property(parameter, propName);
                    var constant = Expression.Constant(propValue);
                    var equality = Expression.Equal(member, constant);
                    predicate = Expression.AndAlso(predicate, equality);
                }

                var lambda = Expression.Lambda<Func<vira_document, bool>>(predicate, parameter);
                query = query.Where(lambda);
            }

            var nis = await query.ToListAsync();

            return Ok(nis);
        }

        [Route("api/vira/get/nis")]
        [AcceptVerbs("get")]
        public async Task<IHttpActionResult> vira_nis_cartable()
        {
            ppa_entities context = new ppa_entities();

            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_nis = new vira_log() { date_create = DateTime.Now };

            List<nis_cartable> result = new List<nis_cartable>();
            string token;

            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;

                log_auth.error_message = null;
                log_auth.paper_id = null;

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_nis);
                var nis_response = await http_post_request(url_nis_cartable, new { userId = 8 }, token, log_nis);
                result = JsonConvert.DeserializeObject<List<nis_cartable>>(nis_response.result);                
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_nis.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }
            return Ok(result);

        }




        [Route("api/mnt/document/request/{id}")]
        [AcceptVerbs("get")]
        public async Task<IHttpActionResult> mnt_request(int id)
        {
            ppa_entities context = new ppa_entities();
            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_request = new vira_log() { date_create = DateTime.Now };

            string token;
            Request result = new Request();

            try
            {

                context.vira_log.Add(log_auth);


                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;

                log_auth.error_message = null;
                log_auth.paper_id = null;

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_request);

                var request_response = await http_get_request(url_request + id, token, log_request);
                result = JsonConvert.DeserializeObject<Request>(request_response.result);


                log_request.error_message = null;
                log_request.paper_id = result.Id;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_request.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            return Ok(result);

        }

        [Route("api/mnt/document/result")]
        [AcceptVerbs("post")]
        public async Task<IHttpActionResult> vira_result(dynamic dto)
        {
            string viraNo = dto.viraNo;
            string token;
            ppa_entities context = new ppa_entities();
            List<StockItems> paper_items = new List<StockItems>();

            var doc_item = await context.vira_document_item.FirstOrDefaultAsync(q => q.fullNo == viraNo);

            vira_log log_auth = new vira_log() { date_create = DateTime.Now };
            vira_log log_stock = new vira_log() { date_create = DateTime.Now };
            vira_log log_item = new vira_log() { date_create = DateTime.Now };


            try
            {

                context.vira_log.Add(log_auth);

                var auth_response = await http_post_request(url_auth, new { username = "test", password = "1234" }, null, log_auth);
                token = JsonConvert.DeserializeObject<auth>(auth_response.result).token;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_auth.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }

            try
            {
                context.vira_log.Add(log_stock);

                var items = await http_post_request(url_stock_item, new { paper_id = dto.paper_id }, token, log_stock);
                paper_items = JsonConvert.DeserializeObject<List<StockItems>>(items.result);

                foreach (var item in paper_items)
                {
                    log_stock.paper_item_id = item.Id;
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                log_stock.error_message = msg;
                await context.SaveAsync();
                return Ok(new
                {
                    IsSuccess = false,
                });

            }


            foreach (var item in paper_items)
            {
                var item_result = new vira_document_item_result();
                context.vira_document_item_result.Add(item_result);

                item_result.document_item_id = doc_item.id;
                item_result.vira_paper_id = item.PaperId;
                item_result.vira_paper_item_id = item.Id;
                item_result.qty = item.Quantity;
                item_result.vira_paper_type = dto.type;
                item_result.vira_paper_no = dto.viraNo;

            }

            await context.SaveChangesAsync();

            return Ok();
        }

        public class http_result
        {
            public bool is_ok { get; set; }
            public string result { get; set; }
        }

        public async Task<http_result> http_post_request(string url, dynamic body, string token, vira_log log)
        {
            http_result result = new http_result() { is_ok = false };
            log.request_url = url;

            try
            {


                using (HttpClient client = new HttpClient())
                {
                    if (token != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    string jsonData = JsonConvert.SerializeObject(body);
                    log.request_payload = jsonData;
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        //  log.response=responseData;  
                        result.is_ok = true;
                        result.result = JsonConvert.DeserializeObject<response>(responseData).data.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "  inner: " + ex.InnerException.Message;
                log.error_message = msg;
                result.is_ok = false;
                return result;
            }



            return result;
        }

        public async Task<http_result> http_get_request(string url, string token, vira_log log)
        {
            http_result result = new http_result() { is_ok = false };
            log.request_url = url;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (token != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        result.is_ok = true;
                        result.result = JsonConvert.DeserializeObject<response>(responseData).data.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "  inner: " + ex.InnerException.Message;
                log.error_message = msg;
                result.is_ok = false;
                return result;
            }
            return result;
        }


    }
}