using MapWebSite.HtmlHelpers;
using MapWebSite.Domain;
using MapWebSite.Model;
 using MapWebSite.Core;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using MapWebSite.Types;
using System.Net.Mime;
using System.Linq;

namespace MapWebSite.Controllers
{
    /// <summary>
    /// Use this ApiController to return pages for the settings layer and to interact with it
    /// </summary> 
    [Authorize]
    [Authorize(Roles = "Administrator")]
    public partial class SettingsController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetUsers(int pageIndex, int itemsPerPage)
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            IEnumerable<User> users = handler.GetUsers(null, pageIndex, itemsPerPage);
            
            var response = new HttpResponseMessage();
            response.Content = new StringContent(users.JSONSerialize());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }

        [HttpGet]
        public HttpResponseMessage GetUserDatasets(string username, int pageIndex, int itemsPerPage)
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            var datasets = handler.GetDataSets(username, null, pageIndex, itemsPerPage)?.Select(dataset => new
            {
                dataset.Name,
                dataset.ID,
                dataset.Username,
                Status = dataset.Status.GetEnumString(),
                dataset.IsValid
            }); ;

            var response = new HttpResponseMessage();
            response.Content = new StringContent(datasets.JSONSerialize());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }


    }
}
