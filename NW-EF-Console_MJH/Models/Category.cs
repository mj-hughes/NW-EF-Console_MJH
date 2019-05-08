using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NW_EF_Console_MJH.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Please enter the category name")]
        [StringLength(15, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public virtual List<Product> Products { get; set; }
    }
}
