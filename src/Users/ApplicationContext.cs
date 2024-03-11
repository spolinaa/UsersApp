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
            string sourcePath = Source == DbSource.PROD 
                ? "../data/users.db" 
                : "test.db";
            optionsBuilder.UseSqlite($"Data Source={sourcePath}");
        }
    }
}