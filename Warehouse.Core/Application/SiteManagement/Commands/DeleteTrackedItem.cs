﻿using FluentValidation;
using MediatR;
using Vayosoft.Commands;
using Vayosoft.Persistence;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Commands
{
    public sealed class DeleteTrackedItem : ICommand
    {
        public string MacAddress { get; set; }
        public class WarehouseRequestValidator : AbstractValidator<DeleteTrackedItem>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.MacAddress).NotEmpty();
            }
        }
    }

    internal sealed class HandleDeleteTrackedItem : ICommandHandler<DeleteTrackedItem>
    {
        private readonly IRepository<TrackedItem> _repository;

        public HandleDeleteTrackedItem(IRepository<TrackedItem> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteTrackedItem request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(request.MacAddress, cancellationToken);

            return Unit.Value;
        }
    }
}
