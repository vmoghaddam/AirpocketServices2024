﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace ApiScheduling
{
    public class SchedulingHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}