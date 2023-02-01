using Sitecore.Data.Items;

namespace Endless.Foundation.JSS.Services
{
    public interface IDataSourceTokenService
    {
        bool HasTokens(string value);

        string ResolveTokens(string value, Item home = null);

        Item ResolveTokensAndGetItem(string value, Item home = null);
    }
}
