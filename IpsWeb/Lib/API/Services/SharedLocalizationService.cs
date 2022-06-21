using System.Reflection;
using IpsWeb.Resources;
using Microsoft.Extensions.Localization;

namespace IpsWeb.Lib.API.Services
{
    public class SharedLocalizationService
    {
        private readonly IStringLocalizer _localizer;
        public SharedLocalizationService(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(SharedResources).GetTypeInfo().Assembly.FullName!);
            _localizer = factory.Create(nameof(SharedResources), assemblyName.Name!);
        }

        public string Get(string key)
        {
            return _localizer[key];
        }

        public string this[string key] => _localizer[key];
    }
}
