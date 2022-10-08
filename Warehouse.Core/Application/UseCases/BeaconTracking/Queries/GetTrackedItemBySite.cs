using FluentValidation;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Warehouse.Core.Application.UseCases.BeaconTracking.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.UseCases.BeaconTracking.Queries
{
    public class GetTrackedItemBySite : IQuery<TrackedItemBySiteDto>
    {
        public string SiteId { set; get; }
        public class AlertRequestValidator : AbstractValidator<GetTrackedItemBySite>
        {
            public AlertRequestValidator()
            {
                RuleFor(q => q.SiteId).NotEmpty();
            }
        }
    }

    internal class HandleGetIpsStatus : IQueryHandler<GetTrackedItemBySite, TrackedItemBySiteDto>
    {
        private readonly IReadOnlyRepository<IndoorPositionStatusEntity> _repository;
        private readonly IMapper _mapper;

        public HandleGetIpsStatus(IReadOnlyRepository<IndoorPositionStatusEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TrackedItemBySiteDto> Handle(GetTrackedItemBySite request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAsync(request.SiteId, cancellationToken);
            return _mapper.Map<TrackedItemBySiteDto>(result);
        }
    }
}
