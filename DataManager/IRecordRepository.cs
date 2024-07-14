using System.Collections.Generic;

namespace DataManager
{
    internal interface IRecordRepository
    {
        IAsyncEnumerable<Record> GetRecordsAsync();
        IAsyncEnumerable<Record> SearchAsync(FilterForm filterForm);
    }
}
