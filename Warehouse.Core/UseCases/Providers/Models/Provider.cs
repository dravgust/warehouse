using System.Globalization;
using Vayosoft.Core.SharedKernel.Enums;

namespace Warehouse.Core.UseCases.Providers.Models
{
    public abstract partial class Provider : Enumeration
    {
        public static Provider Default = new DefaultProvider(0, nameof(Default));

        public abstract CultureInfo Culture { get; }

        protected Provider(int id, string name) : base(id, name) { }

        public static explicit operator Provider(ulong providerId) 
            => ParseId<Provider>((int) providerId);

        public static explicit operator Provider(string providerName) 
            => ParseName<Provider>(providerName);
    }
}
