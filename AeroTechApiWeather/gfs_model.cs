using AeroTechApiWeather.Gfs;
using AeroTechApiWeather.Model;
using AeroTechApiWeather.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AeroTechApiWeather.Model
{
    public class Waypoint
    {
        public string Name { get; set; } // اختیاری
        public double Lat { get; set; }  // degrees
        public double Lon { get; set; }  // degrees (-180..180 or 0..360)
    }

    public class FlightPlan
    {
        public string FlightId { get; set; }
        public int CruiseFlightLevel { get; set; }     // مثلا 350
        public DateTime DepartureTimeUtc { get; set; } // برای انتخاب GFS
        public List<Waypoint> Route { get; set; }

        public string Title
        {
            get
            {
                if (Route == null || Route.Count == 0)
                     return "-";

                return Route.First().Name+"-"+Route.Last().Name;

            }
        }
    }
}

namespace AeroTechApiWeather.Model
{
    public class RouteSamplePoint
    {
        public double DistanceNm { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        // Track along the route at this point (deg true)
        public double TrackDeg { get; set; }

        public int LegIndex { get; set; }

        public bool IsWaypoint { get; set; }
        public string WaypointName { get; set; }
    }
}



namespace AeroTechApiWeather.Model
{
    public class CrossSectionSample
    {
        public double DistanceNm { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        public int FlightLevel { get; set; }

        public double WindDirDeg { get; set; }
        public double WindSpeedKt { get; set; }

        public double TemperatureC { get; set; }

        // +Tail, -Head (kt)
        public double HeadTailKt { get; set; }
    }
}

 
namespace AeroTechApiWeather.Model
{
    public class CrossSectionData
    {
        public string FlightId { get; set; }

        public List<RouteSamplePoint> RoutePoints { get; set; } = new List<RouteSamplePoint>();
        public List<int> FlightLevels { get; set; } = new List<int>();

        public List<CrossSectionSample> Samples { get; set; } = new List<CrossSectionSample>();

        // For flight profile (distance-based)
        public double TotalDistanceNm { get; set; }
        public double TocDistanceNm { get; set; }
        public double TodDistanceNm { get; set; }

        public int CruiseFlightLevel { get; set; }

        public DateTime? ValidTimeUtc { get; set; }
        public DateTime? RunTimeUtc { get; set; }
    }
}


 
//namespace AeroTechApiWeather.Services
//{
//    public class CrossSectionService
//    {
//        public double SampleStepNm { get; set; } = 20.0;

//        /// <summary>
//        /// Builds cross-section samples for (distance x FL) grid using provided fields
//        /// at the already-selected GFS level. (You will call this per level set you want.)
//        /// </summary>
//        public CrossSectionData Build(
//            FlightPlan fp,
//            int[] flightLevels,
//            int cruiseFlightLevel,
//            GfsWindField windField,
//            GfsScalarField tempField,
//            DateTime? runTimeUtc = null,
//            DateTime? validTimeUtc = null)
//        {
//            if (fp == null) throw new ArgumentNullException(nameof(fp));
//            if (fp.Route == null || fp.Route.Count < 2) throw new ArgumentException("Route must have >=2 points.");
//            if (flightLevels == null || flightLevels.Length == 0) throw new ArgumentException("flightLevels is empty.");
//            if (cruiseFlightLevel <= 0) throw new ArgumentException("cruiseFlightLevel must be > 0.");

//            var routePts = RouteSampler.SampleRoute(fp.Route, SampleStepNm);

//            double totalNm = routePts[routePts.Count - 1].DistanceNm;

//            // Find TOC/TOD from waypoint names, else default fractions
//            double tocNm = FindNamedDistance(routePts, "TOC");
//            double todNm = FindNamedDistance(routePts, "TOD");

//            if (tocNm < 0) tocNm = totalNm * 0.20;
//            if (todNm < 0) todNm = totalNm * 0.80;

//            // Ensure ordering
//            tocNm = Math.Max(0, Math.Min(totalNm, tocNm));
//            todNm = Math.Max(0, Math.Min(totalNm, todNm));
//            if (todNm < tocNm)
//            {
//                var t = tocNm;
//                tocNm = todNm;
//                todNm = t;
//            }

//            var data = new CrossSectionData
//            {
//                FlightId = fp.FlightId,
//                RoutePoints = routePts,
//                FlightLevels = flightLevels.Distinct().OrderBy(x => x).ToList(),
//                TotalDistanceNm = totalNm,
//                TocDistanceNm = tocNm,
//                TodDistanceNm = todNm,
//                CruiseFlightLevel = cruiseFlightLevel,
//                RunTimeUtc = runTimeUtc,
//                ValidTimeUtc = validTimeUtc
//            };

//            foreach (var pt in routePts)
//            {
//                foreach (int fl in data.FlightLevels)
//                {
//                    var wind = windField.Sample(pt.Lat, pt.Lon);
//                    double tempC = tempField.Sample(pt.Lat, pt.Lon) - 273.15;

//                    double ht = WindMath.HeadTailComponent(pt.TrackDeg, wind.DirDeg, wind.SpeedKt);

//                    data.Samples.Add(new CrossSectionSample
//                    {
//                        DistanceNm = pt.DistanceNm,
//                        Lat = pt.Lat,
//                        Lon = pt.Lon,
//                        FlightLevel = fl,
//                        WindDirDeg = wind.DirDeg,
//                        WindSpeedKt = wind.SpeedKt,
//                        TemperatureC = tempC,
//                        HeadTailKt = ht
//                    });
//                }
//            }

//            return data;
//        }

//        private static double FindNamedDistance(List<RouteSamplePoint> pts, string token)
//        {
//            if (pts == null || pts.Count == 0) return -1;

//            for (int i = 0; i < pts.Count; i++)
//            {
//                if (!pts[i].IsWaypoint) continue;
//                var name = (pts[i].WaypointName ?? "").Trim();
//                if (name.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
//                    return pts[i].DistanceNm;
//            }

//            return -1;
//        }
//    }
//}


