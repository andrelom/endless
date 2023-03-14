using Endless.Foundation.Core.Extensions;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Endless.Foundation.Core
{
    public static class Utilities
    {
        private const string HomePathSuffix = "/Home";

        private const string LanguageUrlQueryKey = "sc_lang";

        public static Item GetItem(string path, Language language = null, bool core = false)
        {
            if (Context.Database == null || Context.Database.IsCore() && !core)
            {
                return null;
            }

            if (language != null)
            {
                return Context.Database.GetItem(path, language);
            }

            return Context.Database.GetItem(path);
        }

        public static IEnumerable<Item> GetItems(IEnumerable<string> paths, Language language = null, bool core = false)
        {
            return paths?.Select(path => GetItem(path, language, core)).Where(item => item != null) ?? Array.Empty<Item>();
        }

        public static Item GetItem(ID id, Language language = null, bool core = false)
        {
            if (Context.Database == null || Context.Database.IsCore() && !core)
            {
                return null;
            }

            if (language != null)
            {
                return Context.Database.GetItem(id, language);
            }

            return Context.Database.GetItem(id);
        }

        public static IEnumerable<Item> GetItems(IEnumerable<ID> ids, Language language = null, bool core = false)
        {
            return ids?.Select(id => GetItem(id, language, core)).Where(item => item != null) ?? Array.Empty<Item>();
        }

        public static Item GetSiteItemByName(string name, Language language = null)
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

            if (language != null)
            {
                return GetItem(site.RootPath, language);
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
            var site = Context.Item?.GetRelativeSite(Context.Item.Language);

            if (site == null)
            {
                return null;
            }

            return GetItem($"{site.Paths.Path}{HomePathSuffix}", site.Language);
        }

        public static Item GetHttpContextHomeItem()
        {
            var url = HttpContext.Current.Request.Url;
            var query = HttpUtility.ParseQueryString(url.Query);

            var site = GetSiteItemByUrlQuery(query);

            if (site != null)
            {
                return GetItem($"{site.Paths.Path}{HomePathSuffix}", site.Language);
            }

            var item = GetItemByUrlQuery(query);

            if (item == null)
            {
                return null;
            }

            if (item.Paths.Path.EndsWith(HomePathSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }

            return GetItem($"{item.Paths.Path}{HomePathSuffix}", item.Language);
        }

        #region Private Methods

        private static Item GetSiteItemByUrlQuery(NameValueCollection query)
        {
            const string key = "sc_site";

            var site = query.Get(key);

            if (string.IsNullOrWhiteSpace(site))
            {
                return null;
            }

            var lang = query.Get(LanguageUrlQueryKey);

            if (!string.IsNullOrWhiteSpace(lang) && Language.TryParse(lang, out var language))
            {
                return GetSiteItemByName(site, language);
            }

            return GetSiteItemByName(site);
        }

        private static Item GetItemByUrlQuery(NameValueCollection query)
        {
            const string key = "item";

            var guid = query.Get(key);

            if (string.IsNullOrWhiteSpace(guid))
            {
                return null;
            }

            if (!guid?.IsValidGuid() ?? true)
            {
                return null;
            }

            var id = new ID(guid);
            var lang = query.Get(LanguageUrlQueryKey);

            if (!string.IsNullOrWhiteSpace(lang) && Language.TryParse(lang, out var language))
            {
                return GetItem(id, language);
            }

            return GetItem(id);
        }

        #endregion
    }
}
