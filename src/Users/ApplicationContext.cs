using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace UsersApp
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSource Source { get; }

        public ApplicationContext(DbSource source)
        {
            Source = source;
            Database.EnsureCreated();
        }
            
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string sourceName = Source == DbSource.PROD ? "users" : "test";
            optionsBuilder.UseSqlite($"Data Source={sourceName}.db");
        }
    }
}