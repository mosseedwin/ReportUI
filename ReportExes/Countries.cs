using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Report
{
    [XmlRoot("Countries")]
    public class Countries : List<Country>
    {
        private static readonly string STARTUP = Path.GetDirectoryName(typeof(Countries).Assembly.Location);

        private static readonly string FILENAME = Path.Combine(STARTUP, "countries.xml");
        private static XmlSerializer _Serializer;

        private static XmlSerializer Serializer
        {
            get
            {
                return _Serializer ?? (_Serializer = new XmlSerializer(typeof(Countries)));
            }
        }

        public static Countries Load()
        {
            if (!File.Exists(FILENAME))
            {
                FileStream writer = File.Create(FILENAME);
                Serializer.Serialize(writer, new Countries());
                writer.Flush();
                writer.Dispose();
            }
            try
            {
                FileStream reader = File.OpenRead(FILENAME);
                Countries countries = Serializer.Deserialize(reader) as Countries;
                reader.Dispose();
                return countries;
            }
            catch
            {
                return new Countries();
            }
        }

        internal void Save()
        {
            if (File.Exists(FILENAME))
            {
                try
                { File.Delete(FILENAME); }
                catch { }
            }
            try
            {
                FileStream writer = File.Create(FILENAME);
                Serializer.Serialize(writer, this);
                writer.Flush();
                writer.Dispose();
            }
            catch
            {
            }
        }
    }
}
