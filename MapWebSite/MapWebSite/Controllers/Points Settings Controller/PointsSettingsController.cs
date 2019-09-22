using MapWebSite.Core;
using MapWebSite.Interaction;
using MapWebSite.Interaction.ViewModel;
using MapWebSite.Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace MapWebSite.Controllers
{

    /// <summary>
    /// Use this ApiController to return pages for the points settings layer and to interact with it
    /// </summary>
    [Filters.SiteAuthenticationFilter]
    public class PointsSettingsController : Controller
    {
        [System.Web.Mvc.HttpGet]
        public ActionResult GetColorPalettePage()
        {
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();

            return View("~/Views/Home/Points Settings Content/ChosePalette.cshtml",
                new ChosePaletteViewModel(databaseInteractionHandler.GetColorPaletes(
                   Core.Database.ColorMapFilters.None,
                   string.Empty,
                   0,
                   ChosePaletteViewModel.ColorPalettesPerPage
                    )));
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult GetChoseDatasetPage()
        {
            return View("~/Views/Home/Points Settings Content/ChoseDataset.cshtml");
        }
    }

    [Filters.ApiAuthenticationFilter]
    public class PointsSettingsApiController : ApiController
    {
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetColorPalette(string username, string paletteName)
        {
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
            string serializedColorPalete = databaseInteractionHandler.GetColorPaletteSerialization(username, paletteName);

            var response = new HttpResponseMessage();
            response.Content = new StringContent(serializedColorPalete);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return response;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetColorPaletteList(string filterValue, Core.Database.ColorMapFilters filter, int pageIndex)
        {
            var databaseInteractionHandler = new DatabaseInteractionHandler();
            var response = new HttpResponseMessage();
            response.Content = new StringContent(
                databaseInteractionHandler.GetColorPaletes(
                    filter,
                    filterValue ?? string.Empty,
                    pageIndex,
                    ChosePaletteViewModel.ColorPalettesPerPage
                )?.JSONSerialize());
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return response;
        }
    }


}