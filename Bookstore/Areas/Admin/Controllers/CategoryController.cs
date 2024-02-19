using AutoMapper;
using Bookstore.DataAccess.DAL;
using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models.Models;
using Bookstore.Models.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _repo;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public IActionResult Index()
        {

            var model = _repo.Category.GetAll().ToList();


            return View(model);
        }

        [HttpPost]
        public IActionResult Create(EditCreateVM model)
        {

            if (model.Name == model.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Name property cant be the same as the Display Order");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Category category = _mapper.Map<Category>(model);

            _repo.Category.Add(category);

            _repo.SaveChanges();

            TempData["Success"] = "Category Created Successfully!";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {

            Category? category = _repo.Category.GetBy(c => c.Id == id);

            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            EditCreateVM model = _mapper.Map<EditCreateVM>(category);

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditCreateVM model)
        {

            //if(_repo.GetBy(c => c.Name == model.Name && c.Id != model.Id))
            //{
            //    ModelState.AddModelError("name", "There is already a category with that name!");
            //}

            //if(_context.Categories.Any(c => c.DisplayOrder == model.DisplayOrder && c.Id != model.Id))
            //{
            //    ModelState.AddModelError("displayOrder", "There is already a category with that display order!");
            //}

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Category category = _mapper.Map<Category>(model);

            _repo.Category.Update(category);

            _repo.SaveChanges();

            TempData["Success"] = "Category edited successfully!";


            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {

            Category? category = _repo.Category.GetBy(c => c.Id == id);

            if (category == null)
            {
                NotFound();
            }

            EditCreateVM model = _mapper.Map<EditCreateVM>(category);

            return View(model);
        }

        [HttpPost, ActionName(nameof(Delete))]
        public IActionResult DeletePOST(int? id)
        {

            Category? category = _repo.Category.GetBy(c => c.Id == id);

            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _repo.Category.Delete(category);

            _repo.SaveChanges();

            TempData["Success"] = "Category Deleted Successfully!";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View(new EditCreateVM());//d
        }
    }
}
