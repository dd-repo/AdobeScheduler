﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AdobeScheduler.Models
{
    public class AdobeConnectDB : DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }
    }
}