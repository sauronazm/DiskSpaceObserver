using System;
using System.Configuration;
using System.Net.Mail;

namespace DiskSpaceObserver {
    class Mailer {

        #region Constructor
        private Mailer() {

        }
        #endregion

        #region Private Members

        private string _mailFromUsername;
        private string _mailFromServer;
        private string _mailFromPassword;
        private string _mailTo;
        private string _mailSubject;
        private static Mailer _mailer;

        #endregion

        #region Private Methods

        private void ReadMailerConfig() {
            try {
                _mailFromUsername = ConfigurationManager.AppSettings["EmailFromUsername"];
                _mailFromServer = ConfigurationManager.AppSettings["EmailFromServer"];
                _mailFromPassword = ConfigurationManager.AppSettings["EmailFromPassword"];
                _mailTo = ConfigurationManager.AppSettings["EmailTo"];
                _mailSubject = ConfigurationManager.AppSettings["EmailSubject"];
            }
            catch (Exception ex) {
                ErrorManager.ProcessError(ex);
            }
        }

        private void SendMessage(string text, bool important) {
            try {
                var message = new MailMessage();
                if (important) {
                    message.Priority = MailPriority.High;
                }
                else {
                    message.Priority = MailPriority.Low;
                }
                var fromAddress = _mailFromUsername + @"@" + _mailFromServer;
                message.From = new MailAddress(fromAddress);
                string[] recipients = _mailTo.Split(";".ToCharArray());
                foreach (var item in recipients) {
                    message.To.Add(item);
                }
                message.Body = text;
                message.Subject = _mailSubject;
                SmtpClient theClient = new SmtpClient("smtp." + _mailFromServer);
                theClient.EnableSsl = true;
                theClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential theCredential = new System.Net.NetworkCredential(fromAddress, _mailFromPassword);
                theClient.Credentials = theCredential;
                theClient.Send(message);
            }
            catch (Exception ex) {
                ErrorManager.ProcessError(ex);
            }
        }

        #endregion

        #region Public Methods

        public static Mailer GetMailer() {
            if (_mailer == null) {
                _mailer = new Mailer();
                _mailer.ReadMailerConfig();
            }
            return _mailer;
        }

        public void SendEmail(string text, bool important) {
            SendMessage(text, important);
        }

        #endregion
    }
}
