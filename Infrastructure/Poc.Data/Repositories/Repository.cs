using Microsoft.EntityFrameworkCore;
using Poc.Data.Context;
using POC.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poc.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly PocContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(PocContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual void Add(TEntity obj)
        {
            DbSet.Add(obj);
        }

        public virtual TEntity GetById(Guid id)
        {
            return DbSet.Find(id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public int SaveChanges()
        {
            return Db.SaveChanges();
        }


        public IEnumerable<TEntity> PagedResult(int page, int pageSize)
        {
            var result = GetAll();

            if (page <= 1 || pageSize <= 1)
            {
                return result;
            }

            var skip = (page - 1) * pageSize;

            return result.Skip(skip).Take(pageSize);
        }

        public int PageCount(int pageSize)
        {
            var rowCount = GetAll().Count();

            var pageCount = (int)Math.Ceiling((decimal)rowCount / pageSize);

            return pageCount;
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
