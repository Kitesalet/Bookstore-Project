using AutoMapper;
using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models.Models;
using Bookstore.Models.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        public ProductController(IUnitOfWork context, IMapper mapper, IWebHostEnvironment environment)
        {
            
            _context = context;
            _mapper = mapper;
            _environment = environment;

        }

        public IActionResult Index()
        {

            ProductVM model = new ProductVM();

            model.Products = _context.Product.GetAll(c => c.Category).ToList();
            
            return View(model);
        }


        public IActionResult Upsert(int? id)
        {

            IEnumerable<SelectListItem> categoriesItem = _context
                                                        .Category
                                                        .GetAll()
                                                        .Select(c => new SelectListItem
                                                        {
                                                         Text = c.Name,
                                                         Value = c.Id.ToString()
                                                        }
                                                                );

            ProductVM model = new ProductVM();

            model.Categories = categoriesItem;

            //Create
            if (id == null)
            {
                //nada
            }
            else
            {
                //Update
                Product product = _context.Product.GetBy(p => p.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                model = _mapper.Map<ProductVM>(product);
                model.Categories = categoriesItem;
            }


            return View(model);

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM model, IFormFile? file)
        {

            if (!ModelState.IsValid)
            {
                IEnumerable<SelectListItem> categoriesItem = _context.Category.GetAll()
                                                                     .Select(c => new SelectListItem
                                                                     {
                                                                         Text = c.Name,
                                                                         Value = c.Id.ToString()
                                                                     }
                                                                     );


                return View(model);
            }

            //Web Root Path
            string webRootPath = _environment.WebRootPath;
            if(file != null)
            {
                //Nombre aleatorio irrepetible para que no se sobreescriban los files
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                //Ruta a la carpeta de images y producto
                string productPath = Path.Combine(webRootPath, @"images\product");
                //El path completo
                string fileNamePath = Path.Combine(productPath, fileName);

                if (!String.IsNullOrEmpty(model.ImageURL))
                {
                    //Delete old image

                    var oldImagePath = Path.Combine(webRootPath, model.ImageURL.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                }

                using (var fileStream = new FileStream(fileNamePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                string relativePath = fileNamePath.Substring(fileNamePath.IndexOf(@"\images"));

                model.ImageURL = relativePath;
            }

            Product product = _mapper.Map<Product>(model);

            if (model.Id == null)
            {

                _context.Product.Add(product);

                _context.SaveChanges();

                TempData["Success"] = "A product has been added successfully!";

            }
            else
            {
                _context.Product.Update(product);

                _context.SaveChanges();

                TempData["Success"] = "A product has been updated successfully!";

            }

            return RedirectToAction(nameof(Index));

        }


        


        #region APICALLS

        [HttpGet]
        public IActionResult GetAll()
        {

            var products = _context.Product.GetAll(p => p.Category);

            return Ok(new { data = products });


        }

        [HttpDelete]
        public IActionResult Delete(ProductVM model)
        {

            Product product = _context.Product.GetBy(p => p.Id == model.Id);

            if (product == null)
            {
                TempData["Success"] = "Error deleting the product!";


                return Json(new { success = false, message = "Error while deleting the product" });
            }

            if (!String.IsNullOrEmpty(product.ImageURL))
            {

                string rootPath = _environment.WebRootPath;
                string wholePath = Path.Combine(rootPath.TrimEnd('\\'), product.ImageURL);

                if (System.IO.File.Exists(wholePath))
                {
                    System.IO.File.Delete(wholePath);
                }

            }

            _context.Product.Delete(product);
            _context.SaveChanges();

            TempData["Success"] = "A product has been deleted successfully!";

            return Json(new { success = true, message = "Product has been deleted!" });

        }

        #endregion
    }
}
