using System;
using System.Text;

namespace DiskSpaceObserver {
    class ErrorManager {

        //TODO: Добавить возможность писать в файл. 

        #region Public Methods

        public static void ProcessError(Exception ex, string extraMessage = "") {
            if (ex != null) {
                var message = new StringBuilder();
                message.Append("Info: " + extraMessage + Environment.NewLine)
                    .Append("Source: " + ex.Source + Environment.NewLine)
                    .Append("Method: " + ex.TargetSite + Environment.NewLine)
                    .Append("Message" + ex.Message + Environment.NewLine)
                    .Append("StackTrace: " + ex.StackTrace + Environment.NewLine);

                System.Diagnostics.EventLog.WriteEntry("DiskSpaceObserver", message.ToString(), System.Diagnostics.EventLogEntryType.Error);
                var mailer = Mailer.GetMailer();
                mailer.SendEmail("Disk Space Observer got error. See Event Log for details.", true, true);

                if (ex.InnerException != null) {
                    ErrorManager.ProcessError(ex.InnerException);
                }
            }
            else {
                System.Diagnostics.EventLog.WriteEntry("DiskSpaceObserver", "Error Manager got null message. Is it normal?", System.Diagnostics.EventLogEntryType.Error);
            }
        }

        #endregion

    }
}
