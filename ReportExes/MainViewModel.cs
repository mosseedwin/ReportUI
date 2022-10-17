using Microsoft.VisualBasic.FileIO;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace Report
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private static CultureInfo enUS = new CultureInfo("en-US");

        private const string EVENT_TOKEN = "First Occurrence,Event Server Name,Event Server Serial,Severity Name,Host Name,Host uuid,Target Name,Target uuid,Tracking Code,Tracking Code Description,Internal Ticket Id,Maintflag,Target Status,Host Status";

        private const int EVENT_TOKEN_COUNT = 10;

        private const int EVENT_SERVER_INDEX = 4;

        private const int EVENT_DATETIME_INDEX = 0;

        private const string CHANGES_TOKEN = "\"Ticket ID\",\"Creator\",\"Creation Date\",\"Status\",\"Priority\",\"Category\",\"Classification\",\"SubClass\",\"Customer\",\"Owner\",\"Implemented By\",\"Implementer\",\"Implementer Contact\",\"Support Resource\",\"Contact Info\",\"Risk\",\"Filter\",\"Timing is Critical\",\"Monitoring Impacted\",\"Inventory Impacted\",\"Backups Impacted\",\"Recurring\",\"Recurring Period\",\"Infinite Recurring\",\"Recurring End Date\",\"Scheduled Start Date\",\"Scheduled Duration\",\"Requested Date\",\"Implementation Start Date\",\"Implementation End Date\",\"Summary\",\"Affected CIs\",\"Related Incident ID\",\"External ID\"";

        private const int CHANGES_TOKEN_COUNT = 31;

        private const int CHANGES_SERVER_INDEX = 31;

        private const int CHANGES_DATETIME_INDEX = 2;

        private const string INCIDENTS_TOKEN = "Row No,Customer Name,Incident Ticket Id,Ticket Creation Date,Severity,Status,Incident Summary,CI Name,MOS SR IDs,CI Group,";

        private const int INCIDENTS_TOKEN_COUNT = 10;

        private const int INCIDENTS_SUMMARY_INDEX = 6;

        private const int INCIDENTS_SERVER_INDEX = 7;

        private const int INCIDENTS_DATETIME_INDEX = 3;

        private const string AVAILIABILITY_TOKEN = "ComputerSystem,Target CI,Probe Type,Probe Name,";

        private const int AVAILIABILITY_TOKEN_COUNT = 60;

        private const int AVAILIABILITY_SERVER_INDEX = 0;

        private const string AVAILIABILITY_PREFIX = "Availability - ";

        private DateTime initial = DateTime.Today;
        private DateTime final = DateTime.Today;
        private string eventsFile;
        private string incidentsFile;
        private string changesFile;
        private string availabilityFile;
        private string outputFile;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DateTime Initial
        {
            get => initial;
            set
            {
                if (initial != value)
                {
                    initial = value;
                    OnPropertyChanged(nameof(Initial));
                }
            }
        }
        public DateTime Final
        {
            get => final; set
            {
                if (final != value)
                {
                    final = value;
                    OnPropertyChanged(nameof(Final));
                }
            }
        }
        public string EventsFile
        {
            get => eventsFile; set
            {
                if (eventsFile != value)
                {
                    eventsFile = value;
                    OnPropertyChanged(nameof(EventsFile));
                }
            }
        }

        public string IncidentsFile
        {
            get => incidentsFile;
            set
            {
                if (incidentsFile != value)
                {
                    incidentsFile = value;
                    OnPropertyChanged(nameof(IncidentsFile));
                }
            }
        }

        public string ChangesFile
        {
            get => changesFile;
            set
            {
                if (changesFile != value)
                {
                    changesFile = value;
                    OnPropertyChanged(nameof(ChangesFile));
                }
            }
        }

        public string AvailabilityFile
        {
            get => availabilityFile;
            set
            {
                if (availabilityFile != value)
                {
                    availabilityFile = value;
                    OnPropertyChanged(nameof(AvailabilityFile));
                }
            }
        }

        public string OutputFile
        {
            get => outputFile;
            set
            {
                if (outputFile != value)
                {
                    outputFile = value;
                    OnPropertyChanged(nameof(OutputFile));
                }
            }
        }

        public void Start(Countries countries, Categories categories)
        {
            if (string.IsNullOrWhiteSpace(EventsFile))
            {
                MessageBox.Show("Es necesario el archivo de eventos.", "", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrWhiteSpace(IncidentsFile))
            {
                MessageBox.Show("Es necesario el archivo de incidentes.", "", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrWhiteSpace(ChangesFile))
            {
                MessageBox.Show("Es necesario el archivo de cambios.", "", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrWhiteSpace(AvailabilityFile))
            {
                MessageBox.Show("Es necesario el archivo de disponibilidad.", "", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrWhiteSpace(OutputFile))
            {
                MessageBox.Show("Es necesario el archivo de salida.", "", MessageBoxButton.OK);
                return;
            }
            if (!File.Exists(EventsFile))
            {
                MessageBox.Show("No existe archivo de eventos.", "", MessageBoxButton.OK);
                return;
            }
            if (!File.Exists(IncidentsFile))
            {
                MessageBox.Show("No existe archivo de incidentes.", "", MessageBoxButton.OK);
                return;
            }
            if (!File.Exists(ChangesFile))
            {
                MessageBox.Show("No existe archivo de cambios.", "", MessageBoxButton.OK);
                return;
            }
            if (!File.Exists(AvailabilityFile))
            {
                MessageBox.Show("No existe archivo de disponibilidad.", "", MessageBoxButton.OK);
                return;
            }

            Initial = new DateTime(Initial.Year, Initial.Month, Initial.Day, 0, 0, 0, DateTimeKind.Unspecified);
            Final = new DateTime(Final.Year, Final.Month, Final.Day, 23, 59, 59, DateTimeKind.Unspecified);
            if (Initial > Final)
            {
                MessageBox.Show("La fecha final debe ser mayor a la fecha inicial.", "", MessageBoxButton.OK);
                return;
            }

            Dictionary<string, Country> countriesByName = new Dictionary<string, Country>();
            Dictionary<string, string> temp = new Dictionary<string, string>()
            {
                { string.Empty, string.Empty }
            };
            foreach (Country country in countries)
            {
                countriesByName[country.Name] = country;
                foreach (string prefix in country.Prefixes)
                {
                    temp[prefix] = country.Name;
                }
            }

            Dictionary<string, string> countryPrefixes = new Dictionary<string, string>();
            string[] keys = temp.Keys.OrderByDescending(a => a.Length).ToArray();
            foreach (string key in keys)
            {
                countryPrefixes[key] = temp[key];
            }

            temp = new Dictionary<string, string>();
            foreach (Category category in categories)
            {
                foreach (string prefix in category.Prefixes)
                {
                    temp[prefix] = category.Key;
                }
            }

            Dictionary<string, string> categoryPrefixes = new Dictionary<string, string>();
            keys = temp.Keys.OrderByDescending(a => a.Length).ToArray();
            foreach (string key in keys)
            {
                categoryPrefixes[key] = temp[key];
            }

            if (!VerificeContent(out string eventsContent, out string incidentsContent, out string changesContent, out string availabilityHeader, out string availabilityContent))
            {
                return;
            }
            if (!VerificeServers(countryPrefixes, eventsContent, incidentsContent, changesContent, availabilityContent,
                out Dictionary<string, List<string[]>> eventsGrouped, out Dictionary<string, List<string[]>> incidentsGrouped, out Dictionary<string, List<string[]>> changesGrouped, out Dictionary<string, List<string[]>> availabilityGrouped))
            {
                return;
            }

            VerificeDateTime(eventsGrouped, incidentsGrouped, changesGrouped, availabilityHeader, availabilityGrouped,
            out Dictionary<string, Dictionary<DateTime, List<string[]>>> eventsByDateTime, out Dictionary<string, Dictionary<DateTime, List<string[]>>> incidentsByDateTime, out Dictionary<string, Dictionary<DateTime, List<string[]>>> changesByDateTime, out Dictionary<string, Dictionary<DateTime, double>> availiablityByDateTime);

            IEnumerable<TableResult> result = GetGeneralTable(countriesByName, eventsByDateTime, incidentsByDateTime, changesByDateTime, availiablityByDateTime);
            IEnumerable<string[]> events = GetEventInformation(eventsGrouped);
            IEnumerable<string[]> incidents = GetIncidentInformation(categoryPrefixes, incidentsGrouped);

            ExcelExport excelExport = new ExcelExport();
            excelExport.WriteTableResult(result);
            excelExport.WriteEventInformation(events);
            excelExport.WriteIncidentInformation(incidents);
            excelExport.WriteAndClose(OutputFile);

            MessageBox.Show("Finalizado", "", MessageBoxButton.OK);
        }

        private IEnumerable<TableResult> GetGeneralTable(Dictionary<string, Country> countriesByName, Dictionary<string, Dictionary<DateTime, List<string[]>>> eventsByDateTime, Dictionary<string, Dictionary<DateTime, List<string[]>>> incidentsByDateTime, Dictionary<string, Dictionary<DateTime, List<string[]>>> changesByDateTime, Dictionary<string, Dictionary<DateTime, double>> availiablityByDateTime)
        {
            Dictionary<string, TableResult> result = new Dictionary<string, TableResult>();
            foreach (Country country in countriesByName.Values)
            {
                string countryName = country.Name;
                result[country.Name] = new TableResult(country);
                if (!eventsByDateTime.TryGetValue(countryName, out Dictionary<DateTime, List<string[]>> eventValueByDateTime))
                {
                    eventValueByDateTime = new Dictionary<DateTime, List<string[]>>();
                }
                if (!incidentsByDateTime.TryGetValue(countryName, out Dictionary<DateTime, List<string[]>> incidentValueByDateTime))
                {
                    incidentValueByDateTime = new Dictionary<DateTime, List<string[]>>();
                }
                if (!changesByDateTime.TryGetValue(countryName, out Dictionary<DateTime, List<string[]>> changeValueByDateTime))
                {
                    changeValueByDateTime = new Dictionary<DateTime, List<string[]>>();
                }
                if (!availiablityByDateTime.TryGetValue(countryName, out Dictionary<DateTime, double> availaibilityValueByDateTime))
                {
                    availaibilityValueByDateTime = new Dictionary<DateTime, double>();
                }
                TableResult tableResult = result[countryName];
                tableResult.Events = Count(eventValueByDateTime);
                tableResult.Incidents = Count(incidentValueByDateTime);
                tableResult.Changes = Count(changeValueByDateTime);
                tableResult.Availability = Availability(availaibilityValueByDateTime);
            }
            return result.Values;
        }

        private IEnumerable<string[]> GetEventInformation(Dictionary<string, List<string[]>> eventsGrouped)
        {
            string headerContent = EVENT_TOKEN + ",Country";
            List<string[]> row = new List<string[]>
            {
                headerContent.Split(',')
            };
            foreach (var tuple in eventsGrouped)
            {
                string countryName = tuple.Key;
                foreach (var values in tuple.Value)
                {
                    string[] newValue = new string[values.Length + 1];
                    for (int i = 0; i < values.Length; i++)
                    {
                        newValue[i] = values[i];
                    }
                    newValue[values.Length] = countryName;
                    row.Add(newValue);
                }
            }
            return row;
        }

        private static IEnumerable<string[]> GetIncidentInformation(Dictionary<string, string> categoriesPrefixes, Dictionary<string, List<string[]>> incidents)
        {
            var incidentsList = new List<string[]>();
            foreach (var tuple in incidents)
            {
                foreach (string[] value in tuple.Value)
                {
                    string[] newValue = new string[value.Length + 2];
                    for (int i = 0; i < value.Length; i++)
                    {
                        newValue[i] = value[i];
                    }
                    newValue[value.Length + 1] = tuple.Key;
                    string summary = newValue[INCIDENTS_SUMMARY_INDEX];
                    foreach (var item in categoriesPrefixes)
                    {
                        if (summary.Contains(item.Key))
                        {
                            newValue[value.Length] = item.Value;
                            break;
                        }
                    }
                    incidentsList.Add(newValue);
                }
            }

            string headerContent = INCIDENTS_TOKEN + ",Type,Country";
            List<string[]> row = new List<string[]>
            {
                headerContent.Split(',')
            };

            row.AddRange(incidentsList.OrderBy(a => a[0]));
            return row;
        }

        private int Count(Dictionary<DateTime, List<string[]>> collection)
        {
            int count = 0;
            foreach (List<string[]> item in collection.Values)
            {
                count += item.Count;
            }
            return count;
        }

        private double Availability(Dictionary<DateTime, double> availiablity)
        {
            int count = 0;
            double acum = 0;
            foreach (double value in availiablity.Values)
            {
                count++;
                acum += value;
            }
            return count == 0 ? 0 : (acum / count);
        }

        private bool VerificeContent(out string eventsContent, out string incidentsContent, out string changesContent, out string availabilityHeader, out string availabilityContent)
        {
            incidentsContent = string.Empty;
            changesContent = string.Empty;
            availabilityHeader = string.Empty;
            availabilityContent = string.Empty;

            if (!GetNewContent(EventsFile, EVENT_TOKEN, out string _, out eventsContent))
            {
                MessageBox.Show("El formato de archivo de eventos no es el esperado.", "", MessageBoxButton.OK);
                return false;
            }
            if (!GetNewContent(IncidentsFile, INCIDENTS_TOKEN, out string _, out incidentsContent))
            {
                MessageBox.Show("El formato de archivo de incidentes no es el esperado.", "", MessageBoxButton.OK);
                return false;
            }
            if (!GetNewContent(ChangesFile, CHANGES_TOKEN, out string _, out changesContent))
            {
                MessageBox.Show("El formato de archivo de cambios no es el esperado.", "", MessageBoxButton.OK);
                return false;
            }
            if (!GetNewContent(AvailabilityFile, AVAILIABILITY_TOKEN, out availabilityHeader, out availabilityContent))
            {
                MessageBox.Show("El formato de archivo de disponibilidad no es el esperado.", "", MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private bool VerificeServers(Dictionary<string, string> countryPrefixes, string eventsContent, string incidentsContent, string changesContent, string availabilityContent,
            out Dictionary<string, List<string[]>> eventsGrouped, out Dictionary<string, List<string[]>> incidentsGrouped, out Dictionary<string, List<string[]>> changesGrouped, out Dictionary<string, List<string[]>> availabilityGrouped)
        {
            List<string> serverNotFound = new List<string>();
            eventsGrouped = GroupByServerName(countryPrefixes, eventsContent, EVENT_TOKEN_COUNT, EVENT_SERVER_INDEX, serverNotFound);
            incidentsGrouped = GroupByServerName(countryPrefixes, incidentsContent, INCIDENTS_TOKEN_COUNT, INCIDENTS_SERVER_INDEX, serverNotFound);
            changesGrouped = GroupByServerName(countryPrefixes, changesContent, CHANGES_TOKEN_COUNT, CHANGES_SERVER_INDEX, serverNotFound);
            availabilityGrouped = GroupByServerName(countryPrefixes, availabilityContent, AVAILIABILITY_TOKEN_COUNT, AVAILIABILITY_SERVER_INDEX, serverNotFound);
            if (serverNotFound.Count > 0)
            {
                string text = "Servidores sin país.\n";
                foreach (string item in serverNotFound)
                {
                    text += item;
                    text += "\n";
                }
                text += "¿Desea continuar?";
                if (MessageBox.Show(text, "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return false;
                }
            }
            return true;
        }

        private void VerificeDateTime(Dictionary<string, List<string[]>> eventsGrouped, Dictionary<string, List<string[]>> incidentsGrouped, Dictionary<string, List<string[]>> changesGrouped, string availabilityHeader, Dictionary<string, List<string[]>> availabilityGrouped,
            out Dictionary<string, Dictionary<DateTime, List<string[]>>> eventsByDateTime, out Dictionary<string, Dictionary<DateTime, List<string[]>>> incidentsByDateTime, out Dictionary<string, Dictionary<DateTime, List<string[]>>> changesByDateTime, out Dictionary<string, Dictionary<DateTime, double>> availiablityByDateTime)
        {
            eventsByDateTime = GroupByDateTime(Initial, Final, eventsGrouped, EVENT_DATETIME_INDEX);
            incidentsByDateTime = GroupByDateTime(Initial, Final, incidentsGrouped, INCIDENTS_DATETIME_INDEX);
            changesByDateTime = GroupByDateTime(Initial, Final, changesGrouped, CHANGES_DATETIME_INDEX);
            availiablityByDateTime = GroupByDateTime(Initial, Final, availabilityHeader, availabilityGrouped);
        }

        private static bool GetNewContent(string fileName, string token, out string header, out string newContent)
        {
            header = string.Empty;
            newContent = string.Empty;
            if (!File.Exists(fileName))
            {
                return false;
            }
            string[] lines = File.ReadAllLines(fileName);
            StringBuilder builder = new StringBuilder();
            bool first = false;
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (!first)
                {
                    if (line.StartsWith(token))
                    {
                        first = true;
                        header = line;
                    }
                    continue;
                }
                builder.AppendLine(line);
            }
            newContent = builder.ToString();
            return newContent.Length > 0;
        }

        private static Dictionary<string, List<string[]>> GroupByServerName(Dictionary<string, string> prefixes, string content, int minimumLength, int groupIndex, List<string> serverNotFound)
        {
            List<string> serverNames = new List<string>();
            Dictionary<string, List<string[]>> groupsByServer = new Dictionary<string, List<string[]>>();
            TextFieldParser csvParser = new TextFieldParser(new StringReader(content));
            csvParser.HasFieldsEnclosedInQuotes = true;
            csvParser.SetDelimiters(",");
            while (!csvParser.EndOfData)
            {
                string[] values = csvParser.ReadFields();
                if (values.Length <= minimumLength)
                {
                    continue;
                }
                string serverName = values[groupIndex];
                if (string.IsNullOrWhiteSpace(serverName))
                {
                    continue;
                }
                if (!serverNames.Contains(serverName))
                {
                    serverNames.Add(serverName);
                }
                if (!groupsByServer.TryGetValue(serverName, out List<string[]> items))
                {
                    items = new List<string[]>();
                    groupsByServer[serverName] = items;
                }
                items.Add(values);
            }

            Dictionary<string, List<string[]>> groupsByCountry = new Dictionary<string, List<string[]>>();
            foreach (string serverName in serverNames)
            {
                string prefix = string.Empty;
                foreach (string reference in prefixes.Keys)
                {
                    if (string.IsNullOrWhiteSpace(reference))
                    {
                        continue;
                    }
                    if (serverName.StartsWith(reference))
                    {
                        prefix = reference;
                        break;
                    }
                }
                List<string[]> values = groupsByServer[serverName];
                if (string.IsNullOrWhiteSpace(prefix))
                {
                    if (!serverNotFound.Contains(serverName))
                    {
                        serverNotFound.Add(serverName);
                    }
                }
                string countryName = prefixes[prefix];
                if (!groupsByCountry.TryGetValue(countryName, out List<string[]> items))
                {
                    items = new List<string[]>();
                    groupsByCountry[countryName] = items;
                }
                items.AddRange(values);
            }
            return groupsByCountry;
        }

        private static Dictionary<string, Dictionary<DateTime, List<string[]>>> GroupByDateTime(DateTime startTime, DateTime endTime, Dictionary<string, List<string[]>> group, int dateTimeIndex)
        {
            Dictionary<string, Dictionary<DateTime, List<string[]>>> result = new Dictionary<string, Dictionary<DateTime, List<string[]>>>();
            foreach (KeyValuePair<string, List<string[]>> item in group)
            {
                Dictionary<DateTime, List<string[]>> valuesByDateTime = new Dictionary<DateTime, List<string[]>>();
                result[item.Key] = valuesByDateTime;
                foreach (string[] values in item.Value)
                {
                    string time = values[dateTimeIndex].Trim('"').Replace(" COT", "").Replace(" GMT-05:00", "");
                    if (!DateTime.TryParseExact(time, "dd-MM-yyyy HH:mm:ss", enUS, DateTimeStyles.None, out DateTime dateTime))
                    {
                        continue;
                    }
                    if (dateTime < startTime || dateTime > endTime)
                    {
                        continue;
                    }
                    DateTime reference = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Unspecified);
                    if (!valuesByDateTime.TryGetValue(reference, out List<string[]> records))
                    {
                        records = new List<string[]>();
                        valuesByDateTime[reference] = records;
                    }
                    records.Add(values);
                }
            }
            return result;
        }

        private static Dictionary<string, Dictionary<DateTime, double>> GroupByDateTime(DateTime startTime, DateTime endTime, string headerText, Dictionary<string, List<string[]>> group)
        {
            Dictionary<int, DateTime> columns = new Dictionary<int, DateTime>();
            string[] headers = headerText.Split(',');
            for (int i = 0; i < headers.Length; i++)
            {
                string header = headers[i];
                if (!header.StartsWith(AVAILIABILITY_PREFIX))
                {
                    continue;
                }
                string time = header.Replace(AVAILIABILITY_PREFIX, "");
                if (!DateTime.TryParseExact(time, "yyyy-MM-dd", enUS, DateTimeStyles.None, out DateTime dateTime))
                {
                    continue;
                }
                if (dateTime < startTime || dateTime > endTime)
                {
                    continue;
                }
                columns[i] = dateTime;
            }
            Dictionary<string, Dictionary<DateTime, List<double>>> groupByDateTime = new Dictionary<string, Dictionary<DateTime, List<double>>>();
            foreach (KeyValuePair<string, List<string[]>> item in group)
            {
                Dictionary<DateTime, List<double>> valuesByDateTime = new Dictionary<DateTime, List<double>>();
                groupByDateTime[item.Key] = valuesByDateTime;
                foreach (string[] values in item.Value)
                {
                    foreach (KeyValuePair<int, DateTime> tuple in columns)
                    {
                        int columnIndex = tuple.Key;
                        DateTime dateTime = tuple.Value;
                        if (!valuesByDateTime.TryGetValue(dateTime, out List<double> records))
                        {
                            records = new List<double>();
                            valuesByDateTime[dateTime] = records;
                        }
                        string value = values[columnIndex];
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            continue;
                        }
                        string v = value.Replace(".", ",");
                        if (!double.TryParse(v, out double @double))
                        {
                            continue;
                        }
                        records.Add(@double);
                    }
                }
            }
            Dictionary<string, Dictionary<DateTime, double>> result = new Dictionary<string, Dictionary<DateTime, double>>();
            foreach (var item in groupByDateTime)
            {
                Dictionary<DateTime, double> valuesByDateTime = new Dictionary<DateTime, double>();
                result[item.Key] = valuesByDateTime;
                foreach (KeyValuePair<DateTime, List<double>> tuple in item.Value)
                {
                    int count = 0;
                    double acum = 0;
                    foreach (var value in tuple.Value)
                    {
                        acum += value;
                        count++;
                    }
                    if (count == 0)
                    {
                        valuesByDateTime[tuple.Key] = 0;
                    }
                    else
                    {
                        valuesByDateTime[tuple.Key] = acum / count;
                    }
                }
            }
            return result;
        }
    }
}
