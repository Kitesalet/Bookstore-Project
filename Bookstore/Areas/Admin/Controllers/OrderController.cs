using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models.Models;
using Bookstore.Models.Models.ViewModels;
using Bookstore.Utility.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area(StaticDetails.AREA_ADMIN)]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _context;
        public OrderController(IUnitOfWork context)
        {

            _context = context;

        }

        public IActionResult Index()
        {

            return View();

        }

        public IActionResult Details(int? orderId)
        {

            var orderHeader = _context.OrderHeader.GetBy(o => o.Id == orderId, o => o.ApplicationUser);
            var orderDetails = _context.OrderDetail.GetAll(o => o.OrderHeaderId == orderId, o => o.Product).ToList();

            OrderVM model = new OrderVM()
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult UpdateOrderDetails(OrderVM model, int? orderId)
        {

            var orderHeader = _context.OrderHeader.GetBy(o => o.Id == orderId, o => o.ApplicationUser);

            orderHeader.Name = model.OrderHeader.Name;
            orderHeader.PhoneNumber = model.OrderHeader.PhoneNumber;
            orderHeader.StreetAddress = model.OrderHeader.StreetAddress;
            orderHeader.City = model.OrderHeader.City;
            orderHeader.State = model.OrderHeader.State;
            orderHeader.PostalCode = model.OrderHeader.PostalCode;

            if (!String.IsNullOrEmpty(model.OrderHeader.Carrier))
            {
                orderHeader.Carrier = model.OrderHeader.Carrier;
            }

            if (!String.IsNullOrEmpty(model.OrderHeader.TrackingNumber))
            {
                orderHeader.TrackingNumber = model.OrderHeader.TrackingNumber;
            }

            _context.SaveChanges();

            TempData[StaticDetails.NOTIF_SUCCESS] = "Order details updated successfully!";

            return RedirectToAction(nameof(Details), model );
        }


        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult StartProcessing(OrderVM model)
        {

            _context.OrderHeader.UpdateStatus(model.OrderHeader.Id, StaticDetails.STATUS_INPROCESS);
            _context.SaveChanges();

            TempData[StaticDetails.NOTIF_SUCCESS] = "Order Details have been updated!";

            return RedirectToAction(nameof(Details), model);
        }


        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult ShipOrder(OrderVM model)
        {


            var orderHeader = _context.OrderHeader.GetBy(o => o.Id == model.OrderHeader.Id);
            orderHeader.TrackingNumber = model.OrderHeader.TrackingNumber;
            orderHeader.Carrier = model.OrderHeader.Carrier;
            orderHeader.OrderStatus = StaticDetails.STATUS_SHIPPED;
            orderHeader.ShippingDate = DateTime.Now;
           
            if(orderHeader.PaymentStatus == StaticDetails.PAYMENT_DELAYED)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _context.OrderHeader.UpdateStatus(model.OrderHeader.Id, StaticDetails.STATUS_SHIPPED);
            _context.SaveChanges();

            TempData[StaticDetails.NOTIF_SUCCESS] = "Order Shipper Successfully!";

            return RedirectToAction(nameof(Details), model);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult CancelOrder(OrderVM model)
        {

            var orderHeader = _context.OrderHeader.GetBy(o => o.Id == model.OrderHeader.Id);

            if(orderHeader.PaymentStatus == StaticDetails.PAYMENT_APPROVED)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId,

                };

                var service = new RefundService();

                Refund refund = service.Create(options);

                _context.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.STATUS_CANCELLED, StaticDetails.STATUS_REFUNDED);
            }
            else
            {
                _context.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.STATUS_CANCELLED, StaticDetails.STATUS_CANCELLED);

            }

            _context.SaveChanges();

            TempData[StaticDetails.NOTIF_SUCCESS] = "Order Cancelled Successfully!";

            return RedirectToAction(nameof(Details), model);

        }

        [ActionName("Details")]
        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult DetailsPay(OrderVM model)
        {

            var orderHeader = _context.OrderHeader.GetBy(o => o.Id == orderId, o => o.ApplicationUser);
            var orderDetails = _context.OrderDetail.GetAll(o => o.OrderHeaderId == orderId, o => o.Product).ToList();



            //Stripe para regular customer
            var domain = "https://localhost:7235/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{domain}admin/order/PaymentConfirmation?id={model.OrderHeader.Id}",
                CancelUrl = $"{domain}admin/order/details?orderId={model.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                PaymentMethodTypes = new List<string> { "card" }, // Add other payment method types if needed
            };

            foreach (var item in model.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price), // 20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };

                options.LineItems.Add(sessionLineItem);
            }

            //Se crea una nueva session con las opciones 
            var service = new SessionService();
            Session session = service.Create(options);

            _context.OrderHeader.UpdateStripePaymentId(model.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _context.SaveChanges();

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);

        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _context.OrderHeader.GetBy(o => o.Id == orderHeaderId, o => o.ApplicationUser);

            if (orderHeader.PaymentStatus == StaticDetails.PAYMENT_DELAYED)
            {
                //Order y customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {

                    _context.OrderHeader.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentLinkId);
                    _context.OrderHeader.UpdateStatus(orderHeader.Id, orderHeader.OrderStatus, StaticDetails.STATUS_APPROVED);
                    _context.SaveChanges();
                }
            }

          

            return View(orderHeaderId);
        }

        #region APICALLS

        [HttpGet()]
        public IActionResult GetAll(string status)
        {
            //Cuando se hace un retrieve de orderHeader,tambien se quiere hacer uno de OrderDetails
            IEnumerable<OrderHeader> orders;

            if(User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {

                orders = _context.OrderHeader.GetAll(o => o.OrderStatus == status, o => o.ApplicationUser);

            }
            else
            {
                orders = _context.OrderHeader.GetAll(o => o.OrderStatus == status, o => o.ApplicationUser)
                                             .Where(u => u.ApplicationUser.Id == User.Claims
                                             .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            }

            return Ok(new { data = orders });

        }

        

        #endregion

    }
}
