﻿using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Events;

namespace Warehouse.Core.Application.SiteManagement.Commands
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

            await _eventBus.Publish(UserOperationEvent.Delete(request, _userContext.User), cancellationToken);
            return Unit.Value;
        }
    }
}
