using Vayosoft.Core.Mapping;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Aggregates;
using Warehouse.Core.Application.ItemTracking.Services;
using Warehouse.PositioningSystem.Filters;
using Warehouse.PositioningSystem.Methods;
using Warehouse.PositioningSystem.Settings;

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
