using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Movies.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<EntityEntry<T>> AddAsync(T entity);
        EntityEntry<T> Update(T entity);
        void Delete(T entity);
        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        Task<T> GetAsync(int id);
        Task SaveAsync();
    }
}
