using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NetCoreSample.Web
{
    public class NorthwindSlimContextFactory : IDesignTimeDbContextFactory<NorthwindSlimContext>
    {
        public NorthwindSlimContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NorthwindSlimContext>();
            optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;initial catalog=NetCoreSample;Integrated Security=True; MultipleActiveResultSets=True");
            return new NorthwindSlimContext(optionsBuilder.Options);
        }
    }
}