using Bookstore.Models.Models;
using Bookstore.Models.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Utility.Utilities
{
    public static class BooksPrice
    {

        public static double Calculator(int productCount, double price, double price50, double price100)
        {

            double currentPrice;

            if (productCount < 50)
            {
                currentPrice =  productCount * price;
            }
            else if (productCount < 100)
            {
                currentPrice = productCount * price50;
            }
            else
            {
                currentPrice = productCount * price100;
            }
            return currentPrice;
        }

    }
}
