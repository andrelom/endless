using Sitecore.Data.Items;

namespace Endless.Foundation.JSS.Services
{
    public interface IDataSourceTokenService
    {
        bool HasToken(string value);

        Item ResolveSiteOrHomeToken(string value, Item home = null);
    }
}
