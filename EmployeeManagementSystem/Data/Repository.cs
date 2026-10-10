
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmployeeManagementSystem.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext dbContext;
        protected DbSet<T> dbSet;
        public Repository(AppDbContext dbContext)
        {
            dbSet = dbContext.Set<T>();
            this.dbContext = dbContext;
        }
        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await FindByIdAsync(id);
            dbSet.Remove(entity);
        }

        public async Task<T> FindByIdAsync(int id)
        {
            var entity = await dbSet.FindAsync(id);
            return entity;
        }

        public async Task<List<T>> GetAll()
        {
            var list =await dbSet.ToListAsync();
            return list;
        }
        public async Task<List<T>> GetAll(Expression<Func<T, bool>> filter)
        {
            var list = await dbSet.AsQueryable().Where(filter).ToListAsync();
            return list;
        }


        public async Task<int> SaveChangesAsync()
        {
            return (await dbContext.SaveChangesAsync());
        }

        public void  Update(T entity)
        {
            dbSet.Update(entity);
        }
    }
}
