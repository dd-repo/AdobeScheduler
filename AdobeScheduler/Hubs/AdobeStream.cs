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

    public static class DataStore
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    
    [HubName("adobeConnect")]
    public class AdobeStream : Hub
    {
        public void AddAppointment(string id, string name, string roomSize, string url, string path, string Jsdate, string Jstime)
        {
            DateTime t = DateTime.ParseExact(Jstime, "hh:mm tt", CultureInfo.InvariantCulture);
            DateTime Tempdate = DateTime.Parse(Jsdate);
            TimeSpan time = t.TimeOfDay;
            DateTime date = Tempdate.Add(time);
            int Id = int.Parse(id);

            if(String.IsNullOrEmpty(roomSize)){
                Clients.Caller.responceMessage("Missing Data");
            }
            else{
                using (AdobeConnectDB _db = new AdobeConnectDB())
                {
                    Appointment appointment = new Appointment();
                    appointment.userId = Id;
                    appointment.title = name;
                    appointment.roomSize = int.Parse(roomSize);
                    appointment.url = path;
                    appointment.adobeUrl = url;
                    appointment.start = date;
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

        public Task<List<Appointment>> GetAllAppointments()
        {
            return Task.Factory.StartNew(() =>
            {

                using (AdobeConnectDB _db = new AdobeConnectDB())
                {
                    var query = (from r in _db.Appointments select r).ToList();
                    return query;
                }
            });
            
        }
    }
}