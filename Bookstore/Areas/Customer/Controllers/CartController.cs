using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models.Models;
using Bookstore.Models.Models.ViewModels;
using Bookstore.Utility.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SQLitePCL;
using Stripe.Checkout;
using Stripe.Tax;
using System.Security.Claims;

namespace Bookstore.Areas.Customer.Controllers
{
    [Area(StaticDetails.AREA_CUSTOMER)]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _context;
        public CartController(IUnitOfWork context)
        {

            _context = context;
            
        }

        public IActionResult Index()
        {

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            CartVM model = new CartVM()
            {
                Carts = _context.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, c => c.Product).ToList(),
                OrderHeader = new ()
            };
            
            foreach(var cart in model.Carts)
            {
                model.OrderHeader.OrderTotal += cart.CurrentPrice;
            }

            return View(model);
        }

        public IActionResult ProductAdd(int? idProduct, CartVM model)
        {

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            ShoppingCart cart = _context.ShoppingCart.
                                        GetBy(c => c.ProductId == idProduct && c.ApplicationUserId == userId,
                                        c => c.Product);
                                        

            cart.ProductCount++;

            cart.CurrentPrice = BooksPrice.Calculator(cart.ProductCount, cart.Product.ListPrice, cart.Product.Price50, cart.Product.Price100);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));

        }

        public IActionResult ProductSubstract(int? idProduct, CartVM model)
        {

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            ShoppingCart cart = _context.ShoppingCart.
                                        GetBy(c => c.ProductId == idProduct && c.ApplicationUserId == userId,
                                        c => c.Product);

            if(cart.ProductCount <= 1)
            {
                TempData[StaticDetails.NOTIF_ERROR] = "You cant remove more products!";

                return RedirectToAction(nameof(Index));
            }


            cart.ProductCount--;

            cart.CurrentPrice = BooksPrice.Calculator(cart.ProductCount, cart.Product.ListPrice, cart.Product.Price50, cart.Product.Price100);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Delete(int? id)
        {

            ShoppingCart cart = _context.ShoppingCart.GetBy(c => c.Id == id);

            if(cart == null)
            {

                TempData[StaticDetails.NOTIF_ERROR] = "The item couldn't be deleted!";

                return RedirectToAction(nameof(Index));
            }

            _context.ShoppingCart.Delete(cart);

            _context.SaveChanges();

            TempData[StaticDetails.NOTIF_SUCCESS] = "The item was deleted successfully!";

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Summary()
        {

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            CartVM model = new CartVM()
            {
                Carts = _context.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, c => c.Product).ToList(),
                OrderHeader = new()
            };

            var user = _context.ApplicationUser.GetBy(u => u.Id == userId);

            if(user == null)
            {
                TempData[StaticDetails.NOTIF_ERROR] = "User was invalide";
                return RedirectToAction(nameof(Index));
            }

            model.OrderHeader.City = user.City;
            model.OrderHeader.StreetAddress = user.StreetAddress;
            model.OrderHeader.State = user.State;
            model.OrderHeader.PhoneNumber = user.PhoneNumber;
            model.OrderHeader.PostalCode = user.PostalCode;
            model.OrderHeader.ShippingDate = DateTime.UtcNow.AddDays(3);

            foreach (var cart in model.Carts)
            {
                model.OrderHeader.OrderTotal += cart.CurrentPrice;
            }


            return View(model);
        }

        [HttpPost]
        public IActionResult Summary(CartVM model)
        {

			string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var user = _context.ApplicationUser.GetBy(u => u.Id == userId);

			if (!ModelState.IsValid)
            {
                 TempData[StaticDetails.NOTIF_ERROR] = "Please, enter the required data!";
				 model.OrderHeader.OrderDate = DateTime.UtcNow;
			     model.Carts = _context.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, c => c.Product).ToList();

			   	 return View(model);
            }


            model.Carts = _context.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, c => c.Product).ToList();

            if (user == null)
            {
                TempData[StaticDetails.NOTIF_ERROR] = "User was invalid";
                return RedirectToAction(nameof(Index));
            }

            if(user.CompanyId.GetValueOrDefault() == 0)
            {

				//Cliente regular, capturar payment
				
				model.OrderHeader.OrderStatus = StaticDetails.STATUS_PENDING;
				model.OrderHeader.PaymentStatus = StaticDetails.PAYMENT_PENDING;			

			}
			else
            {

				model.OrderHeader.OrderStatus = StaticDetails.STATUS_APPROVED;
				model.OrderHeader.PaymentStatus = StaticDetails.PAYMENT_DELAYED;

			}

			model.OrderHeader.ApplicationUserId = userId;
			model.OrderHeader.OrderDate = DateTime.UtcNow;

			_context.OrderHeader.Add(model.OrderHeader);

			_context.SaveChanges();

			foreach (ShoppingCart cart in model.Carts)
			{
				OrderDetail detail = new OrderDetail()
				{
					ProductId = cart.ProductId,
					Price = cart.CurrentPrice,
					Count = cart.ProductCount,
					OrderHeaderId = model.OrderHeader.Id
				};

				_context.OrderDetail.Add(detail);
			}

			_context.SaveChanges();


			if (user.CompanyId.GetValueOrDefault() == 0)
			{

                //Stripe para regular customer
                var domain = "https://localhost:7235/";

                var options = new SessionCreateOptions
                {
                    SuccessUrl = $"{domain}customer/cart/OrderConfirmation?id={model.OrderHeader.Id}",
                    CancelUrl = $"{domain}customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    PaymentMethodTypes = new List<string> { "card" }, // Add other payment method types if needed
                };

                foreach (var item in model.Carts)
                {
                    var sessionLineItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.CurrentPrice), // 20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.ProductCount
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

			TempData[StaticDetails.NOTIF_SUCCESS] = "The order has been made!";

			return RedirectToAction(nameof(OrderConfirmation), new { id = model.OrderHeader.Id});
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _context.OrderHeader.GetBy(o => o.Id == id, o => o.ApplicationUser);

            if (orderHeader.PaymentStatus != StaticDetails.PAYMENT_DELAYED)
            {
                //Order y customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {

                    _context.OrderHeader.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentLinkId);
                    _context.OrderHeader.UpdateStatus(orderHeader.Id,StaticDetails.STATUS_APPROVED, StaticDetails.STATUS_APPROVED);
                    _context.SaveChanges();
                }
            }

            List<ShoppingCart> shoppingCarts = _context.ShoppingCart
            .GetAll(c => c.ApplicationUserId == orderHeader.ApplicationUser.Id)
            .ToList();

            _context.ShoppingCart.DeleteRange(shoppingCarts);

            _context.SaveChanges();

            return View(id);
        }

    }
}
