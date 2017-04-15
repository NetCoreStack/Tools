using NetCoreStack.Hisar.WebCli.Tools.Models;
using Microsoft.EntityFrameworkCore;

namespace NetCoreStack.Hisar.WebCli.Tools.Context
{
    public class HisarCliContext : DbContext
    {
        public DbSet<Page> Pages { get; set; }

        public HisarCliContext(DbContextOptions options) 
            : base(options)
        {
             
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Page>().ToTable("Pages");
        }
    }
}
