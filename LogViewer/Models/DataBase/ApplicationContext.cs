using Microsoft.EntityFrameworkCore;

namespace LogViewer.Models.DataBase
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Log> LogStrings { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public ApplicationContext() { }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=62.109.29.6;Port=32968;Database=main;Username=admin;Password=w0HMCVFq1");
        }
    }
}
