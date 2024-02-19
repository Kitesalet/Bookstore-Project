using AutoMapper;
using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.Models.ViewModels;
using Bookstore.Utility.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace Bookstore.Areas.Customer.Controllers
{

    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            HomeVM model = new HomeVM();

            model.Products = _context.Product.GetAll(e => e.Category).ToList();


            return View(model);
        }

        public IActionResult Details(int? id)
        {

            Product product = _context.Product.GetBy(p => p.Id == id, p => p.Category);

            if(product == null)
            {

                TempData["Error"] = "Product not found!";

                return RedirectToAction(nameof(Index));
            }

            HomeVM model = _mapper.Map<HomeVM>(product);

            model.Product = product;

            return View(model);
        }
        public IActionResult Privacy()
        {


            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddProductCart(HomeVM model)
        {

            if (!ModelState.IsValid)
            {

                TempData[StaticDetails.NOTIF_ERROR] = "There has been an error!";

                return RedirectToAction(nameof(Details));
            }


            string applicationUserId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            ShoppingCart cart = _context.ShoppingCart.GetBy(c => c.ProductId == model.Product.Id &&
                                                                    c.ApplicationUserId == applicationUserId);

            if (cart == null)
            {
                cart = new()
                {
                    ProductId = model.Product.Id,
                    ApplicationUserId = applicationUserId,
                    ProductCount = model.ProductCount
                };

                cart.CurrentPrice = BooksPrice.Calculator(model.ProductCount, model.Product.ListPrice, model.Product.Price50, model.Product.Price100);

                TempData[StaticDetails.NOTIF_SUCCESS] = "A book has been added to the shopping cart!";

                _context.ShoppingCart.Add(cart);

            }
            else
            {

                cart.ProductCount += model.ProductCount;

                cart.CurrentPrice = BooksPrice.Calculator(cart.ProductCount, model.Product.ListPrice, model.Product.Price50, model.Product.Price100);

                if (cart.ProductCount > 1000)
                {
                    TempData[StaticDetails.NOTIF_ERROR] = "You have reached the maximum quantity of books!";

                    return RedirectToAction(nameof(Index));
                }

                TempData[StaticDetails.NOTIF_SUCCESS] = "The quantity of this book has been updated in your shopping cart!";


            }


            _context.SaveChanges();

            return RedirectToAction(nameof(Index));

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
