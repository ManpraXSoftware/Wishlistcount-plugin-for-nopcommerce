using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.Wishlist.Models
{
    public record WishlistModel : BaseNopModel
    {
        public int ProductId { get;set; } 

        public string? WishlistCounts { get; set; }

        public string? Text { get; set; }

        public string? Days { get; set; }

        public bool EnableInCategory { get; set; }
    }
}
