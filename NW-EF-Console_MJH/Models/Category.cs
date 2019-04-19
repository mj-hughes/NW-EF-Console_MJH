using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NW_EF_Console_MJH.Models
{
    // Reading up on validation needed special handling so that updating the model won't update the validations? https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/models-data/validation-with-the-data-annotation-validators-cs#using-data-annotation-validators-with-the-entity-framework
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Please enter the category name")]
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public virtual List<Product> Products { get; set; }
    }
}
