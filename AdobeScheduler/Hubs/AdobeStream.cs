using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdobeConnectSDK;
using AdobeScheduler.Models;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.UI;
namespace AdobeScheduler.Hubs
{

    public class CalendarData
    {
        public int id {get;set;}
        public string userId { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string adobeUrl { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public bool allDay { get; set; }
        public int roomSize { get; set; }
        public string color {get;set;}

    }
    
    [HubName("adobeConnect")]
    public class AdobeStream : Hub
    {
        public void AddAppointment(string id, string name, string roomSize, string url, string path, string Jsdate, string Jstime,string min)
        {
            DateTime t = DateTime.ParseExact(Jstime, "hh:mm tt", CultureInfo.InvariantCulture);
            DateTime Tempdate = DateTime.Parse(Jsdate);
            int endtime = int.Parse(min);
            TimeSpan time = t.TimeOfDay;
            DateTime date = Tempdate.Add(time);
            DateTime end = date.AddMinutes(endtime);
            if(String.IsNullOrEmpty(roomSize)){
                Clients.Caller.responceMessage("Missing Data");
            }
            else{
                using (AdobeConnectDB _db = new AdobeConnectDB())
                {
                    Appointment appointment = new Appointment();
                    CalendarData callendarData = new CalendarData();
                    appointment.userId = id;
                    appointment.title = name;
                    appointment.roomSize = int.Parse(roomSize);
                    appointment.url = path;
                    appointment.adobeUrl = url;
                    appointment.start = date;
                    appointment.end = end;
                    _db.Appointments.Add(appointment);
                    _db.SaveChanges();
                    Clients.All.addEvent(appointment);
                }
            }
        }

        public bool Login(string username, string password)
        {
            AdobeConnectXmlAPI adobeObj = new AdobeConnectXmlAPI();
            StatusInfo sInfo;
            if (adobeObj.Login(username, password, out sInfo)== false)
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                { return false; }
                else
                {
                    Clients.Caller.responceMessage("Invalid Credentials!");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void addSelf(Appointment data, string id)
        {
           var calendarData = ConstructObject(data,id);
           Clients.Caller.addSelf(calendarData);
        }

        public Task<List<CalendarData>> GetAllAppointments()
        {
            return Task.Factory.StartNew(() =>
            {

                using (AdobeConnectDB _db = new AdobeConnectDB())
                {   

                    var query = (from r in _db.Appointments select r).ToList();
                    List<CalendarData> calList = new List<CalendarData>();
                    foreach(Appointment res in query)
                    {
                        var obj = ConstructObject(res, Context.User.Identity.Name);
                        calList.Add(obj);
                    }

                    return calList;
                }
            });
            
        }

        public CalendarData ConstructObject(Appointment appointment, string id)
        {
            CalendarData callendarData = new CalendarData();
            callendarData.id = appointment.id;
            callendarData.userId = appointment.userId;
            callendarData.title = appointment.title;
            callendarData.url = appointment.url;
            callendarData.adobeUrl = appointment.adobeUrl;
            callendarData.roomSize = appointment.roomSize;
            callendarData.start = appointment.start;
            callendarData.end = appointment.end;

            if (callendarData.userId != id)
            {
                callendarData.color = "green";
                callendarData.url = "";
            }

            if (callendarData.userId == id)
            {
                if (DateTime.Now < callendarData.start)
                {
                    callendarData.color = "black";
                    callendarData.url = "";
                }

            }

            return callendarData;
        }
    }
}