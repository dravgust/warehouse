﻿using Warehouse.Core.Domain.Entities.Identity;

namespace Warehouse.Core.Application.Common.Persistence
{
    public interface IUserStore<T> where T : IUser
    {
        Task<T> FindByIdAsync(object userId, CancellationToken cancellationToken = default);
        Task<T> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<T> FindByNameAsync(string username, CancellationToken cancellationToken = default);

        Task UpdateAsync(T user, CancellationToken cancellationToken);
    }
}
