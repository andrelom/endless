using Endless.Foundation.JSS.Services;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.Mvc.Presentation;
using System;

namespace Endless.Foundation.JSS.Resolvers
{
    public class RenderingContentsResolver : Sitecore.LayoutService.ItemRendering.ContentsResolvers.RenderingContentsResolver
    {
        private readonly IDataSourceTokenService _dataSourceTokenService;

        public RenderingContentsResolver(IDataSourceTokenService dataSourceTokenService)
        {
            _dataSourceTokenService = dataSourceTokenService;
        }

        protected override Item GetContextItem(Rendering rendering, IRenderingConfiguration configuration)
        {
            try
            {
                ResolveSiteOrHomeToken(rendering);
            }
            catch (Exception ex)
            {
                Log.Error("Endless - Rendering Contents Resolver", ex, this);
            }

            return base.GetContextItem(rendering, configuration);
        }

        #region Private Methods

        private void ResolveSiteOrHomeToken(Rendering rendering)
        {
            if (!_dataSourceTokenService.HasToken(rendering.DataSource))
            {
                return;
            }

            var item = _dataSourceTokenService.ResolveSiteOrHomeToken(rendering.DataSource);

            if (item != null)
            {
                rendering.DataSource = item.ID.ToString().ToUpper();
            }
            else
            {
                rendering.DataSource = null;
            }
        }

        #endregion
    }
}
