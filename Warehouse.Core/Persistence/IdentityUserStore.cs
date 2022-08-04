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

        public dynamic GetRolePermissions(string roleid)
        {
            var query = (from rp in _context.Set<SecurityRolePermissionsEntity>()
                join so in _context.Set<SecurityObjectEntity>() on rp.ObjID equals so.Id
                select new
                {
                    ID = rp.Id,
                    RoleID = rp.RoleID,
                    ObjID = rp.ObjID,
                    ObjName = so.ObjName,
                    Permissions = rp.Permissions
                });

            return query.Where(rp => rp.RoleID == roleid).ToList();
        }

        public dynamic GetUserRoles(object userid)
        {
            var query = (from ur in _context.Set<UserRoleEntity>()
                join r in _context.Set<SecurityRoleEntity>()
                    on ur.RoleID equals r.Id
                select new
                {
                    ID = r.Id,
                    RoleName = r.RoleName,
                    RoleDesc = r.RoleDesc,
                    Items = GetRolePermissions(r.Id)
                });

            return query.ToList();
        }

        public Task<UserEntity> FindByIdAsync(object userId, CancellationToken cancellationToken)
        {
            //var roles = GetUserRoles(userId);
            
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
