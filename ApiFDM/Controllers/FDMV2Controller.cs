using System;
using System.Collections.Generic;
using System.Linq;
using ApiFDM.Objects;
using ApiFDM.Models;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data;
using System.Data.Entity;

using System.Data.SqlClient;
using System.Web.Http.Results;
using System.Security.Claims;
using System.Data.Entity.SqlServer;
using Remotion.Collections;
using WebGrease;
using UglyToad.PdfPig.Graphics.Operations.SpecialGraphicsState;
using static ApiFDM.Controllers.FDMV2Controller.FleetAvgMetrics;
using System.Data.SqlTypes;

namespace ApiFDM.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FDMV2Controller : ApiController
    {
        // DTOهای خروجی (بیرون از متد تعریف شوند)
        public sealed class TrendPoint
        {
            public DateTime Date { get; set; }
            public int Count { get; set; }
            public int FlightCount { get; set; }
            public int HighCount { get; set; }
            public int MediumCount { get; set; }
            public int LowCount { get; set; }
            public int TotalScore { get; set; }
            public double EventRatePer100 { get; set; }
            public double Ewma { get; set; }
            public double CusumPos { get; set; }
            public double CusumNeg { get; set; }
            public bool Alarm { get; set; }
        }
        public sealed class ParetoItem
        {
            public string EventTitle { get; set; } = "";
            public int Count { get; set; }
            public double Percent { get; set; }       // سهم هر آیتم از کل (%)
            public double Cumulative { get; set; }    // درصد تجمعی (%)
        }
        public sealed class ParetoResponse
        {
            public object Config { get; set; }
            public IReadOnlyList<ParetoItem> Items { get; set; }
            public int Total { get; set; }
        }
        public class FleetAvgMetrics
        {
            public double avg_high_count_per100 { get; set; }
            public double avg_medium_count_per100 { get; set; }
            public double avg_low_count_per100 { get; set; }
            public double avg_total_count_per100 { get; set; }

            public double avg_high_score_per_flight { get; set; }
            public double avg_medium_score_per_flight { get; set; }
            public double avg_low_score_per_flight { get; set; }
            public double avg_total_score_per_flight { get; set; }
            public class FDMProcessedDto
            {
                public string register { get; set; }
                public int register_id { get; set; }
                public int flight_id { get; set; }
                public string flight_number { get; set; }

                public DateTime std { get; set; }
                public string state_name { get; set; }
                public string type { get; set; }
                public string severity { get; set; }
                public string ac_type { get; set; }
                public string ac_type2 { get; set; }
                public int ac_type_id { get; set; }
                public string arr_iata { get; set; }
                public string dep_iata { get; set; }
                public int? cpt1_id { get; set; }
                public string cpt1_first_name { get; set; }
                public string cpt1_last_name { get; set; }
                public string cp1_name { get; set; }
                public int? cpt2_id { get; set; }
                public string cp2_name { get; set; }
                public int? ip1_id { get; set; }
                public string ip1_name { get; set; }
                public int? ip2_id { get; set; }
                public string ip2_name { get; set; }
                public string phase { get; set; }
                public string value { get; set; }
                public string event_name { get; set; }
                public string route { get; set; }

                // Nullable - فقط برای حالت‌هایی که crew داده شده
                public int? crew_id { get; set; }
                public string position { get; set; }
            }


        };


        //-----------------EWMA----------------------------------------
        [Route("api/fdm/V2/EWMAAllEvents/{dt1}/{dt2}")]
        public async Task<DataResponse> get_EWMAAllevents(DateTime dt1, DateTime dt2)
        {
            // پارامترها
            double alpha = 0.30; // 0..1
            double kFactor = 0.50; // بر حسب σ
            double hFactor = 10.00; // بر حسب σ

            //FDMEntities context = new FDMEntities();
            try
            {
                using (var context = new FDMEntities())
                {
                    var dt2Exclusive = dt2.Date.AddDays(1);
                    // تجمیع روزانه در SQL
                    var daily = await (
                            from y in context.FlightInformations
                            where y.STD >= dt1 && y.STD < dt2Exclusive
                               &&  (y.FlightStatusID == 3 || y.FlightStatusID == 15 || y.FlightStatusID == 7 || y.FlightStatusID == 17)

                            join x in context.view_fdm_processed
                                on y.ID equals x.flight_id into j
                            from x in j.DefaultIfEmpty()
                        group new { x, y } by DbFunctions.TruncateTime(y.STD) into grp
                        let flights = grp.Select(g => g.y.ID).Distinct().Count()
                        let events = grp.Sum(g => g.x.fdm_id != null ? 1 : 0)

                        orderby grp.Key
                        select new
                        {
                            std_date = grp.Key, // Nullable<DateTime>
                                                //flight_count = grp.Count(),
                                                //flight_count = grp.Max(g => (int?)g.flight_count) ?? 1, // Max طبق خواسته شما
                                                //count = grp.Sum(g => g.fdm_id !=null ? 1 : 0),
                            flight_count = flights,
                            count = events,

                            high_count = grp.Sum(g => g.x != null && g.x.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.x!=null && g.x.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.x!=null && g.x.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g =>
                                g.x!=null && g.x.severity == "High" ? 4 :
                                g.x != null && g.x.severity == "Medium" ? 2 :
                                g.x != null && g.x.severity == "Low" ? 1 : 0
                            ),
                            event_rate_per_100 = flights == 0 ? 0.0 : (events * 100.0) / (double)flights
                        }
                    ).ToListAsync();
                    // اگر خالی بود
                    if (daily.Count == 0)
                    {
                        return new DataResponse
                        {
                            Data = new
                            {
                                config = new { alpha, kFactor, hFactor, dt1, dt2 },
                                items = Array.Empty<TrendPoint>(),
                                stats = new { meanRate = 0.0, stdRate = 0.0 }
                            },
                            IsSuccess = true
                        };
                    }
                    // مرتب‌سازی و تبدیل به ساختار میانی
                    var rows = daily
                        .Where(r => r.std_date.HasValue)
                        .OrderBy(r => r.std_date.Value)
                        .Select(r => new
                        {
                            date = r.std_date.Value,
                            r.count,
                            r.flight_count,
                            r.high_count,
                            r.medium_count,
                            r.low_count,
                            r.total_score,
                            r.event_rate_per_100
                        })
                        .ToList();

                    // سری نرخ‌ها
                    var rates = rows.Select(r => r.event_rate_per_100).ToArray();

                    // EWMA
                    var ewma = new double[rates.Length];
                    for (int i = 0; i < rates.Length; i++)
                        ewma[i] = i == 0 ? rates[i] : (alpha * rates[i] + (1 - alpha) * ewma[i - 1]);


                    // CUSUM
                    double mean = rates.Average();
                    double std = Math.Sqrt(rates.Select(r => Math.Pow(r - mean, 2)).DefaultIfEmpty(0).Average());
                    if (std == 0) std = 1; // جلوگیری از صفر

                    double k = kFactor * std;
                    double h = hFactor * std;

                    var cpos = new double[rates.Length];
                    var cneg = new double[rates.Length];
                    var alarm = new bool[rates.Length];

                    cpos[0] = 0.0;
                    cneg[0] = 0.0;
                    alarm[0] = false;

                    for (int i = 1; i < rates.Length; i++)
                    {
                        cpos[i] = Math.Max(0, cpos[i - 1] + (rates[i] - mean - k));
                        cneg[i] = Math.Min(0, cneg[i - 1] + (rates[i] - mean + k));
                        //alarm[i] = cpos[i] > h;// || Math.Abs(cneg[i]) > h;
                    }
                    // تشخیص "روند افزایشی" با پنجره‌ی قبلی (مثلاً window=3)
                    int window = 3;
                    //var alarmIncrease = new bool[rates.Length];
                    for (int i = 0; i < rates.Length; i++)
                    {
                        // میانگین EWMA در پنجرهٔ قبلی (تا i-1)
                        double prevAvgEwma = double.NaN;
                        if (i > 0)
                        {
                            int start = Math.Max(0, i - window);
                            int len = i - start;
                            if (len > 0)
                            {
                                double s = 0;
                                for (int j = start; j < i; j++) s += ewma[j];
                                prevAvgEwma = s / len;
                            }
                        }

                        bool isRising = false;
                        if (!double.IsNaN(prevAvgEwma))
                            isRising = ewma[i] > prevAvgEwma;
                        else if (i > 0)
                            isRising = ewma[i] > ewma[i - 1];

                        // آلارم افزایشی فقط وقتی true که هم CUSUM+ از h عبور کرده و هم EWMA در حال صعود باشد
                        //alarmIncrease[i] =
                        alarm[i]=(cpos[i] > h) && isRising;
                    }


                    // مونتاژ خروجی
                    var items = new List<TrendPoint>(rows.Count);
                    for (int i = 0; i < rows.Count; i++)
                    {
                        var r = rows[i];
                        items.Add(new TrendPoint
                        {
                            Date = r.date,
                            Count = r.count,
                            FlightCount = r.flight_count,
                            HighCount = r.high_count,
                            MediumCount = r.medium_count,
                            LowCount = r.low_count,
                            TotalScore = r.total_score,
                            EventRatePer100 = r.event_rate_per_100,
                            Ewma = ewma[i],
                            //CusumPos = cpos[i],
                            //CusumNeg = cneg[i],
                            Alarm = alarm[i]
                        });
                    }
                    return new DataResponse
                    {
                        Data = new
                        {
                            items
                        },
                        IsSuccess = true
                    };
                   
                }
            }
            catch (Exception ex)
            {
                return new DataResponse { Data = new { error = ex.Message }, IsSuccess = false };
            }
        }

        [Route("api/fdm/V2/EWMAEvent/{dt1}/{dt2}/{event_name}")]
        public async Task<DataResponse> get_EWMAEvent(DateTime dt1, DateTime dt2, string event_name)
        {
            // پارامترها
            double alpha = 0.30; // 0..1
            double kFactor = 0.50; // بر حسب σ
            double hFactor = 5.00; // بر حسب σ

            //FDMEntities context = new FDMEntities();
            try
            {
                using (var context = new FDMEntities())
                {
                    var dt2Exclusive = dt2.Date.AddDays(1);
                    // تجمیع روزانه در SQL
                    var daily = await (
                        from x in context.view_fdm_processed.AsNoTracking()
                        where x.std >= dt1 && x.std < dt2Exclusive && x.event_name == event_name
                        group x by DbFunctions.TruncateTime(x.std) into grp
                        orderby grp.Key
                        select new
                        {
                            std_date = grp.Key, // Nullable<DateTime>
                            count = grp.Count(),
                            flight_count = grp.Max(g => (int?)g.flight_count) ?? 1, // Max طبق خواسته شما
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g =>
                                g.severity == "High" ? 4 :
                                g.severity == "Medium" ? 2 :
                                g.severity == "Low" ? 1 : 0
                            ),
                            event_rate_per_100 =
                                (grp.Count() * 100.0) / (double)(grp.Max(g => (int?)g.flight_count) ?? 1)
                        }
                    ).ToListAsync();
                    var query_flights = await (from x in context.FlightInformations
                                               join y in context.ViewMSNs on x.RegisterID equals y.ID
                                               where x.STD >= dt1 && x.STD <= dt2Exclusive && (x.FlightStatusID == 3 || x.FlightStatusID == 15 || x.FlightStatusID == 7 || x.FlightStatusID == 17)
                                               group x by DbFunctions.TruncateTime(x.STD) into grp
                                               select new
                                               {
                                                   std_date = grp.Key,
                                                   count = grp.Count()
                                               }
                                        ).ToListAsync();
                    var result = (from q in query_flights
                                  let matchedResult = daily.FirstOrDefault(w => w.std_date == q.std_date)
                                  select new
                                  {
                                      q.std_date,
                                      flight_count = q.count,
                                      count = matchedResult?.count ?? 0,
                                      high_count = matchedResult?.high_count ?? 0,
                                      medium_count = matchedResult?.medium_count ?? 0,
                                      low_count = matchedResult?.low_count ?? 0,
                                      total_score = matchedResult?.total_score ?? 0,
                                      event_rate_per_100 = matchedResult?.event_rate_per_100 ?? 0
                                  }).ToList();

                    // اگر خالی بود
                    if (result.Count == 0)
                    {
                        return new DataResponse
                        {
                            Data = new
                            {
                                config = new { alpha, kFactor, hFactor, dt1, dt2 },
                                items = Array.Empty<TrendPoint>(),
                                stats = new { meanRate = 0.0, stdRate = 0.0 }
                            },
                            IsSuccess = true
                        };
                    }
                    // مرتب‌سازی و تبدیل به ساختار میانی
                    var rows = result
                        .Where(r => r.std_date.HasValue)
                        .OrderBy(r => r.std_date.Value)
                        .Select(r => new
                        {
                            date = r.std_date.Value,
                            r.count,
                            r.flight_count,
                            r.high_count,
                            r.medium_count,
                            r.low_count,
                            r.total_score,
                            r.event_rate_per_100
                        })
                        .ToList();

                    // سری نرخ‌ها
                    var rates = rows.Select(r => r.event_rate_per_100).ToArray();

                    // EWMA
                    var ewma = new double[rates.Length];
                    for (int i = 0; i < rates.Length; i++)
                        ewma[i] = i == 0 ? rates[i] : (alpha * rates[i] + (1 - alpha) * ewma[i - 1]);

                    // CUSUM
                    double mean = rates.Average();
                    double std = Math.Sqrt(rates.Select(r => Math.Pow(r - mean, 2)).DefaultIfEmpty(0).Average());
                    if (std == 0) std = 1; // جلوگیری از صفر

                    double k = kFactor * std;
                    double h = hFactor * std;

                    var cpos = new double[rates.Length];
                    var cneg = new double[rates.Length];
                    var alarm = new bool[rates.Length];

                    cpos[0] = 0.0;
                    cneg[0] = 0.0;
                    alarm[0] = false;

                    for (int i = 1; i < rates.Length; i++)
                    {
                        cpos[i] = Math.Max(0, cpos[i - 1] + (rates[i] - mean - k));
                        cneg[i] = Math.Min(0, cneg[i - 1] + (rates[i] - mean + k));
                        //alarm[i] = cpos[i] > h || Math.Abs(cneg[i]) > h;
                    }
                    int window = 3;
                    //var alarmIncrease = new bool[rates.Length];
                    for (int i = 0; i < rates.Length; i++)
                    {
                        // میانگین EWMA در پنجرهٔ قبلی (تا i-1)
                        double prevAvgEwma = double.NaN;
                        if (i > 0)
                        {
                            int start = Math.Max(0, i - window);
                            int len = i - start;
                            if (len > 0)
                            {
                                double s = 0;
                                for (int j = start; j < i; j++) s += ewma[j];
                                prevAvgEwma = s / len;
                            }
                        }

                        bool isRising = false;
                        if (!double.IsNaN(prevAvgEwma))
                            isRising = ewma[i] > prevAvgEwma;
                        else if (i > 0)
                            isRising = ewma[i] > ewma[i - 1];

                        // آلارم افزایشی فقط وقتی true که هم CUSUM+ از h عبور کرده و هم EWMA در حال صعود باشد
                        //alarmIncrease[i] =
                        alarm[i] = (cpos[i] > h) && isRising;
                    }


                    // مونتاژ خروجی
                    var items = new List<TrendPoint>(rows.Count);
                    for (int i = 0; i < rows.Count; i++)
                    {
                        var r = rows[i];
                        items.Add(new TrendPoint
                        {
                            Date = r.date,
                            Count = r.count,
                            FlightCount = r.flight_count,
                            HighCount = r.high_count,
                            MediumCount = r.medium_count,
                            LowCount = r.low_count,
                            TotalScore = r.total_score,
                            EventRatePer100 = r.event_rate_per_100,
                            Ewma = ewma[i],
                           // CusumPos = cpos[i],
                            //CusumNeg = cneg[i],
                            Alarm = alarm[i]
                        });
                    }
                    return new DataResponse
                    {
                        Data = new
                        {
                            config = new { alpha, kFactor, hFactor, dt1, dt2 },
                            items,
                            stats = new { meanRate = mean, stdRate = std }
                        },
                        IsSuccess = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse
                {
                    Data = new { error = ex.Message },
                    IsSuccess = false
                };
            }
        }

        [Route("api/fdm/V2/EWMACptFo/{dt1}/{dt2}/{cpt_id}/{position}")]
        public async Task<DataResponse> get_EWMACpt(DateTime dt1, DateTime dt2, int cpt_id,string position)
        {
            // پارامترها
            double alpha = 0.30; // 0..1
            double kFactor = 0.50; // بر حسب σ
            double hFactor = 5.00; // بر حسب σ

            //FDMEntities context = new FDMEntities();
            try
            {
                using (var context = new FDMEntities())
                {
                    var dt2Exclusive = dt2.Date.AddDays(1);
                    // تجمیع روزانه در SQL
                    var daily = await (
                        from x in context.view_fdm_processed.AsNoTracking()
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std < dt2Exclusive && y.crew_id == cpt_id && y.position == position && (position != "FO" || x.pf == "F")

                        group x by DbFunctions.TruncateTime(x.std) into grp
                        orderby grp.Key
                        select new
                        {
                            std_date = grp.Key, // Nullable<DateTime>
                            count = grp.Count(),
                            flight_count = grp.Max(g => (int?)g.flight_count) ?? 1, // Max طبق خواسته شما
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g =>
                                g.severity == "High" ? 4 :
                                g.severity == "Medium" ? 2 :
                                g.severity == "Low" ? 1 : 0
                            ),
                            event_rate_per_100 =
                                (grp.Count() * 100.0) /// (double)(grp.Max(g => (int?)g.flight_count) ?? 1)
                        }
                    ).ToListAsync();

                    var query_flights = await (from x in context.view_fdm_flight_crew
                                               where x.STD >= dt1 && x.STD <= dt2Exclusive && x.CrewId == cpt_id && x.Position == position && (position != "FO" || x.pf == "F")
                                               group x by DbFunctions.TruncateTime(x.STD) into grp
                                               select new
                                               {
                                                   std_date = grp.Key,
                                                   count = grp.Count()
                                               }
                                        ).ToListAsync();

                    var result = (from q in query_flights
                                  let matchedResult = daily.FirstOrDefault(w => w.std_date == q.std_date)
                                  select new
                                  {
                                      q.std_date,
                                      flight_count = q.count,
                                      count = matchedResult?.count ?? 0,
                                      high_count = matchedResult?.high_count ?? 0,
                                      medium_count = matchedResult?.medium_count ?? 0,
                                      low_count = matchedResult?.low_count ?? 0,
                                      total_score = matchedResult?.total_score ?? 0,
                                      event_rate_per_100 = matchedResult?.event_rate_per_100 / (double)((int?)q.count ?? 1) ?? 0
                                  }).ToList();

                    // اگر خالی بود
                    if (result.Count == 0)
                    {
                        return new DataResponse
                        {
                            Data = new
                            {
                                config = new { alpha, kFactor, hFactor, dt1, dt2 },
                                items = Array.Empty<TrendPoint>(),
                                stats = new { meanRate = 0.0, stdRate = 0.0 }
                            },
                            IsSuccess = true
                        };
                    }
                    // مرتب‌سازی و تبدیل به ساختار میانی
                    var rows = result
                        .Where(r => r.std_date.HasValue)
                        .OrderBy(r => r.std_date.Value)
                        .Select(r => new
                        {
                            date = r.std_date.Value,
                            r.count,
                            r.flight_count,
                            r.high_count,
                            r.medium_count,
                            r.low_count,
                            r.total_score,
                            r.event_rate_per_100
                        })
                        .ToList();

                    // سری نرخ‌ها
                    var rates = rows.Select(r => r.event_rate_per_100).ToArray();

                    // EWMA
                    var ewma = new double[rates.Length];
                    for (int i = 0; i < rates.Length; i++)
                        ewma[i] = i == 0 ? rates[i] : (alpha * rates[i] + (1 - alpha) * ewma[i - 1]);

                    // CUSUM
                    double mean = rates.Average();
                    double std = Math.Sqrt(rates.Select(r => Math.Pow(r - mean, 2)).DefaultIfEmpty(0).Average());
                    if (std == 0) std = 1; // جلوگیری از صفر

                    double k = kFactor * std;
                    double h = hFactor * std;

                    var cpos = new double[rates.Length];
                    var cneg = new double[rates.Length];
                    var alarm = new bool[rates.Length];
                    cpos[0] = 0.0;
                    cneg[0] = 0.0;
                    alarm[0] = false;


                    for (int i = 1; i < rates.Length; i++)
                    {
                        cpos[i] = Math.Max(0, cpos[i - 1] + (rates[i] - mean - k));
                        cneg[i] = Math.Min(0, cneg[i - 1] + (rates[i] - mean + k));
                        //alarm[i] = cpos[i] > h;//|| Math.Abs(cneg[i]) > h;
                    }
                    int window = 3;
                    //var alarmIncrease = new bool[rates.Length];
                    for (int i = 0; i < rates.Length; i++)
                    {
                        // میانگین EWMA در پنجرهٔ قبلی (تا i-1)
                        double prevAvgEwma = double.NaN;
                        if (i > 0)
                        {
                            int start = Math.Max(0, i - window);
                            int len = i - start;
                            if (len > 0)
                            {
                                double s = 0;
                                for (int j = start; j < i; j++) s += ewma[j];
                                prevAvgEwma = s / len;
                            }
                        }

                        bool isRising = false;
                        if (!double.IsNaN(prevAvgEwma))
                            isRising = ewma[i] > prevAvgEwma;
                        else if (i > 0)
                            isRising = ewma[i] > ewma[i - 1];

                        // آلارم افزایشی فقط وقتی true که هم CUSUM+ از h عبور کرده و هم EWMA در حال صعود باشد
                        //alarmIncrease[i] =
                        alarm[i] = (cpos[i] > h) && isRising;
                    }


                    // مونتاژ خروجی
                    var items = new List<TrendPoint>(rows.Count);
                    for (int i = 0; i < rows.Count; i++)
                    {
                        var r = rows[i];
                        items.Add(new TrendPoint
                        {
                            Date = r.date,
                            Count = r.count,
                            FlightCount = r.flight_count,
                            HighCount = r.high_count,
                            MediumCount = r.medium_count,
                            LowCount = r.low_count,
                            TotalScore = r.total_score,
                            EventRatePer100 = r.event_rate_per_100,
                            Ewma = ewma[i],
                           // CusumPos = cpos[i],
                           // CusumNeg = cneg[i],
                            Alarm = alarm[i]
                        });
                    }
                    return new DataResponse
                    {
                        Data = new
                        {
                            //config = new { alpha, kFactor, hFactor, dt1, dt2 },
                            items,
                            //stats = new { meanRate = mean, stdRate = std }
                        },
                        IsSuccess = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse
                {
                    Data = new { error = ex.Message },
                    IsSuccess = false
                };
            }
        }

        //-------------Pareto----------------------------------------
        [Route("api/fdm/V2/ParetoEvent/{dt1}/{dt2}/{top}")]
        public async Task<DataResponse> Get_ParetoEvent(DateTime dt1, DateTime dt2, int top)
        {
            try
            {
                using (var context = new FDMEntities())
                {
                    var dt2Exclusive = dt2.Date.AddDays(1);

                    // 1) گروه‌بندی بر اساس عنوان رخداد
                    var raw = await (
                        from x in context.view_fdm_processed.AsNoTracking()
                        where x.std >= dt1 && x.std < dt2Exclusive
                        // اگر ستون شما اسم دیگری دارد، این خط را به نام صحیح (مثلاً x.event_name) تغییر دهید
                        group x by (x.event_name ?? "Unknown") into grp
                        select new
                        {
                            EventTitle = grp.Key,
                            Count = grp.Count()
                        }
                    )
                    .OrderByDescending(r => r.Count)
                    .ToListAsync();

                    // 2) اگر داده خالی بود
                    if (raw.Count == 0)
                    {
                        return new DataResponse
                        {
                            Data = new ParetoResponse
                            {
                                //Config = new { dt1, dt2, top },
                                Items = Array.Empty<ParetoItem>(),
                                //Total = 0
                            },
                            IsSuccess = true
                        };
                    }

                    // 3) برش Top-N و ادغام بقیه در «Other» (اختیاری ولی مفید برای نمودار)
                    var topItems = raw.Take(top).ToList();
                    var othersCount = raw.Skip(top).Sum(x => x.Count);
                    if (othersCount > 0)
                        topItems.Add(new { EventTitle = "Other", Count = othersCount });

                    // 4) محاسبه درصد و تجمعی
                    var total = topItems.Sum(x => x.Count);
                    double running = 0;
                    var items = topItems.Select(x =>
                    {
                        var pct = total == 0 ? 0 : (x.Count * 100.0 / total);
                        running += pct;
                        return new ParetoItem
                        {
                            EventTitle = x.EventTitle,
                            Count = x.Count,
                            Percent = Math.Round(pct, 2),
                            Cumulative = Math.Round(running, 2)
                        };
                    }).ToList();

                    // 5) بازگشت خروجی استاندارد داشبورد
                    return new DataResponse
                    {
                        Data = new ParetoResponse
                        {
                           // Config = new { dt1, dt2, top },
                            Items =items,
                           // Total = total
                        },
                        IsSuccess = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse { Data = new { error = ex.Message }, IsSuccess = false };
            }
        }

        [Route("api/fdm/V2/ParetoEventCptFo/{dt1}/{dt2}/{top}/{cpt_id}/{position}")]
        public async Task<DataResponse> Get_ParetoEventCpt(DateTime dt1, DateTime dt2, int top,int cpt_id,string position)
        {
            try
            {
                using (var context = new FDMEntities())
                {
                    var dt2Exclusive = dt2.Date.AddDays(1);

                    // 1) گروه‌بندی بر اساس عنوان رخداد
                    var raw = await (
                        from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std < dt2Exclusive && y.crew_id == cpt_id && y.position == position && (position != "FO" || x.pf == "F")
                        group x by (x.event_name ?? "Unknown") into grp
                        select new
                        {
                            EventTitle = grp.Key,
                            Count = grp.Count()
                        }
                    )
                    .OrderByDescending(r => r.Count)
                    .ToListAsync();

                    // 2) اگر داده خالی بود
                    if (raw.Count == 0)
                    {
                        return new DataResponse
                        {
                            Data = new ParetoResponse
                            {
                                //Config = new { dt1, dt2, top },
                                Items = Array.Empty<ParetoItem>(),
                                //Total = 0
                            },
                            IsSuccess = true
                        };
                    }

                    // 3) برش Top-N و ادغام بقیه در «Other» (اختیاری ولی مفید برای نمودار)
                    var topItems = raw.Take(top).ToList();
                    var othersCount = raw.Skip(top).Sum(x => x.Count);
                    if (othersCount > 0)
                        topItems.Add(new { EventTitle = "Other", Count = othersCount });

                    // 4) محاسبه درصد و تجمعی
                    var total = topItems.Sum(x => x.Count);
                    double running = 0;
                    var items = topItems.Select(x =>
                    {
                        var pct = total == 0 ? 0 : (x.Count * 100.0 / total);
                        running += pct;
                        return new ParetoItem
                        {
                            EventTitle = x.EventTitle,
                            Count = x.Count,
                            Percent = Math.Round(pct, 2),
                            Cumulative = Math.Round(running, 2)
                        };
                    }).ToList();

                    // 5) بازگشت خروجی استاندارد داشبورد
                    return new DataResponse
                    {
                        Data = new ParetoResponse
                        {
                            // Config = new { dt1, dt2, top },
                            Items = items,
                            // Total = total
                        },
                        IsSuccess = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse { Data = new { error = ex.Message }, IsSuccess = false };
            }
        }

        //-------------Comparison----------------------------------------

        [Route("api/fdm/V2/MonthlyCPT/{dt1}/{dt2}/{cid}")]
        public async Task<DataResponse> GetCaptainMonthlyScore(DateTime dt1, DateTime dt2,int cid)
        {

            try
            {
                using (var context = new FDMEntities())
                {
                    var dt2Exclusive = dt2.Date.AddDays(1);
                    // --- پایه: همه‌ی رکوردهای بازه  انتخابی
                    var eventsMonthly = await (
                           from x in context.view_fdm_processed
                           join c in context.fdm_crew on x.id equals c.processed_id
                           where x.std >= dt1 && x.std < dt2Exclusive &&  c.crew_id==cid && (c.position == "CPT" || (c.position == "FO" && x.pf == "F"))
                           let Ye = SqlFunctions.DatePart("year", x.std)
                           let Mo = SqlFunctions.DatePart("month", x.std)
                           group x by new
                           {
                               year = (Ye ?? 0),
                               month = (Mo ?? 0),
                               ac_type = x.ac_type2,
                               crew_id = c.crew_id,
                               position=c.position,
                               crew_name = c.name
                           } into g
                           select new
                           {
                               g.Key.year,
                               g.Key.month,
                               g.Key.ac_type,
                               g.Key.crew_id,
                               g.Key.crew_name,
                               g.Key.position,
                               event_count = g.Count(),
                               high_count = g.Sum(r => r.severity == "High" ? 1 : 0),
                               medium_count = g.Sum(r => r.severity == "Medium" ? 1 : 0),
                               low_count = g.Sum(r => r.severity == "Low" ? 1 : 0),
                               high_score = g.Sum(r => r.severity == "High" ? 4 : 0),
                               medium_score = g.Sum(r => r.severity == "Medium" ? 2 : 0),
                               low_score = g.Sum(r => r.severity == "Low" ? 1 : 0),
                               total_score = g.Sum(r =>
                                                    r.severity == "High" ? 4 :
                                                    r.severity == "Medium" ? 2 :
                                                    r.severity == "Low" ? 1 : 0)
                           }
                          ).ToListAsync();


                    // پروازهای ماهانه هر کاپیتان (فقط Position = "CPT")
                    // شمارش لگ یکتا با استفاده از fi.id
                    var flightsMonthly = await
                    (
                        from x in context.view_fdm_flight_crew
                        where x.STD >= dt1 && x.STD < dt2Exclusive && x.CrewId == cid && (x.Position == "CPT" || (x.Position == "FO" && x.pf == "F"))
                        let Ye = SqlFunctions.DatePart("year", x.STD)
                        let Mo = SqlFunctions.DatePart("month", x.STD)
                        group x by new
                        {
                            year = (Ye ?? 0),
                            month = (Mo ?? 0),
                            ac_type = x.ac_type2,
                            crew_id = (int)x.CrewId,
                            position=x.Position,
                            crew_name = x.Name    // در ویو Name از LastName پر شده
                                                  // position = x.Position,
                        }
                        into grp
                        select new
                        {
                            grp.Key.year,
                            grp.Key.month,
                            grp.Key.ac_type,
                            grp.Key.crew_id,
                            grp.Key.crew_name,
                            grp.Key.position,
                            // لگ‌های یکتا برای این خلبان در این ماه
                            flight_count = grp.Select(z => z.id).Distinct().Count()
                        }
                    ).ToListAsync();

                    var combined = (
                          from f in flightsMonthly
                          join e in eventsMonthly
                          on new { f.year, f.month, f.ac_type, f.crew_id, f.crew_name,f.position }
                          equals new { e.year, e.month, e.ac_type, e.crew_id, e.crew_name,e.position }
                          into ge
                          from e in ge.DefaultIfEmpty() // Left Join
                          select new
                          {
                              f.ac_type,
                              f.crew_id,
                              f.crew_name,
                              // rank = f.crew_job_group,         // اگر Rank جداگانه دارید، همان را بگذارید
                              //rank_code = f.crew_job_group_code,
                              position = f.position,

                              f.year,
                              f.month,
                              year_month = (f.year.ToString("0000") + "-" + f.month.ToString("00")),

                              f.flight_count,

                              // اگر e=null بود → صفر
                              event_count = e?.event_count ?? 0,
                              high_count = e?.high_count ?? 0,
                              medium_count = e?.medium_count ?? 0,
                              low_count = e?.low_count ?? 0,

                              high_score = e?.high_score ?? 0,
                              medium_score = e?.medium_score ?? 0,
                              low_score = e?.low_score ?? 0,
                              total_score = e?.total_score ?? 0,

                              // نرخ رویداد به ازای هر 100 پرواز
                              high_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.high_count ?? 0) * 100.0) / f.flight_count,
                              medium_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.medium_count ?? 0) * 100.0) / f.flight_count,
                              low_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.low_count ?? 0) * 100.0) / f.flight_count,
                              total_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.event_count ?? 0) * 100.0) / f.flight_count,

                              // امتیاز/پرواز (اختیاری)
                              high_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.high_score ?? 0) * 1.0) / f.flight_count,
                              medium_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.medium_score ?? 0) * 1.0) / f.flight_count,
                              low_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.low_score ?? 0) * 1.0) / f.flight_count,
                              score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.total_score ?? 0) * 1.0) / f.flight_count

                          }
                    ).ToList();

                    // 4) میانگین ناوگان (FleetAvg) همان ماه و تایپ
                    // -------------------------------

                    var fleetAvg = combined
                        .GroupBy(x => new { x.ac_type, x.year, x.month ,x.position})
                        .Select(g => new
                        {
                            g.Key.ac_type,
                            g.Key.year,
                            g.Key.month,
                            g.Key.position,
                            Metrics = new FleetAvgMetrics
                            {
                                avg_high_count_per100 = g.Average(z => (double)z.high_count_per_100),  // میانگین کل امتیاز CPTها
                                avg_medium_count_per100 = g.Average(z => (double)z.medium_count_per_100),  // میانگین کل امتیاز CPTها
                                avg_low_count_per100 = g.Average(z => (double)z.low_count_per_100),  // میانگین کل امتیاز CPTها
                                avg_total_count_per100 = g.Average(z => (double)z.total_count_per_100),  // میانگین کل امتیاز CPTها

                                avg_high_score_per_flight = g.Average(z => (double)z.high_score_per_flight),  // میانگین کل امتیاز CPTها
                                avg_medium_score_per_flight = g.Average(z => (double)z.medium_score_per_flight), // میانگین کل امتیاز CPTها
                                avg_low_score_per_flight = g.Average(z => (double)z.low_score_per_flight),  // میانگین کل امتیاز CPTها
                                avg_total_score_per_flight = g.Average(z => (double)z.score_per_flight)  // میانگین کل امتیاز CPTها
                            }
                        }).ToDictionary(k => $"{k.ac_type}|{k.position}|{k.year:0000}-{k.month:00}",
                                          v => v.Metrics
                     );

                    // گروه‌بندی ماهانه برای هر CPT
                    var items = combined.Select(x =>
                    {
                        fleetAvg.TryGetValue($"{x.ac_type}|{x.position}|{x.year:0000}-{x.month:00}", out var avg);
                        return new
                        {
                            ac_type = x.ac_type,
                            crew_id = x.crew_id,
                            crew_name = x.crew_name,
                            position = x.position,
                            year = x.year,
                            month = x.month,
                            year_month = x.year_month,
                            flight_count = x.flight_count,
                            event_count = x.event_count,
                            high_count = x.high_count,
                            medium_count = x.medium_count,
                            low_count = x.low_count,
                            high_score = x.high_score,
                            medium_score = x.medium_score,
                            low_score = x.low_score,
                            total_score = x.total_score,
                            high_count_per_100 = x.high_count_per_100,
                            medium_count_per_100 = x.medium_count_per_100,
                            low_count_per_100 = x.low_count_per_100,
                            total_count_per_100 = x.total_count_per_100,
                            high_score_per_flight = x.high_score_per_flight,
                            medium_score_per_flight = x.medium_score_per_flight,
                            low_score_per_flight = x.low_score_per_flight,
                            score_per_flight = x.score_per_flight,
                            FleetAvg = avg
                        };
                    })
                          .OrderBy(x => x.ac_type)
                          .ThenBy(x => x.crew_id)
                          .ThenBy(x => x.year)
                          .ThenBy(x => x.month)
                          .ToList();

                    return new DataResponse
                    {
                        Data = new
                        {
                            items
                        },
                        IsSuccess = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse { Data = new { error = ex.Message }, IsSuccess = false };
            }
        }
        [Route("api/fdm/V2/MonthlyRegister/{dt1}/{dt2}")]
        public async Task<DataResponse> GetRegisterMonthlyScore(DateTime dt1, DateTime dt2)
        {

            try
            {
                using (var context = new FDMEntities())
                {
                    var dt2Exclusive = dt2.Date.AddDays(1);
                    // --- پایه: همه‌ی رکوردهای بازه  انتخابی
                    var eventsMonthly = await (
                           from x in context.view_fdm_processed
                           where x.std >= dt1 && x.std < dt2Exclusive
                           let Ye = SqlFunctions.DatePart("year", x.std)
                           let Mo = SqlFunctions.DatePart("month", x.std)
                           group x by new
                           {
                               year = (Ye ?? 0),
                               month = (Mo ?? 0),
                               ac_type = x.ac_type2,
                               register_id = x.register_id,
                               register = x.register
                           } into g
                           select new
                           {
                               g.Key.year,
                               g.Key.month,
                               g.Key.ac_type,
                               g.Key.register_id,
                               g.Key.register,
                               event_count = g.Count(),
                               high_count = g.Sum(r => r.severity == "High" ? 1 : 0),
                               medium_count = g.Sum(r => r.severity == "Medium" ? 1 : 0),
                               low_count = g.Sum(r => r.severity == "Low" ? 1 : 0),
                               high_score = g.Sum(r => r.severity == "High" ? 4 : 0),
                               medium_score = g.Sum(r => r.severity == "Medium" ? 2 : 0),
                               low_score = g.Sum(r => r.severity == "Low" ? 1 : 0),
                               total_score = g.Sum(r =>
                                                    r.severity == "High" ? 4 :
                                                    r.severity == "Medium" ? 2 :
                                                    r.severity == "Low" ? 1 : 0)
                           }
                          ).ToListAsync();


                    var flightsMonthly = await
                    (
                        from x in context.FlightInformations
                        join y in context.ViewMSNs on x.RegisterID equals y.ID
                        where x.STD >= dt1 && x.STD <= dt2Exclusive && (x.FlightStatusID == 3 || x.FlightStatusID == 15 || x.FlightStatusID == 7 || x.FlightStatusID == 17)

                        //from x in context.view_fdm_flight_crew
                        // where x.STD >= dt1 && x.STD < dt2Exclusive && x.Position == "CPT"
                        let Ye = SqlFunctions.DatePart("year", x.STD)
                        let Mo = SqlFunctions.DatePart("month", x.STD)
                        group x by new
                        {
                            year = (Ye ?? 0),
                            month = (Mo ?? 0),
                            ac_type = y.AircraftType2,
                            register_id = x.RegisterID,
                            register = y.Register
                        }
                        into grp
                        select new
                        {
                            grp.Key.year,
                            grp.Key.month,
                            grp.Key.ac_type,
                            grp.Key.register_id,
                            grp.Key.register,
                            flight_count = grp.Count()

                        }
                    ).ToListAsync();

                    var combined = (
                          from f in flightsMonthly
                          join e in eventsMonthly
                          on new { f.year, f.month, f.ac_type, f.register_id, f.register }
                          equals new { e.year, e.month, e.ac_type, e.register_id, e.register }
                          into ge
                          from e in ge.DefaultIfEmpty() // Left Join
                          select new
                          {
                              f.ac_type,
                              f.register_id,
                              f.register,
                              f.year,
                              f.month,
                              year_month = (f.year.ToString("0000") + "-" + f.month.ToString("00")),
                              f.flight_count,
                              // اگر e=null بود → صفر
                              event_count = e?.event_count ?? 0,
                              high_count = e?.high_count ?? 0,
                              medium_count = e?.medium_count ?? 0,
                              low_count = e?.low_count ?? 0,

                              high_score = e?.high_score ?? 0,
                              medium_score = e?.medium_score ?? 0,
                              low_score = e?.low_score ?? 0,
                              total_score = e?.total_score ?? 0,

                              // نرخ رویداد به ازای هر 100 پرواز
                              high_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.high_count ?? 0) * 100.0) / f.flight_count,
                              medium_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.medium_count ?? 0) * 100.0) / f.flight_count,
                              low_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.low_count ?? 0) * 100.0) / f.flight_count,
                              total_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.event_count ?? 0) * 100.0) / f.flight_count,

                              // امتیاز/پرواز (اختیاری)
                              high_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.high_score ?? 0) * 1.0) / f.flight_count,
                              medium_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.medium_score ?? 0) * 1.0) / f.flight_count,
                              low_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.low_score ?? 0) * 1.0) / f.flight_count,
                              score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.total_score ?? 0) * 1.0) / f.flight_count

                          }
                    ).ToList();

                    // 4) میانگین ناوگان (FleetAvg) همان ماه و تایپ
                    // -------------------------------

                    var fleetAvg = combined
                        .GroupBy(x => new { x.ac_type, x.year, x.month })
                        .Select(g => new
                        {
                            g.Key.ac_type,
                            g.Key.year,
                            g.Key.month,
                            Metrics = new FleetAvgMetrics
                            {
                                avg_high_count_per100 = g.Average(z => (double)z.high_count_per_100),
                                avg_medium_count_per100 = g.Average(z => (double)z.medium_count_per_100),
                                avg_low_count_per100 = g.Average(z => (double)z.low_count_per_100),
                                avg_total_count_per100 = g.Average(z => (double)z.total_count_per_100),

                                avg_high_score_per_flight = g.Average(z => (double)z.high_score_per_flight),  // میانگین کل امتیاز CPTها
                                avg_medium_score_per_flight = g.Average(z => (double)z.medium_score_per_flight), // میانگین کل امتیاز CPTها
                                avg_low_score_per_flight = g.Average(z => (double)z.low_score_per_flight),  // میانگین کل امتیاز CPTها
                                avg_total_score_per_flight = g.Average(z => (double)z.score_per_flight)  // میانگین کل امتیاز CPTها
                            }
                        }).ToDictionary(k => $"{k.ac_type}|{k.year:0000}-{k.month:00}",
                                          v => v.Metrics
                     );

                    var items = combined.Select(x =>
                    {
                        fleetAvg.TryGetValue($"{x.ac_type}|{x.year:0000}-{x.month:00}", out var avg);
                        return new
                        {
                            ac_type = x.ac_type,
                            register_id = x.register_id,
                            register = x.register,
                            year = x.year,
                            month = x.month,
                            year_month = x.year_month,
                            flight_count = x.flight_count,
                            event_count = x.event_count,
                            high_count = x.high_count,
                            medium_count = x.medium_count,
                            low_count = x.low_count,
                            high_score = x.high_score,
                            medium_score = x.medium_score,
                            low_score = x.low_score,
                            total_score = x.total_score,
                            high_count_per_100 = x.high_count_per_100,
                            medium_count_per_100 = x.medium_count_per_100,
                            low_count_per_100 = x.low_count_per_100,
                            total_count_per_100 = x.total_count_per_100,
                            high_score_per_flight = x.high_score_per_flight,
                            medium_score_per_flight = x.medium_score_per_flight,
                            low_score_per_flight = x.low_score_per_flight,
                            score_per_flight = x.score_per_flight,
                            FleetAvg = avg
                        };
                    })
                          .OrderBy(x => x.ac_type)
                          .ThenBy(x => x.register_id)
                          .ThenBy(x => x.year)
                          .ThenBy(x => x.month)
                          .ToList();

                    return new DataResponse
                    {
                        Data = new
                        {
                            items
                        },
                        IsSuccess = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse { Data = new { error = ex.Message }, IsSuccess = false };
            }
        }
        [Route("api/fdm/V2/MonthlyRoute/{dt1}/{dt2}")]
        public async Task<DataResponse> GetRouteMonthlyScore(DateTime dt1, DateTime dt2)
        {

            try
            {
                using (var context = new FDMEntities())
                {
                    var dt2Exclusive = dt2.Date.AddDays(1);
                    // --- پایه: همه‌ی رکوردهای بازه  انتخابی


                    var eventsMonthly = await (
                           from x in context.view_fdm_processed
                           where x.std >= dt1 && x.std < dt2Exclusive
                           let Ye = SqlFunctions.DatePart("year", x.std)
                           let Mo = SqlFunctions.DatePart("month", x.std)
                           group x by new
                           {
                               year = (Ye ?? 0),
                               month = (Mo ?? 0),
                               dep_id = (int)x.dep_id,
                               //x.dep_iata,
                               //x.dep_icao,
                               arr_id = x.arr_id
                               //x.arr_iata,
                               //x.arr_icao
                           } into g
                           select new
                           {
                               g.Key.year,
                               g.Key.month,
                               dep_id = (int)g.Key.dep_id,
                               //g.Key.dep_iata,
                               //g.Key.dep_icao,
                               arr_id = (int)g.Key.arr_id,
                               //g.Key.arr_iata,
                               //g.Key.arr_icao,
                               event_count = g.Count(),
                               high_count = g.Sum(r => r.severity == "High" ? 1 : 0),
                               medium_count = g.Sum(r => r.severity == "Medium" ? 1 : 0),
                               low_count = g.Sum(r => r.severity == "Low" ? 1 : 0),
                               high_score = g.Sum(r => r.severity == "High" ? 4 : 0),
                               medium_score = g.Sum(r => r.severity == "Medium" ? 2 : 0),
                               low_score = g.Sum(r => r.severity == "Low" ? 1 : 0),
                               total_score = g.Sum(r =>
                                                    r.severity == "High" ? 4 :
                                                    r.severity == "Medium" ? 2 :
                                                    r.severity == "Low" ? 1 : 0)
                           }
                          ).ToListAsync();


                    var flightsMonthly = await
                    (
                        from x in context.AppLegs
                        where x.STD >= dt1 && x.STD <= dt2Exclusive && (x.FlightStatusID == 3 || x.FlightStatusID == 15 || x.FlightStatusID == 7 || x.FlightStatusID == 17)
                        let Ye = SqlFunctions.DatePart("year", x.STD)
                        let Mo = SqlFunctions.DatePart("month", x.STD)
                        group x by new
                        {
                            year = (Ye ?? 0),
                            month = (Mo ?? 0),
                            dep_id = x.FromAirport,
                            dep_iata = x.FromAirportIATA,
                            dep_icao = x.FromAirportIATA2,
                            arr_id = x.ToAirport,
                            arr_iata = x.ToAirportIATA2,
                            arr_icao = x.ToAirportIATA
                        }
                        into grp
                        select new
                        {
                            grp.Key.year,
                            grp.Key.month,
                            dep_id = (int)grp.Key.dep_id,
                            dep_iata = grp.Key.dep_iata,
                            dep_icao = grp.Key.dep_icao,
                            arr_id = (int)grp.Key.arr_id,
                            arr_iata = grp.Key.arr_iata,
                            arr_icao = grp.Key.arr_icao,
                            flight_count = grp.Count()

                        }
                    ).ToListAsync();

                    var combined = (
                          from f in flightsMonthly
                          join e in eventsMonthly
                          on new { f.year, f.month, f.dep_id, f.arr_id }
                          equals new { e.year, e.month, e.dep_id, e.arr_id }
                          into ge
                          from e in ge.DefaultIfEmpty() // Left Join
                          select new
                          {
                              f.year,
                              f.month,
                              year_month = (f.year.ToString("0000") + "-" + f.month.ToString("00")),
                              f.dep_id,
                              f.dep_iata,
                              f.dep_icao,
                              f.arr_id,
                              f.arr_iata,
                              f.arr_icao,
                              f.flight_count,
                              // اگر e=null بود → صفر
                              event_count = e?.event_count ?? 0,
                              high_count = e?.high_count ?? 0,
                              medium_count = e?.medium_count ?? 0,
                              low_count = e?.low_count ?? 0,

                              high_score = e?.high_score ?? 0,
                              medium_score = e?.medium_score ?? 0,
                              low_score = e?.low_score ?? 0,
                              total_score = e?.total_score ?? 0,

                              // نرخ رویداد به ازای هر 100 پرواز
                              high_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.high_count ?? 0) * 100.0) / f.flight_count,
                              medium_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.medium_count ?? 0) * 100.0) / f.flight_count,
                              low_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.low_count ?? 0) * 100.0) / f.flight_count,
                              total_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.event_count ?? 0) * 100.0) / f.flight_count,

                              // امتیاز/پرواز (اختیاری)
                              high_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.high_score ?? 0) * 1.0) / f.flight_count,
                              medium_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.medium_score ?? 0) * 1.0) / f.flight_count,
                              low_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.low_score ?? 0) * 1.0) / f.flight_count,
                              score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.total_score ?? 0) * 1.0) / f.flight_count

                          }
                    ).ToList();

                    // 4) میانگین ناوگان (FleetAvg) همان ماه و تایپ
                    // -------------------------------

                    var fleetAvg = combined
                        .GroupBy(x => new { x.year, x.month })
                        .Select(g => new
                        {

                            g.Key.year,
                            g.Key.month,
                            Metrics = new FleetAvgMetrics
                            {
                                avg_high_count_per100 = g.Average(z => (double)z.high_count_per_100),
                                avg_medium_count_per100 = g.Average(z => (double)z.medium_count_per_100),
                                avg_low_count_per100 = g.Average(z => (double)z.low_count_per_100),
                                avg_total_count_per100 = g.Average(z => (double)z.total_count_per_100),

                                avg_high_score_per_flight = g.Average(z => (double)z.high_score_per_flight),  // میانگین کل امتیاز CPTها
                                avg_medium_score_per_flight = g.Average(z => (double)z.medium_score_per_flight), // میانگین کل امتیاز CPTها
                                avg_low_score_per_flight = g.Average(z => (double)z.low_score_per_flight),  // میانگین کل امتیاز CPTها
                                avg_total_score_per_flight = g.Average(z => (double)z.score_per_flight)  // میانگین کل امتیاز CPTها
                            }
                        }).ToDictionary(k => $"|{k.year:0000}-{k.month:00}",
                                          v => v.Metrics
                     );

                    var items = combined.Select(x =>
                    {
                        fleetAvg.TryGetValue($"|{x.year:0000}-{x.month:00}", out var avg);
                        return new
                        {
                            year = x.year,
                            month = x.month,
                            route = x.dep_iata + "-" + x.arr_iata,
                            x.dep_id,
                            x.dep_iata,
                            x.dep_icao,
                            x.arr_id,
                            x.arr_iata,
                            x.arr_icao,
                            year_month = x.year_month,
                            flight_count = x.flight_count,
                            event_count = x.event_count,
                            high_count = x.high_count,
                            medium_count = x.medium_count,
                            low_count = x.low_count,
                            high_score = x.high_score,
                            medium_score = x.medium_score,
                            low_score = x.low_score,
                            total_score = x.total_score,
                            high_count_per_100 = x.high_count_per_100,
                            medium_count_per_100 = x.medium_count_per_100,
                            low_count_per_100 = x.low_count_per_100,
                            total_count_per_100 = x.total_count_per_100,
                            high_score_per_flight = x.high_score_per_flight,
                            medium_score_per_flight = x.medium_score_per_flight,
                            low_score_per_flight = x.low_score_per_flight,
                            score_per_flight = x.score_per_flight,
                            FleetAvg = avg
                        };
                    })
                          .OrderBy(x => x.dep_id)
                          .ThenBy(x => x.arr_id)
                          .ThenBy(x => x.year)
                          .ThenBy(x => x.month)
                          .ToList();

                    return new DataResponse
                    {
                        Data = new
                        {
                            items
                        },
                        IsSuccess = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse { Data = new { error = ex.Message }, IsSuccess = false };
            }
        }

        //-----------------------------------------------------------------

        [Route("api/fdm/V2/events/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            try
            {
                var xxxx = context.ViewMSNs.ToList();
            }
            catch(Exception ex)
            {
                int yyyyyy = 0;
            }
        
            dt2 = dt2.AddDays(1);

            var query = from x in context.view_fdm_processed
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2 } into grp
                        select new
                        {
                            grp.Key.register_id,

                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.FlightInformations
                                join y in context.ViewMSNs on x.RegisterID equals y.ID
                                where x.STD >= dt1 && x.STD <= dt2 && (x.FlightStatusID == 3 || x.FlightStatusID == 15 || x.FlightStatusID == 7 || x.FlightStatusID == 17)
                                group x by new { x.RegisterID, y.Register, y.AircraftType2 } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.AircraftType2,
                                    count = grp.Count()
                                };
            var result_register1 = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register = (from q in _xx
                                   let matchedResult = result_register1.FirstOrDefault(w => w.register_id == q.register_id)
                                   select new
                                   {
                                       q.register_id,
                                       q.register,
                                       q.ac_type,
                                       flight_count = q.count,
                                       event_count = matchedResult?.count ?? 0,
                                       high_count = matchedResult?.high_count ?? 0,
                                       medium_count = matchedResult?.medium_count ?? 0,
                                       low_count = matchedResult?.low_count ?? 0,
                                       high_score = (matchedResult?.high_count ?? 0) * 4,
                                       medium_score = (matchedResult?.medium_count ?? 0) * 2,
                                       low_score = (matchedResult?.low_count ?? 0) * 1,
                                       total_score = matchedResult?.total_score ?? 0,
                                       score_per_event = matchedResult != null && matchedResult.count > 0
                                                            ? Math.Round((double)matchedResult.total_score / matchedResult.count, 1)
                                                            : 0,
                                       score_per_flight = q.count > 0
                                              ? Math.Round((double)(matchedResult?.total_score ?? 0) / q.count, 1)
                                              : 0
                                   }).ToList();

            var result_type = (from x in result_register
                               group x by new { x.ac_type } into grp
                               select new
                               {
                                   grp.Key.ac_type,
                                   count = grp.Sum(q => q.event_count),
                                   flight_count = grp.Sum(q => q.flight_count),
                                   high_count = grp.Sum(q => q.high_count),
                                   medium_count = grp.Sum(q => q.medium_count),
                                   low_count = grp.Sum(q => q.low_count),
                                   high_score = grp.Sum(q => q.high_score),
                                   medium_score = grp.Sum(q => q.medium_score),
                                   low_score = grp.Sum(q => q.low_score),
                                   total_score = grp.Sum(q => q.total_score),
                                   score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                   score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                               }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register,
                    result_type,

                },
                IsSuccess = true
            };

        }
        [Route("api/fdm/V2/eventsCrew/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_captain(DateTime dt1, DateTime dt2)
        {
            var dt1Date = dt1.Date;
            var dt2Exclusive = dt2.Date.AddDays(1);

            using (var context = new FDMEntities())
            {
                // ===== رویدادها (Processed)؛ گروه‌بندی در سطح Register/Crew/Position =====
                var eventsQuery =
                    from x in context.view_fdm_processed.AsNoTracking()
                    join y in context.fdm_crew.AsNoTracking() on x.id equals y.processed_id
                    where x.std >= dt1Date && x.std < dt2Exclusive && (y.position == "CPT" || (y.position == "FO" && x.pf == "F"))
                    group new { x, y } by new
                    {
                        register_id = (int?)x.register_id,
                        register = x.register,
                        ac_type = x.ac_type2,
                        crew_id = (int?)y.crew_id,
                        first_name = y.first_name,
                        last_name = y.last_name,
                        name = y.name,
                        rank = y.rank,
                        rank_code = y.rank_code,
                        position = y.position,
                    }
                    into grp
                    select new
                    {
                        grp.Key.register_id,
                        grp.Key.register,
                        grp.Key.ac_type,
                        grp.Key.crew_id,
                        grp.Key.first_name,
                        grp.Key.last_name,
                        grp.Key.name,
                        grp.Key.rank,
                        grp.Key.rank_code,
                        grp.Key.position,
                        count = grp.Count(),
                        high_count = grp.Sum(g => g.x.severity == "High" ? 1 : 0),
                        medium_count = grp.Sum(g => g.x.severity == "Medium" ? 1 : 0),
                        low_count = grp.Sum(g => g.x.severity == "Low" ? 1 : 0)
                    };

                // ===== پروازها (فقط CPT)؛ گروه‌بندی در سطح Register/Crew/Position =====
                var flightsQuery =
                    from x in context.view_fdm_flight_crew.AsNoTracking()
                    where x.STD >= dt1Date && x.STD < dt2Exclusive && (x.Position == "CPT" || (x.Position == "FO" && x.pf=="F"))
                    group x by new
                    {
                        register_id = (int?)x.RegisterID,
                        register = x.Register,
                        ac_type = x.ac_type2,
                        crew_id = (int?)x.CrewId,
                        crew_name = x.Name,
                        crew_job_group = x.JobGroup,
                        crew_job_group_code = x.JobGroupCode,
                        crew_position = x.Position,
                    }
                    into grp
                    select new
                    {
                        grp.Key.register_id,
                        grp.Key.register,
                        grp.Key.ac_type,
                        grp.Key.crew_id,
                        grp.Key.crew_name,
                        grp.Key.crew_job_group,
                        grp.Key.crew_job_group_code,
                        grp.Key.crew_position,
                        count = grp.Count()
                    };

                var events = await eventsQuery.ToListAsync().ConfigureAwait(false);
                var flights = await flightsQuery.ToListAsync().ConfigureAwait(false);

                // ===== Left Join در حافظه با GroupJoin + casting صریح روی کلیدها =====
                var result_register_crew =
                    (from q in flights
                     join r in events
                     on new
                     {
                         register_id = (int?)q.register_id,
                         crew_id = (int?)q.crew_id,
                         position = q.crew_position,
                     }
                     equals new
                     {
                         register_id = (int?)r.register_id,
                         crew_id = (int?)r.crew_id,
                         position = r.position,
                     }
                     into gj
                     from r in gj.DefaultIfEmpty()
                     select new
                     {
                         q.register_id,
                         q.register,
                         q.ac_type,
                         q.crew_id,
                         q.crew_name,
                         q.crew_job_group,
                         q.crew_job_group_code,
                         q.crew_position,

                         flight_count = q.count,

                         event_count = r?.count ?? 0,
                         high_count = r?.high_count ?? 0,
                         medium_count = r?.medium_count ?? 0,
                         low_count = r?.low_count ?? 0,

                         high_score = (r?.high_count ?? 0) * 4,
                         medium_score = (r?.medium_count ?? 0) * 2,
                         low_score = (r?.low_count ?? 0) * 1,
                         total_score = ((r?.high_count ?? 0) * 4) + ((r?.medium_count ?? 0) * 2) + ((r?.low_count ?? 0) * 1),

                         // نرخ رویداد به ازای هر 100 پرواز (per 100)
                         high_count_per_100 = q.count == 0 ? 0.0 : ((r?.high_count ?? 0) * 100.0) / q.count,
                         medium_count_per_100 = q.count == 0 ? 0.0 : ((r?.medium_count ?? 0) * 100.0) / q.count,
                         low_count_per_100 = q.count == 0 ? 0.0 : ((r?.low_count ?? 0) * 100.0) / q.count,
                         total_count_per_100 = q.count == 0 ? 0.0 : ((r?.count ?? 0) * 100.0) / q.count,
                     })
                    .ToList();

                // ===== تجمیع در سطح type/crew =====
                var result_type_crew =
                    (from x in result_register_crew // همه CPT هستند
                     group x by new { x.ac_type, x.crew_id, x.crew_name,x.crew_position } into grp
                     let sumFlights = grp.Sum(q => q.flight_count)
                     let sumEvents = grp.Sum(q => q.event_count)
                     let highSum = grp.Sum(q => q.high_count)
                     let medSum = grp.Sum(q => q.medium_count)
                     let lowSum = grp.Sum(q => q.low_count)
                     let totalScore = grp.Sum(q => q.total_score)
                     select new
                     {
                         grp.Key.ac_type,
                         grp.Key.crew_id,
                         grp.Key.crew_name,
                         grp.Key.crew_position,
                         flight_count = sumFlights,
                         count = sumEvents,

                         high_count = highSum,
                         medium_count = medSum,
                         low_count = lowSum,

                         high_score = highSum * 4,
                         medium_score = medSum * 2,
                         low_score = lowSum,
                         total_score = totalScore,

                         // per 100 در سطح تجمیع
                         high_count_per_100 = sumFlights == 0 ? 0.0 : (highSum * 100.0) / sumFlights,
                         medium_count_per_100 = sumFlights == 0 ? 0.0 : (medSum * 100.0) / sumFlights,
                         low_count_per_100 = sumFlights == 0 ? 0.0 : (lowSum * 100.0) / sumFlights,
                         total_count_per_100 = sumFlights == 0 ? 0.0 : (sumEvents * 100.0) / sumFlights,

                         // گارد تقسیم بر صفر
                         score_per_event = sumEvents == 0 ? 0.0 : Math.Round(totalScore * 1.0 / sumEvents, 1),
                         score_per_flight = sumFlights == 0 ? 0.0 : Math.Round(totalScore * 1.0 / sumFlights, 1),
                     })
                    .ToList();

                return new DataResponse()
                {
                    Data = new
                    {
                        result_register_crew,
                        result_type_crew,
                    },
                    IsSuccess = true
                };
            }

        }

        [Route("api/fdm/V2/eventsCptFo/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_captain_Fo(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();

            dt2 = dt2.AddDays(1);
            /* from f in flightsMonthly
             join e in eventsMonthly
             on new { f.year, f.month, f.dep_id, f.arr_id }
             equals new { e.year, e.month, e.dep_id, e.arr_id }
             into ge
             from e in ge.DefaultIfEmpty() // Left Join
             select new*/

            var query_event = await (
                 from e in context.view_fdm_processed
                 join ccpt in context.fdm_crew on e.id equals ccpt.processed_id
                 join cfo in context.fdm_crew on e.id equals cfo.processed_id
                 where e.std >= dt1 && e.std < dt2
                 && ccpt.position == "CPT"
                 && cfo.position == "FO"
                 && ccpt.crew_id != cfo.crew_id
                 group new { e, ccpt, cfo } by new
                 {
                     ac_type = e.ac_type2,
                     cpt_id = ccpt.crew_id,
                     cpt_name = ccpt.name,
                     //cpt_rank = ccpt.rank,
                     //cpt_rank_code = ccpt.rank_code,
                     fo_id = cfo.crew_id,
                     fo_name = cfo.name,
                     //fo_rank = cfo.rank,
                     //fo_rank_code = cfo.rank_code
                 }
                 into grp
                 select new
                 {
                     grp.Key.ac_type,
                     cpt_id = (int)grp.Key.cpt_id,
                     grp.Key.cpt_name,
                     //cpt_rank = grp.Key.cpt_rank,
                     //cpt_rank_code = grp.Key.cpt_rank_code,
                     fo_id = (int)grp.Key.fo_id,
                     grp.Key.fo_name,
                     //fo_rank = grp.Key.fo_rank,
                     //fo_rank_code = grp.Key.fo_rank_code,
                     event_count = grp.Count(),
                     high_count = grp.Sum(g => g.e.severity == "High" ? 1 : 0),
                     medium_count = grp.Sum(g => g.e.severity == "Medium" ? 1 : 0),
                     low_count = grp.Sum(g => g.e.severity == "Low" ? 1 : 0),
                     high_score = grp.Sum(g => g.e.severity == "High" ? 4 : 0),
                     medium_score = grp.Sum(g => g.e.severity == "Medium" ? 2 : 0),
                     low_score = grp.Sum(g => g.e.severity == "Low" ? 1 : 0),
                     total_score = grp.Sum(g =>
                                      g.e.severity == "High" ? 4 :
                                      g.e.severity == "Medium" ? 2 :
                                      g.e.severity == "Low" ? 1 : 0)

                 }).ToListAsync();


            var query_flights = await (from x in context.view_fdm_flight_crew
                                       join y in context.view_fdm_flight_crew on x.id equals y.id
                                       where x.STD >= dt1 && x.STD < dt2 &&
                                       x.Position == "CPT" && y.Position == "FO"
                                       group new { x, y } by new
                                       {
                                           ac_type = x.ac_type2,
                                           cpt_id = x.CrewId,
                                           cpt_name = x.Name,
                                           fo_id = y.CrewId,
                                           fo_name = y.Name
                                       } into grp
                                       select new
                                       {
                                           grp.Key.ac_type,
                                           cpt_id = (int)grp.Key.cpt_id,
                                           grp.Key.cpt_name,
                                           fo_id = (int)grp.Key.fo_id,
                                           grp.Key.fo_name,
                                           flight_count = grp.Count()
                                       }).ToListAsync();

            var combined = (
                          from f in query_flights
                          join e in query_event
                          on new { f.ac_type, f.cpt_id, f.fo_id }
                          equals new { e.ac_type, e.cpt_id, e.fo_id }
                          into ge
                          from e in ge.DefaultIfEmpty() // Left Join
                          select new
                          {
                              f.ac_type,
                              f.cpt_id,
                              f.cpt_name,
                              f.fo_id,
                              f.fo_name,
                              f.flight_count,
                              // اگر e=null بود → صفر
                              event_count = e?.event_count ?? 0,
                              high_count = e?.high_count ?? 0,
                              medium_count = e?.medium_count ?? 0,
                              low_count = e?.low_count ?? 0,

                              high_score = e?.high_score ?? 0,
                              medium_score = e?.medium_score ?? 0,
                              low_score = e?.low_score ?? 0,
                              total_score = e?.total_score ?? 0,

                              // نرخ رویداد به ازای هر 100 پرواز
                              high_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.high_count ?? 0) * 100.0) / f.flight_count,
                              medium_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.medium_count ?? 0) * 100.0) / f.flight_count,
                              low_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.low_count ?? 0) * 100.0) / f.flight_count,
                              total_count_per_100 = (f.flight_count == 0) ? 0.0 : ((e?.event_count ?? 0) * 100.0) / f.flight_count,

                              // امتیاز/پرواز (اختیاری)
                              high_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.high_score ?? 0) * 1.0) / f.flight_count,
                              medium_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.medium_score ?? 0) * 1.0) / f.flight_count,
                              low_score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.low_score ?? 0) * 1.0) / f.flight_count,
                              score_per_flight = (f.flight_count == 0) ? 0.0 : ((e?.total_score ?? 0) * 1.0) / f.flight_count

                          }
                    ).ToList();



            return new DataResponse()
            {
                Data = new
                {
                    combined,

                },
                IsSuccess = true
            };

        }
        [Route("api/fdm/V2/eventsCrewRoute/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_captain_route(DateTime dt1, DateTime dt2)
        {
            var dt1Date = dt1.Date;
            var dt2Exclusive = dt2.Date.AddDays(1);

            using (var context = new FDMEntities())
            {
                var query = from x in context.view_fdm_processed.AsNoTracking()
                            join y in context.fdm_crew.AsNoTracking() on x.id equals y.processed_id
                            where x.std >= dt1Date && x.std < dt2Exclusive && y.position=="CPT"
                            group new { x, y } by new
                            {
                                register_id=(int?)x.register_id,
                                x.register,
                                ac_type = x.ac_type2,
                                crew_id=(int?)y.crew_id,
                                y.first_name,
                                y.last_name,
                                y.name,
                                y.rank,
                                y.rank_code,
                                y.position,
                                dep_id=(int?)x.dep_id,
                                x.dep_iata,
                                x.dep_icao,
                                arr_id=(int?)x.arr_id,
                                x.arr_iata,
                                x.arr_icao
                            }
                    into grp
                            select new
                            {
                                grp.Key.register_id,
                                grp.Key.register,
                                grp.Key.ac_type,
                                grp.Key.crew_id,
                                grp.Key.first_name,
                                grp.Key.last_name,
                                grp.Key.name,
                                grp.Key.rank,
                                grp.Key.rank_code,
                                grp.Key.position,
                                grp.Key.dep_id,
                                grp.Key.dep_iata,
                                grp.Key.dep_icao,
                                grp.Key.arr_id,
                                grp.Key.arr_iata,
                                grp.Key.arr_icao,

                                count = grp.Count(),
                                high_count = grp.Sum(g => g.x.severity == "High" ? 1 : 0),
                                medium_count = grp.Sum(g => g.x.severity == "Medium" ? 1 : 0),
                                low_count = grp.Sum(g => g.x.severity == "Low" ? 1 : 0),
                            };

                var query_flights = from x in context.view_fdm_flight_crew.AsNoTracking()
                                    where x.STD >= dt1Date && x.STD < dt2Exclusive && x.Position == "CPT"
                                    group x by new
                                    {
                                        register_id=(int?)x.RegisterID,
                                        x.Register,
                                        ac_type = x.ac_type2,
                                        crew_id=(int?) x.CrewId,
                                        x.Name,
                                        JobGroup = x.JobGroup,
                                        JobGroupCode = x.JobGroupCode,
                                        Position = x.Position,
                                        dep_id=(int?)x.dep_id,
                                        x.dep_iata,
                                        x.dep_icao,
                                        arr_id=(int?)x.arr_id,
                                        x.arr_iata,
                                        x.arr_icao
                                    }
                    into grp
                                    select new
                                    {
                                        register_id = grp.Key.register_id,
                                        register = grp.Key.Register,
                                        grp.Key.ac_type,
                                        crew_id = grp.Key.crew_id,
                                        crew_name = grp.Key.Name,
                                        crew_job_group = grp.Key.JobGroup,
                                        crew_job_group_code = grp.Key.JobGroupCode,
                                        crew_position = grp.Key.Position,
                                        grp.Key.dep_id,
                                        grp.Key.dep_iata,
                                        grp.Key.dep_icao,
                                        grp.Key.arr_id,
                                        grp.Key.arr_iata,
                                        grp.Key.arr_icao,
                                        count = grp.Count()
                                    };

                var result_register_raw = await query.ToListAsync().ConfigureAwait(false);
                var flights = await query_flights.ToListAsync().ConfigureAwait(false);

                var result_register_crew_route =
                    (from q in flights
                     join r in result_register_raw
                         on new { register_id=q.register_id, crew_id=q.crew_id, dep_id=q.dep_id, arr_id=q.arr_id }
                         equals new { register_id=r.register_id, crew_id=r.crew_id, dep_id=r.dep_id, arr_id=r.arr_id }
                         into gj
                     from r in gj.DefaultIfEmpty()
                     select new
                     {
                         q.register_id,
                         q.register,
                         q.ac_type,
                         q.crew_id,
                         q.crew_name,
                         q.crew_job_group,
                         q.crew_job_group_code,
                         
                         q.dep_id,
                         q.dep_iata,
                         q.dep_icao,
                         q.arr_id,
                         q.arr_iata,
                         q.arr_icao,
                         route = (q.dep_iata ?? "UNK") + "_" + (q.arr_iata ?? "UNK"),

                         flight_count = q.count,

                         event_count = r?.count ?? 0,
                         high_count = r?.high_count ?? 0,
                         medium_count = r?.medium_count ?? 0,
                         low_count = r?.low_count ?? 0,

                         high_score = (r?.high_count ?? 0) * 4,
                         medium_score = (r?.medium_count ?? 0) * 2,
                         low_score = (r?.low_count ?? 0) * 1,
                         total_score = ((r?.high_count ?? 0) * 4) + ((r?.medium_count ?? 0) * 2) + ((r?.low_count ?? 0) * 1),

                         // نرخ رویداد به ازای هر 100 پرواز
                         high_count_per_100 = q.count == 0 ? 0.0 : ((r?.high_count ?? 0) * 100.0) / q.count,
                         medium_count_per_100 = q.count == 0 ? 0.0 : ((r?.medium_count ?? 0) * 100.0) / q.count,
                         low_count_per_100 = q.count == 0 ? 0.0 : ((r?.low_count ?? 0) * 100.0) / q.count,
                         total_count_per_100 = q.count == 0 ? 0.0 : ((r?.count ?? 0) * 100.0) / q.count,
                     })
                    .ToList();

                // ====== تجمیع سطح type/crew/route ======
                var result_type_crew_route =
                    (from x in result_register_crew_route // اینجا همه CPT هستند چون از ابتدا فیلتر شده
                     group x by new { x.ac_type, x.crew_name, x.arr_iata, x.dep_iata, x.route }
                    into grp
                     let sumFlights = grp.Sum(q => q.flight_count)
                     let sumEvents = grp.Sum(q => q.event_count)
                     let highSum = grp.Sum(q => q.high_count)
                     let medSum = grp.Sum(q => q.medium_count)
                     let lowSum = grp.Sum(q => q.low_count)
                     let totalScore = grp.Sum(q => q.total_score)
                     select new
                     {
                         grp.Key.ac_type,
                         grp.Key.crew_name,
                         grp.Key.arr_iata,
                         grp.Key.dep_iata,
                         grp.Key.route,

                         count = sumEvents,
                         high_count = highSum,
                         medium_count = medSum,
                         low_count = lowSum,

                         // نرخ به ازای هر 100 پرواز (با گارد تقسیم بر صفر)
                         high_count_per_100 = sumFlights == 0 ? 0.0 : (highSum * 100.0) / sumFlights,
                         medium_count_per_100 = sumFlights == 0 ? 0.0 : (medSum * 100.0) / sumFlights,
                         low_count_per_100 = sumFlights == 0 ? 0.0 : (lowSum * 100.0) / sumFlights,
                         total_count_per_100 = sumFlights == 0 ? 0.0 : (sumEvents * 100.0) / sumFlights,

                         high_score = grp.Sum(q => q.high_score),
                         medium_score = grp.Sum(q => q.medium_score),
                         low_score = grp.Sum(q => q.low_score),
                         total_score = totalScore,

                         score_per_event = sumEvents == 0 ? 0.0 : Math.Round(totalScore * 1.0 / sumEvents, 1),
                         score_per_flight = sumFlights == 0 ? 0.0 : Math.Round(totalScore * 1.0 / sumFlights, 1),
                     })
                    .ToList();

                return new DataResponse()
                {
                    Data = new
                    {
                        result_register_crew_route,
                        result_type_crew_route,
                    },
                    IsSuccess = true
                };
            }
        }

        [Route("api/fdm/V2/eventsRoute/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_route(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);

            var query = from x in context.view_fdm_processed
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.dep_id,
                            grp.Key.dep_iata,
                            grp.Key.dep_icao,
                            grp.Key.arr_id,
                            grp.Key.arr_iata,
                            grp.Key.arr_icao,
                            route = grp.Key.dep_iata + "-" + grp.Key.arr_iata,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };
            
            var query_flights = from x in context.FlightInformations
                                join y in context.ViewMSNs on x.RegisterID equals y.ID
                                where x.STD >= dt1 && x.STD <= dt2 && (x.FlightStatusID == 3 || x.FlightStatusID == 15 || x.FlightStatusID == 7 || x.FlightStatusID == 17)
                                group x by new { x.RegisterID, y.Register, y.AircraftType2, x.FromAirportId, x.ToAirportId } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.AircraftType2,
                                    grp.Key.FromAirportId,
                                    grp.Key.ToAirportId,
                                    count = grp.Count()
                                };
            var result_register1 = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register_route = (from q in _xx
                                         let matchedResult = result_register1.FirstOrDefault(w => w.register_id == q.register_id && w.dep_id == q.FromAirportId && w.arr_id == q.ToAirportId)
                                         select new
                                         {
                                             q.register_id,
                                             q.register,
                                             q.ac_type,
                                             dep_id= matchedResult != null ? matchedResult.dep_id:-1,
                                             dep_icao= matchedResult != null ? matchedResult.dep_icao:"",
                                             dep_iata= matchedResult != null ? matchedResult.dep_iata:"",
                                             arr_id= matchedResult != null ? matchedResult.arr_id:-1,
                                             arr_iata= matchedResult != null ? matchedResult.arr_iata:"",
                                             arr_icao= matchedResult != null ? matchedResult.arr_icao:"",
                                             route = matchedResult!=null? matchedResult.dep_iata + "-" + matchedResult.arr_iata:"",
                                             flight_count = q.count,
                                             event_count = matchedResult?.count ?? 0,
                                             high_count = matchedResult?.high_count ?? 0,
                                             medium_count = matchedResult?.medium_count ?? 0,
                                             low_count = matchedResult?.low_count ?? 0,
                                             high_score = (matchedResult?.high_count ?? 0) * 4,
                                             medium_score = (matchedResult?.medium_count ?? 0) * 2,
                                             low_score = (matchedResult?.low_count ?? 0) * 1,
                                             total_score = matchedResult?.total_score ?? 0,
                                             score_per_event = matchedResult != null && matchedResult.count > 0
                                                                  ? Math.Round((double)matchedResult.total_score / matchedResult.count, 1)
                                                                  : 0,
                                             score_per_flight = q.count > 0
                                                    ? Math.Round((double)(matchedResult?.total_score ?? 0) / q.count, 1)
                                                    : 0
                                         }).ToList();


            var result_type_route = (from x in result_register_route
                                     group x by new { x.ac_type, x.arr_iata, x.dep_iata, x.route } into grp
                                     select new
                                     {
                                         grp.Key.ac_type,
                                         grp.Key.arr_iata,
                                         grp.Key.dep_iata,
                                         grp.Key.route,
                                         count = grp.Sum(q => q.event_count),
                                         high_count = grp.Sum(q => q.high_count),
                                         medium_count = grp.Sum(q => q.medium_count),
                                         low_count = grp.Sum(q => q.low_count),
                                         high_score = grp.Sum(q => q.high_score),
                                         medium_score = grp.Sum(q => q.medium_score),
                                         low_score = grp.Sum(q => q.low_score),
                                         total_score = grp.Sum(q => q.total_score),
                                         score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                         score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1),
                                         //registers= result_register_route.Where(q=>q.ac_type== grp.Key.ac_type && q.route== grp.Key.route).OrderByDescending(q=>q.score_per_flight).ToList(),
                                         registers=grp.ToList(),
                                     }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register_route,
                    result_type_route,

                },
                IsSuccess = true
            };

        }

        [Route("api/fdm/V2/eventsPhase/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_phase(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);

            var query = from x in context.view_fdm_processed
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, x.phase } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.phase,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.FlightInformations
                                join y in context.ViewMSNs on x.RegisterID equals y.ID
                                where x.STD >= dt1 && x.STD <= dt2 && (x.FlightStatusID == 3 || x.FlightStatusID == 15 || x.FlightStatusID == 7 || x.FlightStatusID == 17)
                                group x by new { x.RegisterID, y.Register, y.AircraftType2 } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.AircraftType2,
                                    count = grp.Count()
                                };
            var result_register1 = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register_phase = (from q in result_register1
                                         let matchedResult = _xx.FirstOrDefault(w => w.register_id == q.register_id)
                                         select new
                                         {
                                             q.register_id,
                                             q.register,
                                             q.ac_type,
                                             q.phase,
                                             flight_count = matchedResult.count,
                                             event_count = q?.count ?? 0,
                                             high_count = q?.high_count ?? 0,
                                             medium_count = q?.medium_count ?? 0,
                                             low_count = q?.low_count ?? 0,
                                             high_score = (q?.high_count ?? 0) * 4,
                                             medium_score = (q?.medium_count ?? 0) * 2,
                                             low_score = (q?.low_count ?? 0) * 1,
                                             total_score = q?.total_score ?? 0,
                                             score_per_event = q != null && q.count > 0
                                                                  ? Math.Round((double)q.total_score / q.count, 1)
                                                                  : 0,
                                             score_per_flight = matchedResult.count > 0
                                                    ? Math.Round((double)(q?.total_score ?? 0) / matchedResult.count, 1)
                                                    : 0
                                         }).ToList();


            var result_type_phase = (from x in result_register_phase
                                     group x by new { x.ac_type, x.phase } into grp
                                     select new
                                     {
                                         grp.Key.ac_type,
                                         grp.Key.phase,
                                         count = grp.Sum(q => q.event_count),
                                         high_count = grp.Sum(q => q.high_count),
                                         medium_count = grp.Sum(q => q.medium_count),
                                         low_count = grp.Sum(q => q.low_count),
                                         high_score = grp.Sum(q => q.high_score),
                                         medium_score = grp.Sum(q => q.medium_score),
                                         low_score = grp.Sum(q => q.low_score),
                                         total_score = grp.Sum(q => q.total_score),
                                         score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                         score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                                     }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register_phase,
                    result_type_phase,

                },
                IsSuccess = true
            };

        }
        [Route("api/fdm/V2/eventsCrewPhase/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_captain_phase(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position,x.pf, x.phase } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.crew_id,
                            grp.Key.first_name,
                            grp.Key.last_name,
                            grp.Key.name,
                            grp.Key.rank,
                            grp.Key.rank_code,
                            grp.Key.position,
                            grp.Key.pf,
                            grp.Key.phase,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.view_fdm_flight_crew
                                where x.STD >= dt1 && x.STD <= dt2
                                group x by new { x.RegisterID, x.Register, x.ac_type2, x.CrewId, x.Name, x.JobGroup, x.JobGroupCode, x.Position ,x.pf} into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.ac_type2,
                                    crew_id = grp.Key.CrewId,
                                    crew_name = grp.Key.Name,
                                    crew_job_group = grp.Key.JobGroup,
                                    crew_job_group_code = grp.Key.JobGroupCode,
                                    crew_position = grp.Key.Position,
                                    count = grp.Count()
                                };
            var _ee = await query.ToListAsync();
            var _ff = await query_flights.ToListAsync();
            var result_register_crew_phase = (from q in _ee
                                              where (q.position == "CPT" || (q.position == "FO" && q.pf == "F"))
                                              let matchedResult = _ff.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.crew_position == q.position )
                                              select new
                                              {
                                                  q.register_id,
                                                  q.register,
                                                  q.ac_type,
                                                  q.crew_id,
                                                  crew_name = q.name,
                                                  crew_job_group = matchedResult.crew_job_group,
                                                  crew_job_group_code = matchedResult.crew_job_group_code,
                                                  crew_position = q.position,
                                                  q.phase,
                                                  flight_count = matchedResult.count,
                                                  event_count = q?.count ?? 0,
                                                  high_count = q?.high_count ?? 0,
                                                  medium_count = q?.medium_count ?? 0,
                                                  low_count = q?.low_count ?? 0,
                                                  high_score = q?.high_count * 4 ?? 0,
                                                  medium_score = q?.medium_count * 2 ?? 0,
                                                  low_score = q?.low_count * 1 ?? 0,
                                                  total_score = q?.total_score ?? 0
                                              }).ToList();

            var result_type_crew_phase = (from x in result_register_crew_phase
                                          //where x.crew_position == "CPT"
                                          group x by new { x.ac_type, x.crew_name, x.phase ,x.crew_id,x.crew_position} into grp
                                          select new
                                          {
                                              
                                              grp.Key.crew_id,
                                              grp.Key.ac_type,
                                              grp.Key.crew_name,
                                              grp.Key.crew_position,
                                              grp.Key.phase,
                                              count = grp.Sum(q => q.event_count),
                                              high_count = grp.Sum(q => q.high_count),
                                              medium_count = grp.Sum(q => q.medium_count),
                                              low_count = grp.Sum(q => q.low_count),
                                              high_score = grp.Sum(q => q.high_score),
                                              medium_score = grp.Sum(q => q.medium_score),
                                              low_score = grp.Sum(q => q.low_score),
                                              total_score = grp.Sum(q => q.total_score),
                                              score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                              score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                                          }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register_crew_phase,
                    result_type_crew_phase,

                },
                IsSuccess = true
            };

        }
        [Route("api/fdm/V2/eventsCrewRoutePhase/{dt1}/{dt2}")]
        public async Task<DataResponse> get_events_captain_route_phase(DateTime dt1, DateTime dt2)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std <= dt2
                        group x by new { x.register_id, x.register, x.ac_type2, x.phase, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, y.position, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.phase,
                            grp.Key.crew_id,
                            grp.Key.first_name,
                            grp.Key.last_name,
                            grp.Key.name,
                            grp.Key.rank,
                            grp.Key.rank_code,
                            grp.Key.position,
                            grp.Key.dep_id,
                            grp.Key.dep_iata,
                            grp.Key.dep_icao,
                            grp.Key.arr_id,
                            grp.Key.arr_iata,
                            grp.Key.arr_icao,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.view_fdm_flight_crew
                                where x.STD >= dt1 && x.STD <= dt2
                                group x by new { x.RegisterID, x.Register, x.ac_type2, x.CrewId, x.Name, x.JobGroup, x.JobGroupCode, x.Position, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.ac_type2,
                                    crew_id = grp.Key.CrewId,
                                    crew_name = grp.Key.Name,
                                    crew_job_group = grp.Key.JobGroup,
                                    crew_job_group_code = grp.Key.JobGroupCode,
                                    crew_position = grp.Key.Position,
                                    grp.Key.dep_id,
                                    grp.Key.dep_iata,
                                    grp.Key.dep_icao,
                                    grp.Key.arr_id,
                                    grp.Key.arr_iata,
                                    grp.Key.arr_icao,
                                    count = grp.Count()
                                };
            var _ee = await query.ToListAsync();
            var _ff = await query_flights.ToListAsync();
            var result_register_crew_route_phase = (from q in _ee
                                                    where q.position == "CPT"
                                                    let matchedResult = _ff.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.crew_position == q.position && w.arr_id == q.arr_id && w.dep_id == q.dep_id)

                                                    select new
                                                    {
                                                        q.register_id,
                                                        q.register,
                                                        q.ac_type,
                                                        q.phase,
                                                        q.crew_id,
                                                        crew_name = q.name,
                                                        crew_job_group = matchedResult.crew_job_group,
                                                        crew_job_group_code = matchedResult.crew_job_group_code,
                                                        crew_position = q.position,
                                                        q.dep_id,
                                                        q.dep_iata,
                                                        q.dep_icao,
                                                        q.arr_id,
                                                        q.arr_iata,
                                                        q.arr_icao,
                                                        route = q.dep_iata + '_' + q.arr_iata,
                                                        flight_count = matchedResult.count,
                                                        event_count = q?.count ?? 0,
                                                        high_count = q?.high_count ?? 0,
                                                        medium_count = q?.medium_count ?? 0,
                                                        low_count = q?.low_count ?? 0,
                                                        high_score = q?.high_count * 4 ?? 0,
                                                        medium_score = q?.medium_count * 2 ?? 0,
                                                        low_score = q?.low_count * 1 ?? 0,
                                                        total_score = q?.total_score ?? 0
                                                    }).ToList();

            var result_type_crew_route_phase = (from x in result_register_crew_route_phase
                                                where x.crew_position == "CPT"
                                                group x by new { x.ac_type, x.crew_name, x.arr_iata, x.dep_iata, x.route, x.phase } into grp
                                                select new
                                                {
                                                    grp.Key.ac_type,
                                                    grp.Key.phase,
                                                    grp.Key.crew_name,
                                                    grp.Key.arr_iata,
                                                    grp.Key.dep_iata,
                                                    grp.Key.route,
                                                    count = grp.Sum(q => q.event_count),
                                                    high_count = grp.Sum(q => q.high_count),
                                                    medium_count = grp.Sum(q => q.medium_count),
                                                    low_count = grp.Sum(q => q.low_count),
                                                    high_score = grp.Sum(q => q.high_score),
                                                    medium_score = grp.Sum(q => q.medium_score),
                                                    low_score = grp.Sum(q => q.low_score),
                                                    total_score = grp.Sum(q => q.total_score),
                                                    score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                                    score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                                                }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register_crew_route_phase,
                    result_type_crew_route_phase,

                },
                IsSuccess = true
            };

        }

        [Route("api/fdm/V2/eventsInfo/{dt1}/{dt2}/{type}/{register_id}/{cpt_id}/{route}/{phase}/{severity}/{position}")]
        public async Task<DataResponse> get_events_info(DateTime dt1, DateTime dt2,string type,int register_id,int cpt_id, string route,string phase,string severity,string position)
        {
            try
            {
                using (var context = new FDMEntities())
                {
                    var dt2Exclusive = dt2.Date.AddDays(1);

                    IQueryable<FDMProcessedDto> query;

                    if (cpt_id != 0)
                    {
                        query = from x in context.view_fdm_processed
                                join y in context.fdm_crew on x.id equals y.processed_id
                                where x.std >= dt1 && x.std < dt2Exclusive && y.crew_id == cpt_id && (y.position == "CPT" || (y.position == "FO" && x.pf == "F"))
                                select new FDMProcessedDto
                                {
                                    register = x.register,
                                    register_id = (int)x.register_id,
                                    std = (DateTime)x.std,
                                    flight_id = (int)x.flight_id,
                                    flight_number = x.flight_number,
                                    state_name = x.state_name,
                                    type = x.type,
                                    severity = x.severity,
                                    ac_type = x.ac_type,
                                    ac_type2 = x.ac_type2,
                                    ac_type_id = (int)x.ac_type_id,
                                    arr_iata = x.arr_iata,
                                    dep_iata = x.dep_iata,
                                    route=x.dep_iata+"-"+x.arr_iata,
                                    cpt1_id = x.cpt1_id,
                                    cpt1_first_name = x.cpt1_first_name,
                                    cpt1_last_name = x.cpt1_last_name,
                                    cp1_name = x.cpt1_first_name + " " + x.cpt1_last_name,
                                    cpt2_id = x.cpt2_id,
                                    cp2_name = x.cpt2_first_name + " " + x.cpt2_last_name,
                                    ip1_id = x.ip1_id,
                                    ip1_name = x.ip1_first_name + " " + x.ip1_last_name,
                                    ip2_id = x.ip2_id,
                                    ip2_name = x.ip2_first_name + " " + x.ip2_last_name,
                                    phase = x.phase,
                                    value = x.value,
                                    event_name=x.event_name,
                                    crew_id = y.crew_id,
                                    position = y.position
                                };
                    }
                    else
                    {
                        query = from x in context.view_fdm_processed
                                where x.std >= dt1 && x.std < dt2Exclusive
                                select new FDMProcessedDto
                                {
                                    register = x.register,
                                    register_id = (int)x.register_id,
                                    flight_id=(int)x.flight_id,
                                    flight_number=x.flight_number,
                                    std =(DateTime) x.std,
                                    state_name = x.state_name,
                                    type = x.type,
                                    severity = x.severity,
                                    ac_type = x.ac_type,
                                    ac_type2 = x.ac_type2,
                                    ac_type_id = (int)x.ac_type_id,
                                    arr_iata = x.arr_iata,
                                    dep_iata = x.dep_iata,
                                    route = x.dep_iata + "-" + x.arr_iata,
                                    cpt1_id = x.cpt1_id,
                                    cpt1_first_name = x.cpt1_first_name,
                                    cpt1_last_name = x.cpt1_last_name,
                                    cp1_name = x.cpt1_first_name + " " + x.cpt1_last_name,
                                    cpt2_id = x.cpt2_id,
                                    cp2_name = x.cpt2_first_name + " " + x.cpt2_last_name,
                                    ip1_id = x.ip1_id,
                                    ip1_name = x.ip1_first_name + " " + x.ip1_last_name,
                                    ip2_id = x.ip2_id,
                                    ip2_name = x.ip2_first_name + " " + x.ip2_last_name,
                                    phase = x.phase,
                                    value = x.value,
                                    event_name=x.event_name,
                                    crew_id = null,
                                    position = null
                                };
                    }

                    // فیلترها
                    if (!string.IsNullOrEmpty(type) && type != "-")
                    {
                        query = query.Where(q => q.ac_type2 == type);
                    }

                    if (register_id != 0)
                    {
                        query = query.Where(q => q.register_id == register_id);
                    }
                    if (severity != "0" && severity!="-")
                    {
                        query = query.Where(q => q.severity == severity);
                    }

                    if (!string.IsNullOrEmpty(route) && route != "-")
                    {
                        query = query.Where(q => (q.dep_iata + "-" + q.arr_iata) == route);
                    }

                    if (!string.IsNullOrEmpty(phase) && phase != "-")
                    {
                        query = query.Where(q => q.phase == phase);
                    }
                    if (!string.IsNullOrEmpty(position) && position != "-")
                    {
                        query = query.Where(q => q.position == position);
                    }


                    var raw = query.ToList();

                    return new DataResponse
                    {
                        Data = new { Items = raw },
                        IsSuccess = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse
                {
                    Data = new { error = ex.Message },
                    IsSuccess = false
                };
            }

        }
        
        //------------selected crew-----------------------------
        [Route("api/fdm/V2/crew/{cid}/{dt1}/{dt2}/{position}")]
        public async Task<DataResponse> get_events_captain(int cid, DateTime dt1, DateTime dt2,string position)
        {
            FDMEntities context = new FDMEntities();

            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std < dt2 && y.crew_id == cid && y.position == position && (position !="FO"|| x.pf == "F")

                        group x by new { x.register_id, x.register, x.ac_type2, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.crew_id,
                            grp.Key.first_name,
                            grp.Key.last_name,
                            grp.Key.name,
                            grp.Key.rank,
                            grp.Key.rank_code,

                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.view_fdm_flight_crew
                                where x.STD >= dt1 && x.STD < dt2 && x.CrewId == cid &&  x.Position == position && (position != "FO" || x.pf == "F")
                                group x by new { x.RegisterID, x.Register, x.ac_type2, x.CrewId, x.Name, x.JobGroup, x.JobGroupCode } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.ac_type2,
                                    crew_id = grp.Key.CrewId,
                                    crew_name = grp.Key.Name,
                                    crew_job_group = grp.Key.JobGroup,
                                    crew_job_group_code = grp.Key.JobGroupCode,
                                    

                                    count = grp.Count()
                                };
            var result_register = await query.ToListAsync();
            var _xx = await query_flights.ToListAsync();
            var result_register_crew = (from q in _xx
                                            //  where (q.crew_position == "CPT" || q.crew_position == "FO")
                                        select new
                                        {
                                            q.register_id,
                                            q.register,
                                            q.ac_type,
                                            q.crew_id,
                                            q.crew_name,
                                            q.crew_job_group,
                                            q.crew_job_group_code,
                                            
                                            flight_count = q.count,
                                            event_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id )?.count ?? 0,
                                            high_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id )?.high_count ?? 0,
                                            medium_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id )?.medium_count ?? 0,
                                            low_count = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id )?.low_count ?? 0,
                                            high_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id )?.high_count * 4 ?? 0,
                                            medium_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id )?.medium_count * 2 ?? 0,
                                            low_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id )?.low_count * 1 ?? 0,
                                            total_score = result_register.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id )?.total_score ?? 0
                                        }).ToList();

            var result_type_crew = (from x in result_register_crew
                                        //where (x.crew_position == "CPT" || x.crew_position == "FO")
                                    group x by new { x.ac_type, x.crew_id, x.crew_name} into grp
                                    select new
                                    {
                                        grp.Key.ac_type,
                                        grp.Key.crew_id,
                                        grp.Key.crew_name,
                                        
                                        count = grp.Sum(q => q.event_count),
                                        high_count = grp.Sum(q => q.high_count),
                                        medium_count = grp.Sum(q => q.medium_count),
                                        low_count = grp.Sum(q => q.low_count),
                                        high_score = grp.Sum(q => q.high_score),
                                        medium_score = grp.Sum(q => q.medium_score),
                                        low_score = grp.Sum(q => q.low_score),
                                        total_score = grp.Sum(q => q.total_score),
                                        flight_count = grp.Sum(q => q.flight_count),
                                        score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                        score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                                    }).ToList();




            var query_route = from x in context.view_fdm_processed
                              join y in context.fdm_crew on x.id equals y.processed_id
                              where x.std >= dt1 && x.std < dt2 && y.crew_id == cid &&  y.position == position && (position != "FO" || x.pf == "F")
                              group x by new
                              { x.register_id, x.register, x.ac_type2, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                              select new
                              {
                                  grp.Key.register_id,
                                  grp.Key.register,
                                  ac_type = grp.Key.ac_type2,
                                  grp.Key.crew_id,
                                  grp.Key.first_name,
                                  grp.Key.last_name,
                                  grp.Key.name,
                                  grp.Key.rank,
                                  grp.Key.rank_code,
                                  

                                  grp.Key.dep_id,
                                  grp.Key.dep_iata,
                                  grp.Key.dep_icao,
                                  grp.Key.arr_id,
                                  grp.Key.arr_iata,
                                  grp.Key.arr_icao,
                                  route_iata = grp.Key.dep_iata + "-" + grp.Key.arr_iata,
                                  route_icao = grp.Key.dep_icao + "-" + grp.Key.arr_icao,
                                  count = grp.Count(),
                                  high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                                  medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                                  low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                                  total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                           grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                           grp.Sum(g => g.severity == "Low" ? 1 : 0)
                              };

            var query_flights_route = from x in context.view_fdm_flight_crew
                                      where x.STD >= dt1 && x.STD < dt2 && x.CrewId == cid && x.Position == position && (position != "FO" || x.pf == "F")
                                      group x by new { x.RegisterID, x.Register, x.ac_type2, x.CrewId, x.Name, x.JobGroup, x.JobGroupCode,  x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                                      select new
                                      {
                                          register_id = grp.Key.RegisterID,
                                          register = grp.Key.Register,
                                          ac_type = grp.Key.ac_type2,
                                          crew_id = grp.Key.CrewId,
                                          crew_name = grp.Key.Name,
                                          crew_job_group = grp.Key.JobGroup,
                                          crew_job_group_code = grp.Key.JobGroupCode,
                                          
                                          grp.Key.dep_id,
                                          grp.Key.dep_iata,
                                          grp.Key.dep_icao,
                                          grp.Key.arr_id,
                                          grp.Key.arr_iata,
                                          grp.Key.arr_icao,
                                          count = grp.Count()
                                      };

            var result_register_route = await query_route.ToListAsync();
            var _xx_route = await query_flights_route.ToListAsync();
            var result_register_crew_route = (from q in _xx_route
                                                  //where q.crew_position == "CPT" || q.crew_position=="FO"   
                                              let machedResult = result_register_route.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id && w.dep_id == q.dep_id && w.arr_id == q.arr_id)
                                              select new
                                              {
                                                  q.register_id,
                                                  q.register,
                                                  q.ac_type,
                                                  q.crew_id,
                                                  q.crew_name,
                                                  q.crew_job_group,
                                                  q.crew_job_group_code,
                                                  
                                                  q.dep_id,
                                                  q.dep_iata,
                                                  q.dep_icao,
                                                  q.arr_id,
                                                  q.arr_iata,
                                                  q.arr_icao,
                                                  route_iata = q.dep_iata + '-' + q.arr_iata,
                                                  flight_count = q.count,
                                                  event_count = machedResult?.count ?? 0,
                                                  high_count = machedResult?.high_count ?? 0,
                                                  medium_count = machedResult?.medium_count ?? 0,
                                                  low_count = machedResult?.low_count ?? 0,
                                                  high_score = machedResult?.high_count * 4 ?? 0,
                                                  medium_score = machedResult?.medium_count * 2 ?? 0,
                                                  low_score = machedResult?.low_count * 1 ?? 0,
                                                  total_score = machedResult?.total_score ?? 0
                                              }).ToList();

            var result_type_crew_route = (from x in result_register_crew_route
                                              //where x.crew_position == "CPT" || x.crew_position == "FO"

                                          group x by new { x.ac_type, x.crew_name, x.arr_iata, x.dep_iata, x.route_iata } into grp
                                          select new
                                          {
                                              grp.Key.ac_type,
                                              grp.Key.crew_name,
                                              grp.Key.arr_iata,
                                              grp.Key.dep_iata,
                                              grp.Key.route_iata,
                                              
                                              count = grp.Sum(q => q.event_count),
                                              high_count = grp.Sum(q => q.high_count),
                                              medium_count = grp.Sum(q => q.medium_count),
                                              low_count = grp.Sum(q => q.low_count),
                                              high_score = grp.Sum(q => q.high_score),
                                              medium_score = grp.Sum(q => q.medium_score),
                                              low_score = grp.Sum(q => q.low_score),
                                              total_score = grp.Sum(q => q.total_score),
                                              score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                              score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                                          }).ToList();



            var query_events = from x in context.view_fdm_processed
                               join y in context.fdm_crew on x.id equals y.processed_id
                               where x.std >= dt1 && x.std < dt2 && y.crew_id == cid && y.position == position && (position != "FO" || x.pf == "F")
                               group x by new { y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, x.event_name } into grp
                               select new
                               {
                                   grp.Key.event_name,

                                   grp.Key.crew_id,
                                   grp.Key.first_name,
                                   grp.Key.last_name,
                                   grp.Key.name,
                                   grp.Key.rank,
                                   grp.Key.rank_code,
                                   
                                   count = grp.Count(),
                                   high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                                   medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                                   low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                                   total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                            grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                            grp.Sum(g => g.severity == "Low" ? 1 : 0)
                               };

            var result_events = query_events.OrderByDescending(q => q.count).ToList();
            return new DataResponse()
            {
                Data = new
                {
                    result_register_crew,
                    result_type_crew,
                    result_register_crew_route,
                    result_type_crew_route,
                    result_events
                },
                IsSuccess = true
            };

        }
        [Route("api/fdm/V2/crew/phase/{cid}/{dt1}/{dt2}/{position}")]
        public async Task<DataResponse> get_events_captain_phase(int cid, DateTime dt1, DateTime dt2,string position)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std < dt2 && y.crew_id == cid && y.position == position && (position != "FO" || x.pf == "F")
                        group x by new { x.register_id, x.register, x.ac_type2, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code,  x.phase } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.crew_id,
                            grp.Key.first_name,
                            grp.Key.last_name,
                            grp.Key.name,
                            grp.Key.rank,
                            grp.Key.rank_code,
                            
                            grp.Key.phase,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.view_fdm_flight_crew
                                where x.STD >= dt1 && x.STD < dt2 && x.CrewId == cid && x.Position == position && (position != "FO" || x.pf == "F")
                                group x by new { x.RegisterID, x.Register, x.ac_type2, x.CrewId, x.Name, x.JobGroup, x.JobGroupCode } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.ac_type2,
                                    crew_id = grp.Key.CrewId,
                                    crew_name = grp.Key.Name,
                                    crew_job_group = grp.Key.JobGroup,
                                    crew_job_group_code = grp.Key.JobGroupCode,
                                    
                                    count = grp.Count()
                                };
            var _ee = await query.ToListAsync();
            var _ff = await query_flights.ToListAsync();
            var result_register_crew_phase = (from q in _ee
                                                  //where q.position == "CPT" || q.position=="FO"
                                              let matchedResult = _ff.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id )
                                              select new
                                              {
                                                  q.register_id,
                                                  q.register,
                                                  q.ac_type,
                                                  q.crew_id,
                                                  crew_name = q.name,
                                                  crew_job_group = matchedResult.crew_job_group,
                                                  crew_job_group_code = matchedResult.crew_job_group_code,
                                                  
                                                  q.phase,
                                                  flight_count = matchedResult.count,
                                                  event_count = q?.count ?? 0,
                                                  high_count = q?.high_count ?? 0,
                                                  medium_count = q?.medium_count ?? 0,
                                                  low_count = q?.low_count ?? 0,
                                                  high_score = q?.high_count * 4 ?? 0,
                                                  medium_score = q?.medium_count * 2 ?? 0,
                                                  low_score = q?.low_count * 1 ?? 0,
                                                  total_score = q?.total_score ?? 0
                                              }).ToList();

            var result_type_crew_phase = (from x in result_register_crew_phase
                                              //where x.crew_position == "CPT" || x.crew_position == "FO"
                                          group x by new { x.ac_type, x.crew_name, x.phase, x.crew_id } into grp
                                          select new
                                          {

                                              grp.Key.crew_id,
                                              grp.Key.ac_type,
                                              grp.Key.crew_name,
                                              grp.Key.phase,
                                              count = grp.Sum(q => q.event_count),
                                              high_count = grp.Sum(q => q.high_count),
                                              medium_count = grp.Sum(q => q.medium_count),
                                              low_count = grp.Sum(q => q.low_count),
                                              high_score = grp.Sum(q => q.high_score),
                                              medium_score = grp.Sum(q => q.medium_score),
                                              low_score = grp.Sum(q => q.low_score),
                                              total_score = grp.Sum(q => q.total_score),
                                              score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                              score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                                          }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register_crew_phase,
                    result_type_crew_phase,

                },
                IsSuccess = true
            };

        }
        [Route("api/fdm/V2/crew/route/phase/{cid}/{dt1}/{dt2}/{position}")]
        public async Task<DataResponse> get_events_captain_route_phase(int cid, DateTime dt1, DateTime dt2,string position)
        {
            FDMEntities context = new FDMEntities();
            dt2 = dt2.AddDays(1);
            var query = from x in context.view_fdm_processed
                        join y in context.fdm_crew on x.id equals y.processed_id
                        where x.std >= dt1 && x.std < dt2 && y.crew_id == cid && y.position == position && (position != "FO" || x.pf == "F")
                        group x by new { x.register_id, x.register, x.ac_type2, x.phase, y.crew_id, y.first_name, y.last_name, y.name, y.rank, y.rank_code, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                        select new
                        {
                            grp.Key.register_id,
                            grp.Key.register,
                            ac_type = grp.Key.ac_type2,
                            grp.Key.phase,
                            grp.Key.crew_id,
                            grp.Key.first_name,
                            grp.Key.last_name,
                            grp.Key.name,
                            grp.Key.rank,
                            grp.Key.rank_code,
                            
                            grp.Key.dep_id,
                            grp.Key.dep_iata,
                            grp.Key.dep_icao,
                            grp.Key.arr_id,
                            grp.Key.arr_iata,
                            grp.Key.arr_icao,
                            count = grp.Count(),
                            high_count = grp.Sum(g => g.severity == "High" ? 1 : 0),
                            medium_count = grp.Sum(g => g.severity == "Medium" ? 1 : 0),
                            low_count = grp.Sum(g => g.severity == "Low" ? 1 : 0),
                            total_score = grp.Sum(g => g.severity == "High" ? 1 : 0) * 4 +
                                     grp.Sum(g => g.severity == "Medium" ? 1 : 0) * 2 +
                                     grp.Sum(g => g.severity == "Low" ? 1 : 0)
                        };

            var query_flights = from x in context.view_fdm_flight_crew
                                where x.STD >= dt1 && x.STD < dt2 && x.CrewId == cid && x.Position == position && (position != "FO" || x.pf == "F")
                                group x by new { x.RegisterID, x.Register, x.ac_type2, x.CrewId, x.Name, x.JobGroup, x.JobGroupCode, x.Position, x.dep_id, x.dep_iata, x.dep_icao, x.arr_id, x.arr_iata, x.arr_icao } into grp
                                select new
                                {
                                    register_id = grp.Key.RegisterID,
                                    register = grp.Key.Register,
                                    ac_type = grp.Key.ac_type2,
                                    crew_id = grp.Key.CrewId,
                                    crew_name = grp.Key.Name,
                                    crew_job_group = grp.Key.JobGroup,
                                    crew_job_group_code = grp.Key.JobGroupCode,
                                    
                                    grp.Key.dep_id,
                                    grp.Key.dep_iata,
                                    grp.Key.dep_icao,
                                    grp.Key.arr_id,
                                    grp.Key.arr_iata,
                                    grp.Key.arr_icao,
                                    count = grp.Count()
                                };
            var _ee = await query.ToListAsync();
            var _ff = await query_flights.ToListAsync();
            var result_register_crew_route_phase = (from q in _ee
                                                        //where q.position == "CPT" || q.position == "FO"
                                                    let matchedResult = _ff.FirstOrDefault(w => w.register_id == q.register_id && w.crew_id == q.crew_id &&  w.arr_id == q.arr_id && w.dep_id == q.dep_id)

                                                    select new
                                                    {
                                                        q.register_id,
                                                        q.register,
                                                        q.ac_type,
                                                        q.phase,
                                                        q.crew_id,
                                                        crew_name = q.name,
                                                        crew_job_group = matchedResult.crew_job_group,
                                                        crew_job_group_code = matchedResult.crew_job_group_code,
                                                        
                                                        q.dep_id,
                                                        q.dep_iata,
                                                        q.dep_icao,
                                                        q.arr_id,
                                                        q.arr_iata,
                                                        q.arr_icao,
                                                        route = q.dep_iata + '_' + q.arr_iata,
                                                        flight_count = matchedResult.count,
                                                        event_count = q?.count ?? 0,
                                                        high_count = q?.high_count ?? 0,
                                                        medium_count = q?.medium_count ?? 0,
                                                        low_count = q?.low_count ?? 0,
                                                        high_score = q?.high_count * 4 ?? 0,
                                                        medium_score = q?.medium_count * 2 ?? 0,
                                                        low_score = q?.low_count * 1 ?? 0,
                                                        total_score = q?.total_score ?? 0
                                                    }).ToList();

            var result_type_crew_route_phase = (from x in result_register_crew_route_phase
                                                    //where x.crew_position == "CPT" || x.crew_position == "FO"
                                                group x by new { x.ac_type, x.crew_name, x.arr_iata, x.dep_iata, x.route, x.phase } into grp
                                                select new
                                                {
                                                    grp.Key.ac_type,
                                                    grp.Key.phase,
                                                    grp.Key.crew_name,
                                                    grp.Key.arr_iata,
                                                    grp.Key.dep_iata,
                                                    grp.Key.route,
                                                    count = grp.Sum(q => q.event_count),
                                                    high_count = grp.Sum(q => q.high_count),
                                                    medium_count = grp.Sum(q => q.medium_count),
                                                    low_count = grp.Sum(q => q.low_count),
                                                    high_score = grp.Sum(q => q.high_score),
                                                    medium_score = grp.Sum(q => q.medium_score),
                                                    low_score = grp.Sum(q => q.low_score),
                                                    total_score = grp.Sum(q => q.total_score),
                                                    score_per_event = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.event_count), 1),
                                                    score_per_flight = Math.Round(grp.Sum(q => q.total_score) * 1.0 / grp.Sum(q => q.flight_count), 1)
                                                }).ToList();


            return new DataResponse()
            {
                Data = new
                {
                    result_register_crew_route_phase,
                    result_type_crew_route_phase,

                },
                IsSuccess = true
            };

        }

    }
}
