﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdobeScheduler.Models
{
    public class Appointment
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string adobeUrl { get; set; }
        public DateTime start { get; set; }
        public bool allDay { get; set; }
        public int roomSize { get; set; }

        public Appointment()
        {
            this.allDay = false;
        }
    }
}