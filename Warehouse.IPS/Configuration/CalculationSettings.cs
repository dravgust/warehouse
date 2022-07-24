using Warehouse.IPS.Methods;

namespace Warehouse.IPS.Configuration
{
    public class CalculationSettings : ICalculationSettings
    {
        public ICalculationMethod CalcMethod { get; set; }
        public Type RssiFilter { get; set; }
        public T Resolve<T>(Type filterType)
        {
            return typeof(T).IsAssignableFrom(filterType)
                ? (T)Activator.CreateInstance(filterType)
                : throw new Exception($"{nameof(filterType)} must implement {nameof(T)}.");
        }
    }
}
