using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Specifications
{
    public class WarehouseAlertSpec : SortByIdPaging<AlertEntity, object>, IFilteringSpecification<AlertEntity>
    {
        public WarehouseAlertSpec(int page, int take, string filterString)
            : base(page, take, SortOrder.Desc)
        {
            FilterString = filterString;
            if (!string.IsNullOrEmpty(FilterString))
            {
                FilterBy.Add(e => e.Name);
            }
        }

        public string FilterString { get; }

        public ICollection<Expression<Func<AlertEntity, object>>> FilterBy { get; }
            = new List<Expression<Func<AlertEntity, object>>>();
    }
}