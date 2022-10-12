using Vayosoft.Core.Mapping;
using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Application.PositioningSystem;
using Warehouse.Core.Application.PositioningSystem.Configuration;
using Warehouse.Core.Application.PositioningSystem.Filters;
using Warehouse.Core.Application.PositioningSystem.Methods;

namespace Warehouse.Core.Domain.Entities
{
    [CollectionName("dolav_settings")]
    public class IpsSettings : EntityBase<string>
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
