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
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly AppDbContext _context;
        public ApplicationUserRepository(AppDbContext context) : base(context)
        {

            _context = context;

        }

        public void SaveChanges()
        {

            _context.SaveChanges();

        }

    }
}
