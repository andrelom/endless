using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Endless.Foundation.Core.Extensions
{
    public static class ItemExtensions
    {
        public static Item GetRelativeSite(this Item item, Language language = null)
        {
            var site = SiteContextFactory.Sites
                .Where(entry => !string.IsNullOrWhiteSpace(entry.RootPath) && item.Paths.Path.StartsWith(entry.RootPath, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(entry => entry.RootPath.Length)
                .FirstOrDefault();

            if (site == null)
            {
                return null;
            }

            if (language != null)
            {
                return Utilities.GetItem(site.RootPath, language);
            }

            return Utilities.GetItem(site.RootPath);
        }

        #region Field Assessors

        public static IEnumerable<Field> GetFieldsByTypes(this Item item, string[] types, bool system = false)
        {
            const string prefix = "_";

            var fields = item.Fields?.ToList();

            if (fields == null || types == null)
            {
                return Array.Empty<Field>();
            }

            return fields
                .Where(field => system ? true : !field.Name.StartsWith(prefix))
                .Where(field => types.Any(type => string.Equals(field?.Type, type, StringComparison.OrdinalIgnoreCase)));
        }

        public static IEnumerable<Field> GetFieldsByLinkTypes(this Item item, bool system = false)
        {
            return item.GetFieldsByTypes(Constants.LinkFieldTypes, system);
        }

        public static IEnumerable<Field> GetFieldsByListTypes(this Item item, bool system = false)
        {
            return item.GetFieldsByTypes(Constants.ListFieldTypes, system);
        }

        #endregion

        #region Field Value Assessors

        public static string GetTextFieldValue(this Item item, string key)
        {
            return item != null ? item[key] : string.Empty;
        }

        public static IList<string> GetTextFieldValues(this Item item, string key)
        {
            var source = item.GetTextFieldValue(key);

            if (string.IsNullOrWhiteSpace(source))
            {
                return Array.Empty<string>();
            }

            return source.Split('|')
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();
        }

        public static IList<ID> GetListFieldValues(this Item item, string key)
        {
            return item.GetTextFieldValues(key)
                .Where(value => value.IsValidGuid())
                .Select(value => new ID(value))
                .ToList();
        }

        #endregion
    }
}
