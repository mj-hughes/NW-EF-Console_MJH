using System.Data.Entity;

namespace NW_EF_Console_MJH.Models
{
    public class NWContext : DbContext
    {
        public NWContext() : base("name=NWContext") { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public void AddProduct(Product product)
        {
            this.Products.Add(product);
            this.SaveChanges();
        }

        public void AddCategory(Category category)
        {
            this.Categories.Add(category);
            this.SaveChanges();
        }
    }
}
