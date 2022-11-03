using System.Globalization;
using Vayosoft.Commons.Enums;

namespace Warehouse.Core.Application.SystemAdministration.Models
{
    public abstract partial class Provider : Enumeration
    {
        public static Provider Default = new DefaultProvider(0, nameof(Default));

        public abstract CultureInfo Culture { get; }

        protected Provider(int id, string name) : base(id, name) { }

        public static explicit operator Provider(long providerId) //=>  ParseId<Provider>((int) providerId);
            => GetAll<Provider>().FirstOrDefault(p => p.Id == providerId) ?? Default;

        public static explicit operator Provider(string providerName) //=> ParseName<Provider>(providerName);
            => GetAll<Provider>().FirstOrDefault(p => p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase)) ?? Default;
    }
}
