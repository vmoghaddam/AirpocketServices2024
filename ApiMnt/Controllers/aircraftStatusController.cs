﻿using ApiMnt.Models;
using ApiMnt.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiMnt.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class aircraftStatusController : ApiController
    {

        [Route("api/mnt/aircraft/status")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostAircraftStatus(aircraft_status dto)
        {

            ppa_entities context = new ppa_entities();
            try
            {
                var aircraft = await context.Ac_MSN.Where(q => q.ID == dto.ID).FirstOrDefaultAsync();

                aircraft.deffects_no = dto.deffects_no;
                aircraft.landing_gear_remaining = dto.landing_gear_remaining;
                aircraft.landing_gear_ldg_remaining = dto.landing_gear_ldg_remaining;
                aircraft.apu_remaining = dto.apu_remaining;
                aircraft.ht1_remaining = dto.ht1_remaining;
                aircraft.ht2_remaining = dto.ht2_remaining;
                aircraft.ht3_remaining = dto.ht3_remaining;
                aircraft.first_due_remaining = dto.first_due_remaining;
                aircraft.total_flight_cycle = dto.total_flight_cycle;
                aircraft.total_flight_minute = dto.total_flight_minute;
                aircraft.maintenance_setting_group = dto.maintenance_setting_group;
                aircraft.date_initial = dto.date_initial == null ? null : str_to_date(dto.date_initial);
                aircraft.date_initial_apu = dto.date_initial_apu == null ? null : str_to_date(dto.date_initial_apu);
                aircraft.date_initial_due = dto.date_initial_due == null ? null : str_to_date(dto.date_initial_due);
                aircraft.date_initial_ht1 = dto.date_initial_ht1 == null ? null : str_to_date(dto.date_initial_ht1);
                aircraft.date_initial_ht2 = dto.date_initial_ht2 == null ? null : str_to_date(dto.date_initial_ht2);
                aircraft.date_initial_ht3 = dto.date_initial_ht3 == null ? null : str_to_date(dto.date_initial_ht3);
                aircraft.date_initial_landing_gear = dto.date_initial_landing_gear == null ? null : str_to_date(dto.date_initial_landing_gear);


                await context.SaveChangesAsync();

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }


        public DateTime? str_to_date(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            var prts = str.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var dt = new DateTime(prts[0], prts[1], prts[2]);
            return dt;
        }
        public class dto_id
        {
            public int id { get; set; }
        }
        public class aircraft_status
        {
            public int ID { get; set; }
            public int deffects_no { get; set; }
            // public int eng1_remaining { get; set; }
            // public int eng2_remaining { get; set; }
            public int landing_gear_remaining { get; set; }
            public int landing_gear_ldg_remaining { get; set; }
            public int apu_remaining { get; set; }
            public int ht1_remaining { get; set; }
            public int ht2_remaining { get; set; }
            public int ht3_remaining { get; set; }
            public int first_due_remaining { get; set; }
            public int total_flight_cycle { get; set; }
            public int total_flight_minute { get; set; }
            public string serial_no { get; set; }
            public string date_initial { get; set; }
            public string maintenance_setting_group { get; set; }
            public string date_initial_landing_gear { get; set; }
            public string date_initial_apu { get; set; }
            public string date_initial_ht1 { get; set; }
            public string date_initial_ht2 { get; set; }
            public string date_initial_ht3 { get; set; }
            public string date_initial_due { get; set; }

        }


        public class aircraft_check
        {
            public int id { get; set; }
            public int item_no { get; set; }
            public string check { get; set; }
            public int remaining_hours { get; set; }
            public int remaining_minutes { get; set; }
            public int estimated_working_days { get; set; }
            public int aircraft_id { get; set; }
            public string date_initial { get; set; }
            public string tasks { get; set; }

        }
        [Route("api/mnt/aircraft/check/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostAircraftCheck(aircraft_check dto)
        {
            ppa_entities context = new ppa_entities();
            var check = await context.mnt_aircraft_check.FirstOrDefaultAsync(q => q.id == dto.id);
            if (check == null)
            {
                check = new mnt_aircraft_check() { id = -1 };
                context.mnt_aircraft_check.Add(check);
            }
            check.item_no = dto.item_no;
            check.check = dto.check;
            check.remaining_minutes = dto.remaining_minutes;
            check.remaining_hours = dto.remaining_hours;
            check.estimated_working_days = dto.estimated_working_days;
            check.aircraft_id = dto.aircraft_id;
            check.date_initial = dto.date_initial == null ? null : str_to_date(dto.date_initial);
            check.tasks = dto.tasks;

            await context.SaveChangesAsync();

            return Ok(dto);
        }

        [Route("api/mnt/aircraft/check/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostAcCheckDel(dto_id dto)
        {
            ppa_entities context = new ppa_entities();
            var obj = await context.mnt_aircraft_check.FirstOrDefaultAsync(q => q.id == dto.id);
            if (obj != null)
            {
                context.mnt_aircraft_check.Remove(obj);
                await context.SaveChangesAsync();
            }
            return Ok(true);
        }

        public class aircraft_adsb
        {
            public int id { get; set; }
            public string reference { get; set; }
            public string subject { get; set; }
            public int estimated_working_days { get; set; }
            public string date_due { get; set; }
            public int aircraft_id { get; set; }
            public int remainig_cycles { get; set; }
            public string date_initial { get; set; }

        }

        [Route("api/mnt/aircraft/adsb/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostAircraftADSB(aircraft_adsb dto)
        {
            ppa_entities context = new ppa_entities();
            var adsb = await context.mnt_aircraft_adsb.FirstOrDefaultAsync(q => q.id == dto.id);
            if (adsb == null)
            {
                adsb = new mnt_aircraft_adsb() { id = -1 };
                context.mnt_aircraft_adsb.Add(adsb);
            }
            adsb.reference = dto.reference;
            adsb.subject = dto.subject;
            adsb.estimated_working_days = dto.estimated_working_days;
            adsb.date_due = str_to_date(dto.date_due);
            adsb.aircraft_id = dto.aircraft_id;
            adsb.remainig_cycles = dto.remainig_cycles;
            adsb.date_initial = dto.date_initial == null ? null : str_to_date(dto.date_initial);

            await context.SaveChangesAsync();

            return Ok(dto);
        }


        [Route("api/mnt/aircraft/adsb/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostAcADSBkDel(dto_id dto)
        {
            ppa_entities context = new ppa_entities();
            var obj = await context.mnt_aircraft_adsb.FirstOrDefaultAsync(q => q.id == dto.id);
            if (obj != null)
            {
                context.mnt_aircraft_adsb.Remove(obj);
                await context.SaveChangesAsync();
            }
            return Ok(true);
        }




        public class engine_status
        {
            public int id { get; set; }
            public int engine_no { get; set; }
            public string cat { get; set; }
            public string model { get; set; }
            public string serial_no { get; set; }
            public int remaining_cycles { get; set; }
            public int remaining_minutes { get; set; }
            public int total_flight_hour { get; set; }
            public int total_flight_cycle { get; set; }
            public int remaining_hour { get; set; }
            public int remaining_min { get; set; }
            public int aircraft_id { get; set; }
            public string date_initial { get; set; }


        }
        [Route("api/mnt/engine/status")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEngineStatus(engine_status dto)
        {
            int t = (dto.remaining_hour * 60) + dto.remaining_min;
            ppa_entities context = new ppa_entities();
            var engine = await context.mnt_engine.Where(q => q.id == dto.id).FirstOrDefaultAsync();
            engine.engine_no = dto.engine_no;
            engine.aircraft_id = dto.aircraft_id;
            engine.cat = dto.cat;
            engine.model = dto.model;
            engine.serial_no = dto.serial_no;
            engine.date_initial = dto.date_initial == null ? null : str_to_date(dto.date_initial);
            engine.remaining_cycles = dto.remaining_cycles;
            engine.remaining_minutes = t;
            engine.total_flight_hour = dto.total_flight_hour;
            engine.total_flight_cycle = dto.total_flight_cycle;

            await context.SaveChangesAsync();
            return Ok(dto);
        }
        public class engine_llp
        {
            public int id { get; set; }
            public int engine_id { get; set; }
            public string cat { get; set; }
            public string cat_a { get; set; }
            public string cat_b { get; set; }
            public string cat_c { get; set; }
            public int remaining_cycles { get; set; }
            public string date_initial { get; set; }
            public string date_due { get; set; }
            public string title { get; set; }

        }
        [Route("api/mnt/engine/llp/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEngineLLP(engine_llp dto)
        {
            ppa_entities context = new ppa_entities();
            var engine_llp = await context.mnt_engine_llp.Where(q => q.id == dto.id).FirstOrDefaultAsync();
            if (engine_llp == null)
            {
                engine_llp = new mnt_engine_llp();
                engine_llp.id = -1;
                context.mnt_engine_llp.Add(engine_llp);
            }
            engine_llp.engine_id = dto.engine_id;
            engine_llp.cat = dto.cat;
            engine_llp.cat_a = dto.cat_a;
            engine_llp.cat_b = dto.cat_b;
            engine_llp.cat_c = dto.cat_c;
            engine_llp.title = dto.title;
            engine_llp.remaining_cycles = dto.remaining_cycles;
            engine_llp.date_initial = dto.date_initial == null ? null : str_to_date(dto.date_initial);
            engine_llp.date_due = dto.date_due == null ? null : str_to_date(dto.date_due);



            await context.SaveChangesAsync();
            return Ok(dto);
        }

        [Route("api/mnt/engine/llp/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEngineLLPDel(dto_id dto)
        {
            ppa_entities context = new ppa_entities();
            var obj = await context.mnt_engine_llp.FirstOrDefaultAsync(q => q.id == dto.id);
            if (obj != null)
            {
                context.mnt_engine_llp.Remove(obj);
                await context.SaveChangesAsync();
            }
            return Ok(true);
        }


        public class engine_adsb
        {
            public int id { get; set; }
            public int item_no { get; set; }
            public string reference { get; set; }
            public string subject { get; set; }
            public string remark { get; set; }
            public string date_due { get; set; }
            public int remaining_cycles { get; set; }
            public int engine_id { get; set; }
            public string date_initial { get; set; }

        }
        [Route("api/mnt/engine/adsb/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEngineADSB(engine_adsb dto)
        {
            ppa_entities context = new ppa_entities();
            var engine_adsb = await context.mnt_engine_adsb.Where(q => q.id == dto.id).FirstOrDefaultAsync();
            if (engine_adsb == null)
            {
                engine_adsb = new mnt_engine_adsb();
                engine_adsb.id = -1;
                context.mnt_engine_adsb.Add(engine_adsb);
            }
            engine_adsb.engine_id = dto.engine_id;
            engine_adsb.item_no = dto.item_no;
            engine_adsb.reference = dto.reference;
            engine_adsb.subject = dto.subject;
            engine_adsb.remark = dto.remark;
            engine_adsb.date_due = dto.date_due == null ? null : str_to_date(dto.date_due);
            engine_adsb.remaining_cycles = dto.remaining_cycles;
            engine_adsb.date_initial = dto.date_initial == null ? null : str_to_date(dto.date_initial);



            await context.SaveChangesAsync();
            return Ok(dto);
        }
        [Route("api/mnt/engine/adsb/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEngineADSBDel(dto_id dto)
        {
            ppa_entities context = new ppa_entities();
            var obj = await context.mnt_engine_adsb.FirstOrDefaultAsync(q => q.id == dto.id);
            if (obj != null)
            {
                context.mnt_engine_adsb.Remove(obj);
                await context.SaveChangesAsync();
            }
            return Ok(true);
        }






        [Route("api/mnt/get/total")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetMntStatusTotal()
        {
            ppa_entities context = new ppa_entities();

            var aircrafts = await context.view_mnt_aircraft.OrderBy(q => q.register).ToListAsync();
            var engines = await context.view_mnt_engine.ToListAsync();
            var checks = await context.view_mnt_aircraft_check.OrderBy(q => q.remaining_minutes_actual).ToListAsync();
            var aircraft_adsbs = await context.view_mnt_aircraft_adsb.OrderBy(q => q.remaining_cycles_actual).ToListAsync();

            foreach (var ac in aircrafts)
            {
                ac.engines = engines.Where(q => q.aircraft_id == ac.id).OrderBy(q => q.engine_no).ToList();
                ac.checks = checks.Where(q => q.aircraft_id == ac.id).OrderBy(q => q.remaining_minutes_actual).ToList();
                ac.adsbs = aircraft_adsbs.Where(q => q.aircraft_id == ac.id).OrderBy(q => q.remaining_cycles_actual).ToList();

            }

            return Ok(aircrafts);
        }

        [Route("api/mnt/get/eng/{engid}/{engno}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetEngine(int engid, int engno)
        {
            ppa_entities context = new ppa_entities();
            var result = context.view_mnt_engine.SingleOrDefault(q => q.id == engid && q.engine_no == engno);
            return Ok(result);
        }

        [Route("api/mnt/get/aircraft/llp/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetAircraftLLP(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = await context.view_mnt_aircraft.FirstOrDefaultAsync(q => q.id == id);
            return Ok(reault);
        }

        [Route("api/mnt/get/engine/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetEngine(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = await context.view_mnt_engine.FirstOrDefaultAsync(q => q.id == id);
            return Ok(reault);
        }

        [Route("api/mnt/get/aircraft/check/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetAcCheck(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = await context.view_mnt_aircraft_check.FirstOrDefaultAsync(q => q.id == id);
            return Ok(reault);
        }

        [Route("api/mnt/get/aircraft/checks/aircraft/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetAcChecks(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = context.view_mnt_aircraft_check.Where(q => q.aircraft_id == id).ToList();
            return Ok(reault);
        }


        [Route("api/mnt/get/aircraft/adsb/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetAcADSB(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = await context.view_mnt_aircraft_adsb.FirstOrDefaultAsync(q => q.id == id);
            return Ok(reault);
        }

        [Route("api/mnt/get/aircraft/adsbs/aircraft/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetAcADSBs(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = context.view_mnt_aircraft_adsb.Where(q => q.aircraft_id == id).ToList();
            return Ok(reault);
        }


        [Route("api/mnt/get/engine/llp/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetEngLlp(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = await context.view_mnt_engine_llp.FirstOrDefaultAsync(q => q.id == id);
            return Ok(reault);
        }

        [Route("api/mnt/get/aircraft/llps/engine/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetEngllps(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = context.view_mnt_engine_llp.Where(q => q.engine_id == id).ToList();
            return Ok(reault);
        }

        [Route("api/mnt/get/engine/adsb/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetEngADSB(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = await context.view_mnt_engine_adsb.FirstOrDefaultAsync(q => q.id == id);
            return Ok(reault);
        }

        [Route("api/mnt/get/aircraft/adsbs/engine/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetEngAdsb(int id)
        {
            ppa_entities context = new ppa_entities();
            var reault = context.view_mnt_engine_adsb.Where(q => q.engine_id == id).ToList();
            return Ok(reault);
        }

        public string to_hm(int n)
        {
            var hh = (n / 60).ToString();
            var mm = (n % 60).ToString();
            return hh + ":" + mm.PadLeft(2, '0');
        }
        [Route("api/mnt/dashboard/{id}")]
        [AcceptVerbs("Get")]
        public async Task<IHttpActionResult> GetMntDashboard(int id)
        {
            ppa_entities context = new ppa_entities();
            var aircrafts = context.view_dashboard_aircraft.Where(q => q.register == "RBC" || q.register == "RBA").OrderBy(q => q.register).ToList();

            var ac_ids = aircrafts.Select(q => q.id).ToList();
            var _checks = context.view_mnt_aircraft_check.ToList();
            var _adsbs = context.view_mnt_aircraft_adsb.ToList();

            var _date = DateTime.Now.Date;
            var flights = context.view_mnt_flt.Where(q => q.STDDayLocal == _date && q.FlightStatusID != 4).ToList();



            var result = new List<dashboard_aircraft>();
            foreach (var ac in aircrafts)
            {

                var item = new dashboard_aircraft()
                {
                    id = ac.id,
                    block = to_hm((int)ac.today_departed_block),
                    block_num = ac.today_departed_block,
                    cycle = ac.today_departed_flights,
                    register = ac.register,
                    route = "FLIGHTS ROUTE",
                    tfh_num = ac.total_flight_minute_actual,
                    tfh = to_hm((int)ac.total_flight_minute_actual),
                    tfc = ac.total_flight_cycle_actual,
                    sta = ((DateTime)ac.today_sta).ToString("HH:mm"),
                    std = ((DateTime)ac.today_std).ToString("HH:mm"),
                    scheduled_cycle = ac.today_total_flights,
                    scheduled_block_num = ac.today_total_block,
                    scheduled_block = to_hm((int)ac.today_total_block),
                    landing_gear = new dashboard_lg()
                    {
                        actual_remaining_days = ac.landing_gear_remaining_actual,
                        actual_remaining_ldgs = 0,
                        initial_remaining_days = ac.landing_gear_remaining,
                        initial_remaining_ldgs = 0,
                    },
                    apu = new dashboard_apu()
                    {
                        actual_remaining = ac.apu_remaining_actual,
                        append = "hrs",
                        initial_remaining = ac.apu_remaining,
                    },
                    hts = new List<dashboard_check_adsb>() {
                      new dashboard_check_adsb()
                      {
                            initial_remaining=ac.ht1_remaining,
                             actual_remaining=ac.ht1_remaining_actual,
                              append="days",
                               title="H/T 1",
                      },
                       new dashboard_check_adsb()
                      {
                            initial_remaining=ac.ht2_remaining,
                             actual_remaining=ac.ht2_remaining_actual,
                              append="days",
                               title="H/T 2",
                      },
                        new dashboard_check_adsb()
                      {
                            initial_remaining=ac.ht3_remaining,
                             actual_remaining=ac.ht3_remaining_actual,
                              append="days",
                               title="H/T 3",
                      },

                    },
                    adsbs = new List<dashboard_check_adsb>(),
                    checks = new List<dashboard_check_adsb>(),
                    defects = new dashboard_defect()
                    {
                        initial_remaining = ac.first_due_remaining,
                        actual_remaining = ac.first_due_remaining_actual,
                        append = "cy",
                        count = ac.deffects_no,
                    },
                    engine1 = new dashboard_engine()
                    {
                        append = "cy",
                        id = -1,
                        actual_remaining = ac.engine1_remaining_cycles_actual,
                        initial_remaining = ac.engine1_remaining_cycles,
                        label = "Engine 1",
                        sn = ac.engine2_serial_no
                    },
                    engine2 = new dashboard_engine()
                    {
                        append = "cy",
                        id = -1,
                        actual_remaining = ac.engine2_remaining_cycles_actual,
                        initial_remaining = ac.engine2_remaining_cycles,
                        label = "Engine 2",
                        sn = ac.engine2_serial_no
                    }


                };
                var ac_adsbs = _adsbs.Where(q => q.aircraft_id == ac.id).OrderBy(q => q.remaining_days_actual).ToList();
                var ac_checks = _checks.Where(q => q.aircraft_id == ac.id).OrderBy(q => q.remaining_minutes_actual).ToList();
                foreach (var x in ac_adsbs)
                {
                    item.adsbs.Add(new dashboard_check_adsb()
                    {
                        actual_remaining = x.remaining_days_actual,
                        initial_remaining = x.remaining_days,
                        append = "cy",
                        title = x.reference,
                    });
                }
                foreach (var x in ac_checks)
                {
                    item.checks.Add(new dashboard_check_adsb()
                    {
                        actual_remaining = x.remaining_minutes_actual,
                        initial_remaining = x.remaining_minutes,
                        append = "hrs",
                        title = x.check,
                    });
                }

                var flts = flights.Where(q => q.RegisterID == ac.id).OrderBy(q => q.STD).ToList();
                item.route = get_route(flts);

                result.Add(item);
            }

            return Ok(result);
        }

        private string get_route(List<view_mnt_flt> flts)
        {
            List<string> strs = new List<string>();
            foreach (var flt in flts)
            {
                strs.Add(flt.FromAirportIATA2);
            }
            strs.Add(flts.Last().ToAirportIATA2);
            return string.Join("-", strs);

        }

       

        public class dashboard_aircraft
        {
            public int id { get; set; }
            public string register { get; set; }
            public int? tfh_num { get; set; }
            public int? tfc { get; set; }
            public string tfh { get; set; }

            public int? cycle { get; set; }
            public string block { get; set; }
            public int? block_num { get; set; }
            public int? scheduled_cycle { get; set; }
            public string scheduled_block { get; set; }
            public int? scheduled_block_num { get; set; }
            public string std { get; set; }
            public string sta { get; set; }
            public string route { get; set; }
            public dashboard_engine engine1 { get; set; }
            public dashboard_engine engine2 { get; set; }
            public dashboard_lg landing_gear { get; set; }
            public dashboard_apu apu { get; set; }
            public dashboard_defect defects { get; set; }
            public List<dashboard_check_adsb> checks { get; set; }
            public List<dashboard_check_adsb> adsbs { get; set; }
            public List<dashboard_check_adsb> hts { get; set; }

        }

        public class dashboard_engine
        {
            public int id { get; set; }
            public string sn { get; set; }
            public int? initial_remaining { get; set; }
            public int? actual_remaining { get; set; }
            public string append { get; set; }
            public string label { get; set; }
        }

        public class dashboard_lg
        {
            public int? initial_remaining_days { get; set; }
            public int? initial_remaining_ldgs { get; set; }
            public int? actual_remaining_days { get; set; }
            public int? actual_remaining_ldgs { get; set; }
        }
        public class dashboard_apu
        {
            public int? initial_remaining { get; set; }
            public int? actual_remaining { get; set; }
            public string append { get; set; }
        }
        public class dashboard_defect
        {
            public int? count { get; set; }
            public int? actual_remaining { get; set; }
            public int? initial_remaining { get; set; }
            public string append { get; set; }
        }
        public class dashboard_check_adsb
        {
            public int id { get; set; }
            public string title { get; set; }
            public int? actual_remaining { get; set; }
            public int? initial_remaining { get; set; }
            public string append { get; set; }

        }



    }
}
