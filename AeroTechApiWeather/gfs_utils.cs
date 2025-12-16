using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AeroTechApiWeather.Gfs;
using AeroTechApiWeather.Mapping;
using AeroTechApiWeather.Model;
using AeroTechApiWeather.Navigation;
using AeroTechApiWeather.Rendering;
using AeroTechApiWeather.Utils;
using Grib.Api;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AeroTechApiWeather.Utils
{
    public static class RouteBounds
    {
        public static (double minLat, double maxLat, double minLon, double maxLon)
            GetBoundingBox(IEnumerable<Waypoint> route, double marginDeg = 2.0)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));
            var list = route.ToList();
            if (list.Count == 0) throw new ArgumentException("Route is empty", nameof(route));

            double minLat = list.Min(w => w.Lat) - marginDeg;
            double maxLat = list.Max(w => w.Lat) + marginDeg;
            double minLon = list.Min(w => w.Lon) - marginDeg;
            double maxLon = list.Max(w => w.Lon) + marginDeg;

            return (minLat, maxLat, minLon, maxLon);
        }
    }
}








namespace AeroTechApiWeather.Utils
{
    public static class FlightLevelConverter
    {
        /// <summary>
        /// Approximate conversion FL -> pressure level (hPa) using ISA.
        /// کافی برای انتخاب نزدیک‌ترین isobaric level جهت نمایش نقشه.
        /// </summary>
        public static int FlightLevelToHpa(int flightLevel)
        {
            double hMeters = flightLevel * 100.0 * 0.3048;

            double P0 = 1013.25; // hPa
            double T0 = 288.15;  // K
            double L = 0.0065;   // K/m
            double g = 9.80665;
            double R = 287.05;

            double exponent = g / (R * L);

            if (hMeters <= 11000.0)
            {
                double P = P0 * Math.Pow(1.0 - L * hMeters / T0, exponent);
                return (int)Math.Round(P);
            }
            else
            {
                double h11 = 11000.0;
                double P11 = P0 * Math.Pow(1.0 - L * h11 / T0, exponent);
                double T11 = T0 - L * h11;

                double P = P11 * Math.Exp(-g * (hMeters - h11) / (R * T11));
                return (int)Math.Round(P);
            }
        }
    }
}




namespace AeroTechApiWeather.Gfs
{
    public class GfsScalarField
    {
        public int Ni { get; private set; }
        public int Nj { get; private set; }

        public double LatFirst { get; private set; }
        public double LonFirst { get; private set; } // 0..360
        public double LatLast { get; private set; }
        public double LonLast { get; private set; }

        public double Di { get; private set; }
        public double Dj { get; private set; }

        private readonly double[,] _values; // [j, i]

        private GfsScalarField(int ni, int nj)
        {
            Ni = ni;
            Nj = nj;
            _values = new double[nj, ni];
        }

        public static GfsScalarField FromGribMessage(GribMessage msg)
        {
            int ni = msg["Ni"].AsInt();
            int nj = msg["Nj"].AsInt();

            var field = new GfsScalarField(ni, nj);

            field.LatFirst = msg["latitudeOfFirstGridPointInDegrees"].AsDouble();
            field.LonFirst = msg["longitudeOfFirstGridPointInDegrees"].AsDouble();
            field.LatLast = msg["latitudeOfLastGridPointInDegrees"].AsDouble();
            field.LonLast = msg["longitudeOfLastGridPointInDegrees"].AsDouble();

            field.Di = msg["iDirectionIncrementInDegrees"].AsDouble();
            field.Dj = msg["jDirectionIncrementInDegrees"].AsDouble();

            double[] values;
            msg.Values(out values);

            int k = 0;
            for (int j = 0; j < nj; j++)
                for (int i = 0; i < ni; i++)
                    field._values[j, i] = values[k++];

            return field;
        }

        public double Sample(double lat, double lon)
        {
            if (lon < 0) lon += 360.0;
            if (lon >= 360.0) lon -= 360.0;

            if (lat > LatFirst) lat = LatFirst;
            if (lat < LatLast) lat = LatLast;

            double jFloat = (LatFirst - lat) / Dj; // north->south
            double iFloat = (lon - LonFirst) / Di; // west->east

            int j0 = (int)Math.Floor(jFloat);
            int j1 = j0 + 1;
            double vj = jFloat - j0;

            if (j0 < 0) { j0 = 0; j1 = 0; vj = 0; }
            if (j1 >= Nj) { j1 = Nj - 1; j0 = Nj - 1; vj = 0; }

            int i0 = (int)Math.Floor(iFloat);
            double vi = iFloat - i0;
            int i1 = i0 + 1;

            i0 = Mod(i0, Ni);
            i1 = Mod(i1, Ni);

            double v00 = _values[j0, i0];
            double v10 = _values[j0, i1];
            double v01 = _values[j1, i0];
            double v11 = _values[j1, i1];

            double v0 = v00 + vi * (v10 - v00);
            double v1 = v01 + vi * (v11 - v01);

            return v0 + vj * (v1 - v0);
        }

        private static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}






namespace AeroTechApiWeather.Gfs
{
    public static class GfsLevels
    {
        public static int[] GetAvailableIsobaricLevelsHpa(GribFile file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            var set = new HashSet<int>();

            foreach (var msg in file)
            {
                string typeOfLevel = msg["typeOfLevel"].AsString();
                if (typeOfLevel != "isobaricInhPa")
                    continue;

                int lvl = msg["level"].AsInt();
                set.Add(lvl);
            }

            var arr = new int[set.Count];
            set.CopyTo(arr);
            Array.Sort(arr);
            return arr;
        }

        public static int SnapToNearest(int targetHpa, int[] availableLevels)
        {
            if (availableLevels == null || availableLevels.Length == 0)
                throw new ArgumentException("availableLevels is empty", nameof(availableLevels));

            int best = availableLevels[0];
            int bestDiff = Math.Abs(best - targetHpa);

            for (int i = 1; i < availableLevels.Length; i++)
            {
                int diff = Math.Abs(availableLevels[i] - targetHpa);
                if (diff < bestDiff)
                {
                    best = availableLevels[i];
                    bestDiff = diff;
                }
            }

            return best;
        }
    }
}



namespace AeroTechApiWeather.Gfs
{
    public struct WindSample
    {
        public double SpeedKt;
        public double DirDeg; // FROM (meteorological)
        public double U;
        public double V;
    }

    public class GfsWindField
    {
        private readonly GfsScalarField _u;
        private readonly GfsScalarField _v;

        public GfsWindField(GfsScalarField uField, GfsScalarField vField)
        {
            _u = uField;
            _v = vField;
        }

        public WindSample Sample(double lat, double lon)
        {
            double u = _u.Sample(lat, lon);
            double v = _v.Sample(lat, lon);

            double speedMs = Math.Sqrt(u * u + v * v);
            double speedKt = speedMs * 1.9438444924406;

            double dirRad = Math.Atan2(-u, -v);
            double dirDeg = dirRad * 180.0 / Math.PI;
            if (dirDeg < 0) dirDeg += 360.0;

            return new WindSample
            {
                U = u,
                V = v,
                SpeedKt = speedKt,
                DirDeg = dirDeg
            };
        }
    }
}






namespace AeroTechApiWeather.Gfs
{
    public static class GfsFieldFactory
    {
        public static GfsWindField CreateWindField(GribFile file, int levelHpa)
        {
            GribMessage msgU = null;
            GribMessage msgV = null;

            foreach (var msg in file)
            {
                string shortName = msg["shortName"].AsString();
                string typeOfLevel = msg["typeOfLevel"].AsString();

                if (typeOfLevel != "isobaricInhPa")
                    continue;

                int lvl = msg["level"].AsInt();
                if (lvl != levelHpa)
                    continue;

                if (shortName == "u") msgU = msg;
                else if (shortName == "v") msgV = msg;

                if (msgU != null && msgV != null)
                    break;
            }

            if (msgU == null || msgV == null)
                throw new InvalidOperationException("U or V field not found for level " + levelHpa);

            var uField = GfsScalarField.FromGribMessage(msgU);
            var vField = GfsScalarField.FromGribMessage(msgV);

            return new GfsWindField(uField, vField);
        }

        public static GfsScalarField CreateTemperatureField(GribFile file, int levelHpa)
        {
            foreach (var msg in file)
            {
                string shortName = msg["shortName"].AsString();
                string typeOfLevel = msg["typeOfLevel"].AsString();

                if (shortName == "t" && typeOfLevel == "isobaricInhPa")
                {
                    int lvl = msg["level"].AsInt();
                    if (lvl == levelHpa)
                        return GfsScalarField.FromGribMessage(msg);
                }
            }

            throw new InvalidOperationException("Temperature field not found for level " + levelHpa);
        }
    }
}




 
 

namespace AeroTechApiWeather.Rendering
{
    public class WindTempMapRenderer
    {
        private const int OuterPadding = 6;

        private const int RightBoxWidth = 200;
        private const int RightBoxPadding = 10;

        private const int LeftBottomBoxWidth = 260;
        private const int LeftBottomBoxHeight = 150;
        private const int LeftBottomBoxPadding = 10;

        public Bitmap Render(
            GfsWindField windField,
            GfsScalarField tempField,
            double minLat, double maxLat,
            double minLon, double maxLon,
            int width, int height,
            double gridStepDeg = 1.0,
            IReadOnlyList<Waypoint> route = null,
            MapMetadata metadata = null,
            RouteWindSummary routeWind = null,
            bool showTailHeadwindLabels = true,
            Bitmap basemap = null) // <-- NEW (optional)
        {
            var bmp = new Bitmap(width, height);

            var mapRect = new Rectangle(
                OuterPadding,
                OuterPadding,
                width - OuterPadding * 2,
                height - OuterPadding * 2);

            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.White);

                g.SetClip(mapRect);

                if (basemap != null)
                {
                    // basemap is rendered for the same bounds; draw it as background
                    g.DrawImage(basemap, mapRect);
                }
                else
                {
                    using (var bg = new SolidBrush(Color.Beige))
                        g.FillRectangle(bg, mapRect);
                }

                // X is linear in lon (matches basemap renderer)
                Func<double, float> xFromLon = lon =>
                {
                    double t = (lon - minLon) / (maxLon - minLon);
                    return mapRect.Left + (float)(t * (mapRect.Width - 1));
                };

                // Y uses Mercator (matches basemap renderer)
                Func<double, float> yFromLat = lat =>
                {
                    lat = Clip(lat, -85.05112878, 85.05112878);

                    double yMerc(double la)
                    {
                        double rad = la * Math.PI / 180.0;
                        double s = Math.Sin(rad);
                        return 0.5 - Math.Log((1 + s) / (1 - s)) / (4 * Math.PI);
                    }

                    double y0 = yMerc(maxLat);
                    double y1 = yMerc(minLat);
                    double y = (yMerc(lat) - y0) / (y1 - y0);

                    return mapRect.Top + (float)(y * (mapRect.Height - 1));
                };

                // Wind barbs + temperature labels
                using (var pen = new Pen(Color.Black, 1))
                using (var tempFont = new Font("Segoe UI", 7))
                using (var tempBrush = new SolidBrush(Color.DarkRed))
                {
                    for (double lat = minLat; lat <= maxLat; lat += gridStepDeg)
                    {
                        for (double lon = minLon; lon <= maxLon; lon += gridStepDeg)
                        {
                            var wind = windField.Sample(lat, lon);
                            double tempC = tempField.Sample(lat, lon) - 273.15;

                            float x = xFromLon(lon);
                            float y = yFromLat(lat);

                            DrawWindBarb(g, pen, x, y, wind.DirDeg, wind.SpeedKt);

                            string ts = Math.Round(tempC).ToString();
                            g.DrawString(ts, tempFont, tempBrush, x + 6, y + 6);
                        }
                    }
                }

                if (route != null && route.Count > 0)
                    DrawRouteAndWaypoints(g, route, xFromLon, yFromLat, ShouldLabelOnMap);

                if (showTailHeadwindLabels &&
                    routeWind != null && routeWind.Segments != null && routeWind.Segments.Count > 0 &&
                    route != null && route.Count >= 2)
                {
                    DrawTailHeadwindOnRoute(g, route, routeWind, xFromLon, yFromLat);
                }

                g.ResetClip();
                using (var border = new Pen(Color.Gray, 1))
                    g.DrawRectangle(border, mapRect);

                if (route != null && route.Count >= 2)
                    DrawRightRouteBoxDynamic(g, mapRect, route);

                DrawLeftBottomInfoBoxCentered(g, mapRect, metadata);
            }

            return bmp;
        }

        private static double Clip(double v, double min, double max) => Math.Min(Math.Max(v, min), max);

        // ===== selective map labels (origin/dest/TOC/TOD only) =====
        private bool ShouldLabelOnMap(int index, Waypoint wp, int totalCount)
        {
            if (wp == null) return false;
            if (index == 0 || index == totalCount - 1) return true;

            var name = (wp.Name ?? "").Trim();
            if (name.Length == 0) return false;

            if (name.IndexOf("TOC", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("TOD", StringComparison.OrdinalIgnoreCase) >= 0) return true;

            return false;
        }

        private bool IsSpecialPoint(int index, Waypoint wp, int totalCount) => ShouldLabelOnMap(index, wp, totalCount);

        // ===== RIGHT BOX (dynamic height) =====
        private void DrawRightRouteBoxDynamic(Graphics g, Rectangle mapRect, IReadOnlyList<Waypoint> route)
        {
            using (var bg = new SolidBrush(Color.FromArgb(245, 255, 255, 255)))
            using (var border = new Pen(Color.Black, 2))
            using (var titleFont = new Font("Segoe UI", 11, FontStyle.Bold))
            using (var font = new Font("Segoe UI", 8))
            using (var fontBold = new Font("Segoe UI", 8, FontStyle.Bold))
            using (var dotPen = new Pen(Color.Navy, 2))
            using (var dotFill = new SolidBrush(Color.White))
            using (var textBrush = new SolidBrush(Color.Black))
            {
                int topPad = 10;
                int leftPad = 12;
                int lineH = 14;
                int distLineH = 16;
                int titleBlockH = 24;
                int bottomBlockH = 30;

                int desiredPairs = Math.Max(0, route.Count - 1);
                int contentH = topPad + titleBlockH + desiredPairs * (lineH + distLineH) + bottomBlockH + 10;

                int maxH = mapRect.Height - RightBoxPadding * 2;
                int h = Math.Min(contentH, maxH);

                int w = RightBoxWidth;
                int x = mapRect.Right - RightBoxPadding - w;
                int y = mapRect.Top + RightBoxPadding;

                var box = new Rectangle(x, y, w, h);

                g.FillRectangle(bg, box);
                g.DrawRectangle(border, box);

                int cx = box.Left + leftPad;
                int cy = box.Top + topPad;

                string title = (route[0].Name ?? "ROUTE").Trim();
                g.FillEllipse(dotFill, box.Left + 10, cy, 12, 12);
                g.DrawEllipse(dotPen, box.Left + 10, cy, 12, 12);
                g.DrawString(title, titleFont, textBrush, box.Left + 30, cy - 2);

                cy += titleBlockH;

                int bottomReserve = bottomBlockH + 10;
                int maxY = box.Bottom - bottomReserve;

                for (int i = 1; i < route.Count; i++)
                {
                    if (cy + lineH + distLineH > maxY) break;

                    bool special = IsSpecialPoint(i, route[i], route.Count);
                    var useFont = special ? fontBold : font;

                    string wpName = (route[i].Name ?? ("WP" + (i + 1))).Trim();

                    g.DrawString(i.ToString(), font, textBrush, cx, cy);
                    g.FillEllipse(Brushes.Navy, cx + 18, cy + 3, 6, 6);
                    g.DrawString(wpName, useFont, textBrush, cx + 30, cy);

                    cy += lineH;

                    double nm = Geo.HaversineDistanceM(route[i - 1].Lat, route[i - 1].Lon, route[i].Lat, route[i].Lon) / 1852.0;
                    string distLine = $"{Math.Round(nm):0}NM TO {wpName}";
                    g.DrawString(distLine, font, textBrush, cx + 30, cy);

                    cy += distLineH;
                }

                string last = (route[route.Count - 1].Name ?? "DEST").Trim();
                int by = box.Bottom - 26;
                g.FillEllipse(dotFill, box.Left + 10, by, 12, 12);
                g.DrawEllipse(dotPen, box.Left + 10, by, 12, 12);
                g.DrawString(last, titleFont, textBrush, box.Left + 30, by - 4);
            }
        }

        // ===== LEFT BOTTOM BOX (centered) =====
        private void DrawLeftBottomInfoBoxCentered(Graphics g, Rectangle mapRect, MapMetadata meta)
        {
            int w = LeftBottomBoxWidth;
            int h = LeftBottomBoxHeight;

            int x = mapRect.Left + LeftBottomBoxPadding;
            int y = mapRect.Bottom - LeftBottomBoxPadding - h;

            var box = new Rectangle(x, y, w, h);

            using (var bg = new SolidBrush(Color.FromArgb(245, 255, 255, 255)))
            using (var border = new Pen(Color.Black, 2))
            using (var fTitle = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var fFL = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var f = new Font("Segoe UI", 8))
            using (var text = new SolidBrush(Color.Black))
            using (var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
            {
                g.FillRectangle(bg, box);
                g.DrawRectangle(border, box);

                int cy = box.Top + 10;

                string title = meta?.TitleLeft ?? "WIND/TEMPERATURE";
                g.DrawString(title, fTitle, text, new RectangleF(box.Left + 6, cy, box.Width - 12, 20), sfCenter);
                cy += 26;

                string flLine = ExtractShortFl(meta?.TitleRight);
                if (!string.IsNullOrWhiteSpace(flLine))
                {
                    g.DrawString(flLine, fFL, text, new RectangleF(box.Left + 6, cy, box.Width - 12, 20), sfCenter);
                    cy += 22;
                }

                g.DrawString("PROGNOSTIC CHART", f, text, new RectangleF(box.Left + 6, cy, box.Width - 12, 16), sfCenter);
                cy += 16;

                if (!string.IsNullOrWhiteSpace(meta?.ExtraLine))
                {
                    g.DrawString(meta.ExtraLine, f, text, new RectangleF(box.Left + 6, cy, box.Width - 12, 16), sfCenter);
                    cy += 16;
                }

                if (meta?.ValidTimeUtc != null)
                {
                    g.DrawString("VALID " + meta.ValidTimeUtc.Value.ToString("HHmm'Z' dd MMM yyyy").ToUpperInvariant(),
                        f, text, new RectangleF(box.Left + 6, cy, box.Width - 12, 16), sfCenter);
                    cy += 16;
                }

                if (meta?.RunTimeUtc != null)
                {
                    g.DrawString("BASED ON " + meta.RunTimeUtc.Value.ToString("HHmm'Z' dd MMM yyyy").ToUpperInvariant(),
                        f, text, new RectangleF(box.Left + 6, cy, box.Width - 12, 16), sfCenter);
                    cy += 16;
                }

                g.DrawString("Processed By AEROTECH", f, text,
                    new RectangleF(box.Left + 6, box.Bottom - 20, box.Width - 12, 16), sfCenter);
            }
        }

        private static string ExtractShortFl(string titleRight)
        {
            if (string.IsNullOrWhiteSpace(titleRight)) return "";
            var t = titleRight.Trim();

            int idx = t.IndexOf("FL", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                int j = idx + 2;
                string digits = "";
                while (j < t.Length && char.IsDigit(t[j])) { digits += t[j]; j++; }
                if (!string.IsNullOrEmpty(digits)) return "FL " + digits;
            }
            return t;
        }

        // ===== Tail/Headwind labels (H12 / T18, no boxes) =====
        private void DrawTailHeadwindOnRoute(
            Graphics g,
            IReadOnlyList<Waypoint> route,
            RouteWindSummary summary,
            Func<double, float> xLon,
            Func<double, float> yLat)
        {
            const double MinLegNmForLabel = 20.0;

            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var tailBrush = new SolidBrush(Color.FromArgb(200, 60, 60, 60)))
            using (var headBrush = new SolidBrush(Color.FromArgb(220, 200, 40, 40)))
            using (var haloPen = new Pen(Color.FromArgb(220, 255, 255, 255), 3f))
            {
                var placed = new List<RectangleF>();
                int segCount = Math.Min(summary.Segments.Count, route.Count - 1);

                for (int i = 0; i < segCount; i++)
                {
                    var a = route[i];
                    var b = route[i + 1];

                    if (summary.Segments[i].DistanceNm < MinLegNmForLabel)
                        continue;

                    double tw = summary.Segments[i].TailwindKtAvg;

                    string label;
                    Brush brush;

                    if (tw < 0) { label = "H" + Math.Round(-tw); brush = headBrush; }
                    else { label = "T" + Math.Round(tw); brush = tailBrush; }

                    double midLat = (a.Lat + b.Lat) * 0.5;
                    double midLon = (a.Lon + b.Lon) * 0.5;

                    float mx = xLon(midLon);
                    float my = yLat(midLat);

                    float ax = xLon(a.Lon), ay = yLat(a.Lat);
                    float bx = xLon(b.Lon), by = yLat(b.Lat);
                    float dx = bx - ax, dy = by - ay;

                    float len = (float)Math.Sqrt(dx * dx + dy * dy);
                    float ox = 0, oy = 0;
                    if (len > 0.001f) { ox = -dy / len; oy = dx / len; }

                    var candidates = new[]
                    {
                        new PointF(mx + ox * 12, my + oy * 12),
                        new PointF(mx - ox * 12, my - oy * 12),
                        new PointF(mx + ox * 20, my + oy * 20),
                        new PointF(mx - ox * 20, my - oy * 20),
                        new PointF(mx, my)
                    };

                    SizeF sz = g.MeasureString(label, font);
                    RectangleF chosen = RectangleF.Empty;
                    PointF chosenPoint = PointF.Empty;

                    foreach (var p in candidates)
                    {
                        var r = new RectangleF(p.X - sz.Width / 2, p.Y - sz.Height / 2, sz.Width, sz.Height);
                        if (!IntersectsAny(r, placed)) { chosen = r; chosenPoint = p; break; }
                    }

                    if (chosen == RectangleF.Empty)
                    {
                        chosenPoint = candidates[0];
                        chosen = new RectangleF(chosenPoint.X - sz.Width / 2, chosenPoint.Y - sz.Height / 2, sz.Width, sz.Height);
                    }

                    DrawTextWithHalo(g, label, font, brush, haloPen,
                        chosenPoint.X - sz.Width / 2,
                        chosenPoint.Y - sz.Height / 2);

                    placed.Add(chosen);
                }
            }
        }

        private void DrawTextWithHalo(Graphics g, string text, Font font, Brush fillBrush, Pen haloPen, float x, float y)
        {
            using (var path = new GraphicsPath())
            {
                path.AddString(
                    text,
                    font.FontFamily,
                    (int)font.Style,
                    g.DpiY * font.Size / 72f,
                    new PointF(x, y),
                    StringFormat.GenericDefault);

                g.DrawPath(haloPen, path);
                g.FillPath(fillBrush, path);
            }
        }

        // ===== Route + waypoints (only special point labels) =====
        private void DrawRouteAndWaypoints(
            Graphics g,
            IReadOnlyList<Waypoint> route,
            Func<double, float> xLon,
            Func<double, float> yLat,
            Func<int, Waypoint, int, bool> labelFilter)
        {
            using (var routePen = new Pen(Color.DarkBlue, 2))
            using (var wpPen = new Pen(Color.DarkBlue, 2))
            using (var wpFill = new SolidBrush(Color.White))
            using (var labelFont = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var labelBg = new SolidBrush(Color.FromArgb(210, 255, 255, 255)))
            using (var labelBrush = new SolidBrush(Color.Black))
            {
                var pts = new List<PointF>(route.Count);
                for (int i = 0; i < route.Count; i++)
                    pts.Add(new PointF(xLon(route[i].Lon), yLat(route[i].Lat)));

                if (pts.Count >= 2)
                    g.DrawLines(routePen, pts.ToArray());

                var placed = new List<RectangleF>();

                for (int i = 0; i < route.Count; i++)
                {
                    var wp = route[i];
                    var p = pts[i];

                    float r = (i == 0 || i == route.Count - 1) ? 6f : 4f;
                    g.FillEllipse(wpFill, p.X - r, p.Y - r, 2 * r, 2 * r);
                    g.DrawEllipse(wpPen, p.X - r, p.Y - r, 2 * r, 2 * r);

                    if (labelFilter == null || !labelFilter(i, wp, route.Count))
                        continue;

                    string text = string.IsNullOrWhiteSpace(wp.Name) ? $"WP{i + 1}" : wp.Name.Trim();
                    SizeF sz = g.MeasureString(text, labelFont);

                    var candidates = new[]
                    {
                        new PointF(8, -18),
                        new PointF(8, 6),
                        new PointF(-8, -18),
                        new PointF(-8, 6),
                        new PointF(0, -26),
                        new PointF(0, 10)
                    };

                    RectangleF chosen = RectangleF.Empty;

                    foreach (var off in candidates)
                    {
                        float x = p.X + off.X + (off.X < 0 ? -sz.Width : 0);
                        float y = p.Y + off.Y;
                        var rect = new RectangleF(x - 3, y - 2, sz.Width + 6, sz.Height + 4);

                        if (!IntersectsAny(rect, placed)) { chosen = rect; break; }
                    }

                    if (chosen == RectangleF.Empty)
                        chosen = new RectangleF(p.X + 8 - 3, p.Y - 18 - 2, sz.Width + 6, sz.Height + 4);

                    g.FillRectangle(labelBg, chosen);
                    g.DrawRectangle(Pens.Gray, chosen.X, chosen.Y, chosen.Width, chosen.Height);
                    g.DrawString(text, labelFont, labelBrush, chosen.X + 3, chosen.Y + 2);

                    placed.Add(chosen);
                }
            }
        }

        private static bool IntersectsAny(RectangleF r, List<RectangleF> rects)
        {
            for (int i = 0; i < rects.Count; i++)
                if (r.IntersectsWith(rects[i]))
                    return true;
            return false;
        }

        // ===== Wind barb =====
        private void DrawWindBarb(Graphics g, Pen pen, float x, float y, double windDirFromDeg, double speedKt)
        {
            if (speedKt < 2.0)
            {
                float r = 4f;
                g.DrawEllipse(pen, x - r, y - r, 2 * r, 2 * r);
                return;
            }

            int spd = (int)(Math.Round(speedKt / 5.0) * 5.0);
            int n50 = spd / 50; spd %= 50;
            int n10 = spd / 10; spd %= 10;
            int n5 = spd / 5;

            float staffLen = 26f;
            float featherLen = 12f;
            float featherGap = 3.8f;
            float headInset = 2f;

            double dirRad = (windDirFromDeg + 180.0) * Math.PI / 180.0;
            float ux = (float)Math.Sin(dirRad);
            float uy = (float)-Math.Cos(dirRad);

            var p0 = new PointF(x, y);
            var p1 = new PointF(x + ux * staffLen, y + uy * staffLen);
            g.DrawLine(pen, p0, p1);

            float px = -uy;
            float py = ux;

            float pos = staffLen - headInset;
            Func<float, PointF> onStaff = d => new PointF(p0.X + ux * d, p0.Y + uy * d);

            for (int i = 0; i < n50; i++)
            {
                var a = onStaff(pos);
                var b = onStaff(pos - 7f);
                var c = new PointF(a.X + px * featherLen, a.Y + py * featherLen);

                g.FillPolygon(Brushes.Black, new[] { a, c, b });
                g.DrawPolygon(pen, new[] { a, c, b });

                pos -= (7f + featherGap);
            }

            for (int i = 0; i < n10; i++)
            {
                var a = onStaff(pos);
                var c = new PointF(a.X + px * featherLen, a.Y + py * featherLen);
                g.DrawLine(pen, a, c);
                pos -= featherGap;
            }

            if (n5 > 0)
            {
                var a = onStaff(pos);
                var c = new PointF(a.X + px * (featherLen * 0.5f), a.Y + py * (featherLen * 0.5f));
                g.DrawLine(pen, a, c);
            }
        }
    }
}


 
namespace AeroTechApiWeather.Services
{
    public class WindTempMapService
    {
        private readonly GfsFileLocator _locator;
        private readonly WindTempMapRenderer _renderer;
        private readonly string _outputRoot;

        // controlled from API
        public bool ShowTailHeadwindLabels { get; set; } = true;

        // NEW: basemap toggle + paths
        public bool EnableBasemap { get; set; } = false;

        public string BasemapLandShp { get; set; }
        public string BasemapOceanShp { get; set; }
        public string BasemapLakesShp { get; set; }
        public string BasemapBordersShp { get; set; }

        public WindTempMapService(string gfsRoot, string outputRoot)
        {
            if (string.IsNullOrWhiteSpace(gfsRoot))
                throw new ArgumentNullException(nameof(gfsRoot));
            if (string.IsNullOrWhiteSpace(outputRoot))
                throw new ArgumentNullException(nameof(outputRoot));

            _locator = new GfsFileLocator(gfsRoot);
            _renderer = new WindTempMapRenderer();
            _outputRoot = outputRoot;

            Directory.CreateDirectory(_outputRoot);
        }

        public async Task GenerateRouteMapsAsync(
            FlightPlan fp,
            int[] flightLevels,
            CancellationToken ct)
        {
            if (fp == null) throw new ArgumentNullException(nameof(fp));
            if (fp.Route == null || fp.Route.Count < 2)
                throw new ArgumentException("Flight plan route must have at least 2 waypoints.", nameof(fp));
            if (flightLevels == null || flightLevels.Length == 0)
                throw new ArgumentException("flightLevels is empty", nameof(flightLevels));

            var uniqueLevels = flightLevels.Distinct().OrderBy(fl => fl).ToArray();

            var gfsInfo = _locator.GetFileForFlight(fp.DepartureTimeUtc);

            string gribPath = await HttpDownloadHelper.DownloadFileAsync(
                gfsInfo.RemoteUrl,
                gfsInfo.LocalPath,
                ct);

            var bounds = RouteBounds.GetBoundingBox(fp.Route, marginDeg: 7.0);

            int[] availableLevels;
            using (var f = new GribFile(gribPath))
                availableLevels = GfsLevels.GetAvailableIsobaricLevelsHpa(f);

            if (availableLevels.Length == 0)
                throw new InvalidOperationException("No isobaricInhPa levels found in GRIB: " + gribPath);

            // Basemap renderer (optional)
            SimpleBasemapRenderer basemapRenderer = null;
            if (EnableBasemap)
            {
                if (string.IsNullOrWhiteSpace(BasemapLandShp) ||
                    string.IsNullOrWhiteSpace(BasemapOceanShp) ||
                    string.IsNullOrWhiteSpace(BasemapLakesShp) ||
                    string.IsNullOrWhiteSpace(BasemapBordersShp))
                {
                    throw new InvalidOperationException("Basemap is enabled but one or more shapefile paths are not set.");
                }

                basemapRenderer = new SimpleBasemapRenderer(
                    BasemapLandShp,
                    BasemapOceanShp,
                    BasemapLakesShp,
                    BasemapBordersShp);

                // load once (helps performance)
                basemapRenderer.EnsureLoaded();
            }

            foreach (int fl in uniqueLevels)
            {
                ct.ThrowIfCancellationRequested();

                int targetHpa = FlightLevelConverter.FlightLevelToHpa(fl);
                int levelHpa = GfsLevels.SnapToNearest(targetHpa, availableLevels);

                Console.WriteLine($"[Wx] FL{fl} -> target {targetHpa} hPa -> snapped {levelHpa} hPa");

                GfsWindField windField;
                using (var fileWind = new GribFile(gribPath))
                    windField = GfsFieldFactory.CreateWindField(fileWind, levelHpa);

                GfsScalarField tempField;
                using (var fileTemp = new GribFile(gribPath))
                    tempField = GfsFieldFactory.CreateTemperatureField(fileTemp, levelHpa);

                var windSummary = RouteWindCalculator.Compute(
                    fp.Route,
                    windField,
                    tempField,
                    samplingPerLeg: 7);

                var fl_title = "def";
                if (fl > fp.CruiseFlightLevel)
                    fl_title = "high";
                else if (fl < fp.CruiseFlightLevel)
                    fl_title = "low";

                string baseName = $"WT_{fp.FlightId}_{fl_title}";
                string csvPath = Path.Combine(_outputRoot, baseName + "_RouteWind.csv");
                string jsonPath = Path.Combine(_outputRoot, baseName + "_RouteWind.json");

                RouteWindTableExporter.PrintToConsole(windSummary);
                RouteWindTableExporter.SaveAsCsv(windSummary, csvPath);
                RouteWindTableExporter.SaveAsJson(windSummary, jsonPath);

                var meta = new MapMetadata
                {
                    TitleLeft = "WIND/TEMPERATURE",
                    TitleRight = $"FL{fl} (target {targetHpa} -> {levelHpa} hPa)",
                    RunTimeUtc = gfsInfo.RunTimeUtc,
                    ForecastHour = gfsInfo.ForecastHour,
                    ValidTimeUtc = gfsInfo.RunTimeUtc.AddHours(gfsInfo.ForecastHour),
                    ExtraLine =fp.Title //fp.FlightId
                };

                // Output image size (you can API-control later)
                int outW = 1400;
                int outH = 970;

                Bitmap basemap = null;
                try
                {
                    if (EnableBasemap && basemapRenderer != null)
                    {
                        // basemap same size as output
                        basemap = basemapRenderer.Render(
                            bounds.minLat, bounds.maxLat,
                            bounds.minLon, bounds.maxLon,
                            outW - 12, outH - 12); // mapRect size: subtract padding*2
                    }

                    using (basemap)
                    using (Bitmap bmp = _renderer.Render(
                        windField,
                        tempField,
                        bounds.minLat, bounds.maxLat,
                        bounds.minLon, bounds.maxLon,
                        width: outW,
                        height: outH,
                        gridStepDeg: 1.0,
                        route: fp.Route,
                        metadata: meta,
                        routeWind: windSummary,
                        showTailHeadwindLabels: this.ShowTailHeadwindLabels,
                        basemap: basemap))
                    {
                        string outPng = Path.Combine(_outputRoot, baseName + ".png");
                        bmp.Save(outPng, System.Drawing.Imaging.ImageFormat.Png);
                        Console.WriteLine("[Wx] Saved map: " + outPng);
                    }
                }
                finally
                {
                    basemap?.Dispose();
                }
            }


            //////CROSS SECTION////////////
             


            ////////////////////////////
        }
    }
}


namespace AeroTechApiWeather.Rendering
{
    public class MapMetadata
    {
        public string TitleLeft { get; set; }   // e.g. "WIND / TEMP"
        public string TitleRight { get; set; }  // e.g. "FL350 (target 262 -> 250 hPa)"
        public DateTime? RunTimeUtc { get; set; }
        public int? ForecastHour { get; set; }
        public DateTime? ValidTimeUtc { get; set; }
        public string ExtraLine { get; set; }   // e.g. FlightId
    }
}



namespace AeroTechApiWeather.Navigation
{
    public static class Geo
    {
        private const double EarthRadiusM = 6371000.0;

        public static double DegToRad(double deg) => deg * Math.PI / 180.0;
        public static double RadToDeg(double rad) => rad * 180.0 / Math.PI;

        public static double HaversineDistanceM(double lat1, double lon1, double lat2, double lon2)
        {
            double φ1 = DegToRad(lat1);
            double φ2 = DegToRad(lat2);
            double dφ = DegToRad(lat2 - lat1);
            double dλ = DegToRad(lon2 - lon1);

            double a = Math.Sin(dφ / 2) * Math.Sin(dφ / 2) +
                       Math.Cos(φ1) * Math.Cos(φ2) * Math.Sin(dλ / 2) * Math.Sin(dλ / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusM * c;
        }

        public static double InitialBearingDeg(double lat1, double lon1, double lat2, double lon2)
        {
            double φ1 = DegToRad(lat1);
            double φ2 = DegToRad(lat2);
            double λ1 = DegToRad(lon1);
            double λ2 = DegToRad(lon2);

            double y = Math.Sin(λ2 - λ1) * Math.Cos(φ2);
            double x = Math.Cos(φ1) * Math.Sin(φ2) -
                       Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(λ2 - λ1);

            double θ = Math.Atan2(y, x);
            double brng = (RadToDeg(θ) + 360.0) % 360.0;
            return brng;
        }

        // For short legs this is OK; if you want true great-circle interpolation later we can upgrade.
        public static (double lat, double lon) LerpLatLon(double lat1, double lon1, double lat2, double lon2, double t)
        {
            return (lat1 + (lat2 - lat1) * t, lon1 + (lon2 - lon1) * t);
        }
    }
}






namespace AeroTechApiWeather.Navigation
{
    public class SegmentWindResult
    {
        public string FromName { get; set; }
        public string ToName { get; set; }

        public double DistanceNm { get; set; }
        public double TrackDeg { get; set; }

        // Positive = tailwind, Negative = headwind
        public double TailwindKtAvg { get; set; }

        // Positive = wind to the right of track, Negative = to the left
        public double CrosswindKtAvg { get; set; }

        public double WindSpeedKtAvg { get; set; }
        public double TempCAvg { get; set; } // optional
    }

    public class RouteWindSummary
    {
        public double TotalDistanceNm { get; set; }
        public double TailwindKtDistanceWeighted { get; set; }
        public double CrosswindKtDistanceWeighted { get; set; }
        public List<SegmentWindResult> Segments { get; set; } = new List<SegmentWindResult>();
    }

    public static class RouteWindCalculator
    {
        private const double MsToKt = 1.9438444924406;
        private const double MToNm = 1.0 / 1852.0;

        public static RouteWindSummary Compute(
            IReadOnlyList<Waypoint> route,
            GfsWindField windField,
            GfsScalarField tempField,
            int samplingPerLeg = 7)
        {
            if (route == null || route.Count < 2)
                throw new ArgumentException("Route must have at least 2 waypoints.", nameof(route));
            if (samplingPerLeg < 1) samplingPerLeg = 1;

            var summary = new RouteWindSummary();

            double totalNm = 0.0;
            double sumTailNm = 0.0;
            double sumCrossNm = 0.0;

            for (int i = 0; i < route.Count - 1; i++)
            {
                var a = route[i];
                var b = route[i + 1];

                double distM = Geo.HaversineDistanceM(a.Lat, a.Lon, b.Lat, b.Lon);
                double distNm = distM * MToNm;
                if (distNm <= 0.0001) continue;

                double trackDeg = Geo.InitialBearingDeg(a.Lat, a.Lon, b.Lat, b.Lon);
                double trackRad = Geo.DegToRad(trackDeg);

                // unit along-track in (east, north)
                double e = Math.Sin(trackRad);
                double n = Math.Cos(trackRad);

                // unit cross-track to the right
                double er = Math.Sin(trackRad + Math.PI / 2.0);
                double nr = Math.Cos(trackRad + Math.PI / 2.0);

                double sumAlongMs = 0.0;
                double sumCrossMs = 0.0;
                double sumWindSpdKt = 0.0;
                double sumTempC = 0.0;

                int samples = samplingPerLeg;

                for (int s = 0; s < samples; s++)
                {
                    double t = (samples == 1) ? 0.5 : (s + 0.5) / samples;
                    var p = Geo.LerpLatLon(a.Lat, a.Lon, b.Lat, b.Lon, t);

                    var w = windField.Sample(p.lat, p.lon);
                    double u = w.U; // m/s east
                    double v = w.V; // m/s north

                    double alongMs = (u * e) + (v * n);
                    double crossMs = (u * er) + (v * nr);

                    sumAlongMs += alongMs;
                    sumCrossMs += crossMs;
                    sumWindSpdKt += w.SpeedKt;

                    if (tempField != null)
                        sumTempC += (tempField.Sample(p.lat, p.lon) - 273.15);
                }

                double avgAlongKt = (sumAlongMs / samples) * MsToKt;
                double avgCrossKt = (sumCrossMs / samples) * MsToKt;
                double avgWindSpdKt = sumWindSpdKt / samples;
                double avgTempC = (tempField != null) ? (sumTempC / samples) : double.NaN;

                summary.Segments.Add(new SegmentWindResult
                {
                    FromName = string.IsNullOrWhiteSpace(a.Name) ? $"WP{i + 1}" : a.Name,
                    ToName = string.IsNullOrWhiteSpace(b.Name) ? $"WP{i + 2}" : b.Name,
                    DistanceNm = distNm,
                    TrackDeg = trackDeg,
                    TailwindKtAvg = avgAlongKt,
                    CrosswindKtAvg = avgCrossKt,
                    WindSpeedKtAvg = avgWindSpdKt,
                    TempCAvg = avgTempC
                });

                totalNm += distNm;
                sumTailNm += avgAlongKt * distNm;
                sumCrossNm += avgCrossKt * distNm;
            }

            summary.TotalDistanceNm = totalNm;
            summary.TailwindKtDistanceWeighted = totalNm > 0 ? (sumTailNm / totalNm) : 0.0;
            summary.CrosswindKtDistanceWeighted = totalNm > 0 ? (sumCrossNm / totalNm) : 0.0;

            return summary;
        }
    }
}




namespace AeroTechApiWeather.Navigation
{
    public static class RouteWindTableExporter
    {
        public static void PrintToConsole(RouteWindSummary summary)
        {
            if (summary == null) throw new ArgumentNullException(nameof(summary));

            Console.WriteLine();
            Console.WriteLine("=== ROUTE WIND SUMMARY ===");
            Console.WriteLine($"Total Distance: {summary.TotalDistanceNm:F1} NM");
            Console.WriteLine($"Avg Tail(+)/Head(-): {summary.TailwindKtDistanceWeighted:F1} kt (distance-weighted)");
            Console.WriteLine($"Avg Cross (+right/-left): {summary.CrosswindKtDistanceWeighted:F1} kt (distance-weighted)");
            Console.WriteLine();

            Console.WriteLine(
                Pad("LEG", 18) +
                Pad("DIST(NM)", 10) +
                Pad("TRK", 6) +
                Pad("TAIL", 8) +
                Pad("CROSS", 8) +
                Pad("WIND", 8) +
                Pad("TEMP", 6));

            Console.WriteLine(new string('-', 18 + 10 + 6 + 8 + 8 + 8 + 6));

            foreach (var s in summary.Segments)
            {
                string leg = $"{s.FromName}-{s.ToName}";
                Console.WriteLine(
                    Pad(leg, 18) +
                    Pad(s.DistanceNm.ToString("F1", CultureInfo.InvariantCulture), 10) +
                    Pad(s.TrackDeg.ToString("F0", CultureInfo.InvariantCulture), 6) +
                    Pad(FormatSigned(s.TailwindKtAvg), 8) +
                    Pad(FormatSigned(s.CrosswindKtAvg), 8) +
                    Pad(s.WindSpeedKtAvg.ToString("F0", CultureInfo.InvariantCulture), 8) +
                    Pad(double.IsNaN(s.TempCAvg) ? "-" : s.TempCAvg.ToString("F0", CultureInfo.InvariantCulture), 6));
            }

            Console.WriteLine();
        }

        public static void SaveAsCsv(RouteWindSummary summary, string csvPath)
        {
            if (summary == null) throw new ArgumentNullException(nameof(summary));
            if (string.IsNullOrWhiteSpace(csvPath)) throw new ArgumentNullException(nameof(csvPath));

            var dir = Path.GetDirectoryName(csvPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var sb = new StringBuilder();
            sb.AppendLine("From,To,DistanceNm,TrackDeg,TailwindKtAvg,CrosswindKtAvg,WindSpeedKtAvg,TempCAvg");

            foreach (var s in summary.Segments)
            {
                sb.Append(s.FromName).Append(',')
                  .Append(s.ToName).Append(',')
                  .Append(s.DistanceNm.ToString("F3", CultureInfo.InvariantCulture)).Append(',')
                  .Append(s.TrackDeg.ToString("F1", CultureInfo.InvariantCulture)).Append(',')
                  .Append(s.TailwindKtAvg.ToString("F2", CultureInfo.InvariantCulture)).Append(',')
                  .Append(s.CrosswindKtAvg.ToString("F2", CultureInfo.InvariantCulture)).Append(',')
                  .Append(s.WindSpeedKtAvg.ToString("F2", CultureInfo.InvariantCulture)).Append(',')
                  .Append(double.IsNaN(s.TempCAvg) ? "" : s.TempCAvg.ToString("F2", CultureInfo.InvariantCulture))
                  .AppendLine();
            }

            File.WriteAllText(csvPath, sb.ToString(), Encoding.UTF8);
        }

        public static void SaveAsJson(RouteWindSummary summary, string jsonPath)
        {
            if (summary == null) throw new ArgumentNullException(nameof(summary));
            if (string.IsNullOrWhiteSpace(jsonPath)) throw new ArgumentNullException(nameof(jsonPath));

            var dir = Path.GetDirectoryName(jsonPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Culture = System.Globalization.CultureInfo.InvariantCulture,
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(summary, settings);
            File.WriteAllText(jsonPath, json, Encoding.UTF8);
        }

        private static string Pad(string s, int w)
        {
            s = s ?? "";
            if (s.Length >= w) return s.Substring(0, w);
            return s.PadRight(w);
        }

        private static string FormatSigned(double v)
        {
            // +12.3 or -8.0
            return (v >= 0 ? "+" : "") + v.ToString("F1", CultureInfo.InvariantCulture);
        }
    }
}




 

namespace AeroTechApiWeather.Mapping
{
    /// <summary>
    /// Renders a clean aviation-style basemap (land/water/borders/lakes) similar to the sample image.
    /// Uses Natural Earth shapefiles (10m or 50m).
    /// </summary>
    public class SimpleBasemapRenderer
    {
        private readonly string _landShp;
        private readonly string _oceanShp;
        private readonly string _lakesShp;
        private readonly string _bordersShp;

        // loaded once, reused
        private List<Geometry> _land;
        private List<Geometry> _ocean;
        private List<Geometry> _lakes;
        private List<Geometry> _borders;

        public SimpleBasemapRenderer(string landShp, string oceanShp, string lakesShp, string bordersShp)
        {
            _landShp = landShp ?? throw new ArgumentNullException(nameof(landShp));
            _oceanShp = oceanShp ?? throw new ArgumentNullException(nameof(oceanShp));
            _lakesShp = lakesShp ?? throw new ArgumentNullException(nameof(lakesShp));
            _bordersShp = bordersShp ?? throw new ArgumentNullException(nameof(bordersShp));
        }

        public void EnsureLoaded()
        {
            if (_land != null) return;

            _land = ReadAll(_landShp);
            _ocean = ReadAll(_oceanShp);
            _lakes = ReadAll(_lakesShp);
            _borders = ReadAll(_bordersShp);
        }

        public Bitmap Render(double minLat, double maxLat, double minLon, double maxLon, int width, int height)
        {
            EnsureLoaded();

            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Sample-like colors
                var water = Color.FromArgb(195, 225, 240);
                var land = Color.FromArgb(246, 242, 220);

                using (var oceanBrush = new SolidBrush(water))
                using (var landBrush = new SolidBrush(land))
                using (var lakeBrush = new SolidBrush(water))
                using (var coastPen = new Pen(Color.FromArgb(130, 140, 140, 140), 1f))
                using (var borderPen = new Pen(Color.FromArgb(120, 120, 120, 120), 1f))
                {
                    // Base: ocean
                    g.Clear(water);

                    // Land fill
                    DrawGeometryFill(g, _land, minLat, maxLat, minLon, maxLon, width, height, landBrush);

                    // Lakes fill (water)
                    DrawGeometryFill(g, _lakes, minLat, maxLat, minLon, maxLon, width, height, lakeBrush);

                    // Coastline outline (land boundaries)
                    DrawGeometryStroke(g, _land, minLat, maxLat, minLon, maxLon, width, height, coastPen);

                    // Borders
                    DrawGeometryStroke(g, _borders, minLat, maxLat, minLon, maxLon, width, height, borderPen);
                }
            }

            return bmp;
        }

        private static List<Geometry> ReadAll(string shpPath)
        {
            var list = new List<Geometry>();
            using (var reader = new ShapefileDataReader(shpPath, GeometryFactory.Default))
            {
                while (reader.Read())
                {
                    var geom = reader.Geometry;
                    if (geom != null && !geom.IsEmpty)
                        list.Add(geom);
                }
            }
            return list;
        }

        private static double Clip(double v, double min, double max)
            => Math.Min(Math.Max(v, min), max);

        // Mercator-like mapping (matches aviation-style charts well)
        private static PointF Project(double lat, double lon,
            double minLat, double maxLat, double minLon, double maxLon, int w, int h)
        {
            // lon linear
            double x = (lon - minLon) / (maxLon - minLon);

            // mercator y
            lat = Clip(lat, -85.05112878, 85.05112878);

            double yMerc(double la)
            {
                double rad = la * Math.PI / 180.0;
                double s = Math.Sin(rad);
                return 0.5 - Math.Log((1 + s) / (1 - s)) / (4 * Math.PI);
            }

            double y0 = yMerc(maxLat);
            double y1 = yMerc(minLat);
            double y = (yMerc(lat) - y0) / (y1 - y0);

            return new PointF((float)(x * (w - 1)), (float)(y * (h - 1)));
        }

        private static void DrawGeometryFill(Graphics g, List<Geometry> geoms,
            double minLat, double maxLat, double minLon, double maxLon, int w, int h, Brush brush)
        {
            var viewEnv = new Envelope(minLon, maxLon, minLat, maxLat);

            foreach (var geom in geoms)
            {
                if (!geom.EnvelopeInternal.Intersects(viewEnv))
                    continue;

                using (var path = ToPath(geom, minLat, maxLat, minLon, maxLon, w, h))
                {
                    if (path != null)
                        g.FillPath(brush, path);
                }
            }
        }

        private static void DrawGeometryStroke(Graphics g, List<Geometry> geoms,
            double minLat, double maxLat, double minLon, double maxLon, int w, int h, Pen pen)
        {
            var viewEnv = new Envelope(minLon, maxLon, minLat, maxLat);

            foreach (var geom in geoms)
            {
                if (!geom.EnvelopeInternal.Intersects(viewEnv))
                    continue;

                using (var path = ToPath(geom, minLat, maxLat, minLon, maxLon, w, h))
                {
                    if (path != null)
                        g.DrawPath(pen, path);
                }
            }
        }

        private static GraphicsPath ToPath(Geometry geom,
            double minLat, double maxLat, double minLon, double maxLon, int w, int h)
        {
            if (geom == null || geom.IsEmpty) return null;

            var path = new GraphicsPath(FillMode.Winding);

            void AddRing(Coordinate[] ring)
            {
                if (ring == null || ring.Length < 2) return;

                var pts = new PointF[ring.Length];
                for (int i = 0; i < ring.Length; i++)
                    pts[i] = Project(ring[i].Y, ring[i].X, minLat, maxLat, minLon, maxLon, w, h);

                path.AddLines(pts);
                path.CloseFigure();
            }

            void AddLine(Coordinate[] coords)
            {
                if (coords == null || coords.Length < 2) return;

                var pts = new PointF[coords.Length];
                for (int i = 0; i < coords.Length; i++)
                    pts[i] = Project(coords[i].Y, coords[i].X, minLat, maxLat, minLon, maxLon, w, h);

                path.AddLines(pts);
            }

            switch (geom)
            {
                case Polygon p:
                    AddRing(p.ExteriorRing.Coordinates);
                    for (int i = 0; i < p.NumInteriorRings; i++)
                        AddRing(p.GetInteriorRingN(i).Coordinates);
                    break;

                case MultiPolygon mp:
                    for (int i = 0; i < mp.NumGeometries; i++)
                    {
                        var poly = (Polygon)mp.GetGeometryN(i);
                        AddRing(poly.ExteriorRing.Coordinates);
                        for (int j = 0; j < poly.NumInteriorRings; j++)
                            AddRing(poly.GetInteriorRingN(j).Coordinates);
                    }
                    break;

                case LineString ls:
                    AddLine(ls.Coordinates);
                    break;

                case MultiLineString mls:
                    for (int i = 0; i < mls.NumGeometries; i++)
                        AddLine(((LineString)mls.GetGeometryN(i)).Coordinates);
                    break;

                default:
                    // try boundary for other types
                    var b = geom.Boundary;
                    if (b != null && !b.IsEmpty)
                        return ToPath(b, minLat, maxLat, minLon, maxLon, w, h);
                    break;
            }

            return path.PointCount > 0 ? path : null;
        }
    }
}






/////////////////////  CROSS SECTION
///

 

 

namespace AeroTechApiWeather.Navigation
{
    /// <summary>
    /// Great-circle geometry helpers (independent from existing Geo class).
    /// </summary>
    public static class GeoGreatCircle
    {
        private const double EarthRadiusM = 6371000.0;
        private const double MPerNm = 1852.0;

        public static double HaversineDistanceNm(double lat1, double lon1, double lat2, double lon2)
            => HaversineDistanceM(lat1, lon1, lat2, lon2) / MPerNm;

        public static double HaversineDistanceM(double lat1, double lon1, double lat2, double lon2)
        {
            double φ1 = DegToRad(lat1);
            double φ2 = DegToRad(lat2);
            double dφ = DegToRad(lat2 - lat1);
            double dλ = DegToRad(lon2 - lon1);

            double a =
                Math.Sin(dφ / 2) * Math.Sin(dφ / 2) +
                Math.Cos(φ1) * Math.Cos(φ2) *
                Math.Sin(dλ / 2) * Math.Sin(dλ / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusM * c;
        }

        public static double InitialBearingDeg(double lat1, double lon1, double lat2, double lon2)
        {
            double φ1 = DegToRad(lat1);
            double φ2 = DegToRad(lat2);
            double λ1 = DegToRad(lon1);
            double λ2 = DegToRad(lon2);

            double y = Math.Sin(λ2 - λ1) * Math.Cos(φ2);
            double x =
                Math.Cos(φ1) * Math.Sin(φ2) -
                Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(λ2 - λ1);

            double θ = Math.Atan2(y, x);
            return (RadToDeg(θ) + 360.0) % 360.0;
        }

        /// <summary>
        /// Intermediate point along great-circle at fraction f [0..1]
        /// </summary>
        public static (double lat, double lon) IntermediatePoint(
            double lat1, double lon1,
            double lat2, double lon2,
            double f)
        {
            f = Math.Max(0, Math.Min(1, f));

            double φ1 = DegToRad(lat1);
            double λ1 = DegToRad(lon1);
            double φ2 = DegToRad(lat2);
            double λ2 = DegToRad(lon2);

            double δ = AngularDistanceRad(φ1, λ1, φ2, λ2);
            if (δ < 1e-12)
                return (lat1, lon1);

            double A = Math.Sin((1 - f) * δ) / Math.Sin(δ);
            double B = Math.Sin(f * δ) / Math.Sin(δ);

            double x = A * Math.Cos(φ1) * Math.Cos(λ1) + B * Math.Cos(φ2) * Math.Cos(λ2);
            double y = A * Math.Cos(φ1) * Math.Sin(λ1) + B * Math.Cos(φ2) * Math.Sin(λ2);
            double z = A * Math.Sin(φ1) + B * Math.Sin(φ2);

            double φi = Math.Atan2(z, Math.Sqrt(x * x + y * y));
            double λi = Math.Atan2(y, x);

            return (RadToDeg(φi), NormalizeLon(RadToDeg(λi)));
        }

        private static double AngularDistanceRad(double φ1, double λ1, double φ2, double λ2)
        {
            double dφ = φ2 - φ1;
            double dλ = λ2 - λ1;

            double a =
                Math.Sin(dφ / 2) * Math.Sin(dφ / 2) +
                Math.Cos(φ1) * Math.Cos(φ2) *
                Math.Sin(dλ / 2) * Math.Sin(dλ / 2);

            return 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double NormalizeLon(double lon)
        {
            lon = (lon + 180.0) % 360.0;
            if (lon < 0) lon += 360.0;
            return lon - 180.0;
        }

        private static double DegToRad(double d) => d * Math.PI / 180.0;
        private static double RadToDeg(double r) => r * 180.0 / Math.PI;
    }
}

 
 
namespace AeroTechApiWeather.Navigation
{
    public static class RouteSampler
    {
        /// <summary>
        /// Column-based sampling:
        /// - Always includes all waypoints.
        /// - For long legs, inserts intermediate points every insertStepNm (NM).
        /// - Intermediate point name like: "27NM TO MAGRI" (remaining distance to next waypoint).
        /// </summary>
        public static List<RouteSamplePoint> SampleRouteColumns(
            IReadOnlyList<Waypoint> route,
            double insertStepNm,
            double minLegNmForInsert)
        {
            if (route == null) throw new ArgumentNullException("route");
            if (route.Count < 2) throw new ArgumentException("Route must have at least 2 waypoints.", "route");
            if (insertStepNm <= 0) throw new ArgumentOutOfRangeException("insertStepNm");
            if (minLegNmForInsert < 0) throw new ArgumentOutOfRangeException("minLegNmForInsert");

            var result = new List<RouteSamplePoint>(route.Count * 2);
            double cumNm = 0;

            // First waypoint always
            result.Add(new RouteSamplePoint
            {
                DistanceNm = 0,
                Lat = route[0].Lat,
                Lon = route[0].Lon,
                TrackDeg = GeoGreatCircle.InitialBearingDeg(route[0].Lat, route[0].Lon, route[1].Lat, route[1].Lon),
                LegIndex = 0,
                IsWaypoint = true,
                WaypointName = route[0].Name
            });

            for (int i = 0; i < route.Count - 1; i++)
            {
                Waypoint a = route[i];
                Waypoint b = route[i + 1];

                double legNm = GeoGreatCircle.HaversineDistanceNm(a.Lat, a.Lon, b.Lat, b.Lon);
                if (legNm < 0.001)
                    continue;

                double bearing = GeoGreatCircle.InitialBearingDeg(a.Lat, a.Lon, b.Lat, b.Lon);

                // Insert intermediate points only if leg is long enough
                if (legNm >= minLegNmForInsert && insertStepNm > 0)
                {
                    // place at insertStepNm, 2*insertStepNm, ... < legNm
                    double dFromLegStart = insertStepNm;

                    while (dFromLegStart < legNm - 1e-6)
                    {
                        double frac = dFromLegStart / legNm;
                        var p = GeoGreatCircle.IntermediatePoint(a.Lat, a.Lon, b.Lat, b.Lon, frac);

                        int remainNm = (int)Math.Round(legNm - dFromLegStart);
                        string label = remainNm.ToString() + "NM TO " + (b.Name ?? "").Trim();

                        result.Add(new RouteSamplePoint
                        {
                            DistanceNm = cumNm + dFromLegStart,
                            Lat = p.lat,
                            Lon = p.lon,
                            TrackDeg = bearing,
                            LegIndex = i,
                            IsWaypoint = false,
                            WaypointName = label
                        });

                        dFromLegStart += insertStepNm;
                    }
                }

                // End of leg waypoint
                cumNm += legNm;

                result.Add(new RouteSamplePoint
                {
                    DistanceNm = cumNm,
                    Lat = b.Lat,
                    Lon = b.Lon,
                    TrackDeg = bearing,
                    LegIndex = i,
                    IsWaypoint = true,
                    WaypointName = b.Name
                });
            }

            return result;
        }
    }
}




namespace AeroTechApiWeather.Navigation
{
    public static class WindMath
    {
        /// <summary>
        /// Signed along-track wind component (kt): +Tail, -Head.
        /// trackDegTrue: direction aircraft goes to (true).
        /// windFromDegTrue: wind direction FROM (true).
        /// </summary>
        public static double HeadTailComponent(double trackDegTrue, double windFromDegTrue, double windSpeedKt)
        {
            double windToDeg = NormalizeDeg(windFromDegTrue + 180.0);
            double delta = SmallestAngleDeg(windToDeg - trackDegTrue);
            return windSpeedKt * Math.Cos(DegToRad(delta));
        }

        public static string FormatHT(double headTailKt)
        {
            // H12 / T18
            if (headTailKt < 0) return "H" + Math.Round(-headTailKt).ToString();
            return "T" + Math.Round(headTailKt).ToString();
        }

        public static double NormalizeDeg(double deg)
        {
            deg %= 360.0;
            if (deg < 0) deg += 360.0;
            return deg;
        }

        public static double SmallestAngleDeg(double deg)
        {
            deg = (deg + 180.0) % 360.0;
            if (deg <= 0) deg += 360.0;
            return deg - 180.0;
        }

        private static double DegToRad(double d) => d * Math.PI / 180.0;
    }
}






 
namespace AeroTechApiWeather.Rendering
{
    public class CrossSectionRenderer
    {
        public Bitmap Render(
            CrossSectionData data,
            int width,
            int height,
            bool showTemp,
            bool showWindBarbs,
            bool showHeadTailTable,
            bool showFlightProfile)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Bitmap bmp = new Bitmap(width, height);

            int headerH = 42;
            int leftAxisW = 70;
            int tableH = showHeadTailTable ? 150 : 0;

            int top = headerH + 8;
            int left = leftAxisW;
            int right = 20;
            int bottom = 28 + tableH;

            Rectangle plotRect = new Rectangle(
                left,
                top,
                width - left - right,
                height - top - bottom);

            Rectangle tableRect = new Rectangle(
                left,
                height - tableH - 18,
                width - left - right,
                tableH);

            double maxDist = data.TotalDistanceNm;
            int minFl = data.FlightLevels.First();
            int maxFl = data.FlightLevels.Last();

            Func<double, float> xFromDist =
                d => plotRect.Left + (float)(d / Math.Max(1e-6, maxDist) * plotRect.Width);

            Func<int, float> yFromFl =
                fl => plotRect.Bottom - (float)((fl - minFl) /
                      Math.Max(1e-6, maxFl - minFl) * plotRect.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.White);

                DrawHeader(g, data);
                DrawAxesAndGrid(g, plotRect, data, xFromDist, yFromFl);
                DrawSamples(g, plotRect, data, xFromDist, yFromFl, showTemp, showWindBarbs);

                if (showFlightProfile)
                    DrawFlightProfile(g, plotRect, data, xFromDist, yFromFl);

                g.DrawRectangle(Pens.Black, plotRect);

                if (showHeadTailTable)
                    DrawHeadTailTable(g, tableRect, data);
            }

            return bmp;
        }

        // ------------------------------------------------------------

        private void DrawHeader(Graphics g, CrossSectionData data)
        {
            using (Font titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
            using (Font small = new Font("Segoe UI", 8))
            {
                g.DrawString(
                    "CROSS SECTION – WIND / TEMPERATURE",
                    titleFont,
                    Brushes.Black,
                    10,
                    6);

                string line = data.FlightId + "   CRUISE FL " + data.CruiseFlightLevel;

                if (data.ValidTimeUtc.HasValue)
                    line += "   VALID " + data.ValidTimeUtc.Value.ToString("HHmm'Z' dd MMM yyyy").ToUpperInvariant();

                if (data.RunTimeUtc.HasValue)
                    line += "   BASED " + data.RunTimeUtc.Value.ToString("HHmm'Z' dd MMM yyyy").ToUpperInvariant();

                g.DrawString(line, small, Brushes.Black, 10, 26);
            }
        }

        // ------------------------------------------------------------

        private void DrawAxesAndGrid(
            Graphics g,
            Rectangle plotRect,
            CrossSectionData data,
            Func<double, float> xFromDist,
            Func<int, float> yFromFl)
        {
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            using (Font axisFont = new Font("Segoe UI", 8))
            {
                foreach (int fl in data.FlightLevels)
                {
                    float y = yFromFl(fl);
                    g.DrawLine(gridPen, plotRect.Left, y, plotRect.Right, y);
                    g.DrawString("FL" + fl, axisFont, Brushes.Black, 6, y - 7);
                }

                for (double d = 0; d <= data.TotalDistanceNm + 0.1; d += 100)
                {
                    float x = xFromDist(d);
                    g.DrawLine(gridPen, x, plotRect.Top, x, plotRect.Bottom);

                    string lab = ((int)Math.Round(d)).ToString();
                    SizeF sz = g.MeasureString(lab, axisFont);
                    g.DrawString(lab, axisFont, Brushes.Black, x - sz.Width / 2, plotRect.Bottom + 4);
                }

                g.DrawString("NM", axisFont, Brushes.Black, plotRect.Right + 4, plotRect.Bottom + 4);

                List<RouteSamplePoint> wps = data.RoutePoints.Where(p => p.IsWaypoint).ToList();
                for (int i = 0; i < wps.Count; i++)
                {
                    RouteSamplePoint wp = wps[i];
                    if (!IsSpecialWaypoint(wp.WaypointName, i, wps.Count))
                        continue;

                    float x = xFromDist(wp.DistanceNm);
                    g.DrawLine(Pens.Black, x, plotRect.Bottom, x, plotRect.Bottom - 6);

                    using (Font f = new Font("Segoe UI", 8, FontStyle.Bold))
                    {
                        SizeF sz = g.MeasureString(wp.WaypointName, f);
                        g.DrawString(wp.WaypointName, f, Brushes.Black,
                            x - sz.Width / 2, plotRect.Bottom + 18);
                    }
                }
            }
        }

        // ------------------------------------------------------------

        private void DrawSamples(
            Graphics g,
            Rectangle plotRect,
            CrossSectionData data,
            Func<double, float> xFromDist,
            Func<int, float> yFromFl,
            bool showTemp,
            bool showWind)
        {
            using (Font tempFont = new Font("Segoe UI", 7))
            using (Pen pen = new Pen(Color.Black, 1))
            {
                foreach (CrossSectionSample s in data.Samples)
                {
                    float x = xFromDist(s.DistanceNm);
                    float y = yFromFl(s.FlightLevel);

                    if (x < plotRect.Left || x > plotRect.Right ||
                        y < plotRect.Top || y > plotRect.Bottom)
                        continue;

                    if (showWind)
                        DrawWindBarb(g, pen, x, y, s.WindDirDeg, s.WindSpeedKt);

                    if (showTemp)
                        g.DrawString(
                            Math.Round(s.TemperatureC).ToString(),
                            tempFont,
                            Brushes.DarkRed,
                            x + 6,
                            y + 6);
                }
            }
        }

        // ------------------------------------------------------------

        private void DrawFlightProfile(
            Graphics g,
            Rectangle plotRect,
            CrossSectionData data,
            Func<double, float> xFromDist,
            Func<int, float> yFromFl)
        {
            float x0 = xFromDist(0);
            float xToc = xFromDist(data.TocDistanceNm);
            float xTod = xFromDist(data.TodDistanceNm);
            float xEnd = xFromDist(data.TotalDistanceNm);

            float y0 = plotRect.Bottom;
            float yCruise = yFromFl(data.CruiseFlightLevel);

            using (Pen halo = new Pen(Color.FromArgb(200, 255, 255, 255), 6))
            using (Pen pen = new Pen(Color.FromArgb(220, 30, 90, 170), 3))
            using (Font f = new Font("Segoe UI", 8, FontStyle.Bold))
            {
                PointF[] pts = new[]
                {
                    new PointF(x0, y0),
                    new PointF(xToc, yCruise),
                    new PointF(xTod, yCruise),
                    new PointF(xEnd, y0)
                };

                g.DrawLines(halo, pts);
                g.DrawLines(pen, pts);

                DrawProfileMarker(g, xToc, yCruise, "TOC", f);
                DrawProfileMarker(g, xTod, yCruise, "TOD", f);
            }
        }

        private void DrawProfileMarker(Graphics g, float x, float y, string text, Font f)
        {
            float r = 4;
            g.FillEllipse(Brushes.White, x - r, y - r, r * 2, r * 2);
            g.DrawEllipse(Pens.Black, x - r, y - r, r * 2, r * 2);

            SizeF sz = g.MeasureString(text, f);
            g.DrawString(text, f, Brushes.Black, x - sz.Width / 2, y - sz.Height - 6);
        }

        // ------------------------------------------------------------

        private void DrawHeadTailTable(Graphics g, Rectangle rect, CrossSectionData data)
        {
            List<RouteSamplePoint> wps = data.RoutePoints.Where(p => p.IsWaypoint).ToList();
            List<CrossSectionSample> cruise =
                data.Samples.Where(s => s.FlightLevel == data.CruiseFlightLevel).ToList();

            Func<double, double> nearestHT = d =>
            {
                CrossSectionSample best = null;
                double bestD = double.MaxValue;

                foreach (CrossSectionSample s in cruise)
                {
                    double dd = Math.Abs(s.DistanceNm - d);
                    if (dd < bestD)
                    {
                        bestD = dd;
                        best = s;
                    }
                }
                return best != null ? best.HeadTailKt : 0;
            };

            using (SolidBrush bg = new SolidBrush(Color.FromArgb(245, 255, 255, 255)))
            using (Pen border = new Pen(Color.Black, 2))
            using (Font fTitle = new Font("Segoe UI", 10, FontStyle.Bold))
            using (Font f = new Font("Segoe UI", 8))
            using (Font fb = new Font("Segoe UI", 8, FontStyle.Bold))
            {
                g.FillRectangle(bg, rect);
                g.DrawRectangle(border, rect);

                g.DrawString("ROUTE WIND (CRUISE)", fTitle, Brushes.Black, rect.Left + 10, rect.Top + 8);

                int y = rect.Top + 38;
                for (int i = 0; i < wps.Count; i++)
                {
                    RouteSamplePoint wp = wps[i];
                    bool bold = IsSpecialWaypoint(wp.WaypointName, i, wps.Count);
                    Font useFont = bold ? fb : f;

                    g.DrawString(wp.WaypointName, useFont, Brushes.Black, rect.Left + 10, y);
                    g.DrawString(((int)Math.Round(wp.DistanceNm)).ToString(), useFont, Brushes.Black, rect.Left + 130, y);
                    g.DrawString(WindMath.FormatHT(nearestHT(wp.DistanceNm)), useFont, Brushes.Black, rect.Left + 200, y);

                    y += 16;
                }

                g.DrawString("Processed By AEROTECH", f, Brushes.Black, rect.Left + 10, rect.Bottom - 18);
            }
        }

        // ------------------------------------------------------------

        private static bool IsSpecialWaypoint(string name, int index, int total)
        {
            if (index == 0 || index == total - 1)
                return true;

            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.IndexOf("TOC", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            if (name.IndexOf("TOD", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }

        // ------------------------------------------------------------

        private void DrawWindBarb(Graphics g, Pen pen, float x, float y, double windFromDeg, double speedKt)
        {
            if (speedKt < 2)
            {
                g.DrawEllipse(pen, x - 3, y - 3, 6, 6);
                return;
            }

            int spd = (int)(Math.Round(speedKt / 5.0) * 5);
            int n50 = spd / 50; spd %= 50;
            int n10 = spd / 10; spd %= 10;
            int n5 = spd / 5;

            float len = 18f;
            double dirRad = (windFromDeg + 180) * Math.PI / 180.0;

            float ux = (float)Math.Sin(dirRad);
            float uy = (float)-Math.Cos(dirRad);

            PointF p0 = new PointF(x, y);
            PointF p1 = new PointF(x + ux * len, y + uy * len);
            g.DrawLine(pen, p0, p1);

            float px = -uy;
            float py = ux;
            float pos = len - 2;

            for (int i = 0; i < n50; i++)
            {
                PointF a = new PointF(p0.X + ux * pos, p0.Y + uy * pos);
                PointF b = new PointF(a.X + px * 9, a.Y + py * 9);
                PointF c = new PointF(p0.X + ux * (pos - 6), p0.Y + uy * (pos - 6));
                g.FillPolygon(Brushes.Black, new[] { a, b, c });
                pos -= 8;
            }

            for (int i = 0; i < n10; i++)
            {
                PointF a = new PointF(p0.X + ux * pos, p0.Y + uy * pos);
                PointF b = new PointF(a.X + px * 9, a.Y + py * 9);
                g.DrawLine(pen, a, b);
                pos -= 5;
            }

            if (n5 > 0)
            {
                PointF a = new PointF(p0.X + ux * pos, p0.Y + uy * pos);
                PointF b = new PointF(a.X + px * 4.5f, a.Y + py * 4.5f);
                g.DrawLine(pen, a, b);
            }
        }
    }
}







 

namespace AeroTechApiWeather.Services
{
    /// <summary>
    /// High-level service: generate Cross-Section PNG with one call (like WindTempMapService).
    /// It loads/downloas GRIB, resolves required levels, samples along route, renders PNG.
    /// </summary>
    public class CrossSectionMapService
    {
        private readonly GfsFileLocator _locator;
        private readonly string _outputRoot;

        public double SampleStepNm { get; set; } = 20.0;

        public int Width { get; set; } = 1400;
        public int Height { get; set; } = 900;

        public bool ShowTemperature { get; set; } = true;
        public bool ShowWindBarbs { get; set; } = true;
        public bool ShowHeadTailTable { get; set; } = true;
        public bool ShowFlightProfile { get; set; } = true;

        public double InsertStepNm { get; set; } = 120.0;       // فاصله نقاط میانی داخل legهای طولانی
public double MinLegNmForInsert { get; set; } = 150.0;

        public CrossSectionMapService(string gfsRoot, string outputRoot)
        {
            if (string.IsNullOrWhiteSpace(gfsRoot))
                throw new ArgumentNullException("gfsRoot");
            if (string.IsNullOrWhiteSpace(outputRoot))
                throw new ArgumentNullException("outputRoot");

            _locator = new GfsFileLocator(gfsRoot);
            _outputRoot = outputRoot;

            Directory.CreateDirectory(_outputRoot);
        }

        public async Task<string> GenerateCrossSectionAsync(
            FlightPlan fp,
            int[] flightLevels,
            CancellationToken ct)
        {
            if (fp == null) throw new ArgumentNullException("fp");
            if (fp.Route == null || fp.Route.Count < 2)
                throw new ArgumentException("FlightPlan.Route must have at least 2 waypoints.", "fp");
            if (flightLevels == null || flightLevels.Length == 0)
                throw new ArgumentException("flightLevels is empty", "flightLevels");

            ct.ThrowIfCancellationRequested();

            // 1) locate which GFS GRIB to use (same logic as WindTempMapService)
            var gfsInfo = _locator.GetFileForFlight(fp.DepartureTimeUtc);

            // 2) ensure GRIB exists locally (download if needed)
            string gribPath = await HttpDownloadHelper.DownloadFileAsync(
                gfsInfo.RemoteUrl,
                gfsInfo.LocalPath,
                ct);

            // 3) read available pressure levels from GRIB
            int[] availableLevels;
            using (var f = new GribFile(gribPath))
            {
                availableLevels = GfsLevels.GetAvailableIsobaricLevelsHpa(f);
            }

            if (availableLevels == null || availableLevels.Length == 0)
                throw new InvalidOperationException("No isobaricInhPa levels found in GRIB: " + gribPath);

            // 4) resolve requested FLs -> nearest available hPa levels in file
            int[] uniqueFls = flightLevels.Distinct().OrderBy(x => x).ToArray();

            // map FL -> snapped hPa
            var flToHpa = new Dictionary<int, int>();
            // map snapped hPa -> fields
            var hpaToFields = new Dictionary<int, LevelFields>();

            for (int i = 0; i < uniqueFls.Length; i++)
            {
                int fl = uniqueFls[i];

                int targetHpa = FlightLevelConverter.FlightLevelToHpa(fl);
                int snappedHpa = GfsLevels.SnapToNearest(targetHpa, availableLevels);

                flToHpa[fl] = snappedHpa;

                if (!hpaToFields.ContainsKey(snappedHpa))
                {
                    ct.ThrowIfCancellationRequested();

                    // Load wind/temp fields for this pressure level (copy into memory)
                    GfsWindField windField;
                    using (var gf = new GribFile(gribPath))
                    {
                        windField = GfsFieldFactory.CreateWindField(gf, snappedHpa);
                    }

                    GfsScalarField tempField;
                    using (var gf = new GribFile(gribPath))
                    {
                        tempField = GfsFieldFactory.CreateTemperatureField(gf, snappedHpa);
                    }

                    hpaToFields[snappedHpa] = new LevelFields
                    {
                        Hpa = snappedHpa,
                        Wind = windField,
                        Temp = tempField
                    };
                }
            }

            // 5) sample route
            var routePoints = RouteSampler.SampleRouteColumns(fp.Route, InsertStepNm, MinLegNmForInsert);
            double totalNm = routePoints[routePoints.Count - 1].DistanceNm;

            // TOC/TOD from names if present, else 20%/80%
            double tocNm = FindNamedDistance(routePoints, "TOC");
            double todNm = FindNamedDistance(routePoints, "TOD");
            if (tocNm < 0) tocNm = totalNm * 0.20;
            if (todNm < 0) todNm = totalNm * 0.80;

            if (todNm < tocNm)
            {
                double t = tocNm;
                tocNm = todNm;
                todNm = t;
            }

            // 6) build CrossSectionData (pure data)
            var data = new CrossSectionData
            {
                FlightId = fp.FlightId,
                RoutePoints = routePoints,
                FlightLevels = uniqueFls.ToList(),
                TotalDistanceNm = totalNm,
                TocDistanceNm = tocNm,
                TodDistanceNm = todNm,
                CruiseFlightLevel = fp.CruiseFlightLevel,
                RunTimeUtc = gfsInfo.RunTimeUtc,
                ValidTimeUtc = gfsInfo.RunTimeUtc.AddHours(gfsInfo.ForecastHour)
            };

            // 7) sample atmosphere for each route sample and each FL
            for (int pi = 0; pi < routePoints.Count; pi++)
            {
                ct.ThrowIfCancellationRequested();

                var p = routePoints[pi];

                for (int li = 0; li < uniqueFls.Length; li++)
                {
                    int fl = uniqueFls[li];
                    int hpa = flToHpa[fl];

                    LevelFields fields = hpaToFields[hpa];

                    var w = fields.Wind.Sample(p.Lat, p.Lon);
                    double tC = fields.Temp.Sample(p.Lat, p.Lon) - 273.15;

                    double ht = WindMath.HeadTailComponent(p.TrackDeg, w.DirDeg, w.SpeedKt);

                    data.Samples.Add(new CrossSectionSample
                    {
                        DistanceNm = p.DistanceNm,
                        Lat = p.Lat,
                        Lon = p.Lon,
                        FlightLevel = fl,
                        WindDirDeg = w.DirDeg,
                        WindSpeedKt = w.SpeedKt,
                        TemperatureC = tC,
                        HeadTailKt = ht
                    });
                }
            }

            // 8) render to PNG
            var renderer = new CrossSectionRenderer();

            string fileName = fp.FlightId + "_CROSS.png";
            string outPath = Path.Combine(_outputRoot, fileName);

            using (Bitmap img = renderer.Render(
                data,
                Width,
                Height,
                ShowTemperature,
                ShowWindBarbs,
                ShowHeadTailTable,
                ShowFlightProfile))
            {
                img.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
            }

            return outPath;
        }

        private static double FindNamedDistance(List<RouteSamplePoint> pts, string token)
        {
            if (pts == null || pts.Count == 0) return -1;

            for (int i = 0; i < pts.Count; i++)
            {
                if (!pts[i].IsWaypoint) continue;

                string name = (pts[i].WaypointName ?? "").Trim();
                if (name.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
                    return pts[i].DistanceNm;
            }

            return -1;
        }

        private class LevelFields
        {
            public int Hpa { get; set; }
            public GfsWindField Wind { get; set; }
            public GfsScalarField Temp { get; set; }
        }
    }
}








