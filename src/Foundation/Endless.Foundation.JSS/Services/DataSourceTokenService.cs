using Endless.Foundation.Core;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Endless.Foundation.JSS.Services
{
    internal class DataSourceTokenService : IDataSourceTokenService
    {
        private const string SiteToken = "$site";

        private const string HomeToken = "$home";

        public bool HasTokens(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return value.Contains(SiteToken) ||
                   value.Contains(HomeToken);
        }

        public string ResolveTokens(string value, Item home = null)
        {
            if (!HasTokens(value))
            {
                return value;
            }

            var source = GetSource(home);

            if (source == null)
            {
                return value;
            }

            value = value.Contains(SiteToken) ? ResolveSiteToken(value, source) : value;
            value = value.Contains(HomeToken) ? ResolveHomeToken(value, source) : value;

            return value;
        }

        public Item ResolveTokensAndGetItem(string value, Item home = null)
        {
            if (!HasTokens(value))
            {
                return null;
            }

            var source = GetSource(home);

            if (source == null)
            {
                return null;
            }

            var path = ResolveTokens(value, source);
            var target = Utilities.GetItem(path, source.Language);

            if (target == null)
            {
                Log.Error($"{GetType()}: Unable to resolve item (Value: \"{value}\", Home: \"{source.ID}\").", this);
            }

            return target;
        }

        #region Private Methods

        private Item GetSource(Item home = null)
        {
            var source = home ?? Utilities.GetContextHomeItem();

            if (source != null)
            {
                return home;
            }

            Log.Error($"{GetType()}: Unable to get the home item.", this);

            return null;
        }

        private static string ResolveSiteToken(string value, Item home)
        {
            if (!value?.Contains(SiteToken) ?? true || home == null)
            {
                return null;
            }

            return value.Replace(SiteToken, home.Parent.Paths.Path);
        }

        private static string ResolveHomeToken(string value, Item home)
        {
            if (!value?.Contains(HomeToken) ?? true || home == null)
            {
                return null;
            }

            return value.Replace(HomeToken, home.Paths.Path);
        }

        #endregion
    }
}
