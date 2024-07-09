using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataManager
{
    internal interface IRecordRepository
    {
        Task<IEnumerable<Record>> GetRecords();
        Task<IEnumerable<Record>> Search(FilterForm filterForm);
    }
}
