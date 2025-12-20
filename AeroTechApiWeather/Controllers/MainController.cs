using AeroTechApiWeather.Model;
using AeroTechApiWeather.Models;
using AeroTechApiWeather.Rendering;
using AeroTechApiWeather.Services;
using AeroTechApiWeather.ViewModels;
using Grib.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AeroTechApiWeather.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MainController : ApiController
    {

        //[Route("api/download")]
        //[AcceptVerbs("GET")]
        //public async Task<IHttpActionResult> saveFDPNoCrew()
        //{
        //    var downloader = new  GfsDownloader(@"C:\Users\vahid\Desktop\amw\gfs");

        //    DateTime flightDepUtc = new DateTime(2025, 12, 3, 8, 15, 0, DateTimeKind.Utc);

        //    var cts = new CancellationTokenSource();

        //    string gribPath = await downloader.DownloadForFlightAsync(flightDepUtc, cts.Token);
        //    return Ok(true);
        //}

        [Route("api/test")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> test()
        {


            //string gfsRoot = @"C:\Users\vahid\Desktop\amw\gfs"; //Path.Combine(Environment.CurrentDirectory, "GfsData");

            //var locator = new GfsFileLocator(gfsRoot);
            //var info = locator.GetFileForFlight(DateTime.UtcNow);

            //var cts = new CancellationTokenSource();

            //// اینجاست که فقط متد دانلود صدا زده می‌شه
            //string localPath = await HttpDownloadHelper.DownloadFileAsync(
            //    info.RemoteUrl,
            //    info.LocalPath,
            //    cts.Token);



            var fp = new FlightPlan
            {
                FlightId = "1359",
                CruiseFlightLevel = 350,
                DepartureTimeUtc = new DateTime(2025, 12, 12, 5, 0, 0, DateTimeKind.Utc),
                Route = new List<Waypoint>
                {
 /*new Waypoint { Name ="UDYZ" , Lat = 40.147200, Lon = 44.395800 },
new Waypoint { Name ="GOSIS" , Lat = 39.947200, Lon = 44.986100 },
new Waypoint { Name ="ADANO" , Lat = 39.666700, Lon = 45.833300 },
new Waypoint { Name ="-TOC-" , Lat = 39.492200, Lon = 45.960200 },
new Waypoint { Name ="MAGRI" , Lat = 38.902200, Lon = 46.383300 },
new Waypoint { Name ="DASDA" , Lat = 38.693100, Lon = 46.870600 },
new Waypoint { Name ="SIVIT" , Lat = 37.598100, Lon = 49.086400 },
new Waypoint { Name ="RST" , Lat = 37.326300, Lon = 49.615900 },
new Waypoint { Name ="DEDLA" , Lat = 36.938900, Lon = 50.012200 },
new Waypoint { Name ="-TOD-" , Lat = 36.722400, Lon = 50.121100 },
new Waypoint { Name ="PAXID" , Lat = 36.284200, Lon = 50.339200 },
new Waypoint { Name ="OIIE" , Lat = 35.416100, Lon = 51.152200 },*/
new Waypoint { Name ="OIIE" , Lat = 35.4166670000, Lon = 51.1502777777 },
new Waypoint { Name ="IKA" , Lat = 35.4016670000, Lon = 51.1686111111 },
new Waypoint { Name ="PAROT" , Lat = 36.1847220000, Lon = 49.9686111111 },
new Waypoint { Name ="TOC" , Lat = 36.8019440000, Lon = 48.9002777777 },
new Waypoint { Name ="ASPOK" , Lat = 36.9841670000, Lon = 48.8188888888 },
new Waypoint { Name ="MURPU" , Lat = 37.5019440000, Lon = 48.0508333333 },
new Waypoint { Name ="BUDED" , Lat = 37.8838890000, Lon = 47.3347222222 },
new Waypoint { Name ="RAKED" , Lat = 37.9341670000, Lon = 47.1172222222 },
new Waypoint { Name ="TBZ" , Lat = 38.1358330000, Lon = 46.2022222222 },
new Waypoint { Name ="VUVAG" , Lat = 38.4180560000, Lon = 45.4844444444 },
new Waypoint { Name ="BORES" , Lat = 38.4680560000, Lon = 45.3516666666 },
new Waypoint { Name ="FIR-ANKARA" , Lat = 38.8836110000, Lon = 44.2677777777 },
new Waypoint { Name ="DASIS" , Lat = 38.9013890000, Lon = 44.2013888888 },
new Waypoint { Name ="DORUK" , Lat = 39.2688890000, Lon = 42.1836111111 },
new Waypoint { Name ="LATSU" , Lat = 39.4175000000, Lon = 41.4022222222 },
new Waypoint { Name ="NEGOL" , Lat = 39.4683330000, Lon = 41.0686111111 },
new Waypoint { Name ="ETP 1" , Lat = 39.5341670000, Lon = 40.7183333333 },
new Waypoint { Name ="GELSU" , Lat = 39.5513890000, Lon = 40.5511111111 },
new Waypoint { Name ="ERN" , Lat = 39.7013890000, Lon = 39.5188888888 },
new Waypoint { Name ="EBEDI" , Lat = 39.7180560000, Lon = 39.1177777777 },
new Waypoint { Name ="ELNEM" , Lat = 39.7511110000, Lon = 38.1858333333 },
new Waypoint { Name ="SIV" , Lat = 39.7844440000, Lon = 36.8850000000 },
new Waypoint { Name ="OLPOT" , Lat = 39.8519440000, Lon = 36.3516666666 },
new Waypoint { Name ="ENFOR" , Lat = 39.8677780000, Lon = 36.2680555555 },
new Waypoint { Name ="HAKCE" , Lat = 39.9169440000, Lon = 35.9338888888 },
new Waypoint { Name ="TIMOP" , Lat = 39.9847220000, Lon = 35.3519444444 },
new Waypoint { Name ="BENTA" , Lat = 40.1502780000, Lon = 33.9508333333 },
new Waypoint { Name ="GURBU" , Lat = 40.1669440000, Lon = 33.8333333333 },
new Waypoint { Name ="BUK" , Lat = 40.2347220000, Lon = 33.1008333333 },
new Waypoint { Name ="SALGO" , Lat = 40.4677780000, Lon = 32.2000000000 },
new Waypoint { Name ="YAVRU" , Lat = 40.5691670000, Lon = 31.8333333333 },
new Waypoint { Name ="ATGOB" , Lat = 40.7180560000, Lon = 31.2500000000 },
new Waypoint { Name ="FIR-ISTANBUL" , Lat = 40.7838890000, Lon = 31.0000000000 },
new Waypoint { Name ="ERSEN" , Lat = 40.8525000000, Lon = 30.6666666666 },
new Waypoint { Name ="FM520" , Lat = 40.8838890000, Lon = 30.4505555555 },
new Waypoint { Name ="FM521" , Lat = 40.9847220000, Lon = 30.2858333333 },
new Waypoint { Name ="FM522" , Lat = 41.0852780000, Lon = 30.1350000000 },
new Waypoint { Name ="FM523" , Lat = 40.9691670000, Lon = 30.0019444444 },
new Waypoint { Name ="FM524" , Lat = 40.8669440000, Lon = 29.8688888888 },
new Waypoint { Name ="TOD" , Lat = 40.8669440000, Lon = 29.8022222222 },
new Waypoint { Name ="FM525" , Lat = 40.8666670000, Lon = 29.7352777777 },
new Waypoint { Name ="FM526" , Lat = 40.8666670000, Lon = 29.6350000000 },
new Waypoint { Name ="FM530" , Lat = 40.8833330000, Lon = 29.4672222222 },
new Waypoint { Name ="FM531" , Lat = 40.9680560000, Lon = 29.4522222222 },
new Waypoint { Name ="FM532" , Lat = 41.0522220000, Lon = 29.4183333333 },
new Waypoint { Name ="FM533" , Lat = 41.1344440000, Lon = 29.3516666666 },
new Waypoint { Name ="FM534" , Lat = 41.2000000000, Lon = 29.2675000000 },
new Waypoint { Name ="FM535" , Lat = 41.1005560000, Lon = 29.1833333333 },
new Waypoint { Name ="FM536" , Lat = 41.0013890000, Lon = 29.0855555555 },
new Waypoint { Name ="SADIK" , Lat = 40.9019440000, Lon = 29.0016666666 },
new Waypoint { Name ="AFDUT" , Lat = 40.8836110000, Lon = 28.8186111111 },
new Waypoint { Name ="OZBEQ" , Lat = 40.9669440000, Lon = 28.7675000000 },
new Waypoint { Name ="GAPDI" , Lat = 41.0186110000, Lon = 28.7191666666 },
new Waypoint { Name ="LTFM" , Lat = 41.2680560000, Lon = 28.7502777777 },
                }
            };

            // 2) مسیر ذخیره دیتا و خروجی عکس‌ها
            string gfsRoot = Path.Combine(@"C:\Users\vahid\Desktop\amw", "gfs");
            string imgRoot = Path.Combine(@"C:\Users\vahid\Desktop\amw", "Images");

            // 3) FLهای تست (هر تعداد دلخواه)
            // مثل PPS: پایین‌تر، کروز، بالاتر + چندتا اضافه برای تست
            int baseFl = fp.CruiseFlightLevel;
            int[] fls =
            {
               // baseFl - 20,  // پایین‌تر
                baseFl - 10,
                baseFl,       // کروز
                baseFl + 10,
               // baseFl + 20   // بالاتر
            };

            // 4) اجرای سرویس
            var service = new WindTempMapService(gfsRoot, imgRoot)
            {
                ShowTailHeadwindLabels = true,
                 EnableBasemap = true,
                BasemapLandShp = @"C:\Users\vahid\Desktop\amw\shapes\ne_50m_land\ne_50m_land.shp",
                BasemapOceanShp = @"C:\Users\vahid\Desktop\amw\shapes\ne_50m_ocean\ne_50m_ocean.shp",
                BasemapLakesShp = @"C:\Users\vahid\Desktop\amw\shapes\ne_50m_lakes\ne_50m_lakes.shp",
                BasemapBordersShp = @"C:\Users\vahid\Desktop\amw\shapes\ne_50m_admin_0_boundary_lines_land\ne_50m_admin_0_boundary_lines_land.shp",
            };

            using (var cts = new CancellationTokenSource())
            {
                Console.WriteLine("[Sample] Starting generation...");
                await service.GenerateRouteMapsAsync(fp, fls, cts.Token);
                Console.WriteLine("[Sample] Finished.");
                Console.WriteLine("[Sample] Output folder: " + imgRoot);
            }

            ////cross
            ///

            var crossService = new CrossSectionMapService(gfsRoot, imgRoot)
            {
                InsertStepNm = 120,       // هر 120NM یک ستون میانی
                MinLegNmForInsert = 150,  // فقط legهای بالای 150NM

                Width = 1400,
                Height = 900,

                ShowTemperature = true,
                ShowWindBarbs = true,
                ShowHeadTailTable = true,
                ShowFlightProfile = true
            };
            int[] fls_crs = { 50, 100, 140, 180, 240, 300, 340, 350, 360, 390, 450, 530 };
            using (var cts = new CancellationTokenSource())
            {
                Console.WriteLine("[Sample] Starting CROSS generation...");
                string outPng = await crossService.GenerateCrossSectionAsync(fp, fls_crs, cts.Token);
                Console.WriteLine("[Sample] CROSS saved: " + outPng);
            }


            return Ok(true);
        }


        //[Route("api/fuel/report")]
        //public async Task<IHttpActionResult> GetFuelReport(DateTime dfrom, DateTime dto)
        //{
        //    try
        //    {
        //        dfrom = dfrom.Date;
        //        dto = dto.Date.AddDays(1);
        //        ppa_entities context = new ppa_entities();
        //        var query = from x in context.AppFuels
        //                    where x.STDDay >= dfrom && x.STDDay <= dto
        //                    select x;

        //        var result = await query.OrderBy(q => q.STD).ToListAsync();


        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += "   " + ex.InnerException.Message;
        //        return Ok(msg);
        //    }

        //}


        //[Route("api/roster/fdp/nocrew/save")]
        //[AcceptVerbs("POST")]
        //public async Task<IHttpActionResult> saveFDPNoCrew(dynamic dto)
        //{
        //    var context = new Models.dbEntities();

        //    var userId = Convert.ToInt32(dto.userId);
        //    var flightId = Convert.ToInt32(dto.flightId);
        //    var code = Convert.ToString(dto.code);
        //    var fdp = new FDP()
        //    {
        //        IsTemplate = false,
        //        DutyType = 1165,
        //        CrewId = userId,
        //        GUID = Guid.NewGuid(),
        //        JobGroupId = RosterFDPDto.getRank(code),
        //        FirstFlightId = flightId,
        //        LastFlightId = flightId,

        //        Split = 0,



        //    };

        //    fdp.FDPItems.Add(new FDPItem()
        //    {
        //        FlightId = flightId,
        //        IsPositioning = false,
        //        IsSector = false,
        //        PositionId = RosterFDPDto.getRank(code),
        //        RosterPositionId = 1,

        //    });

        //    context.FDPs.Add(fdp);
        //    var saveResult = await context.SaveChangesAsync();




        //    return Ok(fdp.Id);
        //}


    }
}
