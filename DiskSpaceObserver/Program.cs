namespace DiskSpaceObserver {
    class Program {
        static void Main(string[] args) {
            var observer = new DiskSpaceObserver.DiscObserver();
            observer.CheckMachine();
            if (!observer.AllIsOk || SettingsWrapper.AppSettingWrapper.GetMailSendAlways() == "yes") {
                var mailer = Mailer.GetMailer();
                var subject = SettingsWrapper.AppSettingWrapper.GetMailSubject();
                if (subject != null) {
                    subject = subject + " Time:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                }
                else {
                    subject = "Disc Observer got error while getting email title templte from config.";
                }
                mailer.SendEmail(subject, observer.Message, !observer.AllIsOk, observer.UseHTML);
            }
            System.Diagnostics.EventLog.WriteEntry("DiskSpaceObserver", "Disk Space Observer sucessfully finished.", System.Diagnostics.EventLogEntryType.Information);
        }
    }
}
