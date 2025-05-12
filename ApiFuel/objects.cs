using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ApiFuel
{
    public class FlightPlanRoute
    {
        public List<Waypoint> Waypoints { get; set; }
    }

    public class Waypoint
    {
        public string Name { get; set; }
        public Coordinate Coordinates { get; set; }
        public string Via { get; set; }
        public int AltitudeFL { get; set; } // Flight Level, e.g., 310 = FL310
        public int? MEA { get; set; } // Minimum Enroute Altitude
        public int GroundSpeed { get; set; } // GSP
        public int TrueAirSpeed { get; set; } // TAS
        public TrackInfo Track { get; set; }
        public TimeInfo Time { get; set; }
        public FuelInfo Fuel { get; set; }
        public TemperatureInfo Temperature { get; set; }
        public WidthInfo Width { get; set; }
        public float DistanceFromStart { get; set; } // Total Distance from departure
        public string Key { get; set; }
    }

    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Raw { get; set; } // e.g., N3607.6 E05902.8
    }

    public class TrackInfo
    {
        public int MagneticTrack { get; set; }
        public int TrueTrack { get; set; }
        public int GroundTrack { get; set; }
    }

    public class TimeInfo
    {
        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan TotalTime { get; set; }
    }

    public class FuelInfo
    {
        public int Remaining { get; set; } // in kg or lbs
        public int Used { get; set; }
    }

    public class TemperatureInfo
    {
        public int OutsideAirTemperature { get; set; }
        public string Raw { get; set; } // "-43 / 04/01/37152" for full info
    }

    public class WidthInfo
    {
        public string Raw { get; set; }
        public string Type { get; set; } // e.g., "T" for track or "H" for holding
    }


    public static class FlightPlanParser
    {
        public static FlightPlanRoute ParseRoute(string text)
        {
            var route = new FlightPlanRoute { Waypoints = new List<Waypoint>() };
            var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("Pos:") || line.StartsWith("---") || line.Contains("LAT") || line.Contains("POS:"))
                    continue; // skip headers and dividers

                var columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (columns.Length < 15)
                    continue; // skip malformed lines

                var waypoint = new Waypoint
                {
                    Name = columns[0],
                    Coordinates = ParseCoordinate(columns[1], columns[2]),
                    Via = columns[3],
                    AltitudeFL = TryParseInt(columns[4]),
                    MEA = TryParseIntNullable(columns[5]),
                    GroundSpeed = TryParseInt(columns[6]),
                    TrueAirSpeed = TryParseInt(columns[7]),
                    Track = new TrackInfo
                    {
                        MagneticTrack = TryParseInt(columns[8]),
                        TrueTrack = TryParseInt(columns[9]),
                        GroundTrack = TryParseInt(columns[10]),
                    },
                    Time = new TimeInfo
                    {
                        ElapsedTime = ParseTime(columns[11]),
                        TotalTime = ParseTime(columns[12]),
                    },
                    Fuel = new FuelInfo
                    {
                        Remaining = TryParseInt(columns[13]),
                        Used = TryParseInt(columns[14]),
                    },
                    Temperature = new TemperatureInfo
                    {
                        Raw = columns.Length > 15 ? columns[15] : "",
                        OutsideAirTemperature = TryParseTemperature(columns.Length > 15 ? columns[15] : "")
                    },
                    Width = new WidthInfo
                    {
                        Raw = columns.Length > 16 ? columns[16] : "",
                        Type = (columns.Length > 16 && !string.IsNullOrEmpty(columns[16]))
           ? columns[16].Substring(0, 1)
           : ""
                    },
                    DistanceFromStart = columns.Length > 17 ? TryParseFloat(columns[17]) : 0,
                    Key = columns.Length > 18 ? columns[18] : ""
                };

                route.Waypoints.Add(waypoint);
            }

            return route;
        }

        private static Coordinate ParseCoordinate(string latRaw, string lonRaw)
        {
            return new Coordinate
            {
                Raw = $"{latRaw} {lonRaw}",
                Latitude = ParseLatLong(latRaw),
                Longitude = ParseLatLong(lonRaw)
            };
        }

        private static double ParseLatLong(string raw)
        {
            // Example: N3607.6 or E05902.8
            try
            {
                char dir = raw[0];
                string degrees = raw.Substring(1, dir == 'N' || dir == 'S' ? 2 : 3);
                string minutes = raw.Substring(degrees.Length + 1);

                double deg = double.Parse(degrees, CultureInfo.InvariantCulture);
                double min = double.Parse(minutes, CultureInfo.InvariantCulture);

                double result = deg + (min / 60.0);

                if (dir == 'S' || dir == 'W') result *= -1;
                return result;
            }
            catch
            {
                return 0;
            }
        }

        private static TimeSpan ParseTime(string time)
        {
            // Format: "00:03:00"
            TimeSpan.TryParse(time, out var result);
            return result;
        }

        private static int TryParseInt(string val)
        {
            int.TryParse(val, out var result);
            return result;
        }

        private static int? TryParseIntNullable(string val)
        {
            return int.TryParse(val, out var result) ? result : (int?)null;
        }

        private static int TryParseTemperature(string val)
        {
            // Extract the first number from something like "-43 / 04/01/37152"
            if (string.IsNullOrWhiteSpace(val)) return 0;
            var parts = val.Split('/');
            if (int.TryParse(parts[0].Trim().Split(' ')[0], out int result))
                return result;
            return 0;
        }

        private static float TryParseFloat(string val)
        {
            float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            return result;
        }
    }

    public class NullableIntConverter : JsonConverter<int?>
    {
        public override int? ReadJson(JsonReader reader, Type objectType, int? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var str = reader.Value?.ToString();
                if (int.TryParse(str, out int val))
                    return val;
                return null;
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                return Convert.ToInt32(reader.Value);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, int? value, JsonSerializer serializer)
        {
            if (value.HasValue)
                writer.WriteValue(value.Value);
            else
                writer.WriteNull();
        }
    }

    // Nullable Double Converter
    public class NullableDoubleConverter : JsonConverter<double?>
    {
        public override double? ReadJson(JsonReader reader, Type objectType, double? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var str = reader.Value?.ToString();
                if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                    return val;
                return null;
            }
            else if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
            {
                return Convert.ToDouble(reader.Value);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, double? value, JsonSerializer serializer)
        {
            if (value.HasValue)
                writer.WriteValue(value.Value);
            else
                writer.WriteNull();
        }
    }

    // Nullable DateTime Converter (ISO 8601)
    public class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var str = reader.Value?.ToString();
                if (DateTime.TryParse(str, null, DateTimeStyles.AdjustToUniversal, out DateTime dt))
                    return dt;
                return null;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
        {
            if (value.HasValue)
                writer.WriteValue(value.Value.ToString("o")); // ISO 8601
            else
                writer.WriteNull();
        }
    }


    public class TimeToMinutesConverter : JsonConverter<int?>
    {
        public override int? ReadJson(JsonReader reader, Type objectType, int? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var timeString = reader.Value?.ToString();
                if (!string.IsNullOrEmpty(timeString))
                {
                    // Split time string to hours and minutes
                    var parts = timeString.Split(':');
                    //if (parts.Length == 2)
                    {
                        if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
                        {
                            return hours * 60 + minutes; // Convert to total minutes
                        }
                    }
                }
            }
            return null; // Return null if the time string is not valid
        }

        public override void WriteJson(JsonWriter writer, int? value, JsonSerializer serializer)
        {
            // Just write the value as is (it is already in minutes)
            if (value.HasValue)
                writer.WriteValue(value.Value);
            else
                writer.WriteNull();
        }
    }

    public class _Waypoint
    {
        public string WAP { get; set; }
        public string COR { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? LAT { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? LON { get; set; }
        
        public string FRQ { get; set; }
        public string VIA { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? ALT { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? MEA { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? GMR { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? DIS { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? TDS { get; set; }
        public string WID { get; set; } // می‌تونیم اینو هم اگر ساختار ثابتی داره، به مدل دقیق‌تری تبدیل کنیم
        public string TRK { get; set; } // مشابه بالا
        public string TMP { get; set; } // می‌تونیم تجزیه‌اش کنیم به دما، باد، ...
        
             [JsonConverter(typeof(TimeToMinutesConverter))]
        public int? TME { get; set; }
        [JsonConverter(typeof(TimeToMinutesConverter))]
        public int? TTM { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? FRE { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? FUS { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? TAS { get; set; }
        [JsonConverter(typeof(NullableDoubleConverter))]
        public double? GSP { get; set; }
        public string _key { get; set; }
    }




}