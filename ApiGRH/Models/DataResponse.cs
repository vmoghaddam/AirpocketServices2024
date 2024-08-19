using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiGRH.Models
{
    public class DataResponse
    {
        public bool IsSuccess { get; set; }
        public dynamic Data { get; set; }
    }
}