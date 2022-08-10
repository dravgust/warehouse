using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Management.Models;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetWarehouseSite : WarehouseSiteDto, ICommand
    {
        public class WarehouseRequestValidator : AbstractValidator<SetWarehouseSite>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.Name).NotEmpty();
            }
        }
    }

    internal class HandleSetWarehouseSite : ICommandHandler<SetWarehouseSite>
    {
        private readonly IRepository<WarehouseSiteEntity> _repository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public HandleSetWarehouseSite(IRepository<WarehouseSiteEntity> repository, IMapper mapper, IUserContext userContext)
        {
            _repository = repository;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<Unit> Handle(SetWarehouseSite request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Id))
            {
                await _repository.GetAndUpdateAsync(request.Id, entity =>
                {
                    entity.Name = request.Name;
                    entity.TopLength = request.TopLength;
                    entity.LeftLength = request.LeftLength;
                    entity.Error = request.Error;
                }, cancellationToken);
            }
            else
            {
                var entity = _mapper.Map<WarehouseSiteEntity>(request);
                var providerId = _userContext.User.Identity.GetProviderId();
                entity.ProviderId = providerId;
                await _repository.AddAsync(entity, cancellationToken: cancellationToken);
            }

            return Unit.Value;
        }
    }
}
