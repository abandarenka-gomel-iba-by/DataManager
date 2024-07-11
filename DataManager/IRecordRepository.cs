using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataManager
{
    internal interface IRecordRepository
    {
        Task<IEnumerable<Record>> GetRecordsAsync();
        Task<IEnumerable<Record>> SearchAsync(FilterForm filterForm);
    }
}
