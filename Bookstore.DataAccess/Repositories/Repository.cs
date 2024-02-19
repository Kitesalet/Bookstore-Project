using Bookstore.DataAccess.DAL;
using Bookstore.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _set;

        public Repository(AppDbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }
        public void Add(T entity)
        {
            
            _set.Add(entity);   

        }

        public void Delete(T entity)
        {

            _set.Remove(entity);

        }

        public void DeleteRange(IEnumerable<T> entities)
        {

            _set.RemoveRange(entities);

        }

        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {

            IQueryable<T> query = _set;

            if (includes != null)
            {

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

            }

            return query.ToList();

        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includes)
        {

            IQueryable<T> query = _set;

            if(filter != null)
            {
                query = query.Where(filter);
            }

            if(includes != null)
            {

                foreach(var include in includes)
                {
                    query = query.Include(include);
                }

            }

            return query.ToList();

        }

        public T? GetBy(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[]? includes)
        {

            IQueryable<T> query = _set;

            query = query.Where(filter);

            if(includes != null)
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query.FirstOrDefault();

        }
    }
}
