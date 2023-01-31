using Endless.Foundation.JSS.Services;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering;
using System;

namespace Endless.Foundation.JSS.Pipelines.RenderJsonRendering
{
    public class Initialize : Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering.Initialize
    {
        private readonly IDataSourceTokenService _dataSourceTokenService;

        public Initialize(IConfiguration configuration, IDataSourceTokenService dataSourceTokenService) : base(configuration)
        {
            _dataSourceTokenService = dataSourceTokenService;
        }

        protected override RenderedJsonRendering CreateResultInstance(RenderJsonRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            try
            {
                if (_dataSourceTokenService.HasToken(args.Rendering?.DataSource))
                {
                    return ResolveSiteOrHomeToken(args);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Endless - Initialize Render JSON Rendering", ex, this);
            }

            return base.CreateResultInstance(args);
        }

        #region Private Methods

        private RenderedJsonRendering ResolveSiteOrHomeToken(RenderJsonRenderingArgs args)
        {
            var result = base.CreateResultInstance(args);
            var item = _dataSourceTokenService.ResolveSiteOrHomeToken(result.DataSource);

            if (item != null)
            {
                result.DataSource = item.ID.ToString().ToUpper();
            }
            else
            {
                result.DataSource = null;
            }

            return result;
        }

        #endregion
    }
}
