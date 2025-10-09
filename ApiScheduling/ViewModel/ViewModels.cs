using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using ApiScheduling.ViewModel;
using System.Net;
using System.Data.Entity.Validation;
using System.Text;
using System.Web.Http.ModelBinding;

namespace ApiScheduling
{

    public class Exceptions
    {
        internal static Exception HandleDbUpdateException(DbUpdateException dbu)
        {
            var builder = new StringBuilder("A DbUpdateException was caught while saving changes. ");

            try
            {
                foreach (var result in dbu.Entries)
                {
                    builder.AppendFormat("Type: {0} was part of the problem. ", result.Entity.GetType().Name);
                }
            }
            catch (Exception e)
            {
                builder.Append("Error parsing DbUpdateException: " + e.ToString());
            }

            string message = builder.ToString();
            return new Exception(message, dbu);
        }


        internal static CustomActionResult getModelValidationException(ModelStateDictionary modelState)
        {
            return new CustomActionResult(HttpStatusCode.NotAcceptable, "ERR-VALIDATION-00: validation failed.value required");
        }
        internal static CustomActionResult getNullException(ModelStateDictionary modelState)
        {
            return new CustomActionResult(HttpStatusCode.NotAcceptable, "ERR-VALIDATION-01: validation failed.object is null");
        }

        internal static CustomActionResult getNotFoundException()
        {
            return new CustomActionResult(HttpStatusCode.NotAcceptable, "ERR-VALIDATION-02:object not found");
        }

        internal static CustomActionResult getDuplicateException(string code, string property)
        {
            return new CustomActionResult(HttpStatusCode.NotAcceptable, "ERR-" + code + ":Duplicate " + property + " found.");
        }

        internal static CustomActionResult getCanNotDeleteException(string code)
        {
            return new CustomActionResult(HttpStatusCode.NotAcceptable, "ERR-" + code + ":Item cannot be deleted.");
        }
    }
}


namespace ApiScheduling.Models
{
    public partial class dbEntities
    {
        public async Task<CustomActionResult> SaveAsync()
        {
            try
            {

                await this.SaveChangesAsync();
                return new CustomActionResult(HttpStatusCode.OK, "");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {

                    foreach (var ve in eve.ValidationErrors)
                    {
                        var xxx =
                             ve.PropertyName + " " + ve.ErrorMessage;
                    }
                }
                return new CustomActionResult(HttpStatusCode.InternalServerError, "DbEntityValidationException");
            }
            catch (DbUpdateException dbu)
            {
                var exception = Exceptions.HandleDbUpdateException(dbu);
                return new CustomActionResult(HttpStatusCode.InternalServerError, exception.Message);
            }
            catch (Exception ex)
            {
                return new CustomActionResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public CustomActionResult SaveActionResult()
        {
            try
            {

                this.SaveChangesAsync();
                return new CustomActionResult(HttpStatusCode.OK, "");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {

                    foreach (var ve in eve.ValidationErrors)
                    {
                        var xxx =
                             ve.PropertyName + " " + ve.ErrorMessage;
                    }
                }
                return new CustomActionResult(HttpStatusCode.InternalServerError, "DbEntityValidationException");
            }
            catch (DbUpdateException dbu)
            {
                var exception = Exceptions.HandleDbUpdateException(dbu);
                return new CustomActionResult(HttpStatusCode.InternalServerError, exception.Message);
            }
            catch (Exception ex)
            {
                return new CustomActionResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }



    }
}
namespace ApiScheduling.ViewModel
{
    public class RosterFDPDto
    {
        public int Id { get; set; }
        public int? IsGantt { get; set; }
        public string UserName { get; set; }
        public List<RosterFDPId> ids { get; set; }
        public int crewId { get; set; }
        public string rank { get; set; }
        public int index { get; set; }
        public List<string> flights { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public int homeBase { get; set; }
        public string flts { get; set; }
        public string route { get; set; }
        public string key { get; set; }
        public string group { get; set; }
        public string scheduleName { get; set; }
        public string no { get; set; }
        public int? extension { get; set; }
        public decimal? maxFDP { get; set; }

        public bool split { get; set; }

        public bool? IsSplitDuty { get; set; }
        public int? SplitValue { get; set; }

        public int? IsAdmin { get; set; }

        public int? DeletedFDPId { get; set; }

        public List<RosterFDPDtoItem> items { get; set; }

        public double getDuty(double? default_reporting)
        {
            var def = (double)default_reporting;
            return (this.items.Last().sta.AddMinutes(30) - this.items.First().std.AddMinutes(-1*def)).TotalMinutes;
        }
        public double getFlight()
        {
            double flt = 0;
            foreach (var x in this.items)
                flt += (x.sta - x.std).TotalMinutes;
            return flt;
        }
        public static List<RosterFDPDtoItem> getItems(List<string> flts)
        {
            List<RosterFDPDtoItem> result = new List<RosterFDPDtoItem>();
            foreach (var x in flts)
            {
                var parts = x.Split('_');
                var item = new RosterFDPDtoItem();
                item.flightId = Convert.ToInt32(parts[0]);
                item.dh = Convert.ToInt32(parts[1]);
                var stdStr = parts[2];
                var staStr = parts[3];
                item.std = new DateTime(Convert.ToInt32(stdStr.Substring(0, 4)), Convert.ToInt32(stdStr.Substring(4, 2)), Convert.ToInt32(stdStr.Substring(6, 2))
                    , Convert.ToInt32(stdStr.Substring(8, 2))
                    , Convert.ToInt32(stdStr.Substring(10, 2))
                    , 0
                    ).ToUniversalTime();
                item.sta = new DateTime(Convert.ToInt32(staStr.Substring(0, 4)), Convert.ToInt32(staStr.Substring(4, 2)), Convert.ToInt32(staStr.Substring(6, 2))
                   , Convert.ToInt32(staStr.Substring(8, 2))
                   , Convert.ToInt32(staStr.Substring(10, 2))
                   , 0
                   ).ToUniversalTime();
                item.no = parts[4];
                item.from = parts[5];
                item.to = parts[6];

                result.Add(item);
            }

            return result;
        }
        public static List<string> getFlightsStrs(List<Models.SchFlight> flts, List<RosterFDPId> ids)
        {
            var result = new List<string>();
            //fdp.flights.push(flt.ID + '_' + _d.dh + '_' + $scope.DatetoStr(new Date(flt.ChocksOut)) + '_' + $scope.DatetoStr(new Date(flt.ChocksIn))
            //+ '_' + flt.FlightNumber + '_' + flt.FromAirportIATA + '_' + flt.ToAirportIATA);
            // \"flights\":[\"542235_0_202301200735_202301200905_5822_THR_KIH\",\"542237_0_202301200950_202301201135_5823_KIH_THR\"],
            //\"key2\":\"542235*0_542237*0\",
            foreach(var x in flts)
            {
                var id_item = ids.FirstOrDefault(q => q.id == x.ID);
                var prts = new List<string>();
                prts.Add(x.ID.ToString());
                prts.Add(id_item.dh.ToString());
                prts.Add(((DateTime)(x.ChocksOutLocal ?? x.STDLocal)).ToString("yyyyMMddHHmm"));
                prts.Add(((DateTime)(x.ChocksInLocal ?? x.STALocal)).ToString("yyyyMMddHHmm"));
                prts.Add(x.FlightNumber);
                prts.Add(x.FromAirportIATA);
                prts.Add(x.ToAirportIATA);
                result.Add(string.Join("_",prts));
            }
            return result;
        }

        public static List<string> getFlightsStrs(List<Models.SchFlight> flts, List<int?> dhs)
        {
            var result = new List<string>();
            //fdp.flights.push(flt.ID + '_' + _d.dh + '_' + $scope.DatetoStr(new Date(flt.ChocksOut)) + '_' + $scope.DatetoStr(new Date(flt.ChocksIn))
            //+ '_' + flt.FlightNumber + '_' + flt.FromAirportIATA + '_' + flt.ToAirportIATA);
            // \"flights\":[\"542235_0_202301200735_202301200905_5822_THR_KIH\",\"542237_0_202301200950_202301201135_5823_KIH_THR\"],
            //\"key2\":\"542235*0_542237*0\",
            foreach (var x in flts)
            {
                var dh = dhs.FirstOrDefault(q => q == x.ID);
                var prts = new List<string>();
                prts.Add(x.ID.ToString());
                prts.Add(dh==null?"0":"1");
                prts.Add(((DateTime)x.ChocksOutLocal).ToString("yyyyMMddHHmm"));
                prts.Add(((DateTime)x.ChocksInLocal).ToString("yyyyMMddHHmm"));
                prts.Add(x.FlightNumber);
                prts.Add(x.FromAirportIATA);
                prts.Add(x.ToAirportIATA);
                result.Add(string.Join("_", prts));
            }
            return result;
        }
        public static List<RosterFDPDtoItem> getItemsX(List<Models.SchFlight> flts, List<RosterFDPId> ids)
        {
            //oooo
            List<RosterFDPDtoItem> result = new List<RosterFDPDtoItem>();
            foreach(var x in flts)
            {
                var id_item = ids.FirstOrDefault(q => q.id == x.ID);
                var item = new RosterFDPDtoItem();
                item.flightId = x.ID;
                item.dh = id_item.dh;
                item.pos = id_item.pos;
                item.std =(DateTime) x.STD;
                item.sta = (DateTime)x.STA;
                item.offblock = (DateTime)(x.ChocksOut ?? x.STD);
                item.onblock = (DateTime)(x.ChocksIn ?? x.STA);

                item.no = x.FlightNumber;
                item.from = x.FromAirportIATA;
                item.to = x.ToAirportIATA;
                
                result.Add(item);

            }
          

            return result;
        }


        public static int getRank(string rank)
        {
            if (rank.StartsWith("IP"))
                return 12000;
            if (rank.StartsWith("P1"))
                return 1160;
            if (rank.StartsWith("P2"))
                return 1161;
            if (rank.ToUpper().StartsWith("SAFETY"))
                return 1162;
            if (rank.ToUpper().StartsWith("FE"))
                return 1165;
            if (rank.StartsWith("ISCCM") || rank.StartsWith("CCI"))
                return 10002;
            if (rank.StartsWith("SCCM"))
                return 1157;
            if (rank.StartsWith("CCM"))
                return 1158;
            if (rank.StartsWith("CCE"))
                return 1159;
            if (rank.StartsWith("OBS"))
                return 1153;
            if (rank.StartsWith("CHECK"))
                return 1154;
            if (rank.StartsWith("00103"))
                return 12001;
            if (rank.StartsWith("004"))
                return 12002;
            if (rank.StartsWith("005"))
                return 12003;
            if (rank.StartsWith("004"))
                return 12002;

            return -1;

        }
        public static string getRankStr(int rank)
        {
            if (rank == 12000)
                return "IP";
            if (rank == 1160)
                return "P1";
            if (rank == 1161)
                return "P2";
            if (rank == 1162)
                return "SAFETY";
            if (rank == 10002)
                return "ISCCM";
            if (rank == 1157)
                return "SCCM";
            if (rank == 1158)
                return "CCM";
            if (rank == 1153)
                return "OBS";
            if (rank == 1154)
                return "CHECK";
            return "";
        }


    }



    public class RosterFDPId
    {
        public int id { get; set; }
        public int dh { get; set; }
        public string pos { get; set; }
    }

    public class RosterFDPDtoItem
    {
        public int flightId { get; set; }
        public int dh { get; set; }
        public DateTime std { get; set; }
        public DateTime sta { get; set; }
        public DateTime offblock { get; set; }
        public DateTime onblock { get; set; }
        public int index { get; set; }
        public int rankId { get; set; }
        public string no { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string pos { get; set; }




    }

    public class CustomActionResult : IHttpActionResult
    {

        private System.Net.HttpStatusCode statusCode;

        public object data;
        public System.Net.HttpStatusCode Code { get { return statusCode; } }
        public CustomActionResult(System.Net.HttpStatusCode statusCode, object data)
        {

            this.statusCode = statusCode;

            this.data = data;

        }
        public HttpResponseMessage CreateResponse(System.Net.HttpStatusCode statusCode, object data)
        {

            HttpRequestMessage request = new HttpRequestMessage();
            request.Properties.Add(System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            HttpResponseMessage response = request.CreateResponse(statusCode, data);

            return response;

        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(CreateResponse(this.statusCode, data));
        }

    }





}