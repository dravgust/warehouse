using Warehouse.IPS.Methods;

namespace Warehouse.IPS.Configuration
{
    public interface ICalculationSettings
    {
        public Type RssiFilter { get; set; }
        T Resolve<T>(Type filterType);
        public ICalculationMethod CalcMethod { get; set; }
    }
}
