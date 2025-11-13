using AirpocketTRN.Models;
using AirpocketTRN.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;

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

        string get_part(string str)
        {
            switch (str)
            {
                case "":

                default:
                    return string.Empty;
            }
        }

        [Route("api/files/get/{nid}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_person_folder(string nid)
        {
            var context = new FLYEntities();
            var result=context.view_person_folder.Where(q=>q.nid== nid).ToList();
            return Ok(result);
        }
        [Route("api/files/test")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_person_test( )
        {
            using (var document = PdfDocument.Load(@"C:\Training_Folder\1.pdf"))
            {
                // Render the first page (0-based index)
                using (var image = document.Render(0, 300, 300, true)) // DPI = 300 for good quality
                {
                    // Calculate thumbnail height based on aspect ratio
                    int thumbnailHeight = (int)((double)250 / image.Width * image.Height);

                    using (var thumbnail = new Bitmap(250, 300))
                    using (var graphics = Graphics.FromImage(thumbnail))
                    {
                        graphics.DrawImage(image, 0, 0, 250, thumbnailHeight);
                        thumbnail.Save(@"C:\Training_Folder\1.jpg", ImageFormat.Jpeg); // or .Png
                    }
                }
            }
            return Ok(true);
        }

        [Route("api/files/{nid}/{root}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_files(string nid, string root)
        {
            string startFolder = /*@"C:\Training_Folder"*/ @"C:\Inetpub\vhosts\airpocket.app\ava.airpocket.app\upload\training_folder" + @"\" + nid;
            if (root != "-1")
                startFolder += @"\" + root;

            //var RootDirectory = new DirectoryInfo(startFolder);
            //var listDir = RootDirectory.GetDirectories("*", SearchOption.AllDirectories)
            //    .Where(dir => !dir.GetDirectories().Any())
            //    .ToList();

            DirectoryInfo dir = new DirectoryInfo(startFolder);
            var fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);

            var fileQuery = (from file in fileList

                             orderby file.Name
                             select new
                             {
                                 file.Extension,

                                 file.FullName,
                                 file.Name,
                                 file.CreationTime,
                                 file.Attributes,
                                 Directory = file.Directory.FullName,
                                 grp = file.Directory.FullName.Split('\\')[8],
                                 // file.DirectoryName,
                                 // file_name=file.FullName
                             }).ToList();


            var context = new FLYEntities();
            var profile=context.ViewProfiles.Where(q=>q.NID==nid).FirstOrDefault();
            foreach (var x in fileQuery)
            {
                context.person_folder.Add(new person_folder()
                {
                    creation_time = x.CreationTime,
                    directory = x.Directory,
                    employee_id = profile!=null?(Nullable<int>)profile.Id:null,
                    person_id = profile != null ? (Nullable<int>)profile.PersonId:null,
                    file_extension = x.Extension.Replace(".", ""),
                    file_full_name = x.FullName,
                    file_name = x.Name,
                    folder = x.grp.ToUpper(),
                    nid = nid,
                    last_name = profile != null ? profile.FirstName:null,
                    first_name = profile != null ? profile.LastName:null,

                });
            }
            context.SaveChanges();
            return Ok(fileQuery);
        }

        [Route("api/trn/get/crm/assessment/{flightId}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_crm_assessment(int flightId)
        {
            var result = await trainingService.get_trn_crm_assessment(flightId);
            return Ok(result);
        }

       [Route("api/trn/get/person/files/{nid}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_person_files(string nid)
        {
            var result = await trainingService.get_person_files(nid);
            return Ok(result);
        }

       [Route("api/trn/get/crew/files/{nid}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_crew_files(string nid)
        {
            var result = await trainingService.get_crew_files(nid);
            return Ok(result);
        }

        [Route("api/trn/get/cabin/files/{nid}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_cabin_files(string nid)
        {
            var result = await trainingService.get_cabin_files(nid);
            return Ok(result);
        }


        [Route("api/trn/get/dispatch/files/{nid}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_dispatch_files(string nid)
        {
            var result = await trainingService.get_dispatch_files(nid);
            return Ok(result);
        }


        [Route("api/trn/get/course/ext/{pid}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_course_ext(int pid)
        {
            var result = await trainingService.get_course_ext(pid);
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