using Microsoft.EntityFrameworkCore;

namespace ShippitApp
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions options) : base(options)
        {
        }

        public DbSet<LineHaul> LineHauls => Set<LineHaul>();
    }
}
