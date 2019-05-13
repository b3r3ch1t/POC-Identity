using POC.Domain.Core;
using POC.Domain.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Poc.WebApi.Interfaces
{
    public interface ICrudBase<T>
    {
        Task<Result<object, IErrorMessage>> Register(T model);
        Task<Result<object, IErrorMessage>> GetAll();
        Task<Result<object, IErrorMessage>> ListPaged(int page = 0, int pageSize = 0);
        Task<Result<object, IErrorMessage>> GetById(Guid id);
        Task<Result<object, IErrorMessage>> Update(T model);
        Task<Result<object, IErrorMessage>> Remove(Guid id);
    }
}