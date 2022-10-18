using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Report
{
    [XmlRoot("Categories")]
    public class Categories : List<Category>
    {
        private static readonly string STARTUP = Path.GetDirectoryName(typeof(Categories).Assembly.Location);

        private static readonly string FILENAME = Path.Combine(STARTUP, "categories.xml");
        private static XmlSerializer _Serializer;

        private static XmlSerializer Serializer
        {
            get
            {
                return _Serializer ?? (_Serializer = new XmlSerializer(typeof(Categories)));
            }
        }

        public static Categories Load()
        {
            if (!File.Exists(FILENAME))
            {
                FileStream writer = File.Create(FILENAME);
                Serializer.Serialize(writer, new Categories());
                writer.Flush();
                writer.Dispose();
            }
            try
            {
                FileStream reader = File.OpenRead(FILENAME);
                Categories categories = Serializer.Deserialize(reader) as Categories;
                reader.Dispose();
                return categories;
            }
            catch
            {
                return new Categories();
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
