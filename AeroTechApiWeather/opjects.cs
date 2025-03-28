using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AeroTechApiWeather
{
    public class DataResponse
    {
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
        public object DataExtra { get; set; }
        public List<string> Errors { get; set; }
    }
}