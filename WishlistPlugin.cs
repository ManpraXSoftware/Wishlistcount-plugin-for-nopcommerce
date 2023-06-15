using Nop.Core;
using Nop.Plugin.Widgets.Wishlist.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Wishlist
{
    public class WishlistPlugin : BasePlugin , IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public WishlistPlugin(IWebHelper webHelper,
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
        }

        public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
                return typeof(WishlistViewComponent);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.ProductBoxAddinfoBefore,   //catalog 
                PublicWidgetZones.ProductDetailsOverviewTop  //productdetails 
            });
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/Wishlist/Configure";
        }

        public override async Task InstallAsync()
        {
            //settings
            var settings = new WishlistSettings
            {
                Days = Days.OneMonth,
                Text = "Shoppers"

            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.Wishlist.Fields.Days"] = "Days want to be ",
                ["Plugins.Widgets.Wishlist.Fields.Days.Hint"] = "Specify days",
                ["Plugins.Widgets.Wishlist.Fields.Text"] = "Text which display",
                ["Plugins.Widgets.Wishlist.Fields.Text.Hint"] = "Specify the Text",
                ["Plugins.Widgets.Wishlist.Fields.EnableInCategory"] = "Enable In Category",
                ["Plugins.Widgets.Wishlist.Fields.EnableInCategory.Hint"] = "Determine whether this feature is enable in category page or not",


                ["Plugins.Widgets.Wishlist.Instructions"] = @"
                    <p>
                      Wishlist Plugin Helps to <br>
	                Display the count of wishlisters of products in the Catalog and product details page. <br> 
                    Note :<br> 
                    1. You can enter the texts in the display string. <br> 2. You can choose records for a particular time period <br>
                    <b> For example </b> : if you type <b>'Shoppers' </b> as Text and choose <b>'one month'</b> as period, <br> 
                    The display string shoud be like : <br> <b> 1.5k <u>'shoppers'</u> wishlisted in last <u>30 days</u></b> (1k is an assumption).                     
                    </p>",
            });
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<WishlistSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.Wishlist");

            await base.UninstallAsync();
        }
    }
}