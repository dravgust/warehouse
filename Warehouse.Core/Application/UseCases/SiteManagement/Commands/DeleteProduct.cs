using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Events;

namespace Warehouse.Core.Application.UseCases.SiteManagement.Commands
{
    public sealed class DeleteProduct : ICommand
    {
        public string Id { get; set; } = null!;

        public class CertificateRequestValidator : AbstractValidator<DeleteProduct>
        {
            public CertificateRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }

    internal sealed class HandleDeleteProduct : ICommandHandler<DeleteProduct>
    {
        private readonly IRepositoryBase<ProductEntity> _repository;
        private readonly IUserContext _userContext;
        private readonly IEventBus _eventBus;

        public HandleDeleteProduct(
            IRepositoryBase<ProductEntity> repository,
            IUserContext userContext,
            IEventBus eventBus)
        {
            _repository = repository;
            _userContext = userContext;
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(DeleteProduct request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(new ProductEntity { Id = request.Id }, cancellationToken);

            await _eventBus.Publish(UserOperation.Delete(request, _userContext.User), cancellationToken);
            return Unit.Value;
        }
    }
}
