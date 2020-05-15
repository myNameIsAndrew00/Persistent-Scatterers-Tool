using MapWebSite.Domain;
using MapWebSite.Types;
using MapWebSite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace MapWebSite.Controllers
{
    [Authorize]
    [Authorize(Roles = "Administrator")]
    public partial class SettingsController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetDatasets(int pageIndex, int itemsPerPage)
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            var datasets = handler.GetDataSets(string.Empty, true, null, pageIndex, itemsPerPage)?.Select(dataset => new
            {
                dataset.Name,
                dataset.ID,
                dataset.Username,
                Status = dataset.Status.GetEnumString(),
                dataset.IsValid,
                Source = dataset.PointsSource.ToString()
            }); 

            var response = new HttpResponseMessage();
            response.Content = new StringContent(datasets.JSONSerialize());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }
    }
}