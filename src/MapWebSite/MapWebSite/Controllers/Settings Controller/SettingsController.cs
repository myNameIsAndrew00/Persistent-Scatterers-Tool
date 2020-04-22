using MapWebSite.HtmlHelpers;
using MapWebSite.Domain;
using MapWebSite.Model; 
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http; 
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace MapWebSite.Controllers
{
    /// <summary>
    /// Use this ApiController to return pages for the settings layer and to interact with it
    /// </summary> 
    [Authorize]
    public partial class SettingsController : ApiController
    { 
        /// <summary>
        /// Handle the setting pages names
        /// </summary>
        public enum Page
        {
            About,
            Account,
            ColorPicker,
            UploadPoints,
            UseGeoserverLayer,
            ManageUsersDatapoints
        }
    }
}
