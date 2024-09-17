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
using ApiXLS.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using Spire.Xls;
using System.Globalization;

namespace ApiXLS.Controllers
{
    public class DispatchController : ApiController
    {
        string toHHMM(int? mm)
        {
            if (mm == null || mm <= 0)
                return "00:00";
            // TimeSpan ts = TimeSpan.FromMinutes(Convert.ToDouble(mm));
            // var result = ts.Hours.ToString().PadLeft(2, '0') + ":" + ts.Minutes.ToString().PadLeft(2, '0');
            var hh = mm / 60;
            var min = mm % 60;
            var result = hh.ToString().PadLeft(2, '0') + ":" + min.ToString().PadLeft(2, '0');
            return result;

        }

        [Route("api/xls/crew/flights/{grp}")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXlsCrewFlights(string grp,DateTime df,DateTime dt)
        {
            var cids = new List<int>() {
            4325,4460,4470,4467,4468,4477,4469,4479,4289,4287,4616,4615,4220,4478
            };
            var ctx = new Models.dbEntities();
            var _df = df.Date;
            var _dt = dt.Date.AddDays(1);
            var _query = (from x in ctx.ViewLegCrews
                         where 
                           //x.CrewId == id && 
                          // x.JobGroup==grp &&  
                           x.STDLocal >= df && x.STDLocal < dt
                         //flypersia
                         //&& x.FlightStatusID != 1 
                         && x.FlightStatusID != 4
                        // && cids.Contains(x.CrewId)
                         //orderby x.STDLocal
                         select x);
            if (grp != "-1")
                _query = _query.Where(q => q.JobGroup == grp);

            var query = _query.ToList();


            var grouped = (from x in query
                           group x by new { x.CrewId, x.ScheduleName, x.JobGroup, x.JobGroupCode, x.Name,x.Position,x.PositionId,x.PID } into _grp
                           select new
                           {
                               _grp.Key.CrewId,
                               _grp.Key.ScheduleName,
                               _grp.Key.JobGroup,
                               _grp.Key.PID,
                               Sheet= _grp.Key.JobGroup=="TRE" || _grp.Key.JobGroup=="TRI"?"IP": _grp.Key.JobGroup,
                               _grp.Key.JobGroupCode,
                               _grp.Key.Name,
                               _grp.Key.Position,
                               _grp.Key.PositionId,
                               FlightTime = _grp.Sum(q => q.FlightTime),
                               BlockTime=_grp.Sum(q=>q.BlockTime),
                               JLFlightTime=_grp.Sum(q=>q.JL_FlightTime),
                               JLBlockTime=_grp.Sum(q=>q.JL_BlockTime),
                               FixTime=_grp.Sum(q=>q.FixTime),
                               Flights = _grp.OrderBy(q => q.STD).ToList(),
                           }
                       )
                       
                       .ToList().OrderByDescending(q => q.FixTime).ThenBy(q=>q.ScheduleName).ToList();




            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);
            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "CrewFlights" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);

            List<string> grps = new List<string>() {"IP","P1","P2","ISCCM","SCCM","CCM","F/M" };
            var sh = 0;
            foreach(var sht in grps)
            {
                var _grouped = grouped.Where(q => q.Sheet == sht).ToList();
                //////////////////////////////////////
                Worksheet sheet = workbook.Worksheets[sh];
                sheet.Range[1, 20].Text = (df).Date.ToString("yyyy-MM-dd");
                sheet.Range[2, 20].Text = (dt).Date.ToString("yyyy-MM-dd");
                var ln = 5;
                foreach (var crew in _grouped)
                {


                    sheet.Range[ln, 2].Text = crew.JobGroup;
                    sheet.Range[ln, 3].Text = crew.JobGroupCode;
                    sheet.Range[ln, 4].Text = crew.PID;



                    sheet.Range[ln, 5].Text = crew.ScheduleName;
                    sheet.Range[ln, 6].Text = crew.Position;
                    //f/l
                    sheet.Range[ln, 7].Text = toHHMM(crew.FlightTime).ToString();
                    //b/l
                    sheet.Range[ln, 8].Text = toHHMM(crew.BlockTime).ToString();

                    //f/l jl
                    sheet.Range[ln, 9].Text = toHHMM(crew.JLFlightTime).ToString();
                    //b/l jl
                    sheet.Range[ln, 10].Text = toHHMM(crew.JLBlockTime).ToString();

                    //fix
                    sheet.Range[ln, 11].Text = toHHMM(crew.FixTime).ToString();

                    sheet.Range[ln, 7].Style.Font.IsBold = true;
                    sheet.Range[ln, 8].Style.Font.IsBold = true;
                    sheet.Range[ln, 9].Style.Font.IsBold = true;
                    sheet.Range[ln, 10].Style.Font.IsBold = true;






                    sheet.Range[ln, 2, ln, 22].BorderInside(LineStyleType.Thin);
                    sheet.Range[ln, 2, ln, 22].BorderAround(LineStyleType.Thin);
                    //sheet.Rows[ln - 1].Style.Color = Color.FromArgb(230, 243, 255);
                    sheet.Range[ln, 2, ln, 22].Style.Color = Color.FromArgb(179, 230, 255);
                    sheet.Range[ln, 7, ln, 8].Style.Color = Color.FromArgb(217, 217, 217);
                    sheet.Range[ln, 9, ln, 10].Style.Color = Color.FromArgb(179, 255, 179);
                    //sheet.Range[ln, 10].Style.Color = Color.FromArgb(255, 179, 102);
                    var row = ln + 1;
                    foreach (var flt in crew.Flights)
                    {
                        sheet.Range[row, 2].Text = crew.JobGroup;
                        sheet.Range[row, 3].Text = crew.JobGroupCode;
                        sheet.Range[row, 4].Text = crew.PID;


                        sheet.Range[row, 5].Text = crew.ScheduleName;

                        sheet.Range[row, 6].Text = crew.Position;

                        // sheet.Range[row,2].Style.Color= Color.FromArgb(230, 247, 255);
                        // sheet.Range[row, 3].Style.Color = Color.FromArgb(230, 247, 255);
                        // sheet.Range[row, 4].Style.Color = Color.FromArgb(230, 247, 255);
                        //sheet.Range[row, 5].Style.Color = Color.FromArgb(230, 247, 255);


                        //f/l
                        sheet.Range[row, 7].Text = toHHMM(flt.FlightTime).ToString();
                        //b/l
                        sheet.Range[row, 8].Text = toHHMM(flt.BlockTime).ToString();
                        //f/l jl
                        sheet.Range[row, 9].Text = toHHMM(flt.JL_FlightTime).ToString();
                        //b/l jl
                        sheet.Range[row, 10].Text = toHHMM(flt.JL_BlockTime).ToString();
                        //fix
                        sheet.Range[row, 11].Text = toHHMM(flt.FixTime).ToString();

                        sheet.Range[row, 7, row, 8].Style.Color = Color.FromArgb(242, 242, 242);
                        sheet.Range[row, 9, row, 10].Style.Color = Color.FromArgb(230, 255, 230);
                        //sheet.Range[row, 10,row,10].Style.Color = Color.FromArgb(255, 179, 102);

                        sheet.Range[row, 12].Text = ((DateTime)flt.STDLocal).Date.ToString("yyyy-MM-dd");
                        sheet.Range[row, 13].Text = flt.Register;

                        sheet.Range[row, 14].Text = flt.FlightNumber;
                        sheet.Range[row, 15].Text = flt.FromAirportIATA;
                        sheet.Range[row, 16].Text = flt.ToAirportIATA;

                        sheet.Range[row, 17].Text = ((DateTime)flt.STDLocal).ToString("HH:mm");
                        sheet.Range[row, 18].Text = ((DateTime)flt.STALocal).ToString("HH:mm");


                        sheet.Range[row, 19].Text = flt.JL_OffBlockLocal != null ? ((DateTime)flt.JL_OffBlockLocal).ToString("HH:mm") : "";
                        sheet.Range[row, 20].Text = flt.JL_TakeOffLocal != null ? ((DateTime)flt.JL_TakeOffLocal).ToString("HH:mm") : "";
                        sheet.Range[row, 21].Text = flt.JL_LandingLocal != null ? ((DateTime)flt.JL_LandingLocal).ToString("HH:mm") : "";
                        sheet.Range[row, 22].Text = flt.JL_OnBlockLocal != null ? ((DateTime)flt.JL_OnBlockLocal).ToString("HH:mm") : "";

                        // sheet.Range[row, 16].Text = ((DateTime)flt.ch).Date.ToString("HH:mm");

                        sheet.Rows[row - 1].BorderInside(LineStyleType.Thin);
                        sheet.Rows[row - 1].BorderAround(LineStyleType.Thin);

                        row++;
                    }
                    sheet.GroupByRows(ln + 1, row - 1, true);

                    ln = ln + crew.Flights.Count + 1;
                }

                ////////////////////////////////////////
                sh++;
            }

           




            var name = "FlightTime-" + ((DateTime)df).ToString("yyyy-MMM-dd")+"-"+((DateTime)dt).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;


        }


        [Route("api/xls/pgs/crew/flights/{grp}")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXlsCrewFlightsPGS(string grp, DateTime df, DateTime dt)
        {
            var cids = new List<int>() {
            4325,4460,4470,4467,4468,4477,4469,4479,4289,4287,4616,4615,4220,4478
            };
            var ctx = new Models.dbEntities();
            var _df = df.Date;
            var _dt = dt.Date.AddDays(1);
            var _query = (from x in ctx.ViewLegCrews
                          where
                            //x.CrewId == id && 
                            // x.JobGroup==grp &&  
                            x.STDLocal >= df && x.STDLocal < dt
                          //flypersia
                          //&& x.FlightStatusID != 1 
                          && x.FlightStatusID != 4
                          && cids.Contains(x.CrewId)
                          //orderby x.STDLocal
                          select x);
            if (grp != "-1")
                _query = _query.Where(q => q.JobGroup == grp);

            var query = _query.ToList();


            var grouped = (from x in query
                           group x by new { x.CrewId, x.ScheduleName, x.JobGroup, x.JobGroupCode, x.Name, x.Position, x.PositionId, x.PID } into _grp
                           select new
                           {
                               _grp.Key.CrewId,
                               _grp.Key.ScheduleName,
                               _grp.Key.JobGroup,
                               _grp.Key.PID,
                               Sheet = _grp.Key.JobGroup == "TRE" || _grp.Key.JobGroup == "TRI" ? "IP" : _grp.Key.JobGroup,
                               _grp.Key.JobGroupCode,
                               _grp.Key.Name,
                               _grp.Key.Position,
                               _grp.Key.PositionId,
                               FlightTime = _grp.Sum(q => q.FlightTime),
                               BlockTime = _grp.Sum(q => q.BlockTime),
                               JLFlightTime = _grp.Sum(q => q.JL_FlightTime),
                               JLBlockTime = _grp.Sum(q => q.JL_BlockTime),
                               FixTime = _grp.Sum(q => q.FixTime),
                               Flights = _grp.OrderBy(q => q.STD).ToList(),
                           }
                       )

                       .ToList().OrderByDescending(q => q.FixTime).ThenBy(q => q.ScheduleName).ToList();




            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);
            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "CrewFlights" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);

            List<string> grps = new List<string>() { "IP", "P1", "P2", "ISCCM", "SCCM", "CCM", "F/M" };
            var sh = 0;
            foreach (var sht in grps)
            {
                var _grouped = grouped.Where(q => q.Sheet == sht).ToList();
                //////////////////////////////////////
                Worksheet sheet = workbook.Worksheets[sh];
                sheet.Range[1, 20].Text = (df).Date.ToString("yyyy-MM-dd");
                sheet.Range[2, 20].Text = (dt).Date.ToString("yyyy-MM-dd");
                var ln = 5;
                foreach (var crew in _grouped)
                {


                    sheet.Range[ln, 2].Text = crew.JobGroup;
                    sheet.Range[ln, 3].Text = crew.JobGroupCode;
                    sheet.Range[ln, 4].Text = crew.PID;



                    sheet.Range[ln, 5].Text = crew.ScheduleName;
                    sheet.Range[ln, 6].Text = crew.Position;
                    //f/l
                    sheet.Range[ln, 7].Text = toHHMM(crew.FlightTime).ToString();
                    //b/l
                    sheet.Range[ln, 8].Text = toHHMM(crew.BlockTime).ToString();

                    //f/l jl
                    sheet.Range[ln, 9].Text = toHHMM(crew.JLFlightTime).ToString();
                    //b/l jl
                    sheet.Range[ln, 10].Text = toHHMM(crew.JLBlockTime).ToString();

                    //fix
                    sheet.Range[ln, 11].Text = toHHMM(crew.FixTime).ToString();

                    sheet.Range[ln, 7].Style.Font.IsBold = true;
                    sheet.Range[ln, 8].Style.Font.IsBold = true;
                    sheet.Range[ln, 9].Style.Font.IsBold = true;
                    sheet.Range[ln, 10].Style.Font.IsBold = true;






                    sheet.Range[ln, 2, ln, 22].BorderInside(LineStyleType.Thin);
                    sheet.Range[ln, 2, ln, 22].BorderAround(LineStyleType.Thin);
                    //sheet.Rows[ln - 1].Style.Color = Color.FromArgb(230, 243, 255);
                    sheet.Range[ln, 2, ln, 22].Style.Color = Color.FromArgb(179, 230, 255);
                    sheet.Range[ln, 7, ln, 8].Style.Color = Color.FromArgb(217, 217, 217);
                    sheet.Range[ln, 9, ln, 10].Style.Color = Color.FromArgb(179, 255, 179);
                    //sheet.Range[ln, 10].Style.Color = Color.FromArgb(255, 179, 102);
                    var row = ln + 1;
                    foreach (var flt in crew.Flights)
                    {
                        sheet.Range[row, 2].Text = crew.JobGroup;
                        sheet.Range[row, 3].Text = crew.JobGroupCode;
                        sheet.Range[row, 4].Text = crew.PID;


                        sheet.Range[row, 5].Text = crew.ScheduleName;

                        sheet.Range[row, 6].Text = crew.Position;

                        // sheet.Range[row,2].Style.Color= Color.FromArgb(230, 247, 255);
                        // sheet.Range[row, 3].Style.Color = Color.FromArgb(230, 247, 255);
                        // sheet.Range[row, 4].Style.Color = Color.FromArgb(230, 247, 255);
                        //sheet.Range[row, 5].Style.Color = Color.FromArgb(230, 247, 255);


                        //f/l
                        sheet.Range[row, 7].Text = toHHMM(flt.FlightTime).ToString();
                        //b/l
                        sheet.Range[row, 8].Text = toHHMM(flt.BlockTime).ToString();
                        //f/l jl
                        sheet.Range[row, 9].Text = toHHMM(flt.JL_FlightTime).ToString();
                        //b/l jl
                        sheet.Range[row, 10].Text = toHHMM(flt.JL_BlockTime).ToString();
                        //fix
                        sheet.Range[row, 11].Text = toHHMM(flt.FixTime).ToString();

                        sheet.Range[row, 7, row, 8].Style.Color = Color.FromArgb(242, 242, 242);
                        sheet.Range[row, 9, row, 10].Style.Color = Color.FromArgb(230, 255, 230);
                        //sheet.Range[row, 10,row,10].Style.Color = Color.FromArgb(255, 179, 102);

                        sheet.Range[row, 12].Text = ((DateTime)flt.STDLocal).Date.ToString("yyyy-MM-dd");
                        sheet.Range[row, 13].Text = flt.Register;

                        sheet.Range[row, 14].Text = flt.FlightNumber;
                        sheet.Range[row, 15].Text = flt.FromAirportIATA;
                        sheet.Range[row, 16].Text = flt.ToAirportIATA;

                        sheet.Range[row, 17].Text = ((DateTime)flt.STDLocal).ToString("HH:mm");
                        sheet.Range[row, 18].Text = ((DateTime)flt.STALocal).ToString("HH:mm");


                        sheet.Range[row, 19].Text = flt.JL_OffBlockLocal != null ? ((DateTime)flt.JL_OffBlockLocal).ToString("HH:mm") : "";
                        sheet.Range[row, 20].Text = flt.JL_TakeOffLocal != null ? ((DateTime)flt.JL_TakeOffLocal).ToString("HH:mm") : "";
                        sheet.Range[row, 21].Text = flt.JL_LandingLocal != null ? ((DateTime)flt.JL_LandingLocal).ToString("HH:mm") : "";
                        sheet.Range[row, 22].Text = flt.JL_OnBlockLocal != null ? ((DateTime)flt.JL_OnBlockLocal).ToString("HH:mm") : "";

                        // sheet.Range[row, 16].Text = ((DateTime)flt.ch).Date.ToString("HH:mm");

                        sheet.Rows[row - 1].BorderInside(LineStyleType.Thin);
                        sheet.Rows[row - 1].BorderAround(LineStyleType.Thin);

                        row++;
                    }
                    sheet.GroupByRows(ln + 1, row - 1, true);

                    ln = ln + crew.Flights.Count + 1;
                }

                ////////////////////////////////////////
                sh++;
            }






            var name = "FlightTime-" + ((DateTime)df).ToString("yyyy-MMM-dd") + "-" + ((DateTime)dt).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;


        }

        [Route("api/xls/dispatch/daily/shift")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetIdeaUniqueSync(DateTime dt)
        {

            var ctx = new Models.dbEntities();
            ctx.Database.CommandTimeout = 500;
            var legs = ctx.AppLegs.Where(q => q.STDDay != null && q.STDDay == dt && q.FlightStatusID != 4 ).ToList();
            var pics = legs.Select(q => q.PICId).ToList();
            var piclegs = ctx.AppCrewFlights.Where(q => q.STDDayLocal == dt && pics.Contains(q.CrewId)).ToList();
            var piclegsIds = piclegs.Select(q => q.FlightId).ToList();
            var applegcrew = ctx.AppCrewFlights.Where(q => piclegsIds.Contains(q.FlightId)).ToList();
            var fdpsQry = from leg in piclegs

                          group leg by new { leg.FDPId } into grp
                          select new ShiftData()
                          {
                              FDPId = grp.Key.FDPId,
                              STD = grp.OrderBy(q => q.STD).Select(q => q.STD).First(),
                              REG = grp.First().Register,
                              Flights = grp.Select(q => new ShiftFlight()
                              {
                                  FlightId = q.FlightId,
                                  FlightNumber = q.FlightNumber,
                                  Dep = q.FromAirportIATA,
                                  Dest = q.ToAirportIATA,
                                  STD = q.STD,
                                  STDLocal = q.STDLocal,
                                  STA = q.STA,
                                  STALocal = q.STALocal
                              }).Distinct().OrderBy(q => q.STD).ToList(),
                              FlightIds = grp.Select(q => q.FlightId).Distinct().ToList()
                              //Cockpit=grp.Where(q=>q.JobGroupCode.StartsWith("00101")).OrderBy(q=>q.GroupOrder).Select(q=>q.ScheduleName).Distinct().ToList(),
                              // Cabin = grp.Where(q => q.JobGroupCode.StartsWith("00102")).OrderBy(q => q.GroupOrder).Select(q => q.ScheduleName).Distinct().ToList(),
                          };
            var _fdps = fdpsQry.OrderBy(q => q.STD).ToList();
            List<ShiftData> fdps = new List<ShiftData>();
            foreach (var rec in _fdps)
            {
                if (rec.FlightIds.Count == 4)
                {
                    var thrcnt = rec.Flights.Where(q => q.Dep == "THR").Count();
                    if (thrcnt < 2)
                    {
                        fdps.Add(rec);
                    }
                    else
                    {
                        var grp1 = rec.Flights.Take(2).ToList();
                        var grp2 = rec.Flights.Skip(2).Take(2).ToList();
                        fdps.Add(new ShiftData() { 
                          Flights=grp1,
                          FlightIds=grp1.Select(q=>q.FlightId).ToList(),
                          FDPId=rec.FDPId,
                          STD=grp1.First().STD,
                          REG=rec.REG,
                        });
                        fdps.Add(new ShiftData()
                        {
                            Flights = grp2,
                            FlightIds = grp2.Select(q => q.FlightId).ToList(),
                            FDPId = rec.FDPId,
                            STD = grp2.First().STD,
                            REG = rec.REG,
                        });
                    }
                }
                else
                    fdps.Add(rec);
            }

            foreach (var x in fdps)
            {
                var cabin = applegcrew.Where(q => x.FlightIds.Contains(q.FlightId) && q.JobGroupCode.StartsWith("00102")).OrderBy(q => q.GroupOrder).Select(q => q.ScheduleName).Distinct().ToList();
                var cockpit = applegcrew.Where(q => x.FlightIds.Contains(q.FlightId) && q.JobGroupCode.StartsWith("00101")).OrderBy(q => q.GroupOrder).Select(q => q.ScheduleName).Distinct().ToList();
                x.Cabin = cabin;
                x.Cockpit = cockpit;
            }

            var _sheets = new List<List<ShiftData>>();
            int fdpCount = 0;
            var _sheet = new List<ShiftData>();
            _sheets.Add(_sheet);
            foreach (var fdp in fdps)
            {
                _sheet.Add(fdp);
                if (_sheet.Count == 4)
                {
                    _sheet = new List<ShiftData>();
                    _sheets.Add(_sheet);
                }

            }
            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);
            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "shift" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);


            var sheetNumber = 0;
            foreach (var sh in _sheets)
            {
                Worksheet sheet = workbook.Worksheets[sheetNumber];
                sheet.Range[3, 3].Text = dt.ToString("yyyy-MM-dd");
                var ln = 6;
                foreach (var fdp in sh)
                {
                    sheet.Range[ln + 0, 2].Text = fdp.REG;

                    var _fr = ln + 3;
                    var _fc = 6;
                    foreach (var flt in fdp.Flights)
                    {
                        sheet.Range[_fr, 2].Text = flt.FlightNumber;

                        sheet.Range[ln, _fc].Text = flt.Dep;
                        sheet.Range[ln, _fc + 1].Text = flt.Dest;

                        sheet.Range[ln + 1, _fc].Text = ((DateTime)flt.STDLocal).ToString("HH:mm");
                        sheet.Range[ln + 1, _fc + 1].Text = ((DateTime)flt.STALocal).ToString("HH:mm");

                        _fc = _fc + 2;

                        _fr++;
                    }

                    var _cr = ln;
                    foreach (var c in fdp.Cockpit)
                    {
                        sheet.Range[_cr, 3].Text = c;

                        _cr++;
                    }

                    _cr = ln;
                    foreach (var c in fdp.Cabin)
                    {
                        sheet.Range[_cr, 4].Text = c;

                        _cr++;
                    }

                    ln = ln + 9;
                }


                sheetNumber++;
            }

            var name = "shift-" + ((DateTime)dt).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;



        }

        [Route("api/xls")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLS(DateTime dt1, DateTime dt2, int chr, int time, int cnl, int crew, int sort, int sep)
        {

            var _dt1 = dt1.Date;
            var _dt2 = dt2.Date.AddDays(0);
            var context = new Models.dbEntities();
            var query = (from x in context.ViewTimeTables
                         where x.STDDay >= _dt1 && x.STDDay <= _dt2
                         select x);
            if (cnl == -1)
                query = query.Where(q => q.FlightStatusID != 4);


            var totalcnt = query.Count();
            var grps = (from x in query
                        group x by new { x.STDDay } into grp
                        orderby grp.Key.STDDay
                        select grp).ToList();

            //var query = from x in context.ViewRosterCrewCounts
            //            where x.DateLocal >= _dt1 && x.DateLocal <= _dt2
            //            orderby x.DateLocal, x.STDLocal
            //            select x;
            //var _result = query.ToList();

            //return Ok(_result);
            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);

            List<string> clmns = new List<string>() { "Date",/* "PDate",*/ "FltNo", "Dep", "Arr", "Dep", "Arr", "Dep", "Arr", "A/C", "REG", "Remark" };

            Workbook workbook = new Workbook();

            workbook.Worksheets.Clear();
            Worksheet sheet;
            var startRow = 4;
            int r = -1;
            var newGrp = startRow + 2;
            foreach (var grp in grps)
            {
                if (sep == 1)
                    newGrp = startRow + 2;
                List<ViewTimeTable> flts = new List<ViewTimeTable>();
                switch (sort)
                {
                    case 1:
                        flts = grp.OrderBy(q => getFlightOrder(q)).ThenBy(q => q.STD).ThenBy(q => q.FromAirportIATA).ToList();
                        break;
                    case 2:
                        flts = grp.OrderBy(q => getFlightOrder(q)).ThenBy(q => q.AircraftType).ThenBy(q => q.Register).ThenBy(q => q.STD).ToList();
                        break;
                    case 3:
                        flts = grp.OrderBy(q => getFlightOrder(q)).ThenBy(q => q.STD).ToList();
                        break;
                    case 4:
                        flts = grp.OrderBy(q => getFlightOrder(q)).ThenBy(q => q.FromAirportIATA).ThenBy(q => q.STD).ToList();
                        break;
                    default:
                        flts = grp.OrderBy(q => getFlightOrder(q)).ThenBy(q => q.Register).ThenBy(q => q.STD).ToList();
                        break;
                }

                if (sep == 1)
                {

                    DateTime d = ((DateTime)grp.Key.STDDay);
                    PersianCalendar pc = new PersianCalendar();
                    var sheetName = string.Format("{0}_{1}_{2}", pc.GetYear(d), pc.GetMonth(d).ToString().PadLeft(2, '0'), pc.GetDayOfMonth(d).ToString().PadLeft(2, '0'));
                    //var sheetName = ((DateTime)grp.Key.STDDay).ToString("dddd dd-MMM-yyyy");
                    sheet = workbook.Worksheets.Add(sheetName);
                    sheet.PageSetup.Orientation = PageOrientationType.Portrait;
                }
                else
                {
                    var sheetName = "FLIGHTS";
                    sheet = workbook.Worksheets.Add(sheetName);
                    sheet.PageSetup.Orientation = PageOrientationType.Portrait;

                    // sheet.Range["A" + startRow + ":K" + (totalcnt + 5)].BorderInside(LineStyleType.Thin, Color.Black);
                    // sheet.Range["A" + startRow + ":K" + (totalcnt + 5)].BorderAround(LineStyleType.Medium, Color.Black);
                }


                //string picPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "logo.png");
               // ExcelPicture picture = sheet.Pictures.Add(1, 1, picPath);
                sheet.Range[1, 1].ColumnWidth = 10;
                sheet.Range[1, 1].RowHeight = 20;
              //  picture.TopRowOffset = 5;
              //  picture.LeftColumnOffset = 50;

                sheet.Range["A1:A2"].Merge();

                sheet.Range[1, 12].ColumnWidth = 40;
                //oopp
                sheet.Range["B1:H1"].Merge();
                //  sheet.Range[1, 2].Value = "TABAN AIRLINES FLIGHTS TIMETABLE";
                sheet.Range[1, 2].Value = "TIMETABLE";
                sheet.Range[1, 2].RowHeight = 30;
                sheet.Range[1, 2].Style.Font.Size = 14;

                sheet.Range[2, 2].Text = "Created At " + DateTime.Now.ToString("MMM-dd HH:mm");

                sheet.Range[1, 2].Style.HorizontalAlignment = HorizontalAlignType.Left;
                sheet.Range[1, 2].Style.VerticalAlignment = VerticalAlignType.Center;
                sheet.Range[1, 2].Style.Font.IsBold = true;
                sheet.Range[1, 2].Style.Font.FontName = "Times New Roman";

                //sheet.Range[1, 3].Value = "FLIGHTS TIMETABLE";
                //sheet.Range[1, 3].Style.Font.Size = 13;
                //sheet.Range[1, 3].Style.Font.IsBold = true;
                //sheet.Range[1, 3].Style.HorizontalAlignment = HorizontalAlignType.Left;
                //sheet.Range[1, 3].Style.VerticalAlignment = VerticalAlignType.Center;
                if (sep == 1)
                {
                    sheet.Range[1, 11].Text = ((DateTime)grp.Key.STDDay).ToString("yyyy-MMM-dd");
                    sheet.Range[1, 11].Style.Font.FontName = "Times New Roman";
                    sheet.Range[1, 11].Style.Font.Size = 11;
                    sheet.Range[1, 11].Style.Font.IsBold = true;

                    sheet.Range[1, 11].Style.HorizontalAlignment = HorizontalAlignType.Right;
                    sheet.Range[1, 11].Style.VerticalAlignment = VerticalAlignType.Center;
                    var pdate = grp.First().PDate.Substring(0, 10).Split('/').ToList();
                    sheet.Range[2, 11].Text = pdate[0] + "-" + pdate[1] + "-" + pdate[2];
                    sheet.Range[2, 11].Style.Font.FontName = "Times New Roman";
                    sheet.Range[2, 11].Style.Font.Size = 11;
                    sheet.Range[2, 9].Text = ((DateTime)grp.Key.STDDay).ToString("ddd");
                    sheet.Range[2, 9].Style.Font.IsBold = true;
                    sheet.Range[2, 9].Style.Font.FontName = "Times New Roman";
                    sheet.Range[2, 9].Style.Font.Size = 11;
                    sheet.Range[2, 9].Style.HorizontalAlignment = HorizontalAlignType.Right;

                    sheet.Range[2, 11].Style.Font.IsBold = true;
                    sheet.Range[2, 11].Style.HorizontalAlignment = HorizontalAlignType.Right;
                }




                // sheet.Range["A" + startRow + ":J" + (flts.Count + 5)].Style.HorizontalAlignment = HorizontalAlignType.Center;

                // sheet.Range["A" + startRow + ":K" + (flts.Count + 5)].Style.VerticalAlignment = VerticalAlignType.Bottom;

                // sheet.Range["A" + 1 + ":K" + 2].BorderAround(LineStyleType.Medium, Color.Black);

                sheet.Range["A" + 4 + ":K" + 5].BorderAround(LineStyleType.Thin, Color.Black);
                sheet.Range["A" + 4 + ":K" + 5].BorderInside(LineStyleType.Thin, Color.Black);

                sheet.Range["A" + 4 + ":K" + 5].Style.HorizontalAlignment = HorizontalAlignType.Center;
                //if (sep == 1)
                //{
                //    sheet.Range["A" + startRow + ":K" + (flts.Count + 5)].BorderInside(LineStyleType.Thin, Color.Black);

                //    sheet.Range["A" + startRow + ":K" + (flts.Count + 5)].BorderAround(LineStyleType.Medium, Color.Black);
                //}


                sheet.Range["A" + startRow + ":K" + startRow].Style.Color = Color.FromArgb(221, 255, 221);
                sheet.Range["A" + startRow + ":K" + startRow].Style.Font.IsBold = true;
                sheet.Range["A" + (startRow + 1) + ":K" + (startRow + 1)].Style.Color = Color.FromArgb(221, 255, 221);
                sheet.Range["A" + (startRow + 1) + ":K" + (startRow + 1)].Style.Font.IsBold = true;


                int c = 1;
                foreach (var clmn in clmns)
                {
                    sheet.Range[startRow + 1, c].Value = clmn;
                    sheet.Range[startRow + 1, c].Style.Font.FontName = "Courier New";
                    sheet.Range[startRow + 1, c].Style.Font.Size = 14;


                    if (c == 6)
                    {
                        sheet.Range[startRow, c].Value = "Local";
                        sheet.Range[startRow, c].Style.Font.FontName = "Courier New";
                        sheet.Range[startRow, c].Style.Font.Size = 14;
                    }
                    if (c == 8)
                    {
                        sheet.Range[startRow, c].Value = "UTC";
                        sheet.Range[startRow, c].Style.Font.FontName = "Courier New";
                        sheet.Range[startRow, c].Style.Font.Size = 14;
                    }
                    c++;
                }
                sheet.Rows[startRow].RowHeight = 18;
                sheet.Range["E" + startRow + ":F" + startRow].Merge();
                sheet.Range["G" + startRow + ":H" + startRow].Merge();
                var _lets = new List<string>() { "A", "B", "C", "D", /*"E",*/ "J", "K"/*, "L"*/ };
                foreach (var let in _lets)
                {
                    sheet.Range[let + startRow + ":" + let + (startRow + 1)].Merge();
                }



                if (sep == 1)
                {
                    r = startRow + 2;
                }
                else if (r == -1)
                {
                    r = startRow + 2;
                }

                var _reg = flts.First().Register;
                foreach (var flt in flts)
                {
                    flt.IP = "";
                    if (chr == 1)
                    {
                        flt.IP = flt.ChrCode;
                    }
                    if (chr == 2)
                    {
                        flt.IP = flt.ChrTitle;
                    }
                    if (flt.IP != "")
                        flt.IP += " - ";
                    if (crew == 1)
                    {
                        flt.IP += flt.Cockpit;
                    }
                    if (crew == 2)
                    {
                        flt.IP += flt.Cabin;
                    }
                    if (crew == 3)
                    {
                        flt.IP += flt.Cockpit + ", " + flt.Cabin;
                    }
                    if (flt.IP.Replace(" ", "") == "-")
                        flt.IP = "";
                    if (flt.FlightStatusID == 4)
                    {
                        flt.AircraftType = "-";
                        // sheet.Rows[r].Style.Color = Color.Silver;

                    }
                    // List<string> clmns = new List<string>() {"Date","PDate","Day","Flight No","Status","Reg" };

                    if (r == newGrp)
                    {
                        sheet[r, 1].RowHeight = 23;
                        sheet.Range[r, 1].Text = ((DateTime)flt.STDDayLocal).ToString("yyyy-MM-dd");
                        sheet.Range[r, 1].Style.Font.Size = 14;
                        sheet.Range[r, 1].Style.Font.IsBold = true;
                        sheet.Range[r, 1].Style.Font.FontName = "Courier New";
                        sheet.Range[r, 1].AutoFitColumns();
                        sheet.Range[r, 1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    }
                    else
                    if (r == newGrp + 1)
                    {
                        var _pdate = flt.PDate.Substring(0, 10).Split('/').ToList();

                        sheet[r, 1].RowHeight = 23;
                        sheet.Range[r, 1].Text = _pdate[0] + "-" + _pdate[1] + "-" + _pdate[2];
                        sheet.Range[r, 1].Style.Font.Size = 14;
                        sheet.Range[r, 1].Style.Font.IsBold = true;
                        sheet.Range[r, 1].Style.Font.FontName = "Courier New";
                        sheet.Range[r, 1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        sheet.Range[r, 1].AutoFitColumns();

                        // sheet[r + 1, 1].RowHeight = 23;
                        // sheet.Range[r + 1, 1].Text = ((DateTime)flt.STDDayLocal).ToString("ddd");
                        // sheet.Range[r + 1, 1].Style.Font.Size = 14;
                        //sheet.Range[r + 1, 1].Style.Font.IsBold = true;
                        //sheet.Range[r + 1, 1].Style.Font.FontName = "Courier New";
                        // sheet.Range[r + 1, 1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        //sheet.Range[r + 1, 1].AutoFitColumns();
                    }
                    else
                    {
                        //sheet[r, 1].RowHeight = 23;
                        //sheet.Range[r, 1].Text = "'''";
                        //sheet.Range[r, 1].Style.Font.Size = 14;
                        //sheet.Range[r, 1].Style.Font.IsBold = true;
                        //sheet.Range[r, 1].Style.Font.FontName = "Courier New";
                        //sheet.Range[r, 1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        //sheet.Range[r, 1].AutoFitColumns();
                    }
                    sheet.Range[r, 1].Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
                    //sheet.Range[r, 2].Text = flt.PDateLocal;
                    //sheet.Range[r, 2].Style.Font.FontName = "Courier New";
                    //sheet.Range[r,2 ].Style.Font.Size = 14;
                    //sheet.Range[r, 2].Style.Font.IsBold = true;
                    //sheet.Range[r, 2].AutoFitColumns();

                    sheet.Range[r, 2].Text = flt.FlightNumber;
                    sheet.Range[r, 2].Style.Font.FontName = "Courier New";
                    sheet.Range[r, 2].Style.Font.Size = 14;
                    sheet.Range[r, 2].Style.Font.IsBold = true;
                    sheet.Range[r, 2].Style.HorizontalAlignment = HorizontalAlignType.Left;
                    //sheet.Range[r, 3].AutoFitColumns();
                    sheet.Range[r, 3].Text = flt.FromAirportIATA;
                    sheet.Range[r, 3].Style.Font.FontName = "Courier New";
                    sheet.Range[r, 3].Style.Font.Size = 14;
                    sheet.Range[r, 3].Style.Font.IsBold = true;
                    sheet.Range[r, 3].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    // sheet.Range[r, 4].AutoFitColumns();
                    sheet.Range[r, 4].Text = flt.ToAirportIATA;
                    sheet.Range[r, 4].Style.Font.FontName = "Courier New";
                    sheet.Range[r, 4].Style.Font.Size = 14;
                    sheet.Range[r, 4].Style.Font.IsBold = true;
                    sheet.Range[r, 4].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    // sheet.Range[r, 5].AutoFitColumns();

                    if (flt.FlightStatusID != 4)
                    {
                        sheet.Range[r, 5].Text = ((DateTime)flt.STDLocal).ToString("HH:mm");
                        sheet.Range[r, 5].Style.Font.FontName = "Courier New";
                        sheet.Range[r, 5].Style.Font.Size = 14;
                        sheet.Range[r, 5].Style.Font.IsBold = true;
                        sheet.Range[r, 5].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        sheet.Range[r, 5].AutoFitColumns();

                        sheet.Range[r, 6].Text = ((DateTime)flt.STALocal).ToString("HH:mm");
                        sheet.Range[r, 6].Style.Font.FontName = "Courier New";
                        sheet.Range[r, 6].Style.Font.Size = 14;
                        sheet.Range[r, 6].Style.Font.IsBold = true;
                        sheet.Range[r, 6].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        sheet.Range[r, 6].AutoFitColumns();
                    }


                    if (flt.FlightStatusID != 4)
                    {
                        sheet.Range[r, 7].Text = ((DateTime)flt.STD).ToString("HH:mm");
                        sheet.Range[r, 7].Style.Font.FontName = "Courier New";
                        sheet.Range[r, 7].Style.Font.Size = 14;
                        sheet.Range[r, 7].Style.Font.IsBold = true;
                        sheet.Range[r, 7].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        sheet.Range[r, 7].AutoFitColumns();


                        sheet.Range[r, 8].Text = ((DateTime)flt.STA).ToString("HH:mm");
                        sheet.Range[r, 8].Style.Font.FontName = "Courier New";
                        sheet.Range[r, 8].Style.Font.Size = 14;
                        sheet.Range[r, 8].Style.Font.IsBold = true;
                        sheet.Range[r, 8].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        sheet.Range[r, 8].AutoFitColumns();
                    }
                    if (flt.FlightStatusID == 4)
                    {
                        sheet.Range[r, 5].Text = "Cancel";
                        sheet.Range[r, 5].Style.Font.FontName = "Courier New";
                        sheet.Range[r, 5].Style.Font.Size = 14;
                        sheet.Range[r, 5].Style.Font.IsBold = true;
                        sheet.Range[r, 5].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        sheet.Range["E" + r + ":H" + r].Merge();


                        sheet.Rows[r - 1].Style.Color = Color.Silver;
                        sheet.Range[r, 5].AutoFitColumns();

                    }




                    sheet.Range[r, 9].Text = flt.FlightStatusID == 4 ? "xld" : flt.AircraftType;
                    sheet.Range[r, 9].Style.Font.FontName = "Courier New";
                    sheet.Range[r, 9].Style.Font.Size = 14;
                    sheet.Range[r, 9].Style.Font.IsBold = true;
                    sheet.Range[r, 9].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[r, 9].AutoFitColumns();

                    sheet.Range[r, 10].Text =/* flt.FlightStatusID == 4 ? "TBF" :*/ flt.Register;
                    sheet.Range[r, 10].Style.Font.FontName = "Courier New";
                    sheet.Range[r, 10].Style.Font.Size = 14;
                    sheet.Range[r, 10].Style.Font.IsBold = true;
                    sheet.Range[r, 10].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[r, 10].AutoFitColumns();


                    sheet.Range[r, 11].Text = flt.IP;
                    sheet.Range[r, 11].Style.Font.FontName = "Courier New";
                    sheet.Range[r, 11].Style.Font.Size = 14;
                    sheet.Range[r, 11].Style.Font.IsBold = true;

                    sheet.Range[r, 11].Style.HorizontalAlignment = HorizontalAlignType.Right;
                    sheet.Range[r, 11].Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                    //sheet.Range[r, 11].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thick;
                    sheet.Rows[r - 1].BorderInside(LineStyleType.Thin, Color.Black);

                    //sheet.Rows[r - 1].Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Medium;
                    //sheet.Rows[r - 1].Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Medium;
                    if (flt != flts.Last() && (flt.Register == _reg || flt.Register == "CNL"))
                        sheet.Rows[r - 1].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;
                    else
                    {
                        if (flt == flts.Last())
                            sheet.Rows[r - 1].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;
                        else
                            sheet.Rows[r - 2].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Medium;
                    }

                    if (flt.Register != _reg)
                    {

                        _reg = flt.Register;
                    }
                    r++;

                }
                //sheet.Rows[r - 2].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Medium;
                //r++;
                newGrp = r;

                sheet.Columns[10].AutoFitColumns();
                sheet.Columns[0].AutoFitColumns();
                sheet.Columns[10].Style.HorizontalAlignment = HorizontalAlignType.Right;
            }
            var name = "Flights_Report_" + dt1.ToString("yyyy-MMM-dd") + "_" + dt2.ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");

            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }

        int getFlightOrder(ViewTimeTable t)
        {
            if (t.FlightStatusID == 4)
                return 1000;
            else
                return 1;
        }



        [Route("api/flight/daily/export")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetFlightsDailyExport(DateTime df, DateTime dt, string regs, string routes, string from, string to, string no, string status
           , string type2
           , string idx
           , string chr
           , string time
           , string fuel
           , string weight
            ,string dateref)
        {

            var context = new Models.dbEntities();

            var dayprm = dateref == "std" ? "STDDayLocal" : "TakeOffDayLocal";

            var cmd = "select * from viewflightdailylcb ";
            //string whr = "FlightStatusId<>4  and (STDDayLocal>='" + df.ToString("yyyy-MM-dd") + "' and STDDayLocal<='" + dt.ToString("yyyy-MM-dd") + "')";
            string whr = " ("+ dayprm +">= '" + df.ToString("yyyy-MM-dd") + "' and "+ dayprm +"<= '" + dt.ToString("yyyy-MM-dd") + "')";

            if (!string.IsNullOrEmpty(status) && status != "-1")
            {
                var _regs = status.Split('_').ToList();
                var col = _regs.Select(q => "status=" + q).ToList();
                var _whr = "(" + string.Join(" OR ", col) + ")";
                whr += " AND " + _whr;
            }
            if (!string.IsNullOrEmpty(type2) && type2 != "-1")
            {
                var _regs = type2.Split('_').ToList();
                var col = _regs.Select(q => "FlightType2=N'" + q + "'").ToList();
                var _whr = "(" + string.Join(" OR ", col) + ")";
                whr += " AND " + _whr;
            }

            if (!string.IsNullOrEmpty(idx) && idx != "-1")
            {
                var _regs = idx.Split('_').ToList();
                var col = _regs.Select(q => "FlightIndex=N'" + q + "'").ToList();
                var _whr = "(" + string.Join(" OR ", col) + ")";
                whr += " AND " + _whr;
            }

            if (!string.IsNullOrEmpty(chr) && chr != "-1")
            {
                var _regs = chr.Split('_').ToList();
                var col = _regs.Select(q => "ChrTitle=N'" + q + "'").ToList();
                var _whr = "(" + string.Join(" OR ", col) + ")";
                whr += " AND " + _whr;
            }


            if (!string.IsNullOrEmpty(regs) && regs != "-1")
            {
                var _regs = regs.Split('_').ToList();
                var col = _regs.Select(q => "Register='" + q + "'").ToList();
                var _whr = "(" + string.Join(" OR ", col) + ")";
                whr += " AND " + _whr;
            }

            if (!string.IsNullOrEmpty(from) && from != "-1")
            {
                var _regs = from.Split('_').ToList();
                var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FromAirportIATA='" + q + "'").ToList()) + ")";
                whr += " AND " + _whr;
            }

            if (!string.IsNullOrEmpty(to) && to != "-1")
            {
                var _regs = to.Split('_').ToList();
                var _whr = "(" + string.Join(" OR ", _regs.Select(q => "ToAirportIATA='" + q + "'").ToList()) + ")";
                whr += " AND " + _whr;
            }

            if (!string.IsNullOrEmpty(no) && no != "-1")
            {
                var _regs = no.Split('_').ToList();
                var _whr = "(" + string.Join(" OR ", _regs.Select(q => "FlightNumber='" + q + "'").ToList()) + ")";
                whr += " AND " + _whr;
            }

            if (!string.IsNullOrEmpty(routes) && routes != "-1")
            {
                var _regs = routes.Split('_').ToList();
                var _whr = "(" + string.Join(" OR ", _regs.Select(q => "Route like '%" + q + "%'").ToList()) + ")";
                whr += " AND " + _whr;
            }

            cmd = cmd + " WHERE " + whr + " ORDER BY  STDDay,AircraftType,Register,STD";

            var flts = context.ViewFlightDailyLCBs
                        .SqlQuery(cmd)
                        .ToList<ViewFlightDailyLCB>();

            //var result = await courseService.GetEmployeeCertificates(id);

            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);
            Workbook workbook = new Workbook();
            //1
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "fltdaily" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[6, 21].Text = weight == "kg" ? "CARGO (KG)" : "CARGO (LBS)";
            sheet.Range[6, 20].Text = weight == "kg" ? "BAGGAGES (KG)" : "BAGGAGES (LBS)";
            sheet.Range[6, 36].Text = "FUEL (LTR)";
            if (fuel == "lbs")
            {

                sheet.Range[6, 36].Text = "FUEL (LBS)";
            }

            if (fuel == "kg")
            {

                sheet.Range[6, 36].Text = "FUEL (KG)";
            }

            var ln = 8;
            foreach (var flt in flts)
            {
                sheet.InsertRow(ln);
                sheet.Range[ln, 1].Text = dateref == "std"? flt.PMonthName:flt.PMonthNameTakeOff;
                sheet.Range[ln, 2].Text = flt.FlightType2;
                sheet.Range[ln, 3].Text = flt.FlightIndex;
                sheet.Range[ln, 4].Value2 = flt.FlightStatus;
                sheet.Range[ln, 5].Text = dateref == "std"? flt.PDayName:flt.PDayNameTakeOff;
                sheet.Range[ln, 6].Value2 = dateref == "std"? flt.STDDayLocal :flt.TakeOffDayLocal;
                sheet.Range[ln, 6].NumberFormat = "yyyy-mm-dd";
                sheet.Range[ln, 7].Text = dateref == "std"? flt.PDate:flt.PDateTakeOff;

                var isFlightNumeric = int.TryParse(flt.FlightNumber, out int flightNumberTemp);

                if (isFlightNumeric)
                    sheet.Range[ln, 8].NumberValue = flightNumberTemp;
                else
                    sheet.Range[ln, 8].Text = flt.FlightNumber;

                sheet.Range[ln, 9].Text = flt.AircraftType;
                sheet.Range[ln, 10].Text = flt.Register;
                sheet.Range[ln, 11].Text = flt.FromAirportIATA;
                sheet.Range[ln, 12].Text = flt.ToAirportIATA;

                sheet.Range[ln, 13].Value2 = flt.PaxAdult;
                sheet.Range[ln, 13].NumberFormat = "0";
                sheet.Range[ln, 14].Value2 = flt.PaxChild;
                sheet.Range[ln, 14].NumberFormat = "0";
                sheet.Range[ln, 15].Value2 = flt.PaxInfant;
                sheet.Range[ln, 15].NumberFormat = "0";
                sheet.Range[ln, 16].Formula = "=SUM(Sheet2!$M$" + ln + ":N$" + ln + ")";
                // sheet.Range[ln, 15].Value2 = flt.RevPax;
                sheet.Range[ln, 16].NumberFormat = "0";
                //  sheet.Range[ln, 16].Value2 = flt.TotalPax;
                sheet.Range[ln, 17].Formula = "=SUM(Sheet2!$M$" + ln + ":O$" + ln + ")";
                sheet.Range[ln, 17].NumberFormat = "0";

                sheet.Range[ln, 18].Value2 = null;
                sheet.Range[ln, 18].NumberFormat = "0";
                sheet.Range[ln, 19].Value2 = null;
                sheet.Range[ln, 19].NumberFormat = "0";



                sheet.Range[ln, 20].Value2 = weight == "kg" ? flt.BaggageWeightKg : flt.BaggageWeightLbs;
                sheet.Range[ln, 20].NumberFormat = "0";


                sheet.Range[ln, 21].Value2 = weight == "kg" ? flt.CargoWeightKg : flt.CargoWeightLbs;
                sheet.Range[ln, 21].NumberFormat = "0";

                sheet.Range[ln, 22].Value2 = flt.TotalSeat;
                sheet.Range[ln, 22].NumberFormat = "0";

                sheet.Range[ln, 23].Value2 = flt.EmptySeat;
                sheet.Range[ln, 23].NumberFormat = "0";


                sheet.Range[ln, 24].Value2 = null;
                sheet.Range[ln, 24].NumberFormat = "0";

                var _std = time == "lcl" ? ((DateTime)flt.STDLocal) : ((DateTime)flt.STD);
                //var _stdHour = _std.Hour; //(_std.Hour > 12 ? _std.Hour - 12 : _std.Hour);
                var _sta = time == "lcl" ? ((DateTime)flt.STALocal) : ((DateTime)flt.STA);
                //var _staHour = _sta.Hour; //(_sta.Hour > 12 ? _sta.Hour - 12 : _sta.Hour);

                sheet.Range[ln, 25].TimeSpanValue = _std.TimeOfDay;
                sheet.Range[ln, 25].NumberFormat = "hh:MM";

                sheet.Range[ln, 26].TimeSpanValue = _sta.TimeOfDay;
                sheet.Range[ln, 26].NumberFormat = "hh:MM";

                //sheet.Range[ln, 24].Cells[0].Text = _stdHour.ToString().PadLeft(2, '0') + ":" + _std.Minute.ToString().PadLeft(2, '0'); /*+ (_std.Hour<12?" AM":" PM")*/; //flt.STDLocal;
                //sheet.Range[ln, 25].Cells[0].Text = _staHour.ToString().PadLeft(2, '0') + ":" + _sta.Minute.ToString().PadLeft(2, '0');   /*+(_sta.Hour < 12 ? " AM" : " PM")*/; //flt.STALocal;

                sheet.Range[ln, 27].Formula = "IF(Z" + ln + ">=Y" + ln + ",Z" + ln + "-Y" + ln + ",Z" + ln + "-Y" + ln + "+1)";
                sheet.Range[ln, 27].NumberFormat = "hh:MM";

                var _takeoff = time == "lcl" ? ((DateTime)flt.TakeOffLocal) : ((DateTime)flt.TakeOff);
                var _landing = time == "lcl" ? ((DateTime)flt.LandingLocal) : ((DateTime)flt.Landing);


                sheet.Range[ln, 28].TimeSpanValue = _takeoff.TimeOfDay;
                sheet.Range[ln, 28].NumberFormat = "hh:MM";
                sheet.Range[ln, 29].TimeSpanValue = _landing.TimeOfDay;
                sheet.Range[ln, 29].NumberFormat = "hh:MM";
                sheet.Range[ln, 30].Formula = "IF(AC" + ln + ">=AB" + ln + ",AC" + ln + "-AB" + ln + ",AC" + ln + "-AB" + ln + "+1)";
                sheet.Range[ln, 30].NumberFormat = "hh:MM";

                var _offblock = time == "lcl" ? ((DateTime)flt.BlockOffLocal) : ((DateTime)flt.BlockOff);
                var _onblock = time == "lcl" ? ((DateTime)flt.BlockOnLocal) : ((DateTime)flt.BlockOn);

                sheet.Range[ln, 31].TimeSpanValue = _offblock.TimeOfDay;
                sheet.Range[ln, 32].TimeSpanValue = _onblock.TimeOfDay;
                sheet.Range[ln, 33].Formula = "IF(AF" + ln + ">=AE" + ln + ",AF" + ln + "-AE" + ln + ",AF" + ln + "-AE" + ln + "+1)";

                sheet.Range[ln, 31].NumberFormat = "hh:MM";
                sheet.Range[ln, 32].NumberFormat = "hh:MM";
                sheet.Range[ln, 33].NumberFormat = "hh:MM";

                //sheet.Range[ln, 34].Formula = "IF(AE" + ln + ">=Y" + ln + ",IF(AE" + ln + "==Y" + ln + ",0,AE" + ln + "-Y" + ln + "),AE" + ln + "-Y" + ln + "+1)";
                sheet.Range[ln, 34].Formula = "IF(AE" + ln + ">=Y" + ln + ",IF(AE" + ln + "==Y" + ln + ",0,AE" + ln + "-Y" + ln + "),IF(AND(AF"+ln+"<Y"+ln+",AB"+ln+"<Y"+ln+",AC"+ln+"<Y"+ln+"),AE" + ln + "-Y" + ln + "+1,0))";
                sheet.Range[ln, 34].NumberFormat = "hh:MM";

                //DelayReason
                sheet.Range[ln, 35].Value2 = flt.DelayReason;
                //UsedFuel
                var _fuel = flt.UpliftLtr;

                if (fuel == "lbs")
                {
                    _fuel = flt.UpliftLbs;

                }

                if (fuel == "kg")
                {
                    _fuel = flt.UpliftKg;

                }


                sheet.Range[ln, 36].Value2 = _fuel;
                sheet.Range[ln, 36].NumberFormat = "0";
                //Distance
                sheet.Range[ln, 37].Value2 = flt.Distance;
                sheet.Range[ln, 37].NumberFormat = "0";
                //ChrTitle
                sheet.Range[ln, 38].Value2 = flt.ChrTitle;
                //StationIncome
                sheet.Range[ln, 39].Value2 = flt.StationIncome;
                sheet.Range[ln, 39].NumberFormat = "0";
                //--agancies

                //Remark
                sheet.Range[ln, 54].Value2 = flt.TotalRemark;


                //sheet.Range[r, 11].Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                //sheet.Rows[ln-1].Borders.LineStyle = LineStyleType.Thin;
                sheet.Rows[ln - 1].BorderInside(LineStyleType.Double);
                sheet.Rows[ln - 1].BorderAround(LineStyleType.Double);

                sheet.Rows[ln - 1].Style.Font.IsBold = true;
                ln++;
            }

            sheet.InsertRow(ln);
            var frm_paxadult = "=SUM(Sheet2!$M$8:M$" + (ln - 1) + ")";
            sheet.Range[ln, 13].Formula = frm_paxadult;

            var frm_paxchild = "=SUM(Sheet2!$N$8:N$" + (ln - 1) + ")";
            sheet.Range[ln, 14].Formula = frm_paxchild;

            var frm_paxinfant = "=SUM(Sheet2!$O$8:O$" + (ln - 1) + ")";
            sheet.Range[ln, 15].Formula = frm_paxinfant;

            sheet.Range[ln, 16].Formula = "=SUM(Sheet2!$P$8:P$" + (ln - 1) + ")";
            sheet.Range[ln, 17].Formula = "=SUM(Sheet2!$Q$8:Q$" + (ln - 1) + ")";
            sheet.Range[ln, 18].Formula = "=SUM(Sheet2!$R$8:R$" + (ln - 1) + ")";
            sheet.Range[ln, 19].Formula = "=SUM(Sheet2!$S$8:S$" + (ln - 1) + ")";
            sheet.Range[ln, 20].Formula = "=SUM(Sheet2!$T$8:T$" + (ln - 1) + ")";
            sheet.Range[ln, 21].Formula = "=SUM(Sheet2!$U$8:U$" + (ln - 1) + ")";

            //fixtime
            //Spire.Xls.CellFormatType.Unknown
            sheet.Range[ln, 27].Formula = "=SUM(Sheet2!$AA$8:AA$" + (ln - 1) + ")";
            sheet.Range[ln, 27].Style.NumberFormat = "[h]:mm;@";// .NumberFormat = "[h]:mm;@";

            //  sheet.Range[ln, 26].Style.NumberFormatSettings
            //sheet.Range[ln, 26].DateTimeValue = System.DateTime.Today;



            sheet.Range[ln, 30].Formula = "=SUM(Sheet2!$AD$8:AD$" + (ln - 1) + ")";
            sheet.Range[ln, 30].Style.NumberFormat = "[h]:mm;@";
            sheet.Range[ln, 33].Formula = "=SUM(Sheet2!$AG$8:AG$" + (ln - 1) + ")";
            sheet.Range[ln, 33].Style.NumberFormat = "[h]:mm;@";

            sheet.Range[ln, 34].Formula = "=SUM(Sheet2!$AH$8:AH$" + (ln - 1) + ")";
            sheet.Range[ln, 34].Style.NumberFormat = "[h]:mm;@";

            sheet.Range[ln, 36].Formula = "=SUM(Sheet2!$AJ$8:AJ$" + (ln - 1) + ")";
            sheet.Range[ln, 37].Formula = "=SUM(Sheet2!$AK$8:AK$" + (ln - 1) + ")";

            sheet.Range[ln, 39].Formula = "=SUM(Sheet2!$AM$8:AM$" + (ln - 1) + ")";



            sheet.Rows[ln - 1].BorderInside(LineStyleType.Double);
            sheet.Rows[ln - 1].BorderAround(LineStyleType.Double);
            sheet.Rows[ln - 1].Style.Font.IsBold = true;


            var name = "daily-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }







        public class ShiftFlight
        {
            public int? FlightId { get; set; }
            public string FlightNumber { get; set; }
            public string Dep { get; set; }
            public string Dest { get; set; }
            public DateTime? STD { get; set; }
            public DateTime? STDLocal { get; set; }
            public DateTime? STA { get; set; }
            public DateTime? STALocal { get; set; }
        }
        public class ShiftData
        {
            public int FDPId { get; set; }
            public DateTime? STD { get; set; }
            public string REG { get; set; }
            public List<ShiftFlight> Flights { get; set; }
            public List<string> Cockpit { get; set; }
            public List<string> Cabin { get; set; }
            public List<int?> FlightIds { get; set; }

        }


    }
}
