using Microsoft.EntityFrameworkCore;
using TestWebApplication.Models;

namespace TestWebApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public StreamWriter logStream = new StreamWriter("log.txt", true);
        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            MySqlServerVersion mySqlServerVersion = new MySqlServerVersion(new Version(8, 2, 6));
            optionsBuilder.UseMySql("Server=localhost;Database = test;Uid=root;Pwd=password;", mySqlServerVersion);
            optionsBuilder.LogTo(logStream.WriteLine, LogLevel.Information);
        }
        public override void Dispose()
        {
            base.Dispose();
            logStream.Dispose();
        }
        public override async ValueTask DisposeAsync()
         {
            await base.DisposeAsync();
            await logStream.DisposeAsync();
        }
        public DbSet<Category> Category { get; set; }
        public DbSet<ApplicationType> Type { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}
