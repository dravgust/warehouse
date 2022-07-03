using System.Globalization;

namespace Warehouse.Core.UseCases.Providers.Models
{
    public abstract partial class Provider
    {
        private class DefaultProvider : Provider
        {
            public DefaultProvider(int id, string name) : base(id, name) { }

            public override CultureInfo Culture
                => CultureInfo.GetCultureInfo("en");
        }
    }
}
