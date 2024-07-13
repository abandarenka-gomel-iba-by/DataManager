using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataManager
{
    internal interface IRecordRepository
    {
        IAsyncEnumerable<Record> GetRecordsAsync();
        IAsyncEnumerable<Record> SearchAsync(FilterForm filterForm);
    }
}
