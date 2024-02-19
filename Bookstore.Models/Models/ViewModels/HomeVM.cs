using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Models.Models.ViewModels
{
    public class HomeVM
    {
        [ValidateNever]
        public Product? Product { get; set; }
        public List<Product> Products { get; set;} = new List<Product>();

        [Required]
        [Range(1, 1000)]
        public int ProductCount { get; set; }   

        public int TotalProducts { get; set; }

    }
}
