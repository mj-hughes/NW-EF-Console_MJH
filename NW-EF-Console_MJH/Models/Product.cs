using System;
using System.ComponentModel.DataAnnotations;

namespace NW_EF_Console_MJH.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Please enter the product name")]
        [StringLength(40, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        [Range(0, 32767, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Int16? UnitsInStock { get; set; }
        [Range(0, 32767, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Int16? UnitsOnOrder { get; set; }
        [Range(0, 32767, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Int16? ReorderLevel { get; set; }
        // TODO: True/False validator isn't working with -1 (or synch up with error message in the program)
        [Required(ErrorMessage ="Please choose true or false for discontinued")]
        [Range(typeof(bool), "false", "true", ErrorMessage = "Please choose true or false for discontinued")]
        public bool Discontinued { get; set; }

        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }

        public virtual Category Category { get; set; }
    }
}
