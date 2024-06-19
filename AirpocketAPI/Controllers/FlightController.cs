using AirpocketAPI.Models;
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
using Spire.Xls;
using System.Net.Http.Headers;
using System.Drawing;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Spire.Xls.Core;

namespace AirpocketAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FlightController : ApiController
    {



        //[Route("api/fmis/delay")]
        //[AcceptVerbs("GET")]
        //public IHttpActionResult GetFMISDelay(int id, string fn)
        //{
        //    var context = new AirpocketAPI.Models.FLYEntities();
        //    context.Configuration.AutoDetectChangesEnabled = false;
        //    context.Configuration.ValidateOnSaveEnabled = false;
        //    var delayXmls = context.ViewDelayXMLs.OrderBy(q=>q.Id).Skip(0).Take(5000).ToList();
        //    var flightIds = delayXmls.Select(q => q.FlightId).ToList();
        //    var codes = context.DelayCodes.ToList();
        //    var c99 = codes.Where(q => q.Code == "99").FirstOrDefault();
        //    int ccc = 0;
        //    foreach (var xd in delayXmls)
        //    {
        //        ccc++;
        //        //var text = "<Delay><Item No='1' DigitCode='93' LetterCode='__' Amount='00:05' Description=''/><Item No='2' DigitCode='81' LetterCode='__' Amount='00:05' Description=''/><Item No='3' DigitCode='99' LetterCode='__' Amount='00:10' Description=''/></Delay>";
        //        var text = xd.Delay;
        //        XmlDocument doc = new XmlDocument();
        //        doc.LoadXml(text);
        //        var json = JsonConvert.SerializeXmlNode(doc);
        //        json = json.Replace("@", "");
        //        xmlDelay obj = null;
        //        try
        //        {
        //            obj = JsonConvert.DeserializeObject<xmlDelay>(json);
        //        }
        //        catch(Exception ex)
        //        {
        //            var objs = JsonConvert.DeserializeObject<xmlDelaySingle>(json);
        //            obj = new xmlDelay();
        //            obj.Delay = new xmlDelayItemCol();
        //            obj.Delay.Item = new List<xmlDelayItem>();
        //            obj.Delay.Item.Add(objs.Delay.Item);
        //        }

        //        foreach(var item in obj.Delay.Item)
        //        {
        //            var xitem = new DelayXMLItem() { XmlId = xd.Id, FlightId = xd.FlightId, Description = item.Description, Code = item.DigitCode,Remark="" };
        //            var cid = codes.Where(q => q.Code == xitem.Code).FirstOrDefault();
        //            if (cid == null)
        //            { 
        //                xitem.Remark += "ERR1_";
        //                xitem.CodeId = c99.Id;
        //                xitem.Description += "  Error_Code_Not_Found_" + xitem.Code;
        //            }
        //            else
        //            {
        //                xitem.CodeId = cid.Id;
        //            }

        //            try
        //            {
        //                var hh =Convert.ToInt32( item.Amount.Split(':')[0]);
        //                xitem.HH = hh;
        //            }
        //            catch(Exception ex)
        //            {
        //                xitem.Remark += "ERR2_";
        //            }
        //            try
        //            {
        //                var mm = Convert.ToInt32(item.Amount.Split(':')[1]);
        //                xitem.MM = mm;
        //            }
        //            catch (Exception ex)
        //            {
        //                xitem.Remark += "ERR3_";
        //            }

        //            context.DelayXMLItems.Add(xitem);

        //        }

        //    }
        //    //context.SaveChanges();
        //    context.BulkSaveChanges();
        //    return Ok(true);

        //}

        //[Route("api/send/up")]
        //[AcceptVerbs("GET")]
        //public IHttpActionResult GetSendUserName()
        //{
        //    Magfa m = new Magfa();
        //    var smsResult = m.enqueue(1, user.PhoneNumber, "AirPocket" + "\n" + "Verification Code: " + verification)[0];
        //    return Ok(obj);

        //}
        [Route("api/flight/atc/update/{id}/{fn}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetATcUpdate(int id, string fn)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            try
            {
                var flt = context.FlightInformations.FirstOrDefault(q => q.ID == id);
                if (flt != null)
                    flt.ATCPlan = fn.Split('X')[0] + "." + fn.Split('X')[1]; //".pdf";
                context.SaveChanges();
                return Ok("done");
            }
            catch (Exception ex)
            {
                var msg = ex.Message + " IN:" + (ex.InnerException != null ? ex.InnerException.Message : "NO");
                return Ok(msg);
            }

        }

        [Route("api/flight/atl/update/{id}/{fn}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetATLUpdate(int id, string fn)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            try
            {
                var flt = context.FlightInformations.FirstOrDefault(q => q.ID == id);
                if (flt != null)
                    flt.ATL = fn.Split('X')[0] + ".pdf";
                context.SaveChanges();
                return Ok("done");
            }
            catch (Exception ex)
            {
                var msg = ex.Message + " IN:" + (ex.InnerException != null ? ex.InnerException.Message : "NO");
                return Ok(msg);
            }

        }


        [Route("api/flight/daily")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsDaily(DateTime df, DateTime dt, string regs, string routes, string from, string to, string no
            , string status
            , string type2
            , string idx
            , string chr)
        {
            var cmd = "select * from viewflightdaily ";
            try
            {
                var context = new AirpocketAPI.Models.FLYEntities();
                // var cmd = "select * from viewflightdaily ";
                // string whr = " (STDDayLocal>='" + df.ToString("yyyy-MM-dd") + "' and STDDayLocal<='" + dt.ToString("yyyy-MM-dd") + "')";
                string whr = "  (TAKEOFFDAYLOCAL>='" + df.ToString("yyyy-MM-dd") + "' and TAKEOFFDAYLOCAL<='" + dt.ToString("yyyy-MM-dd") + "')";
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
                    var col = _regs.Select(q => "FlightType2='" + q + "'").ToList();
                    var _whr = "(" + string.Join(" OR ", col) + ")";
                    whr += " AND " + _whr;
                }

                if (!string.IsNullOrEmpty(idx) && idx != "-1")
                {
                    var _regs = idx.Split('_').ToList();
                    var col = _regs.Select(q => "FlightIndex='" + q + "'").ToList();
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

                cmd = cmd + " WHERE " + whr + " ORDER BY STD,Register";

                var flts = context.ViewFlightDailies
                            .SqlQuery(cmd)
                            .ToList<ViewFlightDaily>();

                //var result = await courseService.GetEmployeeCertificates(id);

                return Ok(flts);
            }
            catch (Exception ex)
            {
                return Ok(cmd);
            }

        }

        //09-09
        [Route("api/flight/daily/export/old")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage _GetFlightsDailyExport(DateTime df, DateTime dt, string regs, string routes, string from, string to, string no, string status
            , string type2
            , string idx
            , string chr
            , string time
            , string fuel
            , string weight)
        {

            var context = new AirpocketAPI.Models.FLYEntities();
            var cmd = "select * from viewflightdaily ";
            string whr = "FlightStatusId<>4 and (STDDayLocal>='" + df.ToString("yyyy-MM-dd") + "' and STDDayLocal<='" + dt.ToString("yyyy-MM-dd") + "')";

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

            var flts = context.ViewFlightDailies
                        .SqlQuery(cmd)
                        .ToList<ViewFlightDaily>();

            //var result = await courseService.GetEmployeeCertificates(id);

            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);
            Workbook workbook = new Workbook();
            //1
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "fltdaily" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[6, 20].Text = weight == "kg" ? "بار حمل شده کارگو (کیلوگرم)" : "بار حمل شده کارگو (پوند)";
            sheet.Range[6, 19].Text = weight == "kg" ? "بار حمل شده (کیلوگرم)" : "بار حمل شده (پوند)";
            sheet.Range[6, 35].Text = "سوخت (لیتر)";
            if (fuel == "lbs")
            {

                sheet.Range[6, 35].Text = "سوخت (پوند)";
            }

            if (fuel == "kg")
            {

                sheet.Range[6, 35].Text = "سوخت (کیلوگرم)";
            }

            var ln = 8;
            foreach (var flt in flts)
            {
                sheet.InsertRow(ln);
                sheet.Range[ln, 1].Text = flt.PMonthName;
                sheet.Range[ln, 2].Text = flt.FlightType2;
                sheet.Range[ln, 3].Text = flt.FlightIndex;
                sheet.Range[ln, 4].Text = flt.PDayName;
                sheet.Range[ln, 5].Value2 = flt.STDDay;
                sheet.Range[ln, 5].NumberFormat = "yyyy-mm-dd";
                sheet.Range[ln, 6].Text = flt.PDate;

                sheet.Range[ln, 7].Text = flt.FlightNumber;
                sheet.Range[ln, 8].Text = flt.AircraftType;
                sheet.Range[ln, 9].Text = flt.Register;
                sheet.Range[ln, 10].Text = flt.FromAirportIATA;
                sheet.Range[ln, 11].Text = flt.ToAirportIATA;

                sheet.Range[ln, 12].Value2 = flt.PaxAdult;
                sheet.Range[ln, 12].NumberFormat = "0";
                sheet.Range[ln, 13].Value2 = flt.PaxChild;
                sheet.Range[ln, 13].NumberFormat = "0";
                sheet.Range[ln, 14].Value2 = flt.PaxInfant;
                sheet.Range[ln, 14].NumberFormat = "0";
                sheet.Range[ln, 15].Formula = "=SUM(Sheet2!$L$" + ln + ":M$" + ln + ")";
                // sheet.Range[ln, 15].Value2 = flt.RevPax;
                sheet.Range[ln, 15].NumberFormat = "0";
                //  sheet.Range[ln, 16].Value2 = flt.TotalPax;
                sheet.Range[ln, 16].Formula = "=SUM(Sheet2!$L$" + ln + ":N$" + ln + ")";
                sheet.Range[ln, 16].NumberFormat = "0";

                sheet.Range[ln, 17].Value2 = null;
                sheet.Range[ln, 17].NumberFormat = "0";
                sheet.Range[ln, 18].Value2 = null;
                sheet.Range[ln, 18].NumberFormat = "0";



                sheet.Range[ln, 19].Value2 = weight == "kg" ? flt.BaggageWeightKg : flt.BaggageWeightLbs;
                sheet.Range[ln, 19].NumberFormat = "0.0";


                sheet.Range[ln, 20].Value2 = weight == "kg" ? flt.CargoWeightKg : flt.CargoWeightLbs;
                sheet.Range[ln, 20].NumberFormat = "0.0";

                sheet.Range[ln, 21].Value2 = flt.TotalSeat;
                sheet.Range[ln, 21].NumberFormat = "0";

                sheet.Range[ln, 22].Value2 = flt.EmptySeat;
                sheet.Range[ln, 22].NumberFormat = "0";


                sheet.Range[ln, 23].Value2 = null;
                sheet.Range[ln, 23].NumberFormat = "0";

                var _std = time == "lcl" ? ((DateTime)flt.STDLocal) : ((DateTime)flt.STD);
                var _stdHour = _std.Hour; //(_std.Hour > 12 ? _std.Hour - 12 : _std.Hour);
                var _sta = time == "lcl" ? ((DateTime)flt.STALocal) : ((DateTime)flt.STA);
                var _staHour = _sta.Hour; //(_sta.Hour > 12 ? _sta.Hour - 12 : _sta.Hour);

                sheet.Range[ln, 24].Cells[0].Text = _stdHour.ToString().PadLeft(2, '0') + ":" + _std.Minute.ToString().PadLeft(2, '0'); /*+ (_std.Hour<12?" AM":" PM")*/; //flt.STDLocal;
                sheet.Range[ln, 25].Cells[0].Text = _staHour.ToString().PadLeft(2, '0') + ":" + _sta.Minute.ToString().PadLeft(2, '0');   /*+(_sta.Hour < 12 ? " AM" : " PM")*/; //flt.STALocal;
                                                                                                                                                                                 // sheet.Range[ln, 26].Formula ="IF(Y"+ln+">=X"+ln+ ",Y" + ln + "-" + "X" + ln+",(Y"+ln+"+1)-X"+ln+")"; //"Sheet2!$Y"+ln;
                                                                                                                                                                                 //sheet.Range[ln, 26].Formula ="Y"+ln+"-"+"X"+ln;
                sheet.Range[ln, 24].NumberFormat = "HH:mm";
                sheet.Range[ln, 25].NumberFormat = "HH:mm";
                sheet.Range[ln, 26].NumberFormat = "HH:mm";

                sheet.Range[ln, 26].Formula = "IF(Y" + ln + "+0>=X" + ln + "+0,Y" + ln + "-" + "X" + ln + ",Y" + ln + "-X" + ln + "+1)"; //"Sheet2!$Y"+ln;
                //IF(Y14>=X14,Y14-X14,(Y14+1)-X14)
                //"=SUM(Sheet2!$L$"+ln+":M$" + ln + ")"; 
                // sheet.Range[ln, 26].TimeSpanValue = flt.ScheduledTime==null? TimeSpan.FromMinutes(0):  TimeSpan.FromMinutes(Convert.ToDouble(flt.ScheduledTime));

                //sheet.Range[ln, 26].Text =toHHMM( flt.ScheduledTime);

                var _takeoff = time == "lcl" ? ((DateTime)flt.TakeOffLocal) : ((DateTime)flt.TakeOff);
                var _landing = time == "lcl" ? ((DateTime)flt.LandingLocal) : ((DateTime)flt.Landing);
                sheet.Range[ln, 27].Text = _takeoff.Hour.ToString().PadLeft(2, '0') + ":" + _takeoff.Minute.ToString().PadLeft(2, '0');
                sheet.Range[ln, 28].Text = _landing.Hour.ToString().PadLeft(2, '0') + ":" + _landing.Minute.ToString().PadLeft(2, '0');
                sheet.Range[ln, 29].Formula = "IF(AB" + ln + "+0>=AA" + ln + "+0,AB" + ln + "-" + "AA" + ln + ",AB" + ln + "-AA" + ln + "+1)";
                sheet.Range[ln, 27].NumberFormat = "HH:mm";
                sheet.Range[ln, 28].NumberFormat = "HH:mm";
                sheet.Range[ln, 29].NumberFormat = "HH:mm";
                // sheet.Range[ln, 27].Value2 = flt.TakeOffLocal;
                // sheet.Range[ln, 28].Value2 = flt.LandingLocal;
                // sheet.Range[ln, 29].TimeSpanValue = flt.FlightTime == null ? TimeSpan.FromMinutes(0) : TimeSpan.FromMinutes(Convert.ToDouble(flt.FlightTime));

                var _offblock = time == "lcl" ? ((DateTime)flt.BlockOffLocal) : ((DateTime)flt.BlockOff);
                var _onblock = time == "lcl" ? ((DateTime)flt.BlockOnLocal) : ((DateTime)flt.BlockOn);
                sheet.Range[ln, 30].Text = _offblock.Hour.ToString().PadLeft(2, '0') + ":" + _offblock.Minute.ToString().PadLeft(2, '0');
                sheet.Range[ln, 31].Text = _onblock.Hour.ToString().PadLeft(2, '0') + ":" + _onblock.Minute.ToString().PadLeft(2, '0');
                sheet.Range[ln, 32].Formula = "IF(AE" + ln + "+0>=AD" + ln + "+0,AE" + ln + "-" + "AD" + ln + ",AE" + ln + "-AD" + ln + "+1)";
                sheet.Range[ln, 30].NumberFormat = "HH:mm";
                sheet.Range[ln, 31].NumberFormat = "HH:mm";
                sheet.Range[ln, 32].NumberFormat = "HH:mm";

                //if (flt.DelayOffBlock <= 0)
                //    sheet.Range[ln, 33].TimeSpanValue = TimeSpan.FromMinutes(0);
                //else
                //    sheet.Range[ln, 33].TimeSpanValue = flt.DelayOffBlock == null ? TimeSpan.FromMinutes(0) : TimeSpan.FromMinutes(Convert.ToDouble(flt.DelayOffBlock));
                sheet.Range[ln, 33].Formula = "IF(AD" + ln + "+0>=X" + ln + "+0,AD" + ln + "-X" + ln + ",0)";//"IF(AE" + ln + "+0>=AD" + ln + "+0,AE" + ln + "-" + "AD" + ln + ",AE" + ln + "-AD" + ln + "+1)";
                sheet.Range[ln, 33].NumberFormat = "HH:mm";

                //DelayReason
                sheet.Range[ln, 34].Value2 = flt.DelayReason;
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


                sheet.Range[ln, 35].Value2 = _fuel;
                sheet.Range[ln, 35].NumberFormat = "0.0";
                //Distance
                sheet.Range[ln, 36].Value2 = flt.Distance;
                sheet.Range[ln, 36].NumberFormat = "0";
                //ChrTitle
                sheet.Range[ln, 37].Value2 = flt.ChrTitle;
                //StationIncome
                sheet.Range[ln, 38].Value2 = flt.StationIncome;
                sheet.Range[ln, 38].NumberFormat = "0";
                //--agancies

                //Remark
                sheet.Range[ln, 53].Value2 = flt.TotalRemark;
                sheet.Range[ln, 54].Value2 = flt.FlightStatus;

                //sheet.Range[r, 11].Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                //sheet.Rows[ln-1].Borders.LineStyle = LineStyleType.Thin;
                sheet.Rows[ln - 1].BorderInside(LineStyleType.Double);
                sheet.Rows[ln - 1].BorderAround(LineStyleType.Double);
                ln++;
            }

            sheet.InsertRow(ln);
            var frm_paxadult = "=SUM(Sheet2!$L$8:L$" + (ln - 1) + ")";
            sheet.Range[ln, 12].Formula = frm_paxadult;

            var frm_paxchild = "=SUM(Sheet2!$M$8:M$" + (ln - 1) + ")";
            sheet.Range[ln, 13].Formula = frm_paxchild;

            var frm_paxinfant = "=SUM(Sheet2!$N$8:N$" + (ln - 1) + ")";
            sheet.Range[ln, 14].Formula = frm_paxinfant;

            sheet.Range[ln, 15].Formula = "=SUM(Sheet2!$O$8:O$" + (ln - 1) + ")";
            sheet.Range[ln, 16].Formula = "=SUM(Sheet2!$P$8:P$" + (ln - 1) + ")";
            sheet.Range[ln, 17].Formula = "=SUM(Sheet2!$Q$8:Q$" + (ln - 1) + ")";
            sheet.Range[ln, 18].Formula = "=SUM(Sheet2!$R$8:R$" + (ln - 1) + ")";
            sheet.Range[ln, 19].Formula = "=SUM(Sheet2!$S$8:S$" + (ln - 1) + ")";
            sheet.Range[ln, 20].Formula = "=SUM(Sheet2!$T$8:T$" + (ln - 1) + ")";

            //fixtime
            //Spire.Xls.CellFormatType.Unknown
            sheet.Range[ln, 26].Formula = "=SUM(Sheet2!$Z$8:Z$" + (ln - 1) + ")";
            sheet.Range[ln, 26].Style.NumberFormat = "[h]:mm;@";// .NumberFormat = "[h]:mm;@";

            //  sheet.Range[ln, 26].Style.NumberFormatSettings
            //sheet.Range[ln, 26].DateTimeValue = System.DateTime.Today;



            sheet.Range[ln, 29].Formula = "=SUM(Sheet2!$AC$8:AC$" + (ln - 1) + ")";
            sheet.Range[ln, 29].Style.NumberFormat = "[h]:mm;@";
            sheet.Range[ln, 32].Formula = "=SUM(Sheet2!$AF$8:AF$" + (ln - 1) + ")";
            sheet.Range[ln, 32].Style.NumberFormat = "[h]:mm;@";

            sheet.Range[ln, 33].Formula = "=SUM(Sheet2!$AG$8:AG$" + (ln - 1) + ")";
            sheet.Range[ln, 33].Style.NumberFormat = "[h]:mm;@";

            sheet.Range[ln, 35].Formula = "=SUM(Sheet2!$AI$8:AI$" + (ln - 1) + ")";
            sheet.Range[ln, 36].Formula = "=SUM(Sheet2!$AJ$8:AJ$" + (ln - 1) + ")";

            sheet.Range[ln, 38].Formula = "=SUM(Sheet2!$AL$8:AL$" + (ln - 1) + ")";



            sheet.Rows[ln - 1].BorderInside(LineStyleType.Double);
            sheet.Rows[ln - 1].BorderAround(LineStyleType.Double);


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


        [Route("api/flight/daily/export")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetFlightsDailyExport(DateTime df, DateTime dt, string regs, string routes, string from, string to, string no, string status
             , string type2
             , string idx
             , string chr
             , string time
             , string fuel
             , string weight)
        {

            var context = new AirpocketAPI.Models.FLYEntities();
            var cmd = "select * from viewflightdaily ";
            string whr = "FlightStatusId<>4 and (STDDayLocal>='" + df.ToString("yyyy-MM-dd") + "' and STDDayLocal<='" + dt.ToString("yyyy-MM-dd") + "')";

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

            var flts = context.ViewFlightDailies
                        .SqlQuery(cmd)
                        .ToList<ViewFlightDaily>();

            //var result = await courseService.GetEmployeeCertificates(id);

            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);
            Workbook workbook = new Workbook();
            //1
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "fltdaily" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[6, 21].Text = weight == "kg" ? "بار حمل شده کارگو (کیلوگرم)" : "بار حمل شده کارگو (پوند)";
            sheet.Range[6, 20].Text = weight == "kg" ? "بار حمل شده (کیلوگرم)" : "بار حمل شده (پوند)";
            sheet.Range[6, 36].Text = "سوخت (لیتر)";
            if (fuel == "lbs")
            {

                sheet.Range[6, 36].Text = "سوخت (پوند)";
            }

            if (fuel == "kg")
            {

                sheet.Range[6, 36].Text = "سوخت (کیلوگرم)";
            }

            var ln = 8;
            foreach (var flt in flts)
            {
                sheet.InsertRow(ln);
                sheet.Range[ln, 1].Text = flt.PMonthName;
                sheet.Range[ln, 2].Text = flt.FlightType2;
                sheet.Range[ln, 3].Text = flt.FlightIndex;
                sheet.Range[ln, 4].Value2 = flt.FlightStatus;
                sheet.Range[ln, 5].Text = flt.PDayName;
                sheet.Range[ln, 6].Value2 = flt.STDDay;
                sheet.Range[ln, 6].NumberFormat = "yyyy-mm-dd";
                sheet.Range[ln, 7].Text = flt.PDate;

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

                sheet.Range[ln, 34].Formula = "IF(AE" + ln + ">=Y" + ln + ",IF(AE" + ln + "==Y" + ln + ",0,AE" + ln + "-Y" + ln + "),AE" + ln + "-Y" + ln + "+1)";
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

        string toHHMM(int? mm)
        {
            if (mm == null || mm <= 0)
                return "00:00";
            TimeSpan ts = TimeSpan.FromMinutes(Convert.ToDouble(mm));
            var result = ts.Hours.ToString().PadLeft(2, '0') + ":" + ts.Minutes.ToString().PadLeft(2, '0');
            return result;

        }

        [Route("api/xls/airport/daily")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLS(DateTime dt, string origin)
        {

            var _dt = dt.Date;

            var context = new AirpocketAPI.Models.FLYEntities();
            var query = (from x in context.RptAirportDailies
                         where x.STDDay == _dt && (x.FromAirportIATA == origin || x.ToAirportIATA == origin)
                         orderby x.STD
                         select x).ToList();


            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);

            List<string> clmns = new List<string>() { "ردیف", "بهره بردار", "شماره پرواز", "هواپیما", "تاریخ برنامه ای", "زمان برنامه ای"
            ,"مبدا"
            ,"مقصد"
            ,"تاریخ واقعی"
            ,"زمان واقعی"
            ,"مسافر بزرگسال"
            ,"مسافر خردسال"
            ,"مسافر نوزاد"
            ,"بار هوایی"
            ,"تاریخ تاکسی"
            ,"زمان تاکسی"
            ,"علت تاخیر"
            ,"سوخت"
            ,"پست هوایی"
            ,"مسافر ترانزیت"
            ,"مسافر ترانسفر"
            ,"بار ترانزیت"
            ,"بار ترانسفر"
            };

            Workbook workbook = new Workbook();

            workbook.Worksheets.Clear();
            var sheetName = ((DateTime)_dt).ToString("dddd dd-MMM-yyyy");
            Worksheet sheet = workbook.Worksheets.Add(sheetName);
            var _rng = "A1:W" + (query.Count + 1).ToString();



            int c = 1;
            foreach (var clmn in clmns)
            {
                sheet.Range[1, c].Value = clmn;
                c++;
            }


            int r = 2;

            foreach (var flt in query)
            {
                // List<string> clmns = new List<string>() {"Date","PDate","Day","Flight No","Status","Reg" };
                sheet.Range[r, 1].Value2 = (r - 1).ToString();
                sheet.Range[r, 2].Value2 = flt.Airline;
                sheet.Range[r, 3].Value2 = flt.FlightNumber;
                sheet.Range[r, 4].Value2 = "EP-" + flt.Register;
                sheet.Range[r, 5].Text = flt.PDate;
                sheet.Range[r, 5].Clear(ExcelClearOptions.ClearFormat);


                sheet.Range[r, 6].Value2 = ((DateTime)flt.STDLocal).ToString("HHmm");
                sheet.Range[r, 7].Value2 = flt.FromAirportIATA;
                sheet.Range[r, 8].Value2 = flt.ToAirportIATA;

                sheet.Range[r, 9].Text = flt.PDateTakeOff;
                sheet.Range[r, 9].Clear(ExcelClearOptions.ClearFormat);

                sheet.Range[r, 10].Value2 = ((DateTime)flt.TakeOffLocal).ToString("HHmm");
                sheet.Range[r, 11].Value2 = flt.PaxAdult;
                sheet.Range[r, 12].Value2 = flt.PaxChild;
                sheet.Range[r, 13].Value2 = flt.PaxInfant;
                sheet.Range[r, 14].Value2 = flt.Freight;

                sheet.Range[r, 15].Text = flt.PDateOffBlock;
                sheet.Range[r, 15].Clear(ExcelClearOptions.ClearFormat);

                sheet.Range[r, 16].Value2 = ((DateTime)flt.OffBlockLocal).ToString("HHmm");
                sheet.Range[r, 17].Value2 = flt.Delays;
                sheet.Range[r, 18].Value2 = flt.Uplift;
                sheet.Range[r, 19].Value2 = "****";
                sheet.Range[r, 20].Value2 = "****";
                sheet.Range[r, 21].Value2 = "****";
                sheet.Range[r, 22].Value2 = "****";
                sheet.Range[r, 23].Value2 = "****";
                r++;
            }


            sheet.Range[_rng].BorderInside(LineStyleType.Thin, Color.Black);
            sheet.Range[_rng].BorderAround(LineStyleType.Medium, Color.Black);

            sheet.Range["A1:W1"].Style.Color = Color.MediumSpringGreen;
            sheet.Range["A1:A" + (query.Count + 1).ToString()].Style.Color = Color.MediumSpringGreen;
            sheet.Range["A1:A" + (query.Count + 1).ToString()].AutoFitColumns();
            sheet.Range["A1:W1"].Style.Font.IsBold = true;
            sheet.Range["A1:W100"].Style.HorizontalAlignment = HorizontalAlignType.Center;
            sheet.Range["A1:W100"].Style.VerticalAlignment = VerticalAlignType.Center;


            var name = "Airport_Daily_Report_" + _dt.ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");

            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);


            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            //  Stream strm = new MemoryStream();
            // workbook.SaveToStream(strm);

            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");



            return response;
        }
        //05-20
        [Route("api/xls/flights")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> UploadFlights(string fn)
        {
            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);
            if (!fn.ToLower().Contains(".xls"))
                fn += ".xlsx";
            var filePath = HttpContext.Current.Server.MapPath("~/upload/" + fn);
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(filePath);
            Worksheet sheet = workbook.Worksheets[0];
            DateTime df;
            DateTime dt;
            var _df = sheet.Range[1, 8].Value2;
            var _dt = sheet.Range[1, 9].Value2;
            try
            {
                df = Convert.ToDateTime(_df);
                dt = Convert.ToDateTime(_dt);
            }
            catch (Exception ex)
            {
                return Ok("ERROR- WRONGE DATES");
            }
            var days = new List<string>() { "SAT", "SUN", "MON", "TUE", "WED", "THU", "FRI" };
            var c = 4;
            var emptyRoute = 0;
            var doLoop = true;
            var lastDay = "SAT";
            var xlsFlts = new List<xlsflt>();
            while (doLoop)
            {
                var day = !string.IsNullOrEmpty(sheet.Range[c, 2].Text) ? sheet.Range[c, 2].Text.Replace(" ", "") : "";

                if (string.IsNullOrEmpty(day))
                    day = lastDay;
                if (days.Contains(day.ToUpper()))
                {
                    var route = !string.IsNullOrEmpty(sheet.Range[c, 3].Text) ? sheet.Range[c, 3].Text.Replace(" ", "") : "";
                    if (string.IsNullOrEmpty(route))
                        emptyRoute++;
                    else
                        emptyRoute = 0;
                    if (emptyRoute > 3)
                        doLoop = false;
                    if (!string.IsNullOrEmpty(route))
                    {
                        var flightNumber = sheet.Range[c, 4].Value2.ToString();

                        var _dep = sheet.Range[c, 5].DateTimeValue;
                        var _arr = sheet.Range[c, 6].DateTimeValue;
                        var reg = sheet.Range[c, 7].Text;

                        xlsFlts.Add(new xlsflt() { day = day, arr = _arr, dep = _dep, route = route, flightNumber = flightNumber, reg = reg });

                    }

                    if (!string.IsNullOrEmpty(day))
                        lastDay = day;
                }

                c++;
            }
            var context = new AirpocketAPI.Models.FLYEntities();
            var dblegs = await context.ViewLegTimes.Where(q => q.STDLocal >= df && q.STDLocal <= dt).ToListAsync();
            var dbids = dblegs.Select(q => q.ID).ToList();
            var dbflights = await context.FlightInformations.Where(q => dbids.Contains(q.ID)).ToListAsync();
            var airports = await context.Airports.ToListAsync();
            var registers = await context.Ac_MSN.ToListAsync();
            var _date = df;
            var cnt = 0;
            while (_date <= dt)
            {
                var _day = _date.ToString("ddd").ToUpper();
                var dbs = dblegs.Where(q => q.STDLocal >= _date && q.STDLocal < _date.AddDays(1)).ToList();
                var xls = xlsFlts.Where(q => q.day == _day).Where(q => !string.IsNullOrEmpty(q.route)).ToList();
                foreach (var x in xls)
                {
                    var exist = dbs.FirstOrDefault(q => q.FlightNumber == x.flightNumber);
                    if (exist == null)
                    {
                        var fromIATA = airports.Where(q => q.IATA == x.from).FirstOrDefault();
                        var toIATA = airports.Where(q => q.IATA == x.to).FirstOrDefault();
                        var register = registers.Where(q => q.Register == x.reg).FirstOrDefault();
                        if (fromIATA != null && toIATA != null && register != null)
                        {
                            var _dep = (DateTime)x.dep;
                            var _arr = (DateTime)x.arr;
                            var stdlocal = new DateTime(_date.Year, _date.Month, _date.Day, _dep.Hour, _dep.Minute, 0);
                            var tzoffsetstd = -1 * TimeZoneInfo.Local.GetUtcOffset(stdlocal).TotalMinutes;
                            var stalocal = new DateTime(_date.Year, _date.Month, _date.Day, _arr.Hour, _arr.Minute, 0);
                            if (stalocal < stdlocal)
                                stalocal = stalocal.AddDays(1);
                            var tzoffsetsta = -1 * TimeZoneInfo.Local.GetUtcOffset(stalocal).TotalMinutes;

                            var std = stdlocal.AddMinutes(tzoffsetstd);
                            var sta = stalocal.AddMinutes(tzoffsetsta);
                            var submision = stalocal - stdlocal;
                            double totalMin = submision.TotalMinutes;
                            double hours = (totalMin - totalMin % 60) / 60;
                            double minute = totalMin - hours * 60;
                            var flt = new FlightInformation()
                            {
                                RegisterID = register.ID,
                                FlightTypeID = 9,
                                FlightStatusID = 1,
                                FlightNumber = x.flightNumber,
                                FromAirportId = fromIATA.Id,
                                ToAirportId = toIATA.Id,
                                STD = std,
                                STA = sta,
                                ChocksOut = std,
                                ChocksIn = sta,
                                Takeoff = std,
                                Landing = sta,

                                CustomerId = 4,

                            };
                            try
                            {
                                flt.FlightH = Convert.ToInt32(hours);
                                flt.FlightM = Convert.ToByte(minute);
                            }
                            catch
                            {
                                flt.FlightH = 0;
                                flt.FlightM = 0;
                            }
                            cnt++;
                            context.FlightInformations.Add(flt);


                        }
                    }
                }
                var reult = await context.SaveChangesAsync();
                _date = _date.AddDays(1);
            }




            return Ok(cnt + " FLIGHT(S) INSERTED.");
        }

        [Route("api/xls/flights2")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> UploadFlights2(string fn)
        {
            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);
            if (!fn.ToLower().Contains(".xls"))
                fn += ".xlsx";
            var filePath = HttpContext.Current.Server.MapPath("~/upload/" + fn);
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(filePath);
            Worksheet sheet = workbook.Worksheets[0];
            DateTime df;
            DateTime dt;
            var _df = sheet.Range[1, 9].Value2;
            var _dt = sheet.Range[1, 10].Value2;
            try
            {
                df = Convert.ToDateTime(_df);
                dt = Convert.ToDateTime(_dt);
            }
            catch (Exception ex)
            {
                return Ok("ERROR- WRONGE DATES");
            }
            var days = new List<string>() { "SAT", "SUN", "MON", "TUE", "WED", "THU", "FRI" };
            var c = 4;
            var emptyRoute = 0;
            var doLoop = true;
            var lastDay = "SAT";
            var xlsFlts = new List<xlsflt>();
            while (doLoop)
            {
                var day = !string.IsNullOrEmpty(sheet.Range[c, 2].Text) ? sheet.Range[c, 2].Text.Replace(" ", "") : "";

                if (string.IsNullOrEmpty(day))
                    day = lastDay;
                if (days.Contains(day.ToUpper()))
                {
                    // var route = !string.IsNullOrEmpty(sheet.Range[c, 3].Text) ? sheet.Range[c, 3].Text.Replace(" ", "") : "";
                    var origin = !string.IsNullOrEmpty(sheet.Range[c, 3].Text) ? sheet.Range[c, 3].Text.Replace(" ", "") : "";
                    var destination = !string.IsNullOrEmpty(sheet.Range[c, 4].Text) ? sheet.Range[c, 4].Text.Replace(" ", "") : "";
                    if (string.IsNullOrEmpty(origin))
                        emptyRoute++;
                    else
                        emptyRoute = 0;
                    if (emptyRoute > 3)
                        doLoop = false;
                    if (!string.IsNullOrEmpty(origin) && !string.IsNullOrEmpty(destination))
                    {
                        var flightNumber = sheet.Range[c, 5].Value2.ToString();

                        var _dep = sheet.Range[c, 6].DateTimeValue;
                        var _arr = sheet.Range[c, 7].DateTimeValue;
                        var reg = sheet.Range[c, 8].Text;

                        xlsFlts.Add(new xlsflt() { day = day, arr = _arr, dep = _dep, route = origin + "-" + destination, origin = origin, destination = destination, flightNumber = flightNumber, reg = reg });

                    }

                    if (!string.IsNullOrEmpty(day))
                        lastDay = day;
                }

                c++;
            }
            var context = new AirpocketAPI.Models.FLYEntities();
            var dblegs = await context.ViewLegTimes.Where(q => q.STDLocal >= df && q.STDLocal <= dt).ToListAsync();
            var dbids = dblegs.Select(q => q.ID).ToList();
            var dbflights = await context.FlightInformations.Where(q => dbids.Contains(q.ID)).ToListAsync();
            var airports = await context.Airports.ToListAsync();
            var registers = await context.Ac_MSN.ToListAsync();
            var _date = df;
            var cnt = 0;
            while (_date <= dt)
            {
                var _day = _date.ToString("ddd").ToUpper();
                var dbs = dblegs.Where(q => q.STDLocal >= _date && q.STDLocal < _date.AddDays(1)).ToList();
                var xls = xlsFlts.Where(q => q.day == _day).Where(q => !string.IsNullOrEmpty(q.route)).ToList();
                foreach (var x in xls)
                {
                    var exist = dbs.FirstOrDefault(q => q.FlightNumber == x.flightNumber);
                    if (exist == null)
                    {
                        var fromIATA = airports.Where(q => q.IATA == x.from).FirstOrDefault();
                        var toIATA = airports.Where(q => q.IATA == x.to).FirstOrDefault();
                        var register = registers.Where(q => q.Register == x.reg).FirstOrDefault();
                        if (fromIATA != null && toIATA != null && register != null)
                        {
                            var _dep = (DateTime)x.dep;
                            var _arr = (DateTime)x.arr;
                            var stdlocal = new DateTime(_date.Year, _date.Month, _date.Day, _dep.Hour, _dep.Minute, 0);
                            var tzoffsetstd = -1 * TimeZoneInfo.Local.GetUtcOffset(stdlocal).TotalMinutes;
                            var stalocal = new DateTime(_date.Year, _date.Month, _date.Day, _arr.Hour, _arr.Minute, 0);
                            if (stalocal < stdlocal)
                                stalocal = stalocal.AddDays(1);
                            var tzoffsetsta = -1 * TimeZoneInfo.Local.GetUtcOffset(stalocal).TotalMinutes;

                            var std = stdlocal.AddMinutes(tzoffsetstd);
                            var sta = stalocal.AddMinutes(tzoffsetsta);
                            var submision = stalocal - stdlocal;
                            double totalMin = submision.TotalMinutes;
                            double hours = (totalMin - totalMin % 60) / 60;
                            double minute = totalMin - hours * 60;
                            var flt = new FlightInformation()
                            {
                                RegisterID = register.ID,
                                FlightTypeID = 9,
                                FlightStatusID = 1,
                                FlightNumber = x.flightNumber,
                                FromAirportId = fromIATA.Id,
                                ToAirportId = toIATA.Id,
                                STD = std,
                                STA = sta,
                                ChocksOut = std,
                                ChocksIn = sta,
                                Takeoff = std,
                                Landing = sta,

                                CustomerId = 4,

                            };
                            try
                            {
                                flt.FlightH = Convert.ToInt32(hours);
                                flt.FlightM = Convert.ToByte(minute);
                            }
                            catch
                            {
                                flt.FlightH = 0;
                                flt.FlightM = 0;
                            }
                            cnt++;
                            context.FlightInformations.Add(flt);


                        }
                    }
                }
                var reult = await context.SaveChangesAsync();
                _date = _date.AddDays(1);
            }




            return Ok(cnt + " FLIGHT(S) INSERTED.");
        }

        public class utcdiff
        {
            public string iata { get; set; }
            public int diff { get; set; }
        }
        //varesh
        [Route("api/xls/flights3")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> UploadFlights3(string fn)
        {
            var utcs = new List<utcdiff>()
            {
                 new utcdiff(){ iata="NJF",diff=180},
                 new utcdiff(){ iata="DYU",diff=300},
            };
            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8TGljZW5zZSBLZXk9ImNyOERaN1hKMkR5MUs2UUJBTkRPSVRLdlpjTzZkelVod2lsSHBnVlluQ3k0cXlHV2V6TFZubFJGeFAxNU1mSWZnUmdNWm1XaEdOQWRFNFRqZWZnQ1ovbFR2b1BkSXRIbDZXdDVBNWk1TVhFbnFkQnVPMUthRnovRFFzYUdWTGhzdjlySG1ybnRxSElFRGxJeGRxYUpNcGtLb0Frd1A3d1N6T01KMVkrbUNmVTVVRmV6REwvTjd1enJ4M0Y0d2I1SGErd0E2VFQ5VFJ3SzAzejlFS01aRmwzU1lSL3o0YVU3TE0wZFNYWTlqU0ZKZ2dqZlZzRFVLaUJyVm5td1ljaXVyOUVrYmw5Q3RaWTAzdG1yZm01QlplKzZnaHRFTm4wb2gzMzh0WlJleWpjcjc0QWs3MWhnWWtuTE9CQzE1VllmalhzcXVBVW13MlI2TWNWMlBPT2JyY1RSYlhBZ3pvUWJPeWQ4U2JFWmN3aE43NktQd1dzUVFTMUowdGlZSFVLeE9tMnQ0ZkJWMGhQVmhhOUI4Y0swNHFKUVp0MDBaMWNKRGEwd2I4VWx6RWs5QkhVVzJlbk9mVDE0UnlIQ2krWUdlbVBLY2RDUXJoMXpyWVRGN0ltb0x4N3h1NGV2RFRZc2xzV0JrbFFJb3g4NnJWckVVa1N0dXErQUNTWS9xVTM5L1Zhd3Y5S0FmUjVUZUVicGt3RGhTYjBOQkFqVDhBeXRsRFZkR2ZpZzBxS0czVllpVHBYRnc1cHRMVmgrYmtkK2RnN3Z4dHZyNDVaVVdKZXlyekdOR0g3YUZZZDZwLzJNRy9YSlRsR3ovU05RbzJDUExraU83SlhuOU5HZXhaN3BIbTBkZ3pNWmJHRVhxVmR2bG04MTJhL1hMMVNxeEdVWStvNVpsVUM3WTV4Z2dhRCtGZVA5enpoeUpxSUVwcDk3My9ScTRteG1wQWZMcVNzTzJSeHlTcStpdjFDc3AwQ3JvMDc4OEhybDFteWt4dVQweWRSWVpDNkRTeDhNMi9MWTNkOXNud3U3NkFmYjVDOVF1ZE9Zc0wzREh2aGZncmNVSWUvcUhmVFo5QWF6Y3pUanlyM2RPQkFjczBLZk12Y0xVUzRSeHZDdW1NNDVyNDJnMXJ3UGluN2JBcmYvZnNMTzZtS3g0WWRoSURNWlF6V3RjbkhFSTF5TXJ6aU9pdXhMdE8xalRBV25uU2VLVDJ0cXI3Tm42Qmg5TURHNjZZK2lJaW4xV05TUCtMdDFYdXRkajNKTyt4b1FNUVB5ZFpoZkJYZXpVMEhRMnd0eEdwdzRNczRTMTVJbFg1TEdXR3dXeUdYTWNjVWd3b1RDeFRGYmgyZFo0Vkg3OVZHTEVFR1JRWEZrNTRBdlFLdFBpdUcxY0w4RFo3WEoyRHkxSzZUUWVORE9YeFl2NFNveitCMHNBS0VwTVRrNCtTYWpYNksrSjlUOFhZVXRTOE8wWWZGUFZqZkhIYTZORWQyODdVcUlqMnJnQlF1bjVDV3hCczFHUm5BYmd1Z3MyL2ZQakcwZmdQemdSYzR5Q3ZObFg4V2pKUnloc3U5VFRKTjd1R3NOdnprU2IyZWlyQmhEaG1vQ0Jqa0wyYnMzT3I2d2pnNnBUNVpmNGhEdDF0STBJNXo1aytxQXVSZnRhd1lmamhXYmpMS0xKOTlUVk1kRDZaTCtTenNtQkNWN05lYm96V0RUTWgrRnJPT292R09ZbUk1bWp4Smd1MVRXNnI1V0JUK2oxSjBFNmJIb2tEMWo0Wm1DWUQreVBPUW1PMm1yUTNGdC9jVmZwQWlJdzliRkgwZ1FIbXQ4QnNuZnQ2MVV3c1h6cSs2akNvY1hOOUMvRXZPblhTczZuVlNGSkVBL3l1QmNIazZxOWdqanBnRG1NTEcrNlpxR1VjRWMzZEp2THpuK3pNT0p3TDI4WUQxN3BLSXBUNnd6WFBFVFJwWS9qNHhoMkQvaFhJRVNHcTk1eTVmZE9MNmx1QT09IiBWZXJzaW9uPSI5LjkiPgogICAgPFR5cGU+UnVudGltZTwvVHlwZT4KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+CiAgICA8RW1haWw+ZU1haWxAaG9zdC5jb208L0VtYWlsPgogICAgPE9yZ2FuaXphdGlvbj5Pcmdhbml6YXRpb248L09yZ2FuaXphdGlvbj4KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4KICAgIDxFeHBpcmVkRGF0ZT4yMDk5LTEyLTMxVDEyOjAwOjAwWjwvRXhwaXJlZERhdGU+CiAgICA8UHJvZHVjdHM+CiAgICAgICAgPFByb2R1Y3Q+CiAgICAgICAgICAgIDxOYW1lPlNwaXJlLk9mZmljZSBQbGF0aW51bTwvTmFtZT4KICAgICAgICAgICAgPFZlcnNpb24+OS45OTwvVmVyc2lvbj4KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZERldmVsb3Blcj45OTk5OTwvTnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+CiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWRTaXRlPjk5OTk5PC9OdW1iZXJPZlBlcm1pdHRlZFNpdGU+CiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPgogICAgICAgIDwvUHJvZHVjdD4KICAgIDwvUHJvZHVjdHM+CiAgICA8SXNzdWVyPgogICAgICAgIDxOYW1lPklzc3VlcjwvTmFtZT4KICAgICAgICA8RW1haWw+aXNzdWVyQGlzc3Vlci5jb208L0VtYWlsPgogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+CiAgICA8L0lzc3Vlcj4KPC9MaWNlbnNlPg==";

            Spire.License.LicenseProvider.SetLicenseKey(LData);
            if (!fn.ToLower().Contains(".xls"))
                fn += ".xlsx";
            var filePath = HttpContext.Current.Server.MapPath("~/upload/" + fn);
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(filePath);
            Worksheet sheet = workbook.Worksheets[0];
            DateTime df;
            DateTime dt;
            var _df = sheet.Range[1, 2].Value2;
            var _dt = sheet.Range[1, 3].Value2;
            try
            {
                df = Convert.ToDateTime(_df);
                dt = Convert.ToDateTime(_dt);
            }
            catch (Exception ex)
            {
                return Ok("ERROR- WRONGE DATES");
            }
            var days = new List<string>() { "SAT", "SUN", "MON", "TUE", "WED", "THU", "FRI" };
            var c = 4;

            var doLoop = true;
            var lastDay = "SAT";
            var xlsFlts = new List<xlsflt>();


            var ns = 0;
            foreach (Worksheet wsheet in workbook.Worksheets)
            {
                var _doLoop = true;
                var emptyRoute = 0;
                c = 4;
                while (_doLoop)
                {
                    //var day = !string.IsNullOrEmpty(sheet.Range[c, 2].Text) ? sheet.Range[c, 2].Text.Replace(" ", "") : "";

                    // if (string.IsNullOrEmpty(day))
                    //    day = lastDay;
                    var day = days[ns];
                    if (days.Contains(day.ToUpper()))
                    {
                        // var route = !string.IsNullOrEmpty(sheet.Range[c, 3].Text) ? sheet.Range[c, 3].Text.Replace(" ", "") : "";
                        var origin = !string.IsNullOrEmpty(wsheet.Range[c, 3].Text) ? wsheet.Range[c, 3].Text.Replace(" ", "") : "";
                        var destination = !string.IsNullOrEmpty(wsheet.Range[c, 4].Text) ? wsheet.Range[c, 4].Text.Replace(" ", "") : "";
                        if (string.IsNullOrEmpty(origin))
                            emptyRoute++;
                        else
                            emptyRoute = 0;
                        if (emptyRoute > 3)
                            _doLoop = false;
                        if (!string.IsNullOrEmpty(origin) && !string.IsNullOrEmpty(destination))
                        {
                            var flightNumber = wsheet.Range[c, 5].Value2.ToString();

                            var _dep = wsheet.Range[c, 6].DateTimeValue;
                            if (_dep == DateTime.MinValue)
                            {
                                var _depStr = wsheet.Range[c, 6].Value.Split(':');
                                _dep = new DateTime(2000, 1, 1, Convert.ToInt32(_depStr[0]), Convert.ToInt32(_depStr[1]), 0);

                            }
                            var _arr = wsheet.Range[c, 7].DateTimeValue;
                            if (_arr == DateTime.MinValue)
                            {
                                var _arrStr = wsheet.Range[c, 7].Value.Split(':');
                                _arr = new DateTime(2000, 1, 1, Convert.ToInt32(_arrStr[0]), Convert.ToInt32(_arrStr[1]), 0);
                            }
                            var reg = wsheet.Range[c, 8].Text;

                            xlsFlts.Add(new xlsflt() { day = day, arr = _arr, dep = _dep, route = origin + "-" + destination, origin = origin, destination = destination, flightNumber = flightNumber, reg = reg });

                        }

                        // if (!string.IsNullOrEmpty(day))
                        //    lastDay = day;
                    }

                    c++;
                }
                ns++;
            }






            var context = new AirpocketAPI.Models.FLYEntities();
            var dblegs = await context.ViewLegTimes.Where(q => q.STDLocal >= df && q.STDLocal <= dt).ToListAsync();
            var dbids = dblegs.Select(q => q.ID).ToList();
            var dbflights = await context.FlightInformations.Where(q => dbids.Contains(q.ID)).ToListAsync();
            var airports = await context.Airports.ToListAsync();
            var registers = await context.Ac_MSN.ToListAsync();
            var _date = df;
            var cnt = 0;
            while (_date <= dt)
            {
                var _day = _date.ToString("ddd").ToUpper();
                var dbs = dblegs.Where(q => q.STDLocal >= _date && q.STDLocal < _date.AddDays(1)).ToList();
                var xls = xlsFlts.Where(q => q.day == _day).Where(q => !string.IsNullOrEmpty(q.route)).ToList();
                foreach (var x in xls)
                {
                    var exist = dbs.FirstOrDefault(q => q.FlightNumber == x.flightNumber);

                    // if (exist == null )
                    {
                        var fromIATA = airports.Where(q => q.IATA == x.from).FirstOrDefault();
                        var toIATA = airports.Where(q => q.IATA == x.to).FirstOrDefault();
                        var register = registers.Where(q => q.Register == x.reg).FirstOrDefault();
                        if (fromIATA != null && toIATA != null && register != null)
                        {
                            var _dep = (DateTime)x.dep;
                            var _arr = (DateTime)x.arr;

                            DateTime std;
                            DateTime sta;
                            DateTime stdlocal = new DateTime(_date.Year, _date.Month, _date.Day, _dep.Hour, _dep.Minute, 0);
                            DateTime stalocal = new DateTime(_date.Year, _date.Month, _date.Day, _arr.Hour, _arr.Minute, 0);

                            var isFromInt = utcs.FirstOrDefault(q => q.iata == fromIATA.IATA);
                            var isToInt = utcs.FirstOrDefault(q => q.iata == toIATA.IATA);

                            if (isFromInt == null)
                            {

                                var tzoffsetstd = -1 * TimeZoneInfo.Local.GetUtcOffset(stdlocal).TotalMinutes;
                                std = stdlocal.AddMinutes(tzoffsetstd);
                            }
                            else
                            {
                                std = stdlocal.AddMinutes(-1 * isFromInt.diff);
                            }

                            if (isToInt == null)
                            {

                                if (stalocal < stdlocal)
                                    stalocal = stalocal.AddDays(1);
                                var tzoffsetsta = -1 * TimeZoneInfo.Local.GetUtcOffset(stalocal).TotalMinutes;
                                sta = stalocal.AddMinutes(tzoffsetsta);

                            }
                            else
                            {
                                sta = stalocal.AddMinutes(-1 * isToInt.diff);
                            }

                            // var ddddd=TimeZoneInfo.GetSystemTimeZones();



                            var submision = stalocal - stdlocal;
                            double totalMin = submision.TotalMinutes;
                            double hours = (totalMin - totalMin % 60) / 60;
                            double minute = totalMin - hours * 60;
                            FlightInformation flt = null;
                            if (exist != null)
                            {
                                flt = dbflights.FirstOrDefault(q => q.ID == exist.ID);
                                if (flt != null)
                                {


                                    flt.RegisterID = register.ID;
                                    flt.FlightTypeID = 9;
                                    flt.FlightStatusID = 4;
                                    flt.FlightNumber = x.flightNumber;
                                    flt.FromAirportId = fromIATA.Id;
                                    flt.ToAirportId = toIATA.Id;
                                    flt.STD = std;
                                    flt.STA = sta;
                                    flt.ChocksOut = std;
                                    flt.ChocksIn = sta;
                                    flt.Takeoff = std;
                                    flt.Landing = sta;

                                    flt.CustomerId = 1;


                                    try
                                    {
                                        flt.FlightH = Convert.ToInt32(hours);
                                        flt.FlightM = Convert.ToByte(minute);
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
                                    RegisterID = register.ID,
                                    FlightTypeID = 9,
                                    FlightStatusID = 1,
                                    FlightNumber = x.flightNumber,
                                    FromAirportId = fromIATA.Id,
                                    ToAirportId = toIATA.Id,
                                    STD = std,
                                    STA = sta,
                                    ChocksOut = std,
                                    ChocksIn = sta,
                                    Takeoff = std,
                                    Landing = sta,

                                    CustomerId = 4,

                                };
                                try
                                {
                                    flt.FlightH = Convert.ToInt32(hours);
                                    flt.FlightM = Convert.ToByte(minute);
                                }
                                catch
                                {
                                    flt.FlightH = 0;
                                    flt.FlightM = 0;
                                }
                                context.FlightInformations.Add(flt);
                            }

                            cnt++;



                        }
                    }

                }

                _date = _date.AddDays(1);
            }

            var reult = await context.SaveChangesAsync();
            var _res = new
            {
                message = cnt + " FLIGHT(S) INSERTED.",
                df
               ,
                dt
               ,
                WS = workbook.Worksheets.Count
               ,
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

            public string from
            {
                get
                {
                    return this.route.Split('-')[0].ToUpper();
                }
            }
            public string to
            {
                get
                {
                    return this.route.Split('-')[1].ToUpper();
                }
            }

        }
        int getFlightOrder(ViewTimeTable t)
        {
            if (t.FlightStatusID == 4)
                return 1000;
            else
                return 1;
        }

        [Route("api/xls/rotate")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLS(DateTime dt1, DateTime dt2, int chr, int time, int cnl, int crew, int sort, int sep)
        {

            var _dt1 = dt1.Date;
            var _dt2 = dt2.Date.AddDays(0);
            var context = new AirpocketAPI.Models.FLYEntities();
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


                string picPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "logo.png");
                ExcelPicture picture = sheet.Pictures.Add(1, 1, picPath);
                sheet.Range[1, 1].ColumnWidth = 8;
                sheet.Range[1, 1].RowHeight = 17;
                picture.TopRowOffset = 4;
                picture.LeftColumnOffset = 40;

                sheet.Range["A1:A2"].Merge();

                sheet.Range[1, 12].ColumnWidth = 40;
                //oopp
                sheet.Range["B1:H1"].Merge();
                //  sheet.Range[1, 2].Value = "TABAN AIRLINES FLIGHTS TIMETABLE";
                sheet.Range[1, 2].Value = "FLIGHTS TIMETABLE"; //"VARESH AIRLINES FLIGHTS TIMETABLE";
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
                        var _pdate = flt.PDate.Substring(0, 10).Split('/').ToList();
                        sheet.Range["A" + r + ":A" + (r + grp.Count() - 1)].Merge();
                        sheet.Range[r, 1].Style.Rotation = 90;
                        sheet[r, 1].RowHeight = 23;
                        sheet.Range[r, 1].Text = ((DateTime)flt.STDDayLocal).ToString("yyyy-MM-dd") + "\r\n" + _pdate[0] + "-" + _pdate[1] + "-" + _pdate[2];
                        sheet.Range[r, 1].Style.Font.Size = 14;
                        sheet.Range[r, 1].Style.Font.IsBold = true;
                        sheet.Range[r, 1].Style.Font.FontName = "Courier New";
                        sheet.Range[r, 1].AutoFitColumns();
                        sheet.Range[r, 1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        sheet.Range[r, 1].Style.VerticalAlignment = VerticalAlignType.Top;

                    }
                    else


                        sheet.Range[r, 1].Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;


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

        [Route("api/xls")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSOLD(DateTime dt1, DateTime dt2, int chr, int time, int cnl, int crew, int sort, int sep, bool utcRef)
        {

            var _dt1 = dt1.Date;
            var _dt2 = dt2.Date.AddDays(0);
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = (from x in context.ViewTimeTables
                         //where x.STDDay >= _dt1 && x.STDDay <= _dt2
                         select x);
            query = query.Where(q => (utcRef) ? q.STDDay >= _dt1 && q.STDDay <= _dt2 : q.STDDayLocal >= _dt1 && q.STDDayLocal <= _dt2);
            if (cnl == -1)
                query = query.Where(q => q.FlightStatusID != 4);


            var totalcnt = query.Count();

            var grps = (from x in query
                        group x by new { refSTD = utcRef ? x.STDDay : x.STDDayLocal } into grp
                        orderby grp.Key.refSTD
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

                    DateTime d = ((DateTime)grp.Key.refSTD);
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


                string picPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "logo.png");
                ExcelPicture picture = sheet.Pictures.Add(1, 1, picPath);
                sheet.Range[1, 1].ColumnWidth = 10;
                sheet.Range[1, 1].RowHeight = 20;
                picture.TopRowOffset = 5;
                picture.LeftColumnOffset = 50;

                sheet.Range["A1:A2"].Merge();

                sheet.Range[1, 12].ColumnWidth = 40;
                //oopp
                sheet.Range["B1:H1"].Merge();
                //  sheet.Range[1, 2].Value = "TABAN AIRLINES FLIGHTS TIMETABLE";
                sheet.Range[1, 2].Value = "FLIGHTS TIMETABLE"; //"VARESH AIRLINES FLIGHTS TIMETABLE";
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
                    sheet.Range[1, 11].Text = ((DateTime)grp.Key.refSTD).ToString("yyyy-MMM-dd");
                    sheet.Range[1, 11].Style.Font.FontName = "Times New Roman";
                    sheet.Range[1, 11].Style.Font.Size = 11;
                    sheet.Range[1, 11].Style.Font.IsBold = true;

                    sheet.Range[1, 11].Style.HorizontalAlignment = HorizontalAlignType.Right;
                    sheet.Range[1, 11].Style.VerticalAlignment = VerticalAlignType.Center;
                    var pdate = grp.First().PDate.Substring(0, 10).Split('/').ToList();
                    sheet.Range[2, 11].Text = pdate[0] + "-" + pdate[1] + "-" + pdate[2];
                    sheet.Range[2, 11].Style.Font.FontName = "Times New Roman";
                    sheet.Range[2, 11].Style.Font.Size = 11;
                    sheet.Range[2, 9].Text = ((DateTime)grp.Key.refSTD).ToString("ddd");
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
                    if (flt != flts.Last() && (flt.Register == _reg || flt.Register == "CNL" || flt.FlightStatusID == 4))
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


        [Route("api/xls/taban")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSTABAN(DateTime dt1, DateTime dt2, int chr, int time, int cnl, int crew, int sort, int sep)
        {

            var _dt1 = dt1.Date;
            var _dt2 = dt2.Date.AddDays(0);
            var context = new AirpocketAPI.Models.FLYEntities();
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


                string picPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "logo.png");
                ExcelPicture picture = sheet.Pictures.Add(1, 1, picPath);
                sheet.Range[1, 1].ColumnWidth = 10;
                sheet.Range[1, 1].RowHeight = 30;
                picture.TopRowOffset = 25;
                picture.LeftColumnOffset = 100;

                sheet.Range["A1:A2"].Merge();

                sheet.Range[1, 12].ColumnWidth = 40;
                //oopp
                sheet.Range["B1:H1"].Merge();
                //  sheet.Range[1, 2].Value = "TABAN AIRLINES FLIGHTS TIMETABLE";
                sheet.Range[1, 2].Value = "VARESH AIRLINES FLIGHTS TIMETABLE";
                sheet.Range[1, 2].RowHeight = 30;
                sheet.Range[1, 2].Style.Font.Size = 14;

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

                    sheet.Range[r, 10].Text = flt.FlightStatusID == 4 ? "TBF" : flt.Register;
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



        [Route("api/xls/gd")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSGD(string flts)
        {
            // var flightIds = new List<int?>();
            var flightIds = flts.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
            var context = new AirpocketAPI.Models.FLYEntities();
            var vflights = context.ViewFlightInformations.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).Select(q => new { q.Register, q.AircraftType, q.ID, q.STD, q.FlightNumber, q.FromAirportIATA, q.ToAirportIATA, q.DepartureLocal, q.STA, q.ArrivalLocal }).ToList();

            //TOKO

            var _crews2 = (from x in
                                    //this.context.ViewFlightCrewNews
                                    context.ViewCrewLists
                               //where x.FlightId == flightId

                           where flightIds.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder
                               , x.RosterPositionId, x.Name

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.RosterPositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,
                               PID = x.PID,
                               Mobile = x.Mobile,
                               Address = x.Address,


                           }).ToList();
            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                               x.PID,
                               x.Mobile,
                               x.Address
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             PID = x.Key.PID,
                             Mobile = x.Key.Mobile,
                             Address = x.Key.Address,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == flightIds.Count)
                    x.LegsStr = "";
            }

            //select DISTINCT CrewId,IsPositioning,PositionId,[Position],Name,GroupId,JobGroup,JobGroupCode,GroupOrder,IsCockpit 
            var _route = new List<string>();
            var _flightNo = new List<string>();
            var _regs = new List<string>();
            var _types = new List<string>();
            foreach (var x in vflights)
            {
                _route.Add(x.FromAirportIATA);
                _flightNo.Add(x.FlightNumber);
                _regs.Add("EP-" + x.Register);
                _types.Add(x.AircraftType);


            }
            _route.Add(vflights.Last().ToAirportIATA);
            _regs = _regs.Distinct().ToList();
            _types = _types.Distinct().ToList();

            var result = new
            {
                flights = vflights,
                crew = query, //_crews,
                route = string.Join("-", _route),
                no = string.Join(",", _flightNo),
                no2 = string.Join("-", _flightNo),
                actype = string.Join(",", _types),
                regs = string.Join(",", _regs),
                std = vflights.First().STD,
                sta = vflights.Last().STA,
                stdLocal = vflights.First().DepartureLocal,
                staLocal = vflights.First().ArrivalLocal,
            };

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "_gd" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[2, 6].Text = result.no2;
            if (result.flights.Count > 1)
                sheet.Range[2, 6].Text = result.no2 + " (" + result.route + ")";
            sheet.Range[2, 8].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd");
            sheet.Range[3, 1].Value = "Marks of Nationality and Registration:" + result.regs;

            sheet.Range[3, 6].Text = result.flights.First().FromAirportIATA + " - " + ((DateTime)result.flights.First().DepartureLocal).ToString("HH:mm");
            sheet.Range[3, 8].Text = result.flights.Last().ToAirportIATA + " - " + ((DateTime)result.flights.Last().ArrivalLocal).ToString("HH:mm");

            var r = 5;
            foreach (var cr in result.crew)
            {

                sheet.Range[r, 2].Text = cr.Position;
                sheet.Range[r, 3].Text = cr.Name;
                r++;
            }

            var name = "gd-" + result.route + "-" + result.no2;
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;



        }


        [Route("api/xls/sec")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSec(string flts)
        {
            // var flightIds = new List<int?>();
            var flightIds = flts.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
            var context = new AirpocketAPI.Models.FLYEntities();
            var vflights = context.ViewFlightInformations.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).Select(q => new { q.Register, q.AircraftType, q.ID, q.STD, q.FlightNumber, q.FromAirportIATA, q.ToAirportIATA, q.DepartureLocal, q.STA, q.ArrivalLocal }).ToList();

            //TOKO

            var _crews2 = (from x in
                                    //this.context.ViewFlightCrewNews
                                    context.ViewCrewLists
                               //where x.FlightId == flightId

                           where flightIds.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder
                               , x.RosterPositionId, x.Name

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.RosterPositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,
                               PID = x.PID,
                               Mobile = x.Mobile,
                               Address = x.Address,
                               NID = x.Sex


                           }).ToList();
            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                               x.PID,
                               x.Mobile,
                               x.Address,
                               x.NID
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             PID = x.Key.PID,
                             NID = x.Key.NID,
                             Mobile = x.Key.Mobile,
                             Address = x.Key.Address,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == flightIds.Count)
                    x.LegsStr = "";
            }

            //select DISTINCT CrewId,IsPositioning,PositionId,[Position],Name,GroupId,JobGroup,JobGroupCode,GroupOrder,IsCockpit 
            var _route = new List<string>();
            var _flightNo = new List<string>();
            var _regs = new List<string>();
            var _types = new List<string>();
            foreach (var x in vflights)
            {
                _route.Add(x.FromAirportIATA);
                _flightNo.Add(x.FlightNumber);
                _regs.Add("EP-" + x.Register);
                _types.Add(x.AircraftType);


            }
            _route.Add(vflights.Last().ToAirportIATA);
            _regs = _regs.Distinct().ToList();
            _types = _types.Distinct().ToList();

            var result = new
            {
                flights = vflights,
                crew = query, //_crews,
                route = string.Join("-", _route),
                no = string.Join(",", _flightNo),
                no2 = string.Join("-", _flightNo),
                actype = string.Join(",", _types),
                regs = string.Join(",", _regs),
                std = vflights.First().STD,
                sta = vflights.Last().STA,
                stdLocal = vflights.First().DepartureLocal,
                staLocal = vflights.First().ArrivalLocal,
            };

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "_sec" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[2, 6].Text = result.no2;
            if (result.flights.Count > 1)
                sheet.Range[2, 6].Text = result.no2 + " (" + result.route + ")";
            sheet.Range[2, 8].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd");
            sheet.Range[3, 1].Value = "Register:" + result.regs;

            sheet.Range[3, 6].Text = result.flights.First().FromAirportIATA + " - " + ((DateTime)result.flights.First().DepartureLocal).ToString("HH:mm");
            sheet.Range[3, 8].Text = result.flights.Last().ToAirportIATA + " - " + ((DateTime)result.flights.Last().ArrivalLocal).ToString("HH:mm");

            var r = 5;
            foreach (var cr in result.crew)
            {

                sheet.Range[r, 2].Text = cr.Position;
                sheet.Range[r, 3].Text = cr.Name;
                sheet.Range[r, 4].Text = cr.NID;
                r++;
            }

            var name = "sec-" + result.route + "-" + result.no2;
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;



        }



        //others
        [Route("api/xls/cl/")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSCL(string flts)
        {
            // var flightIds = new List<int?>();
            var flightIds = flts.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
            var context = new AirpocketAPI.Models.FLYEntities();
            var vflights = context.ViewFlightInformations.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).Select(q => new { q.Register, q.AircraftType, q.ID, q.STD, q.FlightNumber, q.FromAirportIATA, q.ToAirportIATA, q.DepartureLocal, q.STA, q.ArrivalLocal }).ToList();

            //TOKO

            var _crews2 = (from x in
                                    //this.context.ViewFlightCrewNews
                                    context.ViewCrewLists
                               //where x.FlightId == flightId

                           where flightIds.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder
                               , x.RosterPositionId, x.Name

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.RosterPositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,
                               PID = x.PID,
                               Mobile = x.Mobile,
                               Address = x.Address,
                               PassportNo = x.Sex

                           }).ToList();
            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                               x.PID,
                               x.Mobile,
                               x.Address,
                               PassportNo = string.IsNullOrEmpty(x.PassportNo) ? "-" : x.PassportNo
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             PID = x.Key.PID,
                             Mobile = x.Key.Mobile,
                             Address = x.Key.Address,
                             PassportNo = x.Key.PassportNo,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == flightIds.Count)
                    x.LegsStr = "";
            }

            //select DISTINCT CrewId,IsPositioning,PositionId,[Position],Name,GroupId,JobGroup,JobGroupCode,GroupOrder,IsCockpit 
            var _route = new List<string>();
            var _flightNo = new List<string>();
            var _regs = new List<string>();
            var _types = new List<string>();
            foreach (var x in vflights)
            {
                _route.Add(x.FromAirportIATA);
                _flightNo.Add(x.FlightNumber);
                _regs.Add("EP-" + x.Register);
                _types.Add(x.AircraftType);


            }
            _route.Add(vflights.Last().ToAirportIATA);
            _regs = _regs.Distinct().ToList();
            _types = _types.Distinct().ToList();

            var result = new
            {
                flights = vflights,
                crew = query, //_crews,
                route = string.Join("-", _route),
                no = string.Join(",", _flightNo),
                no2 = string.Join("-", _flightNo),
                actype = string.Join(",", _types),
                regs = string.Join(",", _regs),
                std = vflights.First().STD,
                sta = vflights.Last().STA,
                stdLocal = vflights.First().DepartureLocal,
                staLocal = vflights.First().ArrivalLocal,
            };

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "_cl" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[6, 3].Text = result.regs;
            sheet.Range[7, 3].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd HH:mm");
            sheet.Range[7, 6].Text = ((DateTime)result.staLocal).ToString("yyyy-MM-dd HH:mm");
            sheet.Range[8, 3].Text = result.route;
            sheet.Range[8, 6].Text = result.no;
            //sheet.Range[2, 6].Text = result.no2;
            //if (result.flights.Count > 1)
            //    sheet.Range[2, 6].Text = result.no2 + " (" + result.route + ")";
            //sheet.Range[2, 8].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd");
            //sheet.Range[3, 1].Value = "Marks of Nationality and Registration:" + result.regs;

            //sheet.Range[3, 6].Text = result.flights.First().FromAirportIATA + " - " + ((DateTime)result.flights.First().DepartureLocal).ToString("HH:mm");
            //sheet.Range[3, 8].Text = result.flights.Last().ToAirportIATA + " - " + ((DateTime)result.flights.Last().ArrivalLocal).ToString("HH:mm");

            var r = 11;
            foreach (var cr in result.crew)
            {

                sheet.Range[r, 3].Text = cr.Position;
                sheet.Range[r, 4].Text = "";
                sheet.Range[r, 5].Text = cr.Name;
                sheet.Range[r, 6].Text = cr.PassportNo;
                r++;
            }

            var name = "cl-" + result.route + "-" + result.no2;
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;



        }


        [Route("api/xls/cl/karun")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSCLKarub(string flts)
        {
            // var flightIds = new List<int?>();
            var flightIds = flts.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
            var context = new AirpocketAPI.Models.FLYEntities();
            var vflights = context.ViewFlightInformations.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).Select(q => new { q.Register, q.AircraftType, q.ID, q.STD, q.FlightNumber, q.FromAirportIATA, q.ToAirportIATA, q.DepartureLocal, q.STA, q.ArrivalLocal }).ToList();

            //TOKO

            var _crews2 = (from x in
                                    //this.context.ViewFlightCrewNews
                                    context.ViewCrewLists
                               //where x.FlightId == flightId

                           where flightIds.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder
                               , x.RosterPositionId, x.Name

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.RosterPositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,
                               PID = x.PID,
                               Mobile = x.Mobile,
                               Address = x.Address,
                               PassportNo = x.Sex

                           }).ToList();
            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                               x.PID,
                               x.Mobile,
                               x.Address,
                               PassportNo = string.IsNullOrEmpty(x.PassportNo) ? "-" : x.PassportNo
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             PID = x.Key.PID,
                             Mobile = x.Key.Mobile,
                             Address = x.Key.Address,
                             PassportNo = x.Key.PassportNo,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == flightIds.Count)
                    x.LegsStr = "";
            }

            //select DISTINCT CrewId,IsPositioning,PositionId,[Position],Name,GroupId,JobGroup,JobGroupCode,GroupOrder,IsCockpit 
            var _route = new List<string>();
            var _flightNo = new List<string>();
            var _regs = new List<string>();
            var _types = new List<string>();
            foreach (var x in vflights)
            {
                _route.Add(x.FromAirportIATA);
                _flightNo.Add(x.FlightNumber);
                _regs.Add(x.AircraftType + "/" + "EP-" + x.Register);
                _types.Add(x.AircraftType);


            }
            _route.Add(vflights.Last().ToAirportIATA);
            _regs = _regs.Distinct().ToList();
            _types = _types.Distinct().ToList();

            var result = new
            {
                flights = vflights,
                crew = query, //_crews,
                route = string.Join("-", _route),
                no = string.Join(",", _flightNo),
                no2 = string.Join("-", _flightNo),
                actype = string.Join(",", _types),
                regs = string.Join(",", _regs),
                std = vflights.First().STD,
                sta = vflights.Last().STA,
                stdLocal = vflights.First().DepartureLocal,
                staLocal = vflights.Last().ArrivalLocal,
            };

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "_clkarun" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[6, 4].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd");
            sheet.Range[7, 4].Text = result.regs;
            sheet.Range[7, 5].Text = ((DateTime)result.stdLocal).ToString("HH:mm");
            sheet.Range[9, 5].Text = ((DateTime)result.staLocal).ToString("HH:mm");
            sheet.Range[8, 4].Text = result.route;
            sheet.Range[9, 4].Text = result.no;
            //sheet.Range[2, 6].Text = result.no2;
            //if (result.flights.Count > 1)
            //    sheet.Range[2, 6].Text = result.no2 + " (" + result.route + ")";
            //sheet.Range[2, 8].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd");
            //sheet.Range[3, 1].Value = "Marks of Nationality and Registration:" + result.regs;

            //sheet.Range[3, 6].Text = result.flights.First().FromAirportIATA + " - " + ((DateTime)result.flights.First().DepartureLocal).ToString("HH:mm");
            //sheet.Range[3, 8].Text = result.flights.Last().ToAirportIATA + " - " + ((DateTime)result.flights.Last().ArrivalLocal).ToString("HH:mm");

            var r = 11;
            foreach (var cr in result.crew)
            {

                sheet.Range[r, 3].Text = cr.Position;

                sheet.Range[r, 4].Text = cr.Name;
                sheet.Range[r, 5].Text = cr.PID;
                r++;
            }

            var name = "cl-" + result.route + "-" + result.no2;
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;



        }

        [Route("api/xls/drc05")]
        public HttpResponseMessage GetXLSDrc05Karun(string flts)
        {
            var flightIds = flts.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
            var context = new AirpocketAPI.Models.FLYEntities();
            var vflights = context.ViewFlightInformations.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).Select(q => new { q.Register, q.AircraftType, q.ID, q.STD, q.FlightNumber, q.FromAirportIATA, q.ToAirportIATA, q.DepartureLocal, q.STA, q.ArrivalLocal }).ToList();
            var _crews2 = (from x in
                                   //this.context.ViewFlightCrewNews
                                   context.ViewCrewLists
                               //where x.FlightId == flightId

                           where flightIds.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder
                               , x.RosterPositionId, x.Name

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.RosterPositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,
                               PID = x.PID,
                               Mobile = x.Mobile,
                               Address = x.Address,
                               PassportNo = x.Sex

                           }).ToList();
            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                               x.PID,
                               x.Mobile,
                               x.Address,
                               PassportNo = string.IsNullOrEmpty(x.PassportNo) ? "-" : x.PassportNo
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             PID = x.Key.PID,
                             Mobile = x.Key.Mobile,
                             Address = x.Key.Address,
                             PassportNo = x.Key.PassportNo,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == flightIds.Count)
                    x.LegsStr = "";
            }
            var _route = new List<string>();
            var _flightNo = new List<string>();
            var _regs = new List<string>();
            var _types = new List<string>();
            foreach (var x in vflights)
            {
                _route.Add(x.FromAirportIATA);
                _flightNo.Add(x.FlightNumber);
                _regs.Add(x.AircraftType + "/" + "EP-" + x.Register);
                _types.Add(x.AircraftType);


            }
            _route.Add(vflights.Last().ToAirportIATA);
            _regs = _regs.Distinct().ToList();
            _types = _types.Distinct().ToList();

            var result = new
            {
                flights = vflights,
                crew = query, //_crews,
                route = string.Join("-", _route),
                no = string.Join(",", _flightNo),
                no2 = string.Join("-", _flightNo),
                actype = string.Join(",", _types),
                regs = string.Join(",", _regs),
                std = vflights.First().STD,
                sta = vflights.Last().STA,
                stdLocal = vflights.First().DepartureLocal,
                staLocal = vflights.Last().ArrivalLocal,
            };
            var _main_crew = result.crew.Where(q => q.IsPositioning == false).ToList();
            var _pos_crew = result.crew.Where(q => q.IsPositioning == true).ToList();

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drc-05" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[6, 3].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd");
            sheet.Range[8, 3].Text = result.regs;
            // sheet.Range[3, 10].Text = ((DateTime)result.stdLocal).ToString("HH:mm");
            // sheet.Range[4, 10].Text = ((DateTime)result.staLocal).ToString("HH:mm");
            sheet.Range[15, 3].Text = _main_crew.First().Name;
            sheet.Range[7, 3].Text = result.no;

            //9,3
            var cockpit_cnt = _main_crew.Where(q => q.IsCockpit == 1).Count();
            var cabin_cnt = _main_crew.Where(q => q.IsCockpit != 1).Count();
            sheet.Range[9, 3].Text = cockpit_cnt + " / " + cabin_cnt + " / ?";


            sheet.Range[10, 3].Text = "DHD: " + _pos_crew.Count();

            var name = "trip-info-" + result.route + "-" + result.no2;
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;

        }

        [Route("api/xls/drc04")]
        public HttpResponseMessage GetXLSDrc04Karun(string flts)
        {
            // var flightIds = new List<int?>();
            var flightIds = flts.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
            var context = new AirpocketAPI.Models.FLYEntities();
            var vflights = context.ViewFlightInformations.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).Select(q => new { q.Register, q.AircraftType, q.ID, q.STD, q.FlightNumber, q.FromAirportIATA, q.ToAirportIATA, q.DepartureLocal, q.STA, q.ArrivalLocal }).ToList();

            //TOKO

            var _crews2 = (from x in
                                    //this.context.ViewFlightCrewNews
                                    context.ViewCrewLists
                               //where x.FlightId == flightId

                           where flightIds.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder
                               , x.RosterPositionId, x.Name

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.RosterPositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,
                               PID = x.PID,
                               Mobile = x.Mobile,
                               Address = x.Address,
                               PassportNo = x.Sex

                           }).ToList();
            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                               x.PID,
                               x.Mobile,
                               x.Address,
                               PassportNo = string.IsNullOrEmpty(x.PassportNo) ? "-" : x.PassportNo
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             PID = x.Key.PID,
                             Mobile = x.Key.Mobile,
                             Address = x.Key.Address,
                             PassportNo = x.Key.PassportNo,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == flightIds.Count)
                    x.LegsStr = "";
            }

            //select DISTINCT CrewId,IsPositioning,PositionId,[Position],Name,GroupId,JobGroup,JobGroupCode,GroupOrder,IsCockpit 
            var _route = new List<string>();
            var _flightNo = new List<string>();
            var _regs = new List<string>();
            var _types = new List<string>();
            foreach (var x in vflights)
            {
                _route.Add(x.FromAirportIATA);
                _flightNo.Add(x.FlightNumber);
                _regs.Add(x.AircraftType + "/" + "EP-" + x.Register);
                _types.Add(x.AircraftType);


            }
            _route.Add(vflights.Last().ToAirportIATA);
            _regs = _regs.Distinct().ToList();
            _types = _types.Distinct().ToList();

            var result = new
            {
                flights = vflights,
                crew = query, //_crews,
                route = string.Join("-", _route),
                no = string.Join(",", _flightNo),
                no2 = string.Join("-", _flightNo),
                actype = string.Join(",", _types),
                regs = string.Join(",", _regs),
                std = vflights.First().STD,
                sta = vflights.Last().STA,
                stdLocal = vflights.First().DepartureLocal,
                staLocal = vflights.Last().ArrivalLocal,
            };

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drc-04" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[2, 3].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd");
            sheet.Range[5, 3].Text = result.regs;
            sheet.Range[3, 10].Text = ((DateTime)result.stdLocal).ToString("HH:mm");
            sheet.Range[4, 10].Text = ((DateTime)result.staLocal).ToString("HH:mm");
            sheet.Range[3, 3].Text = result.route;
            sheet.Range[4, 3].Text = result.no;


            var _main_crew = result.crew.Where(q => q.IsPositioning == false).ToList();
            var _pos_crew = result.crew.Where(q => q.IsPositioning == true).ToList();
            var r = 22;
            foreach (var cr in _main_crew)
            {

                sheet.Range[r, 1].Text = cr.Position;

                sheet.Range[r, 3].Text = cr.Name + (string.IsNullOrEmpty(cr.LegsStr) ? "" : "(" + cr.LegsStr + ")");
                sheet.Range[r, 6].Text = cr.PID;
                r++;
            }


            r = 22;
            foreach (var cr in _pos_crew)
            {

                sheet.Range[r, 8].Text = "DH";

                sheet.Range[r, 10].Text = cr.Name + (string.IsNullOrEmpty(cr.LegsStr) ? "" : "(" + cr.LegsStr + ")");
                sheet.Range[r, 13].Text = cr.PID;
                r++;
            }

            sheet.Range[37, 9].Text = _main_crew.First().Name;

            var name = "dispatch-release-" + result.route + "-" + result.no2;
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;




        }

        public class crew_group
        {
            public int? group_id { get; set; }
            public string group_title { get; set; }
            public List<ViewCrewValidFTL> items { get; set; }
        }
        public class duty_crew
        {
            public int? crew_id { get; set; }
            public string group_title { get; set; }
            public int? group_id { get; set; }
            public DateTime? duty_date { get; set; }
            public List<ViewCrewDutyTimeLineNewGDate> duties { get; set; }
        }

        [Route("api/xls/sch01")]
        public HttpResponseMessage GetXLSSCH01(DateTime df, DateTime dt)
        {
            //2024-01-27
            var context = new AirpocketAPI.Models.FLYEntities();
            var query_crew = (from x in context.ViewCrewValidFTLs select x).ToList();
            var grp_crews = (from x in query_crew
                           group x by new { x.JobGroup, x.GroupId } into grp
                           select new crew_group()
                           {
                               group_id = grp.Key.GroupId,
                               group_title = grp.Key.JobGroup,
                               items = grp.OrderBy(q => q.ScheduleName).ToList()

                           }).ToList();

            var crews = query_crew.OrderBy(q => q.GroupOrder).ThenBy(q=>q.BaseAirportId).ThenBy(q => q.LastName).ThenBy(q => q.FirstName).ToList();
            var utcdiff = 210;
            df = df.AddMinutes(0);
            dt = dt.AddDays(1);
            var cgrps = new List<string>() {"TRI","TRE","P1","P2","CCM","SCCM","ISCCM" };
            var query_duty = (from x in context.ViewCrewDutyTimeLineNewGDates
                             where x.GDate >= df && x.GDate < dt && cgrps.Contains(x.JobGroup)
                             select x).ToList();
            var grp_duties = (from x in query_duty
                              group x by new { x.CrewId, x.JobGroup, x.JobGroupId,x.GDate } into grp
                              select new duty_crew()
                              {
                                  crew_id = grp.Key.CrewId,
                                  group_title = grp.Key.JobGroup,
                                  group_id = grp.Key.JobGroupId,
                                  duty_date=grp.Key.GDate,
                                  duties = grp.OrderBy(q => q.GDate).ToList()
                              }).OrderByDescending(q=>q.duties.Count).ToList();
            var crew_ids = grp_duties.Select(q => q.crew_id).ToList();
            var date_start = df;
            List<DateTime> dates = new List<DateTime>();
            while (date_start <= dt)
            {
                dates.Add(date_start);
                date_start = date_start.AddDays(1);
            }


            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "sch-01" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet;
            var _ranks = new List<string>() { "IP-P1","P2","ISCCM-SCCM","CCM" };

            


            foreach (var _rank in _ranks)
            {
                var ds_crew = new List<ViewCrewValidFTL>();
                switch (_rank)
                {
                    case "IP-P1":
                        ds_crew = crews.Where(q => q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1").ToList();
                        workbook.Worksheets[0].Name = "IP-P1";
                        sheet = workbook.Worksheets[0];
                      //  IPrstGeomShape triangle = sheet.PrstGeomShapes.AddPrstGeomShape(1, 1, 200, 40, PrstGeomShapeType.Rect);
                        
                     //  IPrstGeomShape triangle2 = sheet.PrstGeomShapes.AddPrstGeomShape(2, 1, 200, 40, PrstGeomShapeType.Rect)  ;
                      //  var xxxx = sheet.Range[1, 1].GetRectangles();
                         
                        break;
                    case "ISCCM-SCCM":
                        ds_crew = crews.Where(q =>q.JobGroup == "ISCCM" || q.JobGroup == "SCCM").ToList();
                        sheet=workbook.Worksheets.Add("ISCCM-SCCM");
                        break;
                    default:
                        ds_crew = crews.Where(q => q.JobGroup == _rank).ToList();
                        sheet = workbook.Worksheets.Add(_rank);
                        break;
                }
                var _col = 2;
                foreach (var _date in dates)
                {
                    sheet.Range[1, _col].Text = _date.ToString("yy-MMM-dd");
                    sheet.Range[1, _col].HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[1, _col].VerticalAlignment = VerticalAlignType.Center;
                    sheet.Range[1, _col].Style.Color = Color.FromArgb(203, 203, 203);

                    //sheet.Range[1, _col].BorderInside(LineStyleType.Thin, Color.Black);
                    //sheet.Range[1, _col].BorderAround(LineStyleType.Thin, Color.Black);
                    _col++;
                }
                var _row = 2;
                foreach(var _crew in ds_crew)
                {
                    sheet.Range[_row, 1].Text = _crew.ScheduleName+"("+_crew.JobGroup+")";
                    sheet.Range[_row, 1].HorizontalAlignment = HorizontalAlignType.Left;
                    sheet.Range[_row, 1].VerticalAlignment = VerticalAlignType.Center;
                    sheet.Range[_row, 1].Style.Color = Color.FromArgb(203, 203, 203);


                    _col = 2;
                    foreach (var _date in dates)
                    {

                        var _data = grp_duties.Where(q => q.crew_id == _crew.Id && ((DateTime)q.duty_date).Date == _date.Date).FirstOrDefault();
                        if (_data!=null && _data.duties.Count > 0)
                        {
                            foreach(var dty in _data.duties)
                            {
                                if (dty.InitStartLocal == null || dty.InitEndLocal == null || dty.InitRestToLocal == null)
                                    continue;
                                var _start = ((DateTime)dty.InitStartLocal);
                                var _end= ((DateTime)dty.InitEndLocal);
                                var _start_date = _start.Date;
                                var _end_date = _end.Date;
                                
                                switch (dty.DutyType)
                                {
                                    case 1165:
                                        // sheet.Range[_row, _col].Text += dty.DutyTypeTitle + "\n";
                                        //sheet.Range[_row, _col].IsWrapText = true;
                                        //var txt= sheet.TextBoxes.AddTextBox(_row, _col, 50, 50);
                                        //txt.Text = "FDP";
                                        //sheet.Rows[_row - 1].SetRowHeight(80, false);
                                        try
                                        {
                                            sheet.Range[_row, _col].Text += dty.InitRoute + "\n";
                                            
                                            sheet.Range[_row, _col].Text += ((DateTime)dty.InitStartLocal).ToString("HHmm") + " " + ((DateTime)dty.InitEndLocal).ToString("HHmm") +" "+ ((DateTime)dty.InitRestToLocal).ToString("HHmm") + "\n";
                                           
                                            
                                            sheet.Range[_row, _col].Text += "\n";
                                             sheet.Range[_row, _col].HorizontalAlignment = HorizontalAlignType.Center;
                                            sheet.Range[_row, _col].VerticalAlignment = VerticalAlignType.Top;
                                            sheet.Range[_row, _col].Style.Font.Size = 9;
                                            sheet.Range[_row, _col].Style.Font.Color = Color.Black;
                                            sheet.Range[_row, _col].Style.Font.IsBold = true;



                                        }
                                        catch(Exception ex)
                                        {

                                        }
                                        
                                        break;
                                    case 1167:
                                    case 1168:
                                    case 300013:
                                        try
                                        {

                                            sheet.Range[_row, _col].Text += dty.DutyTypeTitle + "\n";
                                            if (_start_date == _end_date)
                                                sheet.Range[_row, _col].Text += ((DateTime)dty.InitStartLocal).ToString("HHmm") + " " + ((DateTime)dty.InitEndLocal).ToString("HHmm") + " " + ((DateTime)dty.InitRestToLocal).ToString("HHmm") + "\n";
                                            else
                                                sheet.Range[_row, _col].Text += "("+_start.ToString("dd")+")"+(_start).ToString("HHmm") + " " + "("+ _end.ToString("dd") + ")" + (_end).ToString("HHmm") + " " + ((DateTime)dty.InitRestToLocal).ToString("HHmm") + "\n";


                                            sheet.Range[_row, _col].Text += "\n";
                                            sheet.Range[_row, _col].HorizontalAlignment = HorizontalAlignType.Center;
                                            sheet.Range[_row, _col].VerticalAlignment = VerticalAlignType.Top;
                                            sheet.Range[_row, _col].Style.Font.Size = 9;
                                            sheet.Range[_row, _col].Style.Font.Color = Color.FromArgb(219, 9, 229);
                                            sheet.Range[_row, _col].Style.Font.IsBold = true;



                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        break;
                                    case 1170:
                                        try
                                        {
                                            sheet.Range[_row, _col].Text += dty.DutyTypeTitle + "\n";
                                            if (_start_date == _end_date)
                                                sheet.Range[_row, _col].Text += ((DateTime)dty.InitStartLocal).ToString("HHmm") + " " + ((DateTime)dty.InitEndLocal).ToString("HHmm")  + "\n";
                                            else
                                                sheet.Range[_row, _col].Text += "(" + _start.ToString("dd") + ")" + (_start).ToString("HHmm") + " " + "(" + _end.ToString("dd") + ")" + (_end).ToString("HHmm")  + "\n";

                                            sheet.Range[_row, _col].Text += "\n";
                                            sheet.Range[_row, _col].HorizontalAlignment = HorizontalAlignType.Center;
                                            sheet.Range[_row, _col].VerticalAlignment = VerticalAlignType.Top;
                                            sheet.Range[_row, _col].Style.Font.Size = 9;
                                            sheet.Range[_row, _col].Style.Font.Color = Color.FromArgb(233, 91, 27);
                                            sheet.Range[_row, _col].Style.Font.IsBold = true;



                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        break;
                                    case 10000:
                                    case 1166:
                                    case 1169:
                                    case 100007:
                                    case 100008:
                                    case 300009:
                                        try
                                        {
                                            sheet.Range[_row, _col].Text += dty.DutyTypeTitle + "\n";
                                            if (_start_date == _end_date)
                                                sheet.Range[_row, _col].Text += ((DateTime)dty.InitStartLocal).ToString("HHmm") + " " + ((DateTime)dty.InitEndLocal).ToString("HHmm") + "\n";
                                            else
                                                sheet.Range[_row, _col].Text += "(" + _start.ToString("dd") + ")" + (_start).ToString("HHmm") + " " + "(" + _end.ToString("dd") + ")" + (_end).ToString("HHmm") + "\n";
                                            sheet.Range[_row, _col].Text += "\n";
                                            sheet.Range[_row, _col].HorizontalAlignment = HorizontalAlignType.Center;
                                            sheet.Range[_row, _col].VerticalAlignment = VerticalAlignType.Top;
                                            sheet.Range[_row, _col].Style.Font.Size = 9;
                                            sheet.Range[_row, _col].Style.Font.Color = Color.FromArgb(0, 168, 76);
                                            sheet.Range[_row, _col].Style.Font.IsBold = true;
                                           



                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        break;
                                    case 100000:
                                    case 100002:
                                    case 100004:
                                    case 100005:
                                    case 100006:
                                    case 100009:

                                        try
                                        {
                                            sheet.Range[_row, _col].Text += dty.DutyTypeTitle + "\n";
                                            if (_start_date == _end_date)
                                                sheet.Range[_row, _col].Text += ((DateTime)dty.InitStartLocal).ToString("HHmm") + " " + ((DateTime)dty.InitEndLocal).ToString("HHmm") + "\n";
                                            else
                                                sheet.Range[_row, _col].Text += "(" + _start.ToString("dd") + ")" + (_start).ToString("HHmm") + " " + "(" + _end.ToString("dd") + ")" + (_end).ToString("HHmm") + "\n";
                                            sheet.Range[_row, _col].Text += "\n";
                                            sheet.Range[_row, _col].HorizontalAlignment = HorizontalAlignType.Center;
                                            sheet.Range[_row, _col].VerticalAlignment = VerticalAlignType.Top;
                                            sheet.Range[_row, _col].Style.Font.Size = 9;
                                            sheet.Range[_row, _col].Style.Font.Color = Color.Red;
                                            sheet.Range[_row, _col].Style.Font.IsBold = true;



                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        break;


                                    case 5000:
                                    case 5001:
                                    case 100001:
                                    case 100003:
                                    case 100025:
                                    case 300008:

                                        try
                                        {
                                            sheet.Range[_row, _col].Text += dty.DutyTypeTitle + "\n";
                                            if (_start_date == _end_date)
                                                sheet.Range[_row, _col].Text += ((DateTime)dty.InitStartLocal).ToString("HHmm") + " " + ((DateTime)dty.InitEndLocal).ToString("HHmm") + "\n";
                                            else
                                                sheet.Range[_row, _col].Text += "(" + _start.ToString("dd") + ")" + (_start).ToString("HHmm") + " " + "(" + _end.ToString("dd") + ")" + (_end).ToString("HHmm") + "\n";
                                            sheet.Range[_row, _col].Text += "\n";
                                            sheet.Range[_row, _col].HorizontalAlignment = HorizontalAlignType.Center;
                                            sheet.Range[_row, _col].VerticalAlignment = VerticalAlignType.Top;
                                            sheet.Range[_row, _col].Style.Font.Size = 9;
                                            sheet.Range[_row, _col].Style.Font.Color = Color.FromArgb(0, 153, 255);
                                            sheet.Range[_row, _col].Style.Font.IsBold = true;



                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        break;

                                    case 300050:

                                        try
                                        {
                                            sheet.Range[_row, _col].Text += dty.PosFrom+"-"+dty.PosTo + "\n";
                                            if (_start_date == _end_date)
                                                sheet.Range[_row, _col].Text += ((DateTime)dty.InitStartLocal).ToString("HHmm") + " " + ((DateTime)dty.InitEndLocal).ToString("HHmm") + "\n";
                                            else
                                                sheet.Range[_row, _col].Text += "(" + _start.ToString("dd") + ")" + (_start).ToString("HHmm") + " " + "(" + _end.ToString("dd") + ")" + (_end).ToString("HHmm") + "\n";
                                            sheet.Range[_row, _col].Text += dty.PosAirline + "(" + dty.PosRemark + ")"+"\n";
                                            sheet.Range[_row, _col].Text += "\n";
                                            sheet.Range[_row, _col].HorizontalAlignment = HorizontalAlignType.Center;
                                            sheet.Range[_row, _col].VerticalAlignment = VerticalAlignType.Top;
                                            sheet.Range[_row, _col].Style.Font.Size = 9;
                                            sheet.Range[_row, _col].Style.Font.Color = Color.FromArgb(0, 153, 255);
                                            sheet.Range[_row, _col].Style.Font.IsBold = true;



                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        break;

                                    default:

                                        break;
                                }
                              
                            }

                        }
                        _col++;
                    }

                    _row++;
                }

                for (var _c = 1; _c <  _col; _c++)
                    sheet.Columns[_c - 1].AutoFitColumns();
                for (var _c = 1; _c <  _row; _c++)
                    sheet.Rows[_c - 1].AutoFitRows();

                // sheet.Range["A1" + ln + ":Q" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                // sheet.Range["B" + ln + ":Q" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                sheet.Range[1,1,_row-1,_col-1].BorderInside(LineStyleType.Thin, Color.Black);
                sheet.Range[1, 1, _row - 1, _col - 1].BorderAround(LineStyleType.Thin, Color.Black);
                sheet.FreezePanes(2, 2);


            }



            var name = "timeline_" + df.ToString("yyyy-MM-dd") + "_" + dt.ToString("yyyy-MM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

           
            return response;

        }
        //chabahar
        [Route("api/xls/cl/chb")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSCLCHB(string flts)
        {
            // var flightIds = new List<int?>();
            var flightIds = flts.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
            var context = new AirpocketAPI.Models.FLYEntities();
            var vflights = context.ViewFlightInformations.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).Select(q => new { q.Register, q.AircraftType, q.ID, q.STD, q.FlightNumber, q.FromAirportIATA, q.ToAirportIATA, q.DepartureLocal, q.STA, q.ArrivalLocal }).ToList();

            //TOKO

            var _crews2 = (from x in
                                    //this.context.ViewFlightCrewNews
                                    context.ViewCrewLists
                               //where x.FlightId == flightId

                           where flightIds.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder
                               , x.RosterPositionId, x.Name

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.RosterPositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,
                               PID = x.PID,
                               Mobile = x.Mobile,
                               Address = x.Address,
                               PassportNo = x.Sex

                           }).ToList();
            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                               x.PID,
                               x.Mobile,
                               x.Address,
                               PassportNo = string.IsNullOrEmpty(x.PassportNo) ? "-" : x.PassportNo
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             PID = x.Key.PID,
                             Mobile = x.Key.Mobile,
                             Address = x.Key.Address,
                             PassportNo = x.Key.PassportNo,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == flightIds.Count)
                    x.LegsStr = "";
            }

            //select DISTINCT CrewId,IsPositioning,PositionId,[Position],Name,GroupId,JobGroup,JobGroupCode,GroupOrder,IsCockpit 
            var _route = new List<string>();
            var _flightNo = new List<string>();
            var _regs = new List<string>();
            var _types = new List<string>();
            foreach (var x in vflights)
            {
                _route.Add(x.FromAirportIATA);
                _flightNo.Add(x.FlightNumber);
                _regs.Add("EP-" + x.Register);
                _types.Add(x.AircraftType);


            }
            _route.Add(vflights.Last().ToAirportIATA);
            _regs = _regs.Distinct().ToList();
            _types = _types.Distinct().ToList();

            var cockpit = query.Where(q => q.JobGroupCode.StartsWith("00101")).OrderBy(q => q.GroupOrder).ThenBy(q => q.Name).ToList();
            var cabin = query.Where(q => q.JobGroupCode.StartsWith("00102")).OrderBy(q => q.GroupOrder).ThenBy(q => q.Name).ToList();
            var other = query.Where(q => !q.JobGroupCode.StartsWith("00102") && !q.JobGroupCode.StartsWith("00101")).OrderBy(q => q.GroupOrder).ThenBy(q => q.Name).ToList();
            var result = new
            {
                flights = vflights,
                crew = query, //_crews,
                cockpit,
                cabin,
                other,
                route = string.Join("-", _route),
                no = string.Join("-", _flightNo),
                no2 = string.Join("-", _flightNo),
                actype = string.Join(",", _types),
                regs = string.Join(",", _regs),
                std = vflights.First().STD,
                sta = vflights.Last().STA,
                stdLocal = vflights.First().DepartureLocal,
                staLocal = vflights.Last().ArrivalLocal,
            };

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "_clchb" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Range[3, 2].Text = ((DateTime)result.stdLocal).ToString("yyyy-MMM-dd");
            DateTime d = ((DateTime)result.stdLocal);
            PersianCalendar pc = new PersianCalendar();
            sheet.Range[3, 3].Text = "(" + pc.GetYear(d) + "/" + pc.GetMonth(d).ToString().PadLeft(2, '0') + "/" + pc.GetDayOfMonth(d).ToString().PadLeft(2, '0') + ")";

            sheet.Range[3, 7].Text = ((DateTime)result.staLocal).ToString("yyyy-MMM-dd");

            sheet.Range[4, 2].Text = ((DateTime)result.stdLocal).ToString("HH:mm");
            sheet.Range[4, 7].Text = ((DateTime)result.staLocal).ToString("HH:mm");

            sheet.Range[5, 2].Text = result.route;
            sheet.Range[5, 7].Text = result.no;

            sheet.Range[6, 2].Text = result.regs;
            sheet.Range[6, 7].Text = "MD";



            //sheet.Range[2, 6].Text = result.no2;
            //if (result.flights.Count > 1)
            //    sheet.Range[2, 6].Text = result.no2 + " (" + result.route + ")";
            //sheet.Range[2, 8].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd");
            //sheet.Range[3, 1].Value = "Marks of Nationality and Registration:" + result.regs;

            //sheet.Range[3, 6].Text = result.flights.First().FromAirportIATA + " - " + ((DateTime)result.flights.First().DepartureLocal).ToString("HH:mm");
            //sheet.Range[3, 8].Text = result.flights.Last().ToAirportIATA + " - " + ((DateTime)result.flights.Last().ArrivalLocal).ToString("HH:mm");

            var r = 8;
            int rowno = 1;
            foreach (var cr in result.cockpit)
            {
                sheet.Range[r, 1].Text = rowno.ToString();
                sheet.Range[r, 2].Text = cr.Position;
                sheet.Range[r, 3].Text = "";
                sheet.Range[r, 4].Text = cr.Name + (cr.Legs.Count != flightIds.Count ? " (" + cr.LegsStr + ")" : "");
                sheet.Range[r, 5].Text = cr.PassportNo;
                r++;
                rowno++;
            }
            r++;
            rowno = 1;
            foreach (var cr in result.cabin)
            {
                sheet.Range[r, 1].Text = rowno.ToString();
                sheet.Range[r, 2].Text = cr.Position;
                sheet.Range[r, 3].Text = "";
                sheet.Range[r, 4].Text = cr.Name + (cr.Legs.Count != flightIds.Count ? " (" + cr.LegsStr + ")" : "");
                sheet.Range[r, 5].Text = cr.PassportNo;
                r++;
                rowno++;
            }

            r++;
            rowno = 1;
            foreach (var cr in result.other)
            {
                sheet.Range[r, 1].Text = rowno.ToString();
                sheet.Range[r, 2].Text = cr.Position;
                sheet.Range[r, 3].Text = "";
                sheet.Range[r, 4].Text = cr.Name + (cr.Legs.Count != flightIds.Count ? " (" + cr.LegsStr + ")" : "");
                sheet.Range[r, 5].Text = cr.PassportNo;
                r++;
                rowno++;
            }

            var name = "clchb-" + result.route + "-" + result.no2;
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;



        }


        //chabahar
        [Route("api/xls/jl/")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSJLCHB(string flts)
        {
            // var flightIds = new List<int?>();
            var flightIds = flts.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
            var context = new AirpocketAPI.Models.FLYEntities();
            var vflights = context.ViewFlightInformations.Where(q => flightIds.Contains(q.ID)).OrderBy(q => q.STD).Select(q => new { q.Register, q.AircraftType, q.ID, q.STD, q.FlightNumber, q.FromAirportIATA, q.ToAirportIATA, q.DepartureLocal, q.STA, q.ArrivalLocal }).ToList();

            //TOKO

            var _crews2 = (from x in
                                    //this.context.ViewFlightCrewNews
                                    context.ViewCrewLists
                               //where x.FlightId == flightId

                           where flightIds.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder
                               , x.RosterPositionId, x.Name

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.RosterPositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,
                               PID = x.PID,
                               Mobile = x.Mobile,
                               Address = x.Address,
                               PassportNo = x.Sex

                           }).ToList();
            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                               x.PID,
                               x.Mobile,
                               x.Address,
                               PassportNo = string.IsNullOrEmpty(x.PassportNo) ? "-" : x.PassportNo
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             PID = x.Key.PID,
                             Mobile = x.Key.Mobile,
                             Address = x.Key.Address,
                             PassportNo = x.Key.PassportNo,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", vflights.Where(q => xfids.Contains((int)q.ID)).OrderBy(q => q.DepartureLocal).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == flightIds.Count)
                    x.LegsStr = "";
            }

            //select DISTINCT CrewId,IsPositioning,PositionId,[Position],Name,GroupId,JobGroup,JobGroupCode,GroupOrder,IsCockpit 
            var _route = new List<string>();
            var _flightNo = new List<string>();
            var _regs = new List<string>();
            var _types = new List<string>();
            foreach (var x in vflights)
            {
                _route.Add(x.FromAirportIATA);
                _flightNo.Add(x.FlightNumber);
                _regs.Add("EP-" + x.Register);
                _types.Add(x.AircraftType);


            }
            _route.Add(vflights.Last().ToAirportIATA);
            _regs = _regs.Distinct().ToList();
            _types = _types.Distinct().ToList();

            var cockpit = query.Where(q => q.JobGroupCode.StartsWith("00101")).OrderBy(q => q.GroupOrder).ThenBy(q => q.Name).ToList();
            var cabin = query.Where(q => q.JobGroupCode.StartsWith("00102")).OrderBy(q => q.GroupOrder).ThenBy(q => q.Name).ToList();
            var other = query.Where(q => !q.JobGroupCode.StartsWith("00102") && !q.JobGroupCode.StartsWith("00101")).OrderBy(q => q.GroupOrder).ThenBy(q => q.Name).ToList();

            var result = new
            {
                flights = vflights,
                crew = query, //_crews,
                cockpit,
                cabin,
                other,
                route = string.Join("-", _route),
                no = string.Join("-", _flightNo),
                no2 = string.Join("-", _flightNo),
                actype = string.Join(",", _types),
                regs = string.Join(",", _regs),
                std = vflights.First().STD,
                sta = vflights.Last().STA,
                stdLocal = vflights.First().DepartureLocal,
                staLocal = vflights.Last().ArrivalLocal,
            };

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "_jlchb" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];
            // sheet.Range[3, 2].Text = ((DateTime)result.stdLocal).ToString("yyyy-MMM-dd");
            DateTime d = ((DateTime)result.stdLocal);
            PersianCalendar pc = new PersianCalendar();

            sheet.Range[10, 11].Text = result.regs;
            sheet.Range[10, 9].Text = "MD";

            // sheet.Range[3, 3].Text = "(" + pc.GetYear(d) + "/" + pc.GetMonth(d).ToString().PadLeft(2, '0') + "/" + pc.GetDayOfMonth(d).ToString().PadLeft(2, '0') + ")";

            sheet.Range[10, 14].Text = ((DateTime)result.std).ToString("yyyy-MMM-dd");


            var _oflts = vflights.OrderBy(q => q.STD).ToList();
            var _nf = 14;
            foreach (var flt in _oflts)
            {
                sheet.Range[_nf, 4].Text = flt.FlightNumber;
                sheet.Range[_nf, 5].Text = flt.FromAirportIATA;
                sheet.Range[_nf, 6].Text = flt.ToAirportIATA;
                sheet.Range[_nf, 7].Text = ((DateTime)flt.STD).ToString("HH:mm");
                sheet.Range[_nf, 8].Text = ((DateTime)flt.STA).ToString("HH:mm");
                _nf++;

            }



            //  sheet.Range[4, 2].Text = ((DateTime)result.stdLocal).ToString("HH:mm");
            // sheet.Range[4, 7].Text = ((DateTime)result.staLocal).ToString("HH:mm");

            //  sheet.Range[5, 2].Text = result.route;
            // sheet.Range[5, 7].Text = result.no;





            //sheet.Range[2, 6].Text = result.no2;
            //if (result.flights.Count > 1)
            //    sheet.Range[2, 6].Text = result.no2 + " (" + result.route + ")";
            //sheet.Range[2, 8].Text = ((DateTime)result.stdLocal).ToString("yyyy-MM-dd");
            //sheet.Range[3, 1].Value = "Marks of Nationality and Registration:" + result.regs;

            //sheet.Range[3, 6].Text = result.flights.First().FromAirportIATA + " - " + ((DateTime)result.flights.First().DepartureLocal).ToString("HH:mm");
            //sheet.Range[3, 8].Text = result.flights.Last().ToAirportIATA + " - " + ((DateTime)result.flights.Last().ArrivalLocal).ToString("HH:mm");

            var _cf = 21;

            foreach (var cr in result.cockpit)
            {
                sheet.Range[_cf, 1].Text = cr.Position;


                sheet.Range[_cf, 3].Text = cr.Name + (cr.Legs.Count != flightIds.Count ? " (" + cr.LegsStr + ")" : "");

                _cf++;

            }

            foreach (var cr in result.cabin)
            {
                sheet.Range[_cf, 1].Text = cr.Position;


                sheet.Range[_cf, 3].Text = cr.Name + (cr.Legs.Count != flightIds.Count ? " (" + cr.LegsStr + ")" : "");

                _cf++;

            }



            foreach (var cr in result.other)
            {
                sheet.Range[_cf, 1].Text = cr.Position;


                sheet.Range[_cf, 3].Text = cr.Name + (cr.Legs.Count != flightIds.Count ? " (" + cr.LegsStr + ")" : "");

                _cf++;

            }

            var name = "jlchb-" + result.route + "-" + result.no2;
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;



        }


        [Route("api/xls/roster/daily")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSRosterDaily(DateTime df)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            df = ((DateTime)df).Date;
            var dt = df.AddDays(1);
            var fdps = (from x in context.ViewFDPRests
                        where x.DutyType == 1165 && x.STDLocal >= df && x.STDLocal < dt
                        select x.Id).ToList();
            var fdpitems = (from x in context.FDPItems
                            where fdps.Contains(x.FDPId)
                            select x.FlightId).ToList();
            var query = from x in context.ReportRosters
                            //where x.STDDay == df
                        where fdpitems.Contains(x.ID)
                        orderby x.Register, x.STDx
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.ID,
                q.FlightNumber,
                q.Register,
                q.FromAirportIATA,
                Base = q.FromAirportIATA == "IKA" ? "THR" : q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.STDx,
                q.STAx,
                q.STDLocal,
                q.STALocal,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT
            }).ToList();
            var regs = (from x in main

                        group x by new { x.Register } into grp
                        select new
                        {
                            grp.Key.Register,
                            Items = grp.OrderBy(q => q.STDx).ToList(),
                            Base = grp.OrderBy(q => q.STDx).First().Base
                        }
                       ).OrderByDescending(q => q.Base).ToList();
            var dqs = context.ViewCrewDuties.Where(x => (df >= x.DateLocal && df <= x.DateLocal2)).ToList();
            var dutiesQuery = (from x in /*context.ViewCrewDuties*/ dqs
                               where (df >= x.DateLocal && df <= x.DateLocal2) && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001
                               || x.DutyType == 100003
                               || x.DutyType == 1166
                               || x.DutyType == 10000
                                || x.DutyType == 10001
                                 || x.DutyType == 100007

                                 || x.DutyType == 300009

                                 || x.DutyType == 1169

                                 || x.DutyType == 100002

                                   || x.DutyType == 100000

                                    || x.DutyType == 5000

                               )
                               select x).ToList();

            var off = dutiesQuery.Where(q => (q.DutyType == 1166 || q.DutyType == 10000 || q.DutyType == 10001 || q.DutyType == 100007) && q.IsCockpit == 0).ToList();
            var off_mhd = off.Where(q => q.BaseAirportId == 140870).ToList();
            var off_thr = off.Where(q => q.BaseAirportId == 135502).ToList();
            var rest = dutiesQuery.Where(q => q.DutyType == 300009 && q.IsCockpit == 0).ToList();
            var rest_mhd = rest.Where(q => q.BaseAirportId == 140870).ToList();
            var rest_thr = rest.Where(q => q.BaseAirportId == 135502).ToList();
            var rsv = dutiesQuery.Where(q => q.DutyType == 1170 && q.IsCockpit == 0).ToList();
            var rsv_mhd = rsv.Where(q => q.BaseAirportId == 140870).ToList();
            var rsv_thr = rsv.Where(q => q.BaseAirportId == 135502).ToList();
            var vac = dutiesQuery.Where(q => q.DutyType == 1169 && q.IsCockpit == 0).ToList();
            var sick = dutiesQuery.Where(q => q.DutyType == 100002 && q.IsCockpit == 0).ToList();
            var ground = dutiesQuery.Where(q => q.DutyType == 100000 && q.IsCockpit == 0).ToList();
            var trn = dutiesQuery.Where(q => q.DutyType == 5000 && q.IsCockpit == 0).ToList();
            var office = dutiesQuery.Where(q => q.DutyType == 5001 && q.IsCockpit == 0).ToList();
            //140870
            var stbyam_thr = from x in dutiesQuery
                             where x.DutyType == 1168 && x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbyam_mhd = from x in dutiesQuery
                             where x.DutyType == 1168 && x.BaseAirportId == 140870
                             orderby x.OrderIndex, x.ScheduleName
                             select x;

            var stbypm_thr = from x in dutiesQuery
                             where x.DutyType == 1167 && x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbypm_mhd = from x in dutiesQuery
                             where x.DutyType == 1167 && x.BaseAirportId == 140870
                             orderby x.OrderIndex, x.ScheduleName
                             select x;

            var am_sccm_thr = stbyam_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM").ToList();
            var am_sccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM").ToList();
            var pm_sccm_thr = stbypm_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM").ToList();
            var pm_sccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM").ToList();


            var am_ccm_thr = stbyam_thr.Where(q => q.JobGroup == "CCM").ToList();
            var am_ccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "CCM").ToList();
            var pm_ccm_thr = stbypm_thr.Where(q => q.JobGroup == "CCM").ToList();
            var pm_ccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "CCM").ToList();


            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drnew" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[2, 9].Text = ((DateTime)df).ToString("yyyy-MMM-dd");
            sheet.Range[3, 9].Text = ((DateTime)df).ToString("dddd");

            var regcnt = 0;
            var ln = 8;
            foreach (var line in regs)
            {
                foreach (var flt in line.Items)
                {
                    sheet.InsertRow(ln);
                    var rowHeight = sheet.Rows[ln - 1].RowHeight;
                    var rh = 25;
                    sheet.Rows[ln - 1].RowHeight = rh;
                    sheet.Rows[ln - 1].Style.Font.Size = 14;
                    sheet.Rows[ln - 1].Style.VerticalAlignment = VerticalAlignType.Center;

                    sheet.Range[ln, 2].Text = flt.FlightNumber;
                    sheet.Range[ln, 3].Text = flt.Register;

                    sheet.Range[ln, 4].Text = flt.FromAirportIATA;
                    sheet.Range[ln, 5].Text = flt.ToAirportIATA;

                    sheet.Range[ln, 6].Text = flt.DepLocal;
                    sheet.Range[ln, 7].Text = flt.ArrLocal;
                    sheet.Range[ln, 8].Text = flt.Dep;

                    sheet.Range[ln, 9].Text = flt.Arr;
                    // if (string.IsNullOrEmpty(flt.SCCM))
                    //   flt.SCCM = "";

                    var isccms = !string.IsNullOrEmpty(flt.ISCCM) ? flt.ISCCM.Replace(" ", "").Split(',').ToList() : new List<string>();
                    if (!string.IsNullOrEmpty(flt.ISCCM))
                    {
                        //dodo
                        sheet.Range[ln, 10].Text = flt.ISCCM;
                        //sheet.Range["J" + (ln) + ":K" + (ln)].Merge();
                    }

                    var sccms = !string.IsNullOrEmpty(flt.SCCM) ? flt.SCCM.Replace(" ", "").Split(',').ToList() : new List<string>();
                    if (sccms.Count <= 2)
                    {
                        if (sccms.Count > 0)
                            sheet.Range[ln, 11].Text = sccms[0];
                        if (sccms.Count > 1)
                            sheet.Range[ln, 12].Text = sccms[1];
                        // else
                        //    sheet.Range["H" + (ln) + ":I" + (ln)].Merge();
                    }
                    else
                    {
                        //dodo
                        sheet.Range[ln, 11].Text = flt.SCCM;
                        sheet.Range["J" + (ln) + ":K" + (ln)].Merge();
                    }

                    var ccms = !string.IsNullOrEmpty(flt.CCM) ? flt.CCM.Replace(" ", "").Split(',').ToList() : new List<string>();
                    // if (ccms.Count <= 5)
                    if (ccms.Count <= 4)
                    {
                        if (ccms.Count > 0)
                            sheet.Range[ln, 13].Text = ccms[0];
                        if (ccms.Count > 1)
                            sheet.Range[ln, 14].Text = ccms[1];
                        if (ccms.Count > 2)
                            sheet.Range[ln, 15].Text = ccms[2];
                        if (ccms.Count > 3)
                            sheet.Range[ln, 16].Text = ccms[3];
                        //  if (ccms.Count > 4)
                        //  sheet.Range[ln, 16].Text = ccms[4];


                        // else
                        //    sheet.Range["H" + (ln) + ":I" + (ln)].Merge();
                    }
                    else
                    {
                        //dodo
                        sheet.Range[ln, 13].Text = flt.CCM;
                        sheet.Range["L" + (ln) + ":P" + (ln)].Merge();
                    }
                    sheet.Range[ln, 17].Text = string.IsNullOrEmpty(flt.OBSC) ? "" : flt.OBSC;
                    sheet.Range[ln, 18].Text = string.IsNullOrEmpty(flt.POSITIONINGCABIN) ? "" : flt.POSITIONINGCABIN;

                    sheet.Range["B" + ln + ":Q" + ln].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range["B" + ln + ":Q" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                    sheet.Range["B" + ln + ":Q" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                    ln++;

                }

                if (regcnt != regs.Count - 1)
                {
                    sheet.InsertRow(ln);

                    sheet.Range["B" + (ln) + ":Q" + ln].Style.Color = Color.Silver;
                }

                ln++;
                regcnt++;
            }
            sheet.Range[ln, 3].Text = string.Join(", ", trn.Select(q => q.ScheduleName));
            ln = ln + 2;


            sheet.Range[ln, 5].Text = string.Join(", ", am_sccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_sccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_ccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_ccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rsv_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", off_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rest_thr.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_sccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_sccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_ccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_ccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rsv_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", off_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rest_mhd.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", ground.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", sick.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", vac.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", office.Select(q => q.ScheduleName));

            //sheet.Range["B5:N"+ln].BorderInside(LineStyleType.Thin, Color.Black);
            //sheet.Range["B5:N" + ln].BorderAround(LineStyleType.Medium, Color.Black);
            var name = "dr-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }

        [Route("api/xls/roster/daily/kih")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSRosterDailyKIH(DateTime df)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            df = ((DateTime)df).Date;
            var dt = df.AddDays(1);
            var fdps = (from x in context.ViewFDPRests
                        where x.DutyType == 1165 && x.STDLocal >= df && x.STDLocal < dt
                        select x.Id).ToList();
            var fdpitems = (from x in context.FDPItems
                            where fdps.Contains(x.FDPId)
                            select x.FlightId).ToList();
            var query = from x in context.ReportRosters
                            //where x.STDDay == df
                        where fdpitems.Contains(x.ID)
                        orderby x.Register, x.STDx
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.ID,
                q.FlightNumber,
                q.Register,
                q.FromAirportIATA,
                Base = q.FromAirportIATA == "IKA" ? "THR" : q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.STDx,
                q.STAx,
                q.STDLocal,
                q.STALocal,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT
            }).ToList();
            var regs = (from x in main

                        group x by new { x.Register } into grp
                        select new
                        {
                            grp.Key.Register,
                            Items = grp.OrderBy(q => q.STDx).ToList(),
                            Base = grp.OrderBy(q => q.STDx).First().Base
                        }
                       ).OrderByDescending(q => q.Base).ToList();
            var dqs = context.ViewCrewDuties.Where(x => (df >= x.DateLocal && df <= x.DateLocal2)).ToList();
            var dutiesQuery = (from x in /*context.ViewCrewDuties*/ dqs
                               where (df >= x.DateLocal && df <= x.DateLocal2) && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001
                               || x.DutyType == 100003
                               || x.DutyType == 1166
                               || x.DutyType == 10000
                                || x.DutyType == 10001
                                 || x.DutyType == 100007

                                 || x.DutyType == 300009

                                 || x.DutyType == 1169

                                 || x.DutyType == 100002

                                   || x.DutyType == 100000

                                    || x.DutyType == 5000

                               )
                               select x).ToList();

            var off = dutiesQuery.Where(q => (q.DutyType == 1166 || q.DutyType == 10000 || q.DutyType == 10001 || q.DutyType == 100007) && q.IsCockpit == 0).ToList();
            var off_mhd = off.Where(q => q.SMSId == 140857).ToList();
            var off_thr = off.Where(q => q.SMSId == 135502).ToList();
            var rest = dutiesQuery.Where(q => q.DutyType == 300009 && q.IsCockpit == 0).ToList();
            var rest_mhd = rest.Where(q => q.SMSId == 140857).ToList();
            var rest_thr = rest.Where(q => q.SMSId == 135502).ToList();
            var rsv = dutiesQuery.Where(q => q.DutyType == 1170 && q.IsCockpit == 0).ToList();
            var rsv_mhd = rsv.Where(q => q.SMSId == 140857).ToList();
            var rsv_thr = rsv.Where(q => q.SMSId == 135502).ToList();
            var vac = dutiesQuery.Where(q => q.DutyType == 1169 && q.IsCockpit == 0).ToList();
            var sick = dutiesQuery.Where(q => q.DutyType == 100002 && q.IsCockpit == 0).ToList();
            var ground = dutiesQuery.Where(q => q.DutyType == 100000 && q.IsCockpit == 0).ToList();
            var trn = dutiesQuery.Where(q => q.DutyType == 5000 && q.IsCockpit == 0).ToList();
            var office = dutiesQuery.Where(q => q.DutyType == 5001 && q.IsCockpit == 0).ToList();
            //140870
            var stbyam_thr = from x in dutiesQuery
                             where x.DutyType == 1168 && x.SMSId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbyam_mhd = from x in dutiesQuery
                             where x.DutyType == 1168 && x.SMSId == 140857
                             orderby x.OrderIndex, x.ScheduleName
                             select x;

            var stbypm_thr = from x in dutiesQuery
                             where x.DutyType == 1167 && x.SMSId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbypm_mhd = from x in dutiesQuery
                             where x.DutyType == 1167 && x.SMSId == 140857
                             orderby x.OrderIndex, x.ScheduleName
                             select x;

            var am_sccm_thr = stbyam_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM").ToList();
            var am_sccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM").ToList();
            var pm_sccm_thr = stbypm_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM").ToList();
            var pm_sccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM").ToList();


            var am_ccm_thr = stbyam_thr.Where(q => q.JobGroup == "CCM").ToList();
            var am_ccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "CCM").ToList();
            var pm_ccm_thr = stbypm_thr.Where(q => q.JobGroup == "CCM").ToList();
            var pm_ccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "CCM").ToList();


            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drnew" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[2, 9].Text = ((DateTime)df).ToString("yyyy-MMM-dd");
            sheet.Range[3, 9].Text = ((DateTime)df).ToString("dddd");

            var regcnt = 0;
            var ln = 8;
            foreach (var line in regs)
            {
                foreach (var flt in line.Items)
                {
                    sheet.InsertRow(ln);
                    var rowHeight = sheet.Rows[ln - 1].RowHeight;
                    var rh = 25;
                    sheet.Rows[ln - 1].RowHeight = rh;
                    sheet.Rows[ln - 1].Style.Font.Size = 14;
                    sheet.Rows[ln - 1].Style.VerticalAlignment = VerticalAlignType.Center;

                    sheet.Range[ln, 2].Text = flt.FlightNumber;
                    sheet.Range[ln, 3].Text = flt.Register;

                    sheet.Range[ln, 4].Text = flt.FromAirportIATA;
                    sheet.Range[ln, 5].Text = flt.ToAirportIATA;

                    sheet.Range[ln, 6].Text = flt.DepLocal;
                    sheet.Range[ln, 7].Text = flt.ArrLocal;
                    sheet.Range[ln, 8].Text = flt.Dep;

                    sheet.Range[ln, 9].Text = flt.Arr;
                    // if (string.IsNullOrEmpty(flt.SCCM))
                    //   flt.SCCM = "";

                    var isccms = !string.IsNullOrEmpty(flt.ISCCM) ? flt.ISCCM.Replace(" ", "").Split(',').ToList() : new List<string>();
                    if (!string.IsNullOrEmpty(flt.ISCCM))
                    {
                        //dodo
                        sheet.Range[ln, 10].Text = flt.ISCCM;
                        //sheet.Range["J" + (ln) + ":K" + (ln)].Merge();
                    }

                    var sccms = !string.IsNullOrEmpty(flt.SCCM) ? flt.SCCM.Replace(" ", "").Split(',').ToList() : new List<string>();
                    if (sccms.Count <= 2)
                    {
                        if (sccms.Count > 0)
                            sheet.Range[ln, 11].Text = sccms[0];
                        if (sccms.Count > 1)
                            sheet.Range[ln, 12].Text = sccms[1];
                        // else
                        //    sheet.Range["H" + (ln) + ":I" + (ln)].Merge();
                    }
                    else
                    {
                        //dodo
                        sheet.Range[ln, 11].Text = flt.SCCM;
                        sheet.Range["J" + (ln) + ":K" + (ln)].Merge();
                    }

                    var ccms = !string.IsNullOrEmpty(flt.CCM) ? flt.CCM.Replace(" ", "").Split(',').ToList() : new List<string>();
                    // if (ccms.Count <= 5)
                    if (ccms.Count <= 4)
                    {
                        if (ccms.Count > 0)
                            sheet.Range[ln, 13].Text = ccms[0];
                        if (ccms.Count > 1)
                            sheet.Range[ln, 14].Text = ccms[1];
                        if (ccms.Count > 2)
                            sheet.Range[ln, 15].Text = ccms[2];
                        if (ccms.Count > 3)
                            sheet.Range[ln, 16].Text = ccms[3];
                        //  if (ccms.Count > 4)
                        //  sheet.Range[ln, 16].Text = ccms[4];


                        // else
                        //    sheet.Range["H" + (ln) + ":I" + (ln)].Merge();
                    }
                    else
                    {
                        //dodo
                        sheet.Range[ln, 13].Text = flt.CCM;
                        sheet.Range["L" + (ln) + ":P" + (ln)].Merge();
                    }
                    sheet.Range[ln, 17].Text = string.IsNullOrEmpty(flt.OBSC) ? "" : flt.OBSC;
                    sheet.Range[ln, 18].Text = string.IsNullOrEmpty(flt.POSITIONINGCABIN) ? "" : flt.POSITIONINGCABIN;

                    sheet.Range["B" + ln + ":Q" + ln].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range["B" + ln + ":Q" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                    sheet.Range["B" + ln + ":Q" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                    ln++;

                }

                if (regcnt != regs.Count - 1)
                {
                    sheet.InsertRow(ln);

                    sheet.Range["B" + (ln) + ":Q" + ln].Style.Color = Color.Silver;
                }

                ln++;
                regcnt++;
            }
            sheet.Range[ln, 3].Text = string.Join(", ", trn.Select(q => q.ScheduleName));
            ln = ln + 2;


            sheet.Range[ln, 5].Text = string.Join(", ", am_sccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_sccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_ccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_ccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rsv_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", off_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rest_thr.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_sccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_sccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_ccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_ccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rsv_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", off_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rest_mhd.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", ground.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", sick.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", vac.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", office.Select(q => q.ScheduleName));

            //sheet.Range["B5:N"+ln].BorderInside(LineStyleType.Thin, Color.Black);
            //sheet.Range["B5:N" + ln].BorderAround(LineStyleType.Medium, Color.Black);
            var name = "dr-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }

        //05-19
        [Route("api/xls/roster/cockpit/daily")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSRosterCocpitDaily(DateTime df)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            context.Database.CommandTimeout = 500;
            df = ((DateTime)df).Date;
            var dt = df.AddDays(1);
            var fdps = (from x in context.ViewFDPRests
                        where x.DutyType == 1165 && x.STDLocal >= df && x.STDLocal < dt
                        select x.Id).ToList();
            var fdpitems = (from x in context.FDPItems
                            where fdps.Contains(x.FDPId)
                            select x.FlightId).ToList();
            var query = from x in context.ReportRosters
                            //where x.STDDay == df
                        where fdpitems.Contains(x.ID)
                        orderby x.Register, x.STDx
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.ID,
                q.FlightNumber,
                q.Register,
                q.FromAirportIATA,
                Base = q.FromAirportIATA == "IKA" ? "THR" : q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.STDx,
                q.STAx,
                q.STDLocal,
                q.STALocal,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT
            }).ToList();
            var regs = (from x in main

                        group x by new { x.Register } into grp
                        select new
                        {
                            grp.Key.Register,
                            Items = grp.OrderBy(q => q.STDx).ToList(),
                            Base = grp.OrderBy(q => q.STDx).First().Base
                        }
                       ).OrderByDescending(q => q.Base).ToList();
            var dqs = context.ViewCrewDuties.Where(x => (df >= x.DateLocal && df <= x.DateLocal2)).ToList();

            var dutiesQuery = (from x in /*context.ViewCrewDuties*/ dqs
                               where (df >= x.DateLocal && df <= x.DateLocal2) && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001
                               || x.DutyType == 100003
                               || x.DutyType == 1166
                               || x.DutyType == 10000
                                || x.DutyType == 10001
                                 || x.DutyType == 100007

                                 || x.DutyType == 300009

                                 || x.DutyType == 1169

                                 || x.DutyType == 100002

                                   || x.DutyType == 100000

                                    || x.DutyType == 5000

                                    || x.DutyType == 100003

                               )
                               select x).ToList();

            var off = dutiesQuery.Where(q => (q.DutyType == 1166 || q.DutyType == 10000 || q.DutyType == 10001 || q.DutyType == 100007) && q.IsCockpit == 1).ToList();
            var off_mhd = off.Where(q => q.BaseAirportId == 140870).ToList();
            var off_thr = off.Where(q => q.BaseAirportId == 135502).ToList();
            var rest = dutiesQuery.Where(q => q.DutyType == 300009 && q.IsCockpit == 1).ToList();
            var rest_mhd = rest.Where(q => q.BaseAirportId == 140870).ToList();
            var rest_thr = rest.Where(q => q.BaseAirportId == 135502).ToList();
            var rsv = dutiesQuery.Where(q => q.DutyType == 1170 && q.IsCockpit == 1).ToList();
            var rsv_mhd = rsv.Where(q => q.BaseAirportId == 140870).ToList();
            var rsv_thr = rsv.Where(q => q.BaseAirportId == 135502).ToList();
            var vac = dutiesQuery.Where(q => q.DutyType == 1169 && q.IsCockpit == 1).ToList();
            var sick = dutiesQuery.Where(q => q.DutyType == 100002 && q.IsCockpit == 1).ToList();
            var ground = dutiesQuery.Where(q => q.DutyType == 100000 && q.IsCockpit == 1).ToList();
            var trn = dutiesQuery.Where(q => q.DutyType == 5000 && q.IsCockpit == 1).ToList();
            var sim = dutiesQuery.Where(q => q.DutyType == 100003 && q.IsCockpit == 1).ToList();
            var office = dutiesQuery.Where(q => q.DutyType == 5001 && q.IsCockpit == 1).ToList();
            //140870
            var stbyam_thr = from x in dutiesQuery
                             where x.DutyType == 1168 && x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbyam_mhd = from x in dutiesQuery
                             where x.DutyType == 1168 && x.BaseAirportId == 140870
                             orderby x.OrderIndex, x.ScheduleName
                             select x;

            var stbypm_thr = from x in dutiesQuery
                             where x.DutyType == 1167 && x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbypm_mhd = from x in dutiesQuery
                             where x.DutyType == 1167 && x.BaseAirportId == 140870
                             orderby x.OrderIndex, x.ScheduleName
                             select x;

            var am_sccm_thr = stbyam_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1").ToList();
            var am_sccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1").ToList();
            var pm_sccm_thr = stbypm_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1").ToList();
            var pm_sccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1").ToList();


            var am_ccm_thr = stbyam_thr.Where(q => q.JobGroup == "P2").ToList();
            var am_ccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "P2").ToList();
            var pm_ccm_thr = stbypm_thr.Where(q => q.JobGroup == "P2").ToList();
            var pm_ccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "P2").ToList();


            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drcnew" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[2, 9].Text = ((DateTime)df).ToString("yyyy-MMM-dd");
            sheet.Range[3, 9].Text = ((DateTime)df).ToString("dddd");

            var regcnt = 0;
            var ln = 8;
            foreach (var line in regs)
            {
                foreach (var flt in line.Items)
                {
                    sheet.InsertRow(ln);
                    var rowHeight = sheet.Rows[ln - 1].RowHeight;
                    var rh = 25;
                    sheet.Rows[ln - 1].RowHeight = rh;
                    sheet.Rows[ln - 1].Style.Font.Size = 14;
                    sheet.Rows[ln - 1].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet.Range[ln, 2].Text = flt.FlightNumber;


                    sheet.Range[ln, 3].Text = flt.Register;

                    sheet.Range[ln, 4].Text = flt.FromAirportIATA;
                    sheet.Range[ln, 5].Text = flt.ToAirportIATA;

                    sheet.Range[ln, 6].Text = flt.DepLocal;
                    sheet.Range[ln, 7].Text = flt.ArrLocal;
                    sheet.Range[ln, 8].Text = flt.Dep;

                    sheet.Range[ln, 9].Text = flt.Arr;


                    sheet.Range[ln, 10].Text = string.IsNullOrEmpty(flt.IP) ? "" : flt.IP;

                    sheet.Range[ln, 11].Text = string.IsNullOrEmpty(flt.CPT) ? "" : flt.CPT;

                    sheet.Range[ln, 12].Text = string.IsNullOrEmpty(flt.FO) ? "" : flt.FO;

                    sheet.Range[ln, 13].Text = string.IsNullOrEmpty(flt.SAFETY) ? "" : flt.SAFETY;

                    sheet.Range[ln, 14].Text = string.IsNullOrEmpty(flt.CHECK) ? "" : flt.CHECK;

                    sheet.Range[ln, 15].Text = string.IsNullOrEmpty(flt.OBS) ? "" : flt.OBS;
                    sheet.Range[ln, 16].Text = string.IsNullOrEmpty(flt.POSITIONINGCOCKPIT) ? "" : flt.POSITIONINGCOCKPIT;


                    sheet.Range["B" + ln + ":P" + ln].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range["B" + ln + ":P" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                    sheet.Range["B" + ln + ":P" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                    ln++;

                }

                if (regcnt != regs.Count - 1)
                {
                    sheet.InsertRow(ln);

                    sheet.Range["B" + (ln) + ":P" + ln].Style.Color = Color.Silver;
                }

                ln++;
                regcnt++;
            }
            sheet.Range[ln, 3].Text = string.Join(", ", trn.Select(q => q.ScheduleName));
            ln = ln + 2;


            sheet.Range[ln, 5].Text = string.Join(", ", am_sccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_sccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_ccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_ccm_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rsv_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", off_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rest_thr.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_sccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_sccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_ccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_ccm_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rsv_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", off_mhd.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rest_mhd.Select(q => q.ScheduleName));

            ln++;
            //  ln = ln + 7;
            sheet.Range[ln, 5].Text = string.Join(", ", ground.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", sick.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", vac.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", sim.Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", office.Select(q => q.ScheduleName));

            // sheet.Range["B5:N" + ln].BorderInside(LineStyleType.Thin, Color.Black);
            //sheet.Range["B5:N" + ln].BorderAround(LineStyleType.Medium, Color.Black);
            var name = "drc-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }




        [Route("api/xls/roster/cockpit/daily/kih")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSRosterCocpitDailyKIH(DateTime df)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            context.Database.CommandTimeout = 500;
            df = ((DateTime)df).Date;
            var dt = df.AddDays(1);
            var fdps = (from x in context.ViewFDPRests
                        where x.DutyType == 1165 && x.STDLocal >= df && x.STDLocal < dt
                        select x.Id).ToList();
            var fdpitems = (from x in context.FDPItems
                            where fdps.Contains(x.FDPId)
                            select x.FlightId).ToList();
            var query = from x in context.ReportRosters
                            //where x.STDDay == df
                        where fdpitems.Contains(x.ID)
                        orderby x.Register, x.STDx
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.ID,
                q.FlightNumber,
                q.Register,
                q.FromAirportIATA,
                Base = q.FromAirportIATA == "IKA" ? "THR" : q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.STDx,
                q.STAx,
                q.STDLocal,
                q.STALocal,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT
            }).ToList();
            var regs = (from x in main

                        group x by new { x.Register } into grp
                        select new
                        {
                            grp.Key.Register,
                            Items = grp.OrderBy(q => q.STDx).ToList(),
                            Base = grp.OrderBy(q => q.STDx).First().Base
                        }
                       ).OrderByDescending(q => q.Base).ToList();
            var dqs = context.ViewCrewDuties.Where(x => (df >= x.DateLocal && df <= x.DateLocal2)).ToList();

            var dutiesQuery = (from x in /*context.ViewCrewDuties*/ dqs
                               where (df >= x.DateLocal && df <= x.DateLocal2) && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001
                               || x.DutyType == 100003
                               || x.DutyType == 1166
                               || x.DutyType == 10000
                                || x.DutyType == 10001
                                 || x.DutyType == 100007

                                 || x.DutyType == 300009

                                 || x.DutyType == 1169

                                 || x.DutyType == 100002

                                   || x.DutyType == 100000

                                    || x.DutyType == 5000

                                    || x.DutyType == 100003

                               )
                               select x).ToList();

            var off = dutiesQuery.Where(q => (q.DutyType == 1166 || q.DutyType == 10000 || q.DutyType == 10001 || q.DutyType == 100007)).ToList();
            var off_mhd = off.Where(q => q.SMSId == 140857).ToList();
            var off_thr = off.Where(q => q.SMSId == 135502).ToList();
            var rest = dutiesQuery.Where(q => q.DutyType == 300009).ToList();
            var rest_mhd = rest.Where(q => q.SMSId == 140857).ToList();
            var rest_thr = rest.Where(q => q.SMSId == 135502).ToList();
            var rsv = dutiesQuery.Where(q => q.DutyType == 1170).ToList();
            var rsv_mhd = rsv.Where(q => q.SMSId == 140857).ToList();
            var rsv_thr = rsv.Where(q => q.SMSId == 135502).ToList();
            var vac = dutiesQuery.Where(q => q.DutyType == 1169).ToList();
            var sick = dutiesQuery.Where(q => q.DutyType == 100002).ToList();
            var ground = dutiesQuery.Where(q => q.DutyType == 100000).ToList();
            var trn = dutiesQuery.Where(q => q.DutyType == 5000).ToList();
            var sim = dutiesQuery.Where(q => q.DutyType == 100003).ToList();
            var office = dutiesQuery.Where(q => q.DutyType == 5001).ToList();
            //140870
            var stbyam_thr = from x in dutiesQuery
                             where x.DutyType == 1168 && x.SMSId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbyam_mhd = from x in dutiesQuery
                             where x.DutyType == 1168 && x.SMSId == 140857
                             orderby x.OrderIndex, x.ScheduleName
                             select x;

            var stbypm_thr = from x in dutiesQuery
                             where x.DutyType == 1167 && x.SMSId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbypm_mhd = from x in dutiesQuery
                             where x.DutyType == 1167 && x.SMSId == 140857
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            //12-26
            var am_cockpit_thr = stbyam_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").OrderBy(q => getOrder(q.JobGroup)).ToList();
            var am_cockpit_mhd = stbyam_mhd.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").OrderBy(q => getOrder(q.JobGroup)).ToList();
            var pm_cockpit_thr = stbypm_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").OrderBy(q => getOrder(q.JobGroup)).ToList();
            var pm_cockpit_mhd = stbypm_mhd.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").OrderBy(q => getOrder(q.JobGroup)).ToList();


            // var am_p2_thr = stbyam_thr.Where(q => q.JobGroup == "P2").ToList();
            //  var am_p2_mhd = stbyam_mhd.Where(q => q.JobGroup == "P2").ToList();
            //  var pm_p2_thr = stbypm_thr.Where(q => q.JobGroup == "P2").ToList();
            //  var pm_p2_mhd = stbypm_mhd.Where(q => q.JobGroup == "P2").ToList();



            var am_cabin_thr = stbyam_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").OrderBy(q => getOrder(q.JobGroup)).ToList();
            var am_cabin_mhd = stbyam_mhd.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").OrderBy(q => getOrder(q.JobGroup)).ToList();
            var pm_cabin_thr = stbypm_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").OrderBy(q => getOrder(q.JobGroup)).ToList();
            var pm_cabin_mhd = stbypm_mhd.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").OrderBy(q => getOrder(q.JobGroup)).ToList();


            //  var am_ccm_thr = stbyam_thr.Where(q => q.JobGroup == "CCM").ToList();
            //  var am_ccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "CCM").ToList();
            //  var pm_ccm_thr = stbypm_thr.Where(q => q.JobGroup == "CCM").ToList();
            //  var pm_ccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "CCM").ToList();


            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drcnewkih" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[2, 9].Text = ((DateTime)df).ToString("yyyy-MMM-dd");
            sheet.Range[3, 9].Text = ((DateTime)df).ToString("dddd");

            var regcnt = 0;
            var ln = 8;
            foreach (var line in regs)
            {
                foreach (var flt in line.Items)
                {
                    sheet.InsertRow(ln);
                    var rowHeight = sheet.Rows[ln - 1].RowHeight;
                    var rh = 25;
                    sheet.Rows[ln - 1].RowHeight = rh;
                    sheet.Rows[ln - 1].Style.Font.Size = 14;
                    sheet.Rows[ln - 1].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet.Range[ln, 2].Text = flt.FlightNumber;


                    sheet.Range[ln, 3].Text = flt.Register;

                    sheet.Range[ln, 4].Text = flt.FromAirportIATA;
                    sheet.Range[ln, 5].Text = flt.ToAirportIATA;

                    sheet.Range[ln, 6].Text = flt.DepLocal;
                    sheet.Range[ln, 7].Text = flt.ArrLocal;
                    sheet.Range[ln, 8].Text = flt.Dep;

                    sheet.Range[ln, 9].Text = flt.Arr;


                    sheet.Range[ln, 10].Text = string.IsNullOrEmpty(flt.IP) ? "" : flt.IP;

                    sheet.Range[ln, 11].Text = string.IsNullOrEmpty(flt.CPT) ? "" : flt.CPT;

                    sheet.Range[ln, 12].Text = string.IsNullOrEmpty(flt.FO) ? "" : flt.FO;

                    sheet.Range[ln, 13].Text = string.IsNullOrEmpty(flt.SAFETY) ? "" : flt.SAFETY;

                    sheet.Range[ln, 14].Text = string.IsNullOrEmpty(flt.CHECK) ? "" : flt.CHECK;

                    sheet.Range[ln, 15].Text = string.IsNullOrEmpty(flt.OBS) ? "" : flt.OBS;

                    sheet.Range[ln, 16].Text = string.IsNullOrEmpty(flt.ISCCM) ? "" : flt.ISCCM;

                    sheet.Range[ln, 17].Text = string.IsNullOrEmpty(flt.SCCM) ? "" : flt.SCCM;

                    sheet.Range[ln, 18].Text = string.IsNullOrEmpty(flt.CCM) ? "" : flt.CCM;
                    sheet.Range[ln, 19].Text = string.IsNullOrEmpty(flt.OBSC) ? "" : flt.OBSC;

                    var _pos = new List<string>();
                    if (!string.IsNullOrEmpty(flt.POSITIONINGCOCKPIT))
                        _pos.Add(flt.POSITIONINGCOCKPIT);
                    if (!string.IsNullOrEmpty(flt.POSITIONINGCABIN))
                        _pos.Add(flt.POSITIONINGCABIN);

                    sheet.Range[ln, 20].Text = _pos.Count == 0 ? "" : string.Join(",", _pos);


                    sheet.Range["B" + ln + ":T" + ln].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range["B" + ln + ":T" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                    sheet.Range["B" + ln + ":T" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                    ln++;

                }

                if (regcnt != regs.Count - 1)
                {
                    sheet.InsertRow(ln);

                    sheet.Range["B" + (ln) + ":T" + ln].Style.Color = Color.Silver;
                }

                ln++;
                regcnt++;
            }
            sheet.Range[ln, 3].Text = string.Join(", ", trn.OrderBy(q => getOrder(q.JobGroup)).Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln = ln + 2;


            sheet.Range[ln, 6].Text = string.Join(", ", am_cockpit_thr.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", am_cabin_thr.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", pm_cockpit_thr.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", pm_cabin_thr.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", rsv_thr.OrderBy(q => getOrder(q.JobGroup)).Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;


            sheet.Range[ln, 6].Text = string.Join(", ", am_cockpit_mhd.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", am_cabin_mhd.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", pm_cockpit_mhd.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", pm_cabin_mhd.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", rsv_mhd.OrderBy(q => getOrder(q.JobGroup)).Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;




            sheet.Range[ln, 6].Text = "N/A";//string.Join(", ", off_thr.Select(q => q.ScheduleName));
            ln++;
            sheet.Range[ln, 6].Text = "N/A";//string.Join(", ", rest_thr.Select(q => q.ScheduleName));

            ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", am_sccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", pm_sccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", am_ccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", pm_ccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", rsv_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", off_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", rest_mhd.Select(q => q.ScheduleName));

            //ln++;
            //  ln = ln + 7;
            sheet.Range[ln, 6].Text = string.Join(", ", ground.OrderBy(q => getOrder(q.JobGroup)).Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", sick.OrderBy(q => getOrder(q.JobGroup)).Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", vac.OrderBy(q => getOrder(q.JobGroup)).Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", sim.OrderBy(q => getOrder(q.JobGroup)).Select(q => q.ScheduleName));

            ln++;
            sheet.Range[ln, 6].Text = string.Join(", ", office.OrderBy(q => getOrder(q.JobGroup)).Select(q => q.ScheduleName));

            // sheet.Range["B5:N" + ln].BorderInside(LineStyleType.Thin, Color.Black);
            //sheet.Range["B5:N" + ln].BorderAround(LineStyleType.Medium, Color.Black);
            var name = "drc-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }


        [Route("api/xls/roster/all/daily")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSRosterAllDaily(DateTime df)
        {
            //bolan
            var context = new AirpocketAPI.Models.FLYEntities();
            context.Database.CommandTimeout = 500;
            df = ((DateTime)df).Date;
            var dt = df.AddDays(1);
            var fdps = (from x in context.ViewFDPRests
                        where x.DutyType == 1165 && x.STDLocal >= df && x.STDLocal < dt
                        select x.Id).ToList();
            var fdpitems = (from x in context.FDPItems
                            where fdps.Contains(x.FDPId)
                            select x.FlightId).ToList();
            var query = from x in context.ReportRosters
                            //where x.STDDay == df
                        where fdpitems.Contains(x.ID)
                        orderby x.Register, x.STDx
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.ID,
                q.FlightNumber,
                q.Register,
                q.FromAirportIATA,
                Base = q.FromAirportIATA == "IKA" ? "THR" : q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.STDx,
                q.STAx,
                q.STDLocal,
                q.STALocal,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT
            }).ToList();
            var regs = (from x in main

                        group x by new { x.Register } into grp
                        select new
                        {
                            grp.Key.Register,
                            Items = grp.OrderBy(q => q.STDx).ToList(),
                            Base = grp.OrderBy(q => q.STDx).First().Base
                        }
                       ).OrderByDescending(q => q.Base).ToList();
            var dqs = context.ViewCrewDuties.Where(x => (df >= x.DateLocal && df <= x.DateLocal2)).ToList();

            var dutiesQuery = (from x in /*context.ViewCrewDuties*/ dqs
                               where (df >= x.DateLocal && df <= x.DateLocal2) && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001
                               || x.DutyType == 100003
                               || x.DutyType == 1166
                               || x.DutyType == 10000
                                || x.DutyType == 10001
                                 || x.DutyType == 100007

                                 || x.DutyType == 300009

                                 || x.DutyType == 1169

                                 || x.DutyType == 100002

                                   || x.DutyType == 100000

                                    || x.DutyType == 5000

                                    || x.DutyType == 100003

                               )
                               select x).ToList();

            var off = dutiesQuery.Where(q => (q.DutyType == 1166 || q.DutyType == 10000 || q.DutyType == 10001 || q.DutyType == 100007) && q.IsCockpit == 1).ToList();
            var off_mhd = off.Where(q => q.BaseAirportId == 140870).ToList();
            var off_thr = off.Where(q => q.BaseAirportId == 135502).ToList();
            var rest = dutiesQuery.Where(q => q.DutyType == 300009 && q.IsCockpit == 1).ToList();
            var rest_mhd = rest.Where(q => q.BaseAirportId == 140870).ToList();
            var rest_thr = rest.Where(q => q.BaseAirportId == 135502).ToList();
            var rsv = dutiesQuery.Where(q => q.DutyType == 1170 && q.IsCockpit == 1).ToList();
            var rsv_mhd = rsv.Where(q => q.BaseAirportId == 140870).ToList();
            var rsv_thr = rsv.Where(q => q.BaseAirportId == 135502).ToList();
            var vac = dutiesQuery.Where(q => q.DutyType == 1169 && q.IsCockpit == 1).ToList();
            var sick = dutiesQuery.Where(q => q.DutyType == 100002 && q.IsCockpit == 1).ToList();
            var ground = dutiesQuery.Where(q => q.DutyType == 100000 && q.IsCockpit == 1).ToList();
            var trn = dutiesQuery.Where(q => q.DutyType == 5000 && q.IsCockpit == 1).ToList();
            var sim = dutiesQuery.Where(q => q.DutyType == 100003 && q.IsCockpit == 1).ToList();
            var office = dutiesQuery.Where(q => q.DutyType == 5001 && q.IsCockpit == 1).ToList();
            //140870
            var stbyam_thr = from x in dutiesQuery
                             where x.DutyType == 1168 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            /*var stbyam_mhd = from x in dutiesQuery
                             where x.DutyType == 1168 && x.BaseAirportId == 140870
                             orderby x.OrderIndex, x.ScheduleName
                             select x;*/

            var stbypm_thr = from x in dutiesQuery
                             where x.DutyType == 1167 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            /* var stbypm_mhd = from x in dutiesQuery
                              where x.DutyType == 1167 && x.BaseAirportId == 140870
                              orderby x.OrderIndex, x.ScheduleName
                              select x;*/

            var am_cockpit = stbyam_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            //  var am_sccm_mhd = stbyam_mhd/*.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1")*/.ToList();
            var pm_cockpit = stbypm_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            // var pm_sccm_mhd = stbypm_mhd/*.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1")*/.ToList();


            //  var am_ccm_thr = stbyam_thr.Where(q => q.JobGroup == "P2").ToList();
            // var am_ccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "P2").ToList();
            //var pm_ccm_thr = stbypm_thr.Where(q => q.JobGroup == "P2").ToList();
            // var pm_ccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "P2").ToList();
            var am_cabin = stbyam_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();
            var pm_cabin = stbypm_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drcnew" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[2, 9].Text = ((DateTime)df).ToString("yyyy-MMM-dd");
            sheet.Range[3, 9].Text = ((DateTime)df).ToString("dddd");

            var regcnt = 0;
            var ln = 8;
            foreach (var line in regs)
            {
                foreach (var flt in line.Items)
                {
                    sheet.InsertRow(ln);
                    var rowHeight = sheet.Rows[ln - 1].RowHeight;
                    var rh = 25;
                    sheet.Rows[ln - 1].RowHeight = rh;
                    sheet.Rows[ln - 1].Style.Font.Size = 14;
                    sheet.Rows[ln - 1].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet.Range[ln, 2].Text = flt.FlightNumber;


                    sheet.Range[ln, 3].Text = flt.Register;

                    sheet.Range[ln, 4].Text = flt.FromAirportIATA;
                    sheet.Range[ln, 5].Text = flt.ToAirportIATA;

                    sheet.Range[ln, 6].Text = flt.DepLocal;
                    sheet.Range[ln, 7].Text = flt.ArrLocal;
                    sheet.Range[ln, 8].Text = flt.Dep;

                    sheet.Range[ln, 9].Text = flt.Arr;


                    sheet.Range[ln, 10].Text = string.IsNullOrEmpty(flt.IP) ? "" : flt.IP;

                    sheet.Range[ln, 11].Text = string.IsNullOrEmpty(flt.CPT) ? "" : flt.CPT;

                    sheet.Range[ln, 12].Text = string.IsNullOrEmpty(flt.FO) ? "" : flt.FO;

                    sheet.Range[ln, 13].Text = string.IsNullOrEmpty(flt.SAFETY) ? "" : flt.SAFETY;
                    sheet.Range[ln, 13].AutoFitColumns();

                    sheet.Range[ln, 14].Text = string.IsNullOrEmpty(flt.CHECK) ? "" : flt.CHECK;
                    sheet.Range[ln, 14].AutoFitColumns();

                    sheet.Range[ln, 15].Text = string.IsNullOrEmpty(flt.OBS) ? "" : flt.OBS;
                    sheet.Range[ln, 15].AutoFitColumns();

                    //16 IFP
                    sheet.Range[ln, 16].Text = string.IsNullOrEmpty(flt.ISCCM) ? "" : flt.ISCCM;
                    sheet.Range[ln, 16].AutoFitColumns();
                    //17 FP
                    sheet.Range[ln, 17].Text = string.IsNullOrEmpty(flt.SCCM) ? "" : flt.SCCM;
                    //18 FA
                    sheet.Range[ln, 18].Text = string.IsNullOrEmpty(flt.CCM) ? "" : flt.CCM;
                    sheet.Range[ln, 18].AutoFitColumns();
                    //19 check
                    //20 obsc
                    sheet.Range[ln, 19].Text = string.IsNullOrEmpty(flt.OBSC) ? "" : flt.OBSC;
                    sheet.Range[ln, 19].AutoFitColumns();
                    // sheet.Range[ln, 16].Text = string.IsNullOrEmpty(flt.POSITIONINGCOCKPIT) ? "" : flt.POSITIONINGCOCKPIT;




                    sheet.Range["B" + ln + ":U" + ln].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range["B" + ln + ":U" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                    sheet.Range["B" + ln + ":U" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                    ln++;

                }

                if (regcnt != regs.Count - 1)
                {
                    sheet.InsertRow(ln);

                    sheet.Range["B" + (ln) + ":U" + ln].Style.Color = Color.Silver;
                }

                ln++;
                regcnt++;
            }
            sheet.Range[ln, 3].Text = string.Join(", ", trn.Select(q => q.ScheduleName).Distinct());
            ln = ln + 2;


            sheet.Range[ln, 5].Text = string.Join(", ", am_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rsv.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", off.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rest.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", am_sccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", pm_sccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", am_ccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", pm_ccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", rsv_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", off_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", rest_mhd.Select(q => q.ScheduleName));

            //ln++;
            //  ln = ln + 7;
            sheet.Range[ln, 5].Text = string.Join(", ", ground.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", sick.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", vac.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", sim.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", office.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            // sheet.Range["B5:N" + ln].BorderInside(LineStyleType.Thin, Color.Black);
            //sheet.Range["B5:N" + ln].BorderAround(LineStyleType.Medium, Color.Black);
            var name = "drc-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }

        //Form Number DR-02
        [Route("api/xls/roster/all/daily/dr02")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSRosterAllDailyDr02(DateTime df, int rev)
        {
            //bolan
            var context = new AirpocketAPI.Models.FLYEntities();
            context.Database.CommandTimeout = 500;
            df = ((DateTime)df).Date;
            var dt = df.AddDays(1);
            var fdps = (from x in context.ViewFDPRests
                        where x.DutyType == 1165 && x.STDLocal >= df && x.STDLocal < dt
                        select x.Id).ToList();
            var fdpitems = (from x in context.FDPItems
                            where fdps.Contains(x.FDPId)
                            select x.FlightId).ToList();
            var query = from x in context.ReportRosters
                            //where x.STDDay == df
                        where fdpitems.Contains(x.ID)
                        orderby x.Register, x.STDx
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.ID,
                q.FlightNumber,
                q.Register,
                q.AircraftType,
                q.FromAirportIATA,
                Base = q.FromAirportIATA == "IKA" ? "THR" : q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.STDx,
                q.STAx,
                q.STDLocal,
                q.STALocal,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT,
                q.PDATE

            }).ToList();



            var regs = (from x in main

                        group x by new { x.Register, x.AircraftType } into grp
                        select new
                        {
                            grp.Key.Register,
                            grp.Key.AircraftType,
                            Items = grp.OrderBy(q => q.STDx).ToList(),
                            Base = grp.OrderBy(q => q.STDx).First().Base
                        }
                       )
                       //.OrderByDescending(q => q.Base)
                       .OrderBy(q => q.AircraftType).ThenBy(q => q.Register)
                       .ToList();
            var dqs = context.ViewCrewDuties.Where(x => (df >= x.DateLocal && df <= x.DateLocal2)).ToList();

            var dutiesQuery = (from x in /*context.ViewCrewDuties*/ dqs
                               where (df >= x.DateLocal && df <= x.DateLocal2) && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001
                               || x.DutyType == 100003
                               || x.DutyType == 1166
                               || x.DutyType == 10000
                                || x.DutyType == 10001
                                 || x.DutyType == 100007

                                 || x.DutyType == 300009

                                 || x.DutyType == 1169

                                 || x.DutyType == 100002

                                   || x.DutyType == 100000

                                    || x.DutyType == 5000

                                    || x.DutyType == 100003
                                    || x.DutyType == 300013

                               )
                               select x).ToList();

            // var off = dutiesQuery.Where(q => (q.DutyType == 1166 || q.DutyType == 10000 || q.DutyType == 10001 || q.DutyType == 100007) && q.IsCockpit == 1).ToList();
            // var off_mhd = off.Where(q => q.BaseAirportId == 140870).ToList();
            // var off_thr = off.Where(q => q.BaseAirportId == 135502).ToList();
            // var rest = dutiesQuery.Where(q => q.DutyType == 300009 && q.IsCockpit == 1).ToList();
            // var rest_mhd = rest.Where(q => q.BaseAirportId == 140870).ToList();
            // var rest_thr = rest.Where(q => q.BaseAirportId == 135502).ToList();
            var rsv = dutiesQuery.Where(q => q.DutyType == 1170/* && q.IsCockpit == 1*/).ToList();
            // var rsv_mhd = rsv.Where(q => q.BaseAirportId == 140870).ToList();
            // var rsv_thr = rsv.Where(q => q.BaseAirportId == 135502).ToList();
            // var vac = dutiesQuery.Where(q => q.DutyType == 1169 && q.IsCockpit == 1).ToList();
            // var sick = dutiesQuery.Where(q => q.DutyType == 100002 && q.IsCockpit == 1).ToList();
            // var ground = dutiesQuery.Where(q => q.DutyType == 100000 && q.IsCockpit == 1).ToList();
            var trn = dutiesQuery.Where(q => q.DutyType == 5000 && q.IsCockpit == 1).ToList();
            // var sim = dutiesQuery.Where(q => q.DutyType == 100003 && q.IsCockpit == 1).ToList();
            // var office = dutiesQuery.Where(q => q.DutyType == 5001 && q.IsCockpit == 1).ToList();
            //140870
            var stbyam_thr = from x in dutiesQuery
                             where x.DutyType == 1168 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            /*var stbyam_mhd = from x in dutiesQuery
                             where x.DutyType == 1168 && x.BaseAirportId == 140870
                             orderby x.OrderIndex, x.ScheduleName
                             select x;*/

            var stbypm_thr = from x in dutiesQuery
                             where x.DutyType == 1167 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbydaily = from x in dutiesQuery
                            where x.DutyType == 300013 //&& x.BaseAirportId == 135502
                            orderby x.OrderIndex, x.ScheduleName
                            select x;
            /* var stbypm_mhd = from x in dutiesQuery
                              where x.DutyType == 1167 && x.BaseAirportId == 140870
                              orderby x.OrderIndex, x.ScheduleName
                              select x;*/

            var am_cockpit = stbyam_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            //  var am_sccm_mhd = stbyam_mhd/*.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1")*/.ToList();
            var pm_cockpit = stbypm_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            var daily_cockpit = stbydaily.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();

            // var pm_sccm_mhd = stbypm_mhd/*.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1")*/.ToList();


            //  var am_ccm_thr = stbyam_thr.Where(q => q.JobGroup == "P2").ToList();
            // var am_ccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "P2").ToList();
            //var pm_ccm_thr = stbypm_thr.Where(q => q.JobGroup == "P2").ToList();
            // var pm_ccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "P2").ToList();
            var am_cabin = stbyam_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();
            var pm_cabin = stbypm_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();
            var daily_cabin = stbydaily.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drc-02" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[3, 2].Text = ((DateTime)df).ToString("ddd").ToUpper() + " (" + main.First().PDATE + ") " + ((DateTime)df).ToString("yyyy-MMM-dd").ToUpper()

                + (rev > 0 ? " REVISION:" + rev : "");
            // sheet.Range[3, 10].Text = ((DateTime)df).ToString("dddd");

            var regcnt = 0;
            var ln = 8;
            var positioning = (from q in main
                               where !string.IsNullOrEmpty(q.POSITIONING)
                               orderby q.STDLocal
                               select "[" + q.FlightNumber + "(" + q.FromAirportIATA + "-" + q.ToAirportIATA + " " + q.DepLocal + "): " + q.POSITIONING + "]").ToList();

            //if (rev>0)
            //{
            //    sheet.Range[3, 2].Text = "Revision";
            //    sheet.Range[3, 4].Text = rev.ToString();
            //}
            foreach (var line in regs)
            {
                foreach (var flt in line.Items)
                {
                    sheet.InsertRow(ln);
                    var rowHeight = sheet.Rows[ln - 1].RowHeight;
                    var rh = 25;
                    sheet.Rows[ln - 1].RowHeight = rh;
                    sheet.Rows[ln - 1].Style.Font.Size = 12;
                    sheet.Rows[ln - 1].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet.Range[ln, 2].Text = flt.FlightNumber;

                    sheet.Range[ln, 3].Text = flt.AircraftType;

                    sheet.Range[ln, 4].Text = flt.Register;

                    sheet.Range[ln, 5].Text = flt.FromAirportIATA;
                    sheet.Range[ln, 6].Text = flt.ToAirportIATA;

                    sheet.Range[ln, 7].Text = flt.DepLocal;
                    sheet.Range[ln, 8].Text = flt.ArrLocal;
                    sheet.Range[ln, 9].Text = flt.Dep;

                    sheet.Range[ln, 10].Text = flt.Arr;


                    sheet.Range[ln, 11].Text = string.IsNullOrEmpty(flt.IP) ? "" : flt.IP;

                    sheet.Range[ln, 12].Text = string.IsNullOrEmpty(flt.CPT) ? "" : flt.CPT;

                    sheet.Range[ln, 13].Text = string.IsNullOrEmpty(flt.FO) ? "" : flt.FO;

                    sheet.Range[ln, 14].Text = string.IsNullOrEmpty(flt.SAFETY) ? "" : flt.SAFETY;
                    sheet.Range[ln, 14].AutoFitColumns();

                    sheet.Range[ln, 15].Text = string.IsNullOrEmpty(flt.CHECK) ? "" : flt.CHECK;
                    sheet.Range[ln, 15].AutoFitColumns();

                    sheet.Range[ln, 16].Text = string.IsNullOrEmpty(flt.OBS) ? "" : flt.OBS;
                    sheet.Range[ln, 16].AutoFitColumns();

                    //16 IFP
                    sheet.Range[ln, 17].Text = string.IsNullOrEmpty(flt.ISCCM) ? "" : flt.ISCCM;
                    sheet.Range[ln, 17].AutoFitColumns();
                    //17 FP
                    sheet.Range[ln, 18].Text = string.IsNullOrEmpty(flt.SCCM) ? "" : flt.SCCM;
                    //18 FA
                    sheet.Range[ln, 19].Text = string.IsNullOrEmpty(flt.CCM) ? "" : flt.CCM;
                    sheet.Range[ln, 19].AutoFitColumns();

                    sheet.Range[ln, 20].Text = string.IsNullOrEmpty(flt.CHECKC) ? "" : flt.CHECKC;
                    sheet.Range[ln, 20].AutoFitColumns();


                    sheet.Range[ln, 21].Text = string.IsNullOrEmpty(flt.OBSC) ? "" : flt.OBSC;
                    sheet.Range[ln, 21].AutoFitColumns();
                    // sheet.Range[ln, 16].Text = string.IsNullOrEmpty(flt.POSITIONINGCOCKPIT) ? "" : flt.POSITIONINGCOCKPIT;
                    sheet.Range[ln, 22].Text = string.IsNullOrEmpty(flt.FM) ? "" : flt.FM;

                    sheet.Range[ln, 22].AutoFitColumns();
                    sheet.Range[ln, 22].IsWrapText = true;



                    sheet.Range["B" + ln + ":V" + ln].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range["B" + ln + ":V" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                    sheet.Range["B" + ln + ":V" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                    ln++;

                }

                if (regcnt != regs.Count - 1)
                {
                    sheet.InsertRow(ln);

                    sheet.Range["B" + (ln) + ":V" + ln].Style.Color = Color.Silver;
                }

                ln++;
                regcnt++;
            }



            sheet.Range[ln, 5].Text = string.Join(", ", am_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;

            sheet.Range[ln, 5].Text = string.Join(", ", pm_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;

            sheet.Range[ln, 5].Text = string.Join(", ", daily_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;

            sheet.Range[ln, 5].Text = string.Join(", ", daily_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;



            sheet.Range[ln, 5].Text = string.Join(", ", rsv.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", trn.Select(q => q.ScheduleName).Distinct());
            ln++;
            sheet.Range[ln, 5].Text = string.Join(" ", positioning);
            ln++;


            var hasObsC = main.FirstOrDefault(q => !string.IsNullOrEmpty(q.OBSC));
            if (hasObsC == null)
            {
                sheet.DeleteColumn(21);
            }

            if (main.FirstOrDefault(q => !string.IsNullOrEmpty(q.CHECKC)) == null)
            {
                sheet.DeleteColumn(20);
            }

            if (main.FirstOrDefault(q => !string.IsNullOrEmpty(q.ISCCM)) == null)
            {
                sheet.DeleteColumn(17);
            }

            if (main.FirstOrDefault(q => !string.IsNullOrEmpty(q.OBS)) == null)
            {
                sheet.DeleteColumn(16);
            }
            if (main.FirstOrDefault(q => !string.IsNullOrEmpty(q.CHECK)) == null)
            {
                sheet.DeleteColumn(15);
            }
            if (main.FirstOrDefault(q => !string.IsNullOrEmpty(q.SAFETY)) == null)
            {
                sheet.DeleteColumn(14);
            }

            if (main.FirstOrDefault(q => !string.IsNullOrEmpty(q.IP)) == null)
            {
                sheet.DeleteColumn(11);
            }


            sheet.DeleteColumn(9);
            sheet.DeleteColumn(9);

            // sheet.Range[ln, 5].Text = string.Join(", ", off.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            // ln++;
            // sheet.Range[ln, 5].Text = string.Join(", ", rest.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            //ln++;


            //sheet.Range[ln, 5].Text = string.Join(", ", am_sccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", pm_sccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", am_ccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", pm_ccm_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", rsv_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", off_mhd.Select(q => q.ScheduleName));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", rest_mhd.Select(q => q.ScheduleName));

            //ln++;
            //  ln = ln + 7;
            // sheet.Range[ln, 5].Text = string.Join(", ", ground.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            // ln++;
            // sheet.Range[ln, 5].Text = string.Join(", ", sick.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            //  ln++;
            // sheet.Range[ln, 5].Text = string.Join(", ", vac.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            // ln++;
            // sheet.Range[ln, 5].Text = string.Join(", ", sim.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            // ln++;
            // sheet.Range[ln, 5].Text = string.Join(", ", office.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            // sheet.Range["B5:N" + ln].BorderInside(LineStyleType.Thin, Color.Black);
            //sheet.Range["B5:N" + ln].BorderAround(LineStyleType.Medium, Color.Black);
            var name = "drc-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }




        [Route("api/xls/roster/all/daily/dr03")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSRosterAllDailyDr02(DateTime df)
        {
            //bolan
            var context = new AirpocketAPI.Models.FLYEntities();
            context.Database.CommandTimeout = 500;
            df = ((DateTime)df).Date;
            var dt = df.AddDays(1);
            var fdps = (from x in context.ViewFDPRests
                        where x.DutyType == 1165 && x.STDLocal >= df && x.STDLocal < dt
                        select x.Id).ToList();
            var fdpitems = (from x in context.FDPItems
                            where fdps.Contains(x.FDPId)
                            select x.FlightId).ToList();
            var query = from x in context.ReportRosters
                            //where x.STDDay == df
                        where fdpitems.Contains(x.ID)
                        orderby x.Register, x.STDx
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.ID,
                q.FlightNumber,
                q.Register,
                q.AircraftType,
                q.FromAirportIATA,
                Base = q.FromAirportIATA == "IKA" ? "THR" : q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.STDx,
                q.STAx,
                q.STDLocal,
                q.STALocal,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT,
                q.PDATE

            }).ToList();



            var regs = (from x in main

                        group x by new { x.Register, x.AircraftType } into grp
                        select new
                        {
                            grp.Key.Register,
                            grp.Key.AircraftType,
                            //grp.Key.CPT,
                            Items = grp.OrderBy(q => q.STDx).ToList(),
                            Base = grp.OrderBy(q => q.STDx).First().Base
                        }
                       )
                       //.OrderByDescending(q => q.Base)
                       .OrderBy(q => q.AircraftType).ThenBy(q => q.Register)
                       .ToList();






            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drc-03" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];




            var regcnt = 0;
            var ln = 5;
            var positioning = (from q in main
                               where !string.IsNullOrEmpty(q.POSITIONING)
                               orderby q.STDLocal
                               select "[" + q.FlightNumber + "(" + q.FromAirportIATA + "-" + q.ToAirportIATA + " " + q.DepLocal + "): " + q.POSITIONING + "]").ToList();

            var _types = (from x in regs
                          group x by new { x.AircraftType } into grp
                          select new
                          {
                              grp.Key.AircraftType,
                              items = grp.ToList()
                          }).ToList();
            bool hasIP = false;
            bool hasObs = false;
            bool hasIFP = false;
            bool hasObsc = false;
            bool hasSO = false;
            List<int> t_rows = new List<int>();
            foreach (var act in _types)
            {
                sheet.InsertRow(ln);
                sheet.Range[ln, 2].Text = act.AircraftType;

                sheet.Range["B" + ln + ":Y" + ln].Merge();
                sheet.Range[ln, 2].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[ln, 2].Style.VerticalAlignment = VerticalAlignType.Top;
                sheet.Range[ln, 2].Style.Font.IsBold = true;
                t_rows.Add(ln);
                ln++;
                // sheet.Range["B" + (ln - 1) + ":Y" + (ln - 1)].Style.Color = Color.Silver;
                //2023-12-02
                var cpt_merge_start = ln;
                var fo_merge_start = ln;
                var ip_merge_start = ln;
                var obs_merge_start = ln;
                var so_merge_start = ln;
                var ifp_merge_start = ln;
                var fp_merge_start = ln;
                var fa_merge_start = ln;
                var obsc_merge_start = ln;
                var dh_merge_start = ln;
                var fm_merge_start = ln;

                foreach (var line in act.items)
                {
                    var reg_start = ln;
                    //sheet.Range[ln, 16].Text = line.CPT;
                    cpt_merge_start = ln;
                    fo_merge_start = ln;
                    ip_merge_start = ln;
                    obs_merge_start = ln;
                    so_merge_start = ln;
                    ifp_merge_start = ln;
                    fp_merge_start = ln;
                    fa_merge_start = ln;
                    obsc_merge_start = ln;
                    dh_merge_start = ln;
                    fm_merge_start = ln;

                    var cpt = line.Items[0].CPT;
                    var fo = line.Items[0].FO;
                    var ip = line.Items[0].IP;
                    var so = line.Items[0].SAFETY;
                    var obs = line.Items[0].CHECK;
                    if (!string.IsNullOrEmpty(obs)) obs += "," + line.Items[0].OBS; else obs = line.Items[0].OBS;
                    var ifp = line.Items[0].ISCCM;
                    var fp = line.Items[0].SCCM;
                    var fa = line.Items[0].CCM;
                    var obsc = line.Items[0].CHECKC;
                    var fm = line.Items[0].FM;

                    if (!string.IsNullOrEmpty(obsc)) obsc += "," + line.Items[0].OBSC; else obsc = line.Items[0].OBSC;
                    var dh = line.Items[0].POSITIONING;

                    foreach (var flt in line.Items)
                    {
                        sheet.InsertRow(ln);
                        sheet.Range[ln, 2].Text = flt.Register;
                        sheet.Range[ln, 3].Text = flt.FlightNumber;
                        sheet.Range[ln, 4].Text = flt.FromAirportIATA + "-" + flt.ToAirportIATA;
                        sheet.Range[ln, 5].Text = ((DateTime)flt.STDLocal).ToString("HH:mm");
                        sheet.Range[ln, 8].Text = ((DateTime)flt.STALocal).ToString("HH:mm");

                        hasIP = hasIP || !string.IsNullOrEmpty(flt.IP);
                        sheet.Range[ln, 15].Text = flt.IP ?? "";
                        if (ip != flt.IP)
                        {
                            sheet.Range["O" + (ip_merge_start) + ":O" + (ln - 1)].Merge();
                            sheet.Range[ip_merge_start, 15].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[ip_merge_start, 15].Style.VerticalAlignment = VerticalAlignType.Center;
                            ip = flt.IP;
                            ip_merge_start = ln;
                        }


                        sheet.Range[ln, 16].Text = flt.CPT ?? "";
                        if (cpt != flt.CPT)
                        {
                            sheet.Range["P" + (cpt_merge_start) + ":P" + (ln - 1)].Merge();
                            sheet.Range[cpt_merge_start, 16].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[cpt_merge_start, 16].Style.VerticalAlignment = VerticalAlignType.Center;
                            cpt = flt.CPT;
                            cpt_merge_start = ln;
                        }

                        sheet.Range[ln, 17].Text = flt.FO ?? "";
                        if (fo != flt.FO)
                        {
                            sheet.Range["Q" + (fo_merge_start) + ":Q" + (ln - 1)].Merge();
                            sheet.Range[fo_merge_start, 17].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[fo_merge_start, 17].Style.VerticalAlignment = VerticalAlignType.Center;
                            fo = flt.FO;
                            fo_merge_start = ln;
                        }

                        hasSO = hasSO || !string.IsNullOrEmpty(flt.SAFETY);
                        sheet.Range[ln, 18].Text = flt.SAFETY ?? "";
                        if (so != flt.SAFETY)
                        {
                            sheet.Range["R" + (so_merge_start) + ":R" + (ln - 1)].Merge();
                            sheet.Range[so_merge_start, 18].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[so_merge_start, 18].Style.VerticalAlignment = VerticalAlignType.Center;
                            so = flt.SAFETY;
                            so_merge_start = ln;
                        }


                        var _obs = flt.CHECK ?? "";
                        if (!string.IsNullOrEmpty(_obs))
                            _obs += "," + (flt.OBS ?? "");
                        else
                            _obs += (flt.OBS ?? "");

                        hasObs = hasObs || !string.IsNullOrEmpty(_obs);
                        sheet.Range[ln, 19].Text = _obs;
                        if (string.IsNullOrEmpty(_obs)) _obs = null;
                        if (obs != _obs)
                        {
                            sheet.Range["S" + (obs_merge_start) + ":S" + (ln - 1)].Merge();
                            sheet.Range[obs_merge_start, 19].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[obs_merge_start, 19].Style.VerticalAlignment = VerticalAlignType.Center;
                            obs = _obs;
                            obs_merge_start = ln;
                        }



                        sheet.Range[ln, 20].Text = flt.ISCCM ?? "";
                        hasIFP = hasIFP || !string.IsNullOrEmpty(flt.ISCCM);
                        if (ifp != flt.ISCCM)
                        {
                            sheet.Range["T" + (ifp_merge_start) + ":T" + (ln - 1)].Merge();
                            sheet.Range[ifp_merge_start, 20].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[ifp_merge_start, 20].Style.VerticalAlignment = VerticalAlignType.Center;
                            ifp = flt.ISCCM;
                            ifp_merge_start = ln;
                        }

                        sheet.Range[ln, 21].Text = flt.SCCM ?? "";
                        if (fp != flt.SCCM)
                        {
                            sheet.Range["U" + (fp_merge_start) + ":U" + (ln - 1)].Merge();
                            sheet.Range[fp_merge_start, 21].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[fp_merge_start, 21].Style.VerticalAlignment = VerticalAlignType.Center;
                            fp = flt.SCCM;
                            fp_merge_start = ln;
                        }


                        sheet.Range[ln, 22].Text = flt.CCM ?? "";
                        if (fa != flt.CCM)
                        {
                            sheet.Range["V" + (fa_merge_start) + ":V" + (ln - 1)].Merge();
                            sheet.Range[fa_merge_start, 22].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[fa_merge_start, 22].Style.VerticalAlignment = VerticalAlignType.Center;
                            fa = flt.CCM;
                            fa_merge_start = ln;
                        }


                        var _obsc = flt.CHECKC ?? "";
                        if (!string.IsNullOrEmpty(_obsc))
                            _obsc += "," + (flt.OBSC ?? "");
                        else
                            _obsc += (flt.OBSC ?? "");
                        hasObsc = hasObsc || !string.IsNullOrEmpty(_obsc);
                        sheet.Range[ln, 23].Text = _obsc;
                        if (string.IsNullOrEmpty(_obsc)) _obsc = null;
                        if (obsc != _obsc)
                        {
                            sheet.Range["W" + (obsc_merge_start) + ":W" + (ln - 1)].Merge();
                            sheet.Range[obsc_merge_start, 23].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[obsc_merge_start, 23].Style.VerticalAlignment = VerticalAlignType.Center;
                            obsc = _obsc;
                            obsc_merge_start = ln;
                        }


                        sheet.Range[ln, 25].Text = flt.POSITIONING ?? "";
                        if (dh != flt.POSITIONING)
                        {
                            sheet.Range["Y" + (dh_merge_start) + ":Y" + (ln - 1)].Merge();
                            sheet.Range[dh_merge_start, 25].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[dh_merge_start, 25].Style.VerticalAlignment = VerticalAlignType.Center;
                            dh = flt.POSITIONING;
                            dh_merge_start = ln;
                        }


                        sheet.Range[ln, 24].Text = flt.FM ?? "";
                        if (fm != flt.FM)
                        {
                            sheet.Range["X" + (fm_merge_start) + ":X" + (ln - 1)].Merge();
                            sheet.Range[fm_merge_start, 24].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            sheet.Range[fm_merge_start, 24].Style.VerticalAlignment = VerticalAlignType.Center;
                            fm = flt.FM;
                            fm_merge_start = ln;
                        }
                        if (flt != line.Items.Last())
                        {
                            // sheet.Range["B" + ln + ":Y" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                            //   sheet.Range["B" + ln + ":Y" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                            sheet.Range["B" + (ln) + ":Y" + (ln)].Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
                            sheet.Range["B" + (ln) + ":Y" + (ln)].Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                            sheet.Range["B" + (ln) + ":Y" + (ln)].Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                            sheet.Range["B" + (ln) + ":Y" + (ln)].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;
                        }
                        else
                        {
                            //sheet.Range["B" + ln + ":Y" + ln].BorderInside(LineStyleType.Thin, Color.Black);


                            sheet.Range["B" + (ln) + ":Y" + (ln)].Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
                            sheet.Range["B" + (ln) + ":Y" + (ln)].Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                            sheet.Range["B" + (ln) + ":Y" + (ln)].Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                            sheet.Range["B" + (ln) + ":Y" + (ln)].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thick;
                        }


                        ln++;
                    }
                    sheet.Range["B" + reg_start + ":B" + (ln - 1)].Merge();
                    sheet.Range[reg_start, 2].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[reg_start, 2].Style.VerticalAlignment = VerticalAlignType.Center;

                    //ip
                    sheet.Range["O" + (ip_merge_start) + ":O" + (ln - 1)].Merge();
                    sheet.Range[ip_merge_start, 15].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[ip_merge_start, 15].Style.VerticalAlignment = VerticalAlignType.Center;

                    //cpt
                    sheet.Range["P" + (cpt_merge_start) + ":P" + (ln - 1)].Merge();
                    sheet.Range[cpt_merge_start, 16].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[cpt_merge_start, 16].Style.VerticalAlignment = VerticalAlignType.Center;
                    //fo
                    sheet.Range["Q" + (fo_merge_start) + ":Q" + (ln - 1)].Merge();
                    sheet.Range[fo_merge_start, 17].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[fo_merge_start, 17].Style.VerticalAlignment = VerticalAlignType.Center;
                    //sq
                    sheet.Range["R" + (so_merge_start) + ":R" + (ln - 1)].Merge();
                    sheet.Range[so_merge_start, 18].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[so_merge_start, 18].Style.VerticalAlignment = VerticalAlignType.Center;
                    //obs-chek
                    sheet.Range["S" + (obs_merge_start) + ":S" + (ln - 1)].Merge();
                    sheet.Range[obs_merge_start, 19].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[obs_merge_start, 19].Style.VerticalAlignment = VerticalAlignType.Center;
                    //ifp
                    sheet.Range["T" + (ifp_merge_start) + ":T" + (ln - 1)].Merge();
                    sheet.Range[ifp_merge_start, 20].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[ifp_merge_start, 20].Style.VerticalAlignment = VerticalAlignType.Center;
                    //fp
                    sheet.Range["U" + (fp_merge_start) + ":U" + (ln - 1)].Merge();
                    sheet.Range[fp_merge_start, 21].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[fp_merge_start, 21].Style.VerticalAlignment = VerticalAlignType.Center;
                    //fa
                    sheet.Range["V" + (fa_merge_start) + ":V" + (ln - 1)].Merge();
                    sheet.Range[fa_merge_start, 22].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[fa_merge_start, 22].Style.VerticalAlignment = VerticalAlignType.Center;
                    //obs-chek
                    sheet.Range["W" + (obsc_merge_start) + ":W" + (ln - 1)].Merge();
                    sheet.Range[obsc_merge_start, 23].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[obsc_merge_start, 23].Style.VerticalAlignment = VerticalAlignType.Center;

                    //dh
                    sheet.Range["Y" + (dh_merge_start) + ":Y" + (ln - 1)].Merge();
                    sheet.Range[dh_merge_start, 25].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[dh_merge_start, 25].Style.VerticalAlignment = VerticalAlignType.Center;
                    //fm
                    sheet.Range["X" + (fm_merge_start) + ":X" + (ln - 1)].Merge();
                    sheet.Range[fm_merge_start, 24].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[fm_merge_start, 24].Style.VerticalAlignment = VerticalAlignType.Center;




                    //   sheet.Range["B" + (ln -1 ) + ":Y" + (ln-1  )].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thick;
                    //.li(LineStyleType.Thin, Color.Black);


                }

                //ip
                sheet.Range["O" + (ip_merge_start) + ":O" + (ln - 1)].Merge();
                sheet.Range[ip_merge_start, 15].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[ip_merge_start, 15].Style.VerticalAlignment = VerticalAlignType.Center;
                //cpt
                sheet.Range["P" + (cpt_merge_start) + ":P" + (ln - 1)].Merge();
                sheet.Range[cpt_merge_start, 16].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[cpt_merge_start, 16].Style.VerticalAlignment = VerticalAlignType.Center;

                //fo
                sheet.Range["Q" + (fo_merge_start) + ":Q" + (ln - 1)].Merge();
                sheet.Range[fo_merge_start, 17].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[fo_merge_start, 17].Style.VerticalAlignment = VerticalAlignType.Center;
                //sq
                sheet.Range["R" + (so_merge_start) + ":R" + (ln - 1)].Merge();
                sheet.Range[so_merge_start, 18].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[so_merge_start, 18].Style.VerticalAlignment = VerticalAlignType.Center;
                //obs-chek
                sheet.Range["S" + (obs_merge_start) + ":S" + (ln - 1)].Merge();
                sheet.Range[obs_merge_start, 19].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[obs_merge_start, 19].Style.VerticalAlignment = VerticalAlignType.Center;
                //ifp
                sheet.Range["T" + (ifp_merge_start) + ":T" + (ln - 1)].Merge();
                sheet.Range[ifp_merge_start, 20].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[ifp_merge_start, 20].Style.VerticalAlignment = VerticalAlignType.Center;
                //fp
                sheet.Range["U" + (fp_merge_start) + ":U" + (ln - 1)].Merge();
                sheet.Range[fp_merge_start, 21].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[fp_merge_start, 21].Style.VerticalAlignment = VerticalAlignType.Center;
                //fa
                sheet.Range["V" + (fa_merge_start) + ":V" + (ln - 1)].Merge();
                sheet.Range[fa_merge_start, 22].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[fa_merge_start, 22].Style.VerticalAlignment = VerticalAlignType.Center;
                //obs-chek
                sheet.Range["W" + (obsc_merge_start) + ":W" + (ln - 1)].Merge();
                sheet.Range[obsc_merge_start, 23].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[obsc_merge_start, 23].Style.VerticalAlignment = VerticalAlignType.Center;
                //dh
                sheet.Range["Y" + (dh_merge_start) + ":Y" + (ln - 1)].Merge();
                sheet.Range[dh_merge_start, 25].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[dh_merge_start, 25].Style.VerticalAlignment = VerticalAlignType.Center;
                //fm
                sheet.Range["X" + (fm_merge_start) + ":X" + (ln - 1)].Merge();
                sheet.Range[fm_merge_start, 24].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[fm_merge_start, 24].Style.VerticalAlignment = VerticalAlignType.Center;



            }

            for (int i = 14; i <= 24; i++)
            {
                sheet.Columns[i].AutoFitColumns();
                sheet.Columns[i].IsWrapText = true;
            }

            foreach (var n in t_rows)
            {
                sheet.Range["B" + (n) + ":Y" + (n)].Style.Color = Color.Silver;
            }

            if (!hasObsc)
                sheet.DeleteColumn(23);

            if (!hasIFP)
                sheet.DeleteColumn(20);

            if (!hasObs)
                sheet.DeleteColumn(19);

            if (!hasSO)
                sheet.DeleteColumn(18);

            if (!hasIP)
                sheet.DeleteColumn(15);

            sheet.Range[3, 15].Text = "CREW";



            //////////////////////////////
            var dqs = context.ViewCrewDuties.Where(x => (df >= x.DateLocal && df <= x.DateLocal2)).ToList();

            var dutiesQuery = (from x in /*context.ViewCrewDuties*/ dqs
                               where (df >= x.DateLocal && df <= x.DateLocal2) && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001
                               || x.DutyType == 100003
                               || x.DutyType == 1166
                               || x.DutyType == 10000
                                || x.DutyType == 10001
                                 || x.DutyType == 100007

                                 || x.DutyType == 300009

                                 || x.DutyType == 1169

                                 || x.DutyType == 100002

                                   || x.DutyType == 100000

                                    || x.DutyType == 5000

                                    || x.DutyType == 100003
                                    || x.DutyType == 300013

                               )
                               select x).ToList();
            var rsv = dutiesQuery.Where(q => q.DutyType == 1170 && q.IsCockpit == 1).ToList();
            var trn = dutiesQuery.Where(q => q.DutyType == 5000 && q.IsCockpit == 1).ToList();
            var stbyam_thr = from x in dutiesQuery
                             where x.DutyType == 1168 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbypm_thr = from x in dutiesQuery
                             where x.DutyType == 1167 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbydaily = from x in dutiesQuery
                            where x.DutyType == 300013 //&& x.BaseAirportId == 135502
                            orderby x.OrderIndex, x.ScheduleName
                            select x;

            var am_cockpit = stbyam_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            var pm_cockpit = stbypm_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            var daily_cockpit = stbydaily.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            var am_cabin = stbyam_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();
            var pm_cabin = stbypm_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();
            var daily_cabin = stbydaily.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();

            ln = ln + 1;
            sheet.Range[ln, 5].Text = string.Join(", ", am_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", am_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", pm_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;

            sheet.Range[ln, 5].Text = string.Join(", ", pm_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;

            sheet.Range[ln, 5].Text = string.Join(", ", daily_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;

            sheet.Range[ln, 5].Text = string.Join(", ", daily_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 5].Text = string.Join(", ", rsv.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;

            sheet.Range[ln, 5].Text = string.Join(" ", positioning);
            ln++;

            //  sheet.Range[3, 2].Text = ((DateTime)df).ToString("ddd").ToUpper() + " (" + main.First().PDATE + ") " + ((DateTime)df).ToString("yyyy-MMM-dd").ToUpper()
            sheet.Range[2, 4].Text = ((DateTime)df).ToString("yyyy-MMM-dd").ToUpper();
            sheet.Range[2, 7].Text = main.First().PDATE;
            sheet.Range[2, 10].Text = ((DateTime)df).ToString("dddd").ToUpper();
            var p_day = "";
            switch (((DateTime)df).ToString("dddd").ToUpper())
            {
                case "SUNDAY":
                    p_day = "یکشنبه";
                    break;
                case "MONDAY":
                    p_day = "دوشنبه";
                    break;

                case "TUESDAY":
                    p_day = "سه شنبه";
                    break;
                case "WEDNESDAY":
                    p_day = "چهارشنبه";
                    break;
                case "THURSDAY":
                    p_day = "پنج شنبه";
                    break;
                case "FRIDAY":
                    p_day = "جمعه";
                    break;
                case "SATURDAY":
                    p_day = "شنبه";
                    break;
                default:
                    break;
            }
            sheet.Range[2, 11].Text = p_day;
            var name = "drd-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }



        [Route("api/xls/roster/all/daily/dr06")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSRosterAllDailyDr06(DateTime df)
        {
            //bolan
            var context = new AirpocketAPI.Models.FLYEntities();
            context.Database.CommandTimeout = 500;
            df = ((DateTime)df).Date;
            var dt = df.AddDays(1);
            var fdps = (from x in context.ViewFDPRests
                        where x.DutyType == 1165 && x.STDLocal >= df && x.STDLocal < dt
                        select x.Id).ToList();
            var fdpitems = (from x in context.FDPItems
                            where fdps.Contains(x.FDPId)
                            select x.FlightId).ToList();
            var query = from x in context.ReportRosters
                            //where x.STDDay == df
                        where fdpitems.Contains(x.ID)
                        orderby x.Register, x.STDx
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.ID,
                q.FlightNumber,
                q.Register,
                q.AircraftType,
                q.FromAirportIATA,
                Base = q.FromAirportIATA == "IKA" ? "THR" : q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.STDx,
                q.STAx,
                q.STDLocal,
                q.STALocal,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT,
                q.PDATE,
                PIC = !string.IsNullOrEmpty(q.IP) ? q.IP : q.CPT,

            }).ToList();



            var regs = (from x in main

                        group x by new { x.Register, x.AircraftType, x.PIC } into grp
                        select new
                        {
                            grp.Key.Register,
                            grp.Key.AircraftType,
                            //grp.Key.CPT,
                            Items = grp.OrderBy(q => q.STDx).ToList(),
                            Base = grp.OrderBy(q => q.STDx).First().Base,
                            IPs = grp.Where(q => !string.IsNullOrEmpty(q.IP)).Select(q => q.IP).Distinct().ToList(),
                            CPTs = grp.Where(q => !string.IsNullOrEmpty(q.CPT)).Select(q => q.CPT).Distinct().ToList(),
                            FOs = grp.Where(q => !string.IsNullOrEmpty(q.FO)).Select(q => q.FO).Distinct().ToList(),
                            SOs = grp.Where(q => !string.IsNullOrEmpty(q.SAFETY)).Select(q => q.SAFETY).Distinct().ToList(),
                            OBSs = grp.Where(q => !string.IsNullOrEmpty(q.OBS)).Select(q => "OBS:" + q.OBS).Distinct().ToList(),
                            ISCCMs = grp.Where(q => !string.IsNullOrEmpty(q.ISCCM)).Select(q => q.ISCCM).Distinct().ToList(),
                            SCCMs = grp.Where(q => !string.IsNullOrEmpty(q.SCCM)).Select(q => q.SCCM).Distinct().ToList(),
                            CCMs = grp.Where(q => !string.IsNullOrEmpty(q.CCM)).Select(q => q.CCM).Distinct().ToList(),
                            OBSCs = grp.Where(q => !string.IsNullOrEmpty(q.OBSC)).Select(q => "OBS:" + q.OBSC).Distinct().ToList(),
                            DHs = grp.Where(q => !string.IsNullOrEmpty(q.POSITIONING)).Select(q => "D" + q.FlightNumber.Substring(2) + ":" + q.POSITIONING).ToList(),
                            grp.Key.PIC
                        }
                       )
                       //.OrderByDescending(q => q.Base)
                       .OrderBy(q => q.AircraftType).ThenBy(q => q.Register)
                       .ToList();






            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drc-06" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];




            var regcnt = 0;
            var ln = 5;
            var total_row = 0;
            var positioning = (from q in main
                               where !string.IsNullOrEmpty(q.POSITIONING)
                               orderby q.STDLocal
                               select "[" + q.FlightNumber + "(" + q.FromAirportIATA + "-" + q.ToAirportIATA + " " + q.DepLocal + "): " + q.POSITIONING + "]").ToList();

            var _types = (from x in regs
                          group x by new { x.AircraftType } into grp
                          select new
                          {
                              grp.Key.AircraftType,
                              items = grp.ToList()
                          }).ToList();
            bool hasIP = false;
            bool hasObs = false;
            bool hasIFP = false;
            bool hasObsc = false;
            bool hasSO = false;
            List<int> t_rows = new List<int>();
            foreach (var act in _types)
            {
                sheet.InsertRow(ln);
                total_row++;
                sheet.Range[ln, 1].Text = act.AircraftType;
                sheet.Rows[ln - 1].RowHeight = 27;
                sheet.Rows[ln - 1].IsWrapText = true;

                sheet.Range["A" + ln + ":Q" + ln].Merge();
                sheet.Range[ln, 1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range[ln, 1].Style.VerticalAlignment = VerticalAlignType.Top;
                sheet.Range[ln, 1].Style.Font.IsBold = true;
                t_rows.Add(ln);
                ln++;
                // sheet.Range["B" + (ln - 1) + ":Y" + (ln - 1)].Style.Color = Color.Silver;
                //2023-12-02
                var cpt_merge_start = ln;
                var fo_merge_start = ln;
                var ip_merge_start = ln;
                var obs_merge_start = ln;
                var so_merge_start = ln;
                var ifp_merge_start = ln;
                var fp_merge_start = ln;
                var fa_merge_start = ln;
                var obsc_merge_start = ln;
                var dh_merge_start = ln;
                var fm_merge_start = ln;

                foreach (var line in act.items)
                {
                    var reg_start = ln;
                    //sheet.Range[ln, 16].Text = line.CPT;
                    cpt_merge_start = ln;
                    fo_merge_start = ln;
                    ip_merge_start = ln;
                    obs_merge_start = ln;
                    so_merge_start = ln;
                    ifp_merge_start = ln;
                    fp_merge_start = ln;
                    fa_merge_start = ln;
                    obsc_merge_start = ln;
                    dh_merge_start = ln;
                    fm_merge_start = ln;

                    var cpt = line.Items[0].CPT;
                    var fo = line.Items[0].FO;
                    var ip = line.Items[0].IP;
                    var so = line.Items[0].SAFETY;
                    var obs = line.Items[0].CHECK;
                    if (!string.IsNullOrEmpty(obs)) obs += "," + line.Items[0].OBS; else obs = line.Items[0].OBS;
                    var ifp = line.Items[0].ISCCM;
                    var fp = line.Items[0].SCCM;
                    var fa = line.Items[0].CCM;
                    var obsc = line.Items[0].CHECKC;
                    var fm = line.Items[0].FM;

                    if (!string.IsNullOrEmpty(obsc)) obsc += "," + line.Items[0].OBSC; else obsc = line.Items[0].OBSC;
                    var dh = line.Items[0].POSITIONING;

                    var _cpt = string.Join(" , ", line.IPs.Concat(line.CPTs).ToList());
                    var _fo = string.Join(" , ", line.FOs.Concat(line.SOs).Concat(line.OBSs).ToList());
                    var _cabin = string.Join(" , ", line.ISCCMs.Concat(line.SCCMs).Concat(line.CCMs).Concat(line.OBSCs).ToList());
                    var _extra = string.Join("\n", line.DHs);
                    //sheet.Range[reg_start, 14].Text = _cpt;

                    foreach (var flt in line.Items)
                    {
                        sheet.InsertRow(ln);
                        sheet.Range[ln, 1].Text = flt.Register;
                        sheet.Range[ln, 2].Text = flt.FlightNumber;
                        sheet.Range[ln, 3].Text = flt.FromAirportIATA + "-" + flt.ToAirportIATA;
                        sheet.Range[ln, 4].Text = ((DateTime)flt.STDLocal).ToString("HH:mm");
                        sheet.Range[ln, 7].Text = ((DateTime)flt.STALocal).ToString("HH:mm");

                        sheet.Range[ln, 14].Text = _cpt;

                        if (flt != line.Items.Last())
                        {
                            // sheet.Range["B" + ln + ":Y" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                            //   sheet.Range["B" + ln + ":Y" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                            sheet.Range["A" + (ln) + ":Q" + (ln)].Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
                            sheet.Range["A" + (ln) + ":Q" + (ln)].Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                            sheet.Range["A" + (ln) + ":Q" + (ln)].Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                            sheet.Range["A" + (ln) + ":Q" + (ln)].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;
                        }
                        else
                        {
                            //sheet.Range["B" + ln + ":Y" + ln].BorderInside(LineStyleType.Thin, Color.Black);


                            sheet.Range["A" + (ln) + ":Q" + (ln)].Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
                            sheet.Range["A" + (ln) + ":Q" + (ln)].Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                            sheet.Range["A" + (ln) + ":Q" + (ln)].Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                            sheet.Range["A" + (ln) + ":Q" + (ln)].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thick;
                        }

                        ///if (total_row <= 18)
                        ln++;
                        // else
                        //{
                        //    total_row = 0;

                        //    ln=ln+3;
                        //}
                    }
                    sheet.Range["A" + reg_start + ":A" + (ln - 1)].Merge();
                    sheet.Range["N" + reg_start + ":N" + (ln - 1)].Merge();
                    sheet.Range["O" + reg_start + ":O" + (ln - 1)].Merge();
                    sheet.Range["P" + reg_start + ":P" + (ln - 1)].Merge();
                    sheet.Range["Q" + reg_start + ":Q" + (ln - 1)].Merge();
                    sheet.Range[reg_start, 1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[reg_start, 1].Style.VerticalAlignment = VerticalAlignType.Center;

                    sheet.Range[reg_start, 14].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[reg_start, 14].Style.VerticalAlignment = VerticalAlignType.Center;

                    sheet.Range[reg_start, 15].Text = _fo;
                    sheet.Range[reg_start, 15].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[reg_start, 15].Style.VerticalAlignment = VerticalAlignType.Center;

                    sheet.Range[reg_start, 16].Text = _cabin;
                    sheet.Range[reg_start, 16].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[reg_start, 16].Style.VerticalAlignment = VerticalAlignType.Center;


                    sheet.Range[reg_start, 17].Text = _extra;
                    sheet.Range[reg_start, 17].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range[reg_start, 17].Style.VerticalAlignment = VerticalAlignType.Center;









                }



            }

            //for (int i = 14; i <= 24; i++)
            //{
            //    sheet.Columns[i].AutoFitColumns();
            //    sheet.Columns[i].IsWrapText = true;
            //}

            foreach (var n in t_rows)
            {



                sheet.Range["A" + (n) + ":Q" + (n)].Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
                sheet.Range["A" + (n) + ":Q" + (n)].Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                sheet.Range["A" + (n) + ":Q" + (n)].Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                sheet.Range["A" + (n) + ":Q" + (n)].Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;


                sheet.Range["A" + (n) + ":Q" + (n)].Style.Color = Color.Silver;


            }

            for (int i = 5; i <= ln; i++)
            {
                sheet.Rows[i - 1].SetRowHeight(27, true);
                sheet.Range["N" + (i) + ":Q" + (i)].Style.Font.Size = 12;

            }





            //////////////////////////////
            var dqs = context.ViewCrewDuties.Where(x => (df >= x.DateLocal && df <= x.DateLocal2)).ToList();

            var dutiesQuery = (from x in /*context.ViewCrewDuties*/ dqs
                               where (df >= x.DateLocal && df <= x.DateLocal2) && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001
                               || x.DutyType == 100003
                               || x.DutyType == 1166
                               || x.DutyType == 10000
                                || x.DutyType == 10001
                                 || x.DutyType == 100007

                                 || x.DutyType == 300009

                                 || x.DutyType == 1169

                                 || x.DutyType == 100002

                                   || x.DutyType == 100000

                                    || x.DutyType == 5000

                                    || x.DutyType == 100003
                                    || x.DutyType == 300013

                               )
                               select x).ToList();
            var rsv = dutiesQuery.Where(q => q.DutyType == 1170 && q.IsCockpit == 1).ToList();
            var trn = dutiesQuery.Where(q => q.DutyType == 5000 && q.IsCockpit == 1).ToList();
            var stbyam_thr = from x in dutiesQuery
                             where x.DutyType == 1168 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbypm_thr = from x in dutiesQuery
                             where x.DutyType == 1167 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            var stbydaily = from x in dutiesQuery
                            where x.DutyType == 300013 //&& x.BaseAirportId == 135502
                            orderby x.OrderIndex, x.ScheduleName
                            select x;

            var am_cockpit = stbyam_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            var pm_cockpit = stbypm_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            var daily_cockpit = stbydaily.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            var am_cabin = stbyam_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();
            var pm_cabin = stbypm_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();
            var daily_cabin = stbydaily.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();

            ln = ln + 1;
            //sheet.Range[ln, 5].Text = string.Join(", ", am_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", am_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", pm_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;

            //sheet.Range[ln, 5].Text = string.Join(", ", pm_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;

            sheet.Range[ln, 4].Text = string.Join(", ", daily_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;

            sheet.Range[ln, 4].Text = string.Join(", ", daily_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;
            sheet.Range[ln, 4].Text = string.Join(", ", rsv.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            ln++;

            sheet.Range[ln, 4].Text = string.Join(" ", positioning);
            ln++;

            //  sheet.Range[3, 2].Text = ((DateTime)df).ToString("ddd").ToUpper() + " (" + main.First().PDATE + ") " + ((DateTime)df).ToString("yyyy-MMM-dd").ToUpper()
            sheet.Range[2, 3].Text = ((DateTime)df).ToString("yyyy-MMM-dd").ToUpper();
            sheet.Range[2, 6].Text = main.First().PDATE;
            sheet.Range[2, 10].Text = ((DateTime)df).ToString("dddd").ToUpper();
            var p_day = "";
            switch (((DateTime)df).ToString("dddd").ToUpper())
            {
                case "SUNDAY":
                    p_day = "یکشنبه";
                    break;
                case "MONDAY":
                    p_day = "دوشنبه";
                    break;

                case "TUESDAY":
                    p_day = "سه شنبه";
                    break;
                case "WEDNESDAY":
                    p_day = "چهارشنبه";
                    break;
                case "THURSDAY":
                    p_day = "پنج شنبه";
                    break;
                case "FRIDAY":
                    p_day = "جمعه";
                    break;
                case "SATURDAY":
                    p_day = "شنبه";
                    break;
                default:
                    break;
            }
            sheet.Range[2, 9].Text = p_day;
            var name = "drd-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }

        [Route("api/xls/roster/sec/daily")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetXLSRosterSecDaily(DateTime df)
        {
            //bolan
            var context = new AirpocketAPI.Models.FLYEntities();
            context.Database.CommandTimeout = 500;
            df = ((DateTime)df).Date;
            var dt = df.AddDays(1);
            var fdps = (from x in context.ViewFDPRests
                        where x.DutyType == 1165 && x.STDLocal >= df && x.STDLocal < dt
                        select x.Id).ToList();
            var fdpitems = (from x in context.FDPItems
                            where fdps.Contains(x.FDPId)
                            select x.FlightId).ToList();
            var query = from x in context.ReportRosters
                            //where x.STDDay == df
                        where fdpitems.Contains(x.ID)
                        orderby x.Register, x.STDx
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.ID,
                q.FlightNumber,
                q.Register,
                q.FromAirportIATA,
                Base = q.FromAirportIATA == "IKA" ? "THR" : q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.STDx,
                q.STAx,
                q.STDLocal,
                q.STALocal,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT
            }).ToList();
            var regs = (from x in main

                        group x by new { x.Register } into grp
                        select new
                        {
                            grp.Key.Register,
                            Items = grp.OrderBy(q => q.STDx).ToList(),
                            Base = grp.OrderBy(q => q.STDx).First().Base
                        }
                       ).OrderByDescending(q => q.Base).ToList();
            // var dqs = context.ViewCrewDuties.Where(x => (df >= x.DateLocal && df <= x.DateLocal2)).ToList();

            var dutiesQuery = new List<ViewCrewDuty>(); /*(from x in  dqs
                               where (df >= x.DateLocal && df <= x.DateLocal2) && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001
                               || x.DutyType == 100003
                               || x.DutyType == 1166
                               || x.DutyType == 10000
                                || x.DutyType == 10001
                                 || x.DutyType == 100007

                                 || x.DutyType == 300009

                                 || x.DutyType == 1169

                                 || x.DutyType == 100002

                                   || x.DutyType == 100000

                                    || x.DutyType == 5000

                                    || x.DutyType == 100003

                               )
                               select x).ToList();*/

            var off = dutiesQuery.Where(q => (q.DutyType == 1166 || q.DutyType == 10000 || q.DutyType == 10001 || q.DutyType == 100007) && q.IsCockpit == 1).ToList();
            var off_mhd = off.Where(q => q.BaseAirportId == 140870).ToList();
            var off_thr = off.Where(q => q.BaseAirportId == 135502).ToList();
            var rest = dutiesQuery.Where(q => q.DutyType == 300009 && q.IsCockpit == 1).ToList();
            var rest_mhd = rest.Where(q => q.BaseAirportId == 140870).ToList();
            var rest_thr = rest.Where(q => q.BaseAirportId == 135502).ToList();
            var rsv = dutiesQuery.Where(q => q.DutyType == 1170 && q.IsCockpit == 1).ToList();
            var rsv_mhd = rsv.Where(q => q.BaseAirportId == 140870).ToList();
            var rsv_thr = rsv.Where(q => q.BaseAirportId == 135502).ToList();
            var vac = dutiesQuery.Where(q => q.DutyType == 1169 && q.IsCockpit == 1).ToList();
            var sick = dutiesQuery.Where(q => q.DutyType == 100002 && q.IsCockpit == 1).ToList();
            var ground = dutiesQuery.Where(q => q.DutyType == 100000 && q.IsCockpit == 1).ToList();
            var trn = dutiesQuery.Where(q => q.DutyType == 5000 && q.IsCockpit == 1).ToList();
            var sim = dutiesQuery.Where(q => q.DutyType == 100003 && q.IsCockpit == 1).ToList();
            var office = dutiesQuery.Where(q => q.DutyType == 5001 && q.IsCockpit == 1).ToList();
            //140870
            var stbyam_thr = from x in dutiesQuery
                             where x.DutyType == 1168 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            /*var stbyam_mhd = from x in dutiesQuery
                             where x.DutyType == 1168 && x.BaseAirportId == 140870
                             orderby x.OrderIndex, x.ScheduleName
                             select x;*/

            var stbypm_thr = from x in dutiesQuery
                             where x.DutyType == 1167 //&& x.BaseAirportId == 135502
                             orderby x.OrderIndex, x.ScheduleName
                             select x;
            /* var stbypm_mhd = from x in dutiesQuery
                              where x.DutyType == 1167 && x.BaseAirportId == 140870
                              orderby x.OrderIndex, x.ScheduleName
                              select x;*/

            var am_cockpit = stbyam_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            //  var am_sccm_mhd = stbyam_mhd/*.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1")*/.ToList();
            var pm_cockpit = stbypm_thr.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1" || q.JobGroup == "P2").ToList();
            // var pm_sccm_mhd = stbypm_mhd/*.Where(q => q.JobGroup == "IP" || q.JobGroup == "TRE" || q.JobGroup == "TRI" || q.JobGroup == "P1")*/.ToList();


            //  var am_ccm_thr = stbyam_thr.Where(q => q.JobGroup == "P2").ToList();
            // var am_ccm_mhd = stbyam_mhd.Where(q => q.JobGroup == "P2").ToList();
            //var pm_ccm_thr = stbypm_thr.Where(q => q.JobGroup == "P2").ToList();
            // var pm_ccm_mhd = stbypm_mhd.Where(q => q.JobGroup == "P2").ToList();
            var am_cabin = stbyam_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();
            var pm_cabin = stbypm_thr.Where(q => q.JobGroup == "ISCCM" || q.JobGroup == "SCCM" || q.JobGroup == "CCM").ToList();

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "drsec" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            sheet.Range[2, 9].Text = ((DateTime)df).ToString("yyyy-MMM-dd");
            sheet.Range[3, 9].Text = ((DateTime)df).ToString("dddd");

            var regcnt = 0;
            var ln = 8;
            foreach (var line in regs)
            {
                foreach (var flt in line.Items)
                {
                    sheet.InsertRow(ln);
                    var rowHeight = sheet.Rows[ln - 1].RowHeight;
                    var rh = 25;
                    sheet.Rows[ln - 1].RowHeight = rh;
                    sheet.Rows[ln - 1].Style.Font.Size = 14;
                    sheet.Rows[ln - 1].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet.Range[ln, 2].Text = flt.FlightNumber;


                    sheet.Range[ln, 3].Text = flt.Register;

                    sheet.Range[ln, 4].Text = flt.FromAirportIATA;
                    sheet.Range[ln, 5].Text = flt.ToAirportIATA;

                    sheet.Range[ln, 6].Text = flt.DepLocal;
                    sheet.Range[ln, 7].Text = flt.ArrLocal;
                    sheet.Range[ln, 8].Text = flt.Dep;

                    sheet.Range[ln, 9].Text = flt.Arr;


                    sheet.Range[ln, 10].Text = string.IsNullOrEmpty(flt.IP) ? "" : flt.IP;

                    sheet.Range[ln, 11].Text = string.IsNullOrEmpty(flt.CPT) ? "" : flt.CPT;

                    sheet.Range[ln, 12].Text = string.IsNullOrEmpty(flt.FO) ? "" : flt.FO;

                    sheet.Range[ln, 13].Text = string.IsNullOrEmpty(flt.SAFETY) ? "" : flt.SAFETY;
                    sheet.Range[ln, 13].AutoFitColumns();

                    sheet.Range[ln, 14].Text = string.IsNullOrEmpty(flt.CHECK) ? "" : flt.CHECK;
                    sheet.Range[ln, 14].AutoFitColumns();

                    sheet.Range[ln, 15].Text = string.IsNullOrEmpty(flt.OBS) ? "" : flt.OBS;
                    sheet.Range[ln, 15].AutoFitColumns();

                    //16 IFP
                    sheet.Range[ln, 16].Text = string.IsNullOrEmpty(flt.ISCCM) ? "" : flt.ISCCM;
                    sheet.Range[ln, 16].AutoFitColumns();
                    //17 FP
                    sheet.Range[ln, 17].Text = string.IsNullOrEmpty(flt.SCCM) ? "" : flt.SCCM;
                    //18 FA
                    sheet.Range[ln, 18].Text = string.IsNullOrEmpty(flt.CCM) ? "" : flt.CCM;
                    sheet.Range[ln, 18].AutoFitColumns();
                    //19 check
                    //20 obsc
                    sheet.Range[ln, 19].Text = string.IsNullOrEmpty(flt.OBSC) ? "" : flt.OBSC;
                    sheet.Range[ln, 19].AutoFitColumns();
                    // sheet.Range[ln, 16].Text = string.IsNullOrEmpty(flt.POSITIONINGCOCKPIT) ? "" : flt.POSITIONINGCOCKPIT;




                    sheet.Range["B" + ln + ":U" + ln].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet.Range["B" + ln + ":U" + ln].BorderInside(LineStyleType.Thin, Color.Black);
                    sheet.Range["B" + ln + ":U" + ln].BorderAround(LineStyleType.Thin, Color.Black);
                    ln++;

                }

                if (regcnt != regs.Count - 1)
                {
                    sheet.InsertRow(ln);

                    sheet.Range["B" + (ln) + ":U" + ln].Style.Color = Color.Silver;
                }

                ln++;
                regcnt++;
            }


            //sheet.Range[ln, 3].Text = string.Join(", ", trn.Select(q => q.ScheduleName));
            //ln = ln + 2;


            //sheet.Range[ln, 5].Text = string.Join(", ", am_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", pm_cockpit.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", am_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", pm_cabin.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", rsv.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", off.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));
            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", rest.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            //ln++;

            //sheet.Range[ln, 5].Text = string.Join(", ", ground.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", sick.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", vac.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", sim.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            //ln++;
            //sheet.Range[ln, 5].Text = string.Join(", ", office.Select(q => q.ScheduleName + "(" + q.JobGroup + ")"));

            // sheet.Range["B5:N" + ln].BorderInside(LineStyleType.Thin, Color.Black);
            //sheet.Range["B5:N" + ln].BorderAround(LineStyleType.Medium, Color.Black);
            var name = "drsec-" + ((DateTime)df).ToString("yyyy-MMM-dd");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + name + ".xlsx");



            workbook.SaveToFile(mappedPath, ExcelVersion.Version2016);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(mappedPath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = name + ".xlsx";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            return response;
        }


        [Route("api/flight/daily/xls")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsDailyXLS(DateTime df, DateTime dt, string regs, string routes, string from, string to, string no)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var cmd = "select * from viewflightdaily ";
            string whr = "FlightStatusId<>4 and (STDDayLocal>='" + df.ToString("yyyy-MM-dd") + "' and STDDayLocal<='" + dt.ToString("yyyy-MM-dd") + "')";
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

            cmd = cmd + " WHERE " + whr + " ORDER BY STD,Register";

            var flts = context.ViewFlightDailies
                        .SqlQuery(cmd)
                        .ToList<ViewFlightDaily>();

            //var result = await courseService.GetEmployeeCertificates(id);

            Workbook workbook = new Workbook();
            var mappedPathSource = System.Web.Hosting.HostingEnvironment.MapPath("~/upload/" + "fltdaily" + ".xlsx");
            workbook.LoadFromFile(mappedPathSource);
            Worksheet sheet = workbook.Worksheets[0];

            var startRow = 8;
            var ln = startRow;
            foreach (var flt in flts)
            {
                sheet.Range[ln, 1].Value2 = flt.PMonthName;
                ln++;
            }




            return Ok(flts);
        }




        [Route("api/fixtime/save/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostFixTime(FixTimeDto dto)
        {
            var context = new AirpocketAPI.Models.FLYEntities();

            string _period = "";
            switch (dto.Period.ToString())
            {
                case "1":
                    _period = "12/16 - 01/15";
                    break;
                case "2":
                    _period = "01/16 - 02/15";
                    break;
                case "3":
                    _period = "02/16 - 03/15";
                    break;
                case "4":
                    _period = "03/16 - 04/15";
                    break;
                case "5":
                    _period = "04/16 - 05/15";
                    break;
                case "6":
                    _period = "05/16 - 06/15";
                    break;
                case "7":
                    _period = "06/16 - 07/15";
                    break;
                case "8":
                    _period = "07/16 - 08/15";
                    break;
                case "9":
                    _period = "08/16 - 09/15";
                    break;
                case "10":
                    _period = "09/16 - 10/15";
                    break;
                case "11":
                    _period = "10/16 - 11/15";
                    break;
                case "12":
                    _period = "11/16 - 12/15";
                    break;
                default:
                    break;
            }

            var day = context.DayGPs.Where(q => q.PYear == dto.Year && q.PeriodFixTime == _period).OrderByDescending(q => q.GDate).FirstOrDefault();
            if (day == null)
                return Ok(false);

            var _start = day.GDate;
            var totalMinutes = dto.HH * 60 + dto.MM;
            var _end = _start.AddMinutes(totalMinutes);

            if (dto.FlightId != null)
            {
                var flight = context.ViewLegTimes.Where(q => q.ID == dto.FlightId).FirstOrDefault();
                if (flight != null)
                {
                    _start = ((DateTime)flight.STDLocal);
                    _end = _start.AddMinutes(totalMinutes);
                }
            }

            foreach (var x in dto.Ids)
            {
                var duty = new FDP();
                duty.DateStart = _start;
                duty.DateEnd = _end;

                duty.CrewId = x;
                duty.DutyType = dto.Type;
                duty.GUID = Guid.NewGuid();
                duty.IsTemplate = false;
                duty.Remark = dto.Remark;
                duty.UPD = 1;

                duty.InitStart = duty.DateStart;
                duty.InitEnd = duty.DateEnd;

                duty.InitRestTo = duty.DateEnd;

                duty.DelayAmount = dto.Count;
                if (duty.DutyType == 300007)
                    duty.Remark2 = "Count: " + duty.DelayAmount.ToString() + " night(s)";
                if (duty.DutyType == 300010)
                    duty.Remark2 = "Count: " + duty.DelayAmount.ToString();

                context.FDPs.Add(duty);

            }
            context.SaveChanges();
            return Ok(true);
        }




        [Route("api/flights")]
        [AcceptVerbs("POST")]
        ///<summary>
        ///Get List of Flights
        ///</summary>
        ///<remarks>
        ///Flight Statuses
        ///1: Scheduled
        ///4: Canceled

        ///</remarks>


        public IQueryable<ExpFlight> GetFlights(AuthInfo authInfo, DateTime? dfrom = null, DateTime? dto = null, int? status = null, string register = "", string actype = "", string origin = "", string destination = "", string flightNo = "")
        {
            if (!(authInfo.userName == "fs.airpocket" && authInfo.password == "Ap1234@z"))
                return null;

            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ExpFlights

                        select x;
            if (dfrom != null)
            {
                var df = ((DateTime)dfrom).Date;
                query = query.Where(q => q.STDLocal >= df);
            }
            if (dto != null)
            {
                var dt = ((DateTime)dto).Date;
                query = query.Where(q => q.STDLocal <= dt);
            }
            if (status != null)
                query = query.Where(q => q.FlightStatusId == status);
            if (!string.IsNullOrEmpty(register))
                query = query.Where(q => q.Register == register);
            if (!string.IsNullOrEmpty(actype))
                query = query.Where(q => q.AircraftType == actype);
            if (!string.IsNullOrEmpty(origin))
                query = query.Where(q => q.Origin == origin);
            if (!string.IsNullOrEmpty(destination))
                query = query.Where(q => q.Destination == destination);

            if (!string.IsNullOrEmpty(flightNo))
                query = query.Where(q => q.FlightNo == flightNo);

            return query.OrderBy(q => q.STDLocal);


        }
        public class AptRpt
        {
            public int Row { get; set; }
            public int? OutId { get; set; }
            public string OutFlightNo { get; set; }
            public string OutRegister { get; set; }
            public string OutType { get; set; }
            public string OutRoute { get; set; }
            public string OutFrom { get; set; }
            public string OutTo { get; set; }
            public string OutRouteICAO { get; set; }
            public string OutFromICAO { get; set; }
            public string OutToICAO { get; set; }

            public DateTime? OutSTDLocal { get; set; }
            public string OutTimeLocal { get; set; }

            public int? InId { get; set; }
            public string InFlightNo { get; set; }
            public string InRegister { get; set; }
            public string InType { get; set; }
            public string InRoute { get; set; }
            public string InFrom { get; set; }
            public string InTo { get; set; }
            public string InRouteICAO { get; set; }
            public string InFromICAO { get; set; }
            public string InToICAO { get; set; }
            public DateTime? InSTDLocal { get; set; }
            public string InTimeLocal { get; set; }


            public string Temp1 { get; set; }
            public string Temp2 { get; set; }
            public string Temp3 { get; set; }
            public string Temp4 { get; set; }
            public string Temp5 { get; set; }
            public string Airline { get; set; }
            public string BaseApt { get; set; }
            public string Date { get; set; }
            public string DatePersian { get; set; }
            public string Day { get; set; }



        }
        [Route("api/fdps/crew/count")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFDPsCrewCount(DateTime dt1, DateTime dt2)
        {

            var _dt1 = dt1.Date;
            var _dt2 = dt2.Date;
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ViewRosterCrewCounts
                        where x.DateLocal >= _dt1 && x.DateLocal <= _dt2
                        orderby x.DateLocal, x.STDLocal
                        select x;
            var _result = query.ToList();

            return Ok(_result);
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

        [Route("api/crew/certificates/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetEmployeeCertificates(int id)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var certs = context.AppCertificates.Where(q =>
                 q.CrewId == id

           ).OrderBy(q => q.StatusId).ThenBy(q => q.Remain).ToList();

            //var result = await courseService.GetEmployeeCertificates(id);

            return Ok(certs);
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
                var context = new AirpocketAPI.Models.FLYEntities();
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



        [Route("api/day/apts")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetDayApts(DateTime dt)
        {

            var _dt = dt.Date;
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ExpFlights
                        where x.FlightStatusId != 4 && x.DepartureDay == _dt
                        select new { x.Origin, x.Destination };
            var _result = query.ToList();
            var _r1 = _result.Select(q => q.Origin).ToList();
            var _r2 = _result.Select(q => q.Destination).ToList();
            var result = _r1.Concat(_r2).Distinct().OrderBy(q => q).ToList();
            return Ok(result);
        }
        [Route("api/fdr/report")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetViewFDRReport(DateTime df, DateTime dt, int? from = null, int? to = null, string ip = "", string cpt = "", string fo = "", string regs = "", string types = "")
        {
            var _df = df.Date;
            var _dt = dt.Date;//.AddHours(24);
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ViewFDRReports
                        where x.STDDay >= _df && x.STDDay <= _dt
                        select x;
            if (from != null)
            {
                query = query.Where(q => q.FromAirport == from);
            }
            if (to != null)
            {
                query = query.Where(q => q.ToAirport == to);
            }
            if (!string.IsNullOrEmpty(ip))
            {
                var ipids = ip.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
                query = query.Where(q => ipids.Contains(q.IPId));
            }

            if (!string.IsNullOrEmpty(cpt))
            {
                var cptids = cpt.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
                query = query.Where(q => cptids.Contains(q.P1Id));
            }

            if (!string.IsNullOrEmpty(fo))
            {
                var foids = fo.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
                query = query.Where(q => foids.Contains(q.P2Id));
            }

            if (!string.IsNullOrEmpty(regs))
            {
                var regids = regs.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
                query = query.Where(q => regids.Contains(q.RegisterID));
            }

            if (!string.IsNullOrEmpty(types))
            {
                var typeids = types.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
                query = query.Where(q => typeids.Contains(q.TypeId));
            }

            var result = query.OrderBy(q => q.STDDay).ThenBy(q => q.AircraftType).ThenBy(q => q.Register).ThenBy(q => q.STD).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/ofp/sign/check/{ofpid}")]
        public IHttpActionResult GetOFPSignCheck(int ofpid)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.OFPImports.FirstOrDefault(q => q.Id == ofpid);

            if (result == null)
                return Ok(new
                {
                    Data = new { Id = -1 },
                    IsSuccess = true
                });


            return Ok(new
            {
                Data = new
                {
                    result.Id,
                    result.FlightId,
                    result.JLSignedBy,
                    result.JLDatePICApproved,
                    result.PIC,
                    result.PICId
                },
                IsSuccess = true
            });
        }

        public class signcheck
        {
            public List<int?> ofpIds { get; set; }
            public List<int?> flightIds { get; set; }
        }

        [Route("api/ofp/sign/check/group/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult GetOFPSignCheckGroup(signcheck dto)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.OFPImports.Where(q => dto.ofpIds.Contains(q.Id)).ToList();
            var signed = result.Where(q => !string.IsNullOrEmpty(q.JLSignedBy)).Select(
                q => new
                {
                    q.Id,
                    q.FlightId,
                    q.JLSignedBy,
                    q.JLDatePICApproved,
                    q.PIC,
                    q.PICId
                }
                ).ToList();
            return Ok(new
            {
                Data = signed,
                IsSuccess = true
            });
        }

        [Route("api/ofp/sign/check/group/flights/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult GetOFPSignCheckGroupFlights(signcheck dto)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.OFPImports.Where(q => dto.flightIds.Contains(q.FlightId)).ToList();
            var signed = result.Where(q => !string.IsNullOrEmpty(q.JLSignedBy)).Select(
                q => new
                {
                    q.Id,
                    q.FlightId,
                    q.JLSignedBy,
                    q.JLDatePICApproved,
                    q.PIC,
                    q.PICId
                }
                ).ToList();
            return Ok(new
            {
                Data = signed,
                IsSuccess = true
            });
        }

        [HttpGet]
        [Route("api/asr2/flight/{flightId}")]
        public IHttpActionResult GetASRByFlightId(int flightId)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.EFBASRs.FirstOrDefault(q => q.FlightId == flightId);

            return Ok(new
            {
                Data = result,
                IsSuccess = true
            });
        }


        [HttpGet]
        [Route("api/voyage/flight/{flightId}")]
        public IHttpActionResult GetVoyageReportByFlightId(int flightId)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.EFBVoyageReports.FirstOrDefault(q => q.FlightId == flightId);
            //result.EFBFlightIrregularities = context.EFBFlightIrregularities.Where(q => q.VoyageReportId == result.Id).ToList();
            // result.EFBReasons = context.EFBReasons.Where(q => q.VoyageReportId == result.Id).ToList();
            var _vr = new
            {
                result.Id,
                result.FlightId,
                result.Route,
                result.RestReduction,
                result.DutyExtention,
                result.Report,
                result.DatePICSignature,
                result.ActionedById,
                result.DateActioned,
                result.DateConfirmed,
                result.DepDelay,
                result.DateUpdate,
                result.User,
                result.JLSignedBy,
                result.JLDatePICApproved,
                result.PICId,
                result.PIC,
                result.OPSRemark,
                result.OPSRemarkDate,
                result.OPSId,
                result.OPSConfirmDate,
                result.OPSStaffRemark,
                result.OPSStaffDateVisit,
                result.OPSStaffConfirmDate,
                result.OPSStaffId,
                result.OPSStaffRemarkDate,
                result.OPSUser,
                result.OPSStaffUser,
                result.OPSStatusId,
                result.OPSStaffStatusId,
                EFBFlightIrregularities = context.EFBFlightIrregularities.Where(q => q.VoyageReportId == result.Id).Select(q => new { q.Id, q.IrrId }).ToList(),
                EFBReasons = context.EFBReasons.Where(q => q.VoyageReportId == result.Id).Select(q => new { q.Id, q.ReasonId }).ToList()

            };
            object output = new
            {
                Data = _vr,
                IsSuccess = true
            };
            var str = JsonConvert.SerializeObject(output, Formatting.Indented,
new JsonSerializerSettings
{
    PreserveReferencesHandling = PreserveReferencesHandling.Objects
});

            return Ok(str);
        }


        [Route("api/applegs/{crtbl}")]

        //nookp
        public IHttpActionResult GetAppLegs(DateTime? df, DateTime? dt, int? ip, int? cpt, int? status, int? asrvr, int crtbl)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            df = df != null ? ((DateTime)df).Date : DateTime.MinValue.Date;
            dt = dt != null ? ((DateTime)dt).Date : DateTime.MaxValue.Date;
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.AppLegOPS
                            // where x.FlightStatusID != 1 && x.FlightStatusID != 4
                        select x;
            query = query.Where(q => q.STDDay >= df && q.STDDay <= dt);
            if (crtbl == 1)
                query = query.Where(q => q.CRTBL == 1);
            if (ip != null)
                query = query.Where(q => q.IPId == ip);
            if (cpt != null)
                query = query.Where(q => q.P1Id == cpt);
            if (asrvr != null)
            {
                if (asrvr == 1)
                    query = query.Where(q => q.MSN == 1);


            }
            if (status != null)
            {

                List<int?> sts = new List<int?>();
                switch ((int)status)
                {
                    case 1:
                        sts.Add(15);
                        sts.Add(3);
                        query = query.Where(q => sts.Contains(q.FlightStatusID));
                        break;
                    case 2:
                        sts.Add(1);
                        query = query.Where(q => sts.Contains(q.FlightStatusID));
                        break;
                    case 3:
                        sts.Add(4);
                        query = query.Where(q => sts.Contains(q.FlightStatusID));
                        break;
                    case 4:
                        sts.Add(20);
                        sts.Add(21);
                        sts.Add(22);
                        sts.Add(4);
                        sts.Add(2);
                        sts.Add(23);
                        sts.Add(24);
                        sts.Add(25);
                        query = query.Where(q => sts.Contains(q.FlightStatusID));

                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
            }
            var result = query.OrderBy(q => q.STD).ToList();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }



        [Route("api/flighttime/crew/{crewid}/{y1}/{m1}/{y2}/{m2}")]

        //nookp
        public IHttpActionResult GetAppCrewFlightTime(int crewid, int y1, int m1, int y2, int m2)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var df = new DateTime(y1, m1, 1, 0, 0, 0);
            var dt = new DateTime(y2, m2, 1, 0, 0, 0);
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.AppCrewTimes
                        where x.CrewId == crewid && (x.RefDate >= df && x.RefDate <= dt)
                        orderby x.Year, x.Month
                        select x;
            var result = query.ToList();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }

        [Route("api/flighttime/crew/past/month/{crewid}/{m}")]

        //nookp
        public IHttpActionResult GetAppCrewFlightTimePastMonth(int crewid, int m)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var dt2 = DateTime.Now;
            var dt1 = dt2.AddMonths(-1 * m);
            var df = new DateTime(dt1.Year, dt1.Month, 1, 0, 0, 0);
            var dt = new DateTime(dt2.Year, dt2.Month, 1, 0, 0, 0);
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.AppCrewTimes
                        where x.CrewId == crewid && (x.RefDate >= df && x.RefDate <= dt)
                        orderby x.Year, x.Month
                        select x;
            var result = query.ToList();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }

        [Route("api/flighttime/crew/year/{crewid}/{year}")]

        //nookp
        public IHttpActionResult GetAppCrewFlightTimeYear(int crewid, int year)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;

            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.AppCrewTimes
                        where x.CrewId == crewid && x.Year == year
                        orderby x.Year, x.Month
                        select x;
            var result = query.ToList();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }



        [Route("api/ftl")]

        //nookp
        public IHttpActionResult GetAppFTLs(DateTime df, int cockpit)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var context = new AirpocketAPI.Models.FLYEntities();
            df = ((DateTime)df).Date;
            var query = from x in context.AppFTLs
                        where x.CDate == df
                        select x;
            if (cockpit == 1)
                query = query.Where(q => q.IsCockpit == 1);
            if (cockpit == 0)
                query = query.Where(q => q.IsCockpit == 0);
            var result = query.OrderByDescending(q => q.Duty7).ThenByDescending(q => q.Duty14).ThenByDescending(q => q.Duty28).ToList();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }

        //12-11
        public class FTLCrewIds
        {
            public DateTime CDate { get; set; }
            public List<int> CrewIds { get; set; }
        }
        [Route("api/ftl/crews/")]
        [AcceptVerbs("POST")]
        //nookp
        public IHttpActionResult GetAppFTLsByCrewIds(FTLCrewIds dto)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var date = dto.CDate.Date;
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.AppFTLs
                        where x.CDate == date && dto.CrewIds.Contains(x.CrewId)
                        select x;

            var result = query.OrderByDescending(q => q.Duty7).ThenByDescending(q => q.Duty14).ThenByDescending(q => q.Duty28).ToList();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }


        [Route("api/ftl/abs/crews/")]
        [AcceptVerbs("POST")]
        //nookp
        public IHttpActionResult GetAppFTLsABSByCrewIds(FTLCrewIds dto)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var date = dto.CDate.Date;
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.AppFTLAbs
                        where x.CDate == date && dto.CrewIds.Contains(x.CrewId)
                        select x;

            var result = query.OrderByDescending(q => q.Duty7).ThenByDescending(q => q.Duty14).ThenByDescending(q => q.Duty28).ToList();
            var yy = date.Year;
            var mm = date.Month;
            var ratios = context.FTLFlightTimeRatioMonthlies.Where(q => q.Year == yy && q.Month == mm && dto.CrewIds.Contains(q.CrewId)).ToList();
            foreach (var r in ratios)
            {
                var c = result.FirstOrDefault(q => q.CrewId == r.CrewId);
                if (c != null)
                {
                    c.Ratio = Math.Round(Convert.ToDouble(r.Ratio));
                    c.CNT = r.CNT;
                    c.ScheduledFlightTime = r.ScheduledFlightTime;

                }
            }

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }


        [Route("api/duty/timeline/{year}/{month}/{rank}/{type}")]
        [AcceptVerbs("GET")]
        //nookp
        public IHttpActionResult GetDutyTimeLine(int year, int month, string rank, int type)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;

            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.ViewCrewDutyTimeLines
                        where x.YearLocal == year && x.MonthLocal == month
                        select x;
            if (rank.ToLower() == "cabin")
            {
                query = query.Where(q => q.IsCockpit == 0);
            }
            else if (rank == "cockpit")
            {
                query = query.Where(q => q.IsCockpit == 1);
            }
            else
                query = query.Where(q => q.JobGroup == rank);
            if (type != -1)
                query = query.Where(q => q.DutyType == type);

            var duties = query.OrderBy(q => q.GroupOrder).ThenBy(q => q.ScheduleName).ToList();
            //var resuorces = duties.Select(q => new {q.CrewId, id=q.CrewId, text=q.ScheduleName }).ToList();
            var resources = (from x in duties
                             group x by new { x.CrewId, x.ScheduleName, x.JobGroup, x.GroupOrder } into grp
                             select new
                             {
                                 grp.Key.CrewId,
                                 id = grp.Key.CrewId,

                                 grp.Key.ScheduleName,
                                 text = "(" + grp.Key.JobGroup + ")" + grp.Key.ScheduleName,
                                 grp.Key.JobGroup,
                                 grp.Key.GroupOrder,
                             }).OrderBy(q => q.GroupOrder).ThenBy(q => q.ScheduleName).ToList();


            // return result.OrderBy(q => q.STD);
            return Ok(new
            {
                duties,
                resources
            });

        }

        //2023
        [Route("api/duty/timeline/{rank}/{type}")]
        [AcceptVerbs("GET")]
        //nookp
        public IHttpActionResult GetDutyTimeLine(DateTime df, DateTime dt, string rank, int type)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            df = df.Date;
            dt = dt.Date.AddDays(1);
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.ViewCrewDutyTimeLines
                            //where x.YearLocal == year && x.MonthLocal == month
                        where x.StartLocal >= df && x.StartLocal <= dt
                        select x;
            if (rank.ToLower() == "cabin")
            {
                query = query.Where(q => q.IsCockpit == 0);
            }
            else if (rank.ToLower() == "cockpit")
            {
                query = query.Where(q => q.IsCockpit == 1);
            }
            else if (rank == "IP,P1" || rank == "ISCCM,SCCM")
            {
                query = query.Where(q => q.JobGroup2 == rank);
            }
            // else
            //    query = query.Where(q => q.JobGroup == rank);
            if (type != -1)
                query = query.Where(q => q.DutyType == type);

            var duties = query.OrderBy(q => q.GroupOrder).ThenBy(q => q.ScheduleName).ToList();
            var resuorces = duties.Select(q => new { q.CrewId, id = q.CrewId, text = q.ScheduleName }).ToList();



            var resources = (from x in duties
                             group x by new { x.CrewId, x.ScheduleName, x.JobGroup, x.GroupOrder } into grp
                             select new
                             {
                                 grp.Key.CrewId,
                                 id = grp.Key.CrewId,

                                 grp.Key.ScheduleName,
                                 text = "(" + grp.Key.JobGroup + ")" + grp.Key.ScheduleName,
                                 grp.Key.JobGroup,
                                 grp.Key.GroupOrder,
                             }).OrderBy(q => q.GroupOrder).ThenBy(q => q.ScheduleName).ToList();


            // return result.OrderBy(q => q.STD);
            return Ok(new
            {
                duties,
                resources
            });

        }

        [Route("api/assign/grid")]
        [AcceptVerbs("GET")]
        //nookp
        public IHttpActionResult GetAssignGrid(DateTime df, DateTime dt)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            df = df.Date;
            dt = dt.Date.AddDays(1);
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.ViewAssignGrids
                            //where x.YearLocal == year && x.MonthLocal == month
                        where x.StartLocal >= df && x.StartLocal <= dt
                        select x;


            var duties = query.OrderBy(q => q.GroupOrder).ThenBy(q => q.ScheduleName).ToList();

            return Ok(new
            {
                duties,
                // resources
            });

        }


        [Route("api/assign/grid/group/{code}")]
        [AcceptVerbs("GET")]
        //nookp
        public IHttpActionResult GetAssignGridGroup(string code, DateTime df, DateTime dt)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            df = df.Date;
            dt = dt.Date.AddDays(1);
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.ViewAssignGrids
                            //where x.YearLocal == year && x.MonthLocal == month
                        where x.StartLocal >= df && x.StartLocal <= dt && x.JobGroupCode.StartsWith(code)
                        select x;


            var duties = query.OrderBy(q => q.GroupOrder).ThenBy(q => q.ScheduleName).ToList();

            return Ok(new
            {
                duties,
                // resources
            });

        }




        [Route("api/duty/timeline/crew/{id}")]
        [AcceptVerbs("GET")]
        //nookp
        public IHttpActionResult GetDutyTimeLineByCrew(DateTime df, DateTime dt, int id)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            df = df.Date;
            dt = dt.Date.AddDays(1);
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.ViewCrewDutyTimeLines
                            //where x.YearLocal == year && x.MonthLocal == month
                        where x.StartLocal >= df && x.StartLocal <= dt && x.CrewId == id
                        select x;


            var duties = query.OrderByDescending(q => q.StartLocal).ToList();
            //var resuorces = duties.Select(q => new {q.CrewId, id=q.CrewId, text=q.ScheduleName }).ToList();


            // return result.OrderBy(q => q.STD);
            return Ok(duties);

        }

        [Route("api/duty/timeline/{id}")]
        [AcceptVerbs("GET")]
        //nookp
        public IHttpActionResult GetDutyTimeLineByDutyId(int id)
        {

            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.ViewCrewDutyTimeLines
                            //where x.YearLocal == year && x.MonthLocal == month
                        where x.Id == id
                        select x;


            var duties = query.FirstOrDefault();
            //var resuorces = duties.Select(q => new {q.CrewId, id=q.CrewId, text=q.ScheduleName }).ToList();


            // return result.OrderBy(q => q.STD);
            return Ok(duties);

        }

        [Route("api/ftl/crew/date/")]

        //nookp
        public IHttpActionResult GetAppFTLCrew(DateTime df, int crew)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var context = new AirpocketAPI.Models.FLYEntities();
            df = ((DateTime)df).Date;
            var query = from x in context.AppFTLs
                        where x.CDate == df && x.CrewId == crew
                        select x;

            var result = query.FirstOrDefault();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }

        [Route("api/ftl/crew/date/range/")]

        //nookp
        public IHttpActionResult GetAppFTLCrewRange(DateTime df, DateTime dt, int crew)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var context = new AirpocketAPI.Models.FLYEntities();
            df = ((DateTime)df).Date;
            dt = ((DateTime)dt).Date;
            var query = from x in context.AppFTLs
                        where (x.CDate >= df && x.CDate <= dt) && x.CrewId == crew
                        select x;

            var result = query.OrderBy(q => q.CDate).ToList();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }

        [Route("api/ftl/crew/date/range/exceed")]

        //nookp
        public IHttpActionResult GetAppFTLCrewRangeExceed(DateTime? df, DateTime? dt, int crew)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var context = new AirpocketAPI.Models.FLYEntities();
            df = df != null ? ((DateTime)df).Date : DateTime.MinValue.Date;
            dt = dt != null ? ((DateTime)dt).Date : DateTime.MaxValue.Date;
            var query = from x in context.AppFTLs
                        where (x.CDate >= df && x.CDate <= dt) && x.CrewId == crew &&
                        (x.Duty7Remain < 0 || x.Duty28Remain < 0 || x.Duty14Remain < 0 || x.Flight28Remain < 0 || x.FlightCYearRemain < 0 || x.FlightYearRemain < 0)
                        select x;

            var result = query.OrderBy(q => q.CDate).ToList();

            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }

        [Route("api/crew/valid/{cid}")]

        //nookp
        public IHttpActionResult GetValidCrewForRoster(int cid, DateTime dt)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = context.ViewCrewValidFTLs.ToList();



            // return result.OrderBy(q => q.STD);
            return Ok(query);

        }

        [Route("api/crew/valid/group/{code}")]

        //nookp
        public IHttpActionResult GetValidCrewForRosterGroup(string code, DateTime dt)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = context.ViewCrewValidFTLs.Where(q => q.JobGroupCode.StartsWith(code)).ToList();



            // return result.OrderBy(q => q.STD);
            return Ok(query);

        }

        public int getOrder(string g)
        {
            switch (g)
            {
                case "TRE":
                    return 1;
                case "TRI":
                    return 2;
                case "LTC":
                    return 3;
                case "P1":
                    return 4;
                case "P2":
                    return 5;
                case "ISCCM":
                    return 6;
                case "SCCM":
                    return 7;
                case "CCM":
                    return 8;
                default:
                    return 1000;
            }
        }
        [Route("api/roster/report/date/")]

        //nookp
        public IHttpActionResult GetRosterReport(DateTime df, int revision)
        {
            //nooz
            //this.context.Database.CommandTimeout = 160;
            var context = new AirpocketAPI.Models.FLYEntities();
            df = ((DateTime)df).Date;
            var query = from x in context.ReportRosters
                        where x.STDDay == df
                        orderby x.Register, x.STD
                        select x;

            var result = query.ToList();
            var main = result.Select(q => new
            {
                q.FlightNumber,
                q.Register,
                q.FromAirportIATA,
                q.ToAirportIATA,
                DepLocal = q.STDLOC,
                ArrLocal = q.STALOC,
                Dep = q.STD,
                Arr = q.STA,
                q.IP,
                q.CPT,
                q.FO,
                q.SAFETY,
                q.CHECK,
                q.OBS,
                q.ISCCM,
                q.SCCM,
                q.CCM,
                q.CHECKC,
                q.OBSC,
                q.FM,
                q.POSITIONING,
                q.POSITIONINGCABIN,
                q.POSITIONINGCOCKPIT
            }).ToList();
            var hasIP = result.Where(q => !string.IsNullOrEmpty(q.IP)).FirstOrDefault() != null;
            var hasCPT = result.Where(q => !string.IsNullOrEmpty(q.CPT)).FirstOrDefault() != null;
            var hasFO = result.Where(q => !string.IsNullOrEmpty(q.FO)).FirstOrDefault() != null;
            var hasSAFETY = result.Where(q => !string.IsNullOrEmpty(q.SAFETY)).FirstOrDefault() != null;
            var hasOBS = result.Where(q => !string.IsNullOrEmpty(q.OBS)).FirstOrDefault() != null;
            var hasISCCM = result.Where(q => !string.IsNullOrEmpty(q.ISCCM)).FirstOrDefault() != null;
            var hasOBSC = result.Where(q => !string.IsNullOrEmpty(q.OBSC)).FirstOrDefault() != null;
            var hasCHECK = result.Where(q => !string.IsNullOrEmpty(q.CHECK)).FirstOrDefault() != null;
            var hasCHECKC = result.Where(q => !string.IsNullOrEmpty(q.CHECKC)).FirstOrDefault() != null;
            var hasFM = result.Where(q => !string.IsNullOrEmpty(q.FM)).FirstOrDefault() != null;
            var obsQuery = result.Where(q => !string.IsNullOrEmpty(q.POSITIONING)).OrderBy(q => q.STD).ToList();
            List<string> obsList = new List<string>();
            foreach (var x in obsQuery)
            {
                obsList.Add("[(" + x.FlightNumber + ") " + x.POSITIONING + "]");
            }
            var positioning = string.Join(" ", obsList);
            var ticketQuery = (from x in context.ViewTickets
                               where x.DateLocal == df
                               orderby x.JobGroup
                               select x).ToList();
            if (ticketQuery != null && ticketQuery.Count > 0)
            {
                List<string> tickets = new List<string>();
                foreach (var x in ticketQuery)
                    tickets.Add("[ (" + x.PosAirline + " " + x.PosFrom + "-" + x.PosTo + " " + x.FlightNumber + ") " + x.ScheduleName + "(" + x.JobGroup + ")" + " ]");
                positioning += "  TICKET(s): " + string.Join(" ", tickets);
            }
            var dutiesQuery = (from x in context.ViewCrewDuties
                               where x.DateLocal == df && (x.DutyType == 1167 || x.DutyType == 1168 || x.DutyType == 300013 || x.DutyType == 1170 || x.DutyType == 5000 || x.DutyType == 5001
                               || x.DutyType == 100001 || x.DutyType == 100003)
                               select x).ToList();
            var duties = (from x in dutiesQuery
                          group x by new { x.DutyType, x.DutyTypeTitle } into grp
                          select new
                          {
                              grp.Key.DutyType,
                              grp.Key.DutyTypeTitle,
                              items = grp.Select(q => new {
                                  // Title = q.ScheduleName + " (" + q.JobGroup + ")",
                                  Title = grp.Key.DutyType != 300013 ? q.ScheduleName + " (" + q.JobGroup + ")" : q.ScheduleName + " (" + q.JobGroup + ")" + " (" + ((DateTime)q.Start).ToString("HHmm") + " - " + ((DateTime)q.End).ToString("HHmm") + ")",
                                  q.IsCockpit, q.JobGroup, GroupOrder = getOrder(q.JobGroup) }).OrderBy(q => q.GroupOrder).ToList(),
                              itemsStr = string.Join(", ", grp.Select(q => new { 
                                  Title = grp.Key.DutyType!=300013? q.ScheduleName + " (" + q.JobGroup + ")" : q.ScheduleName + " (" + q.JobGroup + ")"+" ("+((DateTime) q.Start).ToString("HHmm")+" - "+ ((DateTime)q.End).ToString("HHmm") + ")",
                                  q.IsCockpit, q.JobGroup, GroupOrder = getOrder(q.JobGroup) }).OrderBy(q => q.GroupOrder).Select(q => q.Title).ToList())
                          }).OrderBy(q => q.DutyType).ToList();

            // return result.OrderBy(q => q.STD);
            var output = new
            {
                main,
                positioning,
                duties,
                date = df,
                pdate = result.First().PDATE,
                day = result.First().DayName,
                revision,
                hasIP,
                hasCPT,
                hasFO,
                hasSAFETY,
                hasOBS,
                hasCHECK,
                hasISCCM,
                hasOBSC,
                hasCHECKC,
                hasFM
            };
            return Ok(output);

        }
        [Route("api/jl/{fid}")]

        //nookp
        public IHttpActionResult GetJourneyLog(int fid)
        {
            //nooz 
            //this.context.Database.CommandTimeout = 160;

            var context = new AirpocketAPI.Models.FLYEntities();
            var appleg = context.AppLegs.FirstOrDefault(q => q.FlightId == fid);
            var pid = appleg.PICId;
            var appFlight = context.AppCrewFlightJLs.Where(q => q.FlightId == fid && q.CrewId == pid).FirstOrDefault();
            var crewlegs = context.AppCrewFlightJLs.Where(q => q.FDPId == appFlight.FDPId).ToList();
            var clegs = crewlegs.Select(q => (int)q.FlightId).ToList();
            var legs = context.AppLegJLs.Where(q => clegs.Contains(q.FlightId)).OrderBy(q => q.STD).ToList();
            var fdp = context.ViewFDPRests.FirstOrDefault(q => q.Id == appFlight.FDPId);
            var asr = legs.FirstOrDefault(q => q.AttASR == true) != null;
            var vr = legs.FirstOrDefault(q => q.AttVoyageReport == true) != null;
            var pos1 = legs.FirstOrDefault(q => q.AttRepositioning1 == true) != null;
            var pos2 = legs.FirstOrDefault(q => q.AttRepositioning2 == true) != null;


            // var flight = legs.Sum(q => (((DateTime)(q.Landing!=null?q.Landing:q.STA) - (DateTime)(q.TakeOff!=null?q.TakeOff:q.STD))).TotalMinutes);
            // var block = legs.Sum(q => (((DateTime)(q.BlockOn!=null?q.BlockOn:q.STA) - (DateTime)(q.BlockOff!=null?q.BlockOff:q.STD))).TotalMinutes);
            var flight = legs.Sum(q => (q.Landing != null && q.TakeOff != null) ? ((DateTime)q.Landing - (DateTime)q.TakeOff).TotalMinutes : 0);
            var block = legs.Sum(q => (q.BlockOn != null && q.BlockOff != null) ? ((DateTime)q.BlockOn - (DateTime)q.BlockOff).TotalMinutes : 0);
            var fids = legs.Select(q => (Nullable<int>)q.FlightId).ToList();
            var _crews2 = (from x in context.ViewFlightCrewNews
                               //where x.FlightId == flightId

                           where fids.Contains(x.FlightId) //&& x.IsPositioning != true
                           orderby x.IsPositioning, x.GroupOrder

                           select new CLJLData()
                           {
                               CrewId = x.CrewId,
                               IsPositioning = x.IsPositioning,
                               PositionId = x.PositionId,
                               Position = x.Position,
                               Name = x.Name,
                               GroupId = x.GroupId,
                               JobGroup = x.JobGroup,
                               JobGroupCode = x.JobGroupCode,
                               GroupOrder = x.GroupOrder,
                               IsCockpit = x.IsCockpit,
                               FlightId = x.FlightId,

                           }).ToList();

            var _gcrews = (from x in _crews2
                           group x by new
                           {
                               x.CrewId,
                               x.IsPositioning,
                               x.PositionId,
                               x.Position,
                               x.Name,
                               x.GroupId,
                               x.JobGroup,
                               x.JobGroupCode,
                               x.GroupOrder,
                               x.IsCockpit,
                           } into grp
                           select grp).ToList();
            var query = (from x in _gcrews
                         let xfids = x.Select(q => Convert.ToInt32(q.FlightId)).ToList()
                         select new CLJLData()
                         {
                             CrewId = x.Key.CrewId,
                             IsPositioning = x.Key.IsPositioning,
                             PositionId = x.Key.PositionId,
                             Position = x.Key.Position,
                             Name = x.Key.Name,
                             GroupId = x.Key.GroupId,
                             JobGroup = x.Key.JobGroup,
                             JobGroupCode = x.Key.JobGroupCode,
                             GroupOrder = x.Key.GroupOrder,
                             IsCockpit = x.Key.IsCockpit,
                             Legs = legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList(),
                             LegsStr = string.Join("-", legs.Where(q => xfids.Contains((int)q.FlightId)).OrderBy(q => q.STD).Select(q => q.FlightNumber).Distinct().ToList()),

                         }).ToList();


            foreach (var x in query)
            {
                if (x.Legs.Count == fids.Count)
                    x.LegsStr = "";
            }

            var result = new
            {
                legs,
                AcType = legs.First().AircraftType,
                Reg = legs.First().Register,
                Date = legs.First().STDDay,
                STD = legs.First().STD,
                fdp.ReportingTime,
                fdp.DutyEnd,
                FDPId = fdp.Id,
               // PIC = legs.First().PIC,
               PIC= appleg.PIC,

                MaxFDP = fdp.MaxFDPExtended,
                fdp.FDP,
                fdp.Duty,
                flight,
                block,
                asr,
                vr,
                pos1,
                pos2,
                crew = query


            };





            // return result.OrderBy(q => q.STD);
            return Ok(result);

        }

        [Route("api/vr/remark/manager/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostVrRemarkManager(dynamic dto)
        {
            string user = Convert.ToString(dto.user);
            string remark = Convert.ToString(dto.remark);
            int status = Convert.ToInt32(dto.status);
            int id = Convert.ToInt32(dto.id);

            var context = new AirpocketAPI.Models.FLYEntities();

            var vr = context.EFBVoyageReports.FirstOrDefault(q => q.Id == id);
            if (vr.OPSStatusId == null)
                vr.OPSStatusId = 0;
            if (vr.OPSRemark == null)
                vr.OPSRemark = "";
            if (vr != null && (vr.OPSStaffStatusId == null || vr.OPSStaffStatusId == 0))
            {
                //if (string.IsNullOrEmpty(remark.Trim().Replace(" ","")))
                //{
                //    vr.OPSRemark = "";
                //    vr.OPSRemarkDate = null;
                //    vr.OPSUser = "";
                //    vr.OPSStatusId = 0;

                //    vr.OPSStaffConfirmDate = null;
                //    vr.OPSStaffDateVisit = null;
                //    vr.OPSStaffRemark = null;
                //    vr.OPSStaffRemarkDate = null;
                //    vr.OPSStaffUser = null;
                //    vr.OPSStaffStatusId = 0;

                //    context.SaveChanges();

                //}
                //else
                {
                    var isnew = vr.OPSRemark.Trim().Replace(" ", "") != remark.Trim().Replace(" ", "") || vr.OPSStatusId != status || vr.OPSUser != user;
                    if (isnew)
                    {
                        vr.OPSRemark = remark;
                        vr.OPSUser = user;
                        vr.OPSRemarkDate = DateTime.Now;
                        vr.OPSStatusId = status;

                        // vr.OPSStaffConfirmDate = null;
                        // vr.OPSStaffDateVisit = null;
                        // vr.OPSStaffRemark = null;
                        // vr.OPSStaffRemarkDate = null;
                        // vr.OPSStaffUser = null;
                        vr.OPSStaffStatusId = 0;


                        context.SaveChanges();

                    }
                }
            }




            return Ok(true);
        }


        [Route("api/vr/remark/staff/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostVrRemarkStaff(dynamic dto)
        {
            string user = Convert.ToString(dto.user);
            string remark = Convert.ToString(dto.remark);
            int status = Convert.ToInt32(dto.status);
            int id = Convert.ToInt32(dto.id);

            var context = new AirpocketAPI.Models.FLYEntities();

            var vr = context.EFBVoyageReports.FirstOrDefault(q => q.Id == id);
            if (vr.OPSStaffStatusId == null)
                vr.OPSStaffStatusId = 0;
            if (vr.OPSStaffRemark == null)
                vr.OPSStaffRemark = "";
            if (vr != null)
            {

                var isnew = vr.OPSStaffRemark.Trim().Replace(" ", "") != remark.Trim().Replace(" ", "") || vr.OPSStaffStatusId != status || vr.OPSStaffUser != user;
                if (isnew)
                {
                    vr.OPSStaffRemark = remark;
                    vr.OPSStaffUser = user;
                    vr.OPSStaffRemarkDate = DateTime.Now;
                    vr.OPSStaffStatusId = status;




                    context.SaveChanges();

                }

            }




            return Ok(true);
        }


        [Route("api/asr/remark/manager/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostASRRemarkManager(dynamic dto)
        {
            string user = Convert.ToString(dto.user);
            string remark = Convert.ToString(dto.remark);
            int status = Convert.ToInt32(dto.status);
            int id = Convert.ToInt32(dto.id);

            var context = new AirpocketAPI.Models.FLYEntities();

            var vr = context.EFBASRs.FirstOrDefault(q => q.Id == id);
            if (vr.OPSStatusId == null)
                vr.OPSStatusId = 0;
            if (vr.OPSRemark == null)
                vr.OPSRemark = "";
            if (vr != null && (vr.OPSStaffStatusId == null || vr.OPSStaffStatusId == 0))
            {
                //if (string.IsNullOrEmpty(remark.Trim().Replace(" ", "")))
                //{
                //    vr.OPSRemark = "";
                //    vr.OPSRemarkDate = null;
                //    vr.OPSUser = "";
                //    vr.OPSStatusId = 0;

                //    vr.OPSStaffConfirmDate = null;
                //    vr.OPSStaffDateVisit = null;
                //    vr.OPSStaffRemark = null;
                //    vr.OPSStaffRemarkDate = null;
                //    vr.OPSStaffUser = null;
                //    vr.OPSStaffStatusId = 0;

                //    context.SaveChanges();

                //}
                //else
                {
                    var isnew = vr.OPSRemark.Trim().Replace(" ", "") != remark.Trim().Replace(" ", "") || vr.OPSStatusId != status || vr.OPSUser != user;
                    if (isnew)
                    {
                        vr.OPSRemark = remark;
                        vr.OPSUser = user;
                        vr.OPSRemarkDate = DateTime.Now;
                        vr.OPSStatusId = status;

                        // vr.OPSStaffConfirmDate = null;
                        // vr.OPSStaffDateVisit = null;
                        // vr.OPSStaffRemark = null;
                        // vr.OPSStaffRemarkDate = null;
                        // vr.OPSStaffUser = null;
                        vr.OPSStaffStatusId = 0;


                        context.SaveChanges();

                    }
                }
            }




            return Ok(true);
        }


        [Route("api/asr/remark/staff/")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostASRRemarkStaff(dynamic dto)
        {
            string user = Convert.ToString(dto.user);
            string remark = Convert.ToString(dto.remark);
            int status = Convert.ToInt32(dto.status);
            int id = Convert.ToInt32(dto.id);

            var context = new AirpocketAPI.Models.FLYEntities();

            var vr = context.EFBASRs.FirstOrDefault(q => q.Id == id);
            if (vr.OPSStaffStatusId == null)
                vr.OPSStaffStatusId = 0;
            if (vr.OPSStaffRemark == null)
                vr.OPSStaffRemark = "";
            if (vr != null)
            {

                var isnew = vr.OPSStaffRemark.Trim().Replace(" ", "") != remark.Trim().Replace(" ", "") || vr.OPSStaffStatusId != status || vr.OPSStaffUser != user;
                if (isnew)
                {
                    vr.OPSStaffRemark = remark;
                    vr.OPSStaffUser = user;
                    vr.OPSStaffRemarkDate = DateTime.Now;
                    vr.OPSStaffStatusId = status;




                    context.SaveChanges();

                }

            }




            return Ok(true);
        }



        [Route("api/person/history/save")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostPersonHistorySave(PersonHistory his)
        {


            var context = new AirpocketAPI.Models.FLYEntities();
            his.DateCreate = DateTime.Now;
            context.PersonHistories.Add(his);
            var x = context.SaveChanges();
            return Ok(x);
        }

        [Route("api/person/telegram")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostTelegram(dynamic dto)
        {
            int eid = Convert.ToInt32(dto.eid);
            string tel = Convert.ToString(dto.tel);
            var context = new AirpocketAPI.Models.FLYEntities();
            var personId = context.ViewEmployeeLights.Where(q => q.Id == eid).Select(q => q.PersonId).FirstOrDefault();

            var person = context.People.Where(q => q.Id == personId).FirstOrDefault();

            if (person == null)
                return Ok(0);

            person.Telegram = tel;
            context.SaveChanges();
            return Ok(1);


        }

        [Route("api/flights/apt")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsByApt(DateTime dt, string apt, string airline, string user = "", string phone = "")
        {

            var _dt = dt.Date;
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ExpFlights
                        where x.FlightStatusId != 4 && x.DepartureDay == _dt && (x.Origin == apt || x.Destination == apt)
                        select x;
            var _result = query.ToList();
            var outResult = _result.Where(q => q.Origin == apt).OrderBy(q => q.STDLocal).ToList();
            var inResult = _result.Where(q => q.Destination == apt).OrderBy(q => q.STDLocal).ToList();
            var cnt = outResult.Count;
            if (inResult.Count > cnt)
                cnt = inResult.Count;

            var result = new List<AptRpt>();
            for (var c = 0; c < cnt; c++)
            {
                var row = new AptRpt()
                {
                    Row = c + 1,
                    Airline = airline,
                    BaseApt = apt,
                    Date = _dt.ToString("yyyy/MM/dd"),
                    DatePersian = _result.First().PersianDate,
                    Day = _result.First().PersianDayName,
                    Temp1 = user,
                    Temp2 = phone
                };

                if (c <= outResult.Count - 1)
                {
                    row.OutId = outResult[c].Id;
                    row.OutFlightNo = outResult[c].FlightNo;
                    row.OutRegister = outResult[c].Register;
                    row.OutType = outResult[c].AircraftType.ToLower().StartsWith("b737") ? "B737" : outResult[c].AircraftType;
                    row.OutRoute = outResult[c].Origin + "-" + outResult[c].Destination;
                    row.OutFrom = outResult[c].Origin;
                    row.OutTo = outResult[c].Destination;
                    row.OutRouteICAO = outResult[c].OriginICAO + "-" + outResult[c].DestinationICAO;
                    row.OutFromICAO = outResult[c].OriginICAO;
                    row.OutToICAO = outResult[c].DestinationICAO;
                    row.OutSTDLocal = outResult[c].STDLocal;
                    row.OutTimeLocal = ((DateTime)outResult[c].STDLocal).ToString("HH:mm");


                }
                if (c <= inResult.Count - 1)
                {
                    row.InId = inResult[c].Id;
                    row.InFlightNo = inResult[c].FlightNo;
                    row.InRegister = inResult[c].Register;
                    row.InType = inResult[c].AircraftType.ToLower().StartsWith("b737") ? "B737" : inResult[c].AircraftType;
                    row.InRoute = inResult[c].Origin + "-" + inResult[c].Destination;
                    row.InFrom = inResult[c].Origin;
                    row.InTo = inResult[c].Destination;
                    row.InRouteICAO = inResult[c].OriginICAO + "-" + inResult[c].DestinationICAO;
                    row.InFromICAO = inResult[c].OriginICAO;
                    row.InToICAO = inResult[c].DestinationICAO;
                    row.InSTDLocal = inResult[c].STALocal;
                    row.InTimeLocal = ((DateTime)inResult[c].STALocal).ToString("HH:mm");
                }


                result.Add(row);

            }




            return Ok(result);


        }

        [Route("api/flights/fdpitem/count/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFDPItemsCount(int id)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var ids = context.FDPItems.Where(q => q.FlightId == id).Select(q => q.FDPId).ToList();
            if (ids.Count == 0)
                return Ok(0);


            var fdps = context.FDPs.Where(q => ids.Contains(q.Id) && !q.IsTemplate).Count();

            return Ok(fdps);
        }
        [Route("api/flights/fdpitem/cnt/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFDPItemsCount2(int id)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var cnt = context.FDPItems.Where(q => q.FlightId == id).Count();
            return Ok(cnt);
        }
        public string getAcType(string type)
        {
            if (type.ToLower().Contains("md82"))
                return "MD-82";
            if (type.ToLower().Contains("md83"))
                return "MD-83";
            if (type.ToLower().Contains("734"))
                return "B737";
            if (type.ToLower().Contains("735"))
                return "B737";
            return type;
        }
        [Route("api/flights/apt/range/{grouped}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsByAptRange(int grouped, DateTime dtfrom, DateTime dtto, string apt, string airline, string user = "", string phone = "")
        {

            var _dtfrom = dtfrom.Date;
            var _dtto = dtto.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ExpFlights
                        where x.FlightStatusId != 4 && (x.DepartureDayLocal >= _dtfrom && x.DepartureDayLocal <= _dtto) && (x.Origin == apt || x.Destination == apt)
                        select x;
            var _result = query.OrderBy(q => q.DepartureDayLocal).ToList();

            var dates = _result.Select(q => q.DepartureDayLocal).Distinct().OrderBy(q => q).ToList();
            var result = new List<AptRpt>();
            foreach (var _dt1 in dates)
            {
                var _dt = (DateTime)_dt1;
                var subResult = _result.Where(q => q.DepartureDayLocal == _dt).ToList();
                var outResult = subResult.Where(q => q.Origin == apt).OrderBy(q => q.STDLocal).ToList();
                var inResult = subResult.Where(q => q.Destination == apt).OrderBy(q => q.STALocal).ToList();
                var cnt = outResult.Count;
                if (inResult.Count > cnt)
                    cnt = inResult.Count;
                for (var c = 0; c < cnt; c++)
                {
                    var row = new AptRpt()
                    {
                        Row = c + 1,
                        Airline = airline,
                        BaseApt = apt,
                        Date = _dt.ToString("yyyy/MM/dd"),
                        DatePersian = subResult.First().PersianDate,
                        Day = subResult.First().PersianDayName,
                        Temp1 = user,
                        Temp2 = phone,
                        Temp3 = _result.First().PersianDate,
                        Temp4 = _result.Last().PersianDate
                    };

                    if (c <= outResult.Count - 1)
                    {
                        row.OutId = outResult[c].Id;
                        row.OutFlightNo = outResult[c].FlightNo;
                        row.OutRegister = outResult[c].Register;
                        row.OutType = getAcType(outResult[c].AircraftType);
                        row.OutRoute = outResult[c].Origin + "-" + outResult[c].Destination;
                        row.OutFrom = outResult[c].Origin;
                        row.OutTo = outResult[c].Destination;
                        row.OutRouteICAO = outResult[c].OriginICAO + "-" + outResult[c].DestinationICAO;
                        row.OutFromICAO = outResult[c].OriginICAO;
                        row.OutToICAO = outResult[c].DestinationICAO;
                        row.OutSTDLocal = outResult[c].STDLocal;
                        row.OutTimeLocal = ((DateTime)outResult[c].STDLocal).ToString("HH:mm");


                    }

                    ///////////  LINKED  TO OUT FLIGHT
                    try
                    {
                        var _fn = Convert.ToInt32(row.OutFlightNo);
                        var _inresult = inResult.Where(q => Convert.ToInt32(q.FlightNo) == (_fn + 1)).FirstOrDefault();
                        if (_inresult != null)
                        {
                            row.InId = _inresult.Id;
                            row.InFlightNo = _inresult.FlightNo;
                            row.InRegister = _inresult.Register;
                            row.InType = getAcType(_inresult.AircraftType);
                            row.InRoute = _inresult.Origin + "-" + _inresult.Destination;
                            row.InFrom = _inresult.Origin;
                            row.InTo = _inresult.Destination;
                            row.InRouteICAO = _inresult.OriginICAO + "-" + _inresult.DestinationICAO;
                            row.InFromICAO = _inresult.OriginICAO;
                            row.InToICAO = _inresult.DestinationICAO;
                            row.InSTDLocal = _inresult.STALocal;
                            row.InTimeLocal = ((DateTime)_inresult.STALocal).ToString("HH:mm");
                            if (_inresult.ArrivalDayLocal != _inresult.DepartureDayLocal)
                                row.InTimeLocal = row.InTimeLocal + "+1";
                        }
                        else
                        {
                            var flt = inResult.Where(q => q.Register == row.OutRegister && q.STD > outResult[c].STD).OrderBy(q => q.STD).FirstOrDefault();
                            if (flt != null)
                            {
                                row.InId = flt.Id;
                                row.InFlightNo = flt.FlightNo;
                                row.InRegister = flt.Register;
                                row.InType = getAcType(flt.AircraftType);
                                row.InRoute = flt.Origin + "-" + flt.Destination;
                                row.InFrom = flt.Origin;
                                row.InTo = flt.Destination;
                                row.InRouteICAO = flt.OriginICAO + "-" + flt.DestinationICAO;
                                row.InFromICAO = flt.OriginICAO;
                                row.InToICAO = flt.DestinationICAO;
                                row.InSTDLocal = flt.STALocal;
                                row.InTimeLocal = ((DateTime)flt.STALocal).ToString("HHmm");
                                if (flt.ArrivalDayLocal != flt.DepartureDayLocal)
                                    row.InTimeLocal = row.InTimeLocal + "+1";
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    ////////////////////////


                    ///// SORTED BY ARRIVAL
                    //if (c <= inResult.Count - 1)
                    //{
                    //    row.InId = inResult[c].Id;
                    //    row.InFlightNo = inResult[c].FlightNo;
                    //    row.InRegister = inResult[c].Register;
                    //    row.InType = getAcType(inResult[c].AircraftType);
                    //    row.InRoute = inResult[c].Origin + "-" + inResult[c].Destination;
                    //    row.InFrom = inResult[c].Origin;
                    //    row.InTo = inResult[c].Destination;
                    //    row.InRouteICAO = inResult[c].OriginICAO + "-" + inResult[c].DestinationICAO;
                    //    row.InFromICAO = inResult[c].OriginICAO;
                    //    row.InToICAO = inResult[c].DestinationICAO;
                    //    row.InSTDLocal = inResult[c].STALocal;
                    //    row.InTimeLocal = ((DateTime)inResult[c].STALocal).ToString("HH:mm");
                    //    if (inResult[c].ArrivalDayLocal != inResult[c].DepartureDayLocal)
                    //        row.InTimeLocal = row.InTimeLocal + "+1";
                    //}
                    //////////////////////////////////////


                    result.Add(row);

                }

            }




            if (grouped == 0)
                return Ok(result);
            else
            {
                var gresult = (from x in result
                               group x by new { x.Date, x.DatePersian, x.Day, x.Airline, x.BaseApt } into grp
                               select new
                               {
                                   //grp.Key.Airline,
                                   //grp.Key.BaseApt,
                                   Airline = _result.First().PersianDate,
                                   BaseApt = _result.Last().PersianDate,
                                   grp.Key.Date,
                                   grp.Key.DatePersian,
                                   grp.Key.Day,
                                   Items = grp.ToList()
                               }).ToList();

                return Ok(gresult);
            }


        }


        [Route("api/flights/apt/range/type2/{grouped}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsByAptRangeType2(
            int grouped, DateTime dtfrom, string apt, string airline, string user = "", string phone = ""
            )
        {
            // var dtfrom = new DateTime(2021,7,31);
            // var dtto = new DateTime(2021, 7, 31);
            //var airline = "CSPN";
            //var apt = "THR";
            //var user = "X";
            //var phone = "0912";
            var _dtfrom = dtfrom.Date;
            var _dtto = _dtfrom.AddHours(23).AddMinutes(59).AddSeconds(59);
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.ExpFlights
                        where x.FlightStatusId != 4
                        //&& (x.DepartureDayLocal >= _dtfrom && x.DepartureDayLocal <= _dtto) 
                        && (x.DepartureDayLocal == _dtfrom || x.ArrivalDayLocal == _dtfrom)
                        && (x.Origin == apt || x.Destination == apt)
                        select x;
            var _result = query.OrderBy(q => q.DepartureDayLocal).ToList();

            var dates = new List<DateTime>() { _dtfrom };// _result.Select(q => q.DepartureDayLocal).Distinct().OrderBy(q => q).ToList();
            var predates = new List<DateTime>() { _dtfrom.AddDays(-1) };
            var lastRec = _result.Last();
            var result = new List<AptRpt>();

            var _cc = 0;
            foreach (var _dt1 in predates)
            {
                var _dt = (DateTime)_dt1;
                var subResult = _result.Where(q => q.DepartureDayLocal == _dt).ToList();
                //var outResult = subResult.Where(q => q.Origin == apt).OrderBy(q => q.STDLocal).ToList();
                var inResult = subResult.Where(q => q.Destination == apt).OrderBy(q => q.STALocal).ToList();
                _cc = inResult.Count;

                for (var c = 0; c < _cc; c++)
                {
                    var row = new AptRpt()
                    {
                        Row = c + 1,
                        Airline = airline,
                        BaseApt = apt,
                        Date = ((DateTime)lastRec.DepartureDayLocal).ToString("yyyy/MM/dd"),
                        DatePersian = lastRec.PersianDate,
                        Day = lastRec.PersianDayName,
                        Temp1 = user,
                        Temp2 = phone,
                        Temp3 = lastRec.PersianDate,
                        Temp4 = lastRec.PersianDate
                    };

                    //if (c <= outResult.Count - 1)
                    //{
                    //    row.OutId = outResult[c].Id;
                    //    row.OutFlightNo = outResult[c].FlightNo;
                    //    row.OutRegister = outResult[c].Register;
                    //    row.OutType = getAcType(outResult[c].AircraftType);
                    //    row.OutRoute = outResult[c].Origin + "-" + outResult[c].Destination;
                    //    row.OutFrom = outResult[c].Origin;
                    //    row.OutTo = outResult[c].Destination;
                    //    row.OutRouteICAO = outResult[c].OriginICAO + "-" + outResult[c].DestinationICAO;
                    //    row.OutFromICAO = outResult[c].OriginICAO;
                    //    row.OutToICAO = outResult[c].DestinationICAO;
                    //    row.OutSTDLocal = outResult[c].STDLocal;
                    //    row.OutTimeLocal = ((DateTime)outResult[c].STDLocal).ToString("HH:mm");


                    //}

                    ///////////  LINKED  TO OUT FLIGHT
                    var _inresult = inResult[c];
                    try
                    {
                        // var _fn = Convert.ToInt32(row.OutFlightNo);
                        // var _inresult = inResult.Where(q => Convert.ToInt32(q.FlightNo) == (_fn + 1)).FirstOrDefault();
                        if (_inresult != null)
                        {
                            row.InId = _inresult.Id;
                            row.InFlightNo = _inresult.FlightNo;
                            row.InRegister = _inresult.Register;
                            row.InType = getAcType(_inresult.AircraftType);
                            row.InRoute = _inresult.Origin + "-" + _inresult.Destination;
                            row.InFrom = _inresult.Origin;
                            row.InTo = _inresult.Destination;
                            row.InRouteICAO = _inresult.OriginICAO + "-" + _inresult.DestinationICAO;
                            row.InFromICAO = _inresult.OriginICAO;
                            row.InToICAO = _inresult.DestinationICAO;
                            row.InSTDLocal = _inresult.STALocal;
                            row.InTimeLocal = ((DateTime)_inresult.STALocal).ToString("HHmm");
                            // if (_inresult.ArrivalDayLocal != _inresult.DepartureDayLocal)
                            //     row.InTimeLocal = row.InTimeLocal + "+1";
                        }
                    }
                    catch (Exception ex)
                    {

                    }



                    result.Add(row);

                }

            }

            foreach (var _dt1 in dates)
            {
                var _dt = (DateTime)_dt1;
                var subResult = _result.Where(q => q.DepartureDayLocal == _dt).ToList();
                var outResult = subResult.Where(q => q.Origin == apt).OrderBy(q => q.STDLocal).ToList();
                var inResult = subResult.Where(q => q.Destination == apt).OrderBy(q => q.STALocal).ToList();
                var cnt = outResult.Count;
                if (inResult.Count > cnt)
                    cnt = inResult.Count;
                for (var c = 0; c < cnt; c++)
                {
                    var row = new AptRpt()
                    {
                        Row = c + 1 + _cc,
                        Airline = airline,
                        BaseApt = apt,
                        Date = ((DateTime)lastRec.DepartureDayLocal).ToString("yyyy/MM/dd"),
                        DatePersian = lastRec.PersianDate,
                        Day = lastRec.PersianDayName,
                        Temp1 = user,
                        Temp2 = phone,
                        Temp3 = lastRec.PersianDate,
                        Temp4 = lastRec.PersianDate
                    };

                    if (c <= outResult.Count - 1)
                    {
                        row.OutId = outResult[c].Id;
                        row.OutFlightNo = outResult[c].FlightNo;
                        row.OutRegister = outResult[c].Register;
                        row.OutType = getAcType(outResult[c].AircraftType);
                        row.OutRoute = outResult[c].Origin + "-" + outResult[c].Destination;
                        row.OutFrom = outResult[c].Origin;
                        row.OutTo = outResult[c].Destination;
                        row.OutRouteICAO = outResult[c].OriginICAO + "-" + outResult[c].DestinationICAO;
                        row.OutFromICAO = outResult[c].OriginICAO;
                        row.OutToICAO = outResult[c].DestinationICAO;
                        row.OutSTDLocal = outResult[c].STDLocal;
                        row.OutTimeLocal = ((DateTime)outResult[c].STDLocal).ToString("HHmm");


                    }

                    ///////////  LINKED  TO OUT FLIGHT
                    try
                    {
                        var _fn = Convert.ToInt32(row.OutFlightNo);
                        var _inresult = inResult.Where(q => Convert.ToInt32(q.FlightNo) == (_fn + 1)).FirstOrDefault();
                        if (_inresult != null)
                        {
                            if (_inresult.ArrivalDayLocal == _inresult.DepartureDayLocal)
                            {
                                row.InId = _inresult.Id;
                                row.InFlightNo = _inresult.FlightNo;
                                row.InRegister = _inresult.Register;
                                row.InType = getAcType(_inresult.AircraftType);
                                row.InRoute = _inresult.Origin + "-" + _inresult.Destination;
                                row.InFrom = _inresult.Origin;
                                row.InTo = _inresult.Destination;
                                row.InRouteICAO = _inresult.OriginICAO + "-" + _inresult.DestinationICAO;
                                row.InFromICAO = _inresult.OriginICAO;
                                row.InToICAO = _inresult.DestinationICAO;
                                row.InSTDLocal = _inresult.STALocal;
                                row.InTimeLocal = ((DateTime)_inresult.STALocal).ToString("HHmm");
                            }

                            //if (_inresult.ArrivalDayLocal != _inresult.DepartureDayLocal)
                            //   row.InTimeLocal = row.InTimeLocal + "+1";
                        }
                        else
                        {
                            var flt = inResult.Where(q => q.Register == row.OutRegister && q.STD > outResult[c].STD).OrderBy(q => q.STD).FirstOrDefault();
                            if (flt != null)
                            {
                                row.InId = flt.Id;
                                row.InFlightNo = flt.FlightNo;
                                row.InRegister = flt.Register;
                                row.InType = getAcType(flt.AircraftType);
                                row.InRoute = flt.Origin + "-" + flt.Destination;
                                row.InFrom = flt.Origin;
                                row.InTo = flt.Destination;
                                row.InRouteICAO = flt.OriginICAO + "-" + flt.DestinationICAO;
                                row.InFromICAO = flt.OriginICAO;
                                row.InToICAO = flt.DestinationICAO;
                                row.InSTDLocal = flt.STALocal;
                                row.InTimeLocal = ((DateTime)flt.STALocal).ToString("HHmm");
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }



                    result.Add(row);

                }

            }




            if (grouped == 0)
                return Ok(result);
            else
            {
                var gresult = (from x in result
                               group x by new { x.Date, x.DatePersian, x.Day, x.Airline, x.BaseApt } into grp
                               select new
                               {
                                   //grp.Key.Airline,
                                   //grp.Key.BaseApt,
                                   Airline = _result.First().PersianDate,
                                   BaseApt = _result.Last().PersianDate,
                                   grp.Key.Date,
                                   grp.Key.DatePersian,
                                   grp.Key.Day,
                                   Items = grp.ToList()
                               }).ToList();

                return Ok(gresult);
            }


        }



        [Route("api/book/file/rename")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostBookFileRename(dynamic dto)
        {
            int id = Convert.ToInt32(dto.id);
            string name = Convert.ToString(dto.name);
            var context = new AirpocketAPI.Models.FLYEntities();
            var bf = context.BookFiles.Where(q => q.Id == id).FirstOrDefault();
            var doc = context.Documents.Where(q => q.Id == bf.DocumentId).FirstOrDefault();
            doc.SysUrl = name;
            context.SaveChanges();

            return Ok(name);



        }

        [Route("api/asmx")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetASMX()
        {
            var prms = new Dictionary<string, string>();
            prms.Add("job", "P1");
            var result = CallWebMethod("http://localhost:58908/soap.asmx/GetCrews2", prms);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);


            var jresult = JsonConvert.SerializeXmlNode(doc);
            var obj = JsonConvert.DeserializeObject(jresult);
            WebService ws = new WebService("http://localhost:58908/soap.asmx", "GetCrews2");
            ws.Params.Add("job", "P1");

            ws.Invoke();


            //return Ok(ws.ResultJSON);

            return Ok(doc);
        }

        [Route("api/_idea")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdea()
        {
            string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/sessions/obj";
            var input = new
            {

            };
            string inputJson = JsonConvert.SerializeObject(input);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            string json = client.DownloadString(apiUrl);
            json = json.Replace("?xml", "d_xml").Replace("soap:Envelope", "d_envelope").Replace("soap:Body", "d_body")
                .Replace("@xmlns:soap", "d_xmlns_soap").Replace("@xmlns:xsi", "d_xmlns_xsi").Replace("@xmlns:xsd", "d_xmlns_xsd")
                .Replace("@xmlns", "d_xmlns")
                .Replace("GetTotalDataJsonResponse", "response")
                .Replace("GetTotalDataJsonResult", "result");
            var obj = JsonConvert.DeserializeObject<IdeaResultSession>(json);
            //List<Customer> customers = (new JavaScriptSerializer()).Deserialize<List<Customer>>(json);
            //if (customers.Count > 0)
            //{
            //    foreach (Customer customer in customers)
            //    {
            //        Console.WriteLine(customer.ContactName);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No records found.");
            //}
            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaSessionX>>(response);
            return Ok(responseJson);
        }

        [Route("api/_idea/{prm}/{type}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaCourseUnique(string prm, string type)
        {
            var prms = new Dictionary<string, string>();
            if (prm == "unique")
                prms.Add("title", "PersonelUniquePassCourse");
            if (prm == "all")
                prms.Add("title", "PersonelAllPassCourse");
            if (prm == "sessions")
                prms.Add("title", "PersonelClassSessions");
            prms.Add("filters", "");

            //var result = CallWebMethod("https://192.168.101.133/IdeaWeb/Apps/Services/TrainingWS.asmx/GetTotalDataJson", prms);
            var result = CallWebMethod("http://trainingweb.caspian.local/IdeaWeb/Apps/Services/TrainingWS.asmx/GetTotalDataJson", prms);
            //http://trainingweb.caspian.local/IdeaWeb/Apps/Services/TrainingWS.asmx
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);


            var jresult = JsonConvert.SerializeXmlNode(doc);
            var obj = JsonConvert.DeserializeObject(jresult);

            if (type == "str")
                return Ok(jresult);
            else if (type == "xml")
                return Ok(doc);
            else return Ok(obj);
        }


        [Route("api/idea/alt/{prm}/{type}/{year}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaCourseUniqueAlt(string prm, string type, string year)
        {



            // WebService ws = new WebService("https://192.168.101.133/IdeaWeb/Apps/Services/TrainingWS.asmx", "GetTotalDataJson");
            //http://trainingweb.caspian.local/IdeaWeb/Apps/Services/TrainingWS.asmx
            WebService ws = new WebService("http://trainingweb.caspian.local/IdeaWeb/Apps/Services/TrainingWS.asmx", "GetTotalDataJson");
            if (prm == "unique")
                ws.Params.Add("title", "PersonelUniquePassCourse");
            if (prm == "all")
                ws.Params.Add("title", "PersonelAllPassCourse");
            if (prm == "sessions")
                ws.Params.Add("title", "PersonelClassSessions");
            if (year == "-1")
                ws.Params.Add("filters", "");
            else
                ws.Params.Add("filters", year);
            ws.Invoke();


            //return Ok(ws.ResultJSON);

            //return Ok(doc);
            if (type == "str")
                return Ok(ws.ResultString);
            else if (type == "xml")
                return Ok(ws.ResultXML);
            else return Ok(ws.ResultJsonObject);
        }

        [Route("api/idea/alt2/{prm}/{type}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaCourseUniqueAlt2(string prm, string type)
        {

            var level = "a";
            try
            {
                // WebService ws = new WebService("https://192.168.101.133/IdeaWeb/Apps/Services/TrainingWS.asmx", "GetTotalDataJson");
                WebService ws = new WebService("http://trainingweb.caspian.local/IdeaWeb/Apps/Services/TrainingWS.asmx", "GetTotalDataJson");
                //http://trainingweb.caspian.local/IdeaWeb/Apps/Services/TrainingWS.asmx
                if (prm == "unique")
                    ws.Params.Add("title", "PersonelUniquePassCourse");
                if (prm == "all")
                    ws.Params.Add("title", "PersonelAllPassCourse");
                if (prm == "sessions")
                    ws.Params.Add("title", "PersonelClassSessions");
                ws.Params.Add("filters", "2021");
                ws.Invoke();
                level = "b";

                //return Ok(ws.R  esultJSON);

                //return Ok(doc);
                if (type == "str")
                    return Ok(ws.ResultString);
                else if (type == "xml")
                    return Ok(ws.ResultXML);
                else return Ok(ws.ResultJsonObject);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "  INNER:   " + ex.InnerException.Message;
                return Ok(msg);
            }

        }

        //kish
        [Route("api/mail/mvt/{flightId}/{sender}/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaUniqueSync(int flightId, string sender, string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            var helper = new MailHelper();
            var result = helper.CreateMVTMessage(flightId, sender);

            return Ok(result);
        }


        [Route("api/mail/mvt/apt/{flightId}/{sender}/{user}/{password}/{apt}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult SendMVTAPT(int flightId, string sender, string user, string password, string apt)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            var helper = new MailHelper();
            var result = helper.CreateMVTMessageAPT(apt, flightId, sender);


            return Ok(result);
        }

        [Route("api/mail/{port}/{ssl}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult TESTEMAIL(int port, int ssl)
        {


            var helper = new MailHelper();
            var result = helper.SendTest("TEST", "TEST", port, ssl);

            return Ok(result);
        }


        [Route("api/mvt/send/{user}/{password}/{force}/{day}/{fn}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetSendMVTByFN(string user, string password, string day, string fn, int force)
        {
            if (user != "fp")
                return BadRequest("Not Authenticated");
            if (password != "Z12345aA")
                return BadRequest("Not Authenticated");

            var helper = new MailHelper();
            var result = helper.CreateMVTMessageByFlightNo(day, fn, force);

            return Ok(result);
        }


        [Route("api/dr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetDRByFlight(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.EFBDSPReleases.FirstOrDefault(q => q.FlightId == fltid);
            return Ok(result);
        }

        [Route("api/dsp/dr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetDRByFlightDSP(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.ViewEFBDSPReleases.FirstOrDefault(q => q.FlightId == fltid);
            //.EFBDSPReleases.FirstOrDefault(q => q.FlightId == fltid);
            return Ok(result);
        }
        [Route("api/dr/save")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostDR(DSPReleaseViewModel DSPRelease)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var release = context.EFBDSPReleases.FirstOrDefault(q => q.FlightId == DSPRelease.FlightId);
            if (release == null)
            {
                release = new EFBDSPRelease();
                context.EFBDSPReleases.Add(release);

            }

            release.User = DSPRelease.User;
            release.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm");


            release.FlightId = DSPRelease.FlightId;
            release.ActualWXDSP = DSPRelease.ActualWXDSP;
            release.ActualWXCPT = DSPRelease.ActualWXCPT;
            release.ActualWXDSPRemark = DSPRelease.ActualWXDSPRemark;
            release.ActualWXCPTRemark = DSPRelease.ActualWXCPTRemark;
            release.WXForcastDSP = DSPRelease.WXForcastDSP;
            release.WXForcastCPT = DSPRelease.WXForcastCPT;
            release.WXForcastDSPRemark = DSPRelease.WXForcastDSPRemark;
            release.WXForcastCPTRemark = DSPRelease.WXForcastCPTRemark;
            release.SigxWXDSP = DSPRelease.SigxWXDSP;
            release.SigxWXCPT = DSPRelease.SigxWXCPT;
            release.SigxWXDSPRemark = DSPRelease.SigxWXDSPRemark;
            release.SigxWXCPTRemark = DSPRelease.SigxWXCPTRemark;
            release.WindChartDSP = DSPRelease.WindChartDSP;
            release.WindChartCPT = DSPRelease.WindChartCPT;
            release.WindChartDSPRemark = DSPRelease.WindChartDSPRemark;
            release.WindChartCPTRemark = DSPRelease.WindChartCPTRemark;
            release.NotamDSP = DSPRelease.NotamDSP;
            release.NotamCPT = DSPRelease.NotamCPT;
            release.NotamDSPRemark = DSPRelease.NotamDSPRemark;
            release.NotamCPTRemark = DSPRelease.NotamCPTRemark;
            release.ComputedFligthPlanDSP = DSPRelease.ComputedFligthPlanDSP;
            release.ComputedFligthPlanCPT = DSPRelease.ComputedFligthPlanCPT;
            release.ComputedFligthPlanDSPRemark = DSPRelease.ComputedFligthPlanDSPRemark;
            release.ComputedFligthPlanCPTRemark = DSPRelease.ComputedFligthPlanCPTRemark;
            release.ATCFlightPlanDSP = DSPRelease.ATCFlightPlanDSP;
            release.ATCFlightPlanCPT = DSPRelease.ATCFlightPlanCPT;
            release.ATCFlightPlanDSPRemark = DSPRelease.ATCFlightPlanDSPRemark;
            release.ATCFlightPlanCPTRemark = DSPRelease.ATCFlightPlanCPTRemark;
            release.PermissionsDSP = DSPRelease.PermissionsDSP;
            release.PermissionsCPT = DSPRelease.PermissionsCPT;
            release.PermissionsDSPRemark = DSPRelease.PermissionsDSPRemark;
            release.PermissionsCPTRemark = DSPRelease.PermissionsCPTRemark;
            release.JeppesenAirwayManualDSP = DSPRelease.JeppesenAirwayManualDSP;
            release.JeppesenAirwayManualCPT = DSPRelease.JeppesenAirwayManualCPT;
            release.JeppesenAirwayManualDSPRemark = DSPRelease.JeppesenAirwayManualDSPRemark;
            release.JeppesenAirwayManualCPTRemark = DSPRelease.JeppesenAirwayManualCPTRemark;
            release.MinFuelRequiredDSP = DSPRelease.MinFuelRequiredDSP;
            release.MinFuelRequiredCPT = DSPRelease.MinFuelRequiredCPT;
            release.MinFuelRequiredCFP = DSPRelease.MinFuelRequiredCFP;
            release.MinFuelRequiredPilotReq = DSPRelease.MinFuelRequiredPilotReq;
            release.GeneralDeclarationDSP = DSPRelease.GeneralDeclarationDSP;
            release.GeneralDeclarationCPT = DSPRelease.GeneralDeclarationCPT;
            release.GeneralDeclarationDSPRemark = DSPRelease.GeneralDeclarationDSPRemark;
            release.GeneralDeclarationCPTRemark = DSPRelease.GeneralDeclarationCPTRemark;
            release.FlightReportDSP = DSPRelease.FlightReportDSP;
            release.FlightReportCPT = DSPRelease.FlightReportCPT;
            release.FlightReportDSPRemark = DSPRelease.FlightReportDSPRemark;
            release.FlightReportCPTRemark = DSPRelease.FlightReportCPTRemark;
            release.TOLndCardsDSP = DSPRelease.TOLndCardsDSP;
            release.TOLndCardsCPT = DSPRelease.TOLndCardsCPT;
            release.TOLndCardsDSPRemark = DSPRelease.TOLndCardsDSPRemark;
            release.TOLndCardsCPTRemark = DSPRelease.TOLndCardsCPTRemark;
            release.LoadSheetDSP = DSPRelease.LoadSheetDSP;
            release.LoadSheetCPT = DSPRelease.LoadSheetCPT;
            release.LoadSheetDSPRemark = DSPRelease.LoadSheetDSPRemark;
            release.LoadSheetCPTRemark = DSPRelease.LoadSheetCPTRemark;
            release.FlightSafetyReportDSP = DSPRelease.FlightSafetyReportDSP;
            release.FlightSafetyReportCPT = DSPRelease.FlightSafetyReportCPT;
            release.FlightSafetyReportDSPRemark = DSPRelease.FlightSafetyReportDSPRemark;
            release.FlightSafetyReportCPTRemark = DSPRelease.FlightSafetyReportCPTRemark;
            release.AVSECIncidentReportDSP = DSPRelease.AVSECIncidentReportDSP;
            release.AVSECIncidentReportCPT = DSPRelease.AVSECIncidentReportCPT;
            release.AVSECIncidentReportDSPRemark = DSPRelease.AVSECIncidentReportDSPRemark;
            release.AVSECIncidentReportCPTRemark = DSPRelease.AVSECIncidentReportCPTRemark;
            release.OperationEngineeringDSP = DSPRelease.OperationEngineeringDSP;
            release.OperationEngineeringCPT = DSPRelease.OperationEngineeringCPT;
            release.OperationEngineeringDSPRemark = DSPRelease.OperationEngineeringDSPRemark;
            release.OperationEngineeringCPTRemark = DSPRelease.OperationEngineeringCPTRemark;
            release.VoyageReportDSP = DSPRelease.VoyageReportDSP;
            release.VoyageReportCPT = DSPRelease.VoyageReportCPT;
            release.VoyageReportDSPRemark = DSPRelease.VoyageReportDSPRemark;
            release.VoyageReportCPTRemark = DSPRelease.VoyageReportCPTRemark;
            release.PIFDSP = DSPRelease.PIFDSP;
            release.PIFCPT = DSPRelease.PIFCPT;
            release.PIFDSPRemark = DSPRelease.PIFDSPRemark;
            release.PIFCPTRemark = DSPRelease.PIFCPTRemark;
            release.GoodDeclarationDSP = DSPRelease.GoodDeclarationDSP;
            release.GoodDeclarationCPT = DSPRelease.GoodDeclarationCPT;
            release.GoodDeclarationDSPRemark = DSPRelease.GoodDeclarationDSPRemark;
            release.GoodDeclarationCPTRemark = DSPRelease.GoodDeclarationCPTRemark;
            release.IPADDSP = DSPRelease.IPADDSP;
            release.IPADCPT = DSPRelease.IPADCPT;
            release.IPADDSPRemark = DSPRelease.IPADDSPRemark;
            release.IPADCPTRemark = DSPRelease.IPADCPTRemark;
            release.DateConfirmed = DSPRelease.DateConfirmed;
            release.DispatcherId = DSPRelease.DispatcherId;
            context.SaveChanges();
            return Ok(release);


        }

        [Route("api/dsp/dr/save")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostDRDSP(DSPReleaseViewModel2 DSPRelease)
        {
            try
            {
                var context = new AirpocketAPI.Models.FLYEntities();
                var release = context.EFBDSPReleases.FirstOrDefault(q => q.FlightId == DSPRelease.FlightId);
                if (release == null)
                {
                    release = new EFBDSPRelease();
                    context.EFBDSPReleases.Add(release);

                }

                // release.User = DSPRelease.User;



                release.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm");


                release.FlightId = DSPRelease.FlightId;
                release.ActualWXDSP = DSPRelease.ActualWXDSP;
                // release.ActualWXCPT = DSPRelease.ActualWXCPT;
                release.ActualWXDSPRemark = DSPRelease.ActualWXDSPRemark;
                //release.ActualWXCPTRemark = DSPRelease.ActualWXCPTRemark;
                release.WXForcastDSP = DSPRelease.WXForcastDSP;
                //release.WXForcastCPT = DSPRelease.WXForcastCPT;
                release.WXForcastDSPRemark = DSPRelease.WXForcastDSPRemark;
                //release.WXForcastCPTRemark = DSPRelease.WXForcastCPTRemark;
                release.SigxWXDSP = DSPRelease.SigxWXDSP;
                //release.SigxWXCPT = DSPRelease.SigxWXCPT;
                release.SigxWXDSPRemark = DSPRelease.SigxWXDSPRemark;
                //release.SigxWXCPTRemark = DSPRelease.SigxWXCPTRemark;
                release.WindChartDSP = DSPRelease.WindChartDSP;
                //release.WindChartCPT = DSPRelease.WindChartCPT;
                release.WindChartDSPRemark = DSPRelease.WindChartDSPRemark;
                //release.WindChartCPTRemark = DSPRelease.WindChartCPTRemark;
                release.NotamDSP = DSPRelease.NotamDSP;
                //release.NotamCPT = DSPRelease.NotamCPT;
                release.NotamDSPRemark = DSPRelease.NotamDSPRemark;
                //release.NotamCPTRemark = DSPRelease.NotamCPTRemark;
                release.ComputedFligthPlanDSP = DSPRelease.ComputedFligthPlanDSP;
                //release.ComputedFligthPlanCPT = DSPRelease.ComputedFligthPlanCPT;
                release.ComputedFligthPlanDSPRemark = DSPRelease.ComputedFligthPlanDSPRemark;
                //release.ComputedFligthPlanCPTRemark = DSPRelease.ComputedFligthPlanCPTRemark;
                release.ATCFlightPlanDSP = DSPRelease.ATCFlightPlanDSP;
                //release.ATCFlightPlanCPT = DSPRelease.ATCFlightPlanCPT;
                release.ATCFlightPlanDSPRemark = DSPRelease.ATCFlightPlanDSPRemark;
                //release.ATCFlightPlanCPTRemark = DSPRelease.ATCFlightPlanCPTRemark;
                release.PermissionsDSP = DSPRelease.PermissionsDSP;
                //release.PermissionsCPT = DSPRelease.PermissionsCPT;
                release.PermissionsDSPRemark = DSPRelease.PermissionsDSPRemark;
                //release.PermissionsCPTRemark = DSPRelease.PermissionsCPTRemark;
                release.JeppesenAirwayManualDSP = DSPRelease.JeppesenAirwayManualDSP;
                //release.JeppesenAirwayManualCPT = DSPRelease.JeppesenAirwayManualCPT;
                release.JeppesenAirwayManualDSPRemark = DSPRelease.JeppesenAirwayManualDSPRemark;
                //release.JeppesenAirwayManualCPTRemark = DSPRelease.JeppesenAirwayManualCPTRemark;
                release.MinFuelRequiredDSP = DSPRelease.MinFuelRequiredDSP;
                //release.MinFuelRequiredCPT = DSPRelease.MinFuelRequiredCPT;
                //release.MinFuelRequiredCFP = DSPRelease.MinFuelRequiredCFP;
                //release.MinFuelRequiredPilotReq = DSPRelease.MinFuelRequiredPilotReq;
                release.GeneralDeclarationDSP = DSPRelease.GeneralDeclarationDSP;
                //release.GeneralDeclarationCPT = DSPRelease.GeneralDeclarationCPT;
                release.GeneralDeclarationDSPRemark = DSPRelease.GeneralDeclarationDSPRemark;
                //release.GeneralDeclarationCPTRemark = DSPRelease.GeneralDeclarationCPTRemark;
                release.FlightReportDSP = DSPRelease.FlightReportDSP;
                //release.FlightReportCPT = DSPRelease.FlightReportCPT;
                release.FlightReportDSPRemark = DSPRelease.FlightReportDSPRemark;
                //release.FlightReportCPTRemark = DSPRelease.FlightReportCPTRemark;
                release.TOLndCardsDSP = DSPRelease.TOLndCardsDSP;
                //release.TOLndCardsCPT = DSPRelease.TOLndCardsCPT;
                release.TOLndCardsDSPRemark = DSPRelease.TOLndCardsDSPRemark;
                //release.TOLndCardsCPTRemark = DSPRelease.TOLndCardsCPTRemark;
                release.LoadSheetDSP = DSPRelease.LoadSheetDSP;
                //release.LoadSheetCPT = DSPRelease.LoadSheetCPT;
                release.LoadSheetDSPRemark = DSPRelease.LoadSheetDSPRemark;
                //release.LoadSheetCPTRemark = DSPRelease.LoadSheetCPTRemark;
                release.FlightSafetyReportDSP = DSPRelease.FlightSafetyReportDSP;
                //release.FlightSafetyReportCPT = DSPRelease.FlightSafetyReportCPT;
                release.FlightSafetyReportDSPRemark = DSPRelease.FlightSafetyReportDSPRemark;
                //release.FlightSafetyReportCPTRemark = DSPRelease.FlightSafetyReportCPTRemark;
                release.AVSECIncidentReportDSP = DSPRelease.AVSECIncidentReportDSP;
                //release.AVSECIncidentReportCPT = DSPRelease.AVSECIncidentReportCPT;
                release.AVSECIncidentReportDSPRemark = DSPRelease.AVSECIncidentReportDSPRemark;
                //release.AVSECIncidentReportCPTRemark = DSPRelease.AVSECIncidentReportCPTRemark;
                release.OperationEngineeringDSP = DSPRelease.OperationEngineeringDSP;
                //release.OperationEngineeringCPT = DSPRelease.OperationEngineeringCPT;
                release.OperationEngineeringDSPRemark = DSPRelease.OperationEngineeringDSPRemark;
                //release.OperationEngineeringCPTRemark = DSPRelease.OperationEngineeringCPTRemark;
                release.VoyageReportDSP = DSPRelease.VoyageReportDSP;
                //release.VoyageReportCPT = DSPRelease.VoyageReportCPT;

                //release.VoyageReportCPTRemark = DSPRelease.VoyageReportCPTRemark;
                release.PIFDSP = DSPRelease.PIFDSP;
                //release.PIFCPT = DSPRelease.PIFCPT;
                release.PIFDSPRemark = DSPRelease.PIFDSPRemark;
                //release.PIFCPTRemark = DSPRelease.PIFCPTRemark;
                release.GoodDeclarationDSP = DSPRelease.GoodDeclarationDSP;
                // release.GoodDeclarationCPT = DSPRelease.GoodDeclarationCPT;
                release.GoodDeclarationDSPRemark = DSPRelease.GoodDeclarationDSPRemark;
                //  release.GoodDeclarationCPTRemark = DSPRelease.GoodDeclarationCPTRemark;
                release.IPADDSP = DSPRelease.IPADDSP;
                //release.IPADCPT = DSPRelease.IPADCPT;
                release.IPADDSPRemark = DSPRelease.IPADDSPRemark;
                //release.IPADCPTRemark = DSPRelease.IPADCPTRemark;
                release.DateConfirmed = DateTime.Now; //DSPRelease.DateConfirmed;
                release.DispatcherId = DSPRelease.DispatcherId;


                release.VoyageReportDSPRemark = DSPRelease.User;


                context.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER: " + ex.InnerException.Message;
                return Ok(msg);
            }

        }


        [Route("api/asr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetASRByFlight(int fltid)
        {

            // GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Re‌​ferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.ViewEFBASRs.FirstOrDefault(q => q.FlightId == fltid);
            return Ok(result);
        }
        [Route("api/asr/flight/view/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetASRByFlightView(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.ViewEFBASRs.FirstOrDefault(q => q.FlightId == fltid);
            return Ok(result);
        }

        [Route("api/vr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVRByFlight(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();

            var result = context.EFBVoyageReports.FirstOrDefault(q => q.FlightId == fltid);
            if (result == null)
                return Ok(result);
            result.EFBReasons = context.EFBReasons.Where(q => q.VoyageReportId == result.Id).ToList();
            result.EFBFlightIrregularities = context.EFBFlightIrregularities.Where(q => q.VoyageReportId == result.Id).ToList();
            return Ok(result);
        }



        [Route("api/log/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightLog(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();

            var result = context.AppLegs.FirstOrDefault(q => q.FlightId == fltid);


            if (result == null)
                return Ok(new
                {
                    IsSuccess = false,
                });
            var crews = context.ViewFlightCrewNewXes.Where(q => q.FlightId == fltid).ToList();
            return Ok(new
            {
                IsSuccess = true,
                Flight = result,
                Crews = crews,
            });


        }
        [Route("api/appleg/crew/{flightId}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetAppLegCrew(int flightId)
        {


            var context = new AirpocketAPI.Models.FLYEntities();
            var crews = context.ViewFlightCrewNewXes.Where(q => q.FlightId == flightId).OrderBy(q => q.IsPositioning).ThenBy(q => q.GroupOrder).ToList();



            return Ok(crews);


        }

        [Route("api/apt/from/{crewId}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetAptFrom(int crewId)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.AppCrewFlights
                        where x.CrewId == crewId
                        select x.FromAirportIATA;
            var result = query.OrderBy(q => q).ToList();
            return Ok(result);
        }

        [Route("api/apt/to/{crewId}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetAptTo(int crewId)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.AppCrewFlights
                        where x.CrewId == crewId
                        select x.ToAirportIATA;
            var result = query.OrderBy(q => q).ToList();
            return Ok(result);
        }

        [Route("api/cockpit/ip/{crewId}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCockpitIP(int crewId)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.AppCrewFlights
                        where x.CrewId == crewId
                        select x.IPScheduleName;
            var result = query.OrderBy(q => q).ToList();
            return Ok(result);
        }

        [Route("api/cockpit/cpt/{crewId}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCockpitCPT(int crewId)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.AppCrewFlights
                        where x.CrewId == crewId
                        select x.P1ScheduleName;
            var result = query.OrderBy(q => q).ToList();
            return Ok(result);
        }

        [Route("api/cockpit/fo/{crewId}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCockpitFO(int crewId)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.AppCrewFlights
                        where x.CrewId == crewId
                        select x.P2ScheduleName;
            var result = query.OrderBy(q => q).ToList();
            return Ok(result);
        }

        [Route("api/appleg/ofp/{flightId}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOPF(int flightId)
        {


            var context = new AirpocketAPI.Models.FLYEntities();
            var ofp = context.OFPImports.FirstOrDefault(q => q.FlightId == flightId);
            if (ofp == null)
                return Ok(new { Id = -1 });
            else
            {
                var props = context.OFPImportProps.Where(q => q.OFPId == ofp.Id).Select(q =>
                  new
                  {
                      q.Id,
                      q.OFPId,
                      q.PropName,
                      q.PropType,
                      q.PropValue,
                      q.User,
                      q.DateUpdate,

                  }
                    ).ToList();
                return Ok(new
                {
                    ofp.Id,
                    ofp.FlightId,
                    ofp.TextOutput,
                    ofp.User,
                    ofp.DateCreate,
                    ofp.PIC,
                    ofp.PICId,
                    ofp.JLSignedBy,
                    ofp.JLDatePICApproved,
                    props

                });
            }



        }



        [Route("api/ofp/details/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOFPDetails(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();

            var result = context.OFPImports.FirstOrDefault(q => q.FlightId == fltid);
            if (result == null)
                return Ok(new
                {
                    IsSuccess = false,
                });
            var props = context.OFPImportProps.Where(q => q.OFPId == result.Id).ToList();
            return Ok(new
            {
                IsSuccess = true,
                OFP = result,
                Props = props,
            });


        }



        [Route("api/login/history/{user}/{from}/{to}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetLoginHistory(string user, string from, string to)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.LoginInfoes

                        select x;
            if (user != "-1")
                query = query.Where(q => q.User == user);
            if (from != "-1")
            {
                var prts1 = from.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                var _from = new DateTime(prts1[0], prts1[1], prts1[2]);
                query = query.Where(q => q.DateCreate >= _from);
            }

            if (to != "-1")
            {
                var prts1 = to.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                var _to = new DateTime(prts1[0], prts1[1], prts1[2]);
                _to = _to.AddHours(23).AddMinutes(59).AddSeconds(50);
                query = query.Where(q => q.DateCreate <= _to);
            }



            var result = query.OrderBy(q => q.DateCreate).ToList();
            var abst = result.Select(q => new { q.User, q.IP, q.LocationCity, q.DateCreate }).ToList();

            return Ok(abst);
        }
        [Route("api/login/history/detailed/{user}/{from}/{to}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetLoginHistoryDetailed(string user, string from, string to)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.LoginInfoes

                        select x;
            if (user != "-1")
                query = query.Where(q => q.User == user);
            if (from != "-1")
            {
                var prts1 = from.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                var _from = new DateTime(prts1[0], prts1[1], prts1[2]);
                query = query.Where(q => q.DateCreate >= _from);
            }

            if (to != "-1")
            {
                var prts1 = to.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                var _to = new DateTime(prts1[0], prts1[1], prts1[2]);
                _to = _to.AddHours(23).AddMinutes(59).AddSeconds(50);
                query = query.Where(q => q.DateCreate <= _to);
            }



            var result = query.OrderBy(q => q.DateCreate).ToList();

            return Ok(result);
        }
        [Route("api/login/save")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostLoginInfo(dynamic dto)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            string user = Convert.ToString(dto.user);
            string ip = Convert.ToString(dto.ip);
            string city = Convert.ToString(dto.city);
            string info = Convert.ToString(dto.info);
            var result = new LoginInfo()
            {
                DateCreate = DateTime.Now,
                Info = info,
                IP = ip,
                LocationCity = city,
                User = user
            };
            context.LoginInfoes.Add(result);
            context.SaveChanges();
            return Ok(new
            {
                result.IP,
                result.LocationCity
            });


        }
        //5-18
        [Route("api/charterers")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCharterers()
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.Charterers
                        select new
                        {
                            x.Id,
                            x.Title1,
                            x.Title2,
                            x.NiraCode,
                            x.Remark,
                            x.Code,
                        };



            var result = query.OrderBy(q => q.Title1).ToList();

            return Ok(result);
        }

        [Route("api/groups")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetJobGroups()
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ViewJobGroups
                        select x;



            var result = query.OrderBy(q => q.FullCode2).ToList();

            return Ok(result);
        }

        [Route("api/charterer/save")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostCharterer(ChartererDto dto)
        {
            var context = new AirpocketAPI.Models.FLYEntities();

            var existCode = context.Charterers.FirstOrDefault(q => q.Id != dto.Id && q.Code == dto.Code);
            if (existCode != null)
            {
                return Ok(new Charterer() { Id = -100, Title1 = "The selected CODE is already taken" });
            }
            var existTitle = context.Charterers.FirstOrDefault(q => q.Id != dto.Id && q.Title1.Replace(" ", "") == dto.Title1.Replace(" ", ""));
            if (existTitle != null)
            {
                return Ok(new Charterer() { Id = -100, Title1 = "The selected TITLE is already taken" });
            }
            var existNira = context.Charterers.FirstOrDefault(q => q.Id != dto.Id && q.NiraCode.Replace(" ", "") == dto.NiraCode.Replace(" ", ""));
            if (existNira != null)
            {
                return Ok(new Charterer() { Id = -100, Title1 = "The selected NIRA CODE is already taken" });
            }


            Charterer entity = null;
            if (dto.Id == -1)
            {
                entity = new Charterer() { Id = -1 };
                context.Charterers.Add(entity);
            }
            else
            {
                entity = context.Charterers.FirstOrDefault(q => q.Id == dto.Id);
            }
            entity.Code = dto.Code;
            entity.Title1 = dto.Title1;
            // entity.Title2 = dto.Title2;
            entity.NiraCode = dto.NiraCode;
            entity.Remark = dto.Remark;

            context.SaveChanges();
            var chr = context.Charterers.FirstOrDefault(q => q.Id == entity.Id);

            return Ok(chr);


        }

        [Route("api/charterer/delete")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostChartererDelete(dynamic dto)
        {
            int Id = Convert.ToInt32(dto.Id);
            var context = new AirpocketAPI.Models.FLYEntities();
            var entity = context.Charterers.FirstOrDefault(q => q.Id == Id);
            context.Charterers.Remove(entity);

            context.SaveChanges();

            return Ok(true);


        }

        [Route("api/flight/charterers/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightCharterers(int id)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ViewFlightCharterers where x.FlightId == id select x;



            var result = query.OrderBy(q => q.Title1).ToList();

            return Ok(result);
        }

        //[Route("api/flight/charterers/report")]
        //[AcceptVerbs("GET")]
        //public IHttpActionResult GetFlightCharterersReport(string df, string dt, int chr)
        //{
        //    var context = new AirpocketAPI.Models.FLYEntities();

        //    var fp = df.Split('-').Select(q => Convert.ToInt32(q)).ToList();
        //    var ft = dt.Split('-').Select(q => Convert.ToInt32(q)).ToList();
        //    var _df = new DateTime(fp[0], fp[1], fp[2]);
        //    var _dt = new DateTime(ft[0], ft[1], ft[2]);
        //    _dt = _dt.AddDays(1);
        //    var query = from x in context.ViewFlightCharterers
        //                where x.STDLocal >= _df && x.STDLocal <= _dt
        //                select x;
        //    if (chr != -1)
        //        query = query.Where(q => q.ChartererId == chr);


        //    var result = query.OrderBy(q => q.STDLocal).ThenBy(q => q.Title1).ToList();

        //    return Ok(result);
        //}

        [Route("api/flight/charterer/save")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostFlightCharterer(FlightChartererDto dto)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            FlightCharterer entity = null;
            if (dto.Id == -1)
            {
                entity = new FlightCharterer() { Id = -1 };
                context.FlightCharterers.Add(entity);
            }
            else
            {
                entity = context.FlightCharterers.FirstOrDefault(q => q.Id == dto.Id);
            }
            entity.FlightId = dto.FlightId;
            entity.ChartererId = dto.ChartererId;
            entity.Capacity = dto.Capacity;
            entity.Adult = dto.Adult;
            entity.Child = dto.Child;
            entity.Infanct = dto.Infanct;
            entity.Remark = dto.Remark;

            context.SaveChanges();
            var chr = context.ViewFlightCharterers.FirstOrDefault(q => q.Id == entity.Id);

            return Ok(chr);


        }

        [Route("api/flight/charterer/delete")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostFlightChartererDelete(dynamic dto)
        {
            int Id = Convert.ToInt32(dto.Id);
            var context = new AirpocketAPI.Models.FLYEntities();
            var entity = context.FlightCharterers.FirstOrDefault(q => q.Id == Id);
            context.FlightCharterers.Remove(entity);

            context.SaveChanges();

            return Ok(true);


        }


        [Route("api/ofp/check")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCheckOFP()
        {
            var user = "demo";
            var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
            var context = new AirpocketAPI.Models.FLYEntities();
            var fnprts = (fn.Split('_')[1]).Split('.');
            var _date = new DateTime(Convert.ToInt32(fnprts[0]), Convert.ToInt32(fnprts[1]), Convert.ToInt32(fnprts[2]));
            var _fltno = (fnprts[5]).PadLeft(4, '0');
            var _origin = fnprts[3];
            var _destination = fnprts[4];
            var cplan = context.OFPImports.FirstOrDefault(q => q.DateFlight == _date && q.FlightNo == _fltno && q.Origin == _origin && q.Destination == _destination);
            if (cplan == null)
                return Ok(new
                {
                    ofpId = -1,
                });
            else
            {
                return Ok(new
                {
                    ofpId = cplan.Id,
                    user = cplan.User,
                    date = cplan.DateCreate,
                    fileName = cplan.FileName,
                });
            }

        }

        [Route("api/ofp/check/flight/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCheckOFP(int id)
        {

            var context = new AirpocketAPI.Models.FLYEntities();

            var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == id);
            if (cplan == null)
                return Ok(new
                {
                    ofpId = -1,
                });
            else
            {
                return Ok(new
                {
                    ofpId = cplan.Id,
                    user = cplan.User,
                    date = cplan.DateCreate,
                    fileName = cplan.FileName,
                });
            }

        }

        [Route("api/ofp/flight/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightOFP(int id)
        {

            var context = new AirpocketAPI.Models.FLYEntities();
            // context.Configuration.LazyLoadingEnabled = false;

            var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == id);
            //var result=JsonConvert.SerializeObject(cplan, Newtonsoft.Json.Formatting.None,
            //            new JsonSerializerSettings()
            //            {
            //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //            });
            if (cplan == null)
                return Ok(new { Id = -1 });
            var result = new
            {
                cplan.Id,
                cplan.FlightId,
                cplan.DateCreate,
                cplan.DateFlight,
                cplan.Destination,
                cplan.FileName,

                cplan.FlightNo,
                cplan.Origin,
                cplan.Text,
                cplan.TextOutput,
                cplan.User,


            };
            return Ok(result);

        }


        [Route("api/ofp/parse")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetImportOFP()
        {
            var user = "demo";
            var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
            var context = new AirpocketAPI.Models.FLYEntities();
            var fnprts = (fn.Split('_')[1]).Split('.');
            var _date = new DateTime(Convert.ToInt32(fnprts[0]), Convert.ToInt32(fnprts[1]), Convert.ToInt32(fnprts[2]));
            var _fltno = (fnprts[5]).PadLeft(4, '0');
            var _origin = fnprts[3];
            var _destination = fnprts[4];


            string folder = HttpContext.Current.Server.MapPath("~/upload");
            string path = Path.Combine(folder, fn);

            var ftext = File.ReadAllText(path);
            var lines = File.ReadAllLines(path).ToList();
            var linesNoSpace = lines.Select(q => q.Replace(" ", "")).ToList();
            var linesTrimStart = lines.Select(q => q.TrimStart()).ToList();

            var flight = context.ViewLegTimes.FirstOrDefault(q => q.STDDay == _date && q.FlightNumber == _fltno);

            var cplan = context.OFPImports.FirstOrDefault(q => q.DateFlight == _date && q.FlightNo == _fltno && q.Origin == _origin && q.Destination == _destination);
            if (cplan != null)
                context.OFPImports.Remove(cplan);

            var plan = new OFPImport()
            {
                DateCreate = DateTime.Now,
                DateFlight = _date,
                FileName = fn,
                FlightNo = _fltno,
                Origin = _origin,
                Destination = _destination,
                User = user,
                Text = ftext,


            };
            if (flight != null)
                plan.FlightId = flight.ID;
            context.OFPImports.Add(plan);


            var fuelParts = OFPHelper.GetFuelParts(lines, linesNoSpace);



            var linesProccessed = new List<string>();
            foreach (var ln in lines)
            {
                var nospace = ln.Replace(" ", "");
                //Cont 05%  000223 00.05            ............  PAX  : .../.../...
                if (nospace.ToLower().StartsWith("cont05"))
                {
                    var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                    linesProccessed.Add(prts[0]);

                }
                // HLD       001299 00.30            A.TAKEOFF W.  CREW : .../.../... 
                else if (nospace.ToLower().StartsWith("hld"))
                {
                    var prts = ln.Split(new string[] { "A." }, StringSplitOptions.None);
                    linesProccessed.Add(prts[0]);
                    //var prts = ln.Split(' ').ToList();
                    //var temp = new List<string>();
                    //foreach (var x in prts)
                    //{
                    //    if (x== "A.TAKEOFF")
                    //    {
                    //        temp.Add(x);
                    //        temp.Add(" ");
                    //    }
                    //    else
                    //    if (x == "CREW:")
                    //    {
                    //        temp.Add("CREW :");
                    //        temp.Add(" ");
                    //    }
                    //    else
                    //    if (x.StartsWith("."))
                    //    {
                    //        var ps = x.Split('/');
                    //        var plst = new List<string>();
                    //        for (int i = 0; i < ps.Count(); i++)
                    //            plst.Add("@crew_" + (i + 1).ToString());
                    //        temp.Add(string.Join("/",plst));
                    //    }
                    //    else
                    //    {
                    //        if (x == "" || x == " ")
                    //            temp.Add(" ");
                    //        else
                    //            temp.Add(x);
                    //    }
                    //}

                    //linesProccessed.Add(string.Join("",temp));
                }
                // TXY       000200                  A.TAKEOFF F.  T.O.B: ...........  
                else if (nospace.ToLower().StartsWith("txy"))
                {
                    var prts = ln.Split(new string[] { "A." }, StringSplitOptions.None);
                    linesProccessed.Add(prts[0]);
                }
                //REQUIRED  006414 02.12            ............
                else if (nospace.ToLower().StartsWith("required"))
                {
                    var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                    linesProccessed.Add(prts[0]);
                }
                //OFP WORKED By CAPT:  .. .. .. .. .. ..  FO: .. .. .. .. .. .. .. ..
                else if (nospace.ToLower().StartsWith("ofpworked"))
                {
                    // var prts = ln.Split(new string[] { "FO" }, StringSplitOptions.None);
                    linesProccessed.Add(" OFP WORKED By CAPT:" +/*@ofbcpt*/"<span class='prop' id='prop_ofbcpt'>  .. .. .. .. .. ..  </span>" + "FO: " +/*@ofbfo"*/"<span class='prop' id='prop_ofbfo'>.. .. .. .. .. .. .. ..</span>");
                }
                // CLEARANCE: .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. ..
                else if (nospace.ToLower().StartsWith("clearance"))
                {
                    // var prts = ln.Split(new string[] { "FO" }, StringSplitOptions.None);
                    linesProccessed.Add(" CLEARANCE: " + "<span class='prop' id='prop_clearance'>.. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. ..</span>");
                }
                else
                    linesProccessed.Add(ln);
            }

            var nospaceproccessed = linesProccessed.Select(q => q.Trim().Replace(" ", "").ToLower()).ToList();
            var _indx1 = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().StartsWith("total")).First());
            _indx1++;
            linesProccessed.Insert(_indx1, " ");

            _indx1++;
            linesProccessed.Insert(_indx1, " CREW : <span class='prop' id='prop_crew1'>...</span>/<span class='prop' id='prop_crew2'>...</span>/<span class='prop' id='prop_crew3'>...</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " PAX  : <span class='prop' id='prop_pax1'>...</span>/<span class='prop' id='prop_pax2'>...</span>/<span class='prop' id='prop_pax3'>...</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " T.O.B: <span class='prop' id='prop_tob'>............</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " A.TAKEOFF W. : <span class='prop' id='prop_atakeoffw'>............</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " A.TAKEOFF F. : <span class='prop' id='prop_atakeofff'>............</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " ");

            // CPT    FL  SOT    TAS WIND   COM AW      ZT   ZD   ETO   ATO  REM
            nospaceproccessed = linesProccessed.Select(q => q.Trim().Replace(" ", "").ToLower()).ToList();
            var _indexS = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().StartsWith("cptflsottas")).First());
            //-------------------------WAYPOINT COORDINATION-----------------------
            var _indexE = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().Contains("waypointcoordination")).First());
            // OIFM   DES                   T12 GADLU1N 0.14 063  ....  .... 000214 
            //                                  062     0.35 0186 ....  .... 001902
            var propIndex = 1;
            for (int i = _indexS; i < _indexE; i++)
            {
                var ln = linesProccessed[i];
                var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                if (prts.Length > 1)
                {
                    var chrs = ln.ToCharArray();
                    var str = "";
                    foreach (var x in prts)
                    {
                        str += x;
                        if (x != prts.Last())
                        {

                            //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                            //    str += "  ";
                            str += "<span class='prop' id='prop_" + propIndex + "'>" + "...." + "</span>"; // "@prop_" + propIndex;
                            propIndex++;
                        }

                    }
                    linesProccessed[i] = str;
                }
            }
            var finalResult = new List<string>();
            //finalResult.Add("<pre>");
            foreach (var x in linesProccessed)
            //finalResult.Add("<div>" + /*Regex.Replace(x, @"\s+", "&nbsp;")*/ReplaceWhitespace(x, "&nbsp;") + " </div>") ;
            {
                finalResult.Add(x);
                plan.OFPImportItems.Add(new OFPImportItem()
                {
                    Line = x
                });
            }
            var _text = "<pre>" + string.Join("<br/>", finalResult) + "</pre>";
            plan.TextOutput = _text;
            context.SaveChanges();
            //finalResult.Add("</pre>");
            return Ok(_text);
        }


        [Route("api/ofp/parse/text2")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostImportOFP2(dynamic dto)
        {
            try
            {
                string user = Convert.ToString(dto.user);
                int fltId = Convert.ToInt32(dto.fltId);
                //var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
                var context = new AirpocketAPI.Models.FLYEntities();

                var flight = context.ViewLegTimes.FirstOrDefault(q => q.ID == fltId);
                //var fnprts = (fn.Split('_')[1]).Split('.');
                //var _date = new DateTime(Convert.ToInt32(fnprts[0]), Convert.ToInt32(fnprts[1]), Convert.ToInt32(fnprts[2]));
                //var _fltno = (fnprts[5]).PadLeft(4, '0');
                // var _origin = fnprts[3];
                //var _destination = fnprts[4];


                //string folder = HttpContext.Current.Server.MapPath("~/upload");
                // string path = Path.Combine(folder, fn);

                //var ftext = File.ReadAllText(path);
                string ftext = Convert.ToString(dto.text);
                ftext = ftext.Replace("........", "^");
                ftext = ftext.Replace(" .......", "........");
                ftext = ftext.Replace("^", "........");


                var lines = ftext.Split(new[] { '\r', '\n' }).ToList();
                var linesNoSpace = lines.Select(q => q.Replace(" ", "")).ToList();
                var linesTrimStart = lines.Select(q => q.TrimStart()).ToList();



                var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == fltId);
                if (cplan != null)
                    context.OFPImports.Remove(cplan);
                List<string> props = new List<string>();
                var plan = new OFPImport()
                {
                    DateCreate = DateTime.Now,
                    DateFlight = flight.STDDay,
                    FileName = "",
                    FlightNo = flight.FlightNumber,
                    Origin = flight.FromAirportICAO,
                    Destination = flight.ToAirportICAO,
                    User = user,
                    Text = ftext,


                };
                if (flight != null)
                    plan.FlightId = flight.ID;
                context.OFPImports.Add(plan);


                //var fuelParts = OFPHelper.GetFuelParts(lines, linesNoSpace);



                var linesProccessed = new List<string>();
                foreach (var ln in lines)
                {
                    var nospace = ln.Replace(" ", "");
                    //Cont 05%  000223 00.05            ............  PAX  : .../.../...
                    if (nospace.ToLower().StartsWith("cont05"))
                    {
                        var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                        linesProccessed.Add(prts[0]);

                    }
                    // HLD       001299 00.30            A.TAKEOFF W.  CREW : .../.../... 
                    else if (nospace.ToLower().StartsWith("hld"))
                    {
                        var prts = ln.Split(new string[] { "A." }, StringSplitOptions.None);
                        linesProccessed.Add(prts[0]);

                    }
                    // TXY       000200                  A.TAKEOFF F.  T.O.B: ...........  
                    else if (nospace.ToLower().StartsWith("txy"))
                    {
                        var prts = ln.Split(new string[] { "A." }, StringSplitOptions.None);
                        linesProccessed.Add(prts[0]);
                    }
                    //REQUIRED  006414 02.12            ............
                    else if (nospace.ToLower().StartsWith("required"))
                    {
                        var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                        linesProccessed.Add(prts[0]);
                    }
                    //OFP WORKED By CAPT:  .. .. .. .. .. ..  FO: .. .. .. .. .. .. .. ..
                    else if (nospace.ToLower().StartsWith("ofpworked"))
                    {
                        // var prts = ln.Split(new string[] { "FO" }, StringSplitOptions.None);
                        linesProccessed.Add(" OFP WORKED By CAPT:" +/*@ofbcpt*/"<span class='prop' id='prop_ofbcpt' ng-click='propClick($event)'>  .. .. .. .. .. ..  </span>" + "FO: " +/*@ofbfo"*/"<span ng-click='propClick($event)' class='prop' id='prop_ofbfo'>.. .. .. .. .. .. .. ..</span>");
                        props.Add("prop_ofbcpt");
                        props.Add("prop_ofbfo");
                    }
                    // CLEARANCE: .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. ..
                    else if (nospace.ToLower().StartsWith("clearance"))
                    {
                        // var prts = ln.Split(new string[] { "FO" }, StringSplitOptions.None);
                        linesProccessed.Add(" CLEARANCE: " + "<span ng-click='propClick($event)' class='prop' id='prop_clearance'>.. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. ..</span>");
                        props.Add("prop_clearance");
                    }
                    else
                        linesProccessed.Add(ln);
                }

                var nospaceproccessed = linesProccessed.Select(q => q.Trim().Replace(" ", "").ToLower()).ToList();
                var _indx1 = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().StartsWith("total")).First());
                _indx1++;
                linesProccessed.Insert(_indx1, " ");

                _indx1++;
                linesProccessed.Insert(_indx1, " CREW : <span ng-click='propClick($event)' class='prop' id='prop_crew1'>...</span>/<span ng-click='propClick($event)' class='prop' id='prop_crew2'>...</span>/<span ng-click='propClick($event)' class='prop' id='prop_crew3'>...</span>");
                props.Add("prop_crew1");
                props.Add("prop_crew2");
                props.Add("prop_crew3");
                _indx1++;
                linesProccessed.Insert(_indx1, " PAX  : <span ng-click='propClick($event)' class='prop' id='prop_pax1'>...</span>/<span ng-click='propClick($event)' class='prop' id='prop_pax2'>...</span>/<span ng-click='propClick($event)' class='prop' id='prop_pax3'>...</span>");
                props.Add("prop_pax1");
                props.Add("prop_pax2");
                props.Add("prop_pax3");
                _indx1++;
                linesProccessed.Insert(_indx1, " T.O.B: <span ng-click='propClick($event)' class='prop' id='prop_tob'>............</span>");
                props.Add("prop_tob");
                _indx1++;
                linesProccessed.Insert(_indx1, " A.TAKEOFF W. : <span ng-click='propClick($event)' class='prop' id='prop_atakeoffw'>............</span>");
                props.Add("prop_atakeoffw");
                _indx1++;
                linesProccessed.Insert(_indx1, " A.TAKEOFF F. : <span ng-click='propClick($event)' class='prop' id='prop_atakeofff'>............</span>");
                props.Add("prop_atakeofff");
                _indx1++;
                linesProccessed.Insert(_indx1, " ");

                // CPT    FL  SOT    TAS WIND   COM AW      ZT   ZD   ETO   ATO  REM
                nospaceproccessed = linesProccessed.Select(q => q.Trim().Replace(" ", "").ToLower()).ToList();
                var _indexS = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().StartsWith("cptflsottas")).First());
                //-------------------------WAYPOINT COORDINATION-----------------------
                var _indexE = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().Contains("waypointcoordination")).First());
                // OIFM   DES                   T12 GADLU1N 0.14 063  ....  .... 000214 
                //                                  062     0.35 0186 ....  .... 001902
                var propIndex = 1;
                for (int i = _indexS; i < _indexE; i++)
                {
                    var ln = linesProccessed[i];
                    List<string> prts = new List<string>();
                    var dots = "";
                    bool dot8 = false;
                    var prts8 = ln.Split(new string[] { "........" }, StringSplitOptions.None);
                    var prts4 = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                    if (prts8.Length > 1)
                    {
                        prts = prts8.ToList();
                        dots = "........";
                        dot8 = true;
                    }
                    else
                    {
                        prts = prts4.ToList();
                        dots = "....";
                    }
                    if (prts.Count > 1)
                    {
                        var chrs = ln.ToCharArray();
                        var str = "";
                        foreach (var x in prts)
                        {
                            str += x;
                            if (x != prts.Last())
                            {

                                //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                //    str += "  ";
                                str += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + dots + "</span>"; // "@prop_" + propIndex;
                                props.Add("prop_" + propIndex);
                                propIndex++;
                            }

                        }
                        linesProccessed[i] = str;
                    }
                }
                var finalResult = new List<string>();
                //finalResult.Add("<pre>");
                foreach (var x in linesProccessed)
                //finalResult.Add("<div>" + /*Regex.Replace(x, @"\s+", "&nbsp;")*/ReplaceWhitespace(x, "&nbsp;") + " </div>") ;
                {
                    finalResult.Add(x);
                    plan.OFPImportItems.Add(new OFPImportItem()
                    {
                        Line = x
                    });
                }
                var _text = "<pre>" + string.Join("<br/>", finalResult) + "</pre>";
                plan.TextOutput = _text;
                var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                foreach (var p in props)
                {
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        DateUpdate = dtupd,
                        PropName = p,
                        PropValue = "",
                        User = user,

                    });
                }
                context.SaveChanges();
                //context.SaveChangesAsync();
                //finalResult.Add("</pre>");
                return Ok(_text);
            }
            catch (Exception ex)
            {
                return Ok("-1");

            }

        }

        [Route("api/ofp/modify/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetModifyOFP(int id)
        {
            try
            {
                var context = new AirpocketAPI.Models.FLYEntities();
                var ofpsQry = context.OFPImports.Where(q => q.Id > 0);
                if (id != -1)
                    ofpsQry = ofpsQry.Where(q => q.Id == id);
                var ofps = ofpsQry.ToList();
                var ofpIds = ofps.Select(q => q.Id).ToList();
                var ofpItemsAll = context.OFPImportItems.Where(q => ofpIds.Contains(q.OFPId)).ToList();
                foreach (var ofp in ofps)
                {
                    var items = ofpItemsAll.Where(q => q.OFPId == ofp.Id).OrderBy(q => q.Id).ToList();
                    var output = new List<string>();
                    foreach (var item in items)
                    {
                        if (!item.Line.Contains("<div class='z5000"))
                        {
                            var str = item.Line.Replace("<span", "<input");
                            str = str.Replace("</span>", "</input>");
                            output.Add(str);

                        }
                        else
                        {
                            output.Add(item.Line);
                        }
                    }

                    //ofp.TextOutput = string.Join("", output);
                    ofp.TextOutput = "<pre>" + string.Join("<br/>", output) + "</pre>";
                }
                context.SaveChanges();

                return Ok(true);

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }



        }


        [Route("api/ofp/parse/text")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostImportOFP(dynamic dto)
        {
            try
            {
                string user = Convert.ToString(dto.user);
                int fltId = Convert.ToInt32(dto.fltId);
                //var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
                var context = new AirpocketAPI.Models.FLYEntities();

                var flight = context.ViewLegTimes.FirstOrDefault(q => q.ID == fltId);
                var flightObj = context.FlightInformations.FirstOrDefault(q => q.ID == fltId);
                flightObj.ALT1 = "";
                flightObj.ALT2 = "";

                //var fnprts = (fn.Split('_')[1]).Split('.');
                //var _date = new DateTime(Convert.ToInt32(fnprts[0]), Convert.ToInt32(fnprts[1]), Convert.ToInt32(fnprts[2]));
                //var _fltno = (fnprts[5]).PadLeft(4, '0');
                // var _origin = fnprts[3];
                //var _destination = fnprts[4];


                //string folder = HttpContext.Current.Server.MapPath("~/upload");
                // string path = Path.Combine(folder, fn);

                //var ftext = File.ReadAllText(path);
                string ftext = Convert.ToString(dto.text);
                ftext = ftext.Replace("..............", "....");
                ftext = ftext.Replace(".............", "....");
                ftext = ftext.Replace("............", "....");
                ftext = ftext.Replace("...........", "....");
                ftext = ftext.Replace("........", "....");
                ftext = ftext.Replace(".......", "....");

                //   ftext = ftext.Replace("........", "^");
                //ftext = ftext.Replace(" .......", "........");
                // ftext = ftext.Replace("^", "........");
                ftext = ftext.Replace(".../.../...", "..../..../....");


                var lines = ftext.Split(new[] { '\r', '\n' }).ToList();
                var linesNoSpace = lines.Select(q => q.Replace(" ", "")).ToList();
                var linesTrimStart = lines.Select(q => q.TrimStart()).ToList();



                var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == fltId);
                if (cplan != null)
                    context.OFPImports.Remove(cplan);
                List<string> props = new List<string>();
                var plan = new OFPImport()
                {
                    DateCreate = DateTime.Now,
                    DateFlight = flight.STDDay,
                    FileName = "",
                    FlightNo = flight.FlightNumber,
                    Origin = flight.FromAirportICAO,
                    Destination = flight.ToAirportICAO,
                    User = user,
                    Text = ftext,


                };
                if (flight != null)
                    plan.FlightId = flight.ID;
                context.OFPImports.Add(plan);


                //var fuelParts = OFPHelper.GetFuelParts(lines, linesNoSpace);


                var propIndex = 1;
                var linesProccessed = new List<string>();
                //foreach (var ln in lines)
                for (int cnt = 0; cnt < lines.Count; cnt++)
                {
                    var ln = lines[cnt];

                    if (ln.Replace(" ", "").ToUpper().Contains("CREW"))
                    {
                        var crewLine = " CREW  : ";
                        crewLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        props.Add("prop_" + propIndex);
                        propIndex++;

                        linesProccessed.Add(crewLine);
                        linesProccessed.Add("<br/>");

                        var paxLine = " PAX   : ";
                        paxLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_adult'>" + "" + "</span>";
                        paxLine += "/";
                        props.Add("prop_pax_adult");
                        paxLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_child'>" + "" + "</span>";
                        paxLine += "/";
                        props.Add("prop_pax_child");
                        paxLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_infant'>" + "" + "</span>";
                        props.Add("prop_pax_infant");
                        linesProccessed.Add(paxLine);
                        linesProccessed.Add("<br/>");

                        var sobLine = " S.O.B : ";
                        sobLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        linesProccessed.Add(sobLine);

                        ln = " ";


                    }
                    else
                    {
                        var alt1 = ln.Split(new string[] { "1ST ALT" }, StringSplitOptions.None).ToList();
                        if (alt1.Count() > 1)
                        {
                            if (!string.IsNullOrEmpty(alt1.Last().Replace(" ", "").Trim()))
                            {
                                var _alt1 = alt1.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT1 = _alt1;
                            }


                        }
                        var alt2 = ln.Split(new string[] { "2ND ALT" }, StringSplitOptions.None).ToList();
                        if (alt2.Count() > 1)
                        {
                            if (!string.IsNullOrEmpty(alt2.Last().Replace(" ", "").Trim()))
                            {
                                var _alt2 = alt2.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT2 = _alt2;
                            }


                        }
                        var clr = ln.Split(new string[] { "ATC CLRNC:" }, StringSplitOptions.None).ToList();
                        if (clr.Count() > 1)
                        {
                            ln = clr[0] + "ATC CLRNC: " + "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        var offblk = ln.Split(new string[] { "OFF BLK" }, StringSplitOptions.None).ToList();
                        if (offblk.Count() > 1)
                        {
                            ln = offblk[0] + "OFF BLK : " + "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_offblock" + "'>" + "" + "</span>";
                            props.Add("prop_offblock");
                            //propIndex++;
                        }
                        var takeoff = ln.Split(new string[] { "TAKE OFF" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("ALTN") && takeoff.Count() > 1)
                        {
                            ln = takeoff[0] + "TAKE OFF: " + "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_takeoff" + "'>" + "" + "</span>";
                            props.Add("prop_takeoff");
                            //propIndex++;
                        }
                        var lnd = ln.Split(new string[] { "ON RUNWAY" }, StringSplitOptions.None).ToList();
                        if (lnd.Count() > 1)
                        {
                            ln = lnd[0] + "ON RUNWAY  : " + "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_landing" + "'>" + "" + "</span>";
                            props.Add("prop_landing");
                            //propIndex++;
                        }
                        var onblock = ln.Split(new string[] { "ON BLK" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("FUEL") && onblock.Count() > 1)
                        {
                            ln = onblock[0] + "ON BLK     : " + "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_onblock" + "'>" + "" + "</span>";
                            props.Add("prop_onblock");
                            //propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("DISP"))
                        {
                            var picIndex = ln.IndexOf("PIC");
                            var dispIndex = ln.IndexOf("DISP");
                            var dispStr = ln.Substring(0, ln.Length - (picIndex + 3));
                            var _disps = dispStr.Replace(":", " ").Split(new string[] { "DISP" }, StringSplitOptions.None).ToList();
                            var dispatcher = _disps[1].Replace(" ", "");

                            var picStr = ln.Substring(picIndex);
                            var _pics = picStr.Replace(":", " ").Split(new string[] { "PIC" }, StringSplitOptions.None).ToList();
                            var pic = _pics[1].Replace(" ", "").Replace(".", ". ");


                            ln = "<div class='z5000 h70'> " + "<span id='sig_disp' data-info='_null_' class='sig'><span class='sig_name'>DISP : " + dispatcher + "</span><img id='sig_disp_img' class='sig_img' /></span>" + "          " + "<span id='sig_pic' data-info='_null_' class='sig left80'><span data-info='_null_' class='sig_name'>" + "PIC : " + pic + "</span><img id='sig_pic_img' class='sig_img' /></span></div>";

                        }

                        var sign = ln.Split(new string[] { "SIGNATURE  :" }, StringSplitOptions.None).ToList();
                        if (sign.Count() > 1)
                        {
                            //ln="<div> "+sign[0]+ "SIGNATURE : "+sign[1]+
                            ln = "";
                        }

                        //OFP WORKED By CAPT:
                        //var cpt = ln.Split(new string[] { "OFP WORKED By CAPT:" }, StringSplitOptions.None).ToList();
                        //if (clr.Count() > 1)
                        //{
                        //    ln = clr[0] + "OFP WORKED By CAPT: " + "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        //    props.Add("prop_" + propIndex);
                        //    propIndex++;
                        //    ln += "      FO: " + "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        //    props.Add("prop_" + propIndex);
                        //    propIndex++;
                        //}
                        //OFP WORKED
                        if (ln.Replace(" ", "").ToUpper().Contains("OFPWORKED"))
                        {

                            ln = " OFP WORKED By CAPT: " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbcpt_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_ofbcpt_" + propIndex);
                            propIndex++;
                            ln += "      FO: " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbfo_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_ofbfo_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS1"))
                        {

                            //lines[cnt + 1] = "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            ln = " ENR ATIS 1 : " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS2"))
                        {

                            //lines[cnt + 1] = "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            ln = " ENR ATIS 2 : " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS3"))
                        {

                            //lines[cnt + 1] = "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            ln = " ENR ATIS 3 : " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS4"))
                        {

                            //lines[cnt + 1] = "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            ln = " ENR ATIS 4 : " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_" + propIndex);
                            propIndex++;
                        }



                        //var crew = ln.Split(new string[] { "..../..../...." }, StringSplitOptions.None).ToList();
                        //if (crew.Count > 1)
                        //{

                        //    //CREW : .../.../...        PAX  : .../.../...       S.O.B: ...........
                        //    var cstr = "";
                        //    foreach (var q in crew)
                        //    {
                        //        if (q.ToLower().Contains("crew"))
                        //        {
                        //            cstr += q;
                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            cstr += "/";
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;

                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            cstr += "/";
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;

                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;
                        //        }
                        //        else if (q.ToLower().Contains("pax"))
                        //        {
                        //            cstr += q;
                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            cstr += "/";
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;

                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            cstr += "/";
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;

                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;
                        //        }
                        //        else
                        //            cstr += q;



                        //    }
                        //    ln = cstr;
                        //}


                        List<string> prts = new List<string>();
                        var dots = "";
                        bool dot8 = false;

                        var prts4 = ln.Split(new string[] { "...." }, StringSplitOptions.None).ToList();
                        if (prts4.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts4)
                            {
                                str += x;
                                if (x != prts4.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }

                        var prts8 = ln.Split(new string[] { "........" }, StringSplitOptions.None).ToList();
                        if (prts8.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts8)
                            {
                                str += x;
                                if (x != prts8.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }
                        var prts7 = ln.Split(new string[] { "......." }, StringSplitOptions.None).ToList();
                        if (prts7.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts7)
                            {
                                str += x;
                                if (x != prts7.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }


                        linesProccessed.Add(ln);
                    }

                }

                //                              062     0.35 0186 ....  .... 001902
                //var propIndex = 1;
                //for (int i = _indexS; i < _indexE; i++)
                //{
                //    var ln = linesProccessed[i];

                //}
                var finalResult = new List<string>();
                //finalResult.Add("<pre>");
                var proOn = false;
                var _f = 0;
                for (int i = 0; i < linesProccessed.Count(); i++)
                {

                    var pln = linesProccessed[i];
                    if (pln.ToUpper().Contains("SCHD DEP"))
                        proOn = true;
                    if (pln.ToUpper().Contains("SCHD ARR"))
                        proOn = false;

                    if (proOn)
                    {
                        if (pln.Contains("-- -- -- --"))
                        {
                            _f++;
                            if (_f > 1)
                            {
                                var next = linesProccessed[i + 1];
                                var spanIndex = next.IndexOf("<span");
                                var str = next.Substring(0, spanIndex).Trim();
                                var prts = str.Split(' ').ToList();
                                var timeStr = prts[prts.Count() - 2].Split('.');
                                var mins = Convert.ToInt32(timeStr[0]) * 60 + Convert.ToInt32(timeStr[1]);
                                var infoIndex = next.IndexOf("_null_");
                                var fin = next.Substring(0, infoIndex) + "time_" + i + "_" + mins + next.Substring(infoIndex + 6);
                                linesProccessed[i + 1] = fin;

                            }
                        }

                    }
                }
                foreach (var x in linesProccessed)
                //finalResult.Add("<div>" + /*Regex.Replace(x, @"\s+", "&nbsp;")*/ReplaceWhitespace(x, "&nbsp;") + " </div>") ;
                {
                    finalResult.Add(x);
                    plan.OFPImportItems.Add(new OFPImportItem()
                    {
                        Line = x
                    });
                }
                var _text = "<pre>" + string.Join("<br/>", finalResult) + "</pre>";
                plan.TextOutput = _text;
                var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                foreach (var p in props)
                {
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        DateUpdate = dtupd,
                        PropName = p,
                        PropValue = "",
                        User = user,

                    });
                }
                context.SaveChanges();
                //context.SaveChangesAsync();
                //finalResult.Add("</pre>");
                return Ok(_text);
            }
            catch (Exception ex)
            {
                return Ok("-1");

            }

        }




        [Route("api/ofp/modify/fuel")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetModifyOFPFuel()
        {
            try
            {
                var context = new AirpocketAPI.Models.FLYEntities();
                var ofpItems = context.OFPImportItems.Where(q => q.Line.Contains("TRIP") || q.Line.Contains("TOTAL")).ToList();
                var ofpIds = ofpItems.Select(q => q.OFPId).ToList();
                var ofps = context.OFPImports.Where(q => ofpIds.Contains(q.Id)).ToList();
                var flightIds = ofps.Select(q => q.FlightId).ToList();
                var flights = context.FlightInformations.Where(q => flightIds.Contains(q.ID)).ToList();

                foreach (var item in ofpItems)
                {
                    var ofp = ofps.FirstOrDefault(q => q.Id == item.OFPId);
                    var flight = flights.FirstOrDefault(q => q.ID == ofp.FlightId);
                    if (ofp != null && flight != null)
                    {
                        var ln = item.Line;
                        try
                        {

                            //ln.Split(new string[] { "TAKE OFF" }, StringSplitOptions.None)
                            var _sprts = ln.Split(' ');
                            var _nos = new List<double>();
                            foreach (var _p in _sprts)
                            {
                                double _n = -1;
                                bool isNumeric = double.TryParse(_p, out _n);
                                if (isNumeric) _nos.Add(_n);

                            }
                            if (_nos.Count > 0)
                            {
                                var _f = Convert.ToDecimal(_nos.First());
                                if (ln.Contains("TRIP"))
                                {
                                    //  ofp.FPTripFuel = _f;
                                    flight.FPTripFuel = _f;
                                }
                                if (ln.Contains("TOTAL"))
                                {
                                    //   ofp.FPFuel = _f;
                                    flight.FPFuel = _f;
                                }
                            }




                        }
                        catch (Exception fexp)
                        {

                        }
                    }


                }

                context.SaveChanges();

                return Ok(true);

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }



        }
        [Route("api/atc/text/input")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostImportATCTextInput(dynamic dto)
        {
            string user = Convert.ToString(dto.user);
            int fltId = Convert.ToInt32(dto.fltId);
            var context = new AirpocketAPI.Models.FLYEntities();

            var flight = context.ViewLegTimes.FirstOrDefault(q => q.ID == fltId);
            var flightObj = context.FlightInformations.FirstOrDefault(q => q.ID == fltId);

            string ftext = Convert.ToString(dto.text);
            flightObj.ATCPlan = ftext;
            context.SaveChanges();
            return Ok(true);
        }

        [Route("api/atc/text/get/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult PostImportATCTextGET(int id)
        {

            var context = new AirpocketAPI.Models.FLYEntities();


            var flightObj = context.FlightInformations.FirstOrDefault(q => q.ID == id);


            return Ok(flightObj.ATCPlan);
        }


        [Route("api/ofp/sky/parse")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetSkyOFP()
        {
            List<string> props = new List<string>();
            var context = new AirpocketAPI.Models.FLYEntities();
            var ofpSky = context.OFPSkyPuters.FirstOrDefault();
            var rawText = ofpSky.OFP;
            // var mpln = rawText.Split(new string[] { "mpln:|" }, StringSplitOptions.None).ToList()[1];
            var parts = rawText.Split(new string[] { "||" }, StringSplitOptions.None).ToList();

            var info = parts.FirstOrDefault(q => q.StartsWith("binfo:|")).Replace("binfo:|", "");
            var infoRows = info.Split(';').ToList();
            //binfo:|OPT=VARESH AIRLINE;FLN=VAR5820;DTE=6/24/2022 12:00:00 AM;ETD=02:35;REG=;MCI=78;FLL=330;DOW=43742
            var opt = infoRows.FirstOrDefault(q => q.StartsWith("OPT")).Split('=')[1];
            var fln = infoRows.FirstOrDefault(q => q.StartsWith("FLN")).Split('=')[1];
            var dte = infoRows.FirstOrDefault(q => q.StartsWith("DTE")).Split('=')[1];
            var etd = infoRows.FirstOrDefault(q => q.StartsWith("ETD")).Split('=')[1];
            var reg = infoRows.FirstOrDefault(q => q.StartsWith("REG")).Split('=')[1];
            var mci = infoRows.FirstOrDefault(q => q.StartsWith("MCI")).Split('=')[1];
            var fll = infoRows.FirstOrDefault(q => q.StartsWith("FLL")).Split('=')[1];
            var dow = infoRows.FirstOrDefault(q => q.StartsWith("DOW")).Split('=')[1];

            var flightDate = DateTime.Parse(dte);
            var no = fln.Substring(3);

            var flight = context.ViewLegTimes.Where(q => q.STDDay == flightDate && q.FlightNumber == no).FirstOrDefault();

            var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == flight.ID);
            if (cplan != null)
                context.OFPImports.Remove(cplan);

            var plan = new OFPImport()
            {
                DateCreate = DateTime.Now,
                DateFlight = flight.STDDay,
                FileName = "",
                FlightNo = flight.FlightNumber,
                Origin = flight.FromAirportICAO,
                Destination = flight.ToAirportICAO,
                User = "airpocket",
                Text = rawText,



            };
            if (!string.IsNullOrEmpty(dow))
                plan.DOW = Convert.ToDecimal(dow);
            if (!string.IsNullOrEmpty(fll))
                plan.FLL = Convert.ToDecimal(fll);
            if (!string.IsNullOrEmpty(mci))
                plan.MCI = Convert.ToDecimal(mci);
            plan.Source = "SkyPuter";

            if (flight != null)
                plan.FlightId = flight.ID;
            context.OFPImports.Add(plan);

            var mpln = parts.FirstOrDefault(q => q.StartsWith("mpln:|")).Replace("mpln:|", "");
            var mplnRows = mpln.Split('|').ToList();
            //WAP=OIKB;COR=N27° 13' 06" ,  E056;FRE= ;VIA=ASMU1A;ALT=CLB;MEA=0;GMR=131;DIS=0;TDS=0;WID=;TRK=;TMP=;TME=00:00:00.0000000;TTM=00:00:00.0000000;FRE=-200;FUS=200;TAS=361;GSP=0
            List<JObject> mplnpJson = new List<JObject>();
            var idx = 0;
            foreach (var r in mplnRows)
            {
                var procStr = "";
                var _r = r.Replace("=;", "= ;");
                var prts = _r.Split(';');
                foreach (var x in prts)
                {
                    var str = x.Replace("\"", "^").Replace("'", "#");
                    var substr = str.Split('=')[0] + ":'" + str.Split('=')[1] + "'";

                    procStr += substr;
                    if (x != prts.Last())
                        procStr += ",";
                }
                procStr = "{" + procStr + "}";

                var jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                var _key = ("mpln_WAP_" + jsonObj.GetValue("WAP").ToString()).ToLower();
                jsonObj.Add("_key", _key);
                props.Add("prop_" + _key + "_eta_" + idx);
                props.Add("prop_" + _key + "_ata_" + idx);
                props.Add("prop_" + _key + "_rem_" + idx);
                props.Add("prop_" + _key + "_usd_" + idx);
                mplnpJson.Add(jsonObj);
                idx++;

            }
            //   var _r0 ="{"+ mplnRows.First().Replace("=;", ":''").Replace("= ;", ":''").Replace("=", ":").Replace(";", ",")+"}";
            // var jsonObj = JsonConvert.DeserializeObject<JObject>(_r0);


            var apln1 = parts.FirstOrDefault(q => q.StartsWith("apln:|")).Replace("apln:|", "");
            var apln1Rows = apln1.Split('|').ToList();
            List<JObject> apln1Json = new List<JObject>();
            idx = 0;
            foreach (var r in apln1Rows)
            {
                var procStr = "";
                var _r = r.Replace("=;", "= ;");
                var prts = _r.Split(';');
                foreach (var x in prts)
                {
                    var str = x.Replace("\"", "^").Replace("'", "#");
                    var substr = str.Split('=')[0] + ":'" + str.Split('=')[1] + "'";

                    procStr += substr;
                    if (x != prts.Last())
                        procStr += ",";
                }
                procStr = "{" + procStr + "}";

                var jsonObj = JsonConvert.DeserializeObject<JObject>(procStr);
                var _key = ("apln_WAP_" + jsonObj.GetValue("WAP").ToString()).ToLower();
                jsonObj.Add("_key", _key);
                props.Add("prop_" + _key + "_eta_" + idx);
                props.Add("prop_" + _key + "_ata_" + idx);
                props.Add("prop_" + _key + "_rem_" + idx);
                props.Add("prop_" + _key + "_usd_" + idx);
                apln1Json.Add(jsonObj);
                idx++;

            }





            var futbl = parts.FirstOrDefault(q => q.StartsWith("futbl:|")).Replace("futbl:|", "");
            var prmParts = futbl.Split('|');
            var fuel = new List<fuelPrm>();
            idx = 0;
            foreach (var x in prmParts)
            {
                var _prts = x.Split(';');
                //PRM=TRIP FUEL;TIM=17:26:00.00000;VAL=99960|
                var prm = _prts[0].Split('=')[1];
                var tim = _prts[1].Split('=')[1];
                var val = _prts[2].Split('=')[1];
                var _key = "fuel_" + (prm != "CONT[5%]" ? prm.ToLower() : "cont05");
                fuel.Add(new fuelPrm()
                {
                    prm = prm,
                    time = tim,
                    value = val,
                    _key = _key,
                });

                if (prm == "TRIP FUEL")
                    plan.FPTripFuel = Convert.ToDecimal(val);


                props.Add("prop_" + _key + "_" + idx);

                idx++;
            }
            fuel.Add(new fuelPrm()
            {
                prm = "REQ",
                _key = "fuel_" + "req"
            }); ;
            props.Add("prop_fuel_" + "req" + "_" + idx);
            var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            foreach (var prop in props)
                plan.OFPImportProps.Add(new OFPImportProp()
                {
                    DateUpdate = dtupd,
                    PropName = prop,
                    PropValue = "",
                    User = "airpocket",

                });
            plan.JFuel = string.Join("", fuel);
            plan.JPlan = string.Join("", mplnpJson);
            plan.JAPlan1 = string.Join("", apln1Json);
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {

                    foreach (var ve in eve.ValidationErrors)
                    {
                        var xxx =
                             ve.PropertyName + " " + ve.ErrorMessage;
                    }
                }
                throw;
            }

            var result = new
            {
                mplnpJson
                ,
                apln1Json
                ,
                fuel
                ,
                plan

            };
            return Ok(result);

        }
        public class fuelPrm
        {
            public string prm { get; set; }
            public string time { get; set; }
            public string value { get; set; }


            public string _key { get; set; }
        }
        public class phpdto
        {
            public string text { get; set; }
            public string user { get; set; }
            public string fltId { get; set; }
        }
        [Route("api/ofp/parse/text/input/varesh")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostImportOFPInputVaresh(phpdto dto)
        {
            //kool
            try
            {
                string ftext = Convert.ToString(dto.text);
                if (ftext.StartsWith("PLAN"))
                    return PostImportOFPInputVareshAIRBUS(dto);

                string user = Convert.ToString(dto.user);
                int fltId = Convert.ToInt32(dto.fltId);
                //var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
                var context = new AirpocketAPI.Models.FLYEntities();

                var flight = context.ViewLegTimes.FirstOrDefault(q => q.ID == fltId);
                var flightObj = context.FlightInformations.FirstOrDefault(q => q.ID == fltId);
                flightObj.ALT1 = "";
                flightObj.ALT2 = "";



                ftext = ftext.Replace("..............", "....");
                ftext = ftext.Replace(".............", "....");
                ftext = ftext.Replace("............", "....");
                ftext = ftext.Replace("...........", "....");
                ftext = ftext.Replace("........", "....");
                ftext = ftext.Replace(".......", "....");
                //ftext = ftext.Replace("Ground ..... ..... ..... .....", "Ground");
                //ftext = ftext.Replace("..... ..... ..... ..... .....", "****");
                ftext = ftext.Replace(".....", "....");


                ftext = ftext.Replace(".../.../...", "..../..../....");


                var lines = ftext.Split(new[] { '\r', '\n' }).ToList();
                lines.Add("ENRATIS1");
                lines.Add("ENRATIS2");
                lines.Add("ENRATIS3");
                lines.Add("ENRATIS4");
                var linesNoSpace = lines.Select(q => q.Replace(" ", "")).ToList();
                var linesTrimStart = lines.Select(q => q.TrimStart()).ToList();



                var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == fltId);
                if (cplan != null)
                    context.OFPImports.Remove(cplan);
                List<string> props = new List<string>();
                var plan = new OFPImport()
                {
                    DateCreate = DateTime.Now,
                    DateFlight = flight.STDDay,
                    FileName = "",
                    FlightNo = flight.FlightNumber,
                    Origin = flight.FromAirportICAO,
                    Destination = flight.ToAirportICAO,
                    User = user,
                    Text = ftext,


                };
                if (flight != null)
                    plan.FlightId = flight.ID;
                context.OFPImports.Add(plan);



                var propIndex = 1;
                var linesProccessed = new List<string>();
                double? fuelTrip = null;
                double? fuelTotal = null;

                var _olines = new List<string>();
                var cln = 0;
                //foreach(var ln in lines)
                while (cln < lines.Count)
                {
                    var ln = lines[cln];
                    //10-17
                    if (ln.Contains("RVSM ALTIMETER CHECKS"))
                    {
                        _olines.Add(ln);

                        _olines.Add("PHASE          CPT              STBY              FO               TIME");
                        _olines.Add("                      ");

                        _olines.Add("Ground         $cptgnd      $stbygnd      $fognd      $timegnd");
                        _olines.Add("                      ");
                        _olines.Add("Flight         $cptflt28      $stbyflt28      $foflt28      $timeflt28");
                        _olines.Add("                      ");
                        _olines.Add("Flight         $cptfltfl      $stbyfltfl      $fofltfl      $timefltfl");
                        _olines.Add("                      ");
                        cln = cln + 7;

                    }
                    else if (ln.Contains("N") && ln.Contains(" E") && ln.Contains("/"))
                    {


                        try
                        {
                            _olines.Add(ln);
                            cln++;
                            var _fl = "                                                                             xxxx";
                            var _pln = lines[cln - 2];
                            if (_pln.Last() != ' ')
                                _pln += " ";
                            var _plnParts = _pln.Split(' ');
                            var _plnf = _plnParts[_plnParts.Length - 2];
                            var _fvalue = Convert.ToDouble(_plnf);
                            _fl += _fvalue.ToString();
                            _olines.Add(_fl);
                            // cln++;

                        }
                        catch (Exception _ex2)
                        {
                            _olines.Add(ln);
                            cln++;
                        }

                    }
                    else
                    {
                        _olines.Add(ln);
                        if (ln.Replace(" ", "").ToUpper().Contains("TOTAL"))
                        {
                            _olines.Add(" REQUESTED ");
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("TAKEOFF:"))
                        {

                            _olines.Add("                                                 TAKE OFF FUEL: ");
                        }

                        cln++;
                    }


                }

                lines = _olines.ToList();


                for (int cnt = 0; cnt < lines.Count; cnt++)
                {
                    var ln = lines[cnt];
                    var preLnFuel = "";

                    if (ln.Contains("N") && ln.Contains(" E") && ln.Contains("/"))
                    {
                        // ln += "....  ....";
                        //"                        ....  ....       ";
                        ln = ln.TrimEnd(new Char[] { ' ' });
                        //ln+= "                        ....  ....       ";
                        ln += "                                                 ....  ";
                        //ln += "                                                 xxxx  ";

                    }
                    try
                    {
                        if (ln.Contains("$cptgnd"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$cptgnd", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop ' id='prop_cptgnd'></input>");
                            props.Add("prop_cptgnd");
                            str = str.Replace("$stbygnd", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop ' id='prop_stbygnd'></input>");
                            props.Add("prop_stbygnd");
                            str = str.Replace("$fognd", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop ' id='prop_fognd'></input>");
                            props.Add("prop_fognd");
                            str = str.Replace("$timegnd", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_timegnd'></input>");
                            props.Add("prop_timegnd");
                            ln = str;


                        }
                        if (ln.Contains("$cptfltfl"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$cptfltfl", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_cptflt'></input>");
                            props.Add("prop_cptflt");
                            str = str.Replace("$stbyfltfl", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_stbyflt'></input>");
                            props.Add("prop_stbyflt");
                            str = str.Replace("$fofltfl", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_foflt'></input>");
                            props.Add("prop_foflt");
                            str = str.Replace("$timefltfl", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_timeflt'></input>");
                            props.Add("prop_timeflt");

                            ln = str;
                        }
                        if (ln.Contains("$cptflt28"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$cptflt28", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_cptflt28'></input>");
                            props.Add("prop_cptflt28");
                            str = str.Replace("$stbyflt28", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_stbyflt28'></input>");
                            props.Add("prop_stbyflt28");
                            str = str.Replace("$foflt28", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_foflt28'></input>");
                            props.Add("prop_foflt28");
                            str = str.Replace("$timeflt28", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_timeflt28'></input>");
                            props.Add("prop_timeflt28");

                            ln = str;
                        }
                        if (ln/*.Replace(" ", "")*/.ToUpper().Contains("DEST"))
                        {
                            //ln.Split(new string[] { "TAKE OFF" }, StringSplitOptions.None)
                            var _sprts = ln.Split(' ');
                            var _nos = new List<double>();
                            foreach (var _p in _sprts)
                            {
                                double _n = -1;
                                bool isNumeric = double.TryParse(_p, out _n);
                                if (isNumeric) _nos.Add(_n);

                            }
                            if (_nos.Count > 0)
                                fuelTrip = _nos.First();

                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("TOTAL"))
                        {
                            var _sprts = ln.Split(' ');
                            var _nos = new List<double>();
                            foreach (var _p in _sprts)
                            {
                                double _n = -1;
                                bool isNumeric = double.TryParse(_p, out _n);
                                if (isNumeric) _nos.Add(_n);

                            }
                            if (_nos.Count > 0)
                                fuelTotal = _nos.First();

                        }
                    }
                    catch (Exception fexp)
                    {

                    }
                    if (ln.Replace(" ", "").ToUpper().Contains("CREW"))
                    {
                        var crewLine = " CREW  : ";
                        crewLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        props.Add("prop_" + propIndex);
                        propIndex++;

                        linesProccessed.Add(crewLine);
                        linesProccessed.Add("<br/>");

                        var paxLine = " PAX   : ";
                        paxLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_adult'>" + "" + "</input>";
                        paxLine += "/";
                        props.Add("prop_pax_adult");
                        paxLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_child'>" + "" + "</input>";
                        paxLine += "/";
                        props.Add("prop_pax_child");
                        paxLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_infant'>" + "" + "</input>";
                        props.Add("prop_pax_infant");
                        linesProccessed.Add(paxLine);
                        linesProccessed.Add("<br/>");

                        var sobLine = " S.O.B : ";
                        sobLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        linesProccessed.Add(sobLine);

                        ln = " ";


                    }
                    else
                    {
                        var alt1 = ln.Split(new string[] { "ALT 1" }, StringSplitOptions.None).ToList();
                        if (alt1.Count() > 1 && alt1[1].Length > 20)
                        {
                            if (!string.IsNullOrEmpty(alt1.Last().Replace(" ", "").Trim()))
                            {
                                var _alt1 = alt1.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT1 = _alt1;
                            }


                        }
                        var alt2 = ln.Split(new string[] { "ALT 2" }, StringSplitOptions.None).ToList();
                        if (alt2.Count() > 1)
                        {
                            if (!string.IsNullOrEmpty(alt2.Last().Replace(" ", "").Trim()))
                            {
                                var _alt2 = alt2.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT2 = _alt2;
                            }


                        }
                        var clr = ln.Split(new string[] { "CLEARANCE:" }, StringSplitOptions.None).ToList();
                        if (clr.Count() > 1)
                        {
                            ln = clr[0] + "CLEARANCE: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        var offblk = ln.Split(new string[] { "OFF BLK" }, StringSplitOptions.None).ToList();
                        if (offblk.Count() > 1)
                        {
                            ln = offblk[0] + "OFF BLK : " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_offblock" + "'>" + "" + "</input>";
                            props.Add("prop_offblock");
                            //propIndex++;
                        }

                        var takeoff = ln.Split(new string[] { "TAKE OFF:" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("ALTN") && takeoff.Count() > 1)
                        {
                            ln = takeoff[0] + "TAKE OFF: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_takeoff" + "'>" + "" + "</input>";
                            props.Add("prop_takeoff");
                            //propIndex++;
                        }
                        var lnd = ln.Split(new string[] { "ON RUNWAY" }, StringSplitOptions.None).ToList();
                        if (lnd.Count() > 1)
                        {
                            ln = lnd[0] + "ON RUNWAY  : " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_landing" + "'>" + "" + "</input>";
                            props.Add("prop_landing");
                            //propIndex++;
                        }
                        var tofuel = ln.Split(new string[] { "TAKE OFF FUEL:" }, StringSplitOptions.None).ToList();
                        if (tofuel.Count() > 1)
                        {
                            ln = tofuel[0] + "TAKE OFF FUEL: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_tofuel" + "'>" + "" + "</input>";
                            props.Add("prop_tofuel");
                            //propIndex++;
                        }
                        var reqfuel = ln.Split(new string[] { "REQUESTED" }, StringSplitOptions.None).ToList();
                        if (reqfuel.Count() > 1)
                        {
                            ln = reqfuel[0] + "REQUESTED: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop  noborder alignleft' id='prop_reqfuel" + "'>" + "" + "</input>";
                            props.Add("prop_reqfuel");
                            //propIndex++;
                        }
                        var onblock = ln.Split(new string[] { "ON BLK" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("FUEL") && onblock.Count() > 1)
                        {
                            ln = onblock[0] + "ON BLK     : " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_onblock" + "'>" + "" + "</input>";
                            props.Add("prop_onblock");
                            //propIndex++;
                        }
                        //ON BLK FUEL
                        var onblockfuel = ln.Split(new string[] { "ON BLK FUEL" }, StringSplitOptions.None).ToList();
                        if (onblockfuel.Count() > 1)
                        {
                            // ln = onblock[0] + "ON BLK FUEL: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_onblock" + "'>" + "" + "</input>";
                            // props.Add("prop_onblock");
                            ln = onblockfuel[0] + "ON BLK FUEL: " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                            props.Add("prop_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("FLIGHTDISPATCHER"))
                        {
                            try
                            {
                                var picIndex = ln.IndexOf("CAPTAIN");
                                var dispIndex = ln.IndexOf("FLIGHT DISPATCHER");
                                var dispStr = ln.Substring(0, ln.Length - (picIndex + 3));
                                var _disps = dispStr.Replace(":", " ").Split(new string[] { "FLIGHT DISPATCHER" }, StringSplitOptions.None).ToList();
                                var dispatcher = _disps[1].Replace(" ", "");

                                var picStr = ln.Substring(picIndex);
                                var _pics = picStr.Replace(":", " ").Split(new string[] { "CAPTAIN" }, StringSplitOptions.None).ToList();
                                var pic = _pics[1].Replace(" ", "").Replace(".", ". ");


                                ln = "<div class='z5000 h70'> " + "<span  id='sig_disp' data-info='_null_' class='sig'><span  class='sig_name'>FLIGHT DISPATCHER : " + dispatcher + "</span><img id='sig_disp_img' class='sig_img'  /></span>" + "          " + "<span type='text' id='sig_pic' data-info='_null_' class='sig left80'><span type='text' data-info='_null_' class='sig_name'>" + "CAPTAIN : " + pic + "</span><img id='sig_pic_img' class='sig_img' /></span></div>";

                            }
                            catch (Exception ex)
                            {

                            }


                        }

                        var sign = ln.Split(new string[] { "SIGNATURE  :" }, StringSplitOptions.None).ToList();
                        if (sign.Count() > 1)
                        {

                            ln = "";
                        }


                        if (ln.Replace(" ", "").ToUpper().Contains("OFPFILLED"))
                        {

                            ln = " OFP FILLED By CAPT: " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbcpt_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_ofbcpt_" + propIndex);
                            propIndex++;
                            ln += "      FO: " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbfo_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_ofbfo_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS1"))
                        {


                            ln = " ENR ATIS 1 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS2"))
                        {


                            ln = " ENR ATIS 2 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS3"))
                        {


                            ln = " ENR ATIS 3 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS4"))
                        {


                            ln = " ENR ATIS 4 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_" + propIndex);
                            propIndex++;
                        }



                        List<string> prts = new List<string>();
                        var dots = "";
                        bool dot8 = false;
                        var r4 = false;
                        if (ln.EndsWith(".... .... "))
                        {
                            ln = ln + ".";
                            r4 = true;
                        }

                        var prtsgrnd = ln.Split(new string[] { "****" }, StringSplitOptions.None).ToList();
                        if (prtsgrnd.Count > 1)
                        {
                            var str = "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "' style='width:606px'>" + "" + "</input>"; // "@prop_" + propIndex;
                            props.Add("prop_" + propIndex);
                            propIndex++;
                            ln = str;
                        }
                        var prts4 = ln.Split(new string[] { "...." }, StringSplitOptions.None).ToList();

                        if (prts4.Count > 1)
                        {

                            var chrs = ln.ToCharArray();
                            var str = "";
                            int _xd = 0;
                            foreach (var x in prts4)
                            {
                                if (_xd == 0 && r4)
                                {
                                    str += x.Substring(0, x.Length - 5);
                                }
                                else
                                    str += x;

                                if (x != prts4.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    var style = "";
                                    var xtraclass = "";
                                    var actfuel = "";
                                    if (ln.Contains("N") && ln.Contains(" E") && ln.Contains("/"))
                                        actfuel += " actfuel";
                                    if (ln.Replace(" ", "").ToUpper().Contains("XTR"))
                                    { style = "-"; xtraclass = "due"; }
                                    if (ln.Replace(" ", "").ToUpper().Contains("REQUESTED"))
                                    { xtraclass = "requestedfuel"; }
                                    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop " + xtraclass + actfuel + "' id='prop_" + propIndex + "' " + (string.IsNullOrEmpty(style) ? "" : "style='width:250px !important'") + ">" + "" + "</input>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }
                                //else if (ln.EndsWith(".... .... "))
                                //{
                                //    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                //    props.Add("prop_" + propIndex);
                                //    propIndex++;

                                //}
                                _xd++;
                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }


                        var prtsx = ln.Split(new string[] { "xxxx" }, StringSplitOptions.None).ToList();

                        if (prtsx.Count > 1)
                        {

                            var str = prtsx[0];
                            str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop " + "diff noborder" + "' data-fuel='" + prtsx[1] + "' id='prop_" + propIndex + "' " + "" + ">" + "" + "</input>"; // "@prop_" + propIndex;
                            props.Add("prop_" + propIndex);
                            propIndex++;


                            ln = str;
                        }

                        var prts8 = ln.Split(new string[] { "........" }, StringSplitOptions.None).ToList();
                        if (prts8.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts8)
                            {
                                str += x;
                                if (x != prts8.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }
                        var prts7 = ln.Split(new string[] { "......." }, StringSplitOptions.None).ToList();
                        if (prts7.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts7)
                            {
                                str += x;
                                if (x != prts7.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }


                        linesProccessed.Add(ln);
                    }

                }


                var finalResult = new List<string>();
                //finalResult.Add("<pre>");
                var proOn = false;
                var _f = 0;
                for (int i = 0; i < linesProccessed.Count(); i++)
                {

                    var pln = linesProccessed[i];
                    if (pln.ToUpper().Contains("SCHD DEP"))
                        proOn = true;
                    if (pln.ToUpper().Contains("SCHD ARR"))
                        proOn = false;

                    if (proOn)
                    {
                        if (pln.Contains("-- -- -- --"))
                        {
                            _f++;
                            if (_f > 1)
                            {
                                var next = linesProccessed[i + 1];
                                var spanIndex = next.IndexOf("<input type='text'");
                                var str = next.Substring(0, spanIndex).Trim();
                                var prts = str.Split(' ').ToList();
                                var timeStr = prts[prts.Count() - 2].Split('.');
                                var mins = Convert.ToInt32(timeStr[0]) * 60 + Convert.ToInt32(timeStr[1]);
                                var infoIndex = next.IndexOf("_null_");
                                var fin = next.Substring(0, infoIndex) + "time_" + i + "_" + mins + next.Substring(infoIndex + 6);
                                linesProccessed[i + 1] = fin;

                            }
                        }

                    }
                }
                foreach (var x in linesProccessed)
                //finalResult.Add("<div>" + /*Regex.Replace(x, @"\s+", "&nbsp;")*/ReplaceWhitespace(x, "&nbsp;") + " </div>") ;
                {
                    finalResult.Add(x);
                    plan.OFPImportItems.Add(new OFPImportItem()
                    {
                        Line = x
                    });
                }
                var _text = "<pre>" + string.Join("<br/>", finalResult) + "</pre>";
                plan.TextOutput = _text;
                var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                foreach (var p in props)
                {
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        DateUpdate = dtupd,
                        PropName = p,
                        PropValue = "",
                        User = user,

                    });
                }
                plan.FPFuel = Convert.ToDecimal(fuelTotal);
                plan.FPTripFuel = Convert.ToDecimal(fuelTrip);

                flightObj.FPFuel = Convert.ToDecimal(fuelTotal);
                flightObj.FPTripFuel = Convert.ToDecimal(fuelTrip);

                context.SaveChanges();
                //context.SaveChangesAsync();
                //finalResult.Add("</pre>");
                return Ok(_text);
            }
            catch (Exception ex)
            {
                return Ok("-1");

            }

        }


        [Route("api/ofp/parse/text/input/varesh/airbus")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostImportOFPInputVareshAIRBUS(dynamic dto)
        {
            //kool
            try
            {
                string user = Convert.ToString(dto.user);
                int fltId = Convert.ToInt32(dto.fltId);
                //var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
                var context = new AirpocketAPI.Models.FLYEntities();

                var flight = context.ViewLegTimes.FirstOrDefault(q => q.ID == fltId);
                var flightObj = context.FlightInformations.FirstOrDefault(q => q.ID == fltId);
                flightObj.ALT1 = "";
                flightObj.ALT2 = "";


                string ftext = Convert.ToString(dto.text);
                ftext = ftext.Replace("DISPATCHER: ....", "DISPATCHER: ");
                ftext = ftext.Replace("CAPTAIN: ....", "CAPTAIN: ");

                var __lines = ftext.Split(new[] { '\r', '\n' }).ToList();
                var __newlines = new List<string>();
                var __c = 0;
                foreach (var str in __lines)
                {
                    if (str.Contains("TAXI") || str.Contains("DEST")
                        || str.Contains("CONT") || str.Contains("ALTN")
                        || str.Contains("FINL") || str.Contains("ADDI")
                        || str.Contains("REQD") || str.Contains("XTRA")
                        || str.Contains("TOTL")
                        )
                        __newlines.Add(str.Replace(".......", "       "));
                    else

                    if (str.Contains("ENG FAIL PROC"))
                        __newlines.Add("ENG FAIL PROC:****");
                    //:  CHECK    :TIME(UTC) :   ALT 1  :   ALT 2   :    SBY    :
                    else if (str.Contains(": GROUND    :    H    Z:          :           :           :"))
                        __newlines.Add(": GROUND    :$gndtime:$gnd1:$gnd2:$gndsby:");
                    else if (str.Contains(":BEFORE RVSM:    H    Z:          :           :           :"))
                        __newlines.Add(":BEFORE RVSM:$rvsmtime:$rvsm1:$rvsm2:$rvsmsby:");
                    else if (str.Contains(":   TOC     :    H    Z:          :           :           :"))
                        __newlines.Add(":   TOC     :$toctime:$toc1:$toc2:$tocsby:");
                    else if (str.Contains(":   01H00   :    H    Z:          :           :           :"))
                        __newlines.Add(":   01H00   :$01time:$011:$012:$01sby:");
                    else if (str.Contains(":   02H00   :    H    Z:          :           :           :"))
                        __newlines.Add(":   02H00   :$02time:$021:$022:$02sby:");
                    else if (str.Contains(":   03H00   :    H    Z:          :           :           :"))
                        __newlines.Add(":   03H00   :$03time:$031:$032:$03sby:");
                    else if (str.Contains(":   04H00   :    H    Z:          :           :           :"))
                        __newlines.Add(":   04H00   :$04time:$041:$042:$04sby:");
                    else if (str.Contains(":   05H00   :    H    Z:          :           :           :"))
                        __newlines.Add(":   05H00   :$05time:$051:$052:$05sby:");
                    else if (str.Contains(" ATIS "))
                    {
                        __newlines.Add(str);
                        __newlines.Add("****.");
                    }

                    else
                        __newlines.Add(str);

                    __c++;
                }

                ftext = string.Join("\r", __newlines);
                ftext = ftext.Replace("**************************************", "======================================");
                ftext = ftext.Replace("********************", "====================");
                ftext = ftext.Replace("********************", "====================");


                ftext = ftext.Replace("* ", "= ");
                ftext = ftext.Replace(" *", " =");
                ftext = ftext.Replace("..............", "....");
                ftext = ftext.Replace(".............", "....");
                ftext = ftext.Replace("............", "....");
                ftext = ftext.Replace("...........", "....");
                ftext = ftext.Replace("..........", "....");
                ftext = ftext.Replace("........", "....");
                ftext = ftext.Replace(".......", "....");
                ftext = ftext.Replace("/...", "/....");


                //ftext = ftext.Replace("Ground ..... ..... ..... .....", "Ground");
                //ftext = ftext.Replace("..... ..... ..... ..... .....", "****");
                ftext = ftext.Replace(".....", "....");


                ftext = ftext.Replace(".../.../...", "..../..../....");


                var lines = ftext.Split(new[] { '\r', '\n' }).ToList();
                // lines.Add("ENRATIS1");
                // lines.Add("ENRATIS2");
                // lines.Add("ENRATIS3");
                // lines.Add("ENRATIS4");
                var linesNoSpace = lines.Select(q => q.Replace(" ", "")).ToList();
                var linesTrimStart = lines.Select(q => q.TrimStart()).ToList();



                var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == fltId);
                if (cplan != null)
                    context.OFPImports.Remove(cplan);
                List<string> props = new List<string>();
                var plan = new OFPImport()
                {
                    DateCreate = DateTime.Now,
                    DateFlight = flight.STDDay,
                    FileName = "",
                    FlightNo = flight.FlightNumber,
                    Origin = flight.FromAirportICAO,
                    Destination = flight.ToAirportICAO,
                    User = user,
                    Text = ftext,


                };
                if (flight != null)
                    plan.FlightId = flight.ID;
                context.OFPImports.Add(plan);



                var propIndex = 1;
                var linesProccessed = new List<string>();
                double? fuelTrip = null;
                double? fuelTotal = null;

                var _olines = new List<string>();
                var cln = 0;
                //foreach(var ln in lines)
                while (cln < lines.Count)
                {
                    var ln = lines[cln];
                    //10-17
                    if (ln.Contains("RVSM ALTIMETER CHECKS"))
                    {
                        _olines.Add(ln);

                        _olines.Add("PHASE          CPT              STBY              FO               TIME");
                        _olines.Add("                      ");

                        _olines.Add("Ground         $cptgnd      $stbygnd      $fognd      $timegnd");
                        _olines.Add("                      ");
                        _olines.Add("Flight         $cptflt28      $stbyflt28      $foflt28      $timeflt28");
                        _olines.Add("                      ");
                        _olines.Add("Flight         $cptfltfl      $stbyfltfl      $fofltfl      $timefltfl");
                        _olines.Add("                      ");
                        cln = cln + 7;

                    }
                    else if (ln.Contains("N") && ln.Contains(" E") && ln.Contains("/"))
                    {


                        try
                        {
                            _olines.Add(ln);
                            cln++;
                            var _fl = "                                                                             xxxx";
                            var _pln = lines[cln - 2];
                            if (_pln.Last() != ' ')
                                _pln += " ";
                            var _plnParts = _pln.Split(' ');
                            var _plnf = _plnParts[_plnParts.Length - 2];
                            var _fvalue = Convert.ToDouble(_plnf);
                            _fl += _fvalue.ToString();
                            _olines.Add(_fl);
                            // cln++;

                        }
                        catch (Exception _ex2)
                        {
                            _olines.Add(ln);
                            cln++;
                        }

                    }
                    else
                    {
                        _olines.Add(ln);
                        if (ln.Replace(" ", "").ToUpper().Contains("TOTAL"))
                        {
                            _olines.Add(" REQUESTED ");
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("TAKEOFF:"))
                        {

                            _olines.Add("                                                 TAKE OFF FUEL: ");
                        }

                        cln++;
                    }


                }

                lines = _olines.ToList();


                for (int cnt = 0; cnt < lines.Count; cnt++)
                {
                    var ln = lines[cnt];
                    var preLnFuel = "";

                    if (ln.Contains("N") && ln.Contains(" E") && ln.Contains("/"))
                    {
                        // ln += "....  ....";
                        //"                        ....  ....       ";
                        ln = ln.TrimEnd(new Char[] { ' ' });
                        //ln+= "                        ....  ....       ";
                        ln += "                                                 ....  ";
                        //ln += "                                                 xxxx  ";

                    }
                    try
                    {
                        if (ln.Contains("$rvsmtime"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$rvsm1", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop ' id='prop_rvsm1'></input>");
                            props.Add("prop_rvsm1");

                            str = str.Replace("$rvsm2", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop ' id='prop_rvsm2'></input>");
                            props.Add("prop_rvsm2");

                            str = str.Replace("$rvsmsby", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop ' id='prop_rvsmsby'></input>");
                            props.Add("prop_rvsmsby");

                            str = str.Replace("$rvsmtime", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_rvsmtime'></input>");
                            props.Add("prop_rvsmtime");
                            ln = str;


                        }
                        if (ln.Contains("$gndtime"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$gnd1", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop ' id='prop_gnd1'></input>");
                            props.Add("prop_gnd1");

                            str = str.Replace("$gnd2", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop ' id='prop_gnd2'></input>");
                            props.Add("prop_gnd2");

                            str = str.Replace("$gndsby", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop ' id='prop_gndsby'></input>");
                            props.Add("prop_gndsby");

                            str = str.Replace("$gndtime", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_gndtime'></input>");
                            props.Add("prop_gndtime");
                            ln = str;


                        }
                        if (ln.Contains("$toctime"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$toc1", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_toc1'></input>");
                            props.Add("prop_toc1");

                            str = str.Replace("$toc2", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_toc2'></input>");
                            props.Add("prop_toc2");

                            str = str.Replace("$tocsby", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_tocsby'></input>");
                            props.Add("prop_tocsby");

                            str = str.Replace("$toctime", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_toctime'></input>");
                            props.Add("prop_toctime");

                            ln = str;
                        }
                        if (ln.Contains("$01time"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$011", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_011'></input>");
                            props.Add("prop_011");

                            str = str.Replace("$012", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_012'></input>");
                            props.Add("prop_012");

                            str = str.Replace("$01sby", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_01sby'></input>");
                            props.Add("prop_01sby");

                            str = str.Replace("$01time", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_01time'></input>");
                            props.Add("prop_01time");

                            ln = str;
                        }
                        if (ln.Contains("$02time"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$021", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_021'></input>");
                            props.Add("prop_021");

                            str = str.Replace("$022", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_022'></input>");
                            props.Add("prop_022");

                            str = str.Replace("$02sby", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_02sby'></input>");
                            props.Add("prop_02sby");

                            str = str.Replace("$02time", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_02time'></input>");
                            props.Add("prop_02time");

                            ln = str;
                        }
                        if (ln.Contains("$03time"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$031", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_031'></input>");
                            props.Add("prop_031");

                            str = str.Replace("$032", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_032'></input>");
                            props.Add("prop_032");

                            str = str.Replace("$03sby", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_03sby'></input>");
                            props.Add("prop_03sby");

                            str = str.Replace("$03time", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_03time'></input>");
                            props.Add("prop_03time");

                            ln = str;
                        }
                        if (ln.Contains("$04time"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$041", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_041'></input>");
                            props.Add("prop_041");

                            str = str.Replace("$042", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_042'></input>");
                            props.Add("prop_042");

                            str = str.Replace("$04sby", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_04sby'></input>");
                            props.Add("prop_04sby");

                            str = str.Replace("$04time", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_04time'></input>");
                            props.Add("prop_04time");

                            ln = str;
                        }

                        if (ln.Contains("$05time"))
                        {
                            var str = ln;
                            //$cptgnd      $stbygnd      $fognd      $timegnd
                            str = str.Replace("$051", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_051'></input>");
                            props.Add("prop_051");

                            str = str.Replace("$052", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_052'></input>");
                            props.Add("prop_052");

                            str = str.Replace("$05sby", "<input type='number' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_05sby'></input>");
                            props.Add("prop_05sby");

                            str = str.Replace("$05time", "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_05time'></input>");
                            props.Add("prop_05time");

                            ln = str;
                        }
                        if (ln/*.Replace(" ", "")*/.ToUpper().Contains("DEST"))
                        {
                            //ln.Split(new string[] { "TAKE OFF" }, StringSplitOptions.None)
                            var _sprts = ln.Split(' ');
                            var _nos = new List<double>();
                            foreach (var _p in _sprts)
                            {
                                double _n = -1;
                                bool isNumeric = double.TryParse(_p, out _n);
                                if (isNumeric) _nos.Add(_n);

                            }
                            if (_nos.Count > 0)
                                fuelTrip = _nos.First();

                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("TOTL"))
                        {
                            var _sprts = ln.Split(' ');
                            var _nos = new List<double>();
                            foreach (var _p in _sprts)
                            {
                                double _n = -1;
                                bool isNumeric = double.TryParse(_p, out _n);
                                if (isNumeric) _nos.Add(_n);

                            }
                            if (_nos.Count > 0)
                                fuelTotal = _nos.First();

                        }
                    }
                    catch (Exception fexp)
                    {

                    }
                    if (ln.Replace(" ", "").ToUpper().Contains("CREW"))
                    {
                        var crewLine = " CREW  : ";
                        crewLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        props.Add("prop_" + propIndex);
                        propIndex++;

                        linesProccessed.Add(crewLine);
                        linesProccessed.Add("<br/>");

                        var paxLine = " PAX   : ";
                        paxLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_adult'>" + "" + "</input>";
                        paxLine += "/";
                        props.Add("prop_pax_adult");
                        paxLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_child'>" + "" + "</input>";
                        paxLine += "/";
                        props.Add("prop_pax_child");
                        paxLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_infant'>" + "" + "</input>";
                        props.Add("prop_pax_infant");
                        linesProccessed.Add(paxLine);
                        linesProccessed.Add("<br/>");

                        var sobLine = " S.O.B : ";
                        sobLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        linesProccessed.Add(sobLine);

                        ln = " ";


                    }
                    else
                    {
                        var alt1 = ln.Split(new string[] { "ALTERNATE - 1" }, StringSplitOptions.None).ToList();
                        if (alt1.Count() > 1 && alt1[1].Length > 20)
                        {
                            if (!string.IsNullOrEmpty(alt1.Last().Replace(" ", "").Trim()))
                            {
                                var _alt1 = alt1.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT1 = _alt1;
                            }


                        }
                        var alt2 = ln.Split(new string[] { "ALTERNATE - 2" }, StringSplitOptions.None).ToList();
                        if (alt2.Count() > 1)
                        {
                            if (!string.IsNullOrEmpty(alt2.Last().Replace(" ", "").Trim()))
                            {
                                var _alt2 = alt2.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT2 = _alt2;
                            }


                        }
                        var clr = ln.Split(new string[] { "CLEARANCE" }, StringSplitOptions.None).ToList();
                        if (clr.Count() > 1)
                        {
                            ln = clr[0] + "CLEARANCE: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        var offblk = ln.Split(new string[] { "OFF BLK" }, StringSplitOptions.None).ToList();
                        if (offblk.Count() > 1)
                        {
                            ln = offblk[0] + "OFF BLK : " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_offblock" + "'>" + "" + "</input>";
                            props.Add("prop_offblock");
                            //propIndex++;
                        }

                        var takeoff = ln.Split(new string[] { "TAKE OFF:" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("ALTN") && takeoff.Count() > 1)
                        {
                            ln = takeoff[0] + "TAKE OFF: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_takeoff" + "'>" + "" + "</input>";
                            props.Add("prop_takeoff");
                            //propIndex++;
                        }
                        var lnd = ln.Split(new string[] { "ON RUNWAY" }, StringSplitOptions.None).ToList();
                        if (lnd.Count() > 1)
                        {
                            ln = lnd[0] + "ON RUNWAY  : " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_landing" + "'>" + "" + "</input>";
                            props.Add("prop_landing");
                            //propIndex++;
                        }
                        var tofuel = ln.Split(new string[] { "TAKE OFF FUEL:" }, StringSplitOptions.None).ToList();
                        if (tofuel.Count() > 1)
                        {
                            ln = tofuel[0] + "TAKE OFF FUEL: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_tofuel" + "'>" + "" + "</input>";
                            props.Add("prop_tofuel");
                            //propIndex++;
                        }
                        //var reqfuel = ln.Split(new string[] { "REQUESTED" }, StringSplitOptions.None).ToList();
                        // if (reqfuel.Count() > 1)
                        // {
                        //    ln = reqfuel[0] + "REQUESTED: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop  noborder alignleft' id='prop_reqfuel" + "'>" + "" + "</input>";
                        //    props.Add("prop_reqfuel");
                        //propIndex++;
                        // }
                        var onblock = ln.Split(new string[] { "ON BLK" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("FUEL") && onblock.Count() > 1)
                        {
                            ln = onblock[0] + "ON BLK     : " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_onblock" + "'>" + "" + "</input>";
                            props.Add("prop_onblock");
                            //propIndex++;
                        }
                        //ON BLK FUEL
                        var onblockfuel = ln.Split(new string[] { "ON BLK FUEL" }, StringSplitOptions.None).ToList();
                        if (onblockfuel.Count() > 1)
                        {
                            // ln = onblock[0] + "ON BLK FUEL: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_onblock" + "'>" + "" + "</input>";
                            // props.Add("prop_onblock");
                            ln = onblockfuel[0] + "ON BLK FUEL: " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                            props.Add("prop_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("FLIGHTDISPATCHER"))
                        {
                            try
                            {
                                var picIndex = ln.IndexOf("CAPTAIN");
                                var dispIndex = ln.IndexOf("FLIGHT DISPATCHER");
                                var dispStr = ln.Substring(0, ln.Length - (picIndex + 3));
                                var _disps = dispStr.Replace(":", " ").Split(new string[] { "FLIGHT DISPATCHER" }, StringSplitOptions.None).ToList();
                                var dispatcher = _disps[1].Replace(" ", "");

                                var picStr = ln.Substring(picIndex);
                                var _pics = picStr.Replace(":", " ").Split(new string[] { "CAPTAIN" }, StringSplitOptions.None).ToList();
                                var pic = _pics[1].Replace(" ", "").Replace(".", ". ");


                                ln = "<div class='z5000 h70'> " + "<span  id='sig_disp' data-info='_null_' class='sig'><span  class='sig_name'>FLIGHT DISPATCHER : " + dispatcher + "</span><img id='sig_disp_img' class='sig_img'  /></span>" + "          " + "<span type='text' id='sig_pic' data-info='_null_' class='sig left80'><span type='text' data-info='_null_' class='sig_name'>" + "CAPTAIN : " + pic + "</span><img id='sig_pic_img' class='sig_img' /></span></div>";

                            }
                            catch (Exception ex)
                            {

                            }


                        }

                        var sign = ln.Split(new string[] { "SIGNATURE  :" }, StringSplitOptions.None).ToList();
                        if (sign.Count() > 1)
                        {

                            ln = "";
                        }


                        if (ln.Replace(" ", "").ToUpper().Contains("OFPFILLED"))
                        {

                            ln = " OFP FILLED By CAPT: " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbcpt_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_ofbcpt_" + propIndex);
                            propIndex++;
                            ln += "      FO: " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbfo_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_ofbfo_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS1"))
                        {


                            ln = " ENR ATIS 1 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS2"))
                        {


                            ln = " ENR ATIS 2 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS3"))
                        {


                            ln = " ENR ATIS 3 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS4"))
                        {


                            ln = " ENR ATIS 4 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_" + propIndex);
                            propIndex++;
                        }



                        List<string> prts = new List<string>();
                        var dots = "";
                        bool dot8 = false;
                        var r4 = false;
                        if (ln.EndsWith(".... .... "))
                        {
                            ln = ln + ".";
                            r4 = true;
                        }

                        var prtsgrnd = ln.Split(new string[] { "****" }, StringSplitOptions.None).ToList();
                        if (prtsgrnd.Count > 1)
                        {
                            if (prtsgrnd[0].Contains("ENG FAIL"))
                            {
                                var str = "ENG FAIL PROC: <input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "' style='width:606px'>" + "" + "</input>"; // "@prop_" + propIndex;
                                props.Add("prop_" + propIndex);
                                propIndex++;
                                ln = str;
                            }
                            else
                            {
                                var str = "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "' style='width:606px'>" + "" + "</input>"; // "@prop_" + propIndex;
                                props.Add("prop_" + propIndex);
                                propIndex++;
                                ln = str;
                            }
                        }
                        var prts4 = ln.Split(new string[] { "...." }, StringSplitOptions.None).ToList();

                        if (prts4.Count > 1)
                        {

                            var chrs = ln.ToCharArray();
                            var str = "";
                            int _xd = 0;
                            foreach (var x in prts4)
                            {
                                if (_xd == 0 && r4)
                                {
                                    str += x.Substring(0, x.Length - 5);
                                }
                                else
                                    str += x;

                                if (x != prts4.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    var style = "";
                                    var xtraclass = "";
                                    var actfuel = "";
                                    if (ln.Contains("N") && ln.Contains(" E") && ln.Contains("/"))
                                        actfuel += " actfuel";
                                    if (ln.Replace(" ", "").ToUpper().Contains("XTR"))
                                    { style = "-"; xtraclass = "due"; }
                                    if (ln.Replace(" ", "").ToUpper().Contains("REQUESTED"))
                                    { xtraclass = "requestedfuel"; }
                                    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop " + xtraclass + actfuel + "' id='prop_" + propIndex + "' " + (string.IsNullOrEmpty(style) ? "" : "style='width:250px !important'") + ">" + "" + "</input>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }
                                //else if (ln.EndsWith(".... .... "))
                                //{
                                //    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                //    props.Add("prop_" + propIndex);
                                //    propIndex++;

                                //}
                                _xd++;
                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }


                        var prtsx = ln.Split(new string[] { "xxxx" }, StringSplitOptions.None).ToList();

                        if (prtsx.Count > 1)
                        {

                            var str = prtsx[0];
                            str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop " + "diff noborder" + "' data-fuel='" + prtsx[1] + "' id='prop_" + propIndex + "' " + "" + ">" + "" + "</input>"; // "@prop_" + propIndex;
                            props.Add("prop_" + propIndex);
                            propIndex++;


                            ln = str;
                        }

                        var prts8 = ln.Split(new string[] { "........" }, StringSplitOptions.None).ToList();
                        if (prts8.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts8)
                            {
                                str += x;
                                if (x != prts8.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }
                        var prts7 = ln.Split(new string[] { "......." }, StringSplitOptions.None).ToList();
                        if (prts7.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts7)
                            {
                                str += x;
                                if (x != prts7.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }


                        linesProccessed.Add(ln);
                    }

                }


                var finalResult = new List<string>();
                //finalResult.Add("<pre>");
                var proOn = false;
                var _f = 0;
                for (int i = 0; i < linesProccessed.Count(); i++)
                {

                    var pln = linesProccessed[i];
                    if (pln.ToUpper().Contains("SCHD DEP"))
                        proOn = true;
                    if (pln.ToUpper().Contains("SCHD ARR"))
                        proOn = false;

                    if (proOn)
                    {
                        if (pln.Contains("-- -- -- --"))
                        {
                            _f++;
                            if (_f > 1)
                            {
                                var next = linesProccessed[i + 1];
                                var spanIndex = next.IndexOf("<input type='text'");
                                var str = next.Substring(0, spanIndex).Trim();
                                var prts = str.Split(' ').ToList();
                                var timeStr = prts[prts.Count() - 2].Split('.');
                                var mins = Convert.ToInt32(timeStr[0]) * 60 + Convert.ToInt32(timeStr[1]);
                                var infoIndex = next.IndexOf("_null_");
                                var fin = next.Substring(0, infoIndex) + "time_" + i + "_" + mins + next.Substring(infoIndex + 6);
                                linesProccessed[i + 1] = fin;

                            }
                        }

                    }
                }
                foreach (var x in linesProccessed)
                //finalResult.Add("<div>" + /*Regex.Replace(x, @"\s+", "&nbsp;")*/ReplaceWhitespace(x, "&nbsp;") + " </div>") ;
                {
                    finalResult.Add(x);
                    plan.OFPImportItems.Add(new OFPImportItem()
                    {
                        Line = x
                    });
                }
                var _text = "<pre>" + string.Join("<br/>", finalResult) + "</pre>";
                plan.TextOutput = _text;
                var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                foreach (var p in props)
                {
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        DateUpdate = dtupd,
                        PropName = p,
                        PropValue = "",
                        User = user,

                    });
                }
                plan.FPFuel = Convert.ToDecimal(fuelTotal);
                plan.FPTripFuel = Convert.ToDecimal(fuelTrip);

                flightObj.FPFuel = Convert.ToDecimal(fuelTotal);
                flightObj.FPTripFuel = Convert.ToDecimal(fuelTrip);

                context.SaveChanges();
                //context.SaveChangesAsync();
                //finalResult.Add("</pre>");
                return Ok(_text);
            }
            catch (Exception ex)
            {
                return Ok("-1");

            }

        }

        [Route("api/ofp/parse/text/input")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostImportOFPInput(dynamic dto)
        {
            //kool
            try
            {
                string user = Convert.ToString(dto.user);
                int fltId = Convert.ToInt32(dto.fltId);
                //var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
                var context = new AirpocketAPI.Models.FLYEntities();

                var flight = context.ViewLegTimes.FirstOrDefault(q => q.ID == fltId);
                var flightObj = context.FlightInformations.FirstOrDefault(q => q.ID == fltId);
                flightObj.ALT1 = "";
                flightObj.ALT2 = "";


                string ftext = Convert.ToString(dto.text);
                ftext = ftext.Replace("..............", "....");
                ftext = ftext.Replace(".............", "....");
                ftext = ftext.Replace("............", "....");
                ftext = ftext.Replace("...........", "....");
                ftext = ftext.Replace("........", "....");
                ftext = ftext.Replace(".......", "....");


                ftext = ftext.Replace(".../.../...", "..../..../....");


                var lines = ftext.Split(new[] { '\r', '\n' }).ToList();
                var linesNoSpace = lines.Select(q => q.Replace(" ", "")).ToList();
                var linesTrimStart = lines.Select(q => q.TrimStart()).ToList();



                var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == fltId);
                if (cplan != null)
                    context.OFPImports.Remove(cplan);
                List<string> props = new List<string>();
                var plan = new OFPImport()
                {
                    DateCreate = DateTime.Now,
                    DateFlight = flight.STDDay,
                    FileName = "",
                    FlightNo = flight.FlightNumber,
                    Origin = flight.FromAirportICAO,
                    Destination = flight.ToAirportICAO,
                    User = user,
                    Text = ftext,


                };
                if (flight != null)
                    plan.FlightId = flight.ID;
                context.OFPImports.Add(plan);



                var propIndex = 1;
                var linesProccessed = new List<string>();
                double? fuelTrip = null;
                double? fuelTotal = null;

                for (int cnt = 0; cnt < lines.Count; cnt++)
                {
                    var ln = lines[cnt];
                    try
                    {
                        if (ln.Replace(" ", "").ToUpper().Contains("TRIP"))
                        {
                            //ln.Split(new string[] { "TAKE OFF" }, StringSplitOptions.None)
                            var _sprts = ln.Split(' ');
                            var _nos = new List<double>();
                            foreach (var _p in _sprts)
                            {
                                double _n = -1;
                                bool isNumeric = double.TryParse(_p, out _n);
                                if (isNumeric) _nos.Add(_n);

                            }
                            if (_nos.Count > 0)
                                fuelTrip = _nos.First();

                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("TOTAL"))
                        {
                            var _sprts = ln.Split(' ');
                            var _nos = new List<double>();
                            foreach (var _p in _sprts)
                            {
                                double _n = -1;
                                bool isNumeric = double.TryParse(_p, out _n);
                                if (isNumeric) _nos.Add(_n);

                            }
                            if (_nos.Count > 0)
                                fuelTotal = _nos.First();

                        }
                    }
                    catch (Exception fexp)
                    {

                    }
                    if (ln.Replace(" ", "").ToUpper().Contains("CREW"))
                    {
                        var crewLine = " CREW  : ";
                        crewLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        props.Add("prop_" + propIndex);
                        propIndex++;

                        linesProccessed.Add(crewLine);
                        linesProccessed.Add("<br/>");

                        var paxLine = " PAX   : ";
                        paxLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_adult'>" + "" + "</input>";
                        paxLine += "/";
                        props.Add("prop_pax_adult");
                        paxLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_child'>" + "" + "</input>";
                        paxLine += "/";
                        props.Add("prop_pax_child");
                        paxLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_infant'>" + "" + "</input>";
                        props.Add("prop_pax_infant");
                        linesProccessed.Add(paxLine);
                        linesProccessed.Add("<br/>");

                        var sobLine = " S.O.B : ";
                        sobLine += "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        linesProccessed.Add(sobLine);

                        ln = " ";


                    }
                    else
                    {
                        var alt1 = ln.Split(new string[] { "1ST ALT" }, StringSplitOptions.None).ToList();
                        if (alt1.Count() > 1)
                        {
                            if (!string.IsNullOrEmpty(alt1.Last().Replace(" ", "").Trim()))
                            {
                                var _alt1 = alt1.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT1 = _alt1;
                            }


                        }
                        var alt2 = ln.Split(new string[] { "2ND ALT" }, StringSplitOptions.None).ToList();
                        if (alt2.Count() > 1)
                        {
                            if (!string.IsNullOrEmpty(alt2.Last().Replace(" ", "").Trim()))
                            {
                                var _alt2 = alt2.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT2 = _alt2;
                            }


                        }
                        var clr = ln.Split(new string[] { "ATC CLRNC:" }, StringSplitOptions.None).ToList();
                        if (clr.Count() > 1)
                        {
                            ln = clr[0] + "ATC CLRNC: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        var offblk = ln.Split(new string[] { "OFF BLK" }, StringSplitOptions.None).ToList();
                        if (offblk.Count() > 1)
                        {
                            ln = offblk[0] + "OFF BLK : " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_offblock" + "'>" + "" + "</input>";
                            props.Add("prop_offblock");
                            //propIndex++;
                        }

                        var takeoff = ln.Split(new string[] { "TAKE OFF" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("ALTN") && takeoff.Count() > 1)
                        {
                            ln = takeoff[0] + "TAKE OFF: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_takeoff" + "'>" + "" + "</input>";
                            props.Add("prop_takeoff");
                            //propIndex++;
                        }
                        var lnd = ln.Split(new string[] { "ON RUNWAY" }, StringSplitOptions.None).ToList();
                        if (lnd.Count() > 1)
                        {
                            ln = lnd[0] + "ON RUNWAY  : " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_landing" + "'>" + "" + "</input>";
                            props.Add("prop_landing");
                            //propIndex++;
                        }
                        var onblock = ln.Split(new string[] { "ON BLK" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("FUEL") && onblock.Count() > 1)
                        {
                            ln = onblock[0] + "ON BLK     : " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_onblock" + "'>" + "" + "</input>";
                            props.Add("prop_onblock");
                            //propIndex++;
                        }
                        //ON BLK FUEL
                        var onblockfuel = ln.Split(new string[] { "ON BLK FUEL" }, StringSplitOptions.None).ToList();
                        if (onblockfuel.Count() > 1)
                        {
                            // ln = onblock[0] + "ON BLK FUEL: " + "<input type='text' ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_onblock" + "'>" + "" + "</input>";
                            // props.Add("prop_onblock");
                            ln = onblockfuel[0] + "ON BLK FUEL: " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                            props.Add("prop_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("DISP"))
                        {
                            var picIndex = ln.IndexOf("PIC");
                            var dispIndex = ln.IndexOf("DISP");
                            var dispStr = ln.Substring(0, ln.Length - (picIndex + 3));
                            var _disps = dispStr.Replace(":", " ").Split(new string[] { "DISP" }, StringSplitOptions.None).ToList();
                            var dispatcher = _disps[1].Replace(" ", "");

                            var picStr = ln.Substring(picIndex);
                            var _pics = picStr.Replace(":", " ").Split(new string[] { "PIC" }, StringSplitOptions.None).ToList();
                            var pic = _pics[1].Replace(" ", "").Replace(".", ". ");


                            ln = "<div class='z5000 h70'> " + "<span  id='sig_disp' data-info='_null_' class='sig'><span  class='sig_name'>DISP : " + dispatcher + "</span><img id='sig_disp_img' class='sig_img' /></span>" + "          " + "<span type='text' id='sig_pic' data-info='_null_' class='sig left80'><span type='text' data-info='_null_' class='sig_name'>" + "PIC : " + pic + "</span><img id='sig_pic_img' class='sig_img' /></span></div>";

                        }

                        var sign = ln.Split(new string[] { "SIGNATURE  :" }, StringSplitOptions.None).ToList();
                        if (sign.Count() > 1)
                        {

                            ln = "";
                        }


                        if (ln.Replace(" ", "").ToUpper().Contains("OFPWORKED"))
                        {

                            ln = " OFP WORKED By CAPT: " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbcpt_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_ofbcpt_" + propIndex);
                            propIndex++;
                            ln += "      FO: " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbfo_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_ofbfo_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS1"))
                        {


                            ln = " ENR ATIS 1 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS2"))
                        {


                            ln = " ENR ATIS 2 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS3"))
                        {


                            ln = " ENR ATIS 3 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS4"))
                        {


                            ln = " ENR ATIS 4 : " + "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>";
                            props.Add("prop_" + propIndex);
                            propIndex++;
                        }



                        List<string> prts = new List<string>();
                        var dots = "";
                        bool dot8 = false;
                        var r4 = false;
                        if (ln.EndsWith(".... .... "))
                        { ln = ln + "."; r4 = true; }
                        var prts4 = ln.Split(new string[] { "...." }, StringSplitOptions.None).ToList();

                        if (prts4.Count > 1)
                        {

                            var chrs = ln.ToCharArray();
                            var str = "";
                            int _xd = 0;
                            foreach (var x in prts4)
                            {
                                if (_xd == 0 && r4)
                                {
                                    str += x.Substring(0, x.Length - 5);
                                }
                                else
                                    str += x;

                                if (x != prts4.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }
                                //else if (ln.EndsWith(".... .... "))
                                //{
                                //    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                //    props.Add("prop_" + propIndex);
                                //    propIndex++;

                                //}
                                _xd++;
                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }

                        var prts8 = ln.Split(new string[] { "........" }, StringSplitOptions.None).ToList();
                        if (prts8.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts8)
                            {
                                str += x;
                                if (x != prts8.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }
                        var prts7 = ln.Split(new string[] { "......." }, StringSplitOptions.None).ToList();
                        if (prts7.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts7)
                            {
                                str += x;
                                if (x != prts7.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<input type='text' data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</input>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }


                        linesProccessed.Add(ln);
                    }

                }


                var finalResult = new List<string>();
                //finalResult.Add("<pre>");
                var proOn = false;
                var _f = 0;
                for (int i = 0; i < linesProccessed.Count(); i++)
                {

                    var pln = linesProccessed[i];
                    if (pln.ToUpper().Contains("SCHD DEP"))
                        proOn = true;
                    if (pln.ToUpper().Contains("SCHD ARR"))
                        proOn = false;

                    if (proOn)
                    {
                        if (pln.Contains("-- -- -- --"))
                        {
                            _f++;
                            if (_f > 1)
                            {
                                var next = linesProccessed[i + 1];
                                var spanIndex = next.IndexOf("<input type='text'");
                                var str = next.Substring(0, spanIndex).Trim();
                                var prts = str.Split(' ').ToList();
                                var timeStr = prts[prts.Count() - 2].Split('.');
                                var mins = Convert.ToInt32(timeStr[0]) * 60 + Convert.ToInt32(timeStr[1]);
                                var infoIndex = next.IndexOf("_null_");
                                var fin = next.Substring(0, infoIndex) + "time_" + i + "_" + mins + next.Substring(infoIndex + 6);
                                linesProccessed[i + 1] = fin;

                            }
                        }

                    }
                }
                foreach (var x in linesProccessed)
                //finalResult.Add("<div>" + /*Regex.Replace(x, @"\s+", "&nbsp;")*/ReplaceWhitespace(x, "&nbsp;") + " </div>") ;
                {
                    finalResult.Add(x);
                    plan.OFPImportItems.Add(new OFPImportItem()
                    {
                        Line = x
                    });
                }
                var _text = "<pre>" + string.Join("<br/>", finalResult) + "</pre>";
                plan.TextOutput = _text;
                var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                foreach (var p in props)
                {
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        DateUpdate = dtupd,
                        PropName = p,
                        PropValue = "",
                        User = user,

                    });
                }
                plan.FPFuel = Convert.ToDecimal(fuelTotal);
                plan.FPTripFuel = Convert.ToDecimal(fuelTrip);

                flightObj.FPFuel = Convert.ToDecimal(fuelTotal);
                flightObj.FPTripFuel = Convert.ToDecimal(fuelTrip);

                context.SaveChanges();
                //context.SaveChangesAsync();
                //finalResult.Add("</pre>");
                return Ok(_text);
            }
            catch (Exception ex)
            {
                return Ok("-1");

            }

        }



        [Route("api/upd/trn")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostUpdTrn(UpdTrnDto dto)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var people = context.People.Where(q => dto.ids.Contains(q.Id)).ToList();

            foreach (var person in people)
            {
                switch (dto.type)
                {
                    //dg
                    case "DG":

                        person.DangerousGoodsExpireDate = dto.expire;
                        person.DangerousGoodsIssueDate = dto.issue;

                        break;
                    //1	SEPT-P
                    case "SEPTP":

                        person.SEPTPExpireDate = dto.expire;
                        person.SEPTPIssueDate = dto.issue;

                        break;
                    //2   SEPT - T
                    case "SEPTT":

                        person.SEPTExpireDate = dto.expire;
                        person.SEPTIssueDate = dto.issue;

                        break;
                    //4	CRM
                    case "CRM":

                        person.UpsetRecoveryTrainingExpireDate = dto.expire;
                        person.UpsetRecoveryTrainingIssueDate = dto.issue;

                        break;
                    //5	CCRM
                    case "CCRM":

                        person.CCRMExpireDate = dto.expire;
                        person.CCRMIssueDate = dto.issue;

                        break;
                    //6	SMS
                    case "SMS":

                        person.SMSExpireDate = dto.expire;
                        person.SMSIssueDate = dto.issue;

                        break;
                    //7	AV-SEC
                    case "AVSEC":

                        person.AviationSecurityExpireDate = dto.expire;
                        person.AviationSecurityIssueDate = dto.issue;

                        break;
                    //8	COLD-WX
                    case "COLDWX":

                        person.ColdWeatherOperationExpireDate = dto.expire;
                        person.ColdWeatherOperationIssueDate = dto.issue;

                        break;
                    //9	HOT-WX
                    case "HOTWX":

                        person.HotWeatherOperationExpireDate = dto.expire;
                        person.HotWeatherOperationIssueDate = dto.issue;

                        break;
                    //10	FIRSTAID
                    case "FIRSTAID":

                        person.FirstAidExpireDate = dto.expire;
                        person.FirstAidIssueDate = dto.issue;

                        break;
                    //lpc
                    case "LINE":

                        person.LineExpireDate = dto.expire;
                        person.LineIssueDate = dto.issue;

                        break;

                    //lpr
                    case "TYPEMD":

                        person.TypeMDExpireDate = dto.expire;
                        person.TypeMDIssueDate = dto.issue;
                        // person.ProficiencyCheckDateOPC = cp.DateIssue;

                        break;
                    case "TYPE737":

                        person.Type737ExpireDate = dto.expire;
                        person.Type737IssueDate = dto.issue;
                        break;
                    case "TYPEAIRBUS":

                        person.TypeAirbusExpireDate = dto.expire;
                        person.TypeAirbusIssueDate = dto.issue;

                        break;
                    //grt

                    //recurrent
                    case "RECURRENT":

                        person.RecurrentExpireDate = dto.expire;
                        person.RecurrentIssueDate = dto.issue;

                        break;
                    //fmt
                    case "FMT":

                        person.FMTExpireDate = dto.expire;
                        person.FMTIssueDate = dto.issue;

                        break;
                    default:
                        break;
                }
            }
            var result = context.SaveChanges();
            return Ok(result);
        }

        public string ReplaceWhitespace(string input, string replace)
        {
            var chrs = input.ToCharArray();
            var str = "";
            foreach (var x in chrs)
            {
                if (Char.IsWhiteSpace(x))
                    str += replace;
                else
                    str += x;
            }
            return str;
        }
        public string CallWebMethod(string url, Dictionary<string, string> dicParameters)
        {
            try
            {
                byte[] requestData = this.CreateHttpRequestData(dicParameters);
                HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.KeepAlive = false;
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.ContentLength = requestData.Length;
                httpRequest.Timeout = 30000;
                HttpWebResponse httpResponse = null;
                String response = String.Empty;

                httpRequest.GetRequestStream().Write(requestData, 0, requestData.Length);
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream baseStream = httpResponse.GetResponseStream();
                StreamReader responseStreamReader = new StreamReader(baseStream);
                response = responseStreamReader.ReadToEnd();
                responseStreamReader.Close();

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private byte[] CreateHttpRequestData(Dictionary<string, string> dic)
        {
            StringBuilder sbParameters = new StringBuilder();
            foreach (string param in dic.Keys)
            {
                sbParameters.Append(param);//key => parameter name
                sbParameters.Append('=');
                sbParameters.Append(dic[param]);//key value
                sbParameters.Append('&');
            }
            sbParameters.Remove(sbParameters.Length - 1, 1);

            UTF8Encoding encoding = new UTF8Encoding();

            return encoding.GetBytes(sbParameters.ToString());

        }

        //[Route("api/flight/status")]
        //[AcceptVerbs("POST")]
        /////<summary>
        /////Get Status Of Flight
        /////</summary>
        /////<remarks>


        /////</remarks>
        //public async Task<IHttpActionResult> PostFlightStatus(AuthInfo authInfo, string date, string no)
        //{
        //    try
        //    {
        //        if (!(authInfo.userName == "pnl.airpocket" && authInfo.password == "Pnl1234@z"))
        //            return BadRequest("Authentication Failed");

        //        no =no.PadLeft(4, '0');
        //        List<int> prts = new List<int>();
        //        try
        //        {
        //            prts = date.Split('-').Select(q => Convert.ToInt32(q)).ToList();
        //        }
        //        catch(Exception ex)
        //        {
        //            return BadRequest("Incorrect Date");
        //        }

        //        if (prts.Count != 3)
        //            return BadRequest("Incorrect Date");
        //        if (prts[0]<1300)
        //            return BadRequest("Incorrect Date (Year)");
        //        //if (prts[1] < 1 || prts[1]>12)
        //        //    return BadRequest("Incorrect Date (Month)");
        //        //if (prts[2] < 1 || prts[1] > 31)
        //        //    return BadRequest("Incorrect Date (Day)");

        //        System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
        //        var gd = (pc.ToDateTime(prts[0], prts[1], prts[2], 0, 0, 0, 0)).Date;
        //        var context = new AirpocketAPI.Models.FLYEntities();
        //        var flight = await context.ExpFlights.Where(q => q.DepartureDay == gd && q.FlightNo == no).FirstOrDefaultAsync();
        //        if (flight == null)
        //            return NotFound();
        //        var delay = (((DateTime)flight.Departure) - ((DateTime)flight.STD)).TotalMinutes;
        //        if (delay < 0)
        //            delay = 0;
        //        var result = new { 
        //          flightId=flight.Id,
        //          flightNo=flight.FlightNo,
        //          date=flight.DepartureDay,
        //          departure=flight.DepartureLocal,
        //          arrival=flight.ArrivalLocal,
        //          departureUTC=flight.Departure,
        //          arrivalUTC=flight.Arrival,
        //          status=flight.FlightStatus,
        //          statusId=flight.FlightStatusId,
        //          origin=flight.Origin,
        //          destination=flight.Destination,
        //          aircraftType=flight.AircraftType,
        //          register=flight.Register,
        //          isDelayed=delay>0,
        //          delay

        //        };
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += "    Inner Exception:" + ex.InnerException.Message;
        //        return BadRequest(msg);
        //    }







        //}


    }



    public class AuthInfo
    {
        public string userName { get; set; }
        public string password { get; set; }
    }

    public class DSPReleaseViewModel
    {
        public int? FlightId { get; set; }
        public bool? ActualWXDSP { get; set; }
        public bool? ActualWXCPT { get; set; }
        public string ActualWXDSPRemark { get; set; }
        public string ActualWXCPTRemark { get; set; }
        public bool? WXForcastDSP { get; set; }
        public bool? WXForcastCPT { get; set; }
        public string WXForcastDSPRemark { get; set; }
        public string WXForcastCPTRemark { get; set; }
        public bool? SigxWXDSP { get; set; }
        public bool? SigxWXCPT { get; set; }
        public string SigxWXDSPRemark { get; set; }
        public string SigxWXCPTRemark { get; set; }
        public bool? WindChartDSP { get; set; }
        public bool? WindChartCPT { get; set; }
        public string WindChartDSPRemark { get; set; }
        public string WindChartCPTRemark { get; set; }
        public bool? NotamDSP { get; set; }
        public bool? NotamCPT { get; set; }
        public string NotamDSPRemark { get; set; }
        public string NotamCPTRemark { get; set; }
        public bool? ComputedFligthPlanDSP { get; set; }
        public bool? ComputedFligthPlanCPT { get; set; }
        public string ComputedFligthPlanDSPRemark { get; set; }
        public string ComputedFligthPlanCPTRemark { get; set; }
        public bool? ATCFlightPlanDSP { get; set; }
        public bool? ATCFlightPlanCPT { get; set; }
        public string ATCFlightPlanDSPRemark { get; set; }
        public string ATCFlightPlanCPTRemark { get; set; }
        public bool? PermissionsDSP { get; set; }
        public bool? PermissionsCPT { get; set; }
        public string PermissionsDSPRemark { get; set; }
        public string PermissionsCPTRemark { get; set; }
        public bool? JeppesenAirwayManualDSP { get; set; }
        public bool? JeppesenAirwayManualCPT { get; set; }
        public string JeppesenAirwayManualDSPRemark { get; set; }
        public string JeppesenAirwayManualCPTRemark { get; set; }
        public bool? MinFuelRequiredDSP { get; set; }
        public bool? MinFuelRequiredCPT { get; set; }
        public decimal? MinFuelRequiredCFP { get; set; }
        public decimal? MinFuelRequiredSFP { get; set; }
        public decimal? MinFuelRequiredPilotReq { get; set; }
        public bool? GeneralDeclarationDSP { get; set; }
        public bool? GeneralDeclarationCPT { get; set; }
        public string GeneralDeclarationDSPRemark { get; set; }
        public string GeneralDeclarationCPTRemark { get; set; }
        public bool? FlightReportDSP { get; set; }
        public bool? FlightReportCPT { get; set; }
        public string FlightReportDSPRemark { get; set; }
        public string FlightReportCPTRemark { get; set; }
        public bool? TOLndCardsDSP { get; set; }
        public bool? TOLndCardsCPT { get; set; }
        public string TOLndCardsDSPRemark { get; set; }
        public string TOLndCardsCPTRemark { get; set; }
        public bool? LoadSheetDSP { get; set; }
        public bool? LoadSheetCPT { get; set; }
        public string LoadSheetDSPRemark { get; set; }
        public string LoadSheetCPTRemark { get; set; }
        public bool? FlightSafetyReportDSP { get; set; }
        public bool? FlightSafetyReportCPT { get; set; }
        public string FlightSafetyReportDSPRemark { get; set; }
        public string FlightSafetyReportCPTRemark { get; set; }
        public bool? AVSECIncidentReportDSP { get; set; }
        public bool? AVSECIncidentReportCPT { get; set; }
        public string AVSECIncidentReportDSPRemark { get; set; }
        public string AVSECIncidentReportCPTRemark { get; set; }
        public bool? OperationEngineeringDSP { get; set; }
        public bool? OperationEngineeringCPT { get; set; }
        public string OperationEngineeringDSPRemark { get; set; }
        public string OperationEngineeringCPTRemark { get; set; }
        public bool? VoyageReportDSP { get; set; }
        public bool? VoyageReportCPT { get; set; }
        public string VoyageReportDSPRemark { get; set; }
        public string VoyageReportCPTRemark { get; set; }
        public bool? PIFDSP { get; set; }
        public bool? PIFCPT { get; set; }
        public string PIFDSPRemark { get; set; }
        public string PIFCPTRemark { get; set; }
        public bool? GoodDeclarationDSP { get; set; }
        public bool? GoodDeclarationCPT { get; set; }
        public string GoodDeclarationDSPRemark { get; set; }
        public string GoodDeclarationCPTRemark { get; set; }
        public bool? IPADDSP { get; set; }
        public bool? IPADCPT { get; set; }
        public string IPADDSPRemark { get; set; }
        public string IPADCPTRemark { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public int? DispatcherId { get; set; }
        public int Id { get; set; }
        public string User { get; set; }
    }


    public class DSPReleaseViewModel2
    {
        public int? FlightId { get; set; }
        public bool? ActualWXDSP { get; set; }
        // public bool? ActualWXCPT { get; set; }
        public string ActualWXDSPRemark { get; set; }
        //public string ActualWXCPTRemark { get; set; }
        public bool? WXForcastDSP { get; set; }
        //public bool? WXForcastCPT { get; set; }
        public string WXForcastDSPRemark { get; set; }
        //public string WXForcastCPTRemark { get; set; }
        public bool? SigxWXDSP { get; set; }
        //public bool? SigxWXCPT { get; set; }
        public string SigxWXDSPRemark { get; set; }
        //public string SigxWXCPTRemark { get; set; }
        public bool? WindChartDSP { get; set; }
        //public bool? WindChartCPT { get; set; }
        public string WindChartDSPRemark { get; set; }
        //public string WindChartCPTRemark { get; set; }
        public bool? NotamDSP { get; set; }
        //public bool? NotamCPT { get; set; }
        public string NotamDSPRemark { get; set; }
        //public string NotamCPTRemark { get; set; }
        public bool? ComputedFligthPlanDSP { get; set; }
        //public bool? ComputedFligthPlanCPT { get; set; }
        public string ComputedFligthPlanDSPRemark { get; set; }
        //public string ComputedFligthPlanCPTRemark { get; set; }
        public bool? ATCFlightPlanDSP { get; set; }
        //public bool? ATCFlightPlanCPT { get; set; }
        public string ATCFlightPlanDSPRemark { get; set; }
        //public string ATCFlightPlanCPTRemark { get; set; }
        public bool? PermissionsDSP { get; set; }
        //public bool? PermissionsCPT { get; set; }
        public string PermissionsDSPRemark { get; set; }
        //public string PermissionsCPTRemark { get; set; }
        public bool? JeppesenAirwayManualDSP { get; set; }
        //public bool? JeppesenAirwayManualCPT { get; set; }
        public string JeppesenAirwayManualDSPRemark { get; set; }
        //public string JeppesenAirwayManualCPTRemark { get; set; }
        public bool? MinFuelRequiredDSP { get; set; }
        //public bool? MinFuelRequiredCPT { get; set; }
        //public decimal? MinFuelRequiredCFP { get; set; }
        //public decimal? MinFuelRequiredSFP { get; set; }
        //public decimal? MinFuelRequiredPilotReq { get; set; }
        public bool? GeneralDeclarationDSP { get; set; }
        //public bool? GeneralDeclarationCPT { get; set; }
        public string GeneralDeclarationDSPRemark { get; set; }
        //public string GeneralDeclarationCPTRemark { get; set; }
        public bool? FlightReportDSP { get; set; }
        //public bool? FlightReportCPT { get; set; }
        public string FlightReportDSPRemark { get; set; }
        //public string FlightReportCPTRemark { get; set; }
        public bool? TOLndCardsDSP { get; set; }
        //public bool? TOLndCardsCPT { get; set; }
        public string TOLndCardsDSPRemark { get; set; }
        //public string TOLndCardsCPTRemark { get; set; }
        public bool? LoadSheetDSP { get; set; }
        //public bool? LoadSheetCPT { get; set; }
        public string LoadSheetDSPRemark { get; set; }
        //public string LoadSheetCPTRemark { get; set; }
        public bool? FlightSafetyReportDSP { get; set; }
        //public bool? FlightSafetyReportCPT { get; set; }
        public string FlightSafetyReportDSPRemark { get; set; }
        //public string FlightSafetyReportCPTRemark { get; set; }
        public bool? AVSECIncidentReportDSP { get; set; }
        //public bool? AVSECIncidentReportCPT { get; set; }
        public string AVSECIncidentReportDSPRemark { get; set; }
        //public string AVSECIncidentReportCPTRemark { get; set; }
        public bool? OperationEngineeringDSP { get; set; }
        //public bool? OperationEngineeringCPT { get; set; }
        public string OperationEngineeringDSPRemark { get; set; }
        //public string OperationEngineeringCPTRemark { get; set; }
        public bool? VoyageReportDSP { get; set; }
        //public bool? VoyageReportCPT { get; set; }
        public string VoyageReportDSPRemark { get; set; }
        //public string VoyageReportCPTRemark { get; set; }
        public bool? PIFDSP { get; set; }
        //public bool? PIFCPT { get; set; }
        public string PIFDSPRemark { get; set; }
        //public string PIFCPTRemark { get; set; }
        public bool? GoodDeclarationDSP { get; set; }
        //public bool? GoodDeclarationCPT { get; set; }
        public string GoodDeclarationDSPRemark { get; set; }
        //public string GoodDeclarationCPTRemark { get; set; }
        public bool? IPADDSP { get; set; }
        //public bool? IPADCPT { get; set; }
        public string IPADDSPRemark { get; set; }
        //public string IPADCPTRemark { get; set; }
        //public DateTime? DateConfirmed { get; set; }
        public int? DispatcherId { get; set; }
        public int Id { get; set; }
        public string User { get; set; }
    }

    public class CLJLData
    {
        public int? CrewId { get; set; }
        public bool? IsPositioning { get; set; }
        public int? PositionId { get; set; }
        public string Position { get; set; }
        public string Name { get; set; }
        public int? GroupId { get; set; }
        public string JobGroup { get; set; }
        public string JobGroupCode { get; set; }
        public int? GroupOrder { get; set; }
        public int IsCockpit { get; set; }
        public string PassportNo { get; set; }

        public List<string> Legs { get; set; }
        public string LegsStr { get; set; }

        public int? FlightId { get; set; }
        public string PID { get; set; }

        public string Mobile { get; set; }
        public string Address { get; set; }
        public string NID { get; set; }

    }

    public class FixTimeDto
    {
        public List<int> Ids { get; set; }
        public int HH { get; set; }
        public int MM { get; set; }
        public int Year { get; set; }
        public int Period { get; set; }
        public int Type { get; set; }
        public string Remark { get; set; }
        public int? FlightId { get; set; }
        public int? Count { get; set; }
    }

    public class FlightChartererDto
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int ChartererId { get; set; }
        public int? Book { get; set; }
        public int? Capacity { get; set; }
        public string Remark { get; set; }
        public int? Adult { get; set; }
        public int? Child { get; set; }
        public int? Infanct { get; set; }

    }
    public class ChartererDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NiraCode { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Remark { get; set; }


    }
    // var text = "<Delay><Item No='1' DigitCode='93' LetterCode='__' Amount='00:05' Description=''/><Item No='2' DigitCode='81' LetterCode='__' Amount='00:05' Description=''/><Item No='3' DigitCode='99' LetterCode='__' Amount='00:10' Description=''/></Delay>";
    public class xmlDelayItem
    {
        public string No { get; set; }
        public string DigitCode { get; set; }
        public string LetterCode { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }

    }
    public class xmlDelayItemCol
    {
        public List<xmlDelayItem> Item { get; set; }

    }
    public class xmlDelayItemSingle
    {
        public xmlDelayItem Item { get; set; }

    }
    public class xmlDelay
    {
        public xmlDelayItemCol Delay { get; set; }
    }
    public class xmlDelaySingle
    {
        public xmlDelayItemSingle Delay { get; set; }
    }

    public class UpdTrnDto
    {
        public List<int> ids { get; set; }
        public DateTime issue { get; set; }
        public DateTime expire { get; set; }
        public string type { get; set; }
    }
}
