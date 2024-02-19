using Bookstore.DataAccess.DAL;
using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {

            _context = context;

        }

        public void SaveChanges()
        {

            _context.SaveChanges();

        }

        public void Update(Category category)
        {

            _context.Update(category);

        }
    }
}
