using Hospital_Management_System.Core.Entities;
using Hospital_Management_System.Core.Interfaces;
using Hospital_Management_System.Core.Specifications;
using Hospital_Management_System.Repository.Contexts;
using Hospital_Management_System.Repository.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext context;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        private IQueryable<T> ApplyQuery(ISpecification<T> spec)
        {
            return BuildQuery<T>.GetQuery(context.Set<T>(), spec);
        }
        public async Task AddAsync(T entity)
        {
             await context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();       }

        public async Task<T> GetByIdAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplyQuery(spec).ToListAsync();
        }

        public async Task<T> GetByIdSpecAsync(ISpecification<T> spec)
        {
            return await ApplyQuery(spec).FirstOrDefaultAsync();  
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplyQuery(spec).CountAsync();

        }
    }
}
