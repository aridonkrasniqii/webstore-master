using WebStore.Models;
using Microsoft.EntityFrameworkCore;

namespace WebStore.DAL
{
    public class WebStoreContext : DbContext {     
        public WebStoreContext(DbContextOptions options) : base(options) {
        
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
