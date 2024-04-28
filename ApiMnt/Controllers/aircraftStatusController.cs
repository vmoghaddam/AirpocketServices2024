using ApiMnt.Models;
using ApiMnt.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            check.date_initial = str_to_date(dto.date_initial);
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
            adsb.date_initial = str_to_date(dto.date_initial);

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
            public int aircraft_id { get; set; }
            public string date_initial { get; set; }

        }
        [Route("api/mnt/engine/status")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEngineStatus(engine_status dto)
        {
            ppa_entities context = new ppa_entities();
            var engine = await context.mnt_engine.Where(q => q.id == dto.id).FirstOrDefaultAsync();
            engine.engine_no = dto.engine_no;
            engine.aircraft_id = dto.aircraft_id;
            engine.cat = dto.cat;
            engine.model = dto.model;
            engine.serial_no = dto.serial_no;
            engine.date_initial = str_to_date(dto.date_initial);
            engine.remaining_cycles = dto.remaining_cycles;
            engine.remaining_minutes = dto.remaining_minutes;
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
            engine_llp.date_initial = str_to_date(dto.date_initial);
            engine_llp.date_due = str_to_date(dto.date_due);



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
            engine_adsb.date_due = str_to_date(dto.date_due);
            engine_adsb.remaining_cycles = dto.remaining_cycles;
            engine_adsb.date_initial = str_to_date(dto.date_initial);



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




    }
}
