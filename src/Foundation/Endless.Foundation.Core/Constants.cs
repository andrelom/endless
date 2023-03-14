namespace Endless.Foundation.Core
{
    public static class Constants
    {
        // All field types that store ID must be listed here.
        // More at: https://doc.sitecore.com/xp/en/developers/102/sitecore-experience-manager/the-link-field-types.html
        public static readonly string[] LinkFieldTypes = new[] {
            "Droplink",
            "Droptree",
            "General Link",
            "General Link with Search"
        };

        // All field types that store a list of IDs must be listed here.
        // More at: https://doc.sitecore.com/xp/en/developers/102/sitecore-experience-manager/the-simple-field-types.html
        public static readonly string[] ListFieldTypes = new[] {
            "Checklist",
            "Multilist",
            "Multilist with Search",
            "Treelist"
        };
    }
}
