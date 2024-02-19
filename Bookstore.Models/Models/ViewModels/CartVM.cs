using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Models.Models.ViewModels
{
    public class CartVM
    {
        [ValidateNever]
        public List<ShoppingCart> Carts { get; set; }

        public OrderHeader OrderHeader { get; set; }

        [ValidateNever]
        public OrderDetail OrderDetail { get; set; }

    }
}
