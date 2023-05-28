using System.Collections.Generic;
using System.Xml.Serialization;

namespace Report
{
    public class Category
    {
        private string _Type = string.Empty;

        [XmlElement("Key")]
        public string Name
        {
            get { return _Type; }
            set { _Type = value ?? string.Empty; }
        }

        private string _Subcategory = string.Empty;

        [XmlElement("Subtype")]
        public string Subcategory
        {
            get { return _Subcategory; }
            set { _Subcategory = value ?? string.Empty; }
        }

        private List<string> _Prefixes = new List<string>();

        public List<string> Prefixes
        {
            get { return _Prefixes; }
            set { _Prefixes = value ?? new List<string>(); }
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Subcategory))
            {
                return Name;
            }
            else
            {
                return Name + " : " + Subcategory;
            }
        }
    }
}
