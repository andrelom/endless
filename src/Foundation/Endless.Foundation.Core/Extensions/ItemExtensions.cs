using Sitecore.Data.Items;
using Sitecore.Sites;
using System;
using System.Linq;

namespace Endless.Foundation.Core.Extensions
{
    public static class ItemExtensions
    {
        public static Item GetRelativeSite(this Item item)
        {
            var site = SiteContextFactory.Sites
                .Where(entry => !string.IsNullOrWhiteSpace(entry.RootPath) && item.Paths.Path.StartsWith(entry.RootPath, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(entry => entry.RootPath.Length)
                .FirstOrDefault();

            if (site == null)
            {
                return null;
            }

            return Utilities.GetItem(site.RootPath);
        }
    }
}
