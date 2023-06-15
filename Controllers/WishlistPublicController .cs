using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.Wishlist.Models;
using Nop.Plugin.Widgets.Wishlist.Services;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.Wishlist.Controllers
{   
    public class WishlistPublicController : BasePluginController
    {
        #region Fields
        
        private readonly IStoreContext _storeContext;
        private readonly WishlistSettings _settings;
        private readonly IWishlistService _wishlistService;

        #endregion

        #region Ctor

        public WishlistPublicController(IStoreContext storeContext,            
            WishlistSettings settings,
            IWishlistService wishlistService)
        {          
            _storeContext = storeContext;
            _settings = settings;
            _wishlistService = wishlistService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<string> WishlistInProductDetails(int productId)
        {
            if (productId == 0)
                throw new ArgumentNullException(nameof(productId));
            var store = await _storeContext.GetCurrentStoreAsync();
            var count = new WishlistModel
            {
                Text = _settings.Text?.ToString()
            };
            var dec = 0.0;
            var cartProduct = await _wishlistService.GetShoppingCartAsync(productId, ShoppingCartType.Wishlist, store.Id);
            var createdFromUtc = DateTime.UtcNow.AddDays(-30);
            var createdToUtc = DateTime.UtcNow;
            if (_settings.Days == Days.OneMonth)
            {
                count.Days = "30 days";
                createdFromUtc = DateTime.UtcNow.AddDays(-30);

            }
            else if (_settings.Days == Days.OneWeek)
            {
                createdFromUtc = DateTime.UtcNow.AddDays(-7);
                count.Days = "7 days";

            }
            else if (_settings.Days == Days.TwoWeek)
            {
                createdFromUtc = DateTime.UtcNow.AddDays(-14);
                count.Days = "2 weeks";

            }
            else if (_settings.Days == Days.ThreeWeek)
            {
                createdFromUtc = DateTime.UtcNow.AddDays(-21);
                count.Days = "3 weeks";

            }
            cartProduct = await _wishlistService.GetShoppingCartAsync(productId, ShoppingCartType.Wishlist, store.Id, createdFromUtc, createdToUtc);

            if (cartProduct != null)
            {
                count.WishlistCounts = cartProduct.Sum(item => item).ToString();
            }
            if (Convert.ToInt32(count.WishlistCounts) > 1000000)
            {
                dec = ((double)Convert.ToInt32(count.WishlistCounts)) / (double)1000000;
                var value = string.Format("{0:0.0}", dec);
                count.WishlistCounts = value.ToString() + "M";
            }
            if (Convert.ToInt32(count.WishlistCounts) > 1000)
            {
                dec = ((double)Convert.ToInt32(count.WishlistCounts))/ (double)1000.0;
                var value = string.Format("{0:0.0}", dec);
                count.WishlistCounts = value.ToString() + "K";

            }

            var str = count.WishlistCounts +" "+ count.Text + " wishlisted in last " + count.Days;
            if(count.WishlistCounts == "0" && count.WishlistCounts == null)
            {
                str = null;
            }

            return str;
        }

        [HttpPost]
        public async Task<string?> WishlistInCategory(int productId)
        {
            if (_settings.EnableInCategory == false)
            {
                return null;
            }
            if (productId == 0)
                throw new ArgumentNullException(nameof(productId));
            var store = await _storeContext.GetCurrentStoreAsync();
            var count = new WishlistModel();           
          
            var cart = await _wishlistService.GetShoppingCartAsync(productId, ShoppingCartType.Wishlist, store.Id);
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
            return count.WishlistCounts;
        }
        #endregion
    }
}