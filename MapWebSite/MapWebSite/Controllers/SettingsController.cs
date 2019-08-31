using MapWebSite.HtmlHelpers;
using MapWebSite.Interaction;
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
    [Filters.ApiAuthenticationFilter]
    public class SettingsController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage SaveColorsPalette(ColorMap colorMap)
        {
            //TODO: save the color map to the database
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            bool result = handler.InsertColorPalette(User.Identity.Name, colorMap);
          
            var response = new HttpResponseMessage();
            response.Content = new StringContent(MessageBoxBuilder.Create(result ? "Success" : "Failed", result ? "You succesfully upload your color palette"
                                                                                                : "Something went wrong. Try to change your palette name or check the connection"));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}
