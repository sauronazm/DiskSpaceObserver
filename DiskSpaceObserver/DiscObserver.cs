using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace DiskSpaceObserver {
    class DiscObserver {

        #region Constants

        static private int MEGABYTESDIVIDER = 1024 * 1024;

        #endregion

        #region Private Members

        private List<DiscInfo> _discsInfo = new List<DiscInfo>();
        private int _defaultLimit = 1000;
        private StringBuilder _message = new StringBuilder();
        private bool _allIsOk = true;

        #endregion

        #region Public Properties

        public string Message {
            get {
                return _message.ToString();
            }
        }
        public bool AllIsOk {
            get {
                return _allIsOk;
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
            }
            catch (Exception ex) {
                ErrorManager.ProcessError(ex, "Error while reading Disc Observer config.");
            }
        }

        private void AppendDiskWarning() {
            _message.Append("!!! ");
        }

        private void AppendDoesNotHaveDisksWarning() {
            _message.Append("Instance Does Not Have Disks!");
        }

        private void SetAsNotIsOk() {
            _allIsOk = false;
        }

        private void AppendDiskInfo(DriveInfo disk) {
            _message.Append("Disk: ")
                .Append(disk.Name)
                .Append(":")
                .Append(Environment.NewLine)
                .Append("  Total Space:")
                .Append(Environment.NewLine)
                .Append("    ")
                .Append(disk.TotalSize / MEGABYTESDIVIDER)
                .Append(" MB")
                .Append(Environment.NewLine)
                .Append("  Free Space:")
                .Append(Environment.NewLine)
                .Append("    ")
                .Append(disk.TotalFreeSpace / MEGABYTESDIVIDER)
                .Append(" MB")
                .Append(Environment.NewLine)
                .Append(Environment.NewLine);
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
                                    AppendDiskWarning();
                                }
                            }
                            else {
                                if (disk.TotalFreeSpace / MEGABYTESDIVIDER < _defaultLimit) {
                                    SetAsNotIsOk();
                                    AppendDiskWarning();
                                }
                            }
                            AppendDiskInfo(disk);
                        }
                    }
                }
                else {
                    SetAsNotIsOk();
                    AppendDoesNotHaveDisksWarning();
                }
            }
            catch (Exception ex) {
                ErrorManager.ProcessError(ex, "Error while getting discs info.");
            }
        }

        #endregion
    }
}
