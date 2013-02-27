using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace DiskSpaceObserver {
    class DiscObserver {

        #region Constants

        static private int MEGABYTESDIVIDER = 1024 * 1024;

        #endregion

        #region Private Members

        private List<DiscInfo> _discsInfo = new List<DiscInfo>();
        private int _defaultLimit = 1000;
        private bool _allIsOk = true;
        private bool _useHTML;
        private IMessageBuilder _messageBuiler;

        #endregion

        #region Public Properties

        public string Message {
            get {
                return ((CommonMessageBuilder)_messageBuiler).Message;
            }
        }

        public bool AllIsOk {
            get {
                return _allIsOk;
            }
        }

        public bool UseHTML {
            get {
                return _useHTML;
            }
        }

        #endregion

        #region Private Methods

        private void ReadConfig() {
            try {
                DiscsConfigurationSection section = ConfigurationManager.GetSection("DiscsConfiguration") as DiscsConfigurationSection;
                if (section.DefaultLimit > 0) {
                    _defaultLimit = section.DefaultLimit;
                }
                if (section != null) {
                    foreach (DiscInfo item in section.DiscItems) {
                        _discsInfo.Add(item);
                    }
                }
                if (ConfigurationManager.AppSettings["MailUseHTML"] != null && ConfigurationManager.AppSettings["MailUseHTML"].ToLower() == "no") {
                    _useHTML = true;
                    _messageBuiler = new PlainTextMessageBuilder();
                }
                else {
                    _useHTML = true;
                    _messageBuiler = new HTMLMessageBuilder();
                }
            }
            catch (Exception ex) {
                ErrorManager.ProcessError(ex, "Error while reading Disc Observer config.");
            }
        }

        private void SetAsNotIsOk() {
            _allIsOk = false;
        }
        #endregion

        #region Public Methods

        public void CheckMachine() {
            try {
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                if (allDrives.Length > 0) {
                    ReadConfig();
                    foreach (DriveInfo disk in allDrives) {
                        if (disk.IsReady == true && disk.DriveType == DriveType.Fixed) {
                            var configuredDisk = _discsInfo.FirstOrDefault(x => x.Letter == disk.Name.Substring(0, 1).ToLower());
                            if (configuredDisk != null) {
                                if (disk.TotalFreeSpace / MEGABYTESDIVIDER < configuredDisk.Limit) {
                                    SetAsNotIsOk();
                                    _messageBuiler.AppendDiskWarning();
                                }
                            }
                            else {
                                if (disk.TotalFreeSpace / MEGABYTESDIVIDER < _defaultLimit) {
                                    SetAsNotIsOk();
                                    _messageBuiler.AppendDiskWarning();
                                }
                            }
                            _messageBuiler.AppendDiskInfo(disk);
                        }
                    }
                }
                else {
                    SetAsNotIsOk();
                    _messageBuiler.AppendDoesNotHaveDisksWarning();
                }
            }
            catch (Exception ex) {
                ErrorManager.ProcessError(ex, "Error while getting discs info.");
            }
        }

        #endregion

        #region Private Classes 

        //TODO: Вынести вложенные классы и интерфейс в отдельные файлы. 

        private abstract class CommonMessageBuilder {

            protected StringBuilder _message;

            public CommonMessageBuilder() {
                _message = new StringBuilder();
            }

            abstract public string Message {
                get;
            }
        }

        private class HTMLMessageBuilder :CommonMessageBuilder, IMessageBuilder {

            public HTMLMessageBuilder() {
                AppendStartTags();
            }

            public override string Message {
                get {
                    AppendEndTags();
                    return _message.ToString();
                }
            }

            public void AppendDiskWarning() {
                _message.Append("!!! ");
            }

            public void AppendDoesNotHaveDisksWarning() {
                _message.Append("<div style='color:red; border: 1px solid red; font-size: 2em;'>Instance Does Not Have Disks!</div>");
            }

            public void AppendDiskInfo(DriveInfo disk) {
                var totalMB = disk.TotalSize / MEGABYTESDIVIDER;
                var freeMB = disk.TotalFreeSpace / MEGABYTESDIVIDER;
                _message.Append("Disk: ")
                    .Append(disk.Name)
                    .Append(":<br/>")
                    .Append("&nbsp;&nbsp;Total Space:<br/>")
                    .Append("&nbsp;&nbsp;&nbsp;&nbsp;")
                    .Append(totalMB.ToString("#,##0"))
                    .Append(" MB<br/>")
                    .Append("&nbsp;&nbsp;Free Space:<br/>")
                    .Append("&nbsp;&nbsp;&nbsp;&nbsp;")
                    .Append(freeMB.ToString("#,##0"))
                    .Append(" MB<br/><br/>")
                    .AppendFormat("<div style='display:inline-block;background-color: red; width:{0}px;' title='{2}'>&nbsp;</div>" +
                                    "<div style='display:inline-block; background-color: blue; width:{1}px;' title='{3}'>&nbsp;</div>"
                                        , 300 * (1 - ((decimal)freeMB / (decimal)totalMB))
                                        , 300 * ((decimal)freeMB / (decimal)totalMB)
                                        , (totalMB - freeMB).ToString("#,##0") + " MB"
                                        , freeMB.ToString("#,##0") + " MB")
                    .Append("<br/>")
                    .Append("<br/>")
                    .Append("<br/>");
            }

            private void AppendEndTags() {
                _message.AppendLine("</body>")
                    .AppendLine("<html>");
            }

            private void AppendStartTags() {
                _message.AppendLine("<!DOCTYPE html>")
                    .AppendLine("<html>")
                    .AppendLine("<body>");
            }

        }

        private class PlainTextMessageBuilder :CommonMessageBuilder, IMessageBuilder {

            public override string Message {
                get {
                    return _message.ToString();
                }
            }

            public void AppendDiskWarning() {
                _message.Append("!!! ");
            }

            public void AppendDoesNotHaveDisksWarning() {
                _message.Append("Instance Does Not Have Disks!");
            }

            public void AppendDiskInfo(DriveInfo disk) { //TODO: Добавить отступы для чисел, чтобы было видно, на сколько порядков они отличаются. 
                var totalMB = disk.TotalSize / MEGABYTESDIVIDER;
                var freeMB = disk.TotalFreeSpace / MEGABYTESDIVIDER;
                _message.Append("Disk: ")
                    .Append(disk.Name)
                    .AppendLine(":")
                    .AppendLine("  Total Space:")
                    .Append("    ")
                    .Append(totalMB.ToString("#,##0"))
                    .AppendLine(" MB")
                    .AppendLine("  Free Space:")
                    .Append("    ")
                    .Append(freeMB.ToString("#,##0"))
                    .Append(" MB")
                    .AppendLine()
                    .AppendLine();
            }

        }

        private interface IMessageBuilder {

            void AppendDiskWarning();
            void AppendDoesNotHaveDisksWarning();
            void AppendDiskInfo(DriveInfo disk);
        }

        #endregion
    }
}
