using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class BooksIssueViewModel
    {
        public Book book;
        public SelectList Users;
        public string Username { get; set; }
    }
    public class ReturnBookViewModel
    {
        public int bookID { get; set; }
        public string Username { get; set; }
        public Book book;
    }
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

        [UIHint("../Shared/YesNo.cshtml")]
        public bool Avaliable { get; set; }
        
        public virtual ApplicationUser TakenBy { get; set; }
    }
}
