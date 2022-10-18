using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI;
using System;
using System.Collections.Generic;
using System.IO;

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
                    new Tuple<object, CellType>("Internal Ticket Id", CellType.String),
                    new Tuple<object, CellType>("Customer Name", CellType.String),
                    new Tuple<object, CellType>("First Occurrence", CellType.String),
                    new Tuple<object, CellType>("Summary", CellType.String),
                    new Tuple<object, CellType>("Category", CellType.String),
                    new Tuple<object, CellType>("EventCode", CellType.String),
                    new Tuple<object, CellType>("Original Severity", CellType.String),
                    new Tuple<object, CellType>("Severity", CellType.String),
                    new Tuple<object, CellType>("Last Occurrence", CellType.String),
                    new Tuple<object, CellType>("Host Name", CellType.String),
                    new Tuple<object, CellType>("Host Status", CellType.String),
                    new Tuple<object, CellType>("Maintflag", CellType.String),
                    new Tuple<object, CellType>("Grade", CellType.String),
                    new Tuple<object, CellType>("Cleared Timestamp", CellType.String),
                    new Tuple<object, CellType>("CTA Receive Time", CellType.String),
                    new Tuple<object, CellType>("ManagingHost", CellType.String),
                    new Tuple<object, CellType>("Tally", CellType.String),
                    new Tuple<object, CellType>("Article Id", CellType.String),
                    new Tuple<object, CellType>("Queuename", CellType.String),
                    new Tuple<object, CellType>("GEO", CellType.String),
                }
            };
            foreach (var record in summary)
            {
                values.Add(new Tuple<object, CellType>[]
                {
                    new Tuple<object, CellType>(record.InternalTicketId, CellType.Numeric),
                    new Tuple<object, CellType>(record.CustomerName, CellType.String),
                    new Tuple<object, CellType>(record.FirstOccurrence, CellType.String),
                    new Tuple<object, CellType>(record.Summary, CellType.String),
                    new Tuple<object, CellType>(record.Category, CellType.String),
                    new Tuple<object, CellType>(record.EventCode, CellType.String),
                    new Tuple<object, CellType>(record.OriginalSeverity, CellType.Numeric),
                    new Tuple<object, CellType>(record.Severity, CellType.Numeric),
                    new Tuple<object, CellType>(record.LastOccurrence, CellType.String),
                    new Tuple<object, CellType>(record.HostName, CellType.String),
                    new Tuple<object, CellType>(record.HostStatus, CellType.String),
                    new Tuple<object, CellType>(record.Maintflag, CellType.Numeric),
                    new Tuple<object, CellType>(record.Grade, CellType.Numeric),
                    new Tuple<object, CellType>(record.ClearedTimestamp, CellType.String),
                    new Tuple<object, CellType>(record.CTAReceiveTime, CellType.String),
                    new Tuple<object, CellType>(record.ManagingHost, CellType.String),
                    new Tuple<object, CellType>(record.Tally, CellType.Numeric),
                    new Tuple<object, CellType>(record.ArticleId, CellType.Numeric),
                    new Tuple<object, CellType>(record.Queuename, CellType.String),
                    new Tuple<object, CellType>(record.GEO, CellType.String),
                });
            }
            AddValues(sheet, values);
            sheet.SetActive(true);
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
                        if (cellContent.Item1 is null)
                        {
                            cell.SetCellType(CellType.Blank);
                        }
                        else
                        {
                            double value = Convert.ToDouble(cellContent.Item1);
                            cell.SetCellType(CellType.Numeric);
                            cell.SetCellValue(value);
                        }
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
