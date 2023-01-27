using Endless.Foundation.Core;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Endless.Foundation.JSS.Services
{
    internal class DataSourceTokenService : IDataSourceTokenService
    {
        private const string TokenSite = "$site";

        private const string TokenHome = "$home";

        public bool HasToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return value.Contains(TokenSite) ||
                   value.Contains(TokenHome);
        }

        public Item ResolveSiteOrHomeToken(string value, Item home = null)
        {
            if (!HasToken(value))
            {
                return null;
            }

            var source = home ?? Utilities.GetContextHomeItem();

            if (source == null)
            {
                Log.Error("Endless - Resolve Site or Home token: Unable to get the home item.", this);

                return null;
            }

            Item target = null;

            if (value.Contains(TokenSite))
            {
                target = ResolveSiteToken(value, source);
            }
            else if (value.Contains(TokenHome))
            {
                target = ResolveHomeToken(value, source);
            }

            if (target == null)
            {
                Log.Error($"Endless - Resolve Site or Home token: Unable to resolve token (Value: \"{value}\", Home: \"{source.ID}\").", this);
            }

            return target;
        }

        #region Private Methods

        private static Item ResolveSiteToken(string value, Item home)
        {
            if (!value?.Contains(TokenSite) ?? true || home == null)
            {
                return null;
            }

            var path = value.Replace(TokenSite, home.Parent.Paths.Path);

            return Utilities.GetItem(path);
        }

        private static Item ResolveHomeToken(string value, Item home)
        {
            if (!value?.Contains(TokenHome) ?? true || home == null)
            {
                return null;
            }

            var path = value.Replace(TokenHome, home.Paths.Path);

            return Utilities.GetItem(path);
        }

        #endregion
    }
}
