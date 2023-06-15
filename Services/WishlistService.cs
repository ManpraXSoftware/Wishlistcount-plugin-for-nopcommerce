using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Data;

namespace Nop.Plugin.Widgets.Wishlist.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IRepository<ShoppingCartItem> _shoppingCartRepository;

        public WishlistService(IRepository<ShoppingCartItem> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        public virtual async Task<IList<int>> GetShoppingCartAsync(int productId, ShoppingCartType? shoppingCartType = null, int storeId = 0, DateTime? createdFromUtc = null, DateTime? createdToUtc = null)
        {
            if (productId == 0)
                return new List<int>();

            var query = from c in _shoppingCartRepository.Table 
                        where c.ProductId == productId && c.StoreId ==storeId && c.ShoppingCartTypeId == Convert.ToInt32(ShoppingCartType.Wishlist)
                        select c.Quantity;
            if(createdFromUtc!= null && createdToUtc != null)
            {
                query = from c in _shoppingCartRepository.Table
                        where c.ProductId == productId && c.StoreId == storeId && c.ShoppingCartTypeId == Convert.ToInt32(ShoppingCartType.Wishlist)
                        && (c.CreatedOnUtc >= createdFromUtc && c.CreatedOnUtc <= createdToUtc)
                        select c.Quantity;
            }

            var cart = await query.ToListAsync();

            return cart;
        }
    }
}
