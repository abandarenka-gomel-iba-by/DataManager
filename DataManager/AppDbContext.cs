using System.Data.Entity;

namespace DataManager
{
    internal class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=AppDbContext")
        {
        }

        public DbSet<Record> Records { get; set; }

        static AppDbContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
        }
    }
}
