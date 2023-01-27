using Endless.Foundation.JSS.Services;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering;
using System;

namespace Endless.Foundation.JSS.Pipelines
{
    public class InitializeRenderJsonRendering : Initialize
    {
        private readonly IDataSourceTokenService _dataSourceTokenService;

        public InitializeRenderJsonRendering(IConfiguration configuration, IDataSourceTokenService dataSourceTokenService) : base(configuration)
        {
            _dataSourceTokenService = dataSourceTokenService;
        }

        protected override RenderedJsonRendering CreateResultInstance(RenderJsonRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var result = base.CreateResultInstance(args);

            try
            {
                ResolveSiteOrHomeToken(result);
            }
            catch (Exception ex)
            {
                Log.Error("Endless - Initialize Render JSON Rendering", ex, this);
            }

            return result;
        }

        #region Private Methods

        private void ResolveSiteOrHomeToken(RenderedJsonRendering result)
        {
            if (!_dataSourceTokenService.HasToken(result.DataSource))
            {
                return;
            }

            var item = _dataSourceTokenService.ResolveSiteOrHomeToken(result.DataSource);

            if (item != null)
            {
                result.DataSource = item.ID.ToString().ToUpper();
            }
        }

        #endregion
    }
}
