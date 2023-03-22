using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace TandemBooking.Services
{
    public static class DatabaseExtensions
    {
        public static IDbContextTransaction GetExistingTransaction(this DatabaseFacade database)
        {
            var relConn = database.GetService<IRelationalConnection>();
            return relConn.CurrentTransaction;
        }

        public static IDbContextTransaction TryBeginTransaction(this DatabaseFacade database)
        {
            return database.GetExistingTransaction() == null
                ? database.BeginTransaction()
                : null;
        }
    }
}