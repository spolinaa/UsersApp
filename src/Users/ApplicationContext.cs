using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace UsersApp
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public ApplicationContext() =>
            Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite("Data Source=users.db");
    }
}