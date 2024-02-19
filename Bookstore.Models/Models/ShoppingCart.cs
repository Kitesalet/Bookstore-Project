using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Models.Models
{
    public class ShoppingCart
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string ApplicationUserId { get; set; }

        public ApplicationUser? User { get; set; }

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        [Range(1,1000,ErrorMessage = "{0} should be between {1} to {2}!")]
        public int ProductCount { get; set; }

        public double CurrentPrice { get; set; }


    }
}
