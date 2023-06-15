using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.Wishlist.Models;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Components;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Wishlist.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class WishlistController : BasePluginController
    {
        #region Fields
        
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public WishlistController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods       

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var wishlistSettings = await _settingService.LoadSettingAsync<WishlistSettings>(storeScope);

            var model = new ConfigurationModel
            {
                DaysId = Convert.ToInt32(wishlistSettings.Days),
                DaysValues = await wishlistSettings.Days.ToSelectListAsync(),
                Text = wishlistSettings.Text,
                EnableInCategory = wishlistSettings.EnableInCategory,
                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.DaysId_OverrideForStore = await _settingService.SettingExistsAsync(wishlistSettings, x => x.Days, storeScope);
                model.Text_OverrideForStore = await _settingService.SettingExistsAsync(wishlistSettings, x => x.Text, storeScope);
                model.EnableInCategory_OverrideForStore = await _settingService.SettingExistsAsync(wishlistSettings, x => x.EnableInCategory, storeScope);
            }

            return View("~/Plugins/Widgets.Wishlist/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var wishlistSettings = await _settingService.LoadSettingAsync<WishlistSettings>(storeScope);

            //save settings
            wishlistSettings.Days = (Days)model.DaysId;
            wishlistSettings.Text = model.Text;
            wishlistSettings.EnableInCategory = model.EnableInCategory;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            await _settingService.SaveSettingOverridablePerStoreAsync(wishlistSettings, x => x.Days, model.DaysId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(wishlistSettings, x => x.Text, model.Text_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(wishlistSettings, x => x.EnableInCategory, model.EnableInCategory_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}