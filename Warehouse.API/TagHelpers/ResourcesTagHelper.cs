﻿using System.Dynamic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using Vayosoft.Utilities;

namespace Warehouse.API.TagHelpers
{
    public class ResourceGroup
    {
        public string Name { get; set; }
        public IEnumerable<LocalizedString> Entries { get; set; }
    }

    [HtmlTargetElement("resources")]
    public class ResourcesTagHelper : TagHelper
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        public ResourcesTagHelper(IStringLocalizerFactory stringLocalizerFactory)
        {
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        [HtmlAttributeName("names")]
        public string[] Resources { get; set; }

        /// <summary>
        /// Execute script only once document is loaded.
        /// </summary>
        public bool OnContentLoaded { get; set; } = false;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (OnContentLoaded)
                await base.ProcessAsync(context, output);
            else
            {
                IEnumerable<ResourceGroup> groupedResources = Resources.Select(x =>
                {
                    IStringLocalizer localizer = _stringLocalizerFactory.Create(x, Assembly.GetEntryAssembly()!.FullName!);
                    return new ResourceGroup { Name = x, Entries = localizer.GetAllStrings(true).ToList() };
                });

                StringBuilder sb = new StringBuilder();
                sb.Append(groupedResources.ToJavascript());

                TagHelperContent content = await output.GetChildContentAsync();
                sb.Append(content.GetContent());

                output.TagName = "script";
                output.Content.AppendHtml(sb.ToString());
            }
        }
    }

    internal static class ResourceGroupExtensions
    {
        /// <summary>
        /// Converts the source data to a Javascript variable
        /// </summary>
        /// <param name="resources">The record to convert</param>
        /// <returns>A valid Javascript object</returns>
        internal static string ToJavascript(this IEnumerable<ResourceGroup> resources)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("window._resources = ");

            ExpandoObject uiCaptions = new ExpandoObject();

            // Get the fields
            foreach (ResourceGroup fieldGroup in resources)
                ((IDictionary<string, object>)uiCaptions)[fieldGroup.Name] =
                    fieldGroup.Entries.ToDictionary(x => x.Name.ToString(), x => x.Value);

            string serialized = uiCaptions.ToJson();
            sb.Append(serialized);
            sb.Append(";");

            return sb.ToString();
        }
    }
}