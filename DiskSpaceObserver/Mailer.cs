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
        private bool _mailUseSSL;
        private static Mailer _mailer;

        #endregion

        #region Private Methods

        private void ReadMailerConfig() {
            try {
                _mailFromUsername = ConfigurationManager.AppSettings["MailFromUsername"];
                _mailFromServer = ConfigurationManager.AppSettings["MailFromServer"];
                _mailFromPassword = ConfigurationManager.AppSettings["MailFromPassword"];
                _mailTo = ConfigurationManager.AppSettings["MailTo"];
                _mailSubject = ConfigurationManager.AppSettings["MailSubject"];
                if (ConfigurationManager.AppSettings["MailUseSSL"] != null && ConfigurationManager.AppSettings["MailUseSSL"].ToLower() == "no") {
                    _mailUseSSL = false;
                }
                else {
                    _mailUseSSL = true;
                }
            }
            catch (Exception ex) {
                ErrorManager.ProcessError(ex, "Read Mailer Config.");
            }
        }

        private void SendMessage(string text, bool important, bool html) {
            using (var theClient = new SmtpClient("smtp." + _mailFromServer)) {
                using (var message = new MailMessage()) {
                    try {
                        if (important) {
                            message.Priority = MailPriority.High;
                        }
                        else {
                            message.Priority = MailPriority.Low;
                        }
                        if (html) {
                            message.IsBodyHtml = true;
                        }
                        var fromAddress = _mailFromUsername + @"@" + _mailFromServer;
                        message.From = new MailAddress(fromAddress);
                        string[] recipients = _mailTo.Split(";".ToCharArray());
                        foreach (var item in recipients) {
                            message.To.Add(item);
                        }
                        message.Body = text;
                        message.Subject = _mailSubject;
                        theClient.EnableSsl = _mailUseSSL;
                        theClient.UseDefaultCredentials = false;
                        theClient.Credentials = new System.Net.NetworkCredential(fromAddress, _mailFromPassword);
                        theClient.Send(message);
                    }
                    catch (Exception ex) {
                        if (ex.HResult == -2146233088 && message != null && theClient != null) { //TODO: Поместить "магическое" число в константу. 
                            try {
                                theClient.EnableSsl = false;
                                theClient.Send(message);
                            }
                            catch (Exception ex1) {
                                ErrorManager.ProcessError(ex1, "Mailer::SendMessage, second attempt.");
                            }
                        }
                        else {
                            ErrorManager.ProcessError(ex, "Mailer::SendMessage");
                        }
                    }
                }
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

        public void SendEmail(string text, bool important, bool html) {
            SendMessage(text, important, html);
        }

        #endregion
    }
}
