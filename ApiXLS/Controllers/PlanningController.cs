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

namespace ApiXLS.Controllers
{
    public class PlanningController : ApiController
    {
        // Uploads file to an upload folder
        [Route("api/plan/uploadfile")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                IHttpActionResult outPut = Ok(200);

                string key = string.Empty;
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var date = DateTime.Now;
                        var formType = httpRequest.Form["form"];
                        DateTime fromDate = Convert.ToDateTime(httpRequest.Form["fromDate"]);
                        DateTime toDate = Convert.ToDateTime(httpRequest.Form["toDate"]);
                        var ext = System.IO.Path.GetExtension(postedFile.FileName);
                        
                        key = formType + "-" + date.ToString("yyyyMMddhhmm") + ext;

                        var filePath = HttpContext.Current.Server.MapPath("~/upload/" + key);
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);

                        outPut = await UploadFlights(key, fromDate, toDate);
                    }
                }
                else
                {
                    outPut = BadRequest();
                }
                return outPut;
            }
            catch (Exception ex)
            {
                return Ok(ex.Message + "   IN    " + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }

        }


        [Route("api/plan/xls/uploadflights")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> UploadFlights(string fileName, DateTime fromDate, DateTime toDate)
        {
            var utcs = new List<utcdiff>()
            {
                 new utcdiff(){ iata="NJF",diff=180},
                 new utcdiff(){ iata="DYU",diff=300},
                 new utcdiff(){ iata="KWI",diff=180},
                 new utcdiff(){ iata="TBS",diff=240}, //Tibilisi, Georgia
                 new utcdiff(){ iata="BUS",diff=240}, // Batumi, Georgia
                 new utcdiff(){ iata="FRU",diff=360}, //Bishkek, Kyrgyzstan
            };
            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";
            Spire.License.LicenseProvider.SetLicenseKey(LData);

            if (!fileName.ToLower().Contains(".xls"))
                fileName += ".xlsx";
            var filePath = HttpContext.Current.Server.MapPath("~/upload/" + fileName);
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(filePath);
                        
            var days = new List<string>() { "SAT", "SUN", "MON", "TUE", "WED", "THU", "FRI" };
            var xlsFlts = new List<xlsflt>();
            

            foreach (Worksheet sheet in workbook.Worksheets)
            {
                var sheetCount = 0;
                var _doLoop = true;
                var FltNoHeader = sheet.FindString("FltNo", false, true);
                var contentRow = FltNoHeader.Row;
                if (FltNoHeader.HasMerged)
                    contentRow = FltNoHeader.MergeArea.LastRow + 1;
                var FltNoCol = FltNoHeader.Column;
                //var DepStnCol = FltNoCol + 1;
                //var ArrStnCol = FltNoCol + 2;
                //var DepCol = FltNoCol + 3;
                //var ArrCol = FltNoCol + 4;
                var RegCol = sheet.FindString("REG", false, true).Column;

                while (_doLoop)
                {
                    var day = days[sheetCount];
                    if (sheet.Name.ToUpper().Contains(day.ToUpper()))
                    {
                        var origin = !string.IsNullOrEmpty(sheet.Range[contentRow, FltNoCol + 1].Text) ? sheet.Range[contentRow, FltNoCol + 1].Text.Replace(" ", "") : "";
                        var destination = !string.IsNullOrEmpty(sheet.Range[contentRow, FltNoCol + 2].Text) ? sheet.Range[contentRow, FltNoCol + 2].Text.Replace(" ", "") : "";
                        if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
                            _doLoop = false;
                        else
                        {
                            var flightNumber = sheet.Range[contentRow, FltNoCol].Value2.ToString();
                            var _dep = sheet.Range[contentRow, FltNoCol + 3].DateTimeValue;
                            if (_dep == DateTime.MinValue)
                            {
                                var _depStr = sheet.Range[contentRow, FltNoCol + 3].Value.Split(':');
                                _dep = new DateTime(2000, 1, 1, Convert.ToInt32(_depStr[0]), Convert.ToInt32(_depStr[1]), 0);
                            }
                            var _arr = sheet.Range[contentRow, FltNoCol + 4].DateTimeValue;
                            if (_arr == DateTime.MinValue)
                            {
                                var _arrStr = sheet.Range[contentRow, FltNoCol + 4].Value.Split(':');
                                _arr = new DateTime(2000, 1, 1, Convert.ToInt32(_arrStr[0]), Convert.ToInt32(_arrStr[1]), 0);
                            }
                            var reg = sheet.Range[contentRow, RegCol].Text;

                            xlsFlts.Add(new xlsflt() { day = day, arr = _arr, dep = _dep, route = origin + "-" + destination, origin = origin, destination = destination, flightNumber = flightNumber, reg = reg });
                            contentRow++;
                        }   
                    } else
                    {
                        sheetCount++;
                    }                        
                }
            }
            
            var context = new ApiXLS.Models.dbEntities();
            var dblegs = await context.ViewLegTimes.Where(q => q.STDLocal >= fromDate && q.STDLocal <= toDate).ToListAsync();
            var dbids = dblegs.Select(q => q.ID).ToList();
            var dbflights = await context.FlightInformations.Where(q => dbids.Contains(q.ID)).ToListAsync();
            var airports = await context.Airports.ToListAsync();
            var registers = await context.Ac_MSN.ToListAsync();

            var _date = fromDate;
            var cnt = 0;
            while (_date <= toDate)
            {
                var _day = _date.ToString("ddd").ToUpper();
                var dbs = dblegs.Where(q => q.STDLocal >= _date && q.STDLocal < _date.AddDays(1)).ToList();
                var dayFlights = xlsFlts.Where(q => q.day == _day).Where(q => !string.IsNullOrEmpty(q.route)).ToList();
                foreach (var dayFlight in dayFlights)
                {
                    var fromIATA = airports.Where(q => q.IATA == dayFlight.origin).FirstOrDefault();
                    var toIATA = airports.Where(q => q.IATA == dayFlight.destination).FirstOrDefault();
                    var register = registers.Where(q => q.Register == dayFlight.reg).FirstOrDefault();
                    if (fromIATA != null && toIATA != null && register != null)
                    {
                        var _dep = (DateTime)dayFlight.dep;
                        var _arr = (DateTime)dayFlight.arr;

                        var isFromInt = utcs.FirstOrDefault(q => q.iata == fromIATA.IATA);
                        var isToInt = utcs.FirstOrDefault(q => q.iata == toIATA.IATA);

                        DateTime stdlocal = new DateTime(_date.Year, _date.Month, _date.Day, _dep.Hour, _dep.Minute, 0);
                        DateTime stalocal = new DateTime(_date.Year, _date.Month, _date.Day, _arr.Hour, _arr.Minute, 0);
                        if (stalocal < stdlocal)
                            stalocal = stalocal.AddDays(1);

                        DateTime std;
                        if (isFromInt == null)
                            std = stdlocal.AddMinutes(-1 * TimeZoneInfo.Local.GetUtcOffset(stdlocal).TotalMinutes);
                        else
                            std = stdlocal.AddMinutes(-1 * isFromInt.diff);

                        DateTime sta;
                        if (isToInt == null)
                            sta = stalocal.AddMinutes(-1 * TimeZoneInfo.Local.GetUtcOffset(stalocal).TotalMinutes);
                        else
                            sta = stalocal.AddMinutes(-1 * isToInt.diff);

                        double totalMin = (stalocal - stdlocal).TotalMinutes;
                        double hours = Math.Floor(totalMin / 60);
                        double minutes = totalMin - hours * 60;

                        var exist = dbs.FirstOrDefault(q => q.FlightNumber == dayFlight.flightNumber);
                        FlightInformation flt = null;

                        if (exist != null)
                        {
                            flt = dbflights.FirstOrDefault(q => q.ID == exist.ID);
                            if (flt != null)
                            {
                                //flt.FlightNumber = dayFlight.flightNumber;
                                flt.RegisterID = register.ID;
                                flt.FromAirportId = fromIATA.Id;
                                flt.ToAirportId = toIATA.Id;
                                flt.STD = std;
                                flt.STA = sta;

                                flt.FlightTypeID = 9;
                                flt.FlightStatusID = 1;
                                flt.CustomerId = 4;

                                flt.ChocksOut = std;
                                flt.ChocksIn = sta;
                                flt.Takeoff = std;
                                flt.Landing = sta;

                                try
                                {
                                    flt.FlightH = Convert.ToInt32(hours);
                                    flt.FlightM = Convert.ToByte(minutes);
                                }
                                catch
                                {
                                    flt.FlightH = 0;
                                    flt.FlightM = 0;
                                }
                            }
                        }
                        else
                        {
                            flt = new FlightInformation()
                            {
                                FlightNumber = dayFlight.flightNumber,
                                RegisterID = register.ID,
                                FromAirportId = fromIATA.Id,
                                ToAirportId = toIATA.Id,
                                STD = std,
                                STA = sta,

                                FlightTypeID = 9,
                                FlightStatusID = 1,
                                CustomerId = 4,

                                ChocksOut = std,
                                ChocksIn = sta,
                                Takeoff = std,
                                Landing = sta
                            };

                            try
                            {
                                flt.FlightH = Convert.ToInt32(hours);
                                flt.FlightM = Convert.ToByte(minutes);
                            }
                            catch
                            {
                                flt.FlightH = 0;
                                flt.FlightM = 0;
                            }
                            context.FlightInformations.Add(flt);

                        }

                    }
                    cnt++;
                }
                _date = _date.AddDays(1);
            }

            var reult = await context.SaveChangesAsync();
            var _res = new {
                message = cnt + " FLIGHT(S) INSERTED.",
                fromDate,
                toDate,
                WS = workbook.Worksheets.Count,
                dbids
            };

            return Ok(_res);
        }

        public class xlsflt
        {
            public string day { get; set; }
            public string reg { get; set; }
            public string route { get; set; }
            public DateTime? dep { get; set; }
            public DateTime? arr { get; set; }
            public string flightNumber { get; set; }
            public string origin { get; set; }
            public string destination { get; set; }
        }

        public class utcdiff
        {
            public string iata { get; set; }
            public int diff { get; set; }
        }

    }
}
