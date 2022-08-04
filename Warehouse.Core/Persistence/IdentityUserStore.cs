using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.Persistence;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Persistence
{
    public class IdentityUserStore : IIdentityUserStore<UserEntity>
    {
        private readonly WarehouseDbContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public IdentityUserStore(WarehouseDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public dynamic GetUserRoles(object userId)
        {
            var query = (from ur in _context.Set<UserRoleEntity>()
                join r in _context.Set<SecurityRoleEntity>() on ur.RoleId equals r.Id
                where ur.UserId.Equals(userId)
                select new
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Items = (from rp in _context.Set<SecurityRolePermissionsEntity>()
                        join so in _context.Set<SecurityObjectEntity>() on rp.ObjectId equals so.Id
                        select new
                        {
                            Id = rp.Id,
                            RoleId = rp.RoleId,
                            ObjectId = rp.ObjectId,
                            ObjectName = so.Name,
                            Permissions = rp.Permissions
                        }).Where(rp => rp.RoleId == r.Id).ToList()
                });

            return query.ToList();
        }

        public Task<UserEntity> FindByIdAsync(object userId, CancellationToken cancellationToken)
        {
            return _context
                .Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.Id.Equals(userId), cancellationToken: cancellationToken);
        }

        public Task<UserEntity> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            return _context
                .Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken), cancellationToken: cancellationToken);
        }

        public Task<UserEntity> FindByNameAsync(string username, CancellationToken cancellationToken)
        {
            return _context
                    .Set<UserEntity>()
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.Username == username, cancellationToken: cancellationToken);
        }

        public async Task UpdateAsync(IIdentityUser user, CancellationToken cancellationToken)
        {
            _context.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
