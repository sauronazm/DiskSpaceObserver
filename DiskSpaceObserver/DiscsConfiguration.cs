using System.Configuration;

namespace DiskSpaceObserver {
    class DiscsConfigurationSection: ConfigurationSection {
        [ConfigurationProperty("DefaultLimit", IsRequired=false)]
        public int DefaultLimit {
            get { return (int)base["DefaultLimit"]; }
            set { base["DefaultLimit"] = value; }
        }

        [ConfigurationProperty("Discs")]
        public DiscsCollection DiscItems {
            get { return ((DiscsCollection)(base["Discs"])); }
        }
    }

    [ConfigurationCollection(typeof(DiscInfo))]
    public class DiscsCollection : ConfigurationElementCollection {
        protected override ConfigurationElement CreateNewElement() {
            return new DiscInfo();
        }
        protected override object GetElementKey(ConfigurationElement element) {
            return ((DiscInfo)element).Letter;
        }
    }

    public class DiscInfo: ConfigurationElement {
        [ConfigurationProperty("Letter", IsKey = true, IsRequired = true)]
        public string Letter {
            get { return ((string)(base["Letter"])); }
            set { base["Letter"] = value; }
        }
        [ConfigurationProperty("Limit", IsRequired = true)]
        public int Limit {
            get { return ((int)(base["Limit"])); }
            set { base["Limit"] = value; }
        }
    }
}
