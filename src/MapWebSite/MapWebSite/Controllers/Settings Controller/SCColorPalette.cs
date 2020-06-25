using MapWebSite.HtmlHelpers;
using MapWebSite.Domain;
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
            DomainInstance handler = new DomainInstance();
            bool success = handler.InsertColorPalette(RouteConfig.CurrentUser.Username, colorMap);
          

            var response = new HttpResponseMessage();
            response.Content = new StringContent(MessageBoxBuilder.Create(success ? TextDictionary.OverlayCPSuccesTitle
                                                                                 : TextDictionary.OverlayCPFailedTitle, 
                                                                          success ? TextDictionary.OverlayCPSuccesText
                                                                                 : TextDictionary.OverlayCPFailedText,
                                                                          success));

            //Log a warning message if insert fails
            if(!success)
              Core.CoreContainers.LogsRepository.LogWarning($"Failed to insert color palette { colorMap.Name } (main color criteria: { colorMap.MainColorCriteria }) for user {RouteConfig.CurrentUser.UserName}", Core.Database.Logs.LogTrigger.Controllers);

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}
