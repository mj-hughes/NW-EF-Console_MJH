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

        public void UpdateProduct(Product updatedProduct)
        {
            Product product = this.Products.Find(updatedProduct.ProductID);
            product.ProductName = updatedProduct.ProductName;
            product.SupplierId = updatedProduct.SupplierId;
            product.CategoryId = updatedProduct.CategoryId;
            product.QuantityPerUnit = updatedProduct.QuantityPerUnit;
            product.UnitPrice = updatedProduct.UnitPrice;
            product.UnitsInStock = updatedProduct.UnitsInStock;
            product.UnitsOnOrder = updatedProduct.UnitsOnOrder;
            product.ReorderLevel = updatedProduct.ReorderLevel;
            product.Discontinued = updatedProduct.Discontinued;
            this.SaveChanges();
        }

        // Display one product
        public string DisplayAProduct(Product product)
        {
            return $"ID:\t\t {product.ProductID}\n" +
                $"Name:\t\t {product.ProductName}\n" +
                $"Supplier ID:\t {product.SupplierId}\n" +
                $"Category ID:\t {product.CategoryId}: {product.Category.CategoryName}\n" +
                $"Quantity/Unit:\t {product.QuantityPerUnit}\n" +
                $"Unit Price:\t {product.UnitPrice,10:c2}\n" +
                $"Units in Stock:\t {product.UnitsInStock}\n"+
                $"Units on Order:\t {product.UnitsOnOrder}\n"+
                $"Reorder Level:\t {product.ReorderLevel}\n"+
                $"Discontinued:\t {product.Discontinued}\n";
        }

    }
}
