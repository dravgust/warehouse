using Microsoft.EntityFrameworkCore;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Persistence;

namespace Warehouse.Infrastructure.Persistence
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<SecurityRoleEntity>> GetRolesAsync(IEnumerable<object> providers, CancellationToken cancellationToken)
        {
            var rl = await GetEmbeddedRolesAsync(cancellationToken);
            var roles = await _context.Set<SecurityRoleEntity>()
                .Where(r => r.ProviderId != null && providers.Contains(r.ProviderId.Value))
                .ToListAsync(cancellationToken: cancellationToken);
            rl.AddRange(roles);
            return rl;
        }

        public Task<List<SecurityRoleEntity>> GetEmbeddedRolesAsync(CancellationToken cancellationToken)
        {
            return _context.Set<SecurityRoleEntity>().Where(r => r.ProviderId == null)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public Task<SecurityRoleEntity> FindRoleByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return _context.Set<SecurityRoleEntity>()
                .AsTracking()
                .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken: cancellationToken);
        }

        public Task<SecurityRoleEntity> FindRoleByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            return _context.Set<SecurityRoleEntity>()
                .AsTracking()
                .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken: cancellationToken);
        }

        public Task<List<SecurityObjectEntity>> GetObjectsAsync(CancellationToken cancellationToken)
        {
            return _context.Set<SecurityObjectEntity>()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public Task<SecurityRolePermissionsEntity> FindRolePermissionsByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return _context.Set<SecurityRolePermissionsEntity>()
                .AsTracking()
                .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken: cancellationToken);
        }

        public Task<List<RolePermissionsDTO>> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken)
        {
            var query = from rp in _context.Set<SecurityRolePermissionsEntity>()
                join so in _context.Set<SecurityObjectEntity>() on rp.ObjectId equals so.Id
                where rp.RoleId == roleId
                select new RolePermissionsDTO
                {
                    Id = rp.Id,
                    RoleId = rp.RoleId,
                    ObjectId = rp.ObjectId,
                    ObjectName = so.Name,
                    Permissions = rp.Permissions
                };

            return query.ToListAsync(cancellationToken: cancellationToken);
        }

        public Task<List<RoleDTO>> GetUserRolesAsync(object userId, CancellationToken cancellationToken = default)
        {
            var query = (from ur in _context.Set<UserRoleEntity>()
                join r in _context.Set<SecurityRoleEntity>() on ur.RoleId equals r.Id
                where ur.UserId.Equals(userId)
                select new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Items = (from rp in _context.Set<SecurityRolePermissionsEntity>()
                        join so in _context.Set<SecurityObjectEntity>() on rp.ObjectId equals so.Id
                        where rp.RoleId == r.Id
                        select new RolePermissionsDTO
                        {
                            Id = rp.Id,
                            RoleId = rp.RoleId,
                            ObjectId = rp.ObjectId,
                            ObjectName = so.Name,
                            Permissions = rp.Permissions
                        }).ToList()
                });

            return query.ToListAsync(cancellationToken: cancellationToken);
        }

        public Task<UserEntity> FindByIdAsync(object userId, CancellationToken cancellationToken)
        {
            return _context
                .Users
                .Include(u => u.RefreshTokens)
                .AsTracking()
                .SingleOrDefaultAsync(u => u.Id.Equals(userId), cancellationToken: cancellationToken);
        }

        public Task<UserEntity> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            return _context
                .Users
                .Include(u => u.RefreshTokens)
                .AsTracking()
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken), cancellationToken: cancellationToken);
        }

        public Task<UserEntity> FindByNameAsync(string username, CancellationToken cancellationToken)
        {
            return _context
                    .Set<UserEntity>()
                    .Include(u => u.RefreshTokens)
                    .AsTracking()
                    .SingleOrDefaultAsync(u => u.Username == username, cancellationToken: cancellationToken);
        }

        public async Task UpdateAsync(UserEntity user, CancellationToken cancellationToken)
        {
            if (user.IsTransient())
                await _context.Users.AddAsync(user, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateSecurityRoleAsync(SecurityRoleEntity entity, CancellationToken cancellationToken)
        {
            //_context.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateUserRolesAsync(object userId, IEnumerable<string> roles, CancellationToken cancellationToken)
        {
            var users = _context.Set<UserRoleEntity>();
            var uRoles = users.Where(r => r.UserId.Equals(userId));
            users.RemoveRange(uRoles);
            users.AddRange(roles.Select(r => new UserRoleEntity { UserId = (long)userId, RoleId = r }));
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRolePermissionsAsync(SecurityRolePermissionsEntity entity, CancellationToken cancellationToken)
        {
            //_context.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
