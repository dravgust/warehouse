using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Exceptions;
using Vayosoft.Data.EF.MySQL;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Persistence
{
    public class IdentityUserStore : IIdentityUserStore
    {
        private readonly DataContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public IdentityUserStore(DataContext context)
        {   
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IIdentityUser GetById(object id)
        {
            var user = _context
                .Set<UserEntity>()
                .Include(u => u.RefreshTokens)
                .SingleOrDefault(u => u.Id.Equals(id));

            if (user == null)
                throw EntityNotFoundException.For<UserEntity>(id);

            return user;
        }

        public IIdentityUser GetUserByRefreshToken(string token)
        {
            var user = _context
                .Set<UserEntity>()
                .Include(u => u.RefreshTokens)
                .SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                throw new ApplicationException("Invalid token");
            return user;
        }

        public IIdentityUser? FindUserByNameAsync(string username)
        {
            return _context
                    .Set<UserEntity>()
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefault(u => u.Username == username);
        }
    }
}
