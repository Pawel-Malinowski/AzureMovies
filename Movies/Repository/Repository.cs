using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Movies.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _dataContext;
        protected DbSet<T> DbSet;

        public Repository(DataContext dataContext)
        {
            _dataContext = dataContext;
            DbSet = dataContext.Set<T>();
        }

        public Task<EntityEntry<T>> AddAsync(T entity)
        {
            return DbSet.AddAsync(entity);
        }

        public EntityEntry<T> Update(T entity)
        {
            return DbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public Task<T> GetAsync(int id)
        {
            return DbSet.FindAsync(id);
        }

        public Task SaveAsync()
        {
            return _dataContext.SaveChangesAsync();
        }
    }
}
