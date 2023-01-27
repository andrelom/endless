using Endless.Foundation.Core.Extensions;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using System.Web;

namespace Endless.Foundation.Core
{
    public static class Utilities
    {
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

            return GetItem($"{site.Paths.Path}/Home");
        }

        public static Item GetHttpContextHomeItem()
        {
            const string key = "item";

            var url = HttpContext.Current.Request.Url;
            var uid = HttpUtility.ParseQueryString(url.Query).Get(key);

            if (string.IsNullOrWhiteSpace(uid))
            {
                return null;
            }

            return GetItem(uid);
        }
    }
}
