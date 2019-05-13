using System;

namespace POC.Domain.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        bool Commit();
    }
}