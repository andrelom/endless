using Endless.Foundation.JSS.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Endless.Foundation.JSS
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection services)
        {
            // Services.
            services.AddTransient<IDataSourceTokenService, DataSourceTokenService>();
        }
    }
}
