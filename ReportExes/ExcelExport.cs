using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI;
using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.Formula.Functions;

namespace Report
{
    [Serializable]
    internal class ExcelExport
    {
        XSSFWorkbook Book { get; set; }

        public ExcelExport()
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            POIXMLProperties properties = workbook.GetProperties();
            properties.CoreProperties.Creator = "Diana Patricia Sanchez";
            Book = workbook;
        }

        ~ExcelExport()
        {
            try
            { Book.Close(); }
            catch
            { }
            Book = null;
        }

        public void WriteTableResult(IEnumerable<TableResult> summary)
        {
            ISheet sheet = Book.CreateSheet("Summary");
            List<Tuple<object, CellType>[]> values = new List<Tuple<object, CellType>[]>()
            {
                new Tuple<object, CellType>[]
                {
                    new Tuple<object, CellType>("Country", CellType.String),
                    new Tuple<object, CellType>("Wave", CellType.String),
                    new Tuple<object, CellType>("Servers", CellType.String),
                    new Tuple<object, CellType>("Events", CellType.String),
                    new Tuple<object, CellType>("Incidents", CellType.String),
                    new Tuple<object, CellType>("Changes", CellType.String),
                    new Tuple<object, CellType>("Availability", CellType.String),
                }
            };
            foreach (var record in summary)
            {
                values.Add(new Tuple<object, CellType>[]
                {
                    new Tuple<object, CellType>(record.Name, CellType.String),
                    new Tuple<object, CellType>(record.Wave, CellType.Numeric),
                    new Tuple<object, CellType>(record.ServerNumber, CellType.String),
                    new Tuple<object, CellType>(record.Events, CellType.Numeric),
                    new Tuple<object, CellType>(record.Incidents, CellType.Numeric),
                    new Tuple<object, CellType>(record.Changes, CellType.Numeric),
                    new Tuple<object, CellType>(record.Availability, CellType.Numeric),
                });
            }
            AddValues(sheet, values);
            sheet.SetActive(true);
        }

        internal void WriteEventInformation(IEnumerable<string[]> events)
        {
            ISheet sheet = Book.CreateSheet("Events");
            List<Tuple<object, CellType>[]> values = new List<Tuple<object, CellType>[]>();
            foreach (string[] record in events)
            {
                Tuple<object, CellType>[] row = new Tuple<object, CellType>[record.Length];
                for (int i = 0; i < record.Length; i++)
                {
                    row[i] = new Tuple<object, CellType>(record[i], CellType.String);
                }
                values.Add(row);
            }
            AddValues(sheet, values);
            sheet.SetActive(false);
        }

        internal void WriteIncidentInformation(IEnumerable<string[]> incidents)
        {
            ISheet sheet = Book.CreateSheet("Incidents");
            List<Tuple<object, CellType>[]> values = new List<Tuple<object, CellType>[]>();
            foreach (string[] record in incidents)
            {
                Tuple<object, CellType>[] row = new Tuple<object, CellType>[record.Length];
                for (int i = 0; i < record.Length; i++)
                {
                    row[i] = new Tuple<object, CellType>(record[i], CellType.String);
                }
                values.Add(row);
            }
            AddValues(sheet, values);
            sheet.SetActive(false);
        }

        private void AddValues(ISheet sheet, List<Tuple<object, CellType>[]> values)
        {
            int i = 0;
            foreach (Tuple<object, CellType>[] rowContent in values)
            {
                IRow row = sheet.CreateRow(i);
                int j = 0;
                foreach (Tuple<object, CellType> cellContent in rowContent)
                {
                    XSSFCell cell = (XSSFCell)row.CreateCell(j);
                    if (cellContent.Item2 == CellType.Numeric)
                    {
                        double value = Convert.ToDouble(cellContent.Item1);
                        cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(value);
                    }
                    else
                    {
                        cell.SetCellType(CellType.String);
                        cell.SetCellValue(cellContent.Item1 as string);
                    }
                    j++;
                }
                i++;
            }
        }

        public void WriteAndClose(string fileName)
        {
            FileStream fileWrite = new FileStream(fileName, FileMode.Create);
            Book.Write(fileWrite);
            fileWrite.Close();
            Book.Close();
        }
    }
}
