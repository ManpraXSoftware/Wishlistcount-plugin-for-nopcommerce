using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Widgets.Wishlist.Services
{
    public interface IWishlistService
    {
        Task<IList<int>> GetShoppingCartAsync(int productId, ShoppingCartType? shoppingCartType = null,
           int storeId = 0, DateTime? createdFromUtc = null, DateTime? createdToUtc = null);
    }
}
