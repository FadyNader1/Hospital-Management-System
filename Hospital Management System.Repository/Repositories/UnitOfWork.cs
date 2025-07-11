using Hospital_Management_System.Core.Entities;
using Hospital_Management_System.Core.Interfaces;
using Hospital_Management_System.Repository.Contexts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Hashtable Repositories;
        private readonly ApplicationDbContext context;

        public UnitOfWork(ApplicationDbContext context)
        {
            Repositories=new Hashtable();
            this.context = context;
        }
        public async Task<int> Complete()
        {
            return await context.SaveChangesAsync();
        }

        public  IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T).Name;
            if (!Repositories.ContainsKey(type))
            {
                var repository=new GenericRepository<T>(context);
                Repositories.Add(type, repository);
            }
            return Repositories[type] as IGenericRepository<T>;
        }
    }
}
