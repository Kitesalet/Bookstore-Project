using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Models.Models.ViewModels
{
    public class CompanyVM
    {

        public Company Company { get; set; }
        public List<Company> Companies { get; set; } = new List<Company>();

    }
}
