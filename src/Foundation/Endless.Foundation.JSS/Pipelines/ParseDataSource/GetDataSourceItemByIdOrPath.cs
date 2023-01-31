using Endless.Foundation.JSS.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.ParseDataSource;
using System;

namespace Endless.Foundation.JSS.Pipelines.ParseDataSource
{
    public class GetDataSourceItemByIdOrPath : Sitecore.Pipelines.ParseDataSource.GetDataSourceItemByIdOrPath
    {
        private readonly IDataSourceTokenService _dataSourceTokenService;

        public GetDataSourceItemByIdOrPath()
        {
            _dataSourceTokenService = ServiceLocator.ServiceProvider.GetService<IDataSourceTokenService>();
        }

        public new void Process(ParseDataSourceArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            try
            {
                if (_dataSourceTokenService.HasToken(args.DataSource))
                {
                    ResolveSiteOrHomeToken(args);
                }
                else
                {
                    base.Process(args);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Get DataSource Item By ID or Path", ex, this);
            }
        }

        #region Private Methods

        private void ResolveSiteOrHomeToken(ParseDataSourceArgs args)
        {
            var item = _dataSourceTokenService.ResolveSiteOrHomeToken(args.DataSource);

            if (item != null)
            {
                args.Items.Add(item);
            }

            args.AbortPipeline();
        }

        #endregion
    }
}
