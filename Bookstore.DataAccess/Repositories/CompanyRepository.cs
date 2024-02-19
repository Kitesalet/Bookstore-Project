using Bookstore.DataAccess.DAL;
using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models.Models;

namespace Bookstore.DataAccess.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {

        private readonly AppDbContext _context;
        public CompanyRepository(AppDbContext context) : base(context)
        {

            _context = context;

        }
        public void Update(Company entity)
        {
            _context.Companies.Update(entity);
        }
    }
}
