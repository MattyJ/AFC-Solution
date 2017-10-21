using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface IService<T> where T : class
    {
        IEnumerable<T> All();
        int Create(T entity);
        void Update(T entity);
        void Delete(int id);
        void Delete(T entity);
        T GetById(int id);
        IQueryable<T> Query(Expression<Func<T, bool>> predicate);

    }
}
