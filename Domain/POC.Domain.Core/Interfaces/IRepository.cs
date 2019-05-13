using System;
using System.Collections.Generic;
using System.Linq;

namespace POC.Domain.Core.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        void Add(TEntity obj);
        TEntity GetById(Guid id);
        IQueryable<TEntity> GetAll();

        int SaveChanges();

        IEnumerable<TEntity> PagedResult(int page, int pageSize);
        int PageCount(int pageSize);
    }
}