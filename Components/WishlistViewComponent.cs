using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Core;
using Nop.Web.Framework.Components;
using Nop.Plugin.Widgets.Wishlist.Services;
using Nop.Plugin.Widgets.Wishlist.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Wishlist.Components
{
    public class WishlistViewComponent : NopViewComponent
    {
        #region Fields 

        private readonly IStoreContext _storeContext;
        private readonly IWishlistService _wishlistService;
        private readonly WishlistSettings _settings;
        #endregion

        #region Ctor

        public WishlistViewComponent(IStoreContext storeContext,
            IWishlistService wishlistService,
            WishlistSettings wishlistSettings)
        {
            _storeContext = storeContext;
            _wishlistService = wishlistService;
            _settings = wishlistSettings;
        }
        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {

            if (string.IsNullOrEmpty(widgetZone))
                throw new ArgumentNullException(nameof(widgetZone));

            if (additionalData == null)
                throw new ArgumentNullException(nameof(additionalData));

            var count = new WishlistModel();


            var model = additionalData as ProductDetailsModel;
            if (widgetZone == PublicWidgetZones.ProductBoxAddinfoBefore)
            {
                count.EnableInCategory = _settings.EnableInCategory;
                var productOverviewModel = additionalData as ProductOverviewModel;

                if (productOverviewModel == null)
                    throw new ArgumentNullException(nameof(productOverviewModel));

                count.ProductId = productOverviewModel.Id;

                if (_settings.EnableInCategory == false)
                {
                    return View("~/Plugins/Widgets.Wishlist/Views/PublicInfo.cshtml", count);
                }
                if (productOverviewModel.Id == 0)
                    throw new ArgumentNullException(nameof(productOverviewModel.Id));
                var store = await _storeContext.GetCurrentStoreAsync();

                var cart = await _wishlistService.GetShoppingCartAsync(productOverviewModel.Id, ShoppingCartType.Wishlist, store.Id,null,null);
                if (cart != null)
                    count.WishlistCounts = cart.Sum(item => item).ToString();
                var dec = 0.0;
                if (Convert.ToInt32(count.WishlistCounts) > 1000000)
                {
                    dec = ((double)Convert.ToInt32(count.WishlistCounts)) / (double)1000000;
                    var value = string.Format("{0:0.0}", dec);
                    count.WishlistCounts = value.ToString() + "M";
                }
                if (Convert.ToInt32(count.WishlistCounts) > 1000)
                {
                    dec = ((double)Convert.ToInt32(count.WishlistCounts)) / (double)1000.0;
                    var value = string.Format("{0:0.0}", dec);
                    count.WishlistCounts = value.ToString() + "K";
                }

                return View("~/Plugins/Widgets.Wishlist/Views/PublicInfo.cshtml", count);
            }

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            count.ProductId = model.Id;


            return View("~/Plugins/Widgets.Wishlist/Views/PublicInfo.Product.cshtml", count);
        }


        #endregion
    }
}
