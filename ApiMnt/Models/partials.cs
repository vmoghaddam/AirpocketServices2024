using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiMnt.Models
{
    public partial class view_mnt_aircraft
    {
        public List<view_mnt_engine> engines { get; set; }
        public List<view_mnt_aircraft_check> checks { get; set; }
        public List<view_mnt_aircraft_adsb> adsbs { get; set; }

    }

    
}