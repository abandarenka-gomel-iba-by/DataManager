using System.Data.Entity;

namespace DataManager
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
    }
}
