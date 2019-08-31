using MapWebSite.Interaction;
using MapWebSite.Interaction.ViewModel;
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
                new ChosePaletteModel(databaseInteractionHandler.GetColorPaletes(
                   Core.Database.ColorMapFilters.None,
                   string.Empty
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
    }


}