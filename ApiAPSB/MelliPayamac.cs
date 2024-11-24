using mpNuget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ApiAPSB
{
    public class MelliPayamac
    {
        public string send(string mobile, string name, string message)
        {
            RestClient client = new RestClient("9354957316", "Rhbsms99@");
            var result = client.Send(mobile, "90009105", message, false).Value;
            return result;
        }
    };



}