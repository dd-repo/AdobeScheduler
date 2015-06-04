using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdobeConnectSDK;
using AdobeScheduler.Models;
using System.Globalization;
using System.Web.UI;
using System.Threading.Tasks;
using System.Data.SqlClient;
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
        public bool editable { get; set; }
        public bool open { get; set; }
        public bool archived { get; set; }

        public CalendarData(){
            this.editable = true;
        }

    }
    
    [HubName("adobeConnect")]
    public class AdobeStream : Hub
    {
        public bool AddAppointment(bool isChecked,bool isUpdate, string roomId, string userId, string name, string roomSize, string url, string path, string JsdateTime, string Jsmin, bool jsHandle)
        {
            
            DateTime date = DateTime.Parse(JsdateTime);
            int endtime = int.Parse(Jsmin);
            DateTime end = date.AddMinutes(endtime);
            if (int.Parse(roomSize) > 50)
            {
                return false;
            }
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
                        Clients.All.addEvent(appointment, isChecked,isUpdate,jsHandle);
                        return true;
                    }
                    else
                    {
                        Clients.Caller.addEvent(appointment, isChecked,isUpdate,jsHandle);
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
                        Clients.All.addEvent(query, isChecked, true,jsHandle);
                        return true;
                    }
                    else
                    {
                        Clients.Caller.addEvent(query, isChecked,isUpdate,jsHandle);
                        return false;
                    }
                }
            }

        }

        public string Login(string username, string password=null)
        {
            AdobeConnectXmlAPI adobeObj = new AdobeConnectXmlAPI();
            StatusInfo sInfo;
            using (AdobeConnectDB _db = new AdobeConnectDB())
            {
                var query = _db.AdobeUserInfo.Where(u => u.Username == username).FirstOrDefault();
                if (password == null)
                {
                    password = query.Password;
                }
                if (adobeObj.Login(username, password, out sInfo) == false)
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
                    string _targetUrl = string.Format("http://turner.southern.edu/api/xml?action=login&login={0}&password={1}", username, password);
                    return _targetUrl;
                }
            }
        }

        public void addSelf(Appointment data, string id, bool isChecked, bool isUpdate, int max, bool jsHandle, string jsDate)
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

                var calendarData = ConstructObject(data, id,jsDate);
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
                    Clients.Caller.addSelf(true, calendarData, remaining,jsHandle);
                    return;
                }
                Clients.Caller.addSelf(false, calendarData, remaining,jsHandle);
                return;

            }

            
           
           
        }

        async public Task<List<CalendarData>> GetAllAppointments(string jsDate)
        {
            DateTime Date = DateTime.Parse(jsDate);
            DateTime DateS = Date.AddHours(-2);
            DateTime DateM = Date.AddMonths(-1);
            using (AdobeConnectDB _db = new AdobeConnectDB())
            {
                /*var query _db.Database.SqlQuery()
                List<Appointment> query = new List<Appointment>();
                try
                {
                    query = (from r in _db.Appointments where (r.end >= DateS && r.start >= DateM) select r).ToList();
                catch(Exception e)
                {
                    throw e;
                }
                //querying the data for the population of the calandar object
                var query = _db.Database.SqlQuery<Appointment>("SELECT * FROM dbo.Appointments WHERE end <= @dateM AND start >= @dateS",
                     new SqlParameter("dateM", DateM), 
                     new SqlParameter("dateS", DateS)).ToList();
                string tDateS = DateM.ToString("yyyy-MM-dd HH:mm:ss"),
                       tDateM = DateS.ToString("yyyy-MM-dd HH:mm:ss");*/

                List<Appointment> query = new List<Appointment>();
                //querying the data for the population of the calandar object
                try
                {
                    /*query = _db.Database.SqlQuery<Appointment>("SELECT * FROM dbo.Appointments   WHERE 'end' >= {0} AND 'start' >= {1}",
                         DateS.ToString("yyyy-MM-dd HH:mm:ss"),
                         DateM.ToString("yyyy-MM-dd HH:mm:ss")).ToList();*/
                    query = (from r in _db.Appointments where (r.end >= DateS && r.start >= DateM) select r).ToList();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
                
                List<CalendarData> calList = new List<CalendarData>();
                foreach(Appointment res in query)
                {
                    var obj = ConstructObject(res, HttpContext.Current.User.Identity.Name,jsDate);
                    calList.Add(obj);
                }
                 return await Task.Run(() => calList);
            }
        }

        public CalendarData ConstructObject(Appointment appointment, string id, string jsDate)
        {
            Clients.Caller.date(jsDate);
            DateTime Date = DateTime.Parse(jsDate);
            CalendarData callendarData = new CalendarData();
            callendarData.id = appointment.id;
            callendarData.userId = appointment.userId;
            callendarData.title = appointment.title;
            callendarData.url = appointment.url;
            callendarData.color = "#c4afb9";
            callendarData.adobeUrl = appointment.adobeUrl;
            callendarData.roomSize = appointment.roomSize;
            callendarData.start = appointment.start;
            callendarData.end = appointment.end;
            callendarData.editable = true;
            callendarData.open = true;
            callendarData.archived = false;

            if (!checkHost(id,callendarData.title))
            {
                callendarData.color = "#d3bf96";
                callendarData.url = "";
                callendarData.editable = false;
            }

            if (checkHost(id, callendarData.title))
            {
                if (Date < callendarData.start)
                {
                    callendarData.color = "#bac7c3";
                    callendarData.open = false;
                    callendarData.url = "";
                }

            }

            if (Date > callendarData.end)
            {
                callendarData.color = "gray";
                callendarData.open = false;
                callendarData.url = "";
                callendarData.editable = false;
                callendarData.archived = true;
            }
            return callendarData;
        }

        public bool Delete(string id)
        {
            int Id = int.Parse(id);
            using (AdobeConnectDB _db =  new AdobeConnectDB()){

                //querying the data for the population of the calandar object for deletion 
                List<Appointment> query = new List<Appointment>();
                try
                {
                    query = (from appointmnet in _db.Appointments where appointmnet.id == Id select appointmnet).ToList();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }              

                //var query = from appointmnet in _db.Appointments where appointmnet.id == Id select appointmnet;
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

        public CalendarData GetEvent(string id, string jsDate)
        {
            int Id = int.Parse(id);
            using (AdobeConnectDB _db = new AdobeConnectDB())
            {

                var query = (from appointmnet in _db.Appointments where appointmnet.id == Id select appointmnet).FirstOrDefault();
                return ConstructObject(query, query.userId,jsDate);
            }
        }

        public bool checkHost(string username, string meeting)
        {
            AdobeConnectXmlAPI adobeObj = new AdobeConnectXmlAPI();
            StatusInfo sInfo;

            using (AdobeConnectDB _db = new AdobeConnectDB())
            {
                var query = _db.AdobeUserInfo.Where(u => u.Username == username).FirstOrDefault();
                List<String> meetingList = new List<String>();
                if (adobeObj.Login(username, query.Password, out sInfo)){
                    var myMeeting = adobeObj.GetMyMeetings();
                    foreach(MeetingItem myMeetingItem in myMeeting){
                        meetingList.Add(myMeetingItem.meeting_name);
                    }
                    var result = meetingList.Contains(meeting);
                    return result;
                }
                return false;
            }
        }
    }
}