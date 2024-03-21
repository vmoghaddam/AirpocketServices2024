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
using System.Web.Http.Cors;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using System.IO;
using LinqToExcel;
using System.Globalization;
using System.Web.Configuration;
using System.Web;

namespace AirpocketAPI.Controllers
{

    public class ConnexionExcel
    {
        public string _pathExcelFile;
        public ExcelQueryFactory _urlConnexion;
        public ConnexionExcel(string path)
        {
            this._pathExcelFile = path;
            this._urlConnexion = new ExcelQueryFactory(_pathExcelFile);
        }
        public string PathExcelFile
        {
            get
            {
                return _pathExcelFile;
            }
        }
        public ExcelQueryFactory UrlConnexion
        {
            get
            {
                return _urlConnexion;
            }
        }
    }

    public class Flights
    {
        public int Id { get; set; }
        public string DATE { get; set; }
        public string ROUTE { get; set; }
        public string NO { get; set; }
        public string DEP { get; set; }
        public string ARR { get; set; }
        public string REG { get; set; }
        public string REGISTER { get; set; }
        public int RegisterID { get; set; }
        public int OriginId { get; set; }
        public int DestinationId { get; set; }


        public DateTime FlightDate
        {
            get
            {

                return Convert.ToDateTime(this.DATE).Date;

            }
        }

        public int DEPHour
        {
            get
            {

                string[] split = this.DEP.Split(':', ' ');

                return Convert.ToInt32(split[1]);
            }
        }
        public int DEPMinute
        {
            get
            {

                string[] split = this.DEP.Split(':', ' ');

                return Convert.ToInt32(split[2]);
            }
        }
        public string DEPAMPM
        {
            get
            {
                if (string.IsNullOrEmpty(this.DEP))
                    return null;
                string[] split = this.DEP.Split(':', ' ');

                return split[4];
            }
        }
        public int ARRHour
        {
            get
            {
                string[] split = this.ARR.Split(':', ' ');

                return Convert.ToInt32(split[1]);
            }
        }
        public int ARRMinute
        {
            get
            {
                string[] split = this.ARR.Split(':', ' ');

                return Convert.ToInt32(split[2]);
            }
        }
        public string ARRAMPM
        {
            get
            {
                if (string.IsNullOrEmpty(this.DEP))
                    return null;
                string[] split = this.ARR.Split(':', ' ');

                return split[4].ToString();
            }
        }

        public DateTime STDLocal
        {
            get
            {
                var hh = DEPHour;
                if (DEPAMPM.ToUpper() == "PM" && hh < 12)
                    hh += 12;
                if (DEPAMPM.ToUpper() == "AM" && hh == 12)
                    hh = 0;
                var dt = this.FlightDate;
                return new DateTime(dt.Year, dt.Month, dt.Day, hh, this.DEPMinute, 0);
                //string hour = string.Empty;
                //if (Int32.Parse(DEPHour) < 10)
                //    hour = "0" + DEPHour;
                //else
                //    hour = DEPHour;



                //string date = FlightDate + " " + hour + ":" + DEPMinute + ":" + "00";

                //return DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null);


            }
        }

        public DateTime STALocal
        {
            get
            {
                var hh = ARRHour;
                if (ARRAMPM.ToUpper() == "PM" && hh < 12)
                    hh += 12;
                if (ARRAMPM.ToUpper() == "AM" && hh == 12)
                    hh = 0;

                var dt = this.FlightDate;
                if (ARRAMPM.ToUpper() == "AM" && DEPAMPM.ToUpper() == "PM")
                    dt = dt.AddDays(1);

                return new DateTime(dt.Year, dt.Month, dt.Day, hh, this.ARRMinute, 0);


            }
        }

        public DateTime STD
        {
            get
            {


                return STDLocal.AddMinutes(-270);


            }
        }

        public DateTime STA
        {
            get
            {
                var hh = ARRHour;
                if (ARRAMPM.ToUpper() == "PM" && hh < 12)
                    hh += 12;
                if (ARRAMPM.ToUpper() == "AM" && hh == 12)
                    hh = 0;

                var dt = this.FlightDate;
                if (ARRAMPM.ToUpper() == "AM" && DEPAMPM.ToUpper() == "PM")
                    dt = STALocal.AddMinutes(-270).AddDays(1);
                else
                    dt = STALocal.AddMinutes(-270);
                //    dt = dt.AddDays(1);

                return dt;
                //var result = new DateTime(dt.Year, dt.Month, dt.Day, hh, this.ARRMinute, 0);

                //return result.ToUniversalTime();


            }
        }
        public string Origin
        {
            get
            {
                string[] result = ROUTE.Split('-');
                return result[0];
            }
        }
        public string Destination
        {
            get
            {
                string[] result = ROUTE.Split('-');
                return result[1];
            }
        }

        public DateTime? STDDay
        {
            get
            {
                return STD.Date;
            }
        }
        public DateTime STADay
        {
            get
            {
                return STA.Date;
            }
        }
        public DateTime STDDayLocal
        {
            get
            {
                return STDLocal.Date;
            }
        }
        public DateTime STADayLocal
        {
            get
            {
                return STALocal.Date;
            }
        }


    }
    public class Flights3
    {
        public int Id { get; set; }
        public string DATE { get; set; }
        public string NO { get; set; }
        public string DEP { get; set; }
        public string ARR { get; set; }
        
        public string REGISTER { get; set; }
        public string REG { get; set; }
        public string ROUTE { get; set; }
        
    }
    public class Flights2
    {
        public int Id { get; set; }
        public string DATE { get; set; }
        public string NO { get; set; }
        public string DEP { get; set; }
        public string ARR { get; set; }
        
        public string REGISTER { get; set; }
        public string REG { get; set; }
        public string ORIGIN { get; set; }
        public string DESTINATION { get; set; }
        public string ROUTE { get; set; }
        public int RegisterID { get; set; }
        public int OriginId { get; set; }
        public int DestinationId { get; set; }


        public DateTime FlightDate
        {
            get
            {

                return Convert.ToDateTime(this.DATE).Date;

            }
        }

        public int DEPHour
        {
            get
            {

                string[] split = this.DEP.Split(':', ' ');

                return Convert.ToInt32(split[1]);
            }
        }
        public int DEPMinute
        {
            get
            {

                string[] split = this.DEP.Split(':', ' ');

                return Convert.ToInt32(split[2]);
            }
        }
        public string DEPAMPM
        {
            get
            {
                if (string.IsNullOrEmpty(this.DEP))
                    return null;
                string[] split = this.DEP.Split(':', ' ');

                return split[4];
            }
        }
        public int ARRHour
        {
            get
            {
                string[] split = this.ARR.Split(':', ' ');

                return Convert.ToInt32(split[1]);
            }
        }
        public int ARRMinute
        {
            get
            {
                string[] split = this.ARR.Split(':', ' ');

                return Convert.ToInt32(split[2]);
            }
        }
        public string ARRAMPM
        {
            get
            {
                if (string.IsNullOrEmpty(this.DEP))
                    return null;
                string[] split = this.ARR.Split(':', ' ');

                return split[4].ToString();
            }
        }

        public DateTime STDLocal
        {
            get
            {
                var hh = DEPHour;
                if (DEPAMPM.ToUpper() == "PM" && hh < 12)
                    hh += 12;
                if (DEPAMPM.ToUpper() == "AM" && hh == 12)
                    hh = 0;
                var dt = this.FlightDate;
                return new DateTime(dt.Year, dt.Month, dt.Day, hh, this.DEPMinute, 0);
            }
        }

        public DateTime STALocal
        {
            get
            {

                var hh = ARRHour;
                if (ARRAMPM.ToUpper() == "PM" && hh < 12)
                    hh += 12;
                if (ARRAMPM.ToUpper() == "AM" && hh == 12)
                    hh = 0;

                var dt = this.FlightDate;
                if (ARRAMPM.ToUpper() == "AM" && DEPAMPM.ToUpper() == "PM")
                    dt = dt.AddDays(1);

                return new DateTime(dt.Year, dt.Month, dt.Day, hh, this.ARRMinute, 0);


            }
        }

        public DateTime STD
        {
            get
            {
                var tzoffset3 =- TimeZoneInfo.Local.GetUtcOffset((DateTime)STDLocal).TotalMinutes;

                return STDLocal.AddMinutes(tzoffset3);


            }
        }

        public DateTime STA
        {
            get
            {
                var tzoffset3 =- TimeZoneInfo.Local.GetUtcOffset((DateTime)STALocal).TotalMinutes;
                return STALocal.AddMinutes(tzoffset3);


            }
        }

        public DateTime STDDay
        {
            get
            {
                return STD.Date;
            }
        }
        public DateTime STADay
        {
            get
            {
                return STA.Date;
            }
        }
        public DateTime STDDayLocal
        {
            get
            {
                return STDLocal.Date;
            }
        }
        public DateTime STADayLocal
        {
            get
            {
                return STALocal.Date;
            }
        }
    }

   [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FileController : ApiController
    {
        [HttpGet]
        [Route("api/path")]
        public async Task<IHttpActionResult> GetPath()
        {

            var sss = System.Web.Hosting.HostingEnvironment.MapPath("~/upload");
            return Ok(sss);
        }


        [HttpGet]
        [Route("api/file/import/flights/{fn}")]
        public async Task<IHttpActionResult> ImportFlights(string fn)
        {
            if (fn.ToLower().IndexOf("xlsx") == -1)
                fn = fn + ".xlsx";
            List<Flights> flights = new List<Flights>();
            var context = new AirpocketAPI.Models.FLYEntities();
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/upload");
            var dir = Path.Combine(path, fn + ".xlsx");
            ConnexionExcel ConxObject = new ConnexionExcel(dir);

            //Query a worksheet with a header row  
            var query = from a in ConxObject.UrlConnexion.Worksheet<Flights>("Sheet1")
                        select a;
            flights = query.ToList();

            var origins = flights.Select(q => q.Origin);
            var destinations = flights.Select(q => q.Destination);
            var conCatList = origins.Concat(destinations).ToList();
            var regList = flights.Select(q => q.REGISTER).ToList();
            var dates = flights.Select(q => q.STDDay).ToList().Distinct();


            var airports = context.Airports.Where(q => conCatList.Contains(q.IATA)).ToList();
            var acMSN = context.Ac_MSN.Where(q => regList.Contains(q.Register)).ToList();
            var legTimes = context.ViewLegTimes.Where(q => dates.Contains(q.STDDay)).ToList();

            foreach (var x in flights)
            {
                var aptOrigin = airports.FirstOrDefault(q => q.IATA == x.Origin);

                if (aptOrigin != null)
                    x.OriginId = aptOrigin.Id;

                var aptDestination = airports.FirstOrDefault(q => q.IATA == x.Destination);

                if (aptDestination != null)
                    x.DestinationId = aptDestination.Id;

                var msnReg = acMSN.FirstOrDefault(q => q.Register == x.REGISTER);
                if (msnReg != null)
                    x.RegisterID = msnReg.ID;

                var legTime = legTimes.FirstOrDefault(q => q.STDDay == x.STDDay && q.FlightNumber == x.NO);


            }





            return Ok(flights);
        }


        [HttpGet]
        [Route("api/file/import/flights/2/{fn}")]
        public async Task<IHttpActionResult> ImportFlights2(string fn)
        {
            var sxmsg = "";
            HttpContext.Current.Server.ScriptTimeout = 300;
            List<FlightInformation> OBJ2 = new List<FlightInformation>();
            List<Flights2> OBJ1 = new List<Flights2>();
            if (fn.ToLower().IndexOf("xlsx") == -1)
                fn = fn + ".xlsx";
            int? result = null;
            string resultStr = string.Empty;
            var flightsInserted = 0;
             try
            {
             
           // return Ok(resultStr);
            List<Flights2> flights = new List<Flights2>();
            var context = new AirpocketAPI.Models.FLYEntities();
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/upload");
            var dir = Path.Combine(path, fn);
                 
            ConnexionExcel ConxObject = new ConnexionExcel(dir);



                 
                var query = from a in ConxObject.UrlConnexion.Worksheet<Flights2>("Sheet")
                            select a;

                flights = query.ToList();

                OBJ1 = flights;
                foreach (var x in flights)
                {
                    if (string.IsNullOrEmpty(x.REGISTER))
                        x.REGISTER = x.REG;
                    if (!string.IsNullOrEmpty(x.ROUTE))
                    {
                        x.ORIGIN = x.ROUTE.Split('-')[0];
                        x.DESTINATION = x.ROUTE.Split('-')[1];
                    }
                    if (x.REGISTER.IndexOf("01") != -1)
                        x.REGISTER = "EP-FPA";
                    if (x.REGISTER.IndexOf("02") != -1)
                        x.REGISTER = "EP-FPC";
                    x.REGISTER = x.REGISTER.Replace(" ", "").Trim();
                    x.ORIGIN = x.ORIGIN.Replace(" ", "").Trim();
                    x.DESTINATION = x.DESTINATION.Replace(" ", "").Trim();


                }
                    var origins = flights.Select(q => q.ORIGIN);
                var destinations = flights.Select(q => q.DESTINATION);
                var conCatList = origins.Concat(destinations).ToList();
                var regList = flights.Select(q => q.REGISTER).ToList();
                var stddays = flights.Select(q => q.STDDay).Distinct().ToList();


                var airports = context.Airports.Where(q => conCatList.Contains(q.IATA)).ToList();
                var acMSNs = context.Ac_MSN.Where(q => regList.Contains(q.Register)).ToList();
                var legTimes = context.ViewLegTimes.Where(q => stddays.Contains(q.STDDay ?? DateTime.Now)).ToList();




                foreach (var x in flights)
                {
                    sxmsg = x.FlightDate.ToString() + "   " + x.NO;
                    //if (string.IsNullOrEmpty(x.REGISTER))
                    //    x.REGISTER = x.REG;
                    //if (!string.IsNullOrEmpty(x.ROUTE))
                    //{
                    //    x.ORIGIN = x.ROUTE.Split('-')[0];
                    //    x.DESTINATION = x.ROUTE.Split('-')[1];
                    //}
                    //if (x.REGISTER.IndexOf("01") != -1)
                    //    x.REGISTER = "EP-FPA";
                    //if (x.REGISTER.IndexOf("02") != -1)
                    //    x.REGISTER = "EP-FPC";

                    var submision = x.STA - x.STD;
                    double totalMin = submision.TotalMinutes;
                    double hours = (totalMin - totalMin % 60) / 60;
                    double minute = totalMin - hours * 60;


                    var aptOrigin = airports.FirstOrDefault(q => q.IATA == x.ORIGIN);

                    if (aptOrigin != null)
                        x.OriginId = aptOrigin.Id;
                    else
                        x.OriginId = -1;

                    var aptDestination = airports.FirstOrDefault(q => q.IATA == x.DESTINATION);

                    if (aptDestination != null)
                        x.DestinationId = aptDestination.Id;
                    else
                        x.DestinationId = -1;

                    var msnReg = acMSNs.FirstOrDefault(q => q.Register == x.REGISTER);
                    if (msnReg != null)
                        x.RegisterID = msnReg.ID;


                    var existFlight = legTimes.FirstOrDefault(q => q.STDDay == x.STDDay && q.FlightNumber == x.NO);
                    if (existFlight == null)
                    {
                         
                            var entity = new FlightInformation()
                            {
                                RegisterID = x.RegisterID,
                                FlightTypeID = 109,
                                FlightStatusID = 1,
                                AirlineOperatorsID = 24,
                                FlightNumber = x.NO,
                                FromAirportId = x.OriginId,
                                ToAirportId = x.DestinationId,
                                STD = x.STD,
                                ChocksOut = x.STD,
                                Takeoff = x.STD,
                                STA = x.STA,
                                ChocksIn = x.STA,
                                Landing = x.STA,
                               // FlightH = Convert.ToInt32(hours),
                               // FlightM = Convert.ToByte(minute),
                                CustomerId = 1,
                                CPRegister = "XXX"
                            };
                         
                        try
                        {
                            entity.FlightH = Convert.ToInt32(hours);
                            entity.FlightM = Convert.ToByte(minute);
                        }
                        catch
                        {
                            entity.FlightH = 0;
                            entity.FlightM = 0;
                        }
                        context.FlightInformations.Add(entity);
                        OBJ2.Add(entity);
                        flightsInserted++;
                         

                            result = 200;
                      

                        //catch (System.Data.Entity.Core.UpdateException ex)
                        //{
                        //    Console.WriteLine(ex.InnerException);
                        //}

                        //catch (System.Data.Entity.Infrastructure.DbUpdateException ex) //DbContext
                        //{
                        //    Console.WriteLine(ex.InnerException);
                        //}

                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.InnerException);
                        //    throw;
                        //}
                    }
                    else
                    {
                       // Console.WriteLine("flight existed");
                    }
                }
               context.SaveChanges();
                resultStr = flightsInserted + " Flight(s) Inserted";

            }
            catch (Exception ex)
            {
                resultStr = sxmsg+"   "+ ex.Message;
                 
                if (ex.InnerException != null)
                    resultStr += " INNER EX: " + ex.InnerException;
                result = -1;
               // resultStr = "An error occured while file uploading. Please check File Type (xlsx), Sheet Name (sheet), Columns and records.";
                return Ok(new {MSG=resultStr, FLTS=OBJ1 });
            }
           // if (resultStr.IndexOf("Inserted")==-1)
           //     resultStr = "An error occured while file uploading. Please check File Type (xlsx), Sheet Name (sheet), Columns and records.";
            return Ok(resultStr);
        }

        [HttpGet]
        [Route("api/file/import/test/{fn}")]
        public async Task<IHttpActionResult> ImportFlights3(string fn)
        {
            var sxmsg = "";
            HttpContext.Current.Server.ScriptTimeout = 300;
            
            List<Flights3> OBJ1 = new List<Flights3>();
            if (fn.ToLower().IndexOf("xlsx") == -1)
                fn = fn + ".xlsx";
            int? result = null;
            string resultStr = string.Empty;
            var flightsInserted = 0;

            
             
            var context = new AirpocketAPI.Models.FLYEntities();
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/upload");
            var dir = Path.Combine(path, fn);

            ConnexionExcel ConxObject = new ConnexionExcel(dir);




            var query = from a in ConxObject.UrlConnexion.Worksheet<Flights3>("Sheet")
                        select a;

            var flights = query.ToList();

            OBJ1 = flights;


            return Ok(OBJ1);
        }

        [Route("api/uploadfile")]
        [AcceptVerbs("POST")]
        public  async Task<IHttpActionResult>  Upload()
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
                        var ext = System.IO.Path.GetExtension(postedFile.FileName);
                        key = date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + ext;

                        var filePath = HttpContext.Current.Server.MapPath("~/upload/" + key);
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);
                    }
                    // outPut = (await ImportFlights2(key));
                    // var ctrl = new FlightController();
                    //  outPut = await ctrl.UploadFlights3(key);
                    outPut = Ok(key);

                }
                else
                {
                    outPut = BadRequest();
                }
                return outPut;
            }
            catch(Exception ex)
            {
                return Ok(ex.Message+"   IN    "+(ex.InnerException!=null?ex.InnerException.Message:""));
            }
           
        }


        [Route("api/upload/atc/flightplan")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> UploadATCFLIGHPLAN()
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
                        var ext = System.IO.Path.GetExtension(postedFile.FileName);
                        key ="atc-"+ date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + ext;

                        var filePath = ConfigurationManager.AppSettings["skybag"] + key; //HttpContext.Current.Server.MapPath("~/upload/" + key);
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);
                    }
                    // outPut = (await ImportFlights2(key));
                    // var ctrl = new FlightController();
                    //  outPut = await ctrl.UploadFlights3(key);
                    outPut = Ok(key);

                }
                else
                {
                    return Ok("error");
                }
                return outPut;
            }
            catch (Exception ex)
            {
                return Ok(ex.Message + "   IN    " + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }

        }

        [Route("api/upload/atl")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> UploadATL()
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
                        var ext = System.IO.Path.GetExtension(postedFile.FileName);
                        key ="atl-"+ date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + ext;

                        var filePath = ConfigurationManager.AppSettings["skybag"] +key; //HttpContext.Current.Server.MapPath("~/upload/" + key);
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);
                    }
                    // outPut = (await ImportFlights2(key));
                    // var ctrl = new FlightController();
                    //  outPut = await ctrl.UploadFlights3(key);
                    outPut = Ok(key);

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

    }
}
