using Endless.Foundation.Core.Extensions;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using System;
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
            const string key = "item";

            var url = HttpContext.Current.Request.Url;
            var uid = HttpUtility.ParseQueryString(url.Query).Get(key);

            if (!uid?.IsValidGuid() ?? true)
            {
                return null;
            }

            var item = GetItem(uid);

            if (item?.Paths.Path.EndsWith(HomePathSuffix, StringComparison.OrdinalIgnoreCase) ?? false)
            {
                return item;
            }

            var site = item?.GetRelativeSite();

            if (site == null)
            {
                return null;
            }

            return GetItem($"{site.Paths.Path}{HomePathSuffix}");
        }
    }
}
