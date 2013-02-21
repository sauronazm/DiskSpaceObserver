﻿using System.Configuration;

namespace DiskSpaceObserver {
    class Program {
        static void Main(string[] args) {
            var observer = new DiskSpaceObserver.DiscObserver();
            observer.CheckMachine();
            if (!observer.AllIsOk || ConfigurationManager.AppSettings["SendMailAlways"] == "yes") {
                var mailer = Mailer.GetMailer();
                mailer.SendEmail(observer.Message, true);
            }
            System.Diagnostics.EventLog.WriteEntry("DiskSpaceObserver", "Disk Space Observer sucessfully finished.", System.Diagnostics.EventLogEntryType.Information);
        }
    }
}