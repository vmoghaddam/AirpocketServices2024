using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;


using System.Web.Http.Description;

using System.Data.Entity.Validation;

using System.Web.Http.ModelBinding;

using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using System.IO;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;
using Formatting = Newtonsoft.Json.Formatting;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using ApiAPSB.Models;
using System.Net.Http.Headers;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using System.Reflection;

namespace ApiAPSB.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OFPController : ApiController
    {
        [HttpPost]
        [Route("api/ofp/flights")]
        public async Task<IHttpActionResult> PostGetOFPsByFlightIds(SimpleDto dto)
        {
            var context = new Models.dbEntities();

            var ofps = await context.view_ofpb_root.Where(q => dto.ids.Contains(q.FlightID)).ToListAsync();
            var ofp_ids = ofps.Select(q => (Nullable<int>)q.Id).ToList();
            var nav_logs = await context.view_ofpb_navlog.Where(q => ofp_ids.Contains(q.RootId)).OrderBy(q => q.RootId).ThenBy(q => q.NavType).ThenBy(q => q.Id).ToListAsync();
            var wts = await context.view_ofpb_wt.Where(q => ofp_ids.Contains(q.OFPId)).OrderBy(q => q.OFPId).ThenBy(q => q.Type).ThenBy(q => q.Id).ToListAsync();

            var ofp_grps = (from x in ofps
                            group x by new { x.FlightID, ofp_id = x.Id } into grp
                            select new ofpb()
                            {
                                flight_id = grp.Key.FlightID,
                                ofp_id = grp.Key.ofp_id,
                                root = grp.First(),
                                main_route = nav_logs.Where(q => q.RootId == grp.Key.ofp_id && q.NavType == "MAIN").ToList(),
                                alt1_route = nav_logs.Where(q => q.RootId == grp.Key.ofp_id && q.NavType == "ALT1").ToList(),
                                alt2_route = nav_logs.Where(q => q.RootId == grp.Key.ofp_id && q.NavType == "ALT2").ToList(),
                                main_wts = wts.Where(q => q.OFPId == grp.Key.ofp_id && q.Type == "MAIN").ToList(),
                                alt1_wts = wts.Where(q => q.OFPId == grp.Key.ofp_id && q.Type == "ALT1").ToList(),
                                alt2_wts = wts.Where(q => q.OFPId == grp.Key.ofp_id && q.Type == "ALT2").ToList(),
                                to_wts = wts.Where(q => q.OFPId == grp.Key.ofp_id && q.Type == "ALTTO").ToList(),
                                routes = nav_logs.Where(q => q.RootId == grp.Key.ofp_id).ToList(),
                                wts = wts.Where(q => q.OFPId == grp.Key.ofp_id).ToList(),

                            }).ToList();


            return Ok(new DataResponse()
            {
                Data = ofp_grps,
                Errors = null,
                IsSuccess = true,
            });


        }

        [HttpGet]
        [Route("api/ofp/flight/{flightId}")]
        public async Task<IHttpActionResult> GetOFPByFlight(int flightId)
        {
            var context = new Models.dbEntities();

            var ofp = await context.view_ofpb_root.Where(q => q.FlightID == flightId).FirstOrDefaultAsync();

            if (ofp == null)
            {
                return Ok(new DataResponse()
                {
                    Data = new ofpb() { ofp_id = -1, flight_id = flightId },
                    Errors = null,
                    IsSuccess = true,
                });
            }

            var nav_logs = await context.view_ofpb_navlog.Where(q => q.RootId == ofp.Id).OrderBy(q => q.RootId).ThenBy(q => q.NavType).ThenBy(q => q.Id).ToListAsync();
            var wts = await context.view_ofpb_wt.Where(q => q.OFPId == ofp.Id).OrderBy(q => q.OFPId).ThenBy(q => q.Type).ThenBy(q => q.Id).ToListAsync();



            var ofp_obj = new ofpb()
            {
                ofp_id = ofp.Id,
                flight_id = flightId,
                root = ofp,
                main_route = nav_logs.Where(q => q.NavType == "MAIN").ToList(),
                alt1_route = nav_logs.Where(q => q.NavType == "ALT1").ToList(),
                alt2_route = nav_logs.Where(q => q.NavType == "ALT2").ToList(),
                main_wts = wts.Where(q => q.Type == "MAIN").ToList(),
                alt1_wts = wts.Where(q => q.Type == "ALT1").ToList(),
                alt2_wts = wts.Where(q => q.Type == "ALT2").ToList(),
                to_wts = wts.Where(q => q.Type == "ALTTO").ToList(),
                routes = nav_logs.ToList(),
                wts = wts.ToList(),
            };


            return Ok(new DataResponse()
            {
                Data = ofp_obj,
                Errors = null,
                IsSuccess = true,
            });


        }


        [HttpGet]
        [Route("api/ofp/flight/report/{flightId}")]
        public async Task<IHttpActionResult> GetOFPByFlightReport(int flightId)
        {
            var context = new Models.dbEntities();

            var ofp = await context.view_ofbb_root_actual .Where(q => q.FlightID == flightId).FirstOrDefaultAsync();

            if (ofp == null)
            {
                return Ok(new DataResponse()
                {
                    Data = new ofpb() { ofp_id = -1, flight_id = flightId },
                    Errors = null,
                    IsSuccess = true,
                });
            }

            var nav_logs = await context.view_ofpb_navlog_actual.Where(q => q.RootId == ofp.Id).OrderBy(q => q.RootId).ThenBy(q => q.NavType).ThenBy(q => q.Id).ToListAsync();
            var wts = await context.view_ofpb_wt.Where(q => q.OFPId == ofp.Id).OrderBy(q => q.OFPId).ThenBy(q => q.Type).ThenBy(q => q.Id).ToListAsync();



            var ofp_obj = new  
            {
                ofp_id = ofp.Id,
                flight_id = flightId,
                root = ofp,
                main_route = nav_logs.Where(q => q.NavType == "MAIN").ToList(),
                alt1_route = nav_logs.Where(q => q.NavType == "ALT1").ToList(),
                alt2_route = nav_logs.Where(q => q.NavType == "ALT2").ToList(),
                main_wts = wts.Where(q => q.Type == "MAIN").ToList(),
                alt1_wts = wts.Where(q => q.Type == "ALT1").ToList(),
                alt2_wts = wts.Where(q => q.Type == "ALT2").ToList(),
                to_wts = wts.Where(q => q.Type == "ALTTO").ToList(),
                routes = nav_logs.ToList(),
                wts = wts.ToList(),
            };


            return Ok(new DataResponse()
            {
                Data = ofp_obj,
                Errors = null,
                IsSuccess = true,
            });


        }
        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        [HttpPost]
        [Route("api/ofp/prop/save/test")]
        public async Task<DataResponse> SaveOFPPropTest(/*int ofpId, string propName, string propValue, string user*/dynamic dto)
        {
            try
            {
                int ofpId = Convert.ToInt32(dto.OFPId);
                string propName = Convert.ToString(dto.PropName);
                string propValue = Convert.ToString(dto.PropValue);
                string user = Convert.ToString(dto.User);

                var property = propName.Split('-')[2];

                var tbl_part = propName.Split('-')[1];
                string table = "";
                switch (tbl_part)
                {
                    case "root":
                        table = "OFPB_Root";
                        break;
                    default:
                        break;
                }



                if (table == "OFPB_Root")
                {
                    //var ofp_root = await context.OFPB_Root.FirstOrDefaultAsync(q => q.Id == ofpId);
                    var ofp_root = new OFPB_Root();
                    if (ofp_root != null)
                    {
                        Type type = ofp_root.GetType();

                        PropertyInfo propinfo = type.GetProperty(property);
                        if (string.IsNullOrEmpty(propValue))
                        {
                            propinfo.SetValue(ofp_root, null, null);
                        }
                        else
                        {
                            var val = ChangeType(propValue, propinfo.PropertyType);

                            propinfo.SetValue(ofp_root, val, null);
                        }


                    }
                }


                return new DataResponse() { IsSuccess = true };

            }
            catch (Exception ex)
            {
                return new DataResponse() { IsSuccess = false };
            }




        }

        [HttpPost]
        [Route("api/ofp/prop/save")]
        public async Task<DataResponse> SaveOFPProp(/*int ofpId, string propName, string propValue, string user*/dynamic dto)
        {
            try
            {
                int ofpId = Convert.ToInt32(dto.OFPId);
                string propName = Convert.ToString(dto.PropName);
                string propValue = Convert.ToString(dto.PropValue);
                string user = Convert.ToString(dto.User);
                var context = new Models.dbEntities();
                var prop = await context.OFPB_Prop.Where(q => q.OFPId == ofpId && q.PropName == propName).FirstOrDefaultAsync();
                //prop-root-crew_cockpit
                var property = propName.Split('-')[2];

                var tbl_part = propName.Split('-')[1];
                string table = "";
                switch (tbl_part)
                {
                    case "root":
                        table = "OFPB_Root";
                        break;
                    default:
                        break;
                }
                if (prop == null)
                {
                    prop = new OFPB_Prop()
                    {
                        OFPId = ofpId,

                        PropName = propName,
                        TableName = table,
                        PropertyName = property,


                    };
                    context.OFPB_Prop.Add(prop);
                }
                prop.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                if (!string.IsNullOrEmpty(propValue.Trim().Replace(" ", "")))
                    prop.PropValue = propValue;
                prop.User = user;


                if (table == "OFPB_Root")
                {
                    var ofp_root = await context.OFPB_Root.FirstOrDefaultAsync(q => q.Id == ofpId);
                    if (ofp_root != null)
                    {
                        Type type = ofp_root.GetType();

                        PropertyInfo propinfo = type.GetProperty(property);
                        if (string.IsNullOrEmpty(propValue))
                        {
                            propinfo.SetValue(ofp_root, null, null);
                        }
                        else
                        {
                            var val = ChangeType(propValue, propinfo.PropertyType);

                            propinfo.SetValue(ofp_root, val, null);
                        }
                    }
                }


                var saveResult = await context.SaveChangesAsync();
                return new DataResponse() { IsSuccess = true, Data = new { prop.Id, prop.OFPId, prop.PropName, prop.PropValue, prop.DateUpdate, prop.User, prop.PropType } };

            }
            catch (Exception ex)
            {
                return new DataResponse() { IsSuccess = false };
            }




        }

        public class _fuel_changes
        {
            public int? FuelUsed2 { get; set; }

            public int? FuelRemained2 { get; set; }


            public int? FuelUsedActual { get; set; }

            public bool IsFuelUsed2 { get; set; }
            public bool IsFuelRemained2 { get; set; }
            public bool IsFuelUsedActual { get; set; }
        }
        public class _fuel_bulk
        {
            public _fuel_changes changes { get; set; }
            public int key { get; set; }
        }
        public class __fuel_bulk_dto
        {
            public List<_fuel_bulk> items { get; set; }
            public string user { get; set; }
        }
        public class _eta_changes
        {
            public string ETA { get; set; }

           
        }
        public class _eta_bulk
        {
            public _eta_changes changes { get; set; }
            public int key { get; set; }
        }
        public class _eta_bulk_dto
        {
            public List<_eta_bulk> items { get; set; }
            public string user { get; set; }
        }

        //{ id: _d.Id, eta: _d.ETA, ata: _d.ata, remained: _d.FuelRemainedActual, used: _d.FuelUsedActual}
        public class _sync_navlog_item
        {
            public int id { get; set; }
            public string eta { get; set; }
            public string ata { get; set; }
            public int? remained { get; set; }
            public int? used { get; set; }

            public int? remained2 { get; set; }
            public int? used2 { get; set; }

        }
        public class _sync_navlog
        {
            public string user { get; set; }
            public List<_sync_navlog_item> items { get; set; }

        }

        public class _sync_navlog_item2
        {
            public int id { get; set; }
            public string key { get; set; }
            public string value { get; set; }
            public string date_update { get; set; }
             

        }
        public class _sync_navlog2
        {
            public string user { get; set; }
            public List<_sync_navlog_item2> items { get; set; }

        }


        public class _sync_root
        {
            public string user { get; set; }
            public List<_sync_root_item> items { get; set; }

        }
        public class _sync_root_item
        {
            public int Id { get; set; }
            public string ReferenceNo { get; set; }
            public string AirlineName { get; set; }
            public string WeightUnit { get; set; }
            public Nullable<int> CruisePerformanceFactor { get; set; }
            public Nullable<int> ContingencyPercent { get; set; }
            public string FlightNo { get; set; }
            public Nullable<System.DateTime> GenerationDate { get; set; }
            public Nullable<System.DateTime> ScheduledTimeDeparture { get; set; }
            public Nullable<System.DateTime> ScheduledTimeArrival { get; set; }
            public string TailNo { get; set; }
            public string CruiseSpeed { get; set; }
            public Nullable<int> CostIndex { get; set; }
            public Nullable<int> MainFlightLevel { get; set; }
            public Nullable<int> DryOperatingWeight { get; set; }
            public Nullable<int> Payload { get; set; }
            public Nullable<int> GroundDistance { get; set; }
            public Nullable<int> AirDistance { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }
            public string Alternate1 { get; set; }
            public string Alternate2 { get; set; }
            public string TakeoffAlternate { get; set; }
            public Nullable<int> MODAlernate1 { get; set; }
            public Nullable<int> MODAlternate2 { get; set; }
            public Nullable<int> Cockpit { get; set; }
            public Nullable<int> Cabin { get; set; }
            public Nullable<int> Extra { get; set; }
            public Nullable<int> Pantry { get; set; }
            public string Pilot1 { get; set; }
            public string Pilot2 { get; set; }
            public string Dispatcher { get; set; }
            public Nullable<int> OriginElevation { get; set; }
            public Nullable<int> DestinationElevation { get; set; }
            public Nullable<int> Alternate1Elevation { get; set; }
            public Nullable<int> Alternate2Elevation { get; set; }
            public Nullable<int> TakeoffAlternateElevation { get; set; }
            public string MaxShear { get; set; }
            public Nullable<int> MaximumZeroFuelWeight { get; set; }
            public Nullable<int> MaximumTakeoffWeight { get; set; }
            public Nullable<int> MaximumLandingWeight { get; set; }
            public Nullable<int> EstimatedZeroFuelWeight { get; set; }
            public Nullable<int> EstimatedTakeoffWeight { get; set; }
            public Nullable<int> EstimatedLandingWeight { get; set; }
            public string MainRoute { get; set; }
            public string Alternate1Route { get; set; }
            public string Alternate2Route { get; set; }
            public string TakeoffAlternateRoute { get; set; }
            public Nullable<System.DateTime> PlanValidity { get; set; }
            public Nullable<int> FlightID { get; set; }
            public Nullable<System.DateTime> DateCreate { get; set; }
            public Nullable<System.DateTime> DateSign { get; set; }
            public Nullable<int> SignedbyId { get; set; }
            public string SignedbyLICL { get; set; }
            public Nullable<int> RawOFPId { get; set; }
            public string MaxWindShearLevel { get; set; }
            public string MaxWindShearPointName { get; set; }
            public string FlightRule { get; set; }
            public string ICAOFlightPlan { get; set; }
            public Nullable<int> fuel_trip { get; set; }
            public Nullable<int> fuel_alt1 { get; set; }
            public Nullable<int> fuel_alt2 { get; set; }
            public Nullable<int> fuel_alt { get; set; }
            public Nullable<int> fuel_holding { get; set; }
            public Nullable<decimal> fuel_contigency { get; set; }
            public Nullable<int> fuel_taxiout { get; set; }
            public Nullable<int> fuel_taxiin { get; set; }
            public Nullable<int> fuel_min_required { get; set; }
            public Nullable<int> fuel_additional { get; set; }
            public Nullable<int> fuel_extra { get; set; }
            public Nullable<int> fuel_total { get; set; }
            public Nullable<int> fuel_landing { get; set; }
            public Nullable<int> fuel_mod_alt1 { get; set; }
            public Nullable<int> fuel_mod_alt2 { get; set; }
            public Nullable<int> time_trip { get; set; }
            public Nullable<int> time_holding { get; set; }
            public Nullable<int> time_alt { get; set; }
            public Nullable<int> time_alt1 { get; set; }
            public Nullable<int> time_alt2 { get; set; }
            public Nullable<int> time_alt_takeof { get; set; }
            public Nullable<int> time_contigency { get; set; }
            public Nullable<int> time_min_required { get; set; }
            public Nullable<int> time_additional { get; set; }
            public Nullable<int> time_extra { get; set; }
            public Nullable<int> time_total { get; set; }
            public string fuel_extra_due { get; set; }
            public Nullable<int> dis_trip { get; set; }
            public Nullable<int> dis_alt1 { get; set; }
            public Nullable<int> dis_alt2 { get; set; }
            public Nullable<int> dis_alt_takeoff { get; set; }
            public Nullable<int> dis_ground { get; set; }
            public Nullable<int> dis_air { get; set; }
            public string burnoffadj_value { get; set; }
            public string burnoffadj_fuel { get; set; }
            public string heightchange_value { get; set; }
            public string heightchange_fuel { get; set; }
            public Nullable<int> Alternate1FlightLevel { get; set; }
            public Nullable<int> Alternate2FlightLevel { get; set; }
            public string Alternate1WC { get; set; }
            public string Alternate2WC { get; set; }
            public string crew_cockpit { get; set; }
            public string crew_cabin { get; set; }
            public string crew_fsg { get; set; }
            public string crew_fm { get; set; }
            public string crew_dh { get; set; }
            public string fuel_total_corr { get; set; }
            public string fuel_total_actual { get; set; }
            public string fuel_used_actual { get; set; }
            public string fuel_remain_actual { get; set; }
            public string zfw_actual { get; set; }
            public string tow_actual { get; set; }
            public string lgw_actual { get; set; }
            public string dow_actual { get; set; }
            public string clear { get; set; }
            public string atis_dep1 { get; set; }
            public string atis_dep2 { get; set; }
            public string atis_arr { get; set; }
            public string rvsm_gnd_time { get; set; }
            public string rvsm_gnd_lalt { get; set; }
            public string rvsm_gnd_ralt { get; set; }
            public string rvsm_gnd_stby { get; set; }
            public string rvsm_flt1_time { get; set; }
            public string rvsm_flt1_lalt { get; set; }
            public string rvsm_flt1_ralt { get; set; }
            public string rvsm_flt1_stby { get; set; }
            public string rvsm_flt1_fl { get; set; }
            public string rvsm_flt2_time { get; set; }
            public string rvsm_flt2_lalt { get; set; }
            public string rvsm_flt2_ralt { get; set; }
            public string rvsm_flt2_stby { get; set; }
            public string rvsm_flt2_fl { get; set; }
            public string rvsm_flt3_time { get; set; }
            public string rvsm_flt3_lalt { get; set; }
            public string rvsm_flt3_ralt { get; set; }
            public string rvsm_flt3_stby { get; set; }
            public string rvsm_flt3_fl { get; set; }
            public string rvsm_flt4_time { get; set; }
            public string rvsm_flt4_lalt { get; set; }
            public string rvsm_flt4_ralt { get; set; }
            public string rvsm_flt4_stby { get; set; }
            public string rvsm_flt4_fl { get; set; }
            public string atis1 { get; set; }
            public string atis2 { get; set; }
            public string atis3 { get; set; }
            public string atis4 { get; set; }
            public string fuel_trip_corr { get; set; }
            public string fuel_alt1_corr { get; set; }
            public string fuel_alt2_corr { get; set; }
            public string fuel_hld_corr { get; set; }
            public string fuel_res_corr { get; set; }
            public string fuel_cont_corr { get; set; }
            public string fuel_taxi_corr { get; set; }
            public string fuel_req_corr { get; set; }
            public string fuel_xtra_corr { get; set; }
            public string fuel_add_corr { get; set; }
            public Nullable<long> date_update { get; set; }
            public string user_update { get; set; }
            public string pax_male { get; set; }
            public string pax_female { get; set; }
            public string pax_adult { get; set; }
            public string pax_child { get; set; }
            public string pax_infant { get; set; }
            public string sob { get; set; }
            public string cargo { get; set; }
            public Nullable<int> fuel_saved_actual { get; set; }
            public Nullable<int> payload_actual { get; set; }
            
            public string to_v1 { get; set; }
            public string to_vr { get; set; }
            public string to_v2 { get; set; }
            public string to_temp { get; set; }
            public string to_epr { get; set; }
            public string to_flaps { get; set; }
            public string to_stab { get; set; }
            public string to_atis { get; set; }
            public string ldg_vref { get; set; }
            public string ldg_fas { get; set; }
            public string ldg_vf { get; set; }
            public string ldg_flaps { get; set; }
            public string ldg_epr { get; set; }
            public string ldg_atis { get; set; }
            public string to_vf { get; set; }
            public string to_tq { get; set; }
            public string to_n1_redu { get; set; }
            public string to_packs { get; set; }
            public string to_n1_toga { get; set; }
            public string ldg_vapp { get; set; }
            public string ldg_vref15 { get; set; }
            public string ldg_ga_n1 { get; set; }
            public string ldg_ga_alt { get; set; }

            public string MSN { get; set; }
            public string AircraftType { get; set; }
            public string AircraftSubType { get; set; }
            public string ManeuveringTime { get; set; }
            public string ManeuveringFuel { get; set; }
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
            public string firs_main { get; set; }
            public Nullable<int> dis_maingcd { get; set; }


            public string Alternate { get; set; }
            public string AlternateEnroute { get; set; }
            public string DestinationIATA { get; set; }
            public string OriginIATA { get; set; }
            public Nullable<int> TakeoffAlternateFlightLevel { get; set; }
            public Nullable<int> time_final_reserve { get; set; }
            public Nullable<int> time_taxi_in { get; set; }
            public Nullable<int> time_taxi_out { get; set; }
            public string AdditionalStr { get; set; }
            public string AlternateStr { get; set; }
            public string Alternate1Str { get; set; }
            public string Alternate2Str { get; set; }
            public string ContingencyStr { get; set; }
            public string ExtraStr { get; set; }
            public string HoldingStr { get; set; }
            public string MinimumRequiredStr { get; set; }
            public string TakeOffAlternateStr { get; set; }
            public string TaxiInStr { get; set; }
            public string TaxiOutStr { get; set; }
            public string TotalStr { get; set; }
            public string TripStr { get; set; }

            public string Pilot1_FPFM { get; set; }
            public string Pilot2_FPFM { get; set; }
            public Nullable<int> fuel_remain_offblock { get; set; }
            public Nullable<int> fuel_remain_takeoff { get; set; }
            public Nullable<int> fuel_remain_landing { get; set; }
            public Nullable<int> fuel_remain_onblock { get; set; }

        }


        [HttpPost]
        [Route("api/ofp/sync/root")]
        public async Task<DataResponse> SyncRoot(/*int ofpId, string propName, string propValue, string user*/_sync_root dto)
        {
            var context = new Models.dbEntities();
            var keys = dto.items.Select(q => q.Id).ToList();
            var ofp_roots = await context.OFPB_Root.Where(q => keys.Contains(q.Id)).ToListAsync();
            var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            foreach (var x in dto.items)
            {
                var item = ofp_roots.FirstOrDefault(q => q.Id == x.Id);
               
                if (item != null)
                {

                    item.date_update = Convert.ToInt64(dtupd);
                    item.user_update = dto.user;


                    
                  
                    item.fuel_extra_due = x.fuel_extra_due;
                     
                    item.crew_cockpit = x.crew_cockpit;
                    item.crew_cabin = x.crew_cabin;
                    item.crew_fsg = x.crew_fsg;
                    item.crew_fm = x.crew_fm;
                    item.crew_dh = x.crew_dh;
                    item.fuel_total_corr = x.fuel_total_corr;
                    item.fuel_total_actual = x.fuel_total_actual;
                    item.fuel_used_actual = x.fuel_used_actual;
                    item.fuel_remain_actual = x.fuel_remain_actual;
                    item.zfw_actual = x.zfw_actual;
                    item.tow_actual = x.tow_actual;
                    item.lgw_actual = x.lgw_actual;
                    item.clear = x.clear;
                    item.atis_dep1 = x.atis_dep1;
                    item.atis_dep2 = x.atis_dep2;
                    item.atis_arr = x.atis_arr;
                    item.rvsm_gnd_time = x.rvsm_gnd_time;
                    item.rvsm_gnd_lalt = x.rvsm_gnd_lalt;
                    item.rvsm_gnd_ralt = x.rvsm_gnd_ralt;
                    item.rvsm_gnd_stby = x.rvsm_gnd_stby;
                    item.rvsm_flt1_time = x.rvsm_flt1_time;
                    item.rvsm_flt1_lalt = x.rvsm_flt1_lalt;
                    item.rvsm_flt1_ralt = x.rvsm_flt1_ralt;
                    item.rvsm_flt1_stby = x.rvsm_flt1_stby;
                    item.rvsm_flt1_fl = x.rvsm_flt1_fl;
                    item.rvsm_flt2_time = x.rvsm_flt2_time;
                    item.rvsm_flt2_lalt = x.rvsm_flt2_lalt;
                    item.rvsm_flt2_ralt = x.rvsm_flt2_ralt;
                    item.rvsm_flt2_stby = x.rvsm_flt2_stby;
                    item.rvsm_flt2_fl = x.rvsm_flt2_fl;
                    item.rvsm_flt3_time = x.rvsm_flt3_time;
                    item.rvsm_flt3_lalt = x.rvsm_flt3_lalt;
                    item.rvsm_flt3_ralt = x.rvsm_flt3_ralt;
                    item.rvsm_flt3_stby = x.rvsm_flt3_stby;
                    item.rvsm_flt3_fl = x.rvsm_flt3_fl;
                    item.rvsm_flt4_time = x.rvsm_flt4_time;
                    item.rvsm_flt4_lalt = x.rvsm_flt4_lalt;
                    item.rvsm_flt4_ralt = x.rvsm_flt4_ralt;
                    item.rvsm_flt4_stby = x.rvsm_flt4_stby;
                    item.rvsm_flt4_fl = x.rvsm_flt4_fl;
                    item.atis1 = x.atis1;
                    item.atis2 = x.atis2;
                    item.atis3 = x.atis3;
                    item.atis4 = x.atis4;
                    item.fuel_trip_corr = x.fuel_trip_corr;
                    item.fuel_alt1_corr = x.fuel_alt1_corr;
                    item.fuel_alt2_corr = x.fuel_alt2_corr;
                    item.fuel_hld_corr = x.fuel_hld_corr;
                    item.fuel_res_corr = x.fuel_res_corr;
                    item.fuel_cont_corr = x.fuel_cont_corr;
                    item.fuel_taxi_corr = x.fuel_taxi_corr;
                    item.fuel_req_corr = x.fuel_req_corr;
                    item.fuel_xtra_corr = x.fuel_xtra_corr;
                    item.fuel_add_corr = x.fuel_add_corr;
                  
                    item.pax_male = x.pax_male;
                    item.pax_female = x.pax_female;
                    item.pax_adult = x.pax_adult;
                    item.pax_child = x.pax_child;
                    item.pax_infant = x.pax_infant;
                    item.sob = x.sob;
                    item.cargo = x.cargo;
                    item.fuel_saved_actual = x.fuel_saved_actual;
                    item.payload_actual = x.payload_actual;
                    item.dow_actual = x.dow_actual;


                    item.to_v1 = x.to_v1;
                    item.to_vr = x.to_vr;
                    item.to_v2 = x.to_v2;
                    item.to_temp = x.to_temp;
                    item.to_epr = x.to_epr;
                    item.to_flaps = x.to_flaps;
                    item.to_stab = x.to_stab;
                    item.to_atis = x.to_atis;
                    item.ldg_vref = x.ldg_vref;
                    item.ldg_fas = x.ldg_fas;
                    item.ldg_vf = x.ldg_vf;
                    item.ldg_flaps = x.ldg_flaps;
                    item.ldg_epr = x.ldg_epr;
                    item.ldg_atis = x.ldg_atis;
                    item.to_vf = x.to_vf;
                    item.to_tq = x.to_tq;
                    item.to_n1_redu = x.to_n1_redu;
                    item.to_packs = x.to_packs;
                    item.to_n1_toga = x.to_n1_toga;
                    item.ldg_vapp = x.ldg_vapp;
                    item.ldg_vref15 = x.ldg_vref15;
                    item.ldg_ga_n1 = x.ldg_ga_n1;
                    item.ldg_ga_alt = x.ldg_ga_alt;




                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-crew_cockpit", PropertyName = "crew_cockpit", PropValue = item.crew_cockpit != null ? item.crew_cockpit.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-crew_cabin", PropertyName = "crew_cabin", PropValue = item.crew_cabin != null ? item.crew_cabin.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-crew_fsg", PropertyName = "crew_fsg", PropValue = item.crew_fsg != null ? item.crew_fsg.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-crew_fm", PropertyName = "crew_fm", PropValue = item.crew_fm != null ? item.crew_fm.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-crew_dh", PropertyName = "crew_dh", PropValue = item.crew_dh != null ? item.crew_dh.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_total_corr", PropertyName = "fuel_total_corr", PropValue = item.fuel_total_corr != null ? item.fuel_total_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_total_actual", PropertyName = "fuel_total_actual", PropValue = item.fuel_total_actual != null ? item.fuel_total_actual.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_used_actual", PropertyName = "fuel_used_actual", PropValue = item.fuel_used_actual != null ? item.fuel_used_actual.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_remain_actual", PropertyName = "fuel_remain_actual", PropValue = item.fuel_remain_actual != null ? item.fuel_remain_actual.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-zfw_actual", PropertyName = "zfw_actual", PropValue = item.zfw_actual != null ? item.zfw_actual.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-tow_actual", PropertyName = "tow_actual", PropValue = item.tow_actual != null ? item.tow_actual.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-lgw_actual", PropertyName = "lgw_actual", PropValue = item.lgw_actual != null ? item.lgw_actual.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-clear", PropertyName = "clear", PropValue = item.clear != null ? item.clear.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-atis_dep1", PropertyName = "atis_dep1", PropValue = item.atis_dep1 != null ? item.atis_dep1.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-atis_dep2", PropertyName = "atis_dep2", PropValue = item.atis_dep2 != null ? item.atis_dep2.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-atis_arr", PropertyName = "atis_arr", PropValue = item.atis_arr != null ? item.atis_arr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_gnd_time", PropertyName = "rvsm_gnd_time", PropValue = item.rvsm_gnd_time != null ? item.rvsm_gnd_time.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_gnd_lalt", PropertyName = "rvsm_gnd_lalt", PropValue = item.rvsm_gnd_lalt != null ? item.rvsm_gnd_lalt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_gnd_ralt", PropertyName = "rvsm_gnd_ralt", PropValue = item.rvsm_gnd_ralt != null ? item.rvsm_gnd_ralt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_gnd_stby", PropertyName = "rvsm_gnd_stby", PropValue = item.rvsm_gnd_stby != null ? item.rvsm_gnd_stby.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt1_time", PropertyName = "rvsm_flt1_time", PropValue = item.rvsm_flt1_time != null ? item.rvsm_flt1_time.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt1_lalt", PropertyName = "rvsm_flt1_lalt", PropValue = item.rvsm_flt1_lalt != null ? item.rvsm_flt1_lalt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt1_ralt", PropertyName = "rvsm_flt1_ralt", PropValue = item.rvsm_flt1_ralt != null ? item.rvsm_flt1_ralt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt1_stby", PropertyName = "rvsm_flt1_stby", PropValue = item.rvsm_flt1_stby != null ? item.rvsm_flt1_stby.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt1_fl", PropertyName = "rvsm_flt1_fl", PropValue = item.rvsm_flt1_fl != null ? item.rvsm_flt1_fl.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt2_time", PropertyName = "rvsm_flt2_time", PropValue = item.rvsm_flt2_time != null ? item.rvsm_flt2_time.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt2_lalt", PropertyName = "rvsm_flt2_lalt", PropValue = item.rvsm_flt2_lalt != null ? item.rvsm_flt2_lalt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt2_ralt", PropertyName = "rvsm_flt2_ralt", PropValue = item.rvsm_flt2_ralt != null ? item.rvsm_flt2_ralt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt2_stby", PropertyName = "rvsm_flt2_stby", PropValue = item.rvsm_flt2_stby != null ? item.rvsm_flt2_stby.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt2_fl", PropertyName = "rvsm_flt2_fl", PropValue = item.rvsm_flt2_fl != null ? item.rvsm_flt2_fl.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt3_time", PropertyName = "rvsm_flt3_time", PropValue = item.rvsm_flt3_time != null ? item.rvsm_flt3_time.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt3_lalt", PropertyName = "rvsm_flt3_lalt", PropValue = item.rvsm_flt3_lalt != null ? item.rvsm_flt3_lalt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt3_ralt", PropertyName = "rvsm_flt3_ralt", PropValue = item.rvsm_flt3_ralt != null ? item.rvsm_flt3_ralt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt3_stby", PropertyName = "rvsm_flt3_stby", PropValue = item.rvsm_flt3_stby != null ? item.rvsm_flt3_stby.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt3_fl", PropertyName = "rvsm_flt3_fl", PropValue = item.rvsm_flt3_fl != null ? item.rvsm_flt3_fl.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt4_time", PropertyName = "rvsm_flt4_time", PropValue = item.rvsm_flt4_time != null ? item.rvsm_flt4_time.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt4_lalt", PropertyName = "rvsm_flt4_lalt", PropValue = item.rvsm_flt4_lalt != null ? item.rvsm_flt4_lalt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt4_ralt", PropertyName = "rvsm_flt4_ralt", PropValue = item.rvsm_flt4_ralt != null ? item.rvsm_flt4_ralt.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt4_stby", PropertyName = "rvsm_flt4_stby", PropValue = item.rvsm_flt4_stby != null ? item.rvsm_flt4_stby.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-rvsm_flt4_fl", PropertyName = "rvsm_flt4_fl", PropValue = item.rvsm_flt4_fl != null ? item.rvsm_flt4_fl.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-atis1", PropertyName = "atis1", PropValue = item.atis1 != null ? item.atis1.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-atis2", PropertyName = "atis2", PropValue = item.atis2 != null ? item.atis2.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-atis3", PropertyName = "atis3", PropValue = item.atis3 != null ? item.atis3.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-atis4", PropertyName = "atis4", PropValue = item.atis4 != null ? item.atis4.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_trip_corr", PropertyName = "fuel_trip_corr", PropValue = item.fuel_trip_corr != null ? item.fuel_trip_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_alt1_corr", PropertyName = "fuel_alt1_corr", PropValue = item.fuel_alt1_corr != null ? item.fuel_alt1_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_alt2_corr", PropertyName = "fuel_alt2_corr", PropValue = item.fuel_alt2_corr != null ? item.fuel_alt2_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_hld_corr", PropertyName = "fuel_hld_corr", PropValue = item.fuel_hld_corr != null ? item.fuel_hld_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_res_corr", PropertyName = "fuel_res_corr", PropValue = item.fuel_res_corr != null ? item.fuel_res_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_cont_corr", PropertyName = "fuel_cont_corr", PropValue = item.fuel_cont_corr != null ? item.fuel_cont_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_taxi_corr", PropertyName = "fuel_taxi_corr", PropValue = item.fuel_taxi_corr != null ? item.fuel_taxi_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_req_corr", PropertyName = "fuel_req_corr", PropValue = item.fuel_req_corr != null ? item.fuel_req_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_xtra_corr", PropertyName = "fuel_xtra_corr", PropValue = item.fuel_xtra_corr != null ? item.fuel_xtra_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_add_corr", PropertyName = "fuel_add_corr", PropValue = item.fuel_add_corr != null ? item.fuel_add_corr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                     context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-pax_male", PropertyName = "pax_male", PropValue = item.pax_male != null ? item.pax_male.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-pax_female", PropertyName = "pax_female", PropValue = item.pax_female != null ? item.pax_female.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-pax_adult", PropertyName = "pax_adult", PropValue = item.pax_adult != null ? item.pax_adult.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-pax_child", PropertyName = "pax_child", PropValue = item.pax_child != null ? item.pax_child.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-pax_infant", PropertyName = "pax_infant", PropValue = item.pax_infant != null ? item.pax_infant.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-sob", PropertyName = "sob", PropValue = item.sob != null ? item.sob.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-cargo", PropertyName = "cargo", PropValue = item.cargo != null ? item.cargo.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-fuel_saved_actual", PropertyName = "fuel_saved_actual", PropValue = item.fuel_saved_actual != null ? item.fuel_saved_actual.ToString() : null, DateUpdate = dtupd, User = dto.user });

                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-payload_actual", PropertyName = "payload_actual", PropValue = item.payload_actual != null ? item.payload_actual.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-dow_actual", PropertyName = "dow_actual", PropValue = item.dow_actual != null ? item.dow_actual.ToString() : null, DateUpdate = dtupd, User = dto.user });

                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_v1", PropertyName = "to_v1", PropValue = item.to_v1 != null ? item.to_v1.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_vr", PropertyName = "to_vr", PropValue = item.to_vr != null ? item.to_vr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_v2", PropertyName = "to_v2", PropValue = item.to_v2 != null ? item.to_v2.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_temp", PropertyName = "to_temp", PropValue = item.to_temp != null ? item.to_temp.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_epr", PropertyName = "to_epr", PropValue = item.to_epr != null ? item.to_epr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_flaps", PropertyName = "to_flaps", PropValue = item.to_flaps != null ? item.to_flaps.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_stab", PropertyName = "to_stab", PropValue = item.to_stab != null ? item.to_stab.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_atis", PropertyName = "to_atis", PropValue = item.to_atis != null ? item.to_atis.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_vref", PropertyName = "ldg_vref", PropValue = item.ldg_vref != null ? item.ldg_vref.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_fas", PropertyName = "ldg_fas", PropValue = item.ldg_fas != null ? item.ldg_fas.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_vf", PropertyName = "ldg_vf", PropValue = item.ldg_vf != null ? item.ldg_vf.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_flaps", PropertyName = "ldg_flaps", PropValue = item.ldg_flaps != null ? item.ldg_flaps.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_epr", PropertyName = "ldg_epr", PropValue = item.ldg_epr != null ? item.ldg_epr.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_atis", PropertyName = "ldg_atis", PropValue = item.ldg_atis != null ? item.ldg_atis.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_vf", PropertyName = "to_vf", PropValue = item.to_vf != null ? item.to_vf.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_tq", PropertyName = "to_tq", PropValue = item.to_tq != null ? item.to_tq.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_n1_redu", PropertyName = "to_n1_redu", PropValue = item.to_n1_redu != null ? item.to_n1_redu.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_packs", PropertyName = "to_packs", PropValue = item.to_packs != null ? item.to_packs.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-to_n1_toga", PropertyName = "to_n1_toga", PropValue = item.to_n1_toga != null ? item.to_n1_toga.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_vapp", PropertyName = "ldg_vapp", PropValue = item.ldg_vapp != null ? item.ldg_vapp.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_vref15", PropertyName = "ldg_vref15", PropValue = item.ldg_vref15 != null ? item.ldg_vref15.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_ga_n1", PropertyName = "ldg_ga_n1", PropValue = item.ldg_ga_n1 != null ? item.ldg_ga_n1.ToString() : null, DateUpdate = dtupd, User = dto.user });
                    context.OFPB_Prop.Add(new OFPB_Prop() { OFPId = item.Id, PropName = "prop-root-ldg_ga_alt", PropertyName = "ldg_ga_alt", PropValue = item.ldg_ga_alt != null ? item.ldg_ga_alt.ToString() : null, DateUpdate = dtupd, User = dto.user });


                }


            }

            var saveResult = await context.SaveChangesAsync();
            return new DataResponse() { IsSuccess = true, Data = new { date_update = dtupd } };



        }

        [HttpPost]
        [Route("api/ofp/sync/nav")]
        public async Task<DataResponse> SyncNavLog(/*int ofpId, string propName, string propValue, string user*/_sync_navlog2 dto)
        {
            var context = new Models.dbEntities();
            var keys = dto.items.Select(q => q.id).ToList();
            var ofp_navs = await context.OFPB_MainNavLog.Where(q => keys.Contains(q.Id)).ToListAsync();
            var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            foreach (var x in dto.items)
            {
                var item = ofp_navs.FirstOrDefault(q => q.Id == x.id);
                if (item != null)
                {
                    switch (x.key)
                    {
                        case "ATA":
                            item.ATA = x.value;
                            context.OFPB_Prop.Add(new OFPB_Prop()
                            {
                                OFPId = (int)item.RootId,

                                PropName = "prop-nav-ATA_" + item.Id,
                                TableName = "OFPB_MainNavLog",
                                PropertyName = "ATA",
                                PropValue = x.value,
                                DateUpdate = x.date_update,
                                User = dto.user


                            });
                            break;
                        case "FuelRemainedActual":
                            item.FuelRemainedActual =string.IsNullOrEmpty(x.value)?null:(Nullable<int>) Convert.ToInt32( x.value);
                            context.OFPB_Prop.Add(new OFPB_Prop()
                            {
                                OFPId = (int)item.RootId,

                                PropName = "prop-nav-FuelRemainedActual_" + item.Id,
                                TableName = "OFPB_MainNavLog",
                                PropertyName = "FuelRemainedActual",
                                PropValue = x.value,
                                DateUpdate = x.date_update,
                                User = dto.user


                            });
                            break;
                        case "FuelUsedActual":
                            item.FuelUsedActual = string.IsNullOrEmpty(x.value) ? null : (Nullable<int>)Convert.ToInt32(x.value);
                            context.OFPB_Prop.Add(new OFPB_Prop()
                            {
                                OFPId = (int)item.RootId,

                                PropName = "prop-nav-FuelUsedActual_" + item.Id,
                                TableName = "OFPB_MainNavLog",
                                PropertyName = "FuelUsedActual",
                                PropValue = x.value,
                                DateUpdate = x.date_update,
                                User = dto.user


                            });
                            break;
                        default:
                            break;
                    }
                    //item.date_update = Convert.ToInt64(dtupd);
                    //item.user_update = dto.user;

                    //item.ETA = x.eta;
                    //var prop1 = new OFPB_Prop()
                    //{
                    //    OFPId = (int)item.RootId,

                    //    PropName = "prop-nav-ETA_" + item.Id,
                    //    TableName = "OFPB_MainNavLog",
                    //    PropertyName = "ETA",
                    //    PropValue = x.eta,
                    //    DateUpdate = dtupd,
                    //    User = dto.user


                    //};
                    //context.OFPB_Prop.Add(prop1);

                    //item.ATA = x.ata;
                    //var prop2 = new OFPB_Prop()
                    //{
                    //    OFPId = (int)item.RootId,

                    //    PropName = "prop-nav-ATA_" + item.Id,
                    //    TableName = "OFPB_MainNavLog",
                    //    PropertyName = "ATA",
                    //    PropValue = x.ata,
                    //    DateUpdate = dtupd,
                    //    User = dto.user


                    //};
                    //context.OFPB_Prop.Add(prop2);


                    //item.FuelRemainedActual = x.remained;
                    //var prop3 = new OFPB_Prop()
                    //{
                    //    OFPId = (int)item.RootId,

                    //    PropName = "prop-nav-FuelRemainedActual_" + item.Id,
                    //    TableName = "OFPB_MainNavLog",
                    //    PropertyName = "FuelRemainedActual",
                    //    PropValue = x.remained == null ? "" : x.remained.ToString(),
                    //    DateUpdate = dtupd,
                    //    User = dto.user


                    //};
                    //context.OFPB_Prop.Add(prop3);

                    //item.FuelUsedActual = x.used;
                    //var prop4 = new OFPB_Prop()
                    //{
                    //    OFPId = (int)item.RootId,

                    //    PropName = "prop-nav-FuelUsedActual_" + item.Id,
                    //    TableName = "OFPB_MainNavLog",
                    //    PropertyName = "FuelUsedActual",
                    //    PropValue = x.used == null ? "" : x.used.ToString(),
                    //    DateUpdate = dtupd,
                    //    User = dto.user


                    //};
                    //context.OFPB_Prop.Add(prop4);






                    //item.FuelRemained2 = x.remained2;
                    //var prop5 = new OFPB_Prop()
                    //{
                    //    OFPId = (int)item.RootId,

                    //    PropName = "prop-nav-FuelRemained2_" + item.Id,
                    //    TableName = "OFPB_MainNavLog",
                    //    PropertyName = "FuelRemained2",
                    //    PropValue = x.remained2 == null ? "" : x.remained2.ToString(),
                    //    DateUpdate = dtupd,
                    //    User = dto.user


                    //};
                    //context.OFPB_Prop.Add(prop5);

                    //item.FuelUsed2 = x.used2;
                    //var prop6 = new OFPB_Prop()
                    //{
                    //    OFPId = (int)item.RootId,

                    //    PropName = "prop-nav-FuelUsed2_" + item.Id,
                    //    TableName = "OFPB_MainNavLog",
                    //    PropertyName = "FuelUsed2",
                    //    PropValue = x.used2 == null ? "" : x.used2.ToString(),
                    //    DateUpdate = dtupd,
                    //    User = dto.user


                    //};
                    //context.OFPB_Prop.Add(prop6);

                }


            }

            var saveResult = await context.SaveChangesAsync();
            return new DataResponse() { IsSuccess = true, Data = new { date_update = dtupd } };



        }
        [HttpPost]
        [Route("api/ofp/sync/nav/old")]
        public async Task<DataResponse> SyncNavLogOLD(/*int ofpId, string propName, string propValue, string user*/_sync_navlog dto)
        {
            var context = new Models.dbEntities();
            var keys = dto.items.Select(q => q.id).ToList();
            var ofp_navs = await context.OFPB_MainNavLog.Where(q => keys.Contains(q.Id)).ToListAsync();
            var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            foreach (var x in dto.items)
            {
                var item = ofp_navs.FirstOrDefault(q => q.Id == x.id);
                if (item != null)
                {
                    
                    item.date_update = Convert.ToInt64(dtupd);
                    item.user_update = dto.user;

                    item.ETA = x.eta;
                    var prop1 = new OFPB_Prop()
                    {
                        OFPId = (int)item.RootId,

                        PropName = "prop-nav-ETA_" + item.Id,
                        TableName = "OFPB_MainNavLog",
                        PropertyName = "ETA",
                        PropValue = x.eta,
                        DateUpdate = dtupd,
                        User = dto.user


                    };
                    context.OFPB_Prop.Add(prop1);

                    item.ATA = x.ata;
                    var prop2 = new OFPB_Prop()
                    {
                        OFPId = (int)item.RootId,

                        PropName = "prop-nav-ATA_" + item.Id,
                        TableName = "OFPB_MainNavLog",
                        PropertyName = "ATA",
                        PropValue = x.ata,
                        DateUpdate = dtupd,
                        User = dto.user


                    };
                    context.OFPB_Prop.Add(prop2);


                    item.FuelRemainedActual = x.remained;
                    var prop3 = new OFPB_Prop()
                    {
                        OFPId = (int)item.RootId,

                        PropName = "prop-nav-FuelRemainedActual_" + item.Id,
                        TableName = "OFPB_MainNavLog",
                        PropertyName = "FuelRemainedActual",
                        PropValue = x.remained==null?"": x.remained.ToString(),
                        DateUpdate = dtupd,
                        User = dto.user


                    };
                    context.OFPB_Prop.Add(prop3);

                    item.FuelUsedActual = x.used;
                    var prop4 = new OFPB_Prop()
                    {
                        OFPId = (int)item.RootId,

                        PropName = "prop-nav-FuelUsedActual_" + item.Id,
                        TableName = "OFPB_MainNavLog",
                        PropertyName = "FuelUsedActual",
                        PropValue = x.used == null ? "" : x.used.ToString(),
                        DateUpdate = dtupd,
                        User = dto.user


                    };
                    context.OFPB_Prop.Add(prop4);






                    item.FuelRemained2 = x.remained2;
                    var prop5 = new OFPB_Prop()
                    {
                        OFPId = (int)item.RootId,

                        PropName = "prop-nav-FuelRemained2_" + item.Id,
                        TableName = "OFPB_MainNavLog",
                        PropertyName = "FuelRemained2",
                        PropValue = x.remained2 == null ? "" : x.remained2.ToString(),
                        DateUpdate = dtupd,
                        User = dto.user


                    };
                    context.OFPB_Prop.Add(prop5);

                    item.FuelUsed2 = x.used2;
                    var prop6 = new OFPB_Prop()
                    {
                        OFPId = (int)item.RootId,

                        PropName = "prop-nav-FuelUsed2_" + item.Id,
                        TableName = "OFPB_MainNavLog",
                        PropertyName = "FuelUsed2",
                        PropValue = x.used2 == null ? "" : x.used2.ToString(),
                        DateUpdate = dtupd,
                        User = dto.user


                    };
                    context.OFPB_Prop.Add(prop6);

                }


            }

            var saveResult = await context.SaveChangesAsync();
            return new DataResponse() { IsSuccess = true, Data = new { date_update = dtupd } };



        }
        [HttpPost]
        [Route("api/ofp/update/eta/bulk")]
        public async Task<DataResponse> UpdateOFPETABULK(/*int ofpId, string propName, string propValue, string user*/_eta_bulk_dto dto)
        {
            var context = new Models.dbEntities();
            var keys = dto.items.Select(q => q.key).ToList();
            var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var ofp_navs = await context.OFPB_MainNavLog.Where(q => keys.Contains(q.Id)).ToListAsync();
            foreach (var x in dto.items)
            {
                var item = ofp_navs.FirstOrDefault(q => q.Id == x.key);
                if (item != null)
                {
                    item.ETA = x.changes.ETA;
                    item.date_update = Convert.ToInt64(dtupd);
                    item.user_update = dto.user;

                    var prop = new OFPB_Prop()
                    {
                        OFPId = (int)item.RootId,

                        PropName = "prop-nav-ETA_" + item.Id,
                        TableName = "OFPB_MainNavLog",
                        PropertyName = "ETA",
                        PropValue = x.changes.ETA,
                        DateUpdate = dtupd,
                        User = dto.user


                    };
                    context.OFPB_Prop.Add(prop);


                }
            }

            var saveResult = await context.SaveChangesAsync();
            return new DataResponse() { IsSuccess = true, Data = new { date_update = dtupd } };


        }




        [Route("api/ofp/update/fuel/bulk")]
        public async Task<DataResponse> UpdateOFPFUELBULK(/*int ofpId, string propName, string propValue, string user*/__fuel_bulk_dto dto)
        {
            var context = new Models.dbEntities();
            var keys = dto.items.Select(q => q.key).ToList();
            var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var ofp_navs = await context.OFPB_MainNavLog.Where(q => keys.Contains(q.Id)).ToListAsync();
            foreach (var x in dto.items)
            {
                var item = ofp_navs.FirstOrDefault(q => q.Id == x.key);
                if (item != null)
                {
                    
                    if (x.changes.IsFuelUsed2)
                    {
                        item.FuelUsed2 = x.changes.FuelUsed2;
                        var prop = new OFPB_Prop()
                        {
                            OFPId = (int)item.RootId,

                            PropName = "prop-nav-FuelUsed2_" + item.Id,
                            TableName = "OFPB_MainNavLog",
                            PropertyName = "FuelUsed2",
                            PropValue = x.changes.FuelUsed2!=null? x.changes.FuelUsed2.ToString():null,
                            DateUpdate = dtupd,
                            User = dto.user


                        };
                        context.OFPB_Prop.Add(prop);

                    }
                    if (x.changes.IsFuelRemained2)
                    {
                        item.FuelRemained2 = x.changes.FuelRemained2;
                        var prop = new OFPB_Prop()
                        {
                            OFPId = (int)item.RootId,

                            PropName = "prop-nav-FuelRemained2_" + item.Id,
                            TableName = "OFPB_MainNavLog",
                            PropertyName = "FuelRemained2",
                            PropValue = x.changes.FuelRemained2 != null ? x.changes.FuelRemained2.ToString() : null,
                            DateUpdate = dtupd,
                            User = dto.user


                        };
                        context.OFPB_Prop.Add(prop);
                    }
                    if (x.changes.IsFuelUsedActual)
                    {
                        item.FuelUsed2 = x.changes.FuelUsedActual;
                        var prop = new OFPB_Prop()
                        {
                            OFPId = (int)item.RootId,

                            PropName = "prop-nav-FuelUsedActual_" + item.Id,
                            TableName = "OFPB_MainNavLog",
                            PropertyName = "FuelUsedActual",
                            PropValue = x.changes.FuelUsedActual != null ? x.changes.FuelUsedActual.ToString() : null,
                            DateUpdate = dtupd,
                            User = dto.user


                        };
                        context.OFPB_Prop.Add(prop);
                    }
                    item.date_update = Convert.ToInt64(dtupd);
                    item.user_update = dto.user;

                    
                   


                }
            }

            var saveResult = await context.SaveChangesAsync();
            return new DataResponse() { IsSuccess = true, Data = new { date_update = dtupd } };


        }

        public class ofp_update_dto
        {
            public int? OFPId { get; set; }
            public string PropName { get; set; }
            public string PropValue { get; set; }
            public string User { get; set; }
            public string date_update { get; set; }

        }
        [HttpPost]
        [Route("api/ofp/update/sync")]
        public async Task<DataResponse> UpdateOFPSync(/*int ofpId, string propName, string propValue, string user*/List<ofp_update_dto> dtos)
        {
            try
            {
                var context = new Models.dbEntities();
                var ofpids = dtos.Select(q => (int)q.OFPId).ToList();
                var ofp_roots = await context.OFPB_Root.Where(q => ofpids.Contains(q.Id)).ToListAsync();

                foreach (var dto in  dtos)
                {
                    var ofp_root = ofp_roots.FirstOrDefault(q => q.Id == dto.OFPId);
                    if (ofp_root != null)
                    {
                        int ofpId = Convert.ToInt32(dto.OFPId);
                        string propName = Convert.ToString(dto.PropName);
                        string propValue = Convert.ToString(dto.PropValue);
                        string user = Convert.ToString(dto.User);
                        Int64 date_update = Convert.ToInt64(dto.date_update);


                        var property = propName.Split('-')[2];


                        var prop = new OFPB_Prop()
                        {
                            OFPId = ofpId,

                            PropName = propName,
                            TableName = "OFPB_Root",
                            PropertyName = property,
                            DateUpdate = date_update.ToString()


                        };


                        //prop.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                        if (!string.IsNullOrEmpty(propValue.Trim().Replace(" ", "")))
                            prop.PropValue = propValue;
                        prop.User = user;
                        context.OFPB_Prop.Add(prop);


                        Type type = ofp_root.GetType();

                        PropertyInfo propinfo = type.GetProperty(property);
                        if (string.IsNullOrEmpty(propValue))
                        {
                            propinfo.SetValue(ofp_root, null, null);
                        }
                        else
                        {
                            var val = ChangeType(propValue, propinfo.PropertyType);

                            propinfo.SetValue(ofp_root, val, null);
                        }
                        ofp_root.date_update = date_update;//Convert.ToInt64(prop.DateUpdate);
                        ofp_root.user_update = user;
                    }

                }

                var saveResult = await context.SaveChangesAsync();
                return new DataResponse() { IsSuccess = true, Data = new { date_update=0 } };

 


            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                if (ex.InnerException != null)
                    mes += "    " + ex.InnerException.Message;
                return new DataResponse() { IsSuccess = false, Errors = new List<string>() { mes } };
            }




        }

        [HttpPost]
        [Route("api/ofp/update")]
        public async Task<DataResponse> UpdateOFP(/*int ofpId, string propName, string propValue, string user*/dynamic dto)
        {
            try
            {
                int ofpId = Convert.ToInt32(dto.OFPId);
                string propName = Convert.ToString(dto.PropName);
                string propValue = Convert.ToString(dto.PropValue);
                string user = Convert.ToString(dto.User);
                Int64 date_update = Convert.ToInt64(dto.date_update);
                var context = new Models.dbEntities();

                var property = propName.Split('-')[2];

                var tbl_part = propName.Split('-')[1];
                string table = "";
                switch (tbl_part)
                {
                    case "root":
                        table = "OFPB_Root";
                        break;
                    case "nav":
                        table = "OFPB_MainNavLog";
                        break;
                    default:
                        break;
                }


                var prop = new OFPB_Prop()
                {
                    OFPId = ofpId,

                    PropName = propName,
                    TableName = table,
                    PropertyName = property,
                     DateUpdate= date_update.ToString()


                };


                //prop.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                if (!string.IsNullOrEmpty(propValue.Trim().Replace(" ", "")))
                    prop.PropValue = propValue;
                prop.User = user;
                context.OFPB_Prop.Add(prop);

                if (table == "OFPB_MainNavLog")
                {
                    int propId = Convert.ToInt32(property.Split('_').Last());
                    property = property.Split('_')[0];
                    var ofp_nav = await context.OFPB_MainNavLog.FirstOrDefaultAsync(q => q.Id == propId);
                    if (ofp_nav != null)
                    {
                        Type type = ofp_nav.GetType();

                        PropertyInfo propinfo = type.GetProperty(property);
                        if (string.IsNullOrEmpty(propValue))
                        {
                            propinfo.SetValue(ofp_nav, null, null);
                        }
                        else
                        {
                            var val = ChangeType(propValue, propinfo.PropertyType);

                            propinfo.SetValue(ofp_nav, val, null);
                        }
                        ofp_nav.date_update = date_update; //Convert.ToInt64(prop.DateUpdate);
                        ofp_nav.user_update = user;
                    }
                    var saveResult = await context.SaveChangesAsync();
                    return new DataResponse() { IsSuccess = true, Data = new { ofp_nav.date_update } };

                }
                if (table == "OFPB_Root")
                {
                    var ofp_root = await context.OFPB_Root.FirstOrDefaultAsync(q => q.Id == ofpId);
                    if (ofp_root != null)
                    {
                        Type type = ofp_root.GetType();

                        PropertyInfo propinfo = type.GetProperty(property);
                        if (string.IsNullOrEmpty(propValue))
                        {
                            propinfo.SetValue(ofp_root, null, null);
                        }
                        else
                        {
                            var val = ChangeType(propValue, propinfo.PropertyType);

                            propinfo.SetValue(ofp_root, val, null);
                        }
                        ofp_root.date_update = date_update;//Convert.ToInt64(prop.DateUpdate);
                        ofp_root.user_update = user;
                    }
                    var saveResult = await context.SaveChangesAsync();
                    return new DataResponse() { IsSuccess = true, Data = new { ofp_root.date_update } };

                }

                return new DataResponse() { IsSuccess = false, Messages = new List<string>() { "table not found" } };


            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                if (ex.InnerException != null)
                    mes += "    " + ex.InnerException.Message;
                return new DataResponse() { IsSuccess = false, Errors = new List<string>() { mes } };
            }




        }


        [HttpGet]
        [Route("api/ofp/sign/check/{ofpid}")]
        public IHttpActionResult GetOFPSignCheck(int ofpid)
        {
            var context = new Models.dbEntities();
            var result = context.view_ofpb_root_report.FirstOrDefault(q => q.Id == ofpid);

            if (result == null)
                return Ok(new
                {
                    Data = new { Id = -1 },
                    IsSuccess = true
                });


            return Ok(new
            {
                Data = new
                {
                    result.Id,
                    result.FlightID,
                    result.JLSignedBy,
                    result.JLDatePICApproved,
                    result.PIC,
                    result.PICId,
                    result.LicNo
                },
                IsSuccess = true
            });
        }


        [HttpGet]
        [Route("api/flight/commanders/{fid}")]
        public IHttpActionResult GetFlightCommanders(int fid)
        {
            var context = new Models.dbEntities();
            var crews = context.XFlightCrews.Where(q => q.FlightId == fid && q.IsPositioning == false
              && (q.Position.ToLower() == "captain" || q.Position.ToLower() == "cpt" || q.Position.ToLower() == "ip"))
                .OrderBy(q => q.GroupOrder)
                .ToList();
 
            return Ok(new
            {
                Data = new
                {
                    commander = crews.FirstOrDefault(),
                    commanders = crews,
                },
               
                IsSuccess = true
            });
        }


    }


}

public class ofpb
{
    public int? flight_id { get; set; }
    public int? ofp_id { get; set; }
    public view_ofpb_root root { get; set; }
    public List<view_ofpb_navlog> main_route { get; set; }
    public List<view_ofpb_navlog> alt1_route { get; set; }
    public List<view_ofpb_navlog> alt2_route { get; set; }
    public List<view_ofpb_wt> main_wts { get; set; }
    public List<view_ofpb_wt> alt1_wts { get; set; }
    public List<view_ofpb_wt> alt2_wts { get; set; }
    public List<view_ofpb_wt> to_wts { get; set; }
    public List<view_ofpb_navlog> routes { get; set; }
    public List<view_ofpb_wt> wts { get; set; }

}
public class DataResponse
{
    public bool IsSuccess { get; set; }
    public object Data { get; set; }
    public List<string> Errors { get; set; }
    public List<string> Messages { get; set; }
}

public class SimpleDto
{
    public List<int?> ids { get; set; }
    public string type { get; set; }
}

