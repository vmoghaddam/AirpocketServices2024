using Newtonsoft.Json;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using static ApiXLS.Controllers.SafetyFormsController;
using Spire.License;
using ApiXLS.Models;

namespace ApiXLS.Controllers
{
    public class neerjaController : ApiController
    {
        [Route("api/scc/report/{crew_id}/{flight_id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetSccReport(int crew_id, int flight_id)
        {
            // License key setup for Spire.XLS
            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";
            var context = new Models.dbEntities();

            var scc_data = context.view_neerja_scc_report.FirstOrDefault(q => q.crew_id == crew_id && q.flight_id == flight_id);
            var flight_data = context.AppLegs.FirstOrDefault(q => q.FlightId == flight_id);
            var crews = context.ViewFlightCrewNewXes.Where(q => q.FlightId == flight_id).OrderBy(q => q.IsPositioning).ThenBy(q => q.GroupOrder).ToList();
            var cabin = crews.Where(q => q.JobGroup.Contains("CCM")).ToList();
            var cockpit = crews.Where(q => q.JobGroup.Contains("TRE") || q.JobGroup.Contains("TRI") || q.JobGroup.Contains("IP") || q.JobGroup.Contains("P1") || q.JobGroup.Contains("P2")).ToList();

            LicenseProvider.SetLicenseKey(LData);
            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "7_sccm_report" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];



            if (scc_data != null)
            {
                sheet.Range["H1"].Text = flight_data.STDDay?.ToString("yyyy-MM-dd") ?? string.Empty;
                sheet.Range["H2"].Text = flight_data.FlightNumber;
                sheet.Range["H3"].Text = flight_data.FromAirportIATA + '-' + flight_data.ToAirportIATA;
                sheet.Range["H4"].Text = flight_data.Register;

                sheet.Range["A7"].Text = scc_data.catering_issue != null ? scc_data.catering_issue : " ";
                sheet.Range["A9"].Text = scc_data.airport_service != null ?scc_data.airport_service : " ";
                sheet.Range["A11"].Text = scc_data.technical != null ?  scc_data.technical : " ";
                sheet.Range["A13"].Text = scc_data.safety_issue != null ? scc_data.safety_issue : " ";
                sheet.Range["A15"].Text = scc_data.general_issue != null ?scc_data.general_issue : " ";
                sheet.Range["F27"].Text = crews.Single(q => q.CrewId == crew_id).Name;

                for (int i = 0; i <= 7; i++)
                {

                    sheet.Range["A" + (i + 18).ToString()].Text = cockpit.ElementAtOrDefault(i) != null ? cockpit[i].JobGroup : " ";
                    sheet.Range["B" + (i + 18).ToString()].Text = cockpit.ElementAtOrDefault(i) != null ? cockpit[i].Name : " ";

                    sheet.Range["E" + (i + 18).ToString()].Text = cabin.ElementAtOrDefault(i) != null ? cabin[i].JobGroup : " ";
                    sheet.Range["F" + (i + 18).ToString()].Text = cabin.ElementAtOrDefault(i) != null ? cabin[i].Name : " ";
                }
            }
            else
            {
                sheet.Range["H1"].Text = flight_data.STDDay?.ToString("yyyy-MM-dd") ?? string.Empty;
                sheet.Range["H2"].Text = flight_data.FlightNumber;
                sheet.Range["H3"].Text = flight_data.FromAirportIATA + '-' + flight_data.ToAirportIATA;
                sheet.Range["H4"].Text = flight_data.Register;

                sheet.Range["A7"].Text = " ";
                sheet.Range["A9"].Text = " ";
                sheet.Range["A11"].Text = " ";
                sheet.Range["A13"].Text = " ";
                sheet.Range["A15"].Text = " ";

            }

            var name = "scc-report-" + flight_data.FlightNumber + "-" + flight_data.STDDay?.ToString("yyyy-MM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");

            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return response;

        }
    }
}