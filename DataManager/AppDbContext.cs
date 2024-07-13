using System.Data.Entity;

namespace DataManager
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Record> Records { get; set; }

        public AppDbContext() : base("name=AppDbContext")
        {
        }

        static AppDbContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
        }
    }
}
