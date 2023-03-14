using Endless.Foundation.Core;
using Endless.Foundation.Core.Extensions;
using Humanizer;
using Newtonsoft.Json.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;
using System;
using System.Linq;

namespace Endless.Foundation.JSS
{
    public class ComplexRenderingContentsResolver : RenderingContentsResolver
    {
        private const string ResolverTypeFieldKey = "_ComplexRenderingContentsResolverType";

        protected override JObject ProcessItem(Item item, Rendering rendering, IRenderingConfiguration configuration)
        {
            if (item == null)
            {
                return new JObject();
            }

            var data = base.ProcessItem(item, rendering, configuration);

            SetUniqueKey(data, item);

            RemoveInternalFieldKeys(data);

            ProcessLinkFields(data, item, rendering, configuration);
            ProcessListFields(data, item, rendering, configuration);

            if (!item.Children.Any())
            {
                return data;
            }

            if (IsResolverType(item, Types.Collection))
            {
                SetDataCollection(data, item, rendering, configuration);
            }
            else if (IsResolverType(item, Types.KeyValuePairs))
            {
                SetDataKeyValuePairs(data, item, rendering, configuration);
            }

            return data;
        }

        #region Private Methods

        private bool IsResolverType(Item data, string target)
        {
            var source = data.GetTextFieldValue(ResolverTypeFieldKey);

            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            return source.Equals(target, StringComparison.OrdinalIgnoreCase);
        }

        private void SetUniqueKey(JObject data, Item item)
        {
            const string key = "uid";

            if (!data.TryGetValue(key, out var _))
            {
                data[key] = item.ID.ToString();
            }
        }

        private void SetDataCollection(JObject data, Item item, Rendering rendering, IRenderingConfiguration configuration)
        {
            const string key = "$children";

            var values = item.Children.Select(child => ProcessItem(child, rendering, configuration));

            data[key] = new JArray(values);
        }

        private void SetDataKeyValuePairs(JObject data, Item item, Rendering rendering, IRenderingConfiguration configuration)
        {
            var grouping = item.Children.GroupBy(child => $"${child.Name.Camelize()}");

            foreach (var group in grouping)
            {
                foreach (var child in group)
                {
                    data[group.Key] = ProcessItem(child, rendering, configuration);
                }
            }
        }

        private void RemoveInternalFieldKeys(JObject data)
        {
            data.Remove(ResolverTypeFieldKey);
        }

        private void ProcessLinkFields(JObject data, Item item, Rendering rendering, IRenderingConfiguration configuration)
        {
            var fields = item.GetFieldsByLinkTypes();

            foreach (var field in fields)
            {
                var text = item.GetTextFieldValue(field.Name);

                if (!text?.IsValidGuid() ?? true)
                {
                    continue;
                }

                var target = Utilities.GetItem(new ID(text));
                var key = field.Name.Camelize();
                var value = ProcessItem(target, rendering, configuration);

                data[key] = value;
            }
        }

        private void ProcessListFields(JObject data, Item item, Rendering rendering, IRenderingConfiguration configuration)
        {
            var fields = item.GetFieldsByListTypes();

            foreach (var field in fields)
            {
                var ids = item.GetListFieldValues(field.Name);
                var targets = Utilities.GetItems(ids);
                var key = field.Name.Camelize();
                var value = new JArray(targets.Select(target => ProcessItem(target, rendering, configuration)));

                data[key] = value;
            }
        }

        #endregion

        #region Nested Classes

        private static class Types
        {
            public const string Collection = nameof(Collection);

            public const string KeyValuePairs = nameof(KeyValuePairs);
        }

        #endregion
    }
}
