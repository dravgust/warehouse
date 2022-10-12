using System.Globalization;
using System.Text.Json.Serialization;

namespace Warehouse.Core.Application.UseCases.Administration.Models
{
    public abstract partial class Provider
    {
        private class DefaultProvider : Provider
        {
            public DefaultProvider(int id, string name) : base(id, name) { }
            [JsonIgnore]
            public override CultureInfo Culture
                => CultureInfo.GetCultureInfo("en");
        }
    }
}
