using MapWebSite.Core;
using MapWebSite.Domain.ViewModel;
using System.Web.Mvc; 
using System.Net.Http;
using MapWebSite.Types;

namespace MapWebSite.Controllers
{
    /// <summary>
    /// This controller provides general purpose pages (or other responses)
    /// </summary>
    [System.Web.Mvc.Authorize] 
    public class MiscellaneousController : Controller
    {
        /// <summary>
        /// Handles the tooltips available in the application. Each enum value is decorated with strings which can provide resources
        /// </summary>
        public enum Tooltip
        {             
            [EnumString("HTM_TOOLTIP_MapType")]
            ChoseMapTypeTooltip,
            [EnumString("HTM_TOOLTIP_PointsSize")]
            ChosePointsSizeTooltip,
            [EnumString("HTM_TOOLTIP_ColorCriteria")]
            ChoseColorCriteriumTooltip,
            [EnumString("HTM_TOOLTIP_Search")]
            SearchTooltip,
            [EnumString("HTM_TOOLTIP_PointsSource")]
            PointsSource
        }

        [HttpGet]
        public ActionResult GetNotificationsPage()
        {
            return View("~/Views/Home/Miscellaneous Content/Notifications.cshtml");
        }

        [HttpGet]
        public ActionResult GetChoseMapTypePage()
        {
            return View("~/Views/Home/Miscellaneous Content/ChoseMapType.cshtml");
        }

        [HttpGet]
        public ActionResult GetChangePointsSizePage()
        {
            return View("~/Views/Home/Miscellaneous Content/ChangePointsSize.cshtml");
        }

        public ActionResult GetChosePointsSourcePage()
        {
            return View("~/Views/Home/Miscellaneous Content/ChosePointsSource.cshtml");
        }

        /// <summary>
        /// Return a view containing a tooltip
        /// </summary>
        /// <param name="tooltip"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetTooltip(Tooltip tooltip)
        {
            return PartialView("~/Views/Home/Miscellaneous Content/Tooltips/TutorialTooltip.cshtml",
                new TutorialTooltipViewModel()
                {
                    DisplayedMessage = 
                        Resources.text.TextDictionary.ResourceManager.GetString(
                            tooltip.GetEnumString(),
                            Resources.text.TextDictionary.Culture),
                    GifPath =
                        $"Resources/resources/gifs/{tooltip.GetEnumString()}.gif"
                }
                );
        }
    }

     
}