using ApiGRH.Models;
using ApiGRH.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;


namespace ApiGRH.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GRHController : ApiController
    {
        ppa_entities context = new ppa_entities();

        public class dto_sign
        {
            public int user_id { get; set; }
            public int flight_id { get; set; }
        }

        [Route("api/grh/turnaround/sign")]
        [AcceptVerbs("Post")]
        public async Task<DataResponse> user_sign(dto_sign dto)
        {
            try
            {
                var form = await context.chk_turn_around.FirstOrDefaultAsync(q => q.flight_id == dto.flight_id);
                form.sign_user_id = dto.user_id;
                form.sign_date = DateTime.Now;

                context.SaveChanges();

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = null
                };

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }

        [Route("api/grh/turnaround/station/manager/sign")]
        [AcceptVerbs("Post")]
        public async Task<DataResponse> station_manager_sign(dto_sign dto)
        {
            try
            {
                var form = await context.chk_turn_around.FirstOrDefaultAsync(q => q.flight_id == dto.flight_id);
                form.station_manager_user_id = dto.user_id;
                form.station_manager_sign_date = DateTime.Now;

                context.SaveChanges();

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = null
                };

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }

        public class item_update
        {
            public int user_id { get; set; }
            public int flight_id { get; set; }
            public int item_id { get; set; }
            public int? actual_start { get; set; }
            public string remark { get; set; }
        }

        [Route("api/grh/turnaround/item/value/update")]
        [AcceptVerbs("Post")]
        public async Task<DataResponse> item_value_update(item_update item)
        {

            try
            {

                var flight = context.AppLegs.FirstOrDefault(q => q.FlightId == item.flight_id);
                var form = await context.chk_turn_around.FirstOrDefaultAsync(q => q.flight_id == item.flight_id);
                var item_value = await context.chk_turn_around_item_value.FirstOrDefaultAsync(q => q.form_id == form.id && q.item_id == item.item_id);

                if (item_value == null)
                {
                    item_value = new chk_turn_around_item_value();
                    context.chk_turn_around_item_value.Add(item_value);
                }

                if (item.actual_start == 0 || item.actual_start == null)
                    item_value.time_start_actual = null;
                else
                    item_value.time_start_actual = item.actual_start;

                item_value.form_id = form.id;
                item_value.item_id = item.item_id;
                item_value.user_id = item.user_id;
                item_value.date_update = DateTime.Now;
                item_value.remark = item.remark;

                context.SaveChanges();

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = item
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }



        public class dto_update
        {
            public int flight_id { get; set; } // default value is 0
            public string update_user_id { get; set; } // string property
            public int fuel_litr { get; set; } // assuming it's a decimal, you can change the type as needed
            public string fuel_sn { get; set; } // string property
            public string remark { get; set; } // string property
            public int pax_male { get; set; } // number of male passengers
            public int pax_female { get; set; } // number of female passengers
            public int pax_adult { get; set; } // number of adult passengers
            public int pax_child { get; set; } // number of child passengers
            public int pax_infant { get; set; } // number of infant passengers
            public DateTime offblock { get; set; } // default value is 1012
            public DateTime takeoff { get; set; } // default value is 1024
            public int delay { get; set; } // default value is 0
            public string delay_code { get; set; } // string property for delay code
        }




        [Route("api/grh/turnaround/update")]
        [AcceptVerbs("Post")]
        public async Task<DataResponse> turn_around_update(dto_update dto)
        {
            try
            {
                var form = await context.chk_turn_around.FirstOrDefaultAsync(q => q.flight_id == dto.flight_id);

                form.update_user_id = dto.update_user_id;
                form.update_date = DateTime.Now;
                form.fuel_litr = dto.fuel_litr;
                form.fuel_sn = dto.fuel_sn;
                form.remark = dto.remark;
                form.pax_adult = dto.pax_adult;
                form.pax_child = dto.pax_child;
                form.pax_infant = dto.pax_infant;
                form.pax_female = dto.pax_female;
                form.pax_male = dto.pax_male;
                form.offblock = dto.offblock;
                form.takeoff = dto.takeoff;
                form.delay = dto.delay;
                form.delay_code = dto.delay_code;

                context.SaveChanges();

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }


        }

        public class Task
        {
            public int? item_id { get; set; }
            public int? order_index { get; set; }
            public string title { get; set; }
            public int? time_start { get; set; }
            public int? time_start_actual { get; set; }
        }

        public class FormData
        {
            public int form_id { get; set; }
            public DateTime Date { get; set; }
            public string station { get; set; }
            public string fuel_sn { get; set; }
            public int? fuel_litr { get; set; }
            public int? delay { get; set; }
            public string delay_code { get; set; }
            public string route { get; set; }
            public string flight_no { get; set; }
            public int? flight_id { get; set; }
            public string register { get; set; }
            public int? register_id { get; set; }
            public string dept { get; set; }
            public string captain { get; set; }
            public string sc_crew { get; set; }
            public int? pax_adult { get; set; }
            public int? pax_child { get; set; }
            public int? pax_infant { get; set; }
            public int? pax_male { get; set; }
            public int? pax_female { get; set; }
            public string remark { get; set; }
            public string station_managerName { get; set; }
            public string station_manager_signature { get; set; }
            public string ac_type { get; set; }
            public string ac_model { get; set; }
            public DateTime? std { get; set; }
            public DateTime? sta { get; set; }
            public List<Task> tasks { get; set; }

            public FormData()
            {
                tasks = new List<Task>();
            }
        }

        [Route("api/grh/turnaround/get/{flight_id}")]
        [AcceptVerbs("Get")]
        public async Task<DataResponse> turn_around_get(int flight_id)
        {
            FormData form = new FormData();

            try
            {
                var info = context.view_grh_turn_around.FirstOrDefault(q => q.flight_id == flight_id);
                form.tasks = context.view_grh_turn_around_item_value
                  .Where(q => q.flight_id == flight_id)
                  .Select(q => new Task
                  {
                      item_id = q.id,
                      order_index = q.order_index,
                      title = q.title,
                      time_start = q.time_start,
                      time_start_actual = q.time_start_actual
                  }).ToList();
                form.form_id = info.id;
                form.pax_infant = info.pax_infant;
                form.pax_child = info.pax_child;
                form.pax_adult = info.pax_adult;
                form.pax_female = info.pax_female;
                form.pax_male = info.pax_male;
                form.register = info.register;
                form.register_id = info.register_id;
                form.remark = info.remark;
                form.fuel_litr = info.fuel_litr;
                form.fuel_sn = info.fuel_sn;
                form.delay = info.delay;
                form.delay_code = info.delay_code;
                form.flight_no = info.flight_no;
                form.flight_id = info.flight_id;
                form.ac_type = info.ac_type;
                form.ac_model = info.ac_model;
                form.std = info.std;
                form.sta = info.sta;




                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = form
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }


        }


        public class get_turnaround_station
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
            public int station_id { get; set; }
        }


        public class flight
        {
            public int? flight_id { get; set; }
            public DateTime? STDDay { get; set; }
            public DateTime? sta { get; set; }
            public string FlightNumber { get; set; }
            public string Register { get; set; }
            public DateTime? block_off { get; set; }
            public DateTime? block_on { get; set; }
            public string FromAirportIATA { get; set; }
            public string ToAirportIATA { get; set; }
            public int? ScheduledFlightTime { get; set; }

            public int? station_id { get; set; }
        }

        [Route("api/grh/turnaround/get/flights")]
        [AcceptVerbs("Post")]
        public async Task<DataResponse> turn_around_station_get(get_turnaround_station dto)
        {
            FormData form = new FormData();

            try
            {
                var flights = context.view_grh_turn_around
                  .Where(q => q.station_id == dto.station_id && q.flight_day == dto.day && q.flight_month == dto.month && q.flight_year == dto.year)
                  .Select(q => new flight
                  {
                      flight_id = q.flight_id,
                      FlightNumber = q.flight_no,
                      FromAirportIATA = q.from_iata,
                      ToAirportIATA = q.to_iata,
                      Register = q.register,
                      STDDay = q.std,
                      sta = q.sta,
                      block_off = q.blockoff_station_local,
                      block_on = q.blockon_station_local,
                      station_id = null
                  }).ToList();


               


                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = flights
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }


        }


    }
}
