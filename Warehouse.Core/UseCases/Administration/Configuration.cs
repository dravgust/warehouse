using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence.Commands;
using Vayosoft.Core.Persistence.Queries;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.Commands;
using Warehouse.Core.UseCases.Administration.Models;
using Warehouse.Core.UseCases.Administration.Queries;
using Warehouse.Core.UseCases.Administration.Specifications;

namespace Warehouse.Core.UseCases.Administration
{ 
    internal static class Configuration
    {
        public static IServiceCollection AddWarehouseAdministrationServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddCommandHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<SpecificationQuery<UserSpec, IPagedEnumerable<UserEntityDto>>, IPagedEnumerable<UserEntityDto>,
                    PagingQueryHandler<long, UserSpec, UserEntity, UserEntityDto>>()
                .AddQueryHandler<SingleQuery<UserEntityDto>, UserEntityDto, SingleQueryHandler<long, UserEntity, UserEntityDto>>()
                .AddQueryHandler<GetUserSubscription, UserSubscription, HandleGetUserSubscription>()
                .AddQueryHandler<GetPermissions, RolePermissions, HandleGetPermissions>();

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
            services
                .AddCommandHandler<CreateOrUpdateCommand<UserEntityDto>, CreateOrUpdateHandler<long, UserEntity, UserEntityDto>>()
                .AddCommandHandler<SavePermissions, HandleSavePermissions>()
                .AddCommandHandler<SaveRole, HandleSaveRole>();
    }
}