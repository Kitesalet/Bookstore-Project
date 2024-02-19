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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {

        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context) : base(context)
        {

            _context = context;

        }

        public void Update(OrderDetail entity)
        {
            _context.Update(entity);
        }

    }
}
