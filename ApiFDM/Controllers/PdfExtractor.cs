using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Web;
using System.IO;
using UglyToad.PdfPig;
using ApiFDM.Models;
using System.Web.UI;
using System.Windows.Media.Media3D;
using ApiFDM.Objects;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Web.Http.Results;
using System.Xml.Serialization;
using System.Runtime.Remoting.Messaging;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using WebGrease;





namespace ApiFDM.Controllers
{
    public class PdfExtractorController : ApiController
    {
        public static string ExtractText(string pdfPath, int minLengthPerPage = 500)
        {
            var builder = new StringBuilder();

            using (var document = PdfDocument.Open(pdfPath))
            {
                foreach (var page in document.GetPages())
                {
                    var text = page.Text?.Trim();
                    if (!string.IsNullOrEmpty(text) && text.Length > minLengthPerPage)
                    {
                        builder.AppendLine(text);
                        builder.AppendLine(new string('-', 80));
                    }
                }
            }

            return builder.ToString();
        }

        public static void SaveTextToFile(string pdfPath, string outputTxtPath)
        {
            var text = ExtractText(pdfPath);
            System.IO.File.WriteAllText(outputTxtPath, text, Encoding.UTF8);
        }

        [HttpGet]
        [Route("api/fdm/V2/pdf")]

        public async Task<DataResponse> DownloadPdfText()
        {
            // فایل PDF و مسیر ذخیره‌سازی متن
            string filePath = @"C:\samira\air\FDM\outout\a.pdf";
            string outputPath = @"C:\samira\air\FDM\outout\a.txt";

            // استخراج متن از PDF
            string extractedText = ExtractText(filePath);

            // ذخیره در فایل متنی
            System.IO.File.WriteAllText(outputPath, extractedText, Encoding.UTF8);

            // تقسیم متن به خطوط (برای تجزیه و تحلیل)
            string[] lines = extractedText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var parts = extractedText.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);



            // فقط صفحه اول را پردازش کن
            var plan = ParseFlightPlan(lines.Take(30).ToArray(), extractedText);

            Console.WriteLine($"Flight: {plan.FlightNumber}, From: {plan.From}, To: {plan.To}, Fuel: {plan.FuelOnBoard} kg");

            return new DataResponse()
            {
                //Data = extractedText,
                Data = new
                {
                    //extractedText,
                    plan,

                },
                IsSuccess = true
            };
        }

        public class FlightPlanHeader
        {
            public string LogNumber { get; set; }
            public string FlightNumber { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string AircraftType { get; set; }
            public double DOW { get; set; }  //Dry Operating Weight (DOW)
            public double ZFW { get; set; }  //Zero Fuel Weight (ZFW)
            public double MZFW { get; set; }  // Maximum Zero Fuel Weight (MZFW)
            public double ETOW { get; set; } // Estimated Takeoff Weight (ETOW)
            public double MTOW { get; set; }  //Maximum Takeoff Weight (MTOW)
            public double ELW { get; set; } //Estimated Landing Weight (ELW)
            public double MLW { get; set; }   // Maximum Landing Weight (MLW)
            public double FuelRequired { get; set; }
            public double FuelOnBoard { get; set; }
            public string Route { get; set; }
            public string NonStopComputed { get; set; }
            public string ETD { get; set; } //Estimated Time of Departure
            public string PROGS { get; set; }
            public string Register { get; set; }
            public double DestFuel { get; set; }
            public string DestTime { get; set; }
            public double DestDistance { get; set; }
            public string DestTimeArrive { get; set; }
            public double DestPayload { get; set; }
            public string ContPercent { get; set; } //contingency percentage
            public double ContFuel { get; set; }
            public string ContTime { get; set; }
        }


        public FlightPlanHeader ParseFlightPlan(string[] lines, string InputText)
        {
            var result = new FlightPlanHeader();

            if (InputText.Contains("Log Nr."))
                result.LogNumber = ExtractBetween(InputText, "Log Nr.:", "Page")?.Trim();

            if (InputText.Contains("DOW"))
                result.DOW = ExtractDoubleAfter(ExtractBetween(InputText, "DOW", "MZFW/ZFW"), ":");


            if (InputText.Contains("MZFW/ZFW"))
            {
                var (mzfw, zfw) = ExtractPairFromLine(ExtractBetween(InputText, "MZFW/ZFW", "PAX"));
                result.MZFW = mzfw;
                result.ZFW = zfw;
            }

            if (InputText.Contains("MTOW/ETOW"))
            {
                var (mtow, etow) = ExtractPairFromLine(ExtractBetween(InputText, "MTOW/ETOW", "CI"));
                result.MTOW = mtow;
                result.ETOW = etow;
            }
            if (InputText.Contains("MLW/ELW"))
            {
                var (mlw, elw) = ExtractPairFromLine(ExtractBetween(InputText, "MLW/ELW", "V2"));
                result.MLW = mlw;
                result.ELW = elw;
            }
            //if (InputText.Contains("PLAN"))
            //    result.FlightNumber = ExtractDoubleAfter(InputText, "PLAN");

            //if (InputText.Contains(" to "))
            //    result.To = ExtractStringAfter(InputText, " to ");

            if (InputText.Contains("PLAN"))
            {
                string ss = ExtractBetween(InputText, "PLAN", " to ");
                var parts = ss.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                result.FlightNumber = parts[0];
                result.From = parts[1];
            }
            if (InputText.Contains("NONSTOP COMPUTED"))
            {
                string ss = ExtractBetween(InputText, " to ", "NONSTOP COMPUTED");
                var parts = ss.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                result.To = parts[0];
                result.AircraftType = parts[1];
                result.NonStopComputed = ExtractStringAfter(InputText, "NONSTOP COMPUTED");

            }
            if (InputText.Contains(" ETD"))
                result.ETD = ExtractStringAfter(InputText, " ETD");

            if (InputText.Contains("PROGS"))
            {
                string ss = ExtractBetween(InputText, "PROGS", "FLIGHT CREW");
                var parts = ss.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                result.PROGS = parts[0];
                result.Register = parts[1];

            }
            if (InputText.Contains("DEST"))
            {
                string ss = ExtractBetween(InputText, "DEST", "CONT");
                var parts = ss.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                result.DestFuel = double.TryParse(parts[1], out var val) ? val : 0;
                result.DestTime = parts[2];
                result.DestDistance = double.TryParse(parts[3], out var val2) ? val2 : 0;

                result.DestTimeArrive = parts[5];
                result.DestPayload = double.TryParse(parts[6], out var val3) ? val3 : 0;

            }
            if (InputText.Contains("CONT"))
            {
                string ss = ExtractBetween(InputText, "CONT", "ALTN");
                var parts = ss.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                result.ContPercent = parts[0];
                result.ContFuel = double.TryParse(parts[1], out var val) ? val : 0;
                result.ContTime = parts[2];

            }

            foreach (var line in lines)
            {

                //if (line.Contains("DOW") && line.Contains(":"))
                //    result.DOW = ExtractDoubleAfter(line, ":");

                //if (line.Contains("REQD"))
                //    result.FuelRequired = ExtractDoubleAfter(line, "REQD");

                if (line.Contains("TOTAL") && line.Contains("FUEL ON BOARD"))
                    result.FuelOnBoard = ExtractDoubleAfter(line, "TOTAL");

                if (line.Trim().StartsWith("UDYZ "))
                    result.Route = line.Trim();
            }

            return result;
        }

        // ابزارهای کمکی:
        private string ExtractBetween(string line, string start, string end)
        {
            int i1 = line.IndexOf(start);
            int i2 = line.IndexOf(end);
            if (i1 >= 0 && i2 > i1)
            {
                return line.Substring(i1 + start.Length, i2 - (i1 + start.Length));
            }
            return null;
        }

        private string ExtractAfter(string line, string keyword)
        {
            int idx = line.IndexOf(keyword);
            if (idx >= 0)
                return line.Substring(idx + keyword.Length);
            return string.Empty;
        }
        private string ExtractStringAfter(string line, string keyword)
        {
            string segment = ExtractAfter(line, keyword);
            var parts = segment.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.FirstOrDefault();
        }

        private double ExtractDoubleAfter(string line, string keyword)
        {
            string segment = ExtractAfter(line, keyword);
            var parts = segment.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return double.TryParse(parts.FirstOrDefault(), out var val) ? val : 0;
        }

        private double ExtractDoubleFromLine(string line, string keyword)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Contains(keyword) && i + 1 < parts.Length)
                    return double.TryParse(parts[i + 1], out var val) ? val : 0;
            }
            return 0;
        }
        private (double first, double second) ExtractPairFromLine(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && double.TryParse(parts[0], out double first) && double.TryParse(parts[1], out double second))
                return (first, second);
            return (0, 0);
        }


        //////////XML////////////////////////////


        [XmlRoot("Flight")]

        public class Responce
        {
            public bool Succeed { get; set; }
        }


        //private static RoutePointEco ParseEco(XElement ecoElem)
        //{
        //    if (ecoElem == null)
        //        return null;

        //    return new RoutePointEco
        //    {
        //        OptSpeedFL = double.TryParse(ecoElem.Element("OptSpeedFL")?.Value, out var osf) ? osf : (double)0,
        //        SpeedGain = double.TryParse(ecoElem.Element("SpeedGain")?.Value, out var sg) ? sg : (byte)0,
        //        OptEcoFL = double.TryParse(ecoElem.Element("OptEcoFL")?.Value, out var oef) ? oef : (double)0,
        //        MoneyGain = double.TryParse(ecoElem.Element("MoneyGain")?.Value, out var mg) ? mg : (double)0,
        //        OptFuelFL = double.TryParse(ecoElem.Element("OptFuelFL")?.Value, out var off) ? off : (double)0,
        //        FuelGain = double.TryParse(ecoElem.Element("FuelGain")?.Value, out var fg) ? fg : (double)0
        //    };
        //}


        [HttpGet]
        [Route("api/fdm/V2/XML")]
        public async Task<DataResponse> ExtractXMLOFP()
        {
            var context = new PPSEntities();
            string xmlFilePath = @"C:\samira\air\Armenia\OFP\AMW222.xml";

            string fileContent = System.IO.File.ReadAllText(xmlFilePath);

            // پیدا کردن تگ اصلی Flight
            int startIndex = fileContent.IndexOf("<Flight");
            int endIndex = fileContent.LastIndexOf("</Flight>");

            if (startIndex >= 0 && endIndex > startIndex)
            {
                string flightXml = fileContent.Substring(startIndex, endIndex - startIndex + "</Flight>".Length);

                XDocument xmlDoc = XDocument.Parse(flightXml);
                XElement root = xmlDoc.Root;

                var flightData = new Models.pps_Flight(); //new pps_Flight();
                context.pps_Flight.Add(flightData);
                
                flightData.FlightLogID = root.Element("FlightLogID")?.Value;
                flightData.XmlID = root.Element("ID")?.Value;
                flightData.PPSName = root.Element("PPSName")?.Value;
                flightData.ACFTAIL = root.Element("ACFTAIL")?.Value;
                flightData.DEP = root.Element("DEP")?.Value;
                flightData.DEST = root.Element("DEST")?.Value;
                flightData.ALT1 = root.Element("ALT1")?.Value;
                flightData.ALT2 = root.Element("ALT2")?.Value;
                flightData.TOA = root.Element("TOA")?.Value;
                flightData.FMDID = root.Element("FMDID")?.Value;
                flightData.DESTSTDALT = root.Element("DESTSTDALT")?.Value;
                flightData.TIMECOMP = root.Element("TIMECOMP")?.Value;
                flightData.TIMECONT = root.Element("TIMECONT")?.Value;
                flightData.TIMEMIN = root.Element("TIMEMIN")?.Value;
                flightData.TIMEEXTRA = root.Element("TIMEEXTRA")?.Value;
                flightData.TIMELDG = root.Element("TIMELDG")?.Value;
                flightData.DestERA = root.Element("DestERA")?.Value;
                flightData.GL = root.Element("GL")?.Value;
                flightData.DISP = root.Element("DISP")?.Value;
                flightData.CustomerDataPPS = root.Element("CustomerDataPPS")?.Value;
                flightData.CustomerDataScheduled = root.Element("CustomerDataScheduled")?.Value;
                flightData.RouteName = root.Element("RouteName")?.Value;
                flightData.RouteRemark = root.Element("RouteRemark")?.Value;
                flightData.FuelPolicy = root.Element("FuelPolicy")?.Value;
                flightData.MFCI = root.Element("MFCI")?.Value;
                flightData.CruiseProfile = root.Element("CruiseProfile")?.Value;
                flightData.Climb = root.Element("Climb")?.Value;
                flightData.Descend = root.Element("Descend")?.Value;
                flightData.FuelPL = root.Element("FuelPL")?.Value;
                flightData.DescendWind = root.Element("DescendWind")?.Value;
                flightData.ClimbProfile = root.Element("ClimbProfile")?.Value;
                flightData.DescendProfile = root.Element("DescendProfile")?.Value;
                flightData.HoldProfile = root.Element("HoldProfile")?.Value;
                flightData.StepClimbProfile = root.Element("StepClimbProfile")?.Value;
                flightData.FuelContDef = root.Element("FuelContDef")?.Value;
                flightData.FuelAltDef = root.Element("FuelAltDef")?.Value;
                flightData.AmexsyStatus = root.Element("AmexsyStatus")?.Value;
                flightData.WeightUnit = root.Element("WeightUnit")?.Value;
                flightData.DEPMetar = root.Element("DEPMetar")?.Value;
                flightData.DESTMetar = root.Element("DESTMetar")?.Value;
                flightData.ALT1Metar = root.Element("ALT1Metar")?.Value;
                flightData.ALT2Metar = root.Element("ALT2Metar")?.Value;
                flightData.DEPIATA = root.Element("DEPIATA")?.Value;
                flightData.DESTIATA = root.Element("DESTIATA")?.Value;
                flightData.FlightSummary = root.Element("FlightSummary")?.Value;
                flightData.GUFI = root.Element("GUFI")?.Value;
                flightData.UnderloadFactor = root.Element("UnderloadFactor")?.Value;
                flightData.HWCorrection20KtsTime = root.Element("HWCorrection20KtsTime")?.Value;

                var atcDataElement = root.Element("ATCData");
                if (atcDataElement != null)
                {
                    flightData.ATCData_ATCID = atcDataElement.Element("ATCID")?.Value;
                    flightData.ATCData_ATCTOA = atcDataElement.Element("ATCTOA")?.Value;
                    flightData.ATCData_ATCRule = atcDataElement.Element("ATCRule")?.Value;
                    flightData.ATCData_ATCType = atcDataElement.Element("ATCType")?.Value;
                    flightData.ATCData_ATCNum = atcDataElement.Element("ATCNum")?.Value;
                    flightData.ATCData_ATCWake = atcDataElement.Element("ATCWake")?.Value;
                    flightData.ATCData_ATCEqui = atcDataElement.Element("ATCEqui")?.Value;
                    flightData.ATCData_ATCSSR = atcDataElement.Element("ATCSSR")?.Value;
                    flightData.ATCData_ATCDep = atcDataElement.Element("ATCDep")?.Value;
                    flightData.ATCData_ATCTime = atcDataElement.Element("ATCTime")?.Value;
                    flightData.ATCData_ATCSpeed = atcDataElement.Element("ATCSpeed")?.Value;
                    flightData.ATCData_ATCFL = atcDataElement.Element("ATCFL")?.Value;
                    flightData.ATCData_ATCRoute = atcDataElement.Element("ATCRoute")?.Value;
                    flightData.ATCData_ATCDest = atcDataElement.Element("ATCDest")?.Value;
                    flightData.ATCData_ATCEET = atcDataElement.Element("ATCEET")?.Value;
                    flightData.ATCData_ATCAlt1 = atcDataElement.Element("ATCAlt1")?.Value;
                    flightData.ATCData_ATCAlt2 = atcDataElement.Element("ATCAlt2")?.Value;
                    flightData.ATCData_ATCInfo = atcDataElement.Element("ATCInfo")?.Value;
                    flightData.ATCData_ATCEndu = atcDataElement.Element("ATCEndu")?.Value;
                    flightData.ATCData_ATCPers = atcDataElement.Element("ATCPers")?.Value;
                    flightData.ATCData_ATCRadi = atcDataElement.Element("ATCRadi")?.Value;
                    flightData.ATCData_ATCSurv = atcDataElement.Element("ATCSurv")?.Value;
                    flightData.ATCData_ATCJack = atcDataElement.Element("ATCJack")?.Value;
                    flightData.ATCData_ATCDing = atcDataElement.Element("ATCDing")?.Value;
                    flightData.ATCData_ATCCap = atcDataElement.Element("ATCCap")?.Value;
                    flightData.ATCData_ATCCover = atcDataElement.Element("ATCCover")?.Value;
                    flightData.ATCData_ATCColo = atcDataElement.Element("ATCColo")?.Value;
                    flightData.ATCData_ATCAcco = atcDataElement.Element("ATCAcco")?.Value;
                    flightData.ATCData_ATCRem = atcDataElement.Element("ATCRem")?.Value;
                    flightData.ATCData_ATCPIC = atcDataElement.Element("ATCPIC")?.Value;
                    flightData.ATCData_ATCCtot = atcDataElement.Element("ATCCtot")?.Value;
                }

                // تبدیل مقادیر عددی و تاریخ با بررسی
                flightData.PAX = double.TryParse(root.Element("PAX")?.Value, out var pax) ? (double?)pax : null;
                flightData.FUEL = double.TryParse(root.Element("FUEL")?.Value, out var fuel) ? (double?)fuel : null;
                flightData.LOAD = double.TryParse(root.Element("LOAD")?.Value, out var load) ? (double?)load : null;
                flightData.ValidHrs = double.TryParse(root.Element("ValidHrs")?.Value, out var validhrs) ? (double?)validhrs :null;
                flightData.MinFL = double.TryParse(root.Element("MinFL")?.Value, out var minfl) ? (double?)minfl :null;
                flightData.MaxFL = double.TryParse(root.Element("MaxFL")?.Value, out var maxfl) ? (double?)maxfl : null;
                flightData.FUELCOMP = double.TryParse(root.Element("FUELCOMP")?.Value, out var fcomp) ? (double?)fcomp : null;
                flightData.FUELCONT = double.TryParse(root.Element("FUELCONT")?.Value, out var fcont) ? (double?)fcont : null;
                flightData.FUELMIN = double.TryParse(root.Element("FUELMIN")?.Value, out var fmin) ? (double?)fmin : null;
                flightData.FUELTAXI = double.TryParse(root.Element("FUELTAXI")?.Value, out var ftaxi) ? (double?)ftaxi : null;
                flightData.TIMETAXI = double.TryParse(root.Element("TIMETAXI")?.Value, out var ttaxi) ? (double?)ttaxi : null;
                flightData.FUELEXTRA = double.TryParse(root.Element("FUELEXTRA")?.Value, out var fextra) ? (double?)fextra : null;
                flightData.FUELLDG = double.TryParse(root.Element("FUELLDG")?.Value, out var fldg) ? (double?)fldg : null;
                flightData.PCTCONT = double.TryParse(root.Element("PCTCONT")?.Value, out var pctc) ? (double?)pctc : null;
                flightData.ZFM = double.TryParse(root.Element("ZFM")?.Value, out var zfm) ? (double?)zfm : null;
                flightData.GCD = double.TryParse(root.Element("GCD")?.Value, out var gcd) ? (double?)gcd : null;
                flightData.ESAD = double.TryParse(root.Element("ESAD")?.Value, out var esad) ? (double?)esad : null;
                flightData.FUELBIAS = double.TryParse(root.Element("FUELBIAS")?.Value, out var fbias) ? (double?)fbias : null;
                flightData.SCHBLOCKTIME = double.TryParse(root.Element("SCHBLOCKTIME")?.Value, out var sch) ? (double?)sch : null;
                flightData.FUELMINTO = double.TryParse(root.Element("FUELMINTO")?.Value, out var fminto) ? (double?)fminto : null;
                flightData.TIMEMINTO = double.TryParse(root.Element("TIMEMINTO")?.Value, out var tminto) ? (double?)tminto : null;
                flightData.ARAMP = double.TryParse(root.Element("ARAMP")?.Value, out var aramp) ? (double?)aramp : null;
                flightData.TIMEACT = double.TryParse(root.Element("TIMEACT")?.Value, out var tact) ? (double?)tact : null;
                flightData.FUELACT = double.TryParse(root.Element("FUELACT")?.Value, out var fact) ? (double?)fact : null;
                flightData.TrafficLoad = double.TryParse(root.Element("TrafficLoad")?.Value, out var tload) ? (double?)tload : null;
                flightData.Fl = double.TryParse(root.Element("Fl")?.Value, out var fl) ? (double?)fl : null;
                flightData.RouteDistNM = double.TryParse(root.Element("RouteDistNM")?.Value, out var rnm) ? (double?)rnm : null;
                flightData.EmptyWeight = double.TryParse(root.Element("EmptyWeight")?.Value, out var eweight) ? (double?)eweight : null;
                flightData.TotalDistance = double.TryParse(root.Element("TotalDistance")?.Value, out var tdist) ? (double?)tdist : null;
                flightData.AltDist = double.TryParse(root.Element("AltDist")?.Value, out var adist) ? (double?)adist : null;
                flightData.DestTime = double.TryParse(root.Element("DestTime")?.Value, out var dtime) ? (double?)dtime : null;
                flightData.AltTime = double.TryParse(root.Element("AltTime")?.Value, out var atime) ? (double?)atime : null;
                flightData.AltFuel = double.TryParse(root.Element("AltFuel")?.Value, out var afuel) ? (double?)afuel : null;
                flightData.HoldTime = double.TryParse(root.Element("HoldTime")?.Value, out var htime) ? (double?)htime : null;
                flightData.ReserveTime = double.TryParse(root.Element("ReserveTime")?.Value, out var rtime) ? (double?)rtime : null;
                flightData.Cargo = double.TryParse(root.Element("Cargo")?.Value, out var cargo) ? (double?)cargo : null;
                flightData.ActTOW = double.TryParse(root.Element("ActTOW")?.Value, out var atow) ? (double?)atow : null;
                flightData.TripFuel = double.TryParse(root.Element("TripFuel")?.Value, out var tfuel) ? (double?)tfuel : null;
                flightData.HoldFuel = double.TryParse(root.Element("HoldFuel")?.Value, out var hfuel) ? (double?)hfuel : null;
                flightData.Elw = double.TryParse(root.Element("Elw")?.Value, out var elw) ? (double?)elw : null;
                flightData.Alt2Time = double.TryParse(root.Element("Alt2Time")?.Value, out var alt2t) ? (double?)alt2t : null;
                flightData.Alt2Fuel = double.TryParse(root.Element("Alt2Fuel")?.Value, out var alt2f) ? (double?)alt2f : null;
                flightData.MaxTOM = double.TryParse(root.Element("MaxTOM")?.Value, out var maxTom) ? (double?)maxTom : null;
                flightData.MaxLM = double.TryParse(root.Element("MaxLM")?.Value, out var maxLM) ? (double?)maxLM : null;
                flightData.MaxZFM = double.TryParse(root.Element("MaxZFM")?.Value, out var maxZfm) ? (double?)maxZfm : null;
                flightData.TempTopOfClimb = double.TryParse(root.Element("TempTopOfClimb")?.Value, out var tempTop) ? (double?)tempTop : null;
                flightData.AvgTrack = double.TryParse(root.Element("AvgTrack")?.Value, out var avgTrk) ? (double?)avgTrk : null;
                flightData.WindComponent = double.TryParse(root.Element("WindComponent")?.Value, out var wind) ? (double?)wind : null;
                flightData.FinalReserveMinutes = double.TryParse(root.Element("FinalReserveMinutes")?.Value, out var reservm) ? (double?)reservm : null;
                flightData.FinalReserveFuel = double.TryParse(root.Element("FinalReserveFuel")?.Value, out var reservf) ? (double?)reservf : null;
                flightData.AddFuelMinutes = double.TryParse(root.Element("AddFuelMinutes")?.Value, out var addfm) ? (double?)addfm : null;
                flightData.AddFuel = double.TryParse(root.Element("AddFuel")?.Value, out var addf) ? (double?)addf : null;
                flightData.FuelINCRBurn = double.TryParse(root.Element("FuelINCRBurn")?.Value, out var flincrb) ? (double?)flincrb : null;
                flightData.FMRI = double.TryParse(root.Element("FMRI")?.Value, out var fmri) ? (double?)fmri : null;
                flightData.MaxRampWeight = double.TryParse(root.Element("MaxRampWeight")?.Value, out var maxrw) ? (double?)maxrw : null;
                flightData.AvgISA = double.TryParse(root.Element("AvgISA")?.Value, out var avgisa) ? (double?)avgisa : null;
                flightData.HWCorrection20KtsFuel = double.TryParse(root.Element("HWCorrection20KtsFuel")?.Value, out var hc20kf) ? (double?)hc20kf : null;
                flightData.Correction1TON = double.TryParse(root.Element("Correction1TON")?.Value, out var c1ton) ? (double?)c1ton : null;
                flightData.Correction2TON = double.TryParse(root.Element("Correction2TON")?.Value, out var c2ton) ? (double?)c2ton : null;
                flightData.StructuralTOM = double.TryParse(root.Element("StructuralTOM")?.Value, out var sltom) ? (double?)sltom : null;
                flightData.FW1 = double.TryParse(root.Element("FW1")?.Value, out var fw1) ? (double?)fw1 : null;
                flightData.FW2 = double.TryParse(root.Element("FW2")?.Value, out var fw2) ? (double?)fw2 : null;
                flightData.FW3 = double.TryParse(root.Element("FW3")?.Value, out var fw3) ? (double?)fw3 : null;
                flightData.FW4 = double.TryParse(root.Element("FW4")?.Value, out var fw4) ? (double?)fw4 : null;
                flightData.FW5 = double.TryParse(root.Element("FW5")?.Value, out var fw5) ? (double?)fw5 : null;
                flightData.FW6 = double.TryParse(root.Element("FW6")?.Value, out var fw6) ? (double?)fw6 : null;
                flightData.FW7 = double.TryParse(root.Element("FW7")?.Value, out var fw7) ? (double?)fw7 : null;
                flightData.FW8 = double.TryParse(root.Element("FW8")?.Value, out var fw8) ? (double?)fw8 : null;
                flightData.FW9 = double.TryParse(root.Element("FW9")?.Value, out var fw9) ? (double?)fw9 : null;
                flightData.TOTALPAXWEIGHT = double.TryParse(root.Element("TOTALPAXWEIGHT")?.Value, out var tpaxw) ? (double?)tpaxw : null;
                flightData.Alt2Dist = double.TryParse(root.Element("Alt2Dist")?.Value, out var alt2dist) ? (double?)alt2dist : null;
                flightData.AircraftFuelBias = double.TryParse(root.Element("AircraftFuelBias")?.Value, out var afb) ? (double?)afb : null;
                flightData.MelFuelBias = double.TryParse(root.Element("MelFuelBias")?.Value, out var mfb) ? (double?)mfb : null;


                //تاریخ ها
                flightData.STD = DateTime.TryParse(root.Element("STD")?.Value, out var std) ? (DateTime?)std : null;
                flightData.STA = DateTime.TryParse(root.Element("STA")?.Value, out var sta) ? (DateTime?)sta :null;
                flightData.ETA = DateTime.TryParse(root.Element("ETA")?.Value, out var eta) ? (DateTime?)eta : null;
                flightData.LastEditDate = DateTime.TryParse(root.Element("LastEditDate")?.Value, out var lastEdit) ? (DateTime?)lastEdit : null;
                flightData.LatestFlightPlanDate = DateTime.TryParse(root.Element("LatestFlightPlanDate")?.Value, out var planDate) ? (DateTime?)planDate : null;
                flightData.LatestDocumentUploadDate = DateTime.TryParse(root.Element("LatestDocumentUploadDate")?.Value, out var uploadDate) ? (DateTime?)uploadDate : null;
                flightData.WeatherObsTime = DateTime.TryParse(root.Element("WeatherObsTime")?.Value, out var obsTime) ? (DateTime?)obsTime : null;
                flightData.WeatherPlanTime = DateTime.TryParse(root.Element("WeatherPlanTime")?.Value, out var planTime) ? (DateTime?)planTime : null;

                if (root.Element("EROPSAltApts")!=null)
                {
                    foreach(var s in root.Element("EROPSAltApts")?.Elements("string"))
                    {
                        var _txt = new Models.pps_String();
                        _txt.Txt=s?.Value       ;
                        _txt.StringType = "EROPSAltApts";
                        flightData.pps_String.Add( _txt);
                        //flightData.Strings.Add(_txt);
                    }
                }
                if (root.Element("AdequateApt") != null)
                {
                    foreach (var s in root.Element("AdequateApt")?.Elements("string"))
                    {
                        var _txt = new Models.pps_String();
                        _txt.Txt = s?.Value;
                        _txt.StringType = "AdequateApt";
                        flightData.pps_String.Add(_txt);
                    }
                }
                if (root.Element("FIR") != null)
                {
                    foreach (var s in root.Element("FIR")?.Elements("string"))
                    {
                        var _txt = new Models.pps_String();
                        _txt.Txt = s?.Value;
                        _txt.StringType = "FIR";
                        flightData.pps_String.Add(_txt);
                    }
                }
                if (root.Element("AltApts") != null)
                {
                    foreach (var s in root.Element("AltApts")?.Elements("string"))
                    {
                        var _txt = new Models.pps_String();
                        _txt.Txt = s?.Value;
                        _txt.StringType = "AltApts";
                        flightData.pps_String.Add(_txt);
                    }
                }
                if (root.Element("Messages") != null)
                {
                    foreach (var s in root.Element("Messages")?.Elements("string"))
                    {
                        var _txt = new Models.pps_String();
                        _txt.Txt = s?.Value;
                        _txt.StringType = "Messages";
                        flightData.pps_String.Add(_txt);
                    }
                }

                var holdingElement = root.Element("Holding");
                if (holdingElement != null)
                {
                    flightData.Holding_Fuel = double.TryParse(holdingElement.Element("Fuel")?.Value, out var hf) ? (double?)hf :null;
                    flightData.Holding_Minutes = double.TryParse(holdingElement.Element("Minutes")?.Value, out var hm) ? (double?)hm : null;
                    flightData.Holding_Profile = holdingElement.Element("Profile")?.Value ?? string.Empty;
                    flightData.Holding_Specification = holdingElement.Element("Specification")?.Value ?? string.Empty;
                    flightData.Holding_FuelFlowType = holdingElement.Element("FuelFlowType")?.Value ?? string.Empty;
                }

                var crewsElement = root.Element("Crews");
                if (crewsElement != null)
                {

                    flightData.pps_Crew=crewsElement.Elements("Crew")
                   .Select(p => new Models.pps_Crew
                   {
                       XmlID = p.Element("ID")?.Value,
                       CrewType = p.Element("CrewType")?.Value,
                       XmlType = "Crew",
                       CrewName = p.Element("CrewName")?.Value,
                       Initials = p.Element("Initials")?.Value,
                       GSM = p.Element("GSM")?.Value,
                       Mass = p.Element("Mass")?.Value

                   }).ToList() ?? new List<Models.pps_Crew>();

                }
                 
                //----------RoutePoints---------------------------

                flightData.pps_RoutePoint = root.Element("RoutePoints")?.Elements("RoutePoint")
                   .Select(p => new Models.pps_RoutePoint
                   {
                       XmlID = p.Element("ID")?.Value,
                       RoutePointType="ROUTE",
                       IDENT = p.Element("IDENT")?.Value,
                       FL = double.TryParse(p.Element("FL")?.Value, out var fl1) ? (double?)fl1 : null,
                       Wind = double.TryParse(p.Element("Wind")?.Value, out var wind1) ? (double?)wind1 : null,
                       Vol = double.TryParse(p.Element("Vol")?.Value, out var vol) ? (double?)vol : null,
                       ISA = double.TryParse(p.Element("ISA")?.Value, out var isa) ? (double?)isa :null,
                       LegTime = double.TryParse(p.Element("LegTime")?.Value, out var legTime) ? (double?)legTime : null,
                       LegCourse = double.TryParse(p.Element("LegCourse")?.Value, out var legCourse) ? (double?)legCourse : null,
                       LegDistance = double.TryParse(p.Element("LegDistance")?.Value, out var legDist) ? (double?)legDist : null,
                       LegCAT = double.TryParse(p.Element("LegCAT")?.Value, out var legCat) ? (double?)legCat :null,
                       LegName = p.Element("LegName")?.Value,
                       LegAWY = p.Element("LegAWY")?.Value,
                       FuelUsed = double.TryParse(p.Element("FuelUsed")?.Value, out var fuelUsed) ? (double?)fuelUsed : null,
                       FuelFlow = double.TryParse(p.Element("FuelFlow")?.Value, out var fuelFlow) ? (double?)fuelFlow : null,
                       LAT = double.TryParse(p.Element("LAT")?.Value, out var lat) ? (double?)lat : null,
                       LON = double.TryParse(p.Element("LON")?.Value, out var lon) ? (double?)lon : null,
                       VARIATION = double.TryParse(p.Element("VARIATION")?.Value, out var varr) ? (double?)varr : null,
                       ACCDIST = double.TryParse(p.Element("ACCDIST")?.Value, out var accd) ? (double?)accd : null,
                       ACCTIME = double.TryParse(p.Element("ACCTIME")?.Value, out var acct) ? (double?)acct : null,
                       MagCourse = double.TryParse(p.Element("MagCourse")?.Value, out var magc) ? (double?)magc : null,
                       TrueAirSpeed = double.TryParse(p.Element("TrueAirSpeed")?.Value, out var tas) ? (double?)tas : null,
                       GroundSpeed = double.TryParse(p.Element("GroundSpeed")?.Value, out var gs) ? (double?)gs : null,
                       FuelRemaining = double.TryParse(p.Element("FuelRemaining")?.Value, out var fr) ? (double?)fr : null,
                       DistRemaining = double.TryParse(p.Element("DistRemaining")?.Value, out var dr) ? (double?)dr : null,
                       TimeRemaining = double.TryParse(p.Element("TimeRemaining")?.Value, out var tr) ? (double?)tr : null,
                       MinReqFuel = double.TryParse(p.Element("MinReqFuel")?.Value, out var mrf) ? (double?)mrf : null,
                       FuelFlowPerEng = double.TryParse(p.Element("FuelFlowPerEng")?.Value, out var ffe) ? (double?)ffe : null,
                       Temperature = double.TryParse(p.Element("Temperature")?.Value, out var temp) ? (double?)temp : null,
                       MORA = double.TryParse(p.Element("MORA")?.Value, out var mora) ? (double?)mora : 0,
                       Frequency = double.TryParse(p.Element("Frequency")?.Value, out var freq) ? (double?)freq : null,
                       WindComponent = double.TryParse(p.Element("WindComponent")?.Value, out var wc) ? (double?)wc : null,
                       MinimumEnrouteAltitude = double.TryParse(p.Element("MinimumEnrouteAltitude")?.Value, out var mea) ? (double?)mea : null,
                       MagneticHeading = double.TryParse(p.Element("MagneticHeading")?.Value, out var magh) ? (double?)magh : null,
                       TrueHeading = double.TryParse(p.Element("TrueHeading")?.Value, out var trueh) ? (double?)trueh : null,
                       MagneticTrack = double.TryParse(p.Element("MagneticTrack")?.Value, out var magt) ? (double?)magt : null,
                       TrueTrack = double.TryParse(p.Element("TrueTrack")?.Value, out var truet) ? (double?)truet : null,
                       HLAEntryExit = p.Element("HLAEntryExit")?.Value,
                       FIR = p.Element("FIR")?.Value,
                       ClimbDescent = p.Element("ClimbDescent")?.Value,
                       LegFuel = double.TryParse(p.Element("LegFuel")?.Value, out var lf) ? (double?)lf : null,

                       Eco_OptSpeedFL = double.TryParse(p.Element("Eco")?.Element("OptSpeedFL")?.Value, out var optspeed) ? (double?)optspeed : null,
                       Eco_SpeedGain = double.TryParse(p.Element("Eco")?.Element("SpeedGain")?.Value, out var speed) ? (double?)speed : null,
                       Eco_OptEcoFL = double.TryParse(p.Element("Eco")?.Element("OptEcoFL")?.Value, out var opteco) ? (double?)opteco : null,
                       Eco_MoneyGain = double.TryParse(p.Element("Eco")?.Element("MoneyGain")?.Value, out var money) ? (double?)money : null,
                       Eco_OptFuelFL = double.TryParse(p.Element("Eco")?.Element("OptFuelFL")?.Value, out var optfuel) ? (double?)optfuel : null,
                       Eco_FuelGain = double.TryParse(p.Element("Eco")?.Element("FuelGain")?.Value, out var fuelgain) ? (double?)fuelgain : null,

                   }).ToList() ?? new List<Models.pps_RoutePoint>();

                var routePointsElement = root.Element("Alt1Points");
                if (routePointsElement != null)
                {
                    var pointElements = routePointsElement.Elements("RoutePoint");
                    foreach (var p in pointElements)
                    {
                        var point = new Models.pps_RoutePoint
                        {
                            XmlID = p.Element("ID")?.Value,
                            RoutePointType = "ALT1",
                            IDENT = p.Element("IDENT")?.Value,
                            FL = double.TryParse(p.Element("FL")?.Value, out var fl1) ? (double?)fl1 : null,
                            Wind = double.TryParse(p.Element("Wind")?.Value, out var wind1) ? (double?)wind1 : null,
                            Vol = double.TryParse(p.Element("Vol")?.Value, out var vol) ? (double?)vol : null,
                            ISA = double.TryParse(p.Element("ISA")?.Value, out var isa) ? (double?)isa : null,
                            LegTime = double.TryParse(p.Element("LegTime")?.Value, out var legTime) ? (double?)legTime : null,
                            LegCourse = double.TryParse(p.Element("LegCourse")?.Value, out var legCourse) ? (double?)legCourse : null,
                            LegDistance = double.TryParse(p.Element("LegDistance")?.Value, out var legDist) ? (double?)legDist : null,
                            LegCAT = double.TryParse(p.Element("LegCAT")?.Value, out var legCat) ? (double?)legCat : null,
                            LegName = p.Element("LegName")?.Value,
                            LegAWY = p.Element("LegAWY")?.Value,
                            FuelUsed = double.TryParse(p.Element("FuelUsed")?.Value, out var fuelUsed) ? (double?)fuelUsed : null,
                            FuelFlow = double.TryParse(p.Element("FuelFlow")?.Value, out var fuelFlow) ? (double?)fuelFlow : null,
                            LAT = double.TryParse(p.Element("LAT")?.Value, out var lat) ? (double?)lat : null,
                            LON = double.TryParse(p.Element("LON")?.Value, out var lon) ? (double?)lon : null,
                            VARIATION = double.TryParse(p.Element("VARIATION")?.Value, out var varr) ? (double?)varr : null,
                            ACCDIST = double.TryParse(p.Element("ACCDIST")?.Value, out var accd) ? (double?)accd : null,
                            ACCTIME = double.TryParse(p.Element("ACCTIME")?.Value, out var acct) ? (double?)acct : null,
                            MagCourse = double.TryParse(p.Element("MagCourse")?.Value, out var magc) ? (double?)magc : null,
                            TrueAirSpeed = double.TryParse(p.Element("TrueAirSpeed")?.Value, out var tas) ? (double?)tas : null,
                            GroundSpeed = double.TryParse(p.Element("GroundSpeed")?.Value, out var gs) ? (double?)gs : null,
                            FuelRemaining = double.TryParse(p.Element("FuelRemaining")?.Value, out var fr) ? (double?)fr : null,
                            DistRemaining = double.TryParse(p.Element("DistRemaining")?.Value, out var dr) ? (double?)dr : null,
                            TimeRemaining = double.TryParse(p.Element("TimeRemaining")?.Value, out var tr) ? (double?)tr : null,
                            MinReqFuel = double.TryParse(p.Element("MinReqFuel")?.Value, out var mrf) ? (double?)mrf : null,
                            FuelFlowPerEng = double.TryParse(p.Element("FuelFlowPerEng")?.Value, out var ffe) ? (double?)ffe : null,
                            Temperature = double.TryParse(p.Element("Temperature")?.Value, out var temp) ? (double?)temp : null,
                            MORA = double.TryParse(p.Element("MORA")?.Value, out var mora) ? (double?)mora : 0,
                            Frequency = double.TryParse(p.Element("Frequency")?.Value, out var freq) ? (double?)freq : null,
                            WindComponent = double.TryParse(p.Element("WindComponent")?.Value, out var wc) ? (double?)wc : null,
                            MinimumEnrouteAltitude = double.TryParse(p.Element("MinimumEnrouteAltitude")?.Value, out var mea) ? (double?)mea : null,
                            MagneticHeading = double.TryParse(p.Element("MagneticHeading")?.Value, out var magh) ? (double?)magh : null,
                            TrueHeading = double.TryParse(p.Element("TrueHeading")?.Value, out var trueh) ? (double?)trueh : null,
                            MagneticTrack = double.TryParse(p.Element("MagneticTrack")?.Value, out var magt) ? (double?)magt : null,
                            TrueTrack = double.TryParse(p.Element("TrueTrack")?.Value, out var truet) ? (double?)truet : null,
                            HLAEntryExit = p.Element("HLAEntryExit")?.Value,
                            FIR = p.Element("FIR")?.Value,
                            ClimbDescent = p.Element("ClimbDescent")?.Value,
                            LegFuel = double.TryParse(p.Element("LegFuel")?.Value, out var lf) ? (double?)lf : null,

                            Eco_OptSpeedFL = double.TryParse(p.Element("Eco")?.Element("OptSpeedFL")?.Value, out var optspeed) ? (double?)optspeed : null,
                            Eco_SpeedGain = double.TryParse(p.Element("Eco")?.Element("SpeedGain")?.Value, out var speed) ? (double?)speed : null,
                            Eco_OptEcoFL = double.TryParse(p.Element("Eco")?.Element("OptEcoFL")?.Value, out var opteco) ? (double?)opteco : null,
                            Eco_MoneyGain = double.TryParse(p.Element("Eco")?.Element("MoneyGain")?.Value, out var money) ? (double?)money : null,
                            Eco_OptFuelFL = double.TryParse(p.Element("Eco")?.Element("OptFuelFL")?.Value, out var optfuel) ? (double?)optfuel : null,
                            Eco_FuelGain = double.TryParse(p.Element("Eco")?.Element("FuelGain")?.Value, out var fuelgain) ? (double?)fuelgain : null,

                        };
                        flightData.pps_RoutePoint.Add(point);
                    }

                }

                var routePoints2 = root.Element("Alt2Points");
                if (routePoints2 != null)
                {
                    var pointElements = routePoints2.Elements("RoutePoint");
                    foreach (var p in pointElements)
                    {

                        var point = new Models.pps_RoutePoint
                        {
                            XmlID = p.Element("ID")?.Value,
                            RoutePointType = "ALT2",
                            IDENT = p.Element("IDENT")?.Value,
                            FL = double.TryParse(p.Element("FL")?.Value, out var fl1) ? (double?)fl1 : null,
                            Wind = double.TryParse(p.Element("Wind")?.Value, out var wind1) ? (double?)wind1 : null,
                            Vol = double.TryParse(p.Element("Vol")?.Value, out var vol) ? (double?)vol : null,
                            ISA = double.TryParse(p.Element("ISA")?.Value, out var isa) ? (double?)isa : null,
                            LegTime = double.TryParse(p.Element("LegTime")?.Value, out var legTime) ? (double?)legTime : null,
                            LegCourse = double.TryParse(p.Element("LegCourse")?.Value, out var legCourse) ? (double?)legCourse : null,
                            LegDistance = double.TryParse(p.Element("LegDistance")?.Value, out var legDist) ? (double?)legDist : null,
                            LegCAT = double.TryParse(p.Element("LegCAT")?.Value, out var legCat) ? (double?)legCat : null,
                            LegName = p.Element("LegName")?.Value,
                            LegAWY = p.Element("LegAWY")?.Value,
                            FuelUsed = double.TryParse(p.Element("FuelUsed")?.Value, out var fuelUsed) ? (double?)fuelUsed : null,
                            FuelFlow = double.TryParse(p.Element("FuelFlow")?.Value, out var fuelFlow) ? (double?)fuelFlow : null,
                            LAT = double.TryParse(p.Element("LAT")?.Value, out var lat) ? (double?)lat : null,
                            LON = double.TryParse(p.Element("LON")?.Value, out var lon) ? (double?)lon : null,
                            VARIATION = double.TryParse(p.Element("VARIATION")?.Value, out var varr) ? (double?)varr : null,
                            ACCDIST = double.TryParse(p.Element("ACCDIST")?.Value, out var accd) ? (double?)accd : null,
                            ACCTIME = double.TryParse(p.Element("ACCTIME")?.Value, out var acct) ? (double?)acct : null,
                            MagCourse = double.TryParse(p.Element("MagCourse")?.Value, out var magc) ? (double?)magc : null,
                            TrueAirSpeed = double.TryParse(p.Element("TrueAirSpeed")?.Value, out var tas) ? (double?)tas : null,
                            GroundSpeed = double.TryParse(p.Element("GroundSpeed")?.Value, out var gs) ? (double?)gs : null,
                            FuelRemaining = double.TryParse(p.Element("FuelRemaining")?.Value, out var fr) ? (double?)fr : null,
                            DistRemaining = double.TryParse(p.Element("DistRemaining")?.Value, out var dr) ? (double?)dr : null,
                            TimeRemaining = double.TryParse(p.Element("TimeRemaining")?.Value, out var tr) ? (double?)tr : null,
                            MinReqFuel = double.TryParse(p.Element("MinReqFuel")?.Value, out var mrf) ? (double?)mrf : null,
                            FuelFlowPerEng = double.TryParse(p.Element("FuelFlowPerEng")?.Value, out var ffe) ? (double?)ffe : null,
                            Temperature = double.TryParse(p.Element("Temperature")?.Value, out var temp) ? (double?)temp : null,
                            MORA = double.TryParse(p.Element("MORA")?.Value, out var mora) ? (double?)mora : 0,
                            Frequency = double.TryParse(p.Element("Frequency")?.Value, out var freq) ? (double?)freq : null,
                            WindComponent = double.TryParse(p.Element("WindComponent")?.Value, out var wc) ? (double?)wc : null,
                            MinimumEnrouteAltitude = double.TryParse(p.Element("MinimumEnrouteAltitude")?.Value, out var mea) ? (double?)mea : null,
                            MagneticHeading = double.TryParse(p.Element("MagneticHeading")?.Value, out var magh) ? (double?)magh : null,
                            TrueHeading = double.TryParse(p.Element("TrueHeading")?.Value, out var trueh) ? (double?)trueh : null,
                            MagneticTrack = double.TryParse(p.Element("MagneticTrack")?.Value, out var magt) ? (double?)magt : null,
                            TrueTrack = double.TryParse(p.Element("TrueTrack")?.Value, out var truet) ? (double?)truet : null,
                            HLAEntryExit = p.Element("HLAEntryExit")?.Value,
                            FIR = p.Element("FIR")?.Value,
                            ClimbDescent = p.Element("ClimbDescent")?.Value,
                            LegFuel = double.TryParse(p.Element("LegFuel")?.Value, out var lf) ? (double?)lf : null,

                            Eco_OptSpeedFL = double.TryParse(p.Element("Eco")?.Element("OptSpeedFL")?.Value, out var optspeed) ? (double?)optspeed : null,
                            Eco_SpeedGain = double.TryParse(p.Element("Eco")?.Element("SpeedGain")?.Value, out var speed) ? (double?)speed : null,
                            Eco_OptEcoFL = double.TryParse(p.Element("Eco")?.Element("OptEcoFL")?.Value, out var opteco) ? (double?)opteco : null,
                            Eco_MoneyGain = double.TryParse(p.Element("Eco")?.Element("MoneyGain")?.Value, out var money) ? (double?)money : null,
                            Eco_OptFuelFL = double.TryParse(p.Element("Eco")?.Element("OptFuelFL")?.Value, out var optfuel) ? (double?)optfuel : null,
                            Eco_FuelGain = double.TryParse(p.Element("Eco")?.Element("FuelGain")?.Value, out var fuelgain) ? (double?)fuelgain : null,

                        };
                        flightData.pps_RoutePoint.Add(point);

                    }
                }

//-------------------------------------------------

                var overflightCostElement = root.Element("OverflightCost");
                if (overflightCostElement != null)
                {

                    flightData.OverflightCost_Currency = overflightCostElement.Element("Currency")?.Value;
                    flightData.OverflightCost_TotalOverflightCost = double.TryParse(overflightCostElement.Element("TotalOverflightCost")?.Value, out var tlfc) ? tlfc : 0;
                    flightData.OverflightCost_TotalTerminalCost = double.TryParse(overflightCostElement.Element("TotalTerminalCost")?.Value, out var ttfc) ? ttfc : 0;
                    flightData.pps_FIROverflightCost = overflightCostElement.Element("Cost")?.Elements("FIROverflightCost")
                           .Select(f => new Models.pps_FIROverflightCost
                           {
                               FIR = f.Element("FIR")?.Value,
                               Distance = double.TryParse(f.Element("Distance")?.Value, out var dist) ? dist : (double?)null,
                               Cost = double.TryParse(f.Element("Cost")?.Value, out var c) ? c : (double?) null
                           })
                            .ToList() ?? new List<Models.pps_FIROverflightCost>();
                }

 //---------------------------------------------------
              
                var localTimeElement = root.Element("LocalTime");
                if (localTimeElement != null)
                {
                    var dep = localTimeElement.Element("Departure");
                    var dest = localTimeElement.Element("Destination");
                    flightData.LocalTime_Departure_ETD = DateTime.TryParse(dep.Element("ETD")?.Value, out var a1) ? (DateTime?)a1 : null;
                    flightData.LocalTime_Departure_STD = DateTime.TryParse(dep.Element("STD")?.Value, out var a2) ? (DateTime?)a2 : null;
                    flightData.LocalTime_Departure_Sunrise = DateTime.TryParse(dep.Element("Sunrise")?.Value, out var a3) ? (DateTime?)a3 : null;
                    flightData.LocalTime_Departure_Sunset = DateTime.TryParse(dep.Element("Sunset")?.Value, out var a4) ? (DateTime?)a4 : null;
                    flightData.LocalTime_Destination_ETA = DateTime.TryParse(dest.Element("ETA")?.Value, out var a5) ? (DateTime?)a5 : null;
                    flightData.LocalTime_Destination_STA = DateTime.TryParse(dest.Element("STA")?.Value, out var a6) ? (DateTime?)a6 : null;
                    flightData.LocalTime_Destination_Sunrise = DateTime.TryParse(dest.Element("Sunrise")?.Value, out var a7) ? (DateTime?)a7 : null;
                    flightData.LocalTime_Destination_Sunset = DateTime.TryParse(dest.Element("Sunset")?.Value, out var a8) ? (DateTime?)a8 : null;
                }

                /// --------------------TAF--------------------
                flightData.pps_TAF = new List<Models.pps_TAF>();
                var tafElement = root.Element("DEPTAF");
                if (tafElement != null)
                {
                    //flightData.DEPTAF = new pps_TAF
                    var _taf = new Models.pps_TAF()
                    {
                        Type = tafElement.Element("Type")?.Value,
                        XmlType = "DEPTAF",
                        Text = tafElement.Element("Text")?.Value,
                        ICAO = tafElement.Element("ICAO")?.Value,
                        ForecastTime = DateTime.TryParse(tafElement.Element("ForecastTime")?.Value, out var ft) ? (DateTime?)ft : null,
                        ForecastStartTime = DateTime.TryParse(tafElement.Element("ForecastStartTime")?.Value, out var fst) ? (DateTime?)fst :null,
                        ForecastEndTime = DateTime.TryParse(tafElement.Element("ForecastEndTime")?.Value, out var fet) ? (DateTime?)fet : null
                    };
                    flightData.pps_TAF.Add(_taf);
                }

                var destTafElement = root.Element("DESTTAF");
                if (destTafElement != null)
                {
                    var _taf = new Models.pps_TAF()
                    {
                        Type = destTafElement.Element("Type")?.Value,
                        XmlType = "DESTTAF",
                        Text = destTafElement.Element("Text")?.Value,
                        ICAO = destTafElement.Element("ICAO")?.Value,
                        ForecastTime = DateTime.TryParse(destTafElement.Element("ForecastTime")?.Value, out var ft) ? (DateTime?)ft : null,
                        ForecastStartTime = DateTime.TryParse(destTafElement.Element("ForecastStartTime")?.Value, out var fst) ? (DateTime?)fst : null,
                        ForecastEndTime = DateTime.TryParse(destTafElement.Element("ForecastEndTime")?.Value, out var fet) ? (DateTime?)fet : null
                    };
                    flightData.pps_TAF.Add(_taf);
                }

                var alt1TafElement = root.Element("ALT1TAF");
                if (alt1TafElement != null)
                {
                    var _taf = new Models.pps_TAF()
                    {
                        Type = alt1TafElement.Element("Type")?.Value,
                        XmlType = "ALT1TAF",
                        Text = alt1TafElement.Element("Text")?.Value,
                        ICAO = alt1TafElement.Element("ICAO")?.Value,
                        ForecastTime = DateTime.TryParse(alt1TafElement.Element("ForecastTime")?.Value, out var ft) ? (DateTime?)ft : null,
                        ForecastStartTime = DateTime.TryParse(alt1TafElement.Element("ForecastStartTime")?.Value, out var fst) ? (DateTime?)fst : null,
                        ForecastEndTime = DateTime.TryParse(alt1TafElement.Element("ForecastEndTime")?.Value, out var fet) ? (DateTime?)fet : null
                    };
                    flightData.pps_TAF.Add(_taf);
                }

                var alt2TafElement = root.Element("ALT2TAF");
                if (alt2TafElement != null)
                {
                    var _taf = new Models.pps_TAF()
                    {
                        Type = alt2TafElement.Element("Type")?.Value,
                        XmlType= "ALT2TAF",
                        Text = alt2TafElement.Element("Text")?.Value,
                        ICAO = alt2TafElement.Element("ICAO")?.Value,
                        ForecastTime = DateTime.TryParse(alt2TafElement.Element("ForecastTime")?.Value, out var ft) ? (DateTime?)ft : null,
                        ForecastStartTime = DateTime.TryParse(alt2TafElement.Element("ForecastStartTime")?.Value, out var fst) ? (DateTime?)fst :null,
                        ForecastEndTime = DateTime.TryParse(alt2TafElement.Element("ForecastEndTime")?.Value, out var fet) ? (DateTime?)fet :null
                    };
                    flightData.pps_TAF.Add(_taf);

                }
                //-------Notam---------------
                var alt1NotamElement = root.Element("ALT1Notam");
                if (alt1NotamElement != null)
                {
                    var notamElements = alt1NotamElement.Elements("Notam");
                    foreach (var notamEl in notamElements) 
                    {
                        if (notamEl != null)
                        {
                            var _notam = new Models.pps_Notam
                            {
                                Notam_Type = "ALT1",
                                Number = notamEl.Element("Number")?.Value,
                                Text = notamEl.Element("Text")?.Value,
                                FromDate = DateTime.TryParse(notamEl.Element("FromDate")?.Value, out var fd) ? (DateTime?)fd : null,
                                ToDate = DateTime.TryParse(notamEl.Element("ToDate")?.Value, out var td) ? (DateTime?)td : null,
                                FromLevel = double.TryParse(notamEl.Element("FromLevel")?.Value, out var fl1) ? (double?) fl1 : null,
                                ToLevel = double.TryParse(notamEl.Element("ToLevel")?.Value, out var tl) ? (double?)tl : null,
                                Fir = notamEl.Element("Fir")?.Value,
                                QCode = notamEl.Element("QCode")?.Value,
                                ECode = notamEl.Element("ECode")?.Value,
                                ICAO = notamEl.Element("ICAO")?.Value,
                                UniformAbbreviation = notamEl.Element("UniformAbbreviation")?.Value,
                                Year = int.TryParse(notamEl.Element("Year")?.Value, out var y) ? y : (int)0,
                                RoutePart = notamEl.Element("RoutePart")?.Value,
                                Provider = notamEl.Element("Provider")?.Value,
                                TotalParts = int.TryParse(notamEl.Element("PartInformation").Element("TotalParts")?.Value, out var tp) ? tp : (int)0,
                            };
                            if (notamEl.Element("PartInformation") != null)
                            {
                                _notam.pps_NotamPart = notamEl.Element("PartInformation")?.Elements("Part")
                                    .Select(p =>new pps_NotamPart() { PartNumber= int.TryParse(p.Value, out var num) ? num : 0 })
                                    .ToList();
                            }
                            flightData.pps_Notam.Add(_notam);
                        }
                    }                      
                }

                var DEPNotamElement = root.Element("DEPNotam");
                if (DEPNotamElement != null)
                {
                    var notamElements = DEPNotamElement.Elements("Notam");
                    foreach (var notamEl in notamElements)
                    {

                        var partInfo = notamEl.Element("PartInformation");
                        int totalParts = 0;
                        List<int> parts = new List<int>();

                        if (partInfo != null)
                        {
                            totalParts = int.TryParse(partInfo.Element("TotalParts")?.Value, out var tp10) ? tp10 : 0;

                            parts = partInfo.Elements("Part")
                                            .Select(p => int.TryParse(p.Value, out var num) ? num : 0)
                                            .ToList();
                        }
                        var _notam = new Models.pps_Notam
                        {
                                Notam_Type = "DEP",
                                Number = notamEl.Element("Number")?.Value,
                                Text = notamEl.Element("Text")?.Value,
                                FromDate = DateTime.TryParse(notamEl.Element("FromDate")?.Value, out var fd) ? (DateTime?)fd : null,
                                ToDate = DateTime.TryParse(notamEl.Element("ToDate")?.Value, out var td) ? (DateTime?)td : null,
                                FromLevel = double.TryParse(notamEl.Element("FromLevel")?.Value, out var fl1) ? (double?)fl1 : null,
                                ToLevel = double.TryParse(notamEl.Element("ToLevel")?.Value, out var tl) ? tl : (double)0,
                                Fir = notamEl.Element("Fir")?.Value,
                                QCode = notamEl.Element("QCode")?.Value,
                                ECode = notamEl.Element("ECode")?.Value,
                                ICAO = notamEl.Element("ICAO")?.Value,
                                UniformAbbreviation = notamEl.Element("UniformAbbreviation")?.Value,
                                Year = int.TryParse(notamEl.Element("Year")?.Value, out var y) ? y : (int)0,
                                RoutePart = notamEl.Element("RoutePart")?.Value,
                                Provider = notamEl.Element("Provider")?.Value,
                                TotalParts = int.TryParse(notamEl.Element("PartInformation").Element("TotalParts")?.Value, out var tp) ? tp : (int)0,
                        };
                           
                            flightData.pps_Notam.Add(_notam);                                              
                    }
                }

                var FIRNotamElement = root.Element("FIRNotam");
                if (FIRNotamElement != null)
                {
                    var notamElements = FIRNotamElement.Elements("Notam");
                    foreach (var notamEl in notamElements)
                    {
                        if (notamEl != null)
                        {
                            var _notam = new Models.pps_Notam
                            {
                                Notam_Type = "FIR",
                                Number = notamEl.Element("Number")?.Value,
                                Text = notamEl.Element("Text")?.Value,
                                FromDate = DateTime.TryParse(notamEl.Element("FromDate")?.Value, out var fd) ? (DateTime?)fd : null,
                                ToDate = DateTime.TryParse(notamEl.Element("ToDate")?.Value, out var td) ? (DateTime?)td : null,
                                FromLevel = double.TryParse(notamEl.Element("FromLevel")?.Value, out var fl1) ? (double?)fl1 : null,
                                ToLevel = double.TryParse(notamEl.Element("ToLevel")?.Value, out var tl) ? (double?)tl : null,
                                Fir = notamEl.Element("Fir")?.Value,
                                QCode = notamEl.Element("QCode")?.Value,
                                ECode = notamEl.Element("ECode")?.Value,
                                ICAO = notamEl.Element("ICAO")?.Value,
                                UniformAbbreviation = notamEl.Element("UniformAbbreviation")?.Value,
                                Year = int.TryParse(notamEl.Element("Year")?.Value, out var y) ? y : (int)0,
                                RoutePart = notamEl.Element("RoutePart")?.Value,
                                Provider = notamEl.Element("Provider")?.Value,
                                TotalParts = int.TryParse(notamEl.Element("PartInformation").Element("TotalParts")?.Value, out var tp) ? tp : (int)0,
                            };
                            if (notamEl.Element("PartInformation") != null)
                            {
                                _notam.pps_NotamPart = notamEl.Element("PartInformation")?.Elements("Part")
                                    .Select(p => new pps_NotamPart() { PartNumber= int.TryParse(p.Value, out var num) ? num : 0 })
                                    .ToList();
                            }
                            flightData.pps_Notam.Add(_notam);
                        }
                    }
                }

                var AdequateNotamElement = root.Element("AdequateNotam");
                if (AdequateNotamElement != null)
                {
                    var notamElements = AdequateNotamElement.Elements("Notam");
                    foreach (var notamEl in notamElements)
                    {
                        if (notamEl != null)
                        {
                            var _notam = new Models.pps_Notam
                            {
                                Notam_Type = "Adequate",
                                Number = notamEl.Element("Number")?.Value,
                                Text = notamEl.Element("Text")?.Value,
                                FromDate = DateTime.TryParse(notamEl.Element("FromDate")?.Value, out var fd) ? (DateTime?)fd : null,
                                ToDate = DateTime.TryParse(notamEl.Element("ToDate")?.Value, out var td) ? (DateTime?)td :null,
                                FromLevel = double.TryParse(notamEl.Element("FromLevel")?.Value, out var fl1) ? (double?)fl1 : null,
                                ToLevel = double.TryParse(notamEl.Element("ToLevel")?.Value, out var tl) ?  (double?)tl : null,
                                Fir = notamEl.Element("Fir")?.Value,
                                QCode = notamEl.Element("QCode")?.Value,
                                ECode = notamEl.Element("ECode")?.Value,
                                ICAO = notamEl.Element("ICAO")?.Value,
                                UniformAbbreviation = notamEl.Element("UniformAbbreviation")?.Value,
                                Year = int.TryParse(notamEl.Element("Year")?.Value, out var y) ? y : (int)0,
                                RoutePart = notamEl.Element("RoutePart")?.Value,
                                Provider = notamEl.Element("Provider")?.Value,
                                TotalParts = int.TryParse(notamEl.Element("PartInformation").Element("TotalParts")?.Value, out var tp) ? tp : (int)0,
                            };
                            if (notamEl.Element("PartInformation") != null)
                            {
                                _notam.pps_NotamPart = notamEl.Element("PartInformation")?.Elements("Part")
                                    .Select(p => new pps_NotamPart() { PartNumber = int.TryParse(p.Value, out var num) ? num : 0 })
                                    .ToList();
                            }
                            flightData.pps_Notam.Add(_notam);
                        }
                    }
                }

                var flightLevelElements = root.Element("OptFlightLevels")?.Elements("FlightLevel");
                if (flightLevelElements != null)
                {
                    flightData.pps_FlightLevel = flightLevelElements.Select(fl2 => new Models.pps_FlightLevel
                    {
                        Level = double.TryParse(fl2.Element("Level")?.Value, out var lvl) ? (double?)lvl : null,
                        Cost = double.TryParse(fl2.Element("Cost")?.Value, out var cost) ? (double?)cost : null,
                        WC = double.TryParse(fl2.Element("WC")?.Value, out var wc) ? (double?)wc : null,
                        TimeNCruise = double.TryParse(fl2.Element("TimeNCruise")?.Value, out var tnc) ? (double?)tnc : null,
                        FuelNCruise = double.TryParse(fl2.Element("FuelNCruise")?.Value, out var fnc) ? (double?)fnc : null,
                        TimeProfile2 = double.TryParse(fl2.Element("TimeProfile2")?.Value, out var tp2) ? (double?)tp2 : null,
                        FuelProfile2 = double.TryParse(fl2.Element("FuelProfile2")?.Value, out var fp2) ? (double?)fp2 : null,
                        TimeProfile3 = double.TryParse(fl2.Element("TimeProfile3")?.Value, out var tp3) ? (double?)tp3 : null,
                        FuelProfile3 = double.TryParse(fl2.Element("FuelProfile3")?.Value, out var fp3) ? (double?)fp3 : null,
                        FuelLower = double.TryParse(fl2.Element("FuelLower")?.Value, out var flwr) ? (double?)flwr : null ,
                        CostDiff = double.TryParse(fl2.Element("CostDiff")?.Value, out var cdiff) ? (double?)cdiff : null
                    }).ToList();
                }

                var airportsElement = root.Element("Airports");
                    if (airportsElement != null)
                    {
                        var airportList = new List<Models.pps_AltAirport>();
                        foreach (var airportEl in airportsElement.Elements("AltAirport"))
                        {
                            var airport = new Models.pps_AltAirport
                            {
                                Type = airportEl.Element("Type")?.Value,
                                Icao = airportEl.Element("Icao")?.Value,
                                Dist = double.TryParse(airportEl.Element("Dist")?.Value, out var dist) ? dist : (double?)null,
                                Time = double.TryParse(airportEl.Element("Time")?.Value, out var time) ? time : (double?)null,
                                Fuel = double.TryParse(airportEl.Element("Fuel")?.Value, out var fuel1) ? fuel1 : (double?)null,
                                MAGCURS = double.TryParse(airportEl.Element("MAGCURS")?.Value, out var mag) ? mag : (double?)null,
                                ATC = airportEl.Element("ATC")?.Value,
                                Lat = double.TryParse(airportEl.Element("Lat")?.Value, out var lat) ? lat : (double?)null,
                                Long = double.TryParse(airportEl.Element("Long")?.Value, out var lon) ? lon : (double?)null,
                                Rwyl = double.TryParse(airportEl.Element("Rwyl")?.Value, out var rwyl) ? rwyl : (double?)null,
                                Elevation = double.TryParse(airportEl.Element("Elevation")?.Value, out var elev) ? elev : (double?)null,
                                Name = airportEl.Element("Name")?.Value,
                                Iata = airportEl.Element("Iata")?.Value,
                                Category = null,
                                Frequencies = null,
                                Frequencies2 = null
                            };
                            airportList.Add(airport);
                        }
                        flightData.pps_AltAirport = airportList;
                }


                    var enrouteAlternatesElement = root.Element("EnrouteAlternates");
                    if (enrouteAlternatesElement != null)
                    {
                    flightData.EnrouteAlternates = enrouteAlternatesElement?.Value;
                    }

                    var stdAlternatesElement = root.Element("StdAlternates");
                    if (stdAlternatesElement != null)
                    {
                        flightData.StdAlternates = stdAlternatesElement?.Value;
                    }

                    var toAltElement = root.Element("TOALT");
                    if (toAltElement != null)
                    {
                        flightData.TOALT = toAltElement?.Value;  // ذخیره کل XML به‌صورت رشته
                    }

                    var PassThroughValuesElement = root.Element("PassThroughValues");
                    if (PassThroughValuesElement != null)
                    {
                        flightData.PassThroughValues = PassThroughValuesElement?.Value;
                    }

                    var ExternalFlightIdElement = root.Element("ExternalFlightId");
                    if (ExternalFlightIdElement != null)
                    {
                        flightData.ExternalFlightId = ExternalFlightIdElement?.Value;  // ذخیره کل XML به‌صورت رشته
                    }

                    var RcfHeaderElement = root.Element("RcfHeader");
                    if (RcfHeaderElement != null)
                    {
                        flightData.RcfHeader = RcfHeaderElement?.Value;  // ذخیره کل XML به‌صورت رشته
                    }

                    var CFMUStatusElement = root.Element("CFMUStatus");
                    if (CFMUStatusElement != null)
                    {
                        flightData.CFMUStatus = CFMUStatusElement?.Value;  // ذخیره کل XML به‌صورت رشته
                    }

                    var FMSIdentElement = root.Element("FMSIdent");
                    if (FMSIdentElement != null)
                    {
                        flightData.FMSIdent = FMSIdentElement?.Value;  // ذخیره کل XML به‌صورت رشته
                    }

                    //bool

                    var isRecalcElement = root.Element("IsRecalc");
                    if (isRecalcElement != null)
                    {
                        string val = isRecalcElement.Value.Trim().ToLower();
                        flightData.IsRecalc = (val == "true" || val == "1");
                    }

                    var alt2AsInfoOnlyElement = root.Element("ALT2AsInfoOnly");
                    if (alt2AsInfoOnlyElement != null)
                    {
                        string val = alt2AsInfoOnlyElement.Value.Trim().ToLower();
                        flightData.ALT2AsInfoOnly = (val == "true" || val == "1");
                    }

                var xElement = root.Element("Responce").Element("Succeed");
                if (xElement != null)
                {
                    string val = xElement.Value.Trim().ToLower();
                    flightData.Responce_Succeed = (val == "true" || val == "1");
                }


                var routeStringsElement = root.Element("RouteStrings");
                if (routeStringsElement != null)
                {
                    flightData.RouteStrings_ToDest = routeStringsElement.Element("ToDest")?.Value;
                    flightData.RouteStrings_ToAlt1 = routeStringsElement.Element("ToAlt1")?.Value;
                    flightData.RouteStrings_ToAlt2 = routeStringsElement.Element("ToAlt2")?.Value;
                    flightData.RouteStrings_TOAlt = routeStringsElement.Element("TOAlt")?.Value;
                }

                var etopsInfoElement = root.Element("EtopsInformation");
                if (etopsInfoElement != null)
                {
                    flightData.EtopsInformation_RuleTimeUsed = double.TryParse(etopsInfoElement.Element("RuleTimeUsed")?.Value, out var rtu) ? rtu : (double?)null;
                    flightData.EtopsInformation_IcingPercentage = double.TryParse(etopsInfoElement.Element("IcingPercentage")?.Value, out var ip) ? ip : (double?)null;
                }

                    var correctionTablesElement = root.Element("CorrectionTable");
                    if (correctionTablesElement != null)
                    {
                        flightData.pps_CorrectionTable = correctionTablesElement.Elements("CorrectionTable")
                            .Select(x => new Models.pps_CorrectionTable
                            {
                                CtID = x.Element("CtID")?.Value,
                                Flightlevel = double.TryParse(x.Element("Flightlevel")?.Value, out var fl6) ? fl6 : (double?)null,
                                WindCorrection = double.TryParse(x.Element("WindCorrection")?.Value, out var wc) ? wc : (double?)null,
                                TimeInMinutesForCruiseProfile = double.TryParse(x.Element("TimeInMinutesForCruiseProfile")?.Value, out var tcp) ? tcp : (double?)null,
                                TimeInHoursMinutesForCruiseProfile = double.TryParse(x.Element("TimeInHoursMinutesForCruiseProfile")?.Value, out var thcp) ? thcp : (double?)null,
                                TimeInHoursMinutesForAltCruiseProfile = x.Element("TimeInHoursMinutesForAltCruiseProfile")?.Value,
                                TimeInMinutesForAltCruiseProfile = x.Element("TimeInMinutesForAltCruiseProfile")?.Value,
                                TimeInMinutesForXProfile = double.TryParse(x.Element("TimeInMinutesForXProfile")?.Value, out var txp) ? txp : (double?)null,
                                TimeInHoursMinutesForXProfile = x.Element("TimeInHoursMinutesForXProfile")?.Value,
                                FuelForSelectedProfile = double.TryParse(x.Element("FuelForSelectedProfile")?.Value, out var fsp) ? fsp : (double?)null,
                                FuelForSecondProfile = double.TryParse(x.Element("FuelForSecondProfile")?.Value, out var fsp2) ? fsp2 : (double?)null,
                                FuelForXProfile = double.TryParse(x.Element("FuelForXProfile")?.Value, out var fxp) ? fxp : 0,
                                DifferentialCost = double.TryParse(x.Element("DifferentialCost")?.Value, out var dc) ? dc : (double?)null,
                                TotalFuelIncreaseWith10ktWind = double.TryParse(x.Element("TotalFuelIncreaseWith10ktWind")?.Value, out var tfi) ? tfi : (double?)null,
                            })
                            .ToList();
                    }
                var sidElement = root.Element("SidAndStarProcedures").Element("Sid");
                if (sidElement != null)
                {
                    flightData.SidProcedures_Name = sidElement.Element("Name")?.Value;
                    flightData.SidProcedures_Info = sidElement.Element("Info")?.Value;
                }
                var starElement = root.Element("SidAndStarProcedures").Element("Star");
                if (starElement != null)
                {
                    flightData.StarProcedures_Name = starElement.Element("Name")?.Value;
                    flightData.StarProcedures_Info = starElement.Element("Info")?.Value;
                }

                var loadElement = root.Element("Load");
                if (loadElement != null)
                {
                    // var fuelElement = loadElement.Element("Fuel");
                    flightData.Load_Fuel_ActTotal = double.TryParse(loadElement.Element("Fuel").Element("ActTotal")?.Value, out var acttotal) ? acttotal : (double?)null;
                    var yElement = loadElement.Element("Fuel").Element("LoadFuel").Element("LoadFuelSection");
                    if (yElement != null)
                    {
                        flightData.Load_Fuel_Section_ActMass = double.TryParse(yElement.Element("ActMass")?.Value, out var actmass) ? (double?)actmass : null;
                        flightData.Load_Fuel_Section_ID = yElement.Element("ID")?.Value;
                    }
                    flightData.Load_Cargo_ActTotal = double.TryParse(loadElement.Element("Cargo").Element("ActTotal")?.Value, out var acttotal1) ? acttotal1 : (double?)null;
                    yElement = loadElement.Element("Cargo").Element("LoadCargo").Element("LoadCargoSection");
                    if (yElement != null)
                    {
                        flightData.Load_Cargo_Section_ActMass = double.TryParse(yElement.Element("ActMass")?.Value, out var actmass) ? (double?)actmass : null;
                        flightData.Load_Cargo_Section_ID = yElement.Element("ID")?.Value;
                    }
                    flightData.Load_Pax_Total = double.TryParse(loadElement.Element("Pax").Element("Total")?.Value, out var paxtotal) ? paxtotal : (double?)null;
                    yElement = loadElement.Element("Pax").Element("PaxData");
                    if (yElement != null)
                    {
                        flightData.Load_Pax_Data_MaxPax = double.TryParse(yElement.Element("MaxPax")?.Value, out var maxpax) ? (double?)maxpax : null;
                        flightData.Load_Pax_Data_ActPax = double.TryParse(yElement.Element("ActPax")?.Value, out var actpax) ? (double?)actpax : null;
                        flightData.Load_Pax_Data_ActMass = double.TryParse(yElement.Element("ActMass")?.Value, out var actmass) ? (double?)actmass : null;
                        flightData.Load_Pax_Data_PaxAmount = double.TryParse(yElement.Element("PaxAmount")?.Value, out var paxamount) ? (double?)paxamount : null;
                        flightData.Load_Pax_Data_Male = double.TryParse(yElement.Element("Male")?.Value, out var male) ? (double?)male : null;
                        flightData.Load_Pax_Data_Female = double.TryParse(yElement.Element("Female")?.Value, out var female) ?  (double?)female : null;
                        flightData.Load_Pax_Data_Children = double.TryParse(yElement.Element("Children")?.Value, out var children) ? (double?)children : null;
                        flightData.Load_Pax_Data_Infant = double.TryParse(yElement.Element("Infant")?.Value, out var infant) ? (double?)infant : null;
                        flightData.Load_Pax_Data_CustMass = yElement.Element("CustMass")?.Value;

                    }
                    yElement = loadElement.Element("Pax")?.Element("PaxSections")?.Element("SimplePaxSection");
                    flightData.pps_PaxSection = new List<Models.pps_PaxSection>();
                    if (yElement != null)
                    {
                        var _PaxSection = new Models.pps_PaxSection();
                        _PaxSection.Row = yElement.Element("Row")?.Value;
                        _PaxSection.PaxSectionType = "Simple";
                        _PaxSection.ActPax = double.TryParse(yElement.Element("ActPax")?.Value, out var actpax) ? (double?)actpax : null;
                        _PaxSection.ActMass = double.TryParse(yElement.Element("ActMass")?.Value, out var actmass) ? (double?)actmass : null;
                        _PaxSection.Male = double.TryParse(yElement.Element("Male")?.Value, out var male) ? (double?)male : null;
                        _PaxSection.Female = double.TryParse(yElement.Element("Female")?.Value, out var female) ? (double?)female : null;
                        _PaxSection.Children = double.TryParse(yElement.Element("Children")?.Value, out var children) ? (double?)children : null;
                        _PaxSection.Infant = double.TryParse(yElement.Element("Infant")?.Value, out var infant) ? (double?)infant : null;
                        _PaxSection.CutsMass = yElement.Element("CustMass")?.Value;
                        flightData.pps_PaxSection.Add(_PaxSection);
                    }
                    yElement = loadElement.Element("MassBalance")?.Element("Takeoff");
                    if (yElement != null)
                    {
                        flightData.Load_MassBalance_Takeoff_ForwardLimit = double.TryParse(yElement.Element("ForwardLimit")?.Value, out var forwardlimit) ? (double?)forwardlimit :null;
                        flightData.Load_MassBalance_Takeoff_ActualPosition = double.TryParse(yElement.Element("ActualPosition")?.Value, out var ap) ? (double?)ap : null;
                        flightData.Load_MassBalance_Takeoff_AftLimit = double.TryParse(yElement.Element("AftLimit")?.Value, out var al) ? (double?)al : null;
                        
                    }

                    yElement = loadElement.Element("MassBalance").Element("Landing");
                    if (yElement != null)
                    {
                        flightData.Load_MassBalance_Landing_ForwardLimit = double.TryParse(yElement.Element("ForwardLimit")?.Value, out var forwardlimit) ? (double?)forwardlimit :null;
                        flightData.Load_MassBalance_Landing_ActualPosition = double.TryParse(yElement.Element("ActualPosition")?.Value, out var ap) ? (double?)ap : null;
                        flightData.Load_MassBalance_Landing_AftLimit = double.TryParse(yElement.Element("AftLimit")?.Value, out var al) ? (double?)al : null;

                    }
                    yElement = loadElement.Element("MassBalance").Element("ZeroFuel");
                    if (yElement != null)
                    {
                        flightData.Load_MassBalance_ZeroFuel_ForwardLimit = double.TryParse(yElement.Element("ForwardLimit")?.Value, out var forwardlimit) ? (double?)forwardlimit : null;
                        flightData.Load_MassBalance_ZeroFuel_ActualPosition = double.TryParse(yElement.Element("ActualPosition")?.Value, out var ap) ? (double?)ap : null;
                        flightData.Load_MassBalance_ZeroFuel_AftLimit = double.TryParse(yElement.Element("AftLimit")?.Value, out var al) ? (double?)al : null;

                    }
                    yElement = loadElement.Element("MassBalance").Element("Index");
                    if (yElement != null)
                    {
                        flightData.Load_MassBalance_Index_DryOperatingIndex = double.TryParse(yElement.Element("DryOperatingIndex")?.Value, out var a1) ? (double?)a1 : null;
                        flightData.Load_MassBalance_Index_ZeroFuelForwardLimit = double.TryParse(yElement.Element("ZeroFuelForwardLimit")?.Value, out var a2) ? (double?)a2 : null;
                        flightData.Load_MassBalance_Index_ZeroFuelWeightIndex = double.TryParse(yElement.Element("ZeroFuelWeightIndex")?.Value, out var a3) ? (double?)a3 : null;
                        flightData.Load_MassBalance_Index_ZeroFuelAftLimit = double.TryParse(yElement.Element("ZeroFuelAftLimit")?.Value, out var a4) ? (double?)a4 :  null;

                    }
                    yElement = loadElement.Element("Payload");
                    if (xElement != null)
                    {
                        flightData.Load_Payload_MaxPayload = double.TryParse(yElement.Element("MaxPayload")?.Value, out var a1) ? (double?)a1 : null;
                        flightData.Load_Payload_Mzfm = double.TryParse(yElement.Element("Mzfm")?.Value, out var a2) ? (double?)a2 : null;
                        flightData.Load_Payload_Mtom = double.TryParse(yElement.Element("Mtom")?.Value, out var a3) ? (double?)a3 : null;
                        flightData.Load_Payload_Mlm = double.TryParse(yElement.Element("Mlm")?.Value, out var a4) ? (double?)a4 : null;
                        flightData.Load_Payload_Mrmp = double.TryParse(yElement.Element("Mrmp")?.Value, out var a5) ? (double?)a5 : null;
                        flightData.Load_Payload_MaxCargo = double.TryParse(yElement.Element("MaxCargo")?.Value, out var a6) ? (double?)a6 : null;

                    }
                    yElement = loadElement.Element("DryOperating");
                    if (yElement != null)
                    {
                        flightData.Load_DryOperating_BasicEmptyArm = double.TryParse(yElement.Element("BasicEmptyArm")?.Value, out var a1) ? (double?)a1 : null;
                        flightData.Load_DryOperating_BasicEmptyWeight = double.TryParse(yElement.Element("BasicEmptyWeight")?.Value, out var a2) ? (double?)a2 : null;
                        flightData.Load_DryOperating_DryOperatingMassArm = double.TryParse(yElement.Element("DryOperatingMassArm")?.Value, out var a3) ? (double?)a3 : null;
                        flightData.Load_DryOperating_DryOperatingWeight = double.TryParse(yElement.Element("DryOperatingWeight")?.Value, out var a4) ? (double?)a4 : null;
                    }
                }

                var acConfigElement = root.Element("AircraftConfiguration");
                if (acConfigElement != null)
                {
                    flightData.AircraftConfiguration_Name=acConfigElement.Element("Name")?.Value;
                    flightData.pps_Crew = new List<Models.pps_Crew>();
                    var acCrewElement = acConfigElement.Element("Crew")?.Elements("Crew");
                    if (acCrewElement != null)
                    {
                        var _crew = new Models.pps_Crew();
                        foreach (var _xElemnt in acCrewElement)
                        {
                            _crew.XmlID=_xElemnt.Element("ID")?.Value;
                            _crew.CrewType = _xElemnt.Element("CrewType")?.Value;
                            _crew.XmlType = "AircraftConfiguration";
                            _crew.CrewName = _xElemnt.Element("CrewName")?.Value;
                            _crew.Initials = _xElemnt.Element("Initials")?.Value;
                            _crew.GSM = _xElemnt.Element("GSM")?.Value;
                            flightData.pps_Crew.Add(_crew);
                        }
                    }
                }

                    var ppsInfoElement = root.Element("PpsVersionInformation");
                    if (ppsInfoElement != null)
                    {
                    flightData.PpsApplicationVersion = ppsInfoElement.Element("PpsApplicationVersion")?.Value;
                    flightData.PpsExeVersion = ppsInfoElement.Element("PpsExeVersion")?.Value;
                    
                    }

                    var customRefElement = root.Element("CustomReferences");
                if (customRefElement != null)
                {                        
                    flightData.CustomReferences_MilID = customRefElement.Element("MilID")?.Value;
                    flightData.CustomReferences_RefID = customRefElement.Element("RefID")?.Value;
                }

                var extraFuelsElements = root.Elements("ExtraFuels")?.Elements("ExtraFuel");
                    if (extraFuelsElements != null)
                    {
                        flightData.pps_ExtraFuel = extraFuelsElements.Select(e => new Models.pps_ExtraFuel
                        {
                            Type = e.Element("Type")?.Value,
                            Fuel = double.TryParse(e.Element("Fuel")?.Value, out var fuelVal) ? (double?)fuelVal : null,
                            Time = e.Element("Time")?.Value

                        }).ToList();
                    }


                    var weatherElements = root.Elements("PlanningEnRouteAlternateAirports")?.Elements("AirportWeatherData");
                    if (weatherElements != null)
                    {
                        flightData.pps_AirportWeatherData = weatherElements.Select(w => new Models.pps_AirportWeatherData
                        {
                            ICAO = w.Element("ICAO")?.Value,
                            Taf_Type= w.Element("TAF")?.Element("Type")?.Value,
                            Taf_Text= w.Element("TAF")?.Element("Text")?.Value,
                            Taf_ICAO= w.Element("TAF")?.Element("ICAO")?.Value,
                            Taf_ForecastTime= DateTime.TryParse(w.Element("TAF")?.Element("ForecastTime")?.Value, out var ft) ? (DateTime?)ft : null,
                            Taf_ForecastStartTime = DateTime.TryParse(w.Element("TAF")?.Element("ForecastStartTime")?.Value, out var ft3) ? (DateTime?) ft3 : null,
                            Taf_ForecastEndTime = DateTime.TryParse(w.Element("TAF")?.Element("ForecastEndTime")?.Value, out var ft2) ? (DateTime?) ft2 : null,
                            Metar_Text = w.Element("Metar")?.Element("Text")?.Value,
                            Metar_ICAO = w.Element("Metar")?.Element("ICAO")?.Value,
                            Metar_ObservationTime = DateTime.TryParse(w.Element("Metar")?.Element("ObservationTime")?.Value, out var ot) ? (DateTime?) ot : null,
                            Metar_ObservationType = w.Element("Metar")?.Element("ObservationType")?.Value
                        }).ToList();
                    }
                var existingEntity = await context.pps_Flight.FindAsync(flightData.Id);
                if (existingEntity != null)
                {
                    // آپدیت رکورد موجود
                    context.Entry(existingEntity).CurrentValues.SetValues(flightData);
                }
                else
                {
                    // اضافه کردن رکورد جدید
                    // await context.AddAsync(flightData);
                    await context.SaveAsync();

                }


                await context.SaveAsync();

                return new DataResponse
                    {
                        Data = new { flightData },
                        IsSuccess = true
                    };
                }
               
                else
                {
                    return new DataResponse
                    {
                        Data = null,
                        IsSuccess = false
                    };
                }




        }

    }
    }
