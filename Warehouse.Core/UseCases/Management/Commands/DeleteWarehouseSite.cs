using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class DeleteWarehouseSite : ICommand
    {
        public string Id { get; set; }
        public class WarehouseRequestValidator : AbstractValidator<DeleteWarehouseSite>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }

    internal class HandleDeleteWarehouseSite : ICommandHandler<DeleteWarehouseSite>
    {
        private readonly IRepository<WarehouseSiteEntity> _repository;
        private readonly IUserContext _userContext;
        private readonly IEventBus _eventBus;

        public HandleDeleteWarehouseSite(IRepository<WarehouseSiteEntity> repository, IEventBus eventBus, IUserContext userContext)
        {
            _repository = repository;
            _eventBus = eventBus;
            _userContext = userContext;
        }

        public async Task<Unit> Handle(DeleteWarehouseSite request, CancellationToken cancellationToken)
        {
            var entity = new WarehouseSiteEntity {Id = request.Id};
            await _repository.DeleteAsync(entity, cancellationToken);

            await _eventBus.Publish(UserOperation.Delete(request, _userContext.User), cancellationToken);
            return Unit.Value;
        }
    }
}
