using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Application.Services.Security;
using Warehouse.Core.Application.UseCases.Management.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.UseCases.Management.Commands
{
    public sealed class SetWarehouseSite : WarehouseSiteDto, ICommand
    {
        public class WarehouseRequestValidator : AbstractValidator<SetWarehouseSite>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.Name).NotEmpty();
            }
        }
    }

    internal sealed class HandleSetWarehouseSite : ICommandHandler<SetWarehouseSite>
    {
        private readonly IRepositoryBase<WarehouseSiteEntity> _repository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public HandleSetWarehouseSite(IRepositoryBase<WarehouseSiteEntity> repository, IMapper mapper, IUserContext userContext)
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
