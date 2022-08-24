using Vayosoft.Core.Queries;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.UseCases.Administration.Queries
{
    public class GetPermissions : IQuery<RolePermissions>
    {
        public GetPermissions(string roleId)
        {
            RoleId = Guard.NotEmpty(roleId, nameof(roleId));
        }

        public string RoleId { get; set; }
    }

    public class HandleGetPermissions : IQueryHandler<GetPermissions, RolePermissions>
    {
        private readonly IUserStore<UserEntity> _userStore;

        public HandleGetPermissions(IUserStore<UserEntity> userStore)
        {
            _userStore = userStore;
        }

        public async Task<RolePermissions> Handle(GetPermissions query, CancellationToken cancellationToken)
        {
            SecurityRoleEntity role = null;
            var permissions = new List<RolePermissionsDTO>();
            if (_userStore is IUserRoleStore store)
            {
                role = await store.FindRoleByIdAsync(query.RoleId, cancellationToken);
                if (role == null)
                    return null;

                permissions = await store.GetRolePermissionsAsync(query.RoleId, cancellationToken);
                var objects = await store.GetObjectsAsync(cancellationToken);
                foreach (var obj in objects)
                {
                    if (permissions.All(p => p.ObjectId != obj.Id))
                        permissions.Add(new RolePermissionsDTO
                        {
                            Id = null,
                            RoleId = query.RoleId,
                            ObjectId = obj.Id,
                            ObjectName = obj.Name,
                            Permissions = SecurityPermissions.None
                        });
                }
            }

            return new RolePermissions(role, permissions);
        }
    }
}
