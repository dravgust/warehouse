using FluentValidation;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardSite : IQuery<DashboardBySite>
    {
        public string SiteId { set; get; }
        public class AlertRequestValidator : AbstractValidator<GetDashboardSite>
        {
            public AlertRequestValidator()
            {
                RuleFor(q => q.SiteId).NotEmpty();
            }
        }
    }

    internal class HandleGetIpsStatus : IQueryHandler<GetDashboardSite, DashboardBySite>
    {
        private readonly IReadOnlyRepository<IndoorPositionStatusEntity> _repository;
        private readonly IMapper _mapper;

        public HandleGetIpsStatus(IReadOnlyRepository<IndoorPositionStatusEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DashboardBySite> Handle(GetDashboardSite request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAsync(request.SiteId, cancellationToken);
            return _mapper.Map<DashboardBySite>(result);
        }
    }
}
