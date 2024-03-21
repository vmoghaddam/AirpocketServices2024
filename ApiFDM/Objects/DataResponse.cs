using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFDM.Objects
{
    public class DataResponse
    {
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
    }
}