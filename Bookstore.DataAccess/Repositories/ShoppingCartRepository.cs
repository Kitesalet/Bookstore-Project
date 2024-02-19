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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {

        private readonly AppDbContext _context;

        public ShoppingCartRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(ShoppingCart entity)
        {

            _context.Update(entity);

        }
    }
}
