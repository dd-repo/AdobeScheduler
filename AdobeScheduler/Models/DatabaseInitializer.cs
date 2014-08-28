using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace AdobeScheduler.Models
{
    public class DatabaseInitializer : CreateDatabaseIfNotExists<AdobeConnectDB>
    {
        protected override void Seed(AdobeConnectDB context)
        {
            System.Diagnostics.Debug.WriteLine("seeding");

            if (!context.AdobeUserInfo.Any(r => r.Username == "admin"))
            {
                //seeding data

                context.SaveChanges();
            }
        }
    }
}