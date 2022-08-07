using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.ValueObjects;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetProduct : ProductDto, ICommand
    {
        public class ProductRequestValidator : AbstractValidator<SetProduct>
        {
            public ProductRequestValidator()
            {
                RuleFor(q => q.Name).NotEmpty();
                //RuleFor(q => q.MacAddress).MacAddress();
            }
        }
    }
    internal class HandleSetProduct : ICommandHandler<SetProduct>
    {
        private readonly IRepository<ProductEntity> _repository;
        private readonly IMapper _mapper;
        private readonly UserContext _context;

        public HandleSetProduct(IRepository<ProductEntity> repository, IMapper mapper, UserContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
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
                entity.ProviderId = _context.ProviderId ?? 0;
                await _repository.AddAsync(entity, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
