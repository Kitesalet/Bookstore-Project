using AutoMapper;
using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models.Models;
using Bookstore.Models.Models.ViewModels;
using Bookstore.Utility.Utilities;
using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Resources;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area(StaticDetails.AREA_ADMIN)]
    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;
        
        public CompanyController(IUnitOfWork context, IMapper mapper)
        {

            _context = context;
            _mapper = mapper;

        }

        public IActionResult Index()
        {

            CompanyVM model = new CompanyVM();

            model.Companies = _context.Company.GetAll().ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {

            List<Company> companies = _context.Company.GetAll().ToList();

            return Json(new {data = companies});

        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {

            CompanyVM model = new CompanyVM() {
            
            Company  = new()
            };

            if (id == null || id == StaticDetails.INT_0)
            {

                return View(model);
            }

            Company company = _context.Company.GetBy(c => c.Id == id);

            if(company == null)
            {
                TempData[StaticDetails.NOTIF_ERROR] = "Company wasnt found!";

                return RedirectToAction(nameof(Index));

            }

            model.Company = company;

            return View(model);

        }

        [HttpPost]
        public IActionResult Upsert(CompanyVM model)
        {

            if (!ModelState.IsValid)
            {

                return RedirectToAction(nameof(Index));

            }

            if(model.Company.Id == null || model.Company.Id == StaticDetails.INT_0)
            {

                Company company = _mapper.Map<Company>(model.Company);

                _context.Company.Add(company);

                _context.SaveChanges();

                TempData[StaticDetails.NOTIF_SUCCESS] = "The company has been added!";

            }
            else
            {


                _context.Company.Update(model.Company);

                _context.SaveChanges();

                TempData[StaticDetails.NOTIF_SUCCESS] = "The company has been updated!";



            }

            return RedirectToAction(nameof(Index));


        }

        public IActionResult Delete(int? id)
        {

            if(id == null)
            {
                return BadRequest();
            }

            var company = _context.Company.GetBy(c => c.Id == id);

            if(company == null)
            {

                TempData[StaticDetails.NOTIF_ERROR] = "The company couldn't be deleted!";

                return RedirectToAction(nameof(Index));

            }

                _context.Company.Delete(company);

                _context.SaveChanges();

                TempData[StaticDetails.NOTIF_SUCCESS] = "The company was deleted!";

                return RedirectToAction(nameof(Index));

        }

    }
}
