using Endless.Foundation.JSS.Services;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering;
using System;

namespace Endless.Foundation.JSS.Pipelines.RenderJsonRendering
{
    public class CustomInitialize : Initialize
    {
        private readonly IDataSourceTokenService _dataSourceTokenService;

        public CustomInitialize(IConfiguration configuration, IDataSourceTokenService dataSourceTokenService) : base(configuration)
        {
            _dataSourceTokenService = dataSourceTokenService;
        }

        protected override void SetResult(RenderJsonRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            try
            {
                if (_dataSourceTokenService.HasTokens(args.Rendering?.DataSource))
                {
                    ResolveSiteOrHomeToken(args);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{GetType()}", ex, this);
            }

            base.SetResult(args);
        }

        #region Private Methods

        private void ResolveSiteOrHomeToken(RenderJsonRenderingArgs args)
        {
            var item = _dataSourceTokenService.ResolveTokensAndGetItem(args.Rendering?.DataSource);

            if (item != null)
            {
                args.Rendering.DataSource = item.ID.ToString().ToUpper();
            }
            else
            {
                Log.Warn($"{GetType()}: Unable to resolve datasource (\"{args.Rendering?.DataSource}\").", this);
            }
        }

        #endregion
    }
}
