using Microsoft.EntityFrameworkCore;
using StoreMVC.Models;

namespace StoreMVC.Services
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options) 
        {
            
        }

        public DbSet<Product> Products { get; set; }
    }
}
