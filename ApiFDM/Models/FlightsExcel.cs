using LinqToExcel;
using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ApiFDM.Models
{

    public class Boeing
    {
        public string Phase { get; set; }
        public string MainParameter { get; set; }
        public string Severity { get; set; }
        public string EventName { get; set; }
        public string Value { get; set; }
        public string Limit { get; set; }
        public string Minor { get; set; }
        public string Major { get; set; }
        public string Critical { get; set; }
        public float Duration { get; set; }
        public string Aircraft { get; set; }
        public string TOAirport { get; set; }
        public string TDAirport { get; set; }
        public string FlightNo { get; set; }
        public string Date { get; set; }
        public string P1 { get; set; }
        public string P2 { get; set; }
        public string IP { get; set; }
        public string FlyBy { get; set; }
        public string StateName { get; set; }
        public string Context { get; set; }
        public string FromAirportIATA { get; set; }
        public string ToAirportIATA { get; set; }
        public string FromAirport { get; set; }
        public string ToAirport { get; set; }
        public string TO_Datetime { get; set; }
        public string TD_Datetime { get; set; }
        public string Type { get; set; }
        public string Units { get; set; }
        public string ValueName { get; set; }
        public string EnginePos { get; set; }
        public int RegisterId { get; set; }
        public string FileName { get; set; }
        public int recordNum { get; set; }


        public string Reg
        {
            get
            {
                string aircraft = Aircraft == null ? null : Aircraft.Skip(Aircraft.IndexOf(("EP-"))).ToString();
                return aircraft;
            }
        }

        public float? ValueX
        {
            get
            {
                float? result = null;
                if (float.TryParse(Value, out float x))
                    result = float.Parse(Value);
                else
                    result = null;

                return result;
            }
        }

        public float? MinorX
        {
            get
            {
                float? result = null;
                if (float.TryParse(Minor, out float x))
                    result = float.Parse(Minor);
                else
                    result = null;

                return result;
            }
        }

        public float? MajorX
        {
            get
            {
                float? result = null;
                if (float.TryParse(Major, out float x))
                    result = float.Parse(Major);
                else
                    result = null;

                return result;
            }
        }
        public float? CriticalX
        {
            get
            {
                float? result = null;
                if (float.TryParse(Critical, out float x))
                    result = float.Parse(Critical);
                else
                    result = null;

                return result;
            }
        }

        //public float? DurationX
        //{
        //    get
        //    {
        //        float? result = 0;

        //        if (float.TryParse(Duration, out float x))
        //        {
        //            result = x;
        //        }
        //        else
        //        {
        //            result = null;
        //        }
        //        return result;
        //    }
        //}

        public DateTime? DateX
        {
            get
            {
                DateTime? result = new DateTime();

                if (DateTime.TryParse(TD_Datetime, out DateTime y))
                {
                    result = y.Date;
                }
                else
                {
                    result = null;
                }
                return result;
            }
        }

        public string SeverityX
        {
            get
            {
                var severity = this.Severity.ToLower();

                string result = null;
                if (severity == "high" || severity == "critical")
                    result = "High";
                else if (severity == "medium" || severity == "major")
                    result = "Medium";
                else if (severity == "low" || severity == "minor")
                    result = "Low";

                return result;
            }
        }



        public string FlightNumber
        {
            get
            {
                string result = null;
                var B737FLTNO = (FlightNo == null) ? (int?)null : FlightNo.Length;
                if (B737FLTNO == 2)
                    result = "00" + FlightNo;
                else
                    result = FlightNo;
                return result;
            }
        }
        public bool IsValid
        {
            get
            {
                if (this.DateX == null || this.EventName == null || this.SeverityX == null || this.Duration == null)
                    return false;
                return true;
            }
        }

    }

    public class MD
    {
        public string ValueName { get; set; }
        public string Severity { get; set; }
        public string EventName { get; set; }
        public float Duration { get; set; }
        public string ExceedValue { get; set; }
        public string LimitValue { get; set; }
        public string Phase { get; set; }
        public string FromAirport { get; set; }
        public string FromAirportIATA { get; set; }
        public string ToAirport { get; set; }
        public string ToAirportIATA { get; set; }
        public string Date { get; set; }

        public string FlightNo { get; set; }
        public string Aircraft { get; set; }
        public string IP { get; set; }
        public string P1 { get; set; }
        public string P2 { get; set; }
        public string PIC { get; set; }
        public string AircraftType { get; set; }
        public string MainParameter { get; set; }
        public string Context { get; set; }
        public string TO_DateTime { get; set; }
        public string TD_DateTime { get; set; }
        public int RegisterId { get; set; }
        public string FileName { get; set; }
        public int recordNum { get; set; }
        public string StateName { get; set; }
        public float? ValueX
        {
            get
            {
                float? result = null;
                if (float.TryParse(ExceedValue, out float x))
                    result = float.Parse(ExceedValue);
                else
                    result = null;

                return result;
            }
        }

        //public float? DurationX
        //{
        //    get
        //    {
        //        float? result = 0;

        //        if (float.TryParse(Duration, out float flt))
        //        {
        //            result = float.Parse(Duration);
        //        }
        //        else
        //        {
        //            result = null;
        //        }
        //        return result;
        //    }
        //}

        public DateTime? DateX
        {
            get
            {
             
                    DateTime? result = new DateTime();

                    if (DateTime.TryParse(TO_DateTime, out DateTime y))
                    {
                        result = y.Date;
                    }
                    else
                    {
                        result = null;
                    }
                    return result;
                
                //DateTime? result = new DateTime();


                ////   var _date = Date.Length == 9 ? "20" + Date : Date;
                //try
                //{
                //    var split = TO_DateTime.Split('-');
                //    var month = 0;
                //    switch (split[1])
                //    {
                //        case "Jan":
                //            month = 1;
                //            break;
                //        case "Feb":
                //            month = 2;
                //            break;
                //        case "Mar":
                //            month = 3;
                //            break;
                //        case "Apr":
                //            month = 4;
                //            break;
                //        case "May":
                //            month = 5;
                //            break;
                //        case "Jun":
                //            month = 6;
                //            break;
                //        case "Jul":
                //            month = 7;
                //            break;
                //        case "Aug":
                //            month = 8;
                //            break;
                //        case "Sep":
                //            month = 9;
                //            break;
                //        case "Oct":
                //            month = 10;
                //            break;
                //        case "Nov":
                //            month = 11;
                //            break;
                //        case "Dec":
                //            month = 12;
                //            break;

                //    }

                //    result = new DateTime(2000 + Int32.Parse(split[0]), month, Int32.Parse(split[2]));

                //}
                //catch (Exception ex)
                //{
                //    result = null;
                //}


                //return result;
            }
        }



        public string FlightNumber
        {
            get
            {
                string result = null;
                var B737FLTNO = (FlightNo == null) ? (int?)null : FlightNo.Length;
                if (B737FLTNO == 2)
                    result = "00" + FlightNo;
                else
                    result = FlightNo;
                return result;
            }
        }


        public bool IsValid
        {
            get
            {
                if (this.DateX == null || this.EventName == null || this.LimitLevelX == null)
                    return false;
                return true;
            }
        }


        public string LimitLevelX
        {
            get
            {
                var limitLevel = this.Severity.ToLower();

                string result = null;
                if (limitLevel == "high" || limitLevel == "critical")
                    result = "High";
                else if (limitLevel == "medium" || limitLevel == "major")
                    result = "Medium";
                else if (limitLevel == "low" || limitLevel == "minor")
                    result = "Low";


                return result;

            }
        }
    }

    public class FailedItmes
    {
        public string flightNo { get; set; }
        public string Severity { get; set; }
        public DateTime? Date { get; set; }
        public string EventName { get; set; }
        public string P1 { get; set; }
        public string P2 { get; set; }
        public string FileName { get; set; }
        public float? Value { get; set; }
        public int? Status { get; set; }
        public TimeSpan Duration { get; set; }
        public string Message { get; set; }
    }


}
