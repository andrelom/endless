using Sitecore.Data;
using System;

namespace Endless.Foundation.Core.Extensions
{
    public static class DatabaseExtensions
    {
        public static bool IsCore(this Database database)
        {
            const string name = "core";

            return string.Equals(database.Name, name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
