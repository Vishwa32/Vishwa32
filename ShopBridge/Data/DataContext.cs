using System.Data.Entity;
using ShopBridge.Models;

namespace ShopBridge.Data
{
    public class DataContext:DbContext
    {
        public DbSet<Product> Product { get; set; }
    }
}