using Vayosoft.Core.Mapping;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Aggregates;
using Warehouse.Core.Application.PositioningSystem.Domain.Filters;
using Warehouse.Core.Application.PositioningSystem.Domain.Methods;
using Warehouse.Core.Application.PositioningSystem.Domain.Settings;
using Warehouse.Core.Application.PositioningSystem.Services;

namespace Warehouse.Core.Domain.Entities
{
    [AggregateName("dolav_settings")]
    public class IpsSettings : EntityBase<string>, IAggregateRoot
    {
        public int CalcMethod { set; get; }
        public int BufferLength { set; get; }
        public SmoothAlgorithm SmoothAlgorithm { set; get; }
        public SelectMethod SelectMethod { set; get; }

        public CalculationSettings GetCalculationSettings()
        {
            return new CalculationSettings
            {
                CalcMethod = CalcMethod switch
                {
                    1 => new CalcMethod1(),
                    _ => new CalcMethod2(),
                },
                RssiFilter = SmoothAlgorithm switch
                {
                    SmoothAlgorithm.Kalman => typeof(KalmanRssiFilter),
                    SmoothAlgorithm.Feedback => typeof(FeedbackRssiFilter),
                    SmoothAlgorithm.Custom => typeof(CustomRssiFilter),
                    _ => typeof(CustomRssiFilter)
                }
            };
        }
    }
}
