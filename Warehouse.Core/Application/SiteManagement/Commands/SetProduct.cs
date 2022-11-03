using FluentValidation;
using MediatR;
using Vayosoft.Commands;
using Vayosoft.Persistence;
using Vayosoft.Commons;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Application.SiteManagement.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Commands
{
    public class SetProduct : ProductDto, ICommand
    {
        public class ProductRequestValidator : AbstractValidator<SetProduct>
        {
            public ProductRequestValidator()
            {
                RuleFor(q => q.Name).NotEmpty();
            }
        }
    }
    internal class HandleSetProduct : ICommandHandler<SetProduct>
    {
        private readonly IRepository<ProductEntity> _repository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public HandleSetProduct(IRepository<ProductEntity> repository, IMapper mapper, IUserContext userContext)
        {
            _repository = repository;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<Unit> Handle(SetProduct request, CancellationToken cancellationToken)
        {
            ProductEntity entity;
            if (!string.IsNullOrEmpty(request.Id) &&
                (entity = await _repository.FindAsync(request.Id, cancellationToken)) != null)
            {
                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.Metadata = request.Metadata;
                await _repository.UpdateAsync(entity, cancellationToken);
            }
            else
            {
                entity = _mapper.Map<ProductEntity>(request);
                var providerId = _userContext.User.Identity.GetProviderId();
                entity.ProviderId = providerId;
                await _repository.AddAsync(entity, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
