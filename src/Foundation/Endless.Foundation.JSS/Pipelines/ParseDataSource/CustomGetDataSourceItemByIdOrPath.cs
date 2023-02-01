using Endless.Foundation.JSS.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.ParseDataSource;
using System;

namespace Endless.Foundation.JSS.Pipelines.ParseDataSource
{
    public class CustomGetDataSourceItemByIdOrPath : GetDataSourceItemByIdOrPath
    {
        private readonly IDataSourceTokenService _dataSourceTokenService;

        public CustomGetDataSourceItemByIdOrPath()
        {
            _dataSourceTokenService = ServiceLocator.ServiceProvider.GetService<IDataSourceTokenService>();
        }

        public new void Process(ParseDataSourceArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            try
            {
                if (_dataSourceTokenService.HasTokens(args.DataSource))
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
                Log.Error($"{GetType()}", ex, this);
            }
        }

        #region Private Methods

        private void ResolveSiteOrHomeToken(ParseDataSourceArgs args)
        {
            var item = _dataSourceTokenService.ResolveTokensAndGetItem(args.DataSource);

            if (item != null)
            {
                args.Items.Add(item);
            }
            else
            {
                Log.Warn($"{GetType()}: Unable to resolve datasource (\"{args.DataSource}\").", this);
            }

            args.AbortPipeline();
        }

        #endregion
    }
}
