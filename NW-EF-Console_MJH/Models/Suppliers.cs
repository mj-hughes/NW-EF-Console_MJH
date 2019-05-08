using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NW_EF_Console_MJH.Models
{
    /* Supplier is included to validate supplier id for new/edit product. Data annotations are not part of project grading. */
    public class Supplier
    {
        public int SupplierId { get; set; }
        [Required(ErrorMessage = "Enter the supplier's company name.")]
        [StringLength(40, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string CompanyName { get; set; }
        [RegularExpression(@"[^?][a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Only a-z and NO question marks.")]
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        [RegularExpression(@"[0-9]{7,11}", ErrorMessage = "Phone number must be 7 to 11 digits")]
        public string Phone { get; set; }
        [RegularExpression(@"[0-9]{7,11}", ErrorMessage = "Fax number must be 7 to 11 digits")]
        public string Fax { get; set; }

        public virtual List<Product> Products { get; set; }
    }
}
