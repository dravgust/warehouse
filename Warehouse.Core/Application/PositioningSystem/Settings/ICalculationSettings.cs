using Warehouse.Core.Application.PositioningSystem.Methods;

namespace Warehouse.Core.Application.PositioningSystem.Settings
{
    public interface ICalculationSettings
    {
        public Type RssiFilter { get; set; }
        T Resolve<T>(Type filterType);
        public ICalculationMethod CalcMethod { get; set; }
    }
}
