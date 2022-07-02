using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Persistence;
using Warehouse.Core.UseCases.Products.Commands;

namespace Warehouse.Core.UseCases.Products
{
    public class ProductCommandHandler : ICommandHandler<SetProduct>, ICommandHandler<DeleteProduct>
    {
        private readonly ICriteriaRepository<ProductEntity, string> _repository;
        private readonly IMapper _mapper;

        public ProductCommandHandler(ICriteriaRepository<ProductEntity, string> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SetProduct request, CancellationToken cancellationToken)
        {
            ProductEntity? entity;
            if (!string.IsNullOrEmpty(request.Id) && (entity = await _repository.FindAsync(request.Id, cancellationToken)) != null)
            {
                await _repository.UpdateAsync(_mapper.Map(request, entity), cancellationToken);
            }
            else
            {
                await _repository.AddAsync(_mapper.Map<ProductEntity>(request), cancellationToken);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteProduct request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(new ProductEntity { Id = request.Id }, cancellationToken);
            return Unit.Value;
        }
    }
}
