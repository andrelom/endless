using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Sites;
using System;
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
    }
}
