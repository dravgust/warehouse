using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence.Queries;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.Commands;
using Warehouse.Core.UseCases.Administration.Models;
using Warehouse.Core.UseCases.Administration.Queries;
using Warehouse.Core.UseCases.Administration.Specifications;
using Vayosoft.Core.Persistence.Commands;

namespace Warehouse.Core.UseCases.Administration
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