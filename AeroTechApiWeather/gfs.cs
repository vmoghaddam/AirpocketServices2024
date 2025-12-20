using AeroTechApiWeather.Gfs;
using AeroTechApiWeather.Model;
using AeroTechApiWeather.Rendering;
using AeroTechApiWeather.Utils;
using Grib.Api;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AeroTechApiWeather
{
    ///// <summary>
    ///// Downloads GFS GRIB2 files (0.25 degree) from NOAA NOMADS servers.
    ///// Compatible with .NET Framework 4.8.
    ///// </summary>
    //public class GfsDownloader : IDisposable
    //{
    //    private readonly HttpClient _httpClient;

    //    /// <summary>
    //    /// Base URL for GFS products.
    //    /// Default is NOMADS operational server.
    //    /// </summary>
    //    public string BaseUrl { get; set; } =
    //        "https://nomads.ncep.noaa.gov/pub/data/nccf/com/gfs/prod";

    //    /// <summary>
    //    /// Local folder where GRIB files are stored.
    //    /// </summary>
    //    public string LocalRoot { get; }

    //    /// <summary>
    //    /// Horizontal resolution string used in file name (0p25, 0p50, 1p00, ...).
    //    /// </summary>
    //    public string ResolutionTag { get; set; } = "0p25";

    //    /// <summary>
    //    /// Max forecast hour allowed (default 240 = 10 days).
    //    /// </summary>
    //    public int MaxForecastHour { get; set; } = 240;

    //    static GfsDownloader()
    //    {
    //        // Ensure TLS 1.2 support (needed on older Windows / .NET)
    //        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
    //    }

    //    public GfsDownloader(string localRoot)
    //    {
    //        if (string.IsNullOrWhiteSpace(localRoot))
    //            throw new ArgumentNullException("localRoot");

    //        LocalRoot = localRoot;

    //       // Directory.CreateDirectory(LocalRoot);

    //        var handler = new HttpClientHandler
    //        {
    //            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    //        };

    //        _httpClient = new HttpClient(handler);
    //        _httpClient.Timeout = TimeSpan.FromMinutes(5);
    //        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GfsDownloader/1.0");
    //    }

    //    /// <summary>
    //    /// Calculates best GFS run time and forecast hour for a given flight time.
    //    /// </summary>
    //    public GfsRunInfo GetBestRun(DateTime flightTimeUtc, int stepHours = 3)
    //    {
    //        if (flightTimeUtc.Kind != DateTimeKind.Utc)
    //            flightTimeUtc = DateTime.SpecifyKind(flightTimeUtc, DateTimeKind.Utc);

    //        int[] runHours = { 0, 6, 12, 18 };

    //        var date = flightTimeUtc.Date;
    //        DateTime? bestRunTime = null;

    //        foreach (var rh in runHours.OrderByDescending(h => h))
    //        {
    //            var candidate = new DateTime(date.Year, date.Month, date.Day, rh, 0, 0, DateTimeKind.Utc);
    //            if (candidate <= flightTimeUtc)
    //            {
    //                bestRunTime = candidate;
    //                break;
    //            }
    //        }

    //        if (!bestRunTime.HasValue)
    //        {
    //            // Use previous day 18Z
    //            var prevDate = date.AddDays(-1);
    //            bestRunTime = new DateTime(prevDate.Year, prevDate.Month, prevDate.Day, 18, 0, 0, DateTimeKind.Utc);
    //        }

    //        var diffHours = (flightTimeUtc - bestRunTime.Value).TotalHours;

    //        int forecastHour = (int)(Math.Round(diffHours / stepHours) * stepHours);
    //        if (forecastHour < 0) forecastHour = 0;
    //        if (forecastHour > MaxForecastHour) forecastHour = MaxForecastHour;

    //        return new GfsRunInfo
    //        {
    //            RunTimeUtc = bestRunTime.Value,
    //            ForecastHour = forecastHour
    //        };
    //    }

    //    /// <summary>
    //    /// Downloads (or reuses existing) GFS GRIB2 file for a given flight time.
    //    /// Returns full local path of the GRIB file.
    //    /// </summary>
    //    public Task<string> DownloadForFlightAsync(DateTime flightTimeUtc, CancellationToken cancellationToken)
    //    {
    //        var runInfo = GetBestRun(flightTimeUtc);
    //        return DownloadAsync(runInfo, cancellationToken);
    //    }

    //    /// <summary>
    //    /// Downloads GFS GRIB2 file for a given run info.
    //    /// </summary>
    //    public async Task<string> DownloadAsync(GfsRunInfo runInfo, CancellationToken cancellationToken)
    //    {
    //        string datePart = runInfo.RunTimeUtc.ToString("yyyyMMdd");
    //        string hourPart = runInfo.RunTimeUtc.ToString("HH");          // 00, 06, 12, 18
    //        string forecastPart = runInfo.ForecastHour.ToString("000");   // 000, 003, 006, ...

    //        string fileName = string.Format(
    //            "gfs.t{0}z.pgrb2.{1}.f{2}",
    //            hourPart,
    //            ResolutionTag,
    //            forecastPart);

    //        string remoteUrl = string.Format(
    //            "{0}/gfs.{1}/{2}/atmos/{3}",
    //            BaseUrl.TrimEnd('/'),
    //            datePart,
    //            hourPart,
    //            fileName);

    //        string dayFolder = Path.Combine(LocalRoot, datePart, hourPart);
    //        Directory.CreateDirectory(dayFolder);

    //        string localPath = Path.Combine(dayFolder, fileName);

    //        // Reuse if already downloaded
    //        if (File.Exists(localPath))
    //            return localPath;

    //        using (var response = await _httpClient.GetAsync(remoteUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
    //        {
    //            if (!response.IsSuccessStatusCode)
    //            {
    //                var msg = string.Format("Failed to download GFS file. HTTP {0} {1}. URL={2}",
    //                    (int)response.StatusCode,
    //                    response.ReasonPhrase,
    //                    remoteUrl);

    //                throw new InvalidOperationException(msg);
    //            }

    //            using (var stream = await response.Content.ReadAsStreamAsync())
    //            using (var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None))
    //            {
    //                await stream.CopyToAsync(fs);
    //            }
    //        }

    //        return localPath;
    //    }

    //    public void Dispose()
    //    {
    //        _httpClient.Dispose();
    //    }
    //}

    ///// <summary>
    ///// Info about a specific GFS model run and forecast hour.
    ///// </summary>
    //public class GfsRunInfo
    //{
    //    public DateTime RunTimeUtc { get; set; }   // e.g. 2025-12-03 06:00Z
    //    public int ForecastHour { get; set; }      // e.g. 3, 6, 9, ...
    //}




    public static class HttpDownloadHelper
    {
        static HttpDownloadHelper()
        {
            // برای اطمینان از TLS 1.2 روی سرورهای جدید
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Downloads a file from url to localPath (if not already exists).
        /// Returns the localPath.
        /// </summary>
        public static async Task<string> DownloadFileAsync(
            string url,
            string localPath,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(localPath))
                throw new ArgumentNullException(nameof(localPath));

            // اگر قبلاً دانلود شده، همونو برگردون
            if (File.Exists(localPath))
                return localPath;

            var dir = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client = new HttpClient(handler))
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("WxDemo-Downloader/1.0");

                using (var response = await client.GetAsync(
                           url,
                           HttpCompletionOption.ResponseHeadersRead,
                           cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException(
                            $"Download failed. HTTP {(int)response.StatusCode} {response.ReasonPhrase}. URL={url}");
                    }

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }
            }

            return localPath;
        }
    }

 
        public class GfsRunInfo
        {
            public DateTime RunTimeUtc { get; set; }   // e.g. 2025-12-03 06:00Z
            public int ForecastHour { get; set; }      // e.g. 3, 6, 9, ...
            public string FileName { get; set; }       // gfs.t06z.pgrb2.0p25.f003
            public string RemoteUrl { get; set; }      // full HTTP url
            public string LocalPath { get; set; }      // full local path
        }

        public class GfsFileLocator
        {
            public string BaseUrl { get; set; } =
                "https://nomads.ncep.noaa.gov/pub/data/nccf/com/gfs/prod";

            public string LocalRoot { get; }

            public string ResolutionTag { get; set; } = "0p25";

            public int MaxForecastHour { get; set; } = 240;

            public GfsFileLocator(string localRoot)
            {
                if (string.IsNullOrWhiteSpace(localRoot))
                    throw new ArgumentNullException(nameof(localRoot));

                LocalRoot = localRoot;
            }

            /// <summary>
            /// پیدا کردن بهترین run و ساختن URL و LocalPath برای یک زمان پرواز
            /// </summary>
            public GfsRunInfo GetFileForFlight(DateTime flightTimeUtc)
            {
                var basicRun = GetBestRun(flightTimeUtc);

                string datePart = basicRun.RunTimeUtc.ToString("yyyyMMdd");
                string hourPart = basicRun.RunTimeUtc.ToString("HH");
                string forecastPart = basicRun.ForecastHour.ToString("000");

                string fileName = $"gfs.t{hourPart}z.pgrb2.{ResolutionTag}.f{forecastPart}";

                string remoteUrl =
                    $"{BaseUrl.TrimEnd('/')}/gfs.{datePart}/{hourPart}/atmos/{fileName}";

                string dayFolder = Path.Combine(LocalRoot, datePart, hourPart);
                string localPath = Path.Combine(dayFolder, fileName);

                return new GfsRunInfo
                {
                    RunTimeUtc = basicRun.RunTimeUtc,
                    ForecastHour = basicRun.ForecastHour,
                    FileName = fileName,
                    RemoteUrl = remoteUrl,
                    LocalPath = localPath
                };
            }

            /// <summary>
            /// فقط انتخاب run و forecastHour (بدون URL)
            /// </summary>
            private GfsRunInfo GetBestRun(DateTime flightTimeUtc, int stepHours = 3)
            {
                if (flightTimeUtc.Kind != DateTimeKind.Utc)
                    flightTimeUtc = DateTime.SpecifyKind(flightTimeUtc, DateTimeKind.Utc);

                int[] runHours = { 0, 6, 12, 18 };

                var date = flightTimeUtc.Date;
                DateTime? bestRunTime = null;

                foreach (var rh in runHours.OrderByDescending(h => h))
                {
                    var candidate = new DateTime(date.Year, date.Month, date.Day, rh, 0, 0, DateTimeKind.Utc);
                    if (candidate <= flightTimeUtc)
                    {
                        bestRunTime = candidate;
                        break;
                    }
                }

                if (!bestRunTime.HasValue)
                {
                    var prevDate = date.AddDays(-1);
                    bestRunTime = new DateTime(prevDate.Year, prevDate.Month, prevDate.Day, 18, 0, 0, DateTimeKind.Utc);
                }

                var diffHours = (flightTimeUtc - bestRunTime.Value).TotalHours;

                int forecastHour = (int)(Math.Round(diffHours / stepHours) * stepHours);
                if (forecastHour < 0) forecastHour = 0;
                if (forecastHour > MaxForecastHour) forecastHour = MaxForecastHour;

                return new GfsRunInfo
                {
                    RunTimeUtc = bestRunTime.Value,
                    ForecastHour = forecastHour
                };
            }
        }



    //public class WindTempMapService
    //{
    //    private readonly GfsFileLocator _locator;
    //    private readonly WindTempMapRenderer _renderer;
    //    private readonly string _outputRoot;

    //    public WindTempMapService(string gfsRoot, string outputRoot)
    //    {
    //        if (string.IsNullOrWhiteSpace(gfsRoot))
    //            throw new ArgumentNullException(nameof(gfsRoot));
    //        if (string.IsNullOrWhiteSpace(outputRoot))
    //            throw new ArgumentNullException(nameof(outputRoot));

    //        _locator = new GfsFileLocator(gfsRoot);
    //        _renderer = new WindTempMapRenderer();
    //        _outputRoot = outputRoot;

    //        Directory.CreateDirectory(_outputRoot);
    //    }

    //    /// <summary>
    //    /// تولید نقشه Wind/Temp برای هر Flight Level دلخواه روی Route.
    //    /// flightLevels می‌تونه هر تعداد FL دلخواه داشته باشه (مثلا 3، 5، 10 تا ...).
    //    /// </summary>
    //    public async Task GenerateRouteMapsAsync(
    //        FlightPlan fp,
    //        int[] flightLevels,
    //        CancellationToken ct)
    //    {
    //        if (fp == null) throw new ArgumentNullException(nameof(fp));
    //        if (fp.Route == null || fp.Route.Count == 0)
    //            throw new ArgumentException("Flight plan route is empty", nameof(fp));
    //        if (flightLevels == null || flightLevels.Length == 0)
    //            throw new ArgumentException("flightLevels is empty", nameof(flightLevels));

    //        // پاک‌سازی تکراری‌ها و مرتب‌سازی صرفاً برای نظم
    //        var uniqueLevels = flightLevels.Distinct().OrderBy(fl => fl).ToArray();

    //        // 1) پیدا کردن فایل GFS مربوط به زمان پرواز + دانلود
    //        var gfsInfo = _locator.GetFileForFlight(fp.DepartureTimeUtc);

    //        string gribPath = await HttpDownloadHelper.DownloadFileAsync(
    //            gfsInfo.RemoteUrl,
    //            gfsInfo.LocalPath,
    //            ct);

    //        // 2) محدوده جغرافیایی اطراف Route
    //        var bounds = RouteBounds.GetBoundingBox(fp.Route, marginDeg: 2.0);

    //        // 3) یک بار GribFile رو باز می‌کنیم، برای همه FLها از همین استفاده می‌کنیم
    //        using (var file = new GribFile(gribPath))
    //        {
    //            foreach (int fl in uniqueLevels)
    //            {
    //                int levelHpa = FlightLevelConverter.FlightLevelToHpa(fl);

    //                Console.WriteLine($"[Wx] Building fields for FL{fl} (~{levelHpa} hPa) ...");

    //                var windField = GfsFieldFactory.CreateWindField(file, levelHpa);
    //                var tempField = GfsFieldFactory.CreateTemperatureField(file, levelHpa);

    //                using (Bitmap bmp = _renderer.Render(
    //                    windField,
    //                    tempField,
    //                    bounds.minLat, bounds.maxLat,
    //                    bounds.minLon, bounds.maxLon,
    //                    width: 1200,
    //                    height: 800,
    //                    gridStepDeg: 2.0))
    //                {
    //                    string fileName = $"{fp.FlightId}_FL{fl}.png";
    //                    string outPath = Path.Combine(_outputRoot, fileName);

    //                    bmp.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
    //                    Console.WriteLine("[Wx] Saved map: " + outPath);
    //                }
    //            }
    //        }
    //    }
    //}






}
