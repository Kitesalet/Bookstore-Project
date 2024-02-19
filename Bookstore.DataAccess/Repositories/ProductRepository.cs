using Bookstore.DataAccess.DAL;
using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {

            _context = context;

        }
        public void Update(Product entity)
        {
            _context.Update(entity);
        }
    }
}
