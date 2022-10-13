using Warehouse.Core.Application.PositioningSystem.Domain.Methods;

namespace Warehouse.Core.Application.PositioningSystem.Domain.Settings
{
    public interface ICalculationSettings
    {
        public Type RssiFilter { get; set; }
        T Resolve<T>(Type filterType);
        public ICalculationMethod CalcMethod { get; set; }
    }
}
