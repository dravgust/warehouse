﻿using Vayosoft.Queries;
using Vayosoft.Utilities;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.SystemAdministration.Models;
using Warehouse.Core.Domain.Entities.Security;

namespace Warehouse.Core.Application.SystemAdministration.Queries
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
        private readonly IUserRepository _userRepository;

        public HandleGetPermissions(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<RolePermissions> Handle(GetPermissions query, CancellationToken cancellationToken)
        {
            SecurityRoleEntity role = null;
            var permissions = new List<RolePermissionsDTO>();
            if (_userRepository is IUserRoleStore store)
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
