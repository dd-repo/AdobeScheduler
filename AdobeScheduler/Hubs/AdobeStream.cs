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
        public bool AddAppointment(bool isChecked,bool isUpdate, string roomId, string userId, string name, string roomSize, string url, string path, string Jsdate, string Jstime,string min)
        {
            DateTime t = DateTime.ParseExact(Jstime, "hh:mm tt", CultureInfo.InvariantCulture);
            DateTime Tempdate = DateTime.Parse(Jsdate);
            int endtime = int.Parse(min);
            TimeSpan time = t.TimeOfDay;
            DateTime date = Tempdate.Add(time);
            DateTime end = date.AddMinutes(endtime);
            if (!isUpdate)
            {
                using (AdobeConnectDB _db = new AdobeConnectDB())
                {
                    Appointment appointment = new Appointment();
                    CalendarData callendarData = new CalendarData();
                    appointment.userId = userId;
                    appointment.title = name;
                    appointment.roomSize = int.Parse(roomSize);
                    appointment.url = path;
                    appointment.adobeUrl = url;
                    appointment.start = date;
                    appointment.end = end;
                    if (isChecked)
                    {
                        _db.Appointments.Add(appointment);
                        _db.SaveChanges();
                        Clients.All.addEvent(appointment, isChecked,isUpdate);
                        return true;
                    }
                    else
                    {
                        Clients.Caller.addEvent(appointment, isChecked,isUpdate);
                        return false;
                    }
                }
            }
            else
            {
                int Id = int.Parse(roomId);
                using (AdobeConnectDB _db = new AdobeConnectDB())
                {
                    var query = (from appointmnet in _db.Appointments where appointmnet.id == Id select appointmnet).Single();
                    query.start = date;
                    query.roomSize = int.Parse(roomSize);
                    query.title = name;
                    query.adobeUrl = url;
                    query.url = path;
                    query.start = date;
                    query.end = end;
                    if (isChecked)
                    {
                        _db.SaveChanges();
                        Clients.All.addEvent(query, isChecked, true);
                        return true;
                    }
                    else
                    {
                        Clients.Caller.addEvent(query, isChecked,isUpdate);
                        return false;
                    }
                }
            }

        }

        public string Login(string username, string password)
        {
            AdobeConnectXmlAPI adobeObj = new AdobeConnectXmlAPI();
            StatusInfo sInfo;
            if (adobeObj.Login(username, password, out sInfo)== false)
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                { return ""; }
                else
                {
                    return "";
                }
            }
            else
            {
                string _targetUrl = string.Format("http://turner.southern.edu/api/xml?action=login&login={0}&password={1}",username,password);
                return _targetUrl;
            }
        }

        public void addSelf(Appointment data, string id, bool isChecked, bool isUpdate, int max)
        {
            int selfTotal = 0;
            int remaining;
            using (AdobeConnectDB _db = new AdobeConnectDB())
            {
                var query = (from r in _db.Appointments
                             where ((data.start >= r.start && data.start <= r.end) || (data.end >= r.start && data.end <= r.end))
                             select r
                    );

                foreach (Appointment appoinment in query)
                {
                    if (appoinment.id != data.id) {
                        selfTotal += appoinment.roomSize;
                    }
                }

                var calendarData = ConstructObject(data, id);
                remaining = 50 - selfTotal;
                if (isUpdate) {
                    if (isChecked)
                    {
                        Clients.Caller.updateSelf(calendarData);
                        return;
                    }
                }
                if (isChecked)
                {
                    Clients.Caller.addSelf(true, calendarData, remaining);
                    return;
                }
                Clients.Client(Context.ConnectionId).addSelf(false, calendarData, remaining);
                return;

            }

            
           
           
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

        public bool Delete(string id)
        {
            int Id = int.Parse(id);
            using (AdobeConnectDB _db =  new AdobeConnectDB()){

                var query = from appointmnet in _db.Appointments where appointmnet.id == Id select appointmnet;
                foreach (Appointment res in query){
                    _db.Appointments.Remove(res);
                }
                if (_db.SaveChanges()>0)
                {
                    Clients.All.removeSelf(Id);
                    return true;
                }

            }
            return false;
        }
    }
}