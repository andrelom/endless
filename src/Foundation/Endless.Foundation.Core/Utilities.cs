using Endless.Foundation.Core.Extensions;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Sites;
using System;
using System.Linq;
using System.Web;

namespace Endless.Foundation.Core
{
    public static class Utilities
    {
        private const string HomePathSuffix = "/Home";

        public static Item GetItem(string path, bool core = false)
        {
            if (Context.Database == null || Context.Database.IsCore() && !core)
            {
                return null;
            }

            return Context.Database.GetItem(path);
        }

        public static Item GetItem(ID id, bool core = false)
        {
            if (Context.Database == null || Context.Database.IsCore() && !core)
            {
                return null;
            }

            return Context.Database.GetItem(id);
        }

        public static Item GetSiteItemByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var site = SiteContextFactory.Sites
                .Where(entry => !string.IsNullOrWhiteSpace(entry.Name) && entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (site == null)
            {
                return null;
            }

            return GetItem(site.RootPath);
        }

        public static Item GetContextHomeItem()
        {
            // If the request came from "/sitecore/api/layout/render/jss", we get it from the HTTP context URL query,
            // otherwise (a.k.a. "/sitecore/api/graph/edge"), we get it from the Sitecore Context Item.
            return GetHttpContextHomeItem() ?? GetSitecoreContextHomeItem();
        }

        public static Item GetSitecoreContextHomeItem()
        {
            var site = Context.Item?.GetRelativeSite();

            if (site == null)
            {
                return null;
            }

            return GetItem($"{site.Paths.Path}{HomePathSuffix}");
        }

        public static Item GetHttpContextHomeItem()
        {
            var url = HttpContext.Current.Request.Url;
            var site = GetSiteItemByUrlQuery(url);

            if (site != null)
            {
                return GetItem($"{site.Paths.Path}{HomePathSuffix}");
            }

            var item = GetItemByUrlQuery(url);

            if (item != null)
            {
                if (item.Paths.Path.EndsWith(HomePathSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }

                return GetItem($"{item.Paths.Path}{HomePathSuffix}");
            }

            return null;
        }

        #region Private Methods

        private static Item GetSiteItemByUrlQuery(Uri url)
        {
            const string key = "sc_site";

            var value = HttpUtility.ParseQueryString(url.Query).Get(key);

            return GetSiteItemByName(value);
        }

        private static Item GetItemByUrlQuery(Uri url)
        {
            const string key = "item";

            var value = HttpUtility.ParseQueryString(url.Query).Get(key);

            if (!value?.IsValidGuid() ?? true)
            {
                return null;
            }

            return GetItem(new ID(value));
        }

        #endregion
    }
}
