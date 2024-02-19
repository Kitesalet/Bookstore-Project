using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, params Expression<Func<T, object>>[]? includes);
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[]? includes);

        T GetBy(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[]? includes);

        void Add(T entity);

        void Delete(T entity);  

        void DeleteRange(IEnumerable<T> entities);

    }
}
