using mpNuget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiCRM
{
    public class sms_obj
    {
        public string mobile { get; set; }
        public string name { get; set; }
        public string message { get; set; }
    }
    public class MelliPayamak
    {
        public string send(sms_obj obj)
        {
            RestClient client = new RestClient("9354957316", "Rhbsms99@");
            var result = client.Send(obj.mobile, "90009105", obj.message, false).Value;
            return result;
        }
    }
}