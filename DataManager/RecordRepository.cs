using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataManager
{
    internal class RecordRepository : IRecordRepository
    {
        private readonly AppDbContext _appDbContext;

        public RecordRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<Record>> GetRecordsAsync()
        {
            return await _appDbContext.Records.ToListAsync();
        }

        public async Task<IEnumerable<Record>> SearchAsync(FilterForm filterForm)
        {
            IQueryable<Record> query = _appDbContext.Records;

            if (!string.IsNullOrEmpty(filterForm.FirstName))
            {
                query = query.Where(r => r.FirstName.Contains(filterForm.FirstName));
            }

            if (!string.IsNullOrEmpty(filterForm.LastName))
            {
                query = query.Where(r => r.LastName.Contains(filterForm.LastName));
            }

            if (!string.IsNullOrEmpty(filterForm.City))
            {
                query = query.Where(r => r.City.Contains(filterForm.City));
            }

            if (!string.IsNullOrEmpty(filterForm.Country))
            {
                query = query.Where(r => r.Country.Contains(filterForm.Country));
            }

            if (filterForm.FromDate.HasValue && filterForm.ToDate.HasValue)
            {
                query = query.Where(r => r.Date >= filterForm.FromDate && r.Date <= filterForm.ToDate);
            }
            else if (filterForm.FromDate.HasValue)
            {
                query = query.Where(r => r.Date >= filterForm.FromDate);
            }
            else if (filterForm.ToDate.HasValue)
            {
                query = query.Where(r => r.Date <= filterForm.ToDate);
            }

            return await query.ToListAsync();
        }
    }
}
