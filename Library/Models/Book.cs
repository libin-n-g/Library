using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Book
    {
        public int BookID { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Author { get; set; }
   
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Genre { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public virtual ApplicationUser TakenBy { get; set; }
    }
}
