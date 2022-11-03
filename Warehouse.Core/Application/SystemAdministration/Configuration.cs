using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Commands;
using Vayosoft.Persistence.Commands;
using Vayosoft.Persistence.Queries;
using Vayosoft.Queries;
using Vayosoft.Commons.Models.Pagination;
using Warehouse.Core.Application.SystemAdministration.Commands;
using Warehouse.Core.Application.SystemAdministration.Models;
using Warehouse.Core.Application.SystemAdministration.Queries;
using Warehouse.Core.Application.SystemAdministration.Specifications;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SystemAdministration
{
    public static class Configuration
    {
        public static IServiceCollection AddAppAdministrationServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddCommandHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<SpecificationQuery<UserSpec, IPagedEnumerable<UserEntityDto>>, IPagedEnumerable<UserEntityDto>,
                    PagingQueryHandler<string, UserSpec, UserEntity, UserEntityDto>>()
                .AddQueryHandler<SingleQuery<UserEntityDto>, UserEntityDto, SingleQueryHandler<long, UserEntity, UserEntityDto>>()
                .AddQueryHandler<GetUserSubscription, UserSubscription, HandleGetUserSubscription>()
                .AddQueryHandler<GetPermissions, RolePermissions, HandleGetPermissions>();

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
            services
                .AddCommandHandler<SaveUser, HandleSaveUser>()
                .AddCommandHandler<DeleteCommand<UserEntity>, DeleteCommandHandler<long, UserEntity>>()

                .AddCommandHandler<SavePermissions, HandleSavePermissions>()
                .AddCommandHandler<SaveRole, HandleSaveRole>()

                .AddCommandHandler<DeleteCommand<ProviderEntity>, DeleteCommandHandler<long, ProviderEntity>>()
                .AddCommandHandler<CreateOrUpdateCommand<ProviderEntity>, CreateOrUpdateHandler<long, ProviderEntity, ProviderEntity>>()
            ;
    }
}