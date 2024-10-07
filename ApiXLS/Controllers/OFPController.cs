using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using static ApiXLS.Controllers.OFPController;
using ApiXLS.Models;
using System.Windows.Markup;
using System.Data.Entity.Validation;
using Microsoft.Ajax.Utilities;
using System.Threading;
using Spire.Pdf.Graphics;
using Spire.Pdf;
using System.Drawing;
using System.IO;
using Spire.Pdf.General.Find;

namespace ApiXLS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OFPController : ApiController
    {
        dbEntities context = new dbEntities();
        public class FlightPlan
        {
            public string ReferenceNo { get; set; }
            public string AirlineName { get; set; }
            public string WeightUnit { get; set; }
            public double CruisePerformanceFactor { get; set; }
            public int ContingencyPercent { get; set; }
            public string FlightNo { get; set; }
            public DateTime GenerationDate { get; set; }
            public DateTime ScheduledTimeDeparture { get; set; }
            public DateTime ScheduledTimeArrival { get; set; }
            public string TailNo { get; set; }
            public string MSN { get; set; }
            public string AircraftType { get; set; }
            public string AircraftSubType { get; set; }
            public int ManeuveringTime { get; set; }
            public int ManeuveringFuel { get; set; }
            public string CruiseSpeed { get; set; }
            public int CostIndex { get; set; }
            public int DryOperatingWeight { get; set; }
            public int Payload { get; set; }
            public string Origin { get; set; }
            public string origin_iata { get; set; }
            public int OriginElevation { get; set; }
            public string Destination { get; set; }
            public string destination_iata { get; set; }
            public int DestinationElevation { get; set; }
            public int MainFlightLevel { get; set; }
            public string Alternate { get; set; }
            public string WeatherCycle { get; set; }
            public string Warning1 { get; set; }
            public string Warning2 { get; set; }
            public string Warning3 { get; set; }
            public string TripAverageWindComponent { get; set; }
            public string TripAverageTempISA { get; set; }
            public string TripLevel { get; set; }
            public string Alternate1AverageWindComponent { get; set; }
            public string Alternate1AverageTempISA { get; set; }
            public string Alternate2AverageWindComponent { get; set; }
            public string Alternate2AverageTempISA { get; set; }
            public string Alternate1 { get; set; }
            public int Alternate1Elevation { get; set; }
            public int Alternate1FlightLevel { get; set; }
            public string Alternate2 { get; set; }
            public int? Alternate2Elevation { get; set; }
            public int Alternate2FlightLevel { get; set; }
            public string TakeoffAlternate { get; set; }
            public int? TakeoffAlternateElevation { get; set; }
            public int TakeoffAlternateFlightLevel { get; set; }
            public string AlternateEnroute { get; set; }
            public int Cockpit { get; set; }
            public int Cabin { get; set; }
            public int Extra { get; set; }
            public int Pantry { get; set; }
            public string Pilot1 { get; set; }
            public string Pilot2 { get; set; }
            public string Dispatcher { get; set; }
            public string MaxWindShearLevel { get; set; }
            public string MaxWindShearPointName { get; set; }
            public string MaxShear { get; set; }
            public string FlightRule { get; set; }
            public string ICAOFlightPlan { get; set; }
            public FIRs FIRs { get; set; }
            public int MaximumZeroFuelWeight { get; set; }
            public int MaximumTakeoffWeight { get; set; }
            public int MaximumLandingWeight { get; set; }
            public int EstimatedZeroFuelWeight { get; set; }
            public int EstimatedTakeoffWeight { get; set; }
            public int EstimatedLandingWeight { get; set; }
            public string MainRoute { get; set; }
            public string Alternate1Route { get; set; }
            public string Alternate2Route { get; set; }
            public string TakeoffAlternateRoute { get; set; }
            public DateTime PlanValidity { get; set; }

            public int nautical_ground_miles { get; set; }
            public int nautical_air_miles { get; set; }
            // Properties from the Fuels class
            public int fuel_trip { get; set; }
            public int fuel_alt { get; set; }
            public int fuel_alt1 { get; set; }
            public int fuel_alt2 { get; set; }
            public int fuel_takeoff_alternate { get; set; }
            public int fuel_holding { get; set; }
            public int fuel_contingency { get; set; }
            public int fuel_taxiout { get; set; }
            public int fuel_taxiin { get; set; }
            public int fuel_min_required { get; set; }
            public int fuel_additional { get; set; }
            public string fuel_additional_description { get; set; }
            public int fuel_extra { get; set; }
            public string fuel_extra_description { get; set; }
            public int fuel_total { get; set; }
            public int fuel_landing { get; set; }
            public int fuel_mod_alt1 { get; set; }
            public int fuel_mod_alt2 { get; set; }
            public string fuel_extra_due { get; set; }

            // Properties from the Times class
            public int time_trip { get; set; }
            public string time_trip_str { get; set; }
            public int time_alt { get; set; }
            public string time_alternate_str { get; set; }
            public int time_alt1 { get; set; }
            public string time_alternate1_str { get; set; }
            public int time_alt2 { get; set; }
            public string time_alternate2_str { get; set; }
            public int time_alt_takeoff { get; set; }
            public string time_takeoff_alternate_str { get; set; }
            public int time_holding { get; set; }
            public string time_holding_str { get; set; }
            public int time_contingency { get; set; }
            public string time_contingency_str { get; set; }
            public int time_minimum_required { get; set; }
            public string time_minimum_required_str { get; set; }
            public int time_additional { get; set; }
            public string time_additional_str { get; set; }
            public int time_extra { get; set; }
            public string time_extra_str { get; set; }
            public int time_taxi_in { get; set; }
            public string time_taxi_in_str { get; set; }
            public int time_taxi_out { get; set; }
            public string time_taxi_out_str { get; set; }
            public int time_total { get; set; }
            public string time_total_str { get; set; }

            // Properties from the Distances class (updated to snake_case)
            public double dis_main_gcd { get; set; }
            public double dis_trip { get; set; }
            public double dis_alt { get; set; }
            public double dis_alt1 { get; set; }
            public double dis_alt2 { get; set; }
            public double dis_alt_takeoff { get; set; }
            public double dis_ground { get; set; }
            public double dis_air { get; set; }

            public List<WindAndTemp> WindAndTemp { get; set; }
            public List<WeightDrift> WeightDrift { get; set; }
            public List<AltitudeDrift> AltitudeDrift { get; set; }
            public List<CriticalSectorData> CriticalSectorsData { get; set; }
            public List<NavLog> MainNavLog { get; set; }
            public List<NavLog> AlternateNavLog { get; set; }
            public HeightChange HeightChange { get; set; }
            public BurnOffAdjustment BurnOffAdjustment { get; set; }
        }

        public class FIRs
        {
            public string Main { get; set; }
        }
        public class WindAndTemp
        {
            public string FL100 { get; set; }
            public string FL140 { get; set; }
            public string FL180 { get; set; }
            public string FL220 { get; set; }
            public string FL260 { get; set; }
            public string FL300 { get; set; }
            public string FL340 { get; set; }
            public string FL380 { get; set; }
            public string Location { get; set; }
        }

        public class WeightDrift
        {
            public int Offset { get; set; }
            public string FuelDelta { get; set; }
        }

        public class AltitudeDrift
        {
            public int FL { get; set; }
            public string AvgWind { get; set; }
            public string FuelDelta { get; set; }
            public string TemperatureDelta { get; set; }
            public string ShearDelta { get; set; }
            public string MachShear { get; set; }
            public int ISADev { get; set; }
        }

        public class CriticalSectorData
        {
            public int ETPNo { get; set; }
            public string Airport { get; set; }
            public string ATIS { get; set; }
            public string ETP { get; set; }
            public int RemainingFuel { get; set; }
            public int RequiredFuel { get; set; }
            public int DifferenceFuel { get; set; }
            public string Time { get; set; }
            public int DistanceNM { get; set; }
            public List<RW> Runways { get; set; }
        }

        public class RW
        {
            public string Runway { get; set; }
            public int Length { get; set; }
            public int MB { get; set; }
            public int? ILS { get; set; }
        }

        public class NavLog
        {
            public string WayPoint { get; set; }
            public string FlightLevel { get; set; }
            public string Frequency { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string Wind { get; set; }
            public string Temperature { get; set; }
            public string ATA { get; set; }
            public string ETA { get; set; }
            public int FuelRemained { get; set; }
            public int FuelUsed { get; set; }
            public int CumulativeFuel { get; set; }
            public int TrueAirSpeed { get; set; }
            public int GroundSpeed { get; set; }

            public string CumulativeTime { get; set; }
            public string ZoneTime { get; set; }
            public string MagneticTrack { get; set; }
            public string Heading { get; set; }
            public string ZoneDistance { get; set; }
            public string CumulativeDistance { get; set; }
            public string MEA { get; set; }
            public string GMR { get; set; }
            public string MORA { get; set; }
            public string Airway { get; set; }
        }

        public class HeightChange
        {
            public int Value { get; set; }
            public int Fuel { get; set; }
        }

        public class BurnOffAdjustment
        {
            public int? Value { get; set; }
            public int? Fuel { get; set; }
        }

        public class skyputer
        {
            public string plan { get; set; }
            public string fltno { get; set; }
            public string date { get; set; }

            public string key { get; set; }
        }


        [Route("api/generate/certificate")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> generate_certificate()
        {
            PdfDocument pdf = new PdfDocument();

            string uploadFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "upload");
            string pdfTemplatePath = Path.Combine(uploadFolderPath, "certificate.pdf");


            pdf.LoadFromFile(pdfTemplatePath);

            // Access the first page of the PDF
            PdfPageBase page = pdf.Pages[0];

            // Define dynamic content (the person's name)
            string personName = "John Doe"; // Replace this with the actual name

            PlaceTextAtCoordinates(page,  personName);

            // Save the modified PDF
            string outputPdfPath = Path.Combine(uploadFolderPath, "GeneratedCertificateWithReplacedName7.pdf");
            pdf.SaveToFile(outputPdfPath);

            // Close the document
            pdf.Close();


            return Ok(page);
        }



        static void PlaceTextAtCoordinates(PdfPageBase page, string replacementText)
        {
            // Define the font and brush for the replacement text
            PdfTrueTypeFont font = new PdfTrueTypeFont(new Font("Arial", 18f, FontStyle.Bold), true);
            PdfSolidBrush brush = new PdfSolidBrush(Color.Black);

            // Manually set the coordinates where the placeholder text should be replaced
            // Adjust the X and Y values to position the text correctly
            float xPosition = 360; // Adjust this based on where the text should appear horizontally
            float yPosition = 250; // Adjust this based on where the text should appear vertically

            // Draw the replacement text at the specified coordinates
            page.Canvas.DrawString(replacementText, font, brush, new PointF(xPosition, yPosition));
        }


        [Route("api/post/ofp")]
        [AcceptVerbs("Post")]
        public async Task<IHttpActionResult> post_ofp(skyputer dto)
        {
            var entity = new OFPSkyPuter()
            {
                OFP = dto.plan,
                DateCreate = DateTime.Now,
                UploadStatus = 0,


            };
           
            context.OFPSkyPuters.Add(entity);
            context.SaveChanges();
            new Thread(async () =>
            {
                await OFPParser(entity.Id);
            }).Start();
            return Ok(true);
        }

        [Route("api/get/ofp")]
        [AcceptVerbs("Get")]

        public async Task<IHttpActionResult> OFPParser(int id)
        {

            //string input = "binfo:|OPT=VARESH AIRLINES;UNT=KGS;FPF=-> FUEL INCLUDES 11.0 PCT PERF FACTOR;FLN= VRH 6825 ;DTE=AUG 12 2024;ETD=03:45;ETA=05:03;REG=EPVAV (737-3S3) - MSN: 27454;THM=FMACH: ;MCI=74;FLL=340;DOW=33694;PLD=11500;NGM=450.4;NAM=472;ALTN=ORBI;MODA=[ORBI, 2506];MODB=;CRW=2-4;CM1=BONAKDAR, FARAZ?0;CM2=ALIEBRAHIMI , AMIR?0;DSP=JAVAHERIPOUR, FAEZEH  1170;SPT=;ELDP=ELEV: 3305;ELDS=ELEV: 107;ELAL=ELEV: 114;ELBL=;MSH=OIIE [1];MZFW=48307;MTOW=61234;MLDW=51709;EZFW=45194;ETOW=54914;ELDW=51198;RTS=OIIE - ORNI;RTM=OIIE.PAVE1G PAVET A647 RAGET Z431 LOVEK LOVE1B.ORNI;RTA=ORNI.SEPT1A SEPTU .ORBI;RTB=;RTT=||;DID=DOC ID#: 2024HBTKBCI||;VDT=OFP Generated On: 11 Aug 2024 20:57:09 and Valid until: 12 Aug 2024 09:45:00 (UTC)||futbl:|PRM=TRIP FUEL;TIM=01:18:00.00000;VAL=3716|PRM=CONT[5%];TIM=00:05:00.00000;VAL=196|PRM=ALTN 1;TIM=00:22:00.00000;VAL=1326|PRM=ALTN 2;TIM=00:00:00.00000;VAL=0|PRM=FINAL RES;TIM=00:30:00.00000;VAL=1180|PRM=ETOPS/ADDNL;TIM=00:00:00.00000;VAL=0|PRM=OPS.EXTRA;TIM=00:00:00.00000;VAL=0|PRM=MIN TOF;TIM=02:14:00.00000;VAL=6418|PRM=TANKERING;TIM=01:10:00.00000;VAL=3302|PRM=TAXI;TIM=00:00:00.00000;VAL=200|PRM=TOTAL FUEL;TIM=03:24:00.00000;VAL=9920|PRM=EZFW;TIM=00:00:00.00000;VAL=45194|PRM=ETOW;TIM=00:00:00.00000;VAL=54914|PRM=ELW;TIM=00:00:00.00000;VAL=51198||mpln:|WAP=OIIE;GEO=N3525.0 E05109.1;LAT=35.4161110000;LON=51.1522220000;FRQ=;VIA=PAVE1G;ALT=033;MEA=0;GMR=153;DIS=0;TDS=0;WID=000/000/001 H;TRK=000/000;TMP=09 / 00/01/0;TME=00:00:00.0000000;TTM=00:00:00.0000000;FRE=9720;FUS=200;TAS=150;GSP=150|WAP=IKA;GEO=N3524.6 E05110.7;LAT=35.4096670000;LON=51.1784720000;FRQ=;VIA=PAVE1G;ALT=093;MEA=0;GMR=153;DIS=2;TDS=2;WID=000/000/001 H;TRK=273/273;TMP=03 / 00/01/53112;TME=00:01:00.0000000;TTM=00:01:00.0000000;FRE=9618;FUS=302;TAS=165;GSP=165|WAP=PAVET;GEO=N3526.7 E04953.0;LAT=35.4441670000;LON=49.8836110000;FRQ=;VIA=A647;ALT=303;MEA=210;GMR=121;DIS=64;TDS=66;WID=247/020/019 H;TRK=229/229;TMP=-19 / 20/01/53493;TME=00:10:00.0000000;TTM=00:11:00.0000000;FRE=8585;FUS=1335;TAS=321;GSP=302|WAP=LOXAM;GEO=N3504.2 E04916.0;LAT=35.0708330000;LON=49.2669440000;FRQ=;VIA=A647;ALT=321;MEA=180;GMR=121;DIS=37.6;TDS=103.6;WID=257/024/023 H;TRK=247/247;TMP=-32 / 19/01/53493;TME=00:06:00.0000000;TTM=00:17:00.0000000;FRE=7966;FUS=1954;TAS=366;GSP=343|WAP=TOC;GEO=N3501.0 E04906.6;LAT=35.0161;LON=49.1099;FRQ=;VIA=A647;ALT=340;MEA=;GMR=121;DIS=8.4;TDS=112;WID=257/032/031 H;TRK=247/247;TMP=-34 / 19/01/53493;TME=00:01:00.0000000;TTM=00:18:00.0000000;FRE=7864;FUS=2056;TAS=445;GSP=414|WAP=HAM;GEO=N3452.0 E04833.0;LAT=34.8668890000;LON=48.5502780000;FRQ=;VIA=A647;ALT=340;MEA=180;GMR=141;DIS=28.9;TDS=140.9;WID=263/027/024 H;TRK=235/236;TMP=-34 / 19/01/53686;TME=00:04:00.0000000;TTM=00:22:00.0000000;FRE=7710;FUS=2210;TAS=447;GSP=423|WAP=ASRIL;GEO=N3428.2 E04745.2;LAT=34.4702780000;LON=47.7536110000;FRQ=;VIA=A647;ALT=340;MEA=160;GMR=134;DIS=46;TDS=186.9;WID=260/025/024 H;TRK=251/251;TMP=-34 / 19/01/54883;TME=00:07:00.0000000;TTM=00:29:00.0000000;FRE=7468;FUS=2452;TAS=447;GSP=423|WAP=ETP 1;GEO=N3423.0 E04719.3;LAT=34.383;LON=47.3216;FRQ=;VIA=A647;ALT=340;MEA=;GMR=134;DIS=22;TDS=208.9;WID=256/024/022 H;TRK=251/251;TMP=-34 / 19/01/54883;TME=00:03:00.0000000;TTM=00:32:00.0000000;FRE=7352;FUS=2568;TAS=447;GSP=425|WAP=KMS;GEO=N3420.4 E04710.2;LAT=34.3397220000;LON=47.1691390000;FRQ=;VIA=A647;ALT=340;MEA=160;GMR=134;DIS=8;TDS=216.9;WID=256/024/022 H;TRK=228/229;TMP=-34 / 19/01/54883;TME=00:01:00.0000000;TTM=00:33:00.0000000;FRE=7311;FUS=2609;TAS=447;GSP=425|WAP=FIR-BAGHDAD;GEO=N3331.6 E04555.0;LAT=33.5265475855;LON=45.9166670000;FRQ=;VIA=ORBB;ALT=340;MEA=0;GMR=77;DIS=79.3;TDS=296.2;WID=256/024/022 H;TRK=228/229;TMP=-34 / 19/01/54883;TME=00:11:00.0000000;TTM=00:44:00.0000000;FRE=6895;FUS=3025;TAS=447;GSP=425|WAP=RAGET;GEO=N3330.8 E04553.8;LAT=33.5133330000;LON=45.8966670000;FRQ=;VIA=Z431;ALT=340;MEA=0;GMR=77;DIS=1.2;TDS=297.4;WID=249/018/017 H;TRK=244/244;TMP=-34 / 19/01/55297;TME=00:00:00.0000000;TTM=00:44:00.0000000;FRE=6889;FUS=3031;TAS=447;GSP=430|WAP=VAXEN;GEO=N3318.0 E04515.0;LAT=33.3000000000;LON=45.2500000000;FRQ=;VIA=Z431;ALT=340;MEA=160;GMR=77;DIS=34.9;TDS=332.3;WID=243/018/014 H;TRK=204/205;TMP=-34 / 19/01/55089;TME=00:05:00.0000000;TTM=00:49:00.0000000;FRE=6710;FUS=3210;TAS=447;GSP=433|WAP=TOD;GEO=N3256.0 E04503.3;LAT=32.9336;LON=45.0557;FRQ=;VIA=Z431;ALT=340;MEA=;GMR=19;DIS=24.1;TDS=356.4;WID=243/018/013 H;TRK=204/205;TMP=-34 / 19/01/55089;TME=00:04:00.0000000;TTM=00:53:00.0000000;FRE=6563;FUS=3357;TAS=447;GSP=434|WAP=LOVEK;GEO=N3222.1 E04440.0;LAT=32.3688890000;LON=44.6669440000;FRQ=;VIA=LOVE1B;ALT=191;MEA=160;GMR=18;DIS=39;TDS=395.4;WID=211/014/012 H;TRK=240/240;TMP=-06 / 19/01/54883;TME=00:10:00.0000000;TTM=01:03:00.0000000;FRE=6339;FUS=3581;TAS=250;GSP=250|WAP=NI307;GEO=N3216.9 E04429.4;LAT=32.2814170000;LON=44.4903890000;FRQ=;VIA=LOVE1B;ALT=151;MEA=0;GMR=18;DIS=11;TDS=406.4;WID=000/000/001 H;TRK=195/195;TMP=-15 / 19/01/54883;TME=00:03:00.0000000;TTM=01:06:00.0000000;FRE=6272;FUS=3648;TAS=250;GSP=250|WAP=NI306;GEO=N3208.1 E04426.8;LAT=32.1356390000;LON=44.4472780000;FRQ=;VIA=LOVE1B;ALT=151;MEA=0;GMR=18;DIS=10;TDS=416.4;WID=000/000/001 H;TRK=104/104;TMP=-15 / 19/01/54883;TME=00:03:00.0000000;TTM=01:09:00.0000000;FRE=6205;FUS=3715;TAS=250;GSP=250|WAP=ELUTA;GEO=N3205.3 E04440.1;LAT=32.0882500000;LON=44.6680560000;FRQ=;VIA=LOVE1B;ALT=101;MEA=0;GMR=18;DIS=12;TDS=428.4;WID=000/000/001 H;TRK=195/195;TMP=-05 / 19/01/54883;TME=00:03:00.0000000;TTM=01:12:00.0000000;FRE=6138;FUS=3782;TAS=250;GSP=250|WAP=DATIN;GEO=N3156.5 E04437.5;LAT=31.9425280000;LON=44.6247780000;FRQ=;VIA=LOVE1B;ALT=051;MEA=0;GMR=23;DIS=10;TDS=438.4;WID=000/000/001 H;TRK=285/285;TMP=05 / 19/01/54883;TME=00:03:00.0000000;TTM=01:15:00.0000000;FRE=6071;FUS=3849;TAS=250;GSP=250|WAP=ORNI;GEO=N3159.4 E04424.3;LAT=31.9897220000;LON=44.4041670000;FRQ=;VIA=LOVE1B;ALT=001;MEA=0;GMR=134;DIS=12;TDS=450.4;WID=;TRK=000/000;TMP=;TME=00:03:00.0000000;TTM=01:18:00.0000000;FRE=6004;FUS=3916;TAS=250;GSP=250||apln:|WAP=ORNI;GEO=N3159.4 E04424.3;LAT=31.9897220000;LON=44.4041670000;FRQ=;VIA=SEPT1A;ALT=001;MEA=0;GMR=23;DIS=0.0;TDS=0.0;WID=000/000/001 H;TRK=000/000;TMP=15 / 00/01/0;TME=00:00:00.0000000;TTM=00:00:00.0000000;FRE=6004;FUS=3916;TAS=150;GSP=0|WAP=NI222;GEO=N3157.2 E04434.5;LAT=31.9531670000;LON=44.5753330000;FRQ=;VIA=SEPT1A;ALT=101;MEA=0;GMR=23;DIS=9.0;TDS=9.0;WID=000/000/001 H;TRK=014/014;TMP=-05 / 00/01/0;TME=00:04:00.0000000;TTM=00:04:00.0000000;FRE=5462;FUS=4458;TAS=150;GSP=0|WAP=TOC;GEO=N3158.1 E04434.8;LAT=31.9693;LON=44.5801;FRQ=;VIA=SEPT1A;ALT=90;MEA=;GMR=23;DIS=1.0;TDS=10.0;WID=000/000/001 H;TRK=014/014;TMP=-05 / -02/01/0;TME=00:00:00.0000000;TTM=00:04:00.0000000;FRE=5462;FUS=4458;TAS=348;GSP=0|WAP=NI228;GEO=N3210.8 E04438.5;LAT=32.1798610000;LON=44.6426110000;FRQ=;VIA=SEPT1A;ALT=90;MEA=0;GMR=18;DIS=14.0;TDS=24.0;WID=001/001/001 H;TRK=014/014;TMP=-03 / 00/01/0;TME=00:02:00.0000000;TTM=00:06:00.0000000;FRE=5365;FUS=4555;TAS=453;GSP=0|WAP=NI229;GEO=N3214.7 E04439.7;LAT=32.2446390000;LON=44.6619170000;FRQ=;VIA=SEPT1A;ALT=90;MEA=0;GMR=18;DIS=5.0;TDS=29.0;WID=001/001/001 H;TRK=003/003;TMP=-03 / 00/01/0;TME=00:01:00.0000000;TTM=00:07:00.0000000;FRE=5331;FUS=4589;TAS=453;GSP=0|WAP=NI231;GEO=N3227.7 E04440.7;LAT=32.4613330000;LON=44.6776940000;FRQ=;VIA=SEPT1A;ALT=90;MEA=0;GMR=18;DIS=14.0;TDS=43.0;WID=001/001/001 H;TRK=003/003;TMP=-03 / 00/01/0;TME=00:02:00.0000000;TTM=00:09:00.0000000;FRE=5234;FUS=4686;TAS=453;GSP=0|WAP=TOD;GEO=N3309.5 E04443.3;LAT=33.1591;LON=44.7214;FRQ=;VIA=SEPT1A;ALT=90;MEA=;GMR=24;DIS=42.0;TDS=85.0;WID=001/001/001 H;TRK=003/003;TMP=-03 / 00/01/0;TME=00:06:00.0000000;TTM=00:15:00.0000000;FRE=4921;FUS=4999;TAS=453;GSP=453|WAP=SEPTU;GEO=N3313.0 E04444.0;LAT=33.2166670000;LON=44.7333330000;FRQ=;VIA=DCT;ALT=090;MEA=0;GMR=24;DIS=4.0;TDS=89.0;WID=001/001/001 H;TRK=000;TMP=-03 / 00/01/55089;TME=00:01:00.0000000;TTM=00:16:00.0000000;FRE=4887;FUS=5033;TAS=279;GSP=279|WAP=ORBI;GEO=N3315.8 E04414.1;LAT=33.2625390000;LON=44.2345780000;FRQ=;VIA=0;ALT=001;MEA=0;GMR=24;DIS=26.0;TDS=115.0;WID=;TRK=000;TMP=;TME=00:06:00.0000000;TTM=00:22:00.0000000;FRE=4678;FUS=5242;TAS=279;GSP=279||||cstbl:|ETN=1;APT=OIIE;ETP=KMS/ -8.00;ATI=127.200;FRQ=[APP - MEHRABAD] 119;RWY=[RW11L,13773,104] [RW11R,13425,104] [RW29L,13425,284] [RW29R,13773,284,IIKA,2] ;FUR=7352;FUQ=2122;FUD=5230;TIM=00:31;DIS=200|ETN=0;APT=ORNI;ETP=DESTINATION;ATI=123.900;FRQ=[APP - BAGHDAD] 120.;RWY=[RW10,9842,100,IALI,1] [RW28,9842,280,INJF,1] ;FUR=6004;FUQ=0;FUD=0;TIM=00:00;DIS=0||||aldrf:|240  016H   0194  - 04   01  01   18/260  017H   0150  - 03   01  01   18/280  018H   0037  - 03   01  01   18/300  018H   0085  - 01   01  02   17/320  020H   0052  - 01   01  01   17/360  021H  - 0008   00   01  01   16/|340  021H   0000   00  01   01    17/||wtdrf:|- 6   - 0239   - 4   - 0166   - 2   - 0085    2    0091    4    0187    6    0291   ||wdtmp:| HAM/ ASRIL/ KMS/ RAGET/ VAXEN/|  FL300: 248/018 (-26)  FL320: 255/022 (-30)  FL340: 263/027 (-34)  FL360: 265/026 (-39)  FL380: 266/025 (-44)  FL300: 250/021 (-26)  FL320: 255/023 (-30)  FL340: 260/025 (-34)  FL360: 261/025 (-39)  FL380: 262/025 (-44)  FL300: 253/023 (-25)  FL320: 255/023 (-29)  FL340: 256/024 (-34)  FL360: 259/023 (-39)  FL380: 262/022 (-44)  FL300: 254/020 (-24)  FL320: 252/019 (-29)  FL340: 249/018 (-34)  FL360: 254/017 (-39)  FL380: 258/016 (-44)  FL300: 250/021 (-25)  FL320: 247/020 (-29)  FL340: 243/018 (-34)  FL360: 247/017 (-39)  FL380: 252/016 (-43)||wdclb:|FL100: 140/025 FL140: 201/021 FL180: 214/023 FL220: 217/025 FL260: 244/021 FL300: 244/022 FL340: 264/031 FL380: 270/034 ||wddes:|FL380: 224/005 FL340: 257/012 FL300: 243/017 FL260: 250/016 FL220: 236/010 FL180: 205/015 FL140: 219/018 FL100: 237/018 ||icatc:|(FPL-VRH6825-IS /-B733/M-DFHIRSWY/S /-OIIE0345 /-N0445F340 PAVET1G PAVET A647 RAGET Z431 LOVEK LOVEK1B /-ORNI0118 ORBI  /-PBN/B3B4B5 DOF/240812 REG/EPVAV/ EET/ORBB0044 PER/C /-E/0324 P/TBN R/V S/ J/L D/04 160 C Y A/WHITE WITH BLUE  AND COMPANY LOGO ON THE TAIL /C/BONAKDAR)||";

            OFPSkyPuter result = context.OFPSkyPuters.FirstOrDefault(x => x.Id == id);
            var input = result.OFP;

            try
            {
                var parsed_data = ParseFlightPlan(input);
                parsed_data.RawOFPId = result.Id;

                var flight_number = parsed_data.FlightNo;
                var register = parsed_data.TailNo.Substring(2);
                var flight_date = parsed_data.GenerationDate;

                var flight = context.ViewLegTimes.Where(q => q.STDDay == flight_date && q.Register == register && q.FlightNumber == flight_number && q.FlightStatusID != 4).FirstOrDefault();
                if (flight == null)
                    return Ok("Flight Not Found");
                var fltobj = context.FlightInformations.Where(q => q.ID == flight.ID).FirstOrDefault();

                fltobj.OFPTRIPFUEL = parsed_data.fuel_trip;
                fltobj.OFPCONTFUEL = Convert.ToInt32(parsed_data.fuel_contigency);
                fltobj.OFPALT1FUEL = parsed_data.fuel_alt1;
                fltobj.OFPALT2FUEL = parsed_data.fuel_alt2;
                fltobj.OFPFINALRESFUEL = parsed_data.fuel_holding;
                fltobj.OFPETOPSADDNLFUEL = parsed_data.fuel_additional;
                fltobj.OFPOPSEXTRAFUEL = parsed_data.fuel_ops_extra;
                fltobj.OFPMINTOFFUEL = parsed_data.fuel_min_required;
                fltobj.OFPTANKERINGFUEL = parsed_data.fuel_tankering;
                fltobj.ACTUALTANKERINGFUEL = parsed_data.fuel_tankering;
                fltobj.OFPTAXIFUEL = parsed_data.fuel_taxiout;
                fltobj.OFPTOTALFUEL = parsed_data.fuel_total;
                //fltobj.OFPExtra = parsed_data.fuel_extra;




                context.OFPB_Root.Add(parsed_data);
                await context.SaveChangesAsync();


                var ofp_history = new ofpb_import_history();
                ofp_history.date_create = DateTime.Now;
                ofp_history.fight_id = flight.ID;
                ofp_history.import_id = parsed_data.Id;
                ofp_history.source_id = parsed_data.RawOFPId;

                context.ofpb_import_history.Add(ofp_history);
                await context.SaveChangesAsync();

                return Ok(parsed_data.Id);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                return Ok(e);
            }


        }

        public static OFPB_Root ParseFlightPlan(string input)
        {
            var flightPlan = new OFPB_Root();

            string[] sections = input.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var section in sections)
            {
                if (section.StartsWith("binfo:"))
                {
                    ParseBasicInfo(section, flightPlan);
                }
                else if (section.StartsWith("futbl:"))
                {
                    ParseFuelInfo(section, flightPlan);
                }
                else if (section.StartsWith("mpln:"))
                {
                    ParseMainNavLog(section, flightPlan);
                }
                else if (section.StartsWith("apln:"))
                {
                    ParseAlternateLog(section, flightPlan);
                }
                else if (section.StartsWith("wdtmp:"))
                {
                    ParseWindTemp(section, flightPlan);
                }
                else if (section.StartsWith("wdclb:"))
                {
                    ParseWindAndTempClimb(section, flightPlan);
                }
                else if (section.StartsWith("wddes:"))
                {
                    ParseWindAndTempDescent(section, flightPlan);
                }
                else if (section.StartsWith("cstbl:"))
                {
                    ParseCriticalSectorData(section, flightPlan);
                }
                else if (section.StartsWith("aldrf:"))
                {
                    ParseAltitudeDriftData(section, flightPlan);
                }
                else if (section.StartsWith("wtdrf:"))
                {
                    ParseWeightDriftData(section, flightPlan);
                }
                else if (section.StartsWith("icatc:"))
                {
                    ParseAtcFltPln(section, flightPlan);
                }
            }

            return flightPlan;
        }

        private static void ParseBasicInfo(string section, OFPB_Root flightPlan)
        {
            var info = section.Replace("binfo:|", "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in info)
            {
                var keyValue = part.Split(new[] { '=' }, 2);

                if (keyValue.Length < 2)
                    continue;

                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();

                switch (key)
                {
                    case "OPT":
                        flightPlan.AirlineName = value;
                        break;
                    case "UNT":
                        flightPlan.WeightUnit = value;
                        break;
                    case "FPF":
                        flightPlan.CruisePerformanceFactor = ExtractPercentage(value);
                        break;
                    case "FLN":
                        string pattern = @"^([A-Z]+)\s*(\d+)$";
                        Match match = Regex.Match(value, pattern);
                        flightPlan.FlightNo = match.Groups[2].Value;
                        flightPlan.airline_code = match.Groups[1].Value;
                        break;
                    case "DTE":
                        flightPlan.GenerationDate = DateTime.ParseExact(value, "MMM dd yyyy", CultureInfo.InvariantCulture);
                        break;
                    case "ETD":
                        flightPlan.ScheduledTimeDeparture = DateTime.ParseExact(value, "HH:mm", CultureInfo.InvariantCulture);
                        break;
                    case "ETA":
                        flightPlan.ScheduledTimeArrival = DateTime.ParseExact(value, "HH:mm", CultureInfo.InvariantCulture);
                        break;
                    case "REG":

                        flightPlan.TailNo = value.Split(' ')[0];
                        var type = value.Split('(')[1].Split(')')[0].Split('-');
                        flightPlan.AircraftType = type[0];
                        flightPlan.AircraftSubType = type[1];
                        flightPlan.MSN = value.Split(new[] { "MSN:" }, StringSplitOptions.None)[1].Trim();
                        break;
                    case "MCI":
                        flightPlan.CruiseSpeed = value;
                        break;
                    case "DOW":
                        flightPlan.DryOperatingWeight = int.Parse(value);
                        break;
                    case "PLD":
                        flightPlan.Payload = int.Parse(value);
                        break;
                    //case "NGM":
                    //    flightPlan.nautical_ground_miles = int.Parse(value.Split('.')[0]);
                    //    break;
                    //case "NAM":
                    //    flightPlan.nautical_air_miles = int.Parse(value);
                    //    break;
                    case "ALTN":
                        flightPlan.Alternate = value;
                        break;
                    case "CM1":
                        flightPlan.Pilot1 = value;
                        break;
                    case "CM2":
                        flightPlan.Pilot2 = value;
                        break;
                    case "DSP":
                        flightPlan.Dispatcher = value;
                        break;
                    case "CRW":
                        var crew = value.Split(new[] { '-' }, 2);
                        flightPlan.Cockpit = Int32.Parse(crew[0]);
                        flightPlan.Cabin = Int32.Parse(crew[1]);
                        break;
                    case "RTS":
                        var route = value.Replace(" ", "").Split(new[] { '-' }, 2);
                        flightPlan.Origin = route[0];
                        flightPlan.Destination = route[1];
                        break;
                    case "MZFW":
                        flightPlan.MaximumZeroFuelWeight = Int32.Parse(value);
                        break;
                    case "MTOW":
                        flightPlan.MaximumTakeoffWeight = Int32.Parse(value);
                        break;
                    case "MLDW":
                        flightPlan.MaximumLandingWeight = Int32.Parse(value);
                        break;
                    case "EZFW":
                        flightPlan.EstimatedZeroFuelWeight = Int32.Parse(value);
                        break;
                    case "ETOW":
                        flightPlan.EstimatedTakeoffWeight = Int32.Parse(value);
                        break;
                    case "ELDW":
                        flightPlan.EstimatedLandingWeight = Int32.Parse(value);
                        break;
                    case "FLL":
                        flightPlan.MainFlightLevel = Int32.Parse(value);
                        break;
                    case "RTM":
                        flightPlan.MainRoute = value;
                        break;
                    case "VDT":
                        int validUntilIndex = value.IndexOf("Valid until:");
                        string dateTimeString = value.Substring(validUntilIndex + "Valid until:".Length).Trim();
                        DateTime validUntilDateTime;
                        DateTime.TryParseExact(dateTimeString, "dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out validUntilDateTime);

                        flightPlan.PlanValidity = validUntilDateTime;
                        break;

                }
            }
        }

        private static void ParseFuelInfo(string section, OFPB_Root flightPlan)
        {
            var info = section.Replace("futbl:|", "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in info)
            {
                var keyValue = part.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                var kvp = keyValue[2].Split(new[] { '=' }, 2);
                var kvt = keyValue[1].Split(new[] { '=' }, 2);
                TimeSpan timeSpan;
                TimeSpan.TryParse(kvt[1], out timeSpan);
                int time = (int)timeSpan.TotalMinutes;

                var value = int.Parse(kvp[1].Trim());
                switch (keyValue[0])
                {
                    case "PRM=TRIP FUEL":
                        flightPlan.fuel_trip = value;
                        flightPlan.time_trip = time;
                        break;
                    case "PRM=TOTAL FUEL":
                        flightPlan.fuel_total = value;
                        flightPlan.time_total = time;
                        break;
                    case "PRM=CONT[5%]":
                        string pattern = @"\[(\d+)%\]";
                        Match match = Regex.Match(keyValue[0], pattern);
                        var ContPCT = Int32.Parse(match.Groups[1].Value);
                        flightPlan.ContingencyPercent = ContPCT;
                        flightPlan.fuel_contigency = value;
                        flightPlan.time_contigency = time;
                        break;
                    case "PRM=ALTN 1":
                        flightPlan.fuel_alt1 = value;
                        flightPlan.time_alt1 = time;
                        break;
                    case "PRM=ALTN 2":
                        flightPlan.fuel_alt2 = value;
                        flightPlan.time_alt2 = time;
                        break;
                    case "PRM=ETOPS/ADDNL":
                        flightPlan.fuel_additional = value;
                        flightPlan.time_additional = time;
                        break;
                    //case "PRM=EXTRA":
                    //    flightPlan.fuel_extra = value;
                    //    break;
                    case "PRM=FINAL RES":
                        flightPlan.fuel_holding = value;
                        flightPlan.time_holding = time;
                        break;
                    case "PRM=TAXI":
                        flightPlan.fuel_taxiout = value;
                        flightPlan.time_taxi_out = time;
                        break;
                    case "PRM=EZFW":
                        flightPlan.EstimatedZeroFuelWeight = value;
                        break;
                    case "PRM=ETOW":
                        flightPlan.EstimatedTakeoffWeight = value;
                        break;
                    case "PRM=ELW":
                        flightPlan.EstimatedLandingWeight = value;
                        break;
                    case "PRM=MIN TOF":
                        flightPlan.fuel_min_tof = value;
                        break;
                    case "PRM=OPS.EXTRA":
                        flightPlan.fuel_ops_extra = value;
                        break;
                    case "PRM=TANKERING":
                        flightPlan.fuel_min_tof = value;
                        break;

                }
            }
        }

        private static void ParseMainNavLog(string section, OFPB_Root flightPlan)
        {
            var navLogEntries = section.Replace("mpln:|", "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            flightPlan.OFPB_MainNavLog = new List<OFPB_MainNavLog>();

            for (int i = 0; i < navLogEntries.Length; i++)
            {

                var navLog = new OFPB_MainNavLog();
                var keyValuePairs = navLogEntries[i].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in keyValuePairs)
                {
                    var kvp = item.Split(new[] { '=' }, 2);

                    if (kvp.Length < 2)
                        continue;

                    var key = kvp[0].Trim();
                    var value = kvp[1].Trim();

                    switch (key)
                    {
                        case "WAP":
                            navLog.WayPoint = value;
                            break;
                        case "LAT":
                            navLog.Latitude = float.Parse(value);
                            break;
                        case "LON":
                            navLog.Longitude = float.Parse(value);
                            break;
                        case "ALT":
                            navLog.FlightLevel = value;
                            break;
                        case "MEA":
                            navLog.MEA = value;
                            break;
                        //case "GMR":
                        //    navLog.GMR = value;
                        //    break;
                        case "DIS":
                            navLog.ZoneDistance = float.Parse(value);
                            break;
                        case "WID":
                            navLog.Wind = value;
                            break;
                        case "TRK":
                            navLog.MagneticTrack = value;

                            break;
                        case "FUS":
                            navLog.FuelUsed = float.Parse(value);
                            break;
                        case "FRE":
                            navLog.FuelRemained = float.Parse(value);
                            break;
                        case "TMP":
                            navLog.Temperature = value;
                            break;
                        case "TTM":
                            navLog.CumulativeTime = value;
                            break;
                        case "TME":
                            navLog.ZoneTime = value;
                            break;
                        case "FRQ":
                            navLog.MagneticTrack = value;
                            break;
                        case "TAS":
                            navLog.TrueAirSpeed = float.Parse(value);
                            break;
                        case "GSP":
                            navLog.GroundSpeed = float.Parse(value);
                            break;
                        case "TDS":
                            navLog.CumulativeDistance = float.Parse(value);
                            if (navLogEntries.Length - 1 == i)
                                flightPlan.dis_trip = Convert.ToInt32(Math.Floor(float.Parse(value)));
                            break;
                    }
                }

                flightPlan.OFPB_MainNavLog.Add(navLog);


            }
        }

        private static void ParseAlternateLog(string section, OFPB_Root flightPlan)
        {
            var navLogEntries = section.Replace("apln:|", "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            flightPlan.OFPB_Alternate1NavLog = new List<OFPB_Alternate1NavLog>();

            for (int i = 0; i < navLogEntries.Length; i++)
            {

                var navLog = new OFPB_Alternate1NavLog();
                var keyValuePairs = navLogEntries[i].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in keyValuePairs)
                {
                    var kvp = item.Split(new[] { '=' }, 2);

                    if (kvp.Length < 2)
                        continue;

                    var key = kvp[0].Trim();
                    var value = kvp[1].Trim();

                    switch (key)
                    {
                        case "WAP":
                            navLog.WayPoint = value;
                            break;
                        case "LAT":
                            navLog.Latitude = float.Parse(value);
                            break;
                        case "LON":
                            navLog.Longitude = float.Parse(value);
                            break;
                        case "ALT":
                            navLog.FlightLevel = value;
                            break;
                        case "MEA":
                            navLog.MEA = value;
                            break;
                        //case "GMR":
                        //    navLog.GMR = value;
                        //    break;
                        case "DIS":
                            navLog.ZoneDistance = float.Parse(value);
                            break;
                        case "WID":
                            navLog.Wind = value;
                            break;
                        case "TRK":
                            navLog.MagneticTrack = value;
                            break;
                        case "FRQ":
                            navLog.MagneticTrack = value;
                            break;
                        case "FUS":
                            navLog.FuelUsed = float.Parse(value);
                            break;
                        case "FRE":
                            navLog.FuelRemained = float.Parse(value);
                            break;
                        case "TMP":
                            navLog.Temperature = value;
                            break;
                        case "TTM":
                            navLog.CumulativeTime = value;
                            break;
                        case "TAS":
                            navLog.TrueAirSpeed = float.Parse(value);
                            break;
                        case "GSP":
                            navLog.GroundSpeed = float.Parse(value);
                            break;
                        case "TME":
                            navLog.ZoneTime = value;
                            break;
                        case "TDS":
                            navLog.CumulativeDistance = float.Parse(value);
                            if (navLogEntries.Length - 1 == i)
                                flightPlan.dis_alt1 = Convert.ToInt32(Math.Floor(float.Parse(value)));
                            break;
                    }
                }
                flightPlan.OFPB_Alternate1NavLog.Add(navLog);
            }
        }
        private static void ParseWindTemp(string section, OFPB_Root flightPlan)
        {

            var wt = section.Replace("wdtmp:|", "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            flightPlan.OFPB_WindTemperature = new List<OFPB_WindTemperature>();



            var locations = wt[0].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> fl = wt[1].Trim().Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < locations.Length; i++)
            {


                var dataPairs = fl.GetRange(i * 5, 5);

                foreach (var dataPair in dataPairs)
                {
                    var windAndTemp = new OFPB_WindTemperature();
                    windAndTemp.WayPoint = locations[i];

                    var parts = dataPair.Split(new[] { ':' }, 2);
                    if (parts.Length < 2) continue;

                    var flightLevel = parts[0].Trim();
                    var values = parts[1].Trim().Split(new[] { ' ' }, 2);
                    var wind = values[0];
                    var temp = values[1];

                    windAndTemp.FlightLevel = flightLevel;
                    windAndTemp.WindTemprature = temp;
                    windAndTemp.Wind = wind;


                    //switch (flightLevel)
                    //{
                    //    case "FL300":
                    //        windAndTemp.FL300 = values;
                    //        break;
                    //    case "FL340":
                    //        windAndTemp.FL340 = values;
                    //        break;
                    //    case "FL380":
                    //        windAndTemp.FL380 = values;
                    //        break;

                    //}

                    flightPlan.OFPB_WindTemperature.Add(windAndTemp);
                }


            }
        }

        private static void ParseWindAndTempClimb(string section, OFPB_Root flightPlan)
        {
            var wt = section.Replace("wdclb:|", "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var windAndTemp = new OFPB_WindTemperature
            {
                WayPoint = "CLB"
            };


            var fl = wt[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> dataPairs = new List<string>();


            for (int i = 0; i < fl.Length; i += 2)
            {
                string keyValuePair = fl[i] + fl[i + 1];
                dataPairs.Add(keyValuePair);
            }

            foreach (var dataPair in dataPairs)
            {
                var parts = dataPair.Split(new[] { ':' }, 2);
                if (parts.Length < 2) continue;

                var flightLevel = parts[0].Trim();
                var values = parts[1].Trim();

                windAndTemp.FlightLevel = flightLevel;
                windAndTemp.Wind = values;

                //switch (flightLevel)
                //{
                //    case "FL100":
                //        windAndTemp.FL100 = values;
                //        break;
                //    case "FL140":
                //        windAndTemp.FL140 = values;
                //        break;
                //    case "FL180":
                //        windAndTemp.FL180 = values;
                //        break;
                //    case "FL220":
                //        windAndTemp.FL220 = values;
                //        break;
                //    case "FL260":
                //        windAndTemp.FL260 = values;
                //        break;
                //    case "FL300":
                //        windAndTemp.FL300 = values;
                //        break;
                //    case "FL340":
                //        windAndTemp.FL340 = values;
                //        break;
                //    case "FL380":
                //        windAndTemp.FL380 = values;
                //        break;
                //}
            }

            flightPlan.OFPB_WindTemperature.Add(windAndTemp);
        }

        private static void ParseWindAndTempDescent(string section, OFPB_Root flightPlan)
        {
            var wt = section.Replace("wddes:|", "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var windAndTemp = new OFPB_WindTemperature
            {
                WayPoint = "DES"
            };


            var fl = wt[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> dataPairs = new List<string>();

            for (int i = 0; i < fl.Length; i += 2)
            {
                string keyValuePair = fl[i] + fl[i + 1];
                dataPairs.Add(keyValuePair);
            }

            foreach (var dataPair in dataPairs)
            {
                var parts = dataPair.Split(new[] { ':' }, 2);
                if (parts.Length < 2) continue;

                var flightLevel = parts[0].Trim();
                var values = parts[1].Trim();

                windAndTemp.FlightLevel = flightLevel;
                windAndTemp.Wind = values;

                //switch (flightLevel)
                //{
                //    case "FL100":
                //        windAndTemp.FL100 = values;
                //        break;
                //    case "FL140":
                //        windAndTemp.FL140 = values;
                //        break;
                //    case "FL180":
                //        windAndTemp.FL180 = values;
                //        break;
                //    case "FL220":
                //        windAndTemp.FL220 = values;
                //        break;
                //    case "FL260":
                //        windAndTemp.FL260 = values;
                //        break;
                //    case "FL300":
                //        windAndTemp.FL300 = values;
                //        break;
                //    case "FL340":
                //        windAndTemp.FL340 = values;
                //        break;
                //    case "FL380":
                //        windAndTemp.FL380 = values;
                //        break;
                //}
            }

            flightPlan.OFPB_WindTemperature.Add(windAndTemp);
        }

        private static void ParseCriticalSectorData(string section, OFPB_Root flightPlan)
        {
            var sectors = section.Replace("cstbl:|", "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            flightPlan.OFPB_CriticalSection = new List<OFPB_CriticalSection>();

            foreach (var sector in sectors)
            {
                if (string.IsNullOrWhiteSpace(sector)) continue;

                var criticalSectorData = new OFPB_CriticalSection();
                criticalSectorData.OFPB_CS_Runway = new List<OFPB_CS_Runway>();

                var properties = sector.Split(';');

                foreach (var property in properties)
                {
                    if (property.Contains("ETN="))
                    {
                        criticalSectorData.etp_no = int.Parse(property.Split('=')[1]);
                    }
                    else if (property.Contains("APT="))
                    {
                        criticalSectorData.airport = property.Split('=')[1];
                    }
                    //else if (property.Contains("ETP="))
                    //{
                    //    criticalSectorData.etp = property.Split('=')[1];
                    //}
                    else if (property.Contains("ATI="))
                    {
                        criticalSectorData.atis = property.Split('=')[1];
                    }
                    else if (property.Contains("RWY="))
                    {
                        var runwaysStr = property.Split('=')[1].Trim();
                        var runways = runwaysStr.Split(new[] { ']' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var runway in runways)
                        {
                            var runwayData = runway.Trim().TrimStart('[').Split(',');
                            var rwy = new OFPB_CS_Runway()
                            {
                                runway = runwayData[0],
                                length = double.Parse(runwayData[1]),
                                mb = double.Parse(runwayData[2])
                            };


                            criticalSectorData.OFPB_CS_Runway.Add(rwy);
                        }
                    }
                    else if (property.Contains("FUR="))
                    {
                        criticalSectorData.remaining_fuel = property.Split('=')[1];
                    }
                    else if (property.Contains("FUQ="))
                    {
                        criticalSectorData.required_fuel = property.Split('=')[1];
                    }
                    else if (property.Contains("FUD="))
                    {
                        criticalSectorData.difference_fuel = property.Split('=')[1];
                    }
                    else if (property.Contains("TIM="))
                    {
                        criticalSectorData.time = property.Split('=')[1];
                    }
                    else if (property.Contains("DIS="))
                    {
                        criticalSectorData.dis_nautical_mile = property.Split('=')[1];
                    }
                }

                flightPlan.OFPB_CriticalSection.Add(criticalSectorData);
            }
        }

        private static void ParseAltitudeDriftData(string section, OFPB_Root flightPlan)
        {
            var segments = section.Replace("aldrf:|", "").Replace("|", "").Replace("- ", "-").Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            flightPlan.OFPB_AltDrift = new List<OFPB_AltDrift>();

            foreach (var segment in segments)
            {
                var parts = segment.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < parts.Length; i += 7)
                {
                    var altitudeDrift = new OFPB_AltDrift
                    {
                        flight_level = int.Parse(parts[i]),
                        avg_wind = parts[i + 1],
                        fuel_delta = int.Parse(parts[i + 2]),
                        temp_delta = int.Parse(parts[i + 3]),
                        shear_delta = int.Parse(parts[i + 4]),
                        mach_shear = int.Parse(parts[i + 5]),
                        isa_dev = int.Parse(parts[i + 6])
                    };

                    flightPlan.OFPB_AltDrift.Add(altitudeDrift);
                }
            }


        }

        private static void ParseWeightDriftData(string section, OFPB_Root flightPlan)
        {
            var parts = section.Replace("wtdrf:|", "").Split(new[] { "   " }, StringSplitOptions.RemoveEmptyEntries);
            flightPlan.OFPB_WeightDrift = new List<OFPB_WeightDrift>();

            for (int i = 0; i < parts.Length; i += 2)
            {
                int offSet = int.Parse(parts[i].Replace(" ", ""));
                string fuelDelta = parts[i + 1].Replace(" ", "");
                var weightDrift = new OFPB_WeightDrift
                {
                    off_set = offSet.ToString(),
                    fuel_delta = fuelDelta
                };

                flightPlan.OFPB_WeightDrift.Add(weightDrift);
            }


        }

        private static void ParseAtcFltPln(string sections, OFPB_Root flightPlan)
        {
            var fltPlan = sections.Replace("icatc:|", "").Replace("/", "\n");
            flightPlan.ICAOFlightPlan = fltPlan;
        }

        private static int ExtractPercentage(string value)
        {
            var start = value.IndexOf("INCLUDES") + 9;
            var end = value.IndexOf("PCT");
            var percentage = value.Substring(start, end - start - 3).Trim();
            return Int32.Parse(percentage);
        }
    }
}