using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace Report
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private const string EVENT_TOKEN = "Row No,Internal Ticket Id,Customer Name,First Occurrence,Summary,Original Severity,Severity,Last Occurrence,Host Name,Host Status,Target Name,Target Status,Maintflag,Grade,Cleared Timestamp,CTA Receive Time,Managing Host,Tally,Article Id,Queuename,GEO,Tracking Code,Tracking Code Description,Journal,";

        private const int EVENT_TOKEN_COUNT = 24;

        private const int EVENT_SERVER_INDEX = 8;

        private const int EVENT_SUMMARY_INDEX = 4;

        private string eventsFile;

        private string outputFile;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

            Dictionary<string, string> countryPrefixes = GetCountryPrefixes(countries);
            Dictionary<string, Tuple<string, string>> categoryPrefixes = GetCategoryPrefixes(categories);

            if (!VerificeContent(out string eventsContent))
            {
                return;
            }
            if (!VerificeServers(countryPrefixes, categoryPrefixes, eventsContent, out List<TableResult> eventsGrouped))
            {
                return;
            }

            ExcelExport excelExport = new ExcelExport();
            excelExport.WriteTableResult(eventsGrouped);
            excelExport.WriteAndClose(OutputFile);

            MessageBox.Show("Finalizado", "", MessageBoxButton.OK);
        }

        private static Dictionary<string, string> GetCountryPrefixes(Countries countries)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>()
            {
                { string.Empty, string.Empty }
            };
            foreach (Country country in countries)
            {
                foreach (string prefix in country.Prefixes)
                {
                    temp[prefix] = country.Name;
                }
            }
            Dictionary<string, string> countryPrefixes = new Dictionary<string, string>();
            foreach (string key in temp.Keys.OrderByDescending(a => a.Length).ToArray())
            {
                countryPrefixes[key] = temp[key];
            }
            return countryPrefixes;
        }

        private static Dictionary<string, Tuple<string, string>> GetCategoryPrefixes(Categories categories)
        {
            Dictionary<string, Tuple<string, string>> temp = new Dictionary<string, Tuple<string, string>>()
            {
                { string.Empty, new Tuple<string, string>(string.Empty, string.Empty) }
            };
            foreach (Category category in categories)
            {
                foreach (string prefix in category.Prefixes)
                {
                    temp[prefix] = new Tuple<string, string>(category.Name, category.Subcategory);
                }
            }
            Dictionary<string, Tuple<string, string>> categoryPrefixes = new Dictionary<string, Tuple<string, string>>();
            foreach (string key in temp.Keys.OrderByDescending(a => a.Length).ToArray())
            {
                categoryPrefixes[key] = temp[key];
            }
            return categoryPrefixes;
        }

        private bool VerificeContent(out string eventsContent)
        {
            if (!GetNewContent(EventsFile, EVENT_TOKEN, out string _, out eventsContent))
            {
                MessageBox.Show("El formato de archivo de eventos no es el esperado.", "", MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private bool VerificeServers(Dictionary<string, string> countryPrefixes, Dictionary<string, Tuple<string, string>> categoryPrefixes, string eventsContent, out List<TableResult> eventsGrouped)
        {
            List<string> serverNotFound = new List<string>();
            Dictionary<string, List<string[]>> groups = GroupByServerName(countryPrefixes, eventsContent, EVENT_TOKEN_COUNT, EVENT_SERVER_INDEX, serverNotFound);
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
                    eventsGrouped = null;
                    return false;
                }
            }
            eventsGrouped = new List<TableResult>();
            foreach (KeyValuePair<string, List<string[]>> tuple in groups)
            {
                string countryName = tuple.Key;
                foreach (string[] value in tuple.Value)
                {
                    string summary = value[EVENT_SUMMARY_INDEX];
                    Tuple<string, string> prefix = new Tuple<string, string>(string.Empty, string.Empty);
                    foreach (KeyValuePair<string, Tuple<string, string>> categoryTuple in categoryPrefixes)
                    {
                        string key = categoryTuple.Key;
                        if (summary.Contains(key))
                        {
                            prefix = categoryTuple.Value;
                            break;
                        }
                    }
                    TableResult result = new TableResult(countryName, value, prefix.Item1, prefix.Item2);
                    eventsGrouped.Add(result);
                }
            }
            return true;
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
    }
}
