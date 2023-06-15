using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.Wishlist.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        public int DaysId { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.Wishlist.Fields.Days")]
        public SelectList? DaysValues { get; set; }
        public bool DaysId_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Plugins.Widgets.Wishlist.Fields.Text")]
        public string? Text { get; set; }
        public bool Text_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Wishlist.Fields.EnableInCategory")]
        public bool EnableInCategory { get; set; }
        public bool EnableInCategory_OverrideForStore { get; set; }
    }
}