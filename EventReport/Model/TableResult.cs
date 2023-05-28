using System;
using System.Globalization;

namespace Report
{
    internal class TableResult
    {
        private static CultureInfo enUS = new CultureInfo("en-US");

        public int? InternalTicketId { get; }

        public string CustomerName { get; }

        public string FirstOccurrence { get; }

        public string Summary { get; }

        public string Category { get; }

        public string Subcategory { get; }

        public int? OriginalSeverity { get; }

        public int? Severity { get; }

        public string LastOccurrence { get; }

        public string HostName { get; }

        public string HostStatus { get; }

        public int? Maintflag { get; }

        public int? Grade { get; }

        public string ClearedTimestamp { get; }

        public string CTAReceiveTime { get; }

        public string ManagingHost { get; }

        public int? Tally { get; }

        public int? ArticleId { get; }

        public string Queuename { get; }

        public string GEO { get; }

        public string Country { get; }

        public TableResult(string country, string[] values, string category, string subcategory)
        {
            Country = country;
            InternalTicketId = GetInt(values[1]);
            CustomerName = values[2];
            FirstOccurrence = GetDateTime(values[3]);
            Summary = values[4];
            Category = category;
            Subcategory = subcategory;
            OriginalSeverity = GetInt(values[5]);
            Severity = GetInt(values[6]);
            LastOccurrence = GetDateTime(values[7]);
            HostName = values[8];
            HostStatus = values[9];
            Maintflag = GetInt(values[12]);
            Grade = GetInt(values[13]);
            ClearedTimestamp = GetDateTime(values[14]);
            CTAReceiveTime = GetDateTime(values[15]);
            ManagingHost = values[16];
            Tally = GetInt(values[17]);
            ArticleId = GetInt(values[18]);
            Queuename = values[19];
            GEO = values[20];
        }

        public int? GetInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            return int.Parse(value);
        }

        public string GetDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            string v = value.Replace(" COT", "");
            DateTime time = DateTime.ParseExact(v, "dd-MM-yyyy HH:mm:ss", enUS, DateTimeStyles.None);
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
