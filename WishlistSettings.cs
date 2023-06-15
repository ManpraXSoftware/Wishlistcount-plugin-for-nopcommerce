using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Wishlist
{
    public class WishlistSettings : ISettings
    {
        /// <summary>
        /// Gets or sets time period -
        /// </summary>
        public Days Days { get; set; }

        /// <summary>
        /// Gets or sets text visible 
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this feature is enabled in category or not
        /// </summary>
        public bool EnableInCategory { get; set; }
    }
    
}
