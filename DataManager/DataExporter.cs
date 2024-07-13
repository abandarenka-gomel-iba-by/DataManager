using ClosedXML.Excel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataManager
{
    internal class DataExporter
    {
        public async Task ExportToExcelAsync(IEnumerable<Record> records, string filePath)
        {
            await Task.Run(() =>
            {
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Records");

                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "FirstName";
                worksheet.Cell(1, 3).Value = "LastName";
                worksheet.Cell(1, 4).Value = "SurName";
                worksheet.Cell(1, 5).Value = "City";
                worksheet.Cell(1, 6).Value = "Country";

                for (int i = 0; i < records.Count(); i++)
                {
                    var record = records.ElementAt(i);
                    worksheet.Cell(i + 2, 1).Value = record.Date;
                    worksheet.Cell(i + 2, 2).Value = record.FirstName;
                    worksheet.Cell(i + 2, 3).Value = record.LastName;
                    worksheet.Cell(i + 2, 4).Value = record.SurName;
                    worksheet.Cell(i + 2, 5).Value = record.City;
                    worksheet.Cell(i + 2, 6).Value = record.Country;
                }

                workbook.SaveAs(filePath);
            });
        }

        public async Task ExportToXmlAsync(IEnumerable<Record> records, string filePath)
        {
            await Task.Run(() =>
            {
                var xDocument = new XDocument(
                    new XElement("DataManager",
                        records.Select(record =>
                            new XElement("Record",
                                new XAttribute("id", record.Id),
                                new XElement("Date", record.Date),
                                new XElement("FirstName", record.FirstName),
                                new XElement("LastName", record.LastName),
                                new XElement("SurName", record.SurName),
                                new XElement("City", record.City),
                                new XElement("Country", record.Country)
                            )
                        )
                    )
                );

                xDocument.Save(filePath);
            });
        }
    }
}
