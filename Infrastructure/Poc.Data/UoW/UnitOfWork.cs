using Poc.Data.Context;
using POC.Domain.Core.Interfaces;

namespace Poc.Data.UoW
{


    namespace Equinox.Infra.Data.UoW
    {
        public class UnitOfWork : IUnitOfWork
        {
            private readonly PocContext _context;

            public UnitOfWork(PocContext context)
            {
                _context = context;
            }

            public bool Commit()
            {
                return _context.SaveChanges() > 0;
            }

            public void Dispose()
            {
                _context.Dispose();
            }
        }
    }

}
