using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Models
{
    public class History : DbContext
    {
        public History(DbContextOptions<History> options) : base(options)
        {
        }

        public DbSet<AssemblyReport> Assemblies { get; set; }

        public DbSet<TestReport> Tests { get; set; }
    }
}
