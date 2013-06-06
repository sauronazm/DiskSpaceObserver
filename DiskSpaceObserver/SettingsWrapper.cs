using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DiskSpaceObserver {
    class SettingsWrapper {

        public class DiscsConfigurationSectionWrapper {
            class DiscsConfigurationSectionKeys {
                public const string SECTION_NAME = "DiscsConfiguration";
            }

            public static DiscsConfigurationSection GetDiskConfigurationSection() {
                return ConfigurationManager.GetSection(DiscsConfigurationSectionKeys.SECTION_NAME) as DiscsConfigurationSection;
            }
        }

        public class AppSettingWrapper {
            public class AppSettingsKeys {
                public const string MAIL_FROM_USERNAME = "MailFromUsername";
                public const string MAIL_FROM_SERVER = "MailFromServer";
                public const string MAIL_FROM_PASSWORD = "MailFromPassword";
                public const string MAIL_TO = "MailTo";
                public const string MAIL_SUBJECT = "MailSubject";
                public const string MAIL_SEND_ALWAYS = "MailSendAlways";
                public const string MAIL_USE_SSL = "MailUseSSL";
                public const string MAIL_USE_HTML = "MailUseHTML";
            }

            public static string GetMailFromUsername() {
                return ConfigurationManager.AppSettings[AppSettingsKeys.MAIL_FROM_USERNAME];
            }
            public static string GetMailFromServer() {
                return ConfigurationManager.AppSettings[AppSettingsKeys.MAIL_FROM_SERVER];
            }
            public static string GetMailFromPassword() {
                return ConfigurationManager.AppSettings[AppSettingsKeys.MAIL_FROM_PASSWORD];
            }
            public static string GetMailTo() {
                return ConfigurationManager.AppSettings[AppSettingsKeys.MAIL_TO];
            }
            public static string GetMailSubject() {
                return ConfigurationManager.AppSettings[AppSettingsKeys.MAIL_SUBJECT];
            }
            public static string GetMailSendAlways() {
                return ConfigurationManager.AppSettings[AppSettingsKeys.MAIL_SEND_ALWAYS];
            }
            public static string GetMailUseSSL() {
                return ConfigurationManager.AppSettings[AppSettingsKeys.MAIL_USE_SSL];
            }
            public static string GetMailUseHTML() {
                return ConfigurationManager.AppSettings[AppSettingsKeys.MAIL_USE_HTML];
            }
        }
    }
}
