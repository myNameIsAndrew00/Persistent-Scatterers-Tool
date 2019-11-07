using MapWebSite.HtmlHelpers;
using MapWebSite.Interaction;
using MapWebSite.Model; 
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using MapWebSite.Resources.text;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace MapWebSite.Controllers
{
    /// <summary>
    /// Use this ApiController to return pages for the settings layer and to interact with it
    /// </summary> 
    [Authorize]
    public partial class SettingsController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage SaveColorsPalette(ColorMap colorMap)
        { 
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            bool result = handler.InsertColorPalette(RouteConfig.CurrentUser.Username, colorMap);
          


            var response = new HttpResponseMessage();
            response.Content = new StringContent(MessageBoxBuilder.Create(result ? TextDictionary.OverlayCPSuccesTitle
                                                                                 : TextDictionary.OverlayCPFailedTitle, 
                                                                          result ? TextDictionary.OverlayCPSuccesText
                                                                                 : TextDictionary.OverlayCPFailedText));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}
