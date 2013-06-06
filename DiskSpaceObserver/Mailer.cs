using System;
using System.Net.Mail;
using System.Runtime.InteropServices;

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
        private bool _mailUseSSL;
        private static Mailer _mailer;

        #endregion

        #region Private Methods

        private void ReadMailerConfig() {
            try {
                _mailFromUsername = SettingsWrapper.AppSettingWrapper.GetMailFromUsername();
                _mailFromServer = SettingsWrapper.AppSettingWrapper.GetMailFromServer();
                _mailFromPassword = SettingsWrapper.AppSettingWrapper.GetMailFromPassword();
                _mailTo = SettingsWrapper.AppSettingWrapper.GetMailTo();
                if (SettingsWrapper.AppSettingWrapper.GetMailUseSSL() != null && SettingsWrapper.AppSettingWrapper.GetMailUseSSL() == "no") {
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

        private void SendMessage(string subject, string text, bool important, bool html) {
            const int SSL_ERROR_CODE = -2146233088;
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
                        message.Subject = subject;
                        theClient.EnableSsl = _mailUseSSL;
                        theClient.UseDefaultCredentials = false;
                        theClient.Credentials = new System.Net.NetworkCredential(fromAddress, _mailFromPassword);
                        theClient.Send(message);
                    }
                    catch (Exception ex) {
                        if ( Marshal.GetHRForException(ex) == SSL_ERROR_CODE && message != null && theClient != null) {  
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

        public void SendEmail(string subject, string text, bool important, bool html) {
            SendMessage(subject, text, important, html);
        }

        #endregion
    }
}
