using ApiFDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.IO;
using LinqToExcel;
using System.Web;
using ApiFDM.Objects;
using System.Web.Hosting;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Globalization;
using Spire.Xls;

namespace ApiFDM.Controllers
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


    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FileController : ApiController
    {







        //public async Task<List<FailedItmes>> boeingImport(string fn)
        //{

        //    var context = new dbEntities();

        //    List<Boeing> flights = new List<Boeing>();
        //    List<FailedItmes> failedItems = new List<FailedItmes>();

        //    var path = HostingEnvironment.MapPath("~/upload");
        //    var dir = Path.Combine(path, fn);
        //    ConnexionExcel excelFile = new ConnexionExcel(dir);

        //    try
        //    {
        //        var names = excelFile.UrlConnexion.GetWorksheetNames();
        //        var event_sheet = names.Where(q => q.ToUpper().Contains("EVENT")).FirstOrDefault();
        //        if (string.IsNullOrEmpty(event_sheet))
        //            event_sheet = names.First();
        //        var query = from x in excelFile.UrlConnexion.WorksheetNoHeader(event_sheet)
        //                    select x;

        //        var rows = query.ToList();


        //        for (int i = 1; i < rows.Count(); i++)
        //        {
        //            var flight = new Boeing();
        //            var row = rows[i];

        //            flight.Severity = row[0];
        //            flight.EventName = row[1];
        //            flight.Value = row[2];
        //            flight.Minor = row[3];
        //            flight.Major = row[4];
        //            flight.Critical = row[5];
        //            flight.Duration = TimeSpan.Parse(row[6]);
        //            flight.TOAirport = row[7];
        //            flight.TDAirport = row[8];
        //            flight.RecdFltNum = row[9];
        //            flight.TDDatetime = row[10];
        //            //flight.IP = row[11];
        //            //flight.P1 = row[12];
        //            //flight.P2 = row[13];
        //            flight.Aircraft = row[11];
        //            flight.FlightPhase = row[12];
        //            flight.StateName = row[13];
        //            flight.Context = row[14];
        //            flight.TORunway = row[15];
        //            flight.TDRunway = row[16];
        //            flight.TODatetime = row[17];
        //            flight.Type = row[18];
        //            flight.Units = row[19];
        //            flight.ValueName = row[20];
        //            flight.EnginePos = row[21];

        //            flights.Add(flight);
        //        }



        //        var fileExist = context.FDMs.Where(q => q.FileName == fn).ToList();
        //        context.FDMs.RemoveRange(fileExist);



        //        var dates = flights.Where(q => q.DateX != null).Select(q => (Nullable<DateTime>)((DateTime)q.DateX).Date).ToList();
        //        var existFlight = context.AppLegs.Where(q => dates.Contains(q.STDDay)).ToList();
        //        var P1Codes = flights.Select(q => q.P1).ToList();
        //        var P2Codes = flights.Select(q => q.P2).ToList();
        //        var IPCodes = flights.Select(q => q.IP).ToList();
        //        var PilotsCode = P1Codes.Concat(P2Codes.Concat(IPCodes)).ToList();
        //        var PilotsId = context.CrewSecretCodes.Where(q => PilotsCode.Contains(q.Code)).ToList();
        //        var fdmKeys = context.FDMs.Select(q => q.Key).ToList();


        //        foreach (var y in flights)
        //        {
        //            var flight = existFlight.FirstOrDefault(q => q.STDDay == y.DateX && q.FlightNumber == y.FlightNumber);
        //            var Key = (y.EventName).Trim() + Trim(y.Aircraft) + DateConvert(y.DateX) + y.TOAirport + y.TDAirport + (y.FlightPhase == null ? string.Empty : y.FlightPhase);
        //            if (fdmKeys.Contains(Key))
        //                continue;



        //            if (y.IsValid && flight != null && flight.AircraftType.Contains("B"))
        //            {
        //                var entity = new FDM();
        //                entity.Severity = y.SeverityX;
        //                entity.Date = y.DateX;
        //                entity.EventName = y.EventName;
        //                entity.Duration = y.Duration;
        //                //entity.P1 = y.P1;
        //                //entity.P1Id = PilotsId.Where(q => q.Code == y.P1).Select(w => (int?)w.CrewId).DefaultIfEmpty().First();
        //                //entity.P2 = y.P2;
        //                //entity.P2Id = PilotsId.Where(q => q.Code == y.P2).Select(w => (int?)w.CrewId).DefaultIfEmpty().First();
        //                //entity.IP = y.IP;
        //                //entity.IPId = PilotsId.Where(q => q.Code == y.IP).Select(w => (int?)w.CrewId).DefaultIfEmpty().First();
        //                entity.P1 = flight.P1Name;
        //                entity.P1Id = flight.P1Id;
        //                entity.P2 = flight.P2Name;
        //                entity.P2Id = flight.P2Id;
        //                entity.IP = flight.IPName;
        //                entity.IPId = flight.IPId;
        //                entity.PFLR = flight.PFLR.ToString();
        //                entity.Value = y.Value;
        //                entity.Minor = y.MinorX;
        //                entity.Major = y.MajorX;
        //                entity.Critical = y.CriticalX;
        //                entity.Phase = y.FlightPhase;
        //                entity.Context = y.Context;
        //                entity.StateName = y.StateName;
        //                entity.Type = y.Type;
        //                entity.Units = y.Units;
        //                entity.TODatetime = null;
        //                entity.TDDatetime = null;
        //                entity.FileName = fn;
        //                entity.AircraftType = flight.AircraftType;
        //                entity.Key = Key;
        //                entity.FlightId = flight.FlightId;
        //                entity.FromAirport = flight.FromAirport;
        //                entity.ToAirport = flight.ToAirport;
        //                entity.FromAirportIATA = flight.FromAirportIATA;
        //                entity.ToAirportIATA = flight.ToAirportIATA;
        //                entity.FlightId = flight.FlightId;
        //                entity.AircraftTypeId = flight.TypeId;
        //                entity.Approved = false;
        //                entity.Removed = false;
        //                entity.Islocked = false;
        //                entity.IsVisible = false;
        //                context.FDMs.Add(entity);

        //            }
        //            else
        //            {

        //                var failedItem = new FailedItmes();
        //                failedItem.Severity = y.Severity;
        //                failedItem.Date = y.DateX;
        //                failedItem.EventName = y.EventName;
        //                failedItem.P1 = y.P1;
        //                failedItem.P2 = y.P2;
        //                failedItem.FileName = fn;
        //                failedItem.Value = y.ValueX;
        //                failedItem.Duration = y.Duration;
        //                failedItem.flightNo = y.FlightNumber;
        //                failedItem.Message = flight != null ? string.Empty : "Flight record not found";
        //                failedItems.Add(failedItem);
        //            }

        //        }

        //        context.SaveChanges();
        //        return failedItems;

        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += " INNSER: " + ex.InnerException.Message;
        //        var exist = new FailedItmes();
        //        exist.Status = 500;
        //        exist.FileName = fn;
        //        exist.Message = msg;
        //        var inner = ex.ToString();
        //        failedItems.Add(exist);
        //        return failedItems;
        //    }
        //}


        public async Task<List<FailedItmes>> boeingImport(string fn)
        {
            try
            {
                var context = new dbEntities();

                List<Boeing> flights = new List<Boeing>();
                List<FailedItmes> failedItems = new List<FailedItmes>();

                var path = HostingEnvironment.MapPath("~/upload");
                var dir = Path.Combine(path, fn);
                ConnexionExcel excelFile = new ConnexionExcel(dir);


                var names = excelFile.UrlConnexion.GetWorksheetNames();
                var event_sheet = names.Where(q => q.ToUpper().Contains("EVENT")).FirstOrDefault();
                if (string.IsNullOrEmpty(event_sheet))
                    event_sheet = names.First();
                var query = from x in excelFile.UrlConnexion.WorksheetNoHeader(event_sheet)
                            select x;

                var rows = query.ToList();


                for (int i = 1; i < rows.Count(); i++)
                {
                    var flight = new Boeing();
                    var row = rows[i];

                    flight.Severity = row[0];
                    flight.EventName = row[1];
                    flight.Duration = float.Parse(row[2]);
                    flight.Value = row[3];
                    flight.Limit = row[4];
                    flight.FromAirport = row[5];
                    flight.FromAirportIATA =row[6];
                    flight.ToAirport = row[7];
                    flight.ToAirportIATA = row[7];
                    flight.TO_Datetime = row[9];
                    flight.TD_Datetime = row[10];
                    flight.FlightNo = row[11];
                    flight.Aircraft = row[12];
                    flight.IP = row[13];
                    flight.P1 = row[14];
                    flight.P2 = row[15];
                    flight.FlyBy = row[16];
                    flight.Phase = row[17];
                    flight.MainParameter = row[18];
                    flight.Context = row[19];
                    
                    flights.Add(flight);
                }



                var fileExist = context.FDMs.Where(q => q.FileName == fn).ToList();
                context.FDMs.RemoveRange(fileExist);



                var dates = flights.Where(q => q.DateX != null).Select(q => (Nullable<DateTime>)((DateTime)q.DateX).Date).ToList();
                var existFlight = context.AppLegs.Where(q => dates.Contains(q.STDDay)).ToList();
                var P1Codes = flights.Select(q => q.P1).ToList();
                var P2Codes = flights.Select(q => q.P2).ToList();
                var IPCodes = flights.Select(q => q.IP).ToList();
                var PilotsCode = P1Codes.Concat(P2Codes.Concat(IPCodes)).ToList();
                var PilotsId = context.CrewSecretCodes.Where(q => PilotsCode.Contains(q.Code)).ToList();
                var fdmKeys = context.FDMs.Select(q => q.Key).ToList();


                foreach (var y in flights)
                {
                    var flight = existFlight.FirstOrDefault(q => q.STDDay == y.DateX && q.FlightNumber == y.FlightNumber);
                    var Key = (y.EventName).Trim() + Trim(y.Aircraft) + DateConvert(y.DateX) + y.TOAirport + y.TDAirport + (y.Phase == null ? string.Empty : y.Phase);
                    if (fdmKeys.Contains(Key))
                        continue;



                    if (y.IsValid && flight != null && flight.AircraftType.Contains("B"))
                    {
                        var entity = new FDM();
                        entity.Severity = y.SeverityX;
                        entity.Date = y.DateX;
                        entity.EventName = y.EventName;
                        entity.Duration = TimeSpan.FromSeconds((double)new decimal(y.Duration));
                        //entity.P1 = y.P1;
                        //entity.P1Id = PilotsId.Where(q => q.Code == y.P1).Select(w => (int?)w.CrewId).DefaultIfEmpty().First();
                        //entity.P2 = y.P2;
                        //entity.P2Id = PilotsId.Where(q => q.Code == y.P2).Select(w => (int?)w.CrewId).DefaultIfEmpty().First();
                        //entity.IP = y.IP;
                        //entity.IPId = PilotsId.Where(q => q.Code == y.IP).Select(w => (int?)w.CrewId).DefaultIfEmpty().First();
                        entity.P1 = flight.P1Name;
                        entity.P1Id = flight.P1Id;
                        entity.P2 = flight.P2Name;
                        entity.P2Id = flight.P2Id;
                        entity.IP = flight.IPName;
                        entity.IPId = flight.IPId;
                        entity.PFLR = flight.PFLR.ToString();
                        entity.Value = y.Value;
                        entity.Minor = y.MinorX;
                        entity.Major = y.MajorX;
                        entity.Critical = y.CriticalX;
                        entity.Phase = y.Phase;
                        entity.Context = y.Context;
                        entity.StateName = y.StateName;
                        entity.Type = y.Type;
                        entity.Units = y.Units;
                        entity.TODatetime = null;
                        entity.TDDatetime = null;
                        entity.FileName = fn;
                        entity.AircraftType = flight.AircraftType;
                        entity.Key = Key;
                        entity.FlightId = flight.FlightId;
                        entity.FromAirport = flight.FromAirport;
                        entity.ToAirport = flight.ToAirport;
                        entity.FromAirportIATA = flight.FromAirportIATA;
                        entity.ToAirportIATA = flight.ToAirportIATA;
                        entity.FlightId = flight.FlightId;
                        entity.AircraftTypeId = flight.TypeId;
                        entity.Approved = false;
                        entity.Removed = false;
                        entity.Confirmation = false;
                        entity.IsVisible = false;
                        entity.Validity = 0;
                        entity.MainParameter = y.MainParameter;
                        context.FDMs.Add(entity);

                    }
                    else
                    {

                        var failedItem = new FailedItmes();
                        failedItem.Severity = y.Severity;
                        failedItem.Date = y.DateX;
                        failedItem.EventName = y.EventName;
                        failedItem.P1 = y.P1;
                        failedItem.P2 = y.P2;
                        failedItem.FileName = fn;
                        failedItem.Value = y.ValueX;
                        failedItem.Duration = TimeSpan.FromSeconds((double)new decimal(y.Duration));
                        failedItem.flightNo = y.FlightNumber;
                        failedItem.Message = flight != null ? string.Empty : "Flight record not found";
                        failedItems.Add(failedItem);
                    }

                }

                var date = flights[0].DateX.ToString();
                int year = DateTime.Parse(date).Year;
                int month = DateTime.Parse(date).Month;
                context.SaveChanges();
                UpdateFdmTbl(year, month, year, month);
                return failedItems;

            }
            catch (Exception ex)
            {
                var ds = new List<FailedItmes>();
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " INNSER: " + ex.InnerException.Message;
                var exist = new FailedItmes();
                exist.Status = 500;
                exist.FileName = fn;
                exist.Message = msg;
                var inner = ex.ToString();
                ds.Add(exist);
                return ds;
            }
        }


        public async Task<List<FailedItmes>> mdImport(string fn)
        {

            var context = new dbEntities();
            List<MD> flights = new List<MD>();
            List<FailedItmes> failedItems = new List<FailedItmes>();


            var path = HostingEnvironment.MapPath("~/upload");
            var dir = Path.Combine(path, fn);
            ConnexionExcel excelFile = new ConnexionExcel(dir);
            try
            {

                var names = excelFile.UrlConnexion.GetWorksheetNames();
                var event_sheet = names.Where(q => q.ToUpper().Contains("EVENT")).FirstOrDefault();
                if (string.IsNullOrEmpty(event_sheet))
                    event_sheet = names.First();
                var query = from x in excelFile.UrlConnexion.WorksheetNoHeader(event_sheet)
                            select x;

                var rows = query.ToList();


                for (int i = 1; i < rows.Count(); i++)
                {

                    var flight = new MD();
                    var row = rows[i];

                    flight.Severity = row[0];
                    flight.EventName = row[1];
                    flight.Duration = float.Parse(row[2]);
                    flight.ExceedValue = row[3];
                    flight.LimitValue = row[4];
                    flight.Phase = row[5];
                    flight.FromAirport = row[6];
                    flight.FromAirportIATA = row[7];
                    flight.ToAirport = row[8];
                    flight.ToAirportIATA = row[9];
                    flight.FlightNo = row[10];
                    flight.Aircraft = row[11];
                    flight.IP = row[12];
                    flight.P1 = row[13];
                    flight.P2 = row[14];
                    flight.PIC = row[15];
                    flight.AircraftType = row[16];
                    flight.MainParameter = row[17];
                    flight.Context = row[18];
                    flight.TO_DateTime = row[19];
                    flight.TD_DateTime = row[20];
                    //flight.Date = row[5];
                    //flight.FlightNo = row[6];
                    //flight.Reg = row[7];
                    //flight.From = row[8];
                    //flight.To = row[9];
                    //flight.FlightPhase = row[10];
                    //flight.StateName = row[11];
                    ////flight.IP = row[10];
                    ////flight.P1 = row[11];
                    ////flight.P2 = row[12];
                    ////flight.PFLR = row[13];
                    flights.Add(flight);
                }




                var fileExist = context.FDMs.Where(q => q.FileName == fn).ToList();
                context.FDMs.RemoveRange(fileExist);



                var dates = flights.Where(q => q.DateX != null).Select(q => (Nullable<DateTime>)((DateTime)q.DateX).Date).ToList();

                var existFlight = context.AppLegs.Where(q => dates.Contains(q.STDDay)).ToList();
                var P1Codes = flights.Select(q => q.P1).ToList();
                var P2Codes = flights.Select(q => q.P2).ToList();
                var IPCodes = flights.Select(q => q.IP).ToList();
                var PilotsCode = P1Codes.Concat(P2Codes.Concat(IPCodes)).ToList();
                var PilotsId = context.CrewSecretCodes.Where(q => PilotsCode.Contains(q.Code)).ToList();
                var fdmKeys = context.FDMs.Select(q => q.Key).ToList();


                foreach (var y in flights)
                {

                    var flight = existFlight.FirstOrDefault(q => q.STDDay == y.DateX && q.FlightNumber == y.FlightNumber);
                    var Key = Trim(y.EventName) + Trim(y.Aircraft) + DateConvert(y.DateX) + y.FromAirportIATA + y.ToAirportIATA;

                    if (fdmKeys.Contains(Key))
                        continue;


                    if (y.IsValid && flight != null && flight.AircraftType.Contains("MD"))
                    {
                        var entity = new FDM();
                        entity.Severity = y.LimitLevelX;
                        entity.Date = y.DateX;
                        entity.EventName = y.EventName;
                        entity.Duration = TimeSpan.FromSeconds((double)new decimal(y.Duration));
                        entity.Value = y.ExceedValue;
                        //entity.P1 = y.P1;
                        //entity.P1Id = PilotsId.Where(q => q.Code == y.P1).Select(w => (int?)w.CrewId).DefaultIfEmpty().First();
                        //entity.P2 = y.P2;
                        //entity.P2Id = PilotsId.Where(q => q.Code == y.P2).Select(w => (int?)w.CrewId).DefaultIfEmpty().First();
                        //entity.IP = y.IP;
                        //entity.IPId = PilotsId.Where(q => q.Code == y.IP).Select(w => (int?)w.CrewId).DefaultIfEmpty().First();
                        //entity.PFLR = y.PFLR;
                        entity.P1 = flight.P1Name;
                        entity.P1Id = flight.P1Id;
                        entity.P2 = flight.P2Name;
                        entity.P2Id = flight.P2Id;
                        entity.IP = flight.IPName;
                        entity.IPId = flight.IPId;
                        entity.PFLR = flight.PFLR.ToString();
                        entity.FileName = fn;
                        entity.RegisterId = flight.RegisterID;
                        entity.AircraftType = flight.AircraftType;
                        entity.FromAirport = flight.FromAirport;
                        entity.ToAirport = flight.ToAirport;
                        entity.FromAirportIATA = flight.FromAirportIATA;
                        entity.ToAirportIATA = flight.ToAirportIATA;
                        entity.FlightId = flight.FlightId;
                        //entity.AircraftTypeId = flight.TypeId;
                        entity.Limit = y.LimitValue;
                        entity.Key = Key;
                        entity.Approved = false;
                        entity.Removed = false;
                        entity.Confirmation = false;
                        entity.IsVisible = false;
                        entity.Validity = 0;
                        entity.Phase = y.Phase;
                        entity.StateName = y.StateName;
                        entity.MainParameter = y.MainParameter;
                        entity.PIC = y.PIC;
                        entity.Context = y.Context;
                        context.FDMs.Add(entity);

                    }
                    else
                    {
                        var failedItem = new FailedItmes();
                        failedItem.Severity = y.Severity;
                        failedItem.Date = y.DateX;
                        failedItem.EventName = y.EventName;
                        failedItem.P1 = y.P1;
                        failedItem.P2 = y.P2;
                        failedItem.Value = y.ValueX;
                        failedItem.FileName = fn;
                        failedItem.Duration = TimeSpan.FromSeconds((double)new decimal(y.Duration));
                        failedItem.flightNo = y.FlightNo;
                        failedItem.Message = flight != null ? string.Empty : "Flight record not found";
                        failedItems.Add(failedItem);

                    }

                }





                var date = flights[0].DateX.ToString();
                int year = DateTime.Parse(date).Year;
                int month = DateTime.Parse(date).Month;
                context.SaveChanges();
                UpdateFdmTbl(year, month, year, month);
                return failedItems;


            }
            catch (Exception ex)
            {

                var exist = new FailedItmes();
                exist.Status = 500;
                exist.FileName = fn;
                var inner = ex.ToString();
                failedItems.Add(exist);
                return failedItems;
            }

        }











        [HttpGet]
        [Route("api/fdr/{fn}")]
        public async Task<DataResponse> FDRImport(string fn)
        {

            var path = HostingEnvironment.MapPath("~/upload");
            var dir = Path.Combine(path, fn + ".htm");
            var doc = new HtmlDocument();
            doc.Load(dir);


            var text = doc.DocumentNode.SelectNodes("//table/tr/td/tt").Select(q => q.InnerText).ToList();

            //var result = text.Skip(text.IndexOf("Piloting occurrences")).Take(text.IndexOf("All systems") - text.IndexOf("Piloting occurrences"));
            var finalText = text.Skip(text.IndexOf("Algorythm Pack for MD-83 Aircrafts (Version 1.4a)"));

            List<string> Names = new List<string>();
            List<string> Values = new List<string>();
            foreach (var x in finalText)
            {

                if (Regex.IsMatch(x, @"^[A-Z0-9.:]+$") == true)
                {
                    Names.Add(x);
                }
                else
                {
                    Values.Add(x);
                }
            }



            return new DataResponse
            {
                Data = new { Names, Values, DateTime.Now },
                IsSuccess = true
            };


        }

        [HttpPost]
        [Route("api/uploadfile")]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                //var context = new FLYEntities();
                string key = string.Empty;
                var Files = HttpContext.Current.Request.Files;

                var docfiles = new List<string>();
                foreach (string file in Files)
                {
                    var postedFile = Files[file];
                    key = postedFile.FileName;
                    var filePath = HttpContext.Current.Server.MapPath("~/upload/" + key);
                    postedFile.SaveAs(filePath);
                    docfiles.Add(filePath);
                }

                return Ok(docfiles);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [HttpGet]
        [Route("api/fdm/update/fdmtbl")]
        public async Task<DataResponse> UpdateFdmTbl(int yf, int mf, int yt, int mt)
        {
            try
            {
                var context = new dbEntities();
                context.Database.CommandTimeout = 200;
                var result = context.FillFDMCptMonthlyTBL(yf, mf, yt, mt);

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex
                };
            }
            //try
            //{
            //    var context = new dbEntities();
            //    var fdmds = context.FDMCptMonthlies.ToList();
            //    foreach (var i in context.FDMCptMonthlyTBLs) 
            //    {
            //        context.FDMCptMonthlyTBLs.Remove(i); 
            //    }
            //    foreach (var f in fdmds)
            //    {
            //        var entity = new FDMCptMonthlyTBL();
            //        entity.Year = (int)(f.Year == null ? -1 : f.Year);
            //        entity.Month = (int)(f.Month == null ? -1 : f.Month);
            //        entity.YearMonth = f.YearMonth;
            //        entity.ScorePerFlight = f.ScorePerFlight;
            //        entity.ScorePerEvent = f.ScorePerEvent;
            //        entity.Score = f.Score;
            //        entity.MediumScore = f.MediumScore;
            //        entity.MediumCount = f.MediumCount;
            //        entity.LowScore = f.LowScore;
            //        entity.LowCount = f.LowCount;
            //        entity.HighScore = f.HighScore;
            //        entity.HighCount = f.HighCount;
            //        entity.JobGroup = f.JobGroup;
            //        entity.FlightCount = f.FlightCount;
            //        entity.EventPerFlight = f.EventPerFlight;
            //        entity.EventCount = f.EventCount;
            //        entity.CptName = f.CptName;
            //        entity.CptId = f.CptId;
            //        entity.CptCode = f.CptCode;
            //        entity.AircraftTypeId = (int)(f.TypeId == null ? -1 : f.TypeId);
            //        entity.AircraftType = f.AircraftType;
            //        context.FDMCptMonthlyTBLs.Add(entity);

            //    }
            //    context.SaveChanges();
            //    return Ok();
            //}
            //catch (Exception ex)

            //{
            //    return Ok(ex);
            //}
        }



        [HttpPost]
        [Route("api/import")]
        public async Task<IHttpActionResult> fileImport(string[] names)
        {
            DataResponse outPut = new DataResponse { };
            List<FailedItmes> failedItmes = new List<FailedItmes>();

            foreach (string name in names)
            {
                if (name.Contains("737"))
                {
                    failedItmes.AddRange(await boeingImport(name));
                }
                if (name.Contains("MD"))
                {
                    failedItmes.AddRange(await mdImport(name));
                }
            }

            return Ok(failedItmes);
        }




        //[HttpGet]
        //[Route("api/import/excel/{fn}")]
        //public async Task<List<string>> importExcel(string fn)
        //{

        //    string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pg0KPExpY2Vuc2UgS2V5PSJjcjhEWjdYSjJEeTFLNlFCQU5ET0lUS3ZaY082ZHpVaHdpbEhwZ1ZZbkN5NHF5R1dlekxWbmxSRnhQMTVNZklmZ1JnTVptV2hHTkFkRTRUamVmZ0NaL2xUdm9QZEl0SGw2V3Q1QTVpNU1YRW5xZEJ1TzFLYUZ6L0RRc2FHVkxoc3Y5ckhtcm50cUhJRURsSXhkcWFKTXBrS29Ba3dQN3dTek9NSjFZK21DZlU1VUZlekRML043dXpyeDNGNHdiNUhhK3dBNlRUOVRSd0swM3o5RUtNWkZsM1NZUi96NGFVN0xNMGRTWFk5alNGSmdnamZWc0RVS2lCclZubXdZY2l1cjlFa2JsOUN0WlkwM3RtcmZtNUJaZSs2Z2h0RU5uMG9oMzM4dFpSZXlqY3I3NEFrNzFoZ1lrbkxPQkMxNVZZZmpYc3F1QVVtdzJSNk1jVjJQT09icmNUUmJYQWd6b1FiT3lkOFNiRVpjd2hONzZLUHdXc1FRUzFKMHRpWUhVS3hPbTJ0NGZCVjBoUFZoYTlCOGNLMDRxSlFadDAwWjFjSkRhMHdiOFVsekVrOUJIVVcyZW5PZlQxNFJ5SENpK1lHZW1QS2NkQ1FyaDF6cllURjdJbW9MeDd4dTRldkRUWXNsc1dCa2xRSW94ODZyVnJFVWtTdHVxK0FDU1kvcVUzOS9WYXd2OUtBZlI1VGVFYnBrd0RoU2IwTkJBalQ4QXl0bERWZEdmaWcwcUtHM1ZZaVRwWEZ3NXB0TFZoK2JrZCtkZzd2eHR2cjQ1WlVXSmV5cnpHTkdIN2FGWWQ2cC8yTUcvWEpUbEd6L1NOUW8yQ1BMa2lPN0pYbjlOR2V4WjdwSG0wZGd6TVpiR0VYcVZkdmxtODEyYS9YTDFTcXhHVVkrbzVabFVDN1k1eGdnYUQrRmVQOXp6aHlKcUlFcHA5NzMvUnE0bXhtcEFmTHFTc08yUnh5U3EraXYxQ3NwMENybzA3ODhIcmwxbXlreHVUMHlkUllaQzZEU3g4TTIvTFkzZDlzbnd1NzZBZmI1QzlRdWRPWXNMM0RIdmhmZ3JjVUllL3FIZlRaOUFhemN6VGp5cjNkT0JBY3MwS2ZNdmNMVVM0Unh2Q3VtTTQ1cjQyZzFyd1BpbjdiQXJmL2ZzTE82bUt4NFlkaElETVpReld0Y25IRUkxeU1yemlPaXV4THRPMWpUQVdublNlS1QydHFyN05uNkJoOU1ERzY2WStpSWluMVdOU1ArTHQxWHV0ZGozSk8reG9RTVFQeWRaaGZCWGV6VTBIUTJ3dHhHcHc0TXM0UzE1SWxYNUxHV0d3V3lHWE1jY1Vnd29UQ3hURmJoMmRaNFZINzlWR0xFRUdSUVhGazU0QXZRS3RQaXVHMWNMOERaN1hKMkR5MUs2VFFlTkRPWHhZdjRTb3orQjBzQUtFcE1UazQrU2FqWDZLK0o5VDhYWVV0UzhPMFlmRlBWamZISGE2TkVkMjg3VXFJajJyZ0JRdW41Q1d4QnMxR1JuQWJndWdzMi9mUGpHMGZnUHpnUmM0eUN2TmxYOFdqSlJ5aHN1OVRUSk43dUdzTnZ6a1NiMmVpckJoRGhtb0NCamtMMmJzM09yNndqZzZwVDVaZjRoRHQxdEkwSTV6NWsrcUF1UmZ0YXdZZmpoV2JqTEtMSjk5VFZNZEQ2WkwrU3pzbUJDVjdOZWJveldEVE1oK0ZyT09vdkdPWW1JNW1qeEpndTFUVzZyNVdCVCtqMUowRTZiSG9rRDFqNFptQ1lEK3lQT1FtTzJtclEzRnQvY1ZmcEFpSXc5YkZIMGdRSG10OEJzbmZ0NjFVd3NYenErNmpDb2NYTjlDL0V2T25YU3M2blZTRkpFQS95dUJjSGs2cTlnampwZ0RtTUxHKzZacUdVY0VjM2RKdkx6bit6TU9Kd0wyOFlEMTdwS0lwVDZ3elhQRVRScFkvajR4aDJEL2hYSUVTR3E5NXk1ZmRPTDZsdUE9PSIgVmVyc2lvbj0iOS45Ij4NCiAgICA8VHlwZT5SdW50aW1lPC9UeXBlPg0KICAgIDxVc2VybmFtZT5Vc2VyTmFtZTwvVXNlcm5hbWU+DQogICAgPEVtYWlsPmVNYWlsQGhvc3QuY29tPC9FbWFpbD4NCiAgICA8T3JnYW5pemF0aW9uPk9yZ2FuaXphdGlvbjwvT3JnYW5pemF0aW9uPg0KICAgIDxMaWNlbnNlZERhdGU+MjAxNi0wMS0wMVQxMjowMDowMFo8L0xpY2Vuc2VkRGF0ZT4NCiAgICA8RXhwaXJlZERhdGU+MjA5OS0xMi0zMVQxMjowMDowMFo8L0V4cGlyZWREYXRlPg0KICAgIDxQcm9kdWN0cz4NCiAgICAgICAgPFByb2R1Y3Q+DQogICAgICAgICAgICA8TmFtZT5TcGlyZS5PZmZpY2UgUGxhdGludW08L05hbWU+DQogICAgICAgICAgICA8VmVyc2lvbj45Ljk5PC9WZXJzaW9uPg0KICAgICAgICAgICAgPFN1YnNjcmlwdGlvbj4NCiAgICAgICAgICAgICAgICA8TnVtYmVyT2ZQZXJtaXR0ZWREZXZlbG9wZXI+OTk5OTk8L051bWJlck9mUGVybWl0dGVkRGV2ZWxvcGVyPg0KICAgICAgICAgICAgICAgIDxOdW1iZXJPZlBlcm1pdHRlZFNpdGU+OTk5OTk8L051bWJlck9mUGVybWl0dGVkU2l0ZT4NCiAgICAgICAgICAgIDwvU3Vic2NyaXB0aW9uPg0KICAgICAgICA8L1Byb2R1Y3Q+DQogICAgPC9Qcm9kdWN0cz4NCiAgICA8SXNzdWVyPg0KICAgICAgICA8TmFtZT5Jc3N1ZXI8L05hbWU+DQogICAgICAgIDxFbWFpbD5pc3N1ZXJAaXNzdWVyLmNvbTwvRW1haWw+DQogICAgICAgIDxVcmw+aHR0cDovL3d3dy5pc3N1ZXIuY29tPC9Vcmw+DQogICAgPC9Jc3N1ZXI+DQo8L0xpY2Vuc2U+";
        //    Spire.License.LicenseProvider.SetLicenseKey(LData);
        //    var context = new dbEntities();

        //    var fileExist = context.FDMs.Where(q => q.FileName == fn).ToList();
        //    foreach (var x in fileExist)
        //    {
        //        context.FDMs.Remove(x);
        //    }
        //    context.SaveChanges();

        //    List<string> eventName = new List<string>();
        //    Workbook newBook = new Workbook();
        //    Worksheet worksheet = newBook.Worksheets[0];

        //    Workbook workbook = new Workbook();
        //    var path = HostingEnvironment.MapPath("~/upload");
        //    var dir = Path.Combine(path, fn + ".xlsx");
        //    workbook.LoadFromFile(dir);

        //    Worksheet sheet = workbook.Worksheets[0];

        //    for (int i = 1; i < sheet.Rows.Count(); i++)
        //    {
        //        for (int j = 1; j < sheet.Columns.Count(); j++)
        //        {
        //            eventName.Add(sheet.Range[i, j].DisplayedText);
        //        }
        //    }

        //    return eventName;
        //}

        public string Trim(string x)
        {
            string result = string.Empty;
            if (x != null)
                result = x?.Trim().Replace(" ", "").ToLower();
            return result;
        }

        public string DateConvert(DateTime? x)
        {
            var result = x?.ToString("yyyy-MMM-dd");
            return result;
        }




    }
}
