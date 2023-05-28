using System.Collections.Generic;
using System.Xml.Serialization;

namespace Report
{
    public class Country
    {
        private string name = string.Empty;
        public string Name
        {
            get => name;
            set => name = value ?? string.Empty;
        }

        private byte wave = 1;
        public byte Wave
        {
            get => wave;
            set => wave = value == 0 ? (byte)1 : value;
        }

        private byte servers = 1;
        public byte Servers
        {
            get => servers;
            set => servers = value == 0 ? (byte)1 : value;
        }

        private byte maximum = 1;
        public byte Maximum
        {
            get => maximum;
            set => maximum = value == 0 ? (byte)1 : value;
        }

        private List<string> prefixes = new List<string>();
        [XmlArrayItem("P")]
        public List<string> Prefixes
        {
            get => prefixes;
            set => prefixes = value ?? new List<string>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
