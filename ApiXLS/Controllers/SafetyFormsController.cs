using Newtonsoft.Json;
using Spire.Xls;
using Spire.License;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using System.Net;
using System.Web.Http.Cors;

namespace ApiXLS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SafetyFormsController : ApiController
    {
        public class FlightEvent
        {
            public int Id { get; set; }
            public int FlightId { get; set; }
            public DateTime DateOccurrence { get; set; }
            public DateTime DateSign { get; set; }
            public string AircraftType { get; set; }
            public string Register { get; set; }
            public int? ComponentSpecificationId { get; set; }
            public string ComponentSpecification { get; set; }
            public string Route { get; set; }
            public string FlightNumber { get; set; }
            public DateTime FlightDate { get; set; }
            public string ATLNo { get; set; }
            public string TaskNo { get; set; }
            public string Reference { get; set; }
            public int StationId { get; set; }
            public string Station { get; set; }
            public string EventDescription { get; set; }
            public string ActionTakenDescription { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string CAALicenceNo { get; set; }
            public string AuthorizationNo { get; set; }
            public string SerialNumber { get; set; }
            public string PartNumber { get; set; }
            public DateTime STD { get; set; }
            public string Status { get; set; }
            public DateTime? DateStatus { get; set; }
            public int? StatusEmployeeId { get; set; }
            public string StatusEmployeeName { get; set; }
            public string Result { get; set; }
            public string DelayReason { get; set; }
            public TimeSpan? Delay { get; set; }
            public string FormNo { get; set; }
        }

        public class res_type
        {
            public bool IsSuccess { get; set; }
            public FlightEvent Data { get; set; }
        }




        [Route("api/mor/save/xls/{id}")]
        [AcceptVerbs("Get")]

        public async Task<HttpResponseMessage> qa_excel(int id)
        {
            // License key setup for Spire.XLS
            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            LicenseProvider.SetLicenseKey(LData);
            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "safety_form_mor_v1" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            var sh = 0;
            Worksheet sheet = workbook.Worksheets[sh];

            FlightEvent flightEvent = new FlightEvent();


            using (HttpClient client = new HttpClient())
            {
                var url = "https://apiqatemp.apvaresh.com/api/get/mor/byid/" + id;
                var result = await client.GetStringAsync(url);
                res_type http_res = JsonConvert.DeserializeObject<res_type>(result);
                flightEvent = http_res.Data; // Assuming `response.Data` contains the FlightEvent object
            }


            sheet.Range["B3"].Text = "FORM ID: VRH-MOR-" + flightEvent.Id;
            sheet.Range["B4"].DateTimeValue = flightEvent.DateOccurrence;
            sheet.Range["B5"].Text = flightEvent.Station ?? " ";
            sheet.Range["B6"].Text = flightEvent.AircraftType ?? " ";
            sheet.Range["B7"].Text = flightEvent.Route ?? " ";
            sheet.Range["B8"].Text = flightEvent.TaskNo ?? " ";
            sheet.Range["B10"].Text = flightEvent.ComponentSpecification ?? " ";
            sheet.Range["B11"].Text = flightEvent.PartNumber ?? " ";
            sheet.Range["B17"].Text = flightEvent.EmployeeName ?? " ";
            sheet.Range["B18"].Text = flightEvent.AuthorizationNo ?? " ";
            sheet.Range["D4"].DateTimeValue = flightEvent.DateOccurrence.ToLocalTime();
            sheet.Range["D5"].Text = flightEvent.Register ?? " ";
            sheet.Range["D6"].Text = flightEvent.FlightNumber ?? " ";
            sheet.Range["D7"].Text = flightEvent.ATLNo ?? " ";
            sheet.Range["D8"].Text = flightEvent.Reference ?? " ";
            sheet.Range["D10"].Text = flightEvent.SerialNumber ?? " ";
            sheet.Range["D17"].Text = flightEvent.CAALicenceNo ?? " ";
            sheet.Range["D18"].DateTimeValue = flightEvent.DateSign;
            sheet.Range["A13"].Text = flightEvent.EventDescription ?? " ";
            sheet.Range["A15"].Text = flightEvent.ActionTakenDescription ?? " ";

            var name = "safety-mor-" + flightEvent.STD.ToString("yyyy-MMM-dd" + "-" + flightEvent.Id);
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");

            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return response;

        }

        [Route("api/get/sn/{id}")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> get_safety_notice(int id)
        {

            try
            {
                var context = new Models.dbEntities();
                var result = context.ViewBookApplicableEmployees.Where(q => q.Category == "Safety Notice" && q.IsExposed == 1 && q.EmployeeId == id).OrderByDescending(q => q.No).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return BadRequest(msg);
            }
        }
    }
}