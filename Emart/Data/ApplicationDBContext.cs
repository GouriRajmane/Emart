using Microsoft.EntityFrameworkCore;

namespace Emart.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }
    }
}

