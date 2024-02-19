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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {

        private readonly AppDbContext _context;
        
        public OrderHeaderRepository(AppDbContext context) : base(context)
        {
            _context = context; 
        }
        public void Update(OrderHeader entity)
        {
            _context.Update(entity);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {

            var orderFromDb = _context.OrderHeaders.FirstOrDefault(o => o.Id == id);

            if(orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;

                if(!String.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {

            var orderFromDb = _context.OrderHeaders.FirstOrDefault(o => o.Id == id);

            if (!String.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }
            if (!String.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
}
