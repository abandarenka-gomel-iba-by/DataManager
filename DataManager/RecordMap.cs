using CsvHelper.Configuration;

namespace DataManager
{
    internal class RecordMap : ClassMap<Record>
    {
        public RecordMap()
        {
            Map(m => m.Date).Name("Date");
            Map(m => m.FirstName).Name("FirstName");
            Map(m => m.LastName).Name("LastName");
            Map(m => m.SurName).Name("SurName");
            Map(m => m.City).Name("City");
            Map(m => m.Country).Name("Country");
        }
    }
}
