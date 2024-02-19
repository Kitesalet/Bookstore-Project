using Bookstore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {

        public void Update(Product entity);

    }
}
