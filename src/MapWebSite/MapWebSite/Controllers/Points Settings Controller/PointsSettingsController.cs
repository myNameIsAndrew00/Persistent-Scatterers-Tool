using MapWebSite.Authentication;
using MapWebSite.Core;
using MapWebSite.Domain;
using MapWebSite.Domain.ViewModel;
using MapWebSite.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MapWebSite.Types;
using MapWebSite.Core.Database;

namespace MapWebSite.Controllers
{

    /// <summary>
    /// Use this ApiController to return pages for the points settings layer and to interact with it
    /// </summary> 
    [System.Web.Mvc.Authorize]
    public class PointsSettingsController : Controller
    {
        [System.Web.Mvc.HttpGet]
        public ActionResult GetColorPalettePage()
        {
            try
            {
                DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();

                return View("~/Views/Home/Points Settings Content/ChosePalette.cshtml",
                    new ChosePaletteViewModel(databaseInteractionHandler.GetColorPaletes(
                        new List<Tuple<Core.Database.ColorMapFilters, string>>()
                            {
                                new Tuple<Core.Database.ColorMapFilters, string>(Core.Database.ColorMapFilters.None, string.Empty)
                            },
                           0,
                           ChosePaletteViewModel.ColorPalettesPerPage
                        )));
            }
            catch
            {
                return View();
            }
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult GetChoseDatasetPage()
        {
            try
            {
                DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
                return View("~/Views/Home/Points Settings Content/ChoseDataset.cshtml",
                    new ChoseDatasetViewModel(databaseInteractionHandler.GetDataSets(
                        RouteConfig.CurrentUser?.Username,
                        false,
                        new List<Tuple<DataSetsFilters, string>>()
                        {
                            new Tuple<DataSetsFilters, string>(
                                   DataSetsFilters.None,
                                   string.Empty)
                        },
                        ChoseDatasetViewModel.DataPointsPerPage
                        )));
            }
            catch (Exception e)
            {
                return View();
            }
        }

        public ActionResult GetChoseDisplayCriteriaPage()
        {
            List<string> model = new List<string>();

            foreach (PointBase.VisualisationCriteria suit in (PointBase.VisualisationCriteria[])Enum.GetValues(typeof(PointBase.VisualisationCriteria)))
                model.Add(suit.GetEnumString());

            return View("~/Views/Home/Points Settings Content/ChoseCriteria.cshtml",
                            model);
        }



    }

    /// <summary>
    /// Represents the api used for points settings layer
    /// </summary>
    [System.Web.Http.Authorize]
    public class PointsSettingsApiController : ApiController
    {

        [System.Web.Http.HttpGet]
        public HttpResponseMessage ValidateGeoserverStyle(string datasetName, string datasetUsername, string paletteName, string paletteUsername)
        {
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();

            bool validationResult = databaseInteractionHandler.ValidateOrSetPaletteToGeoserverLayer(datasetName, datasetUsername, paletteName, paletteUsername);

            var response = new HttpResponseMessage();
            response.StatusCode = validationResult ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Conflict;

            return response;
        }

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
        public HttpResponseMessage GetColorPaletteList(int pageIndex)
        {
            var databaseInteractionHandler = new DatabaseInteractionHandler();
            var response = new HttpResponseMessage();


            List<Tuple<ColorMapFilters, string>> filters = buildFilters<ColorMapFilters>(Request.GetQueryNameValuePairs());

            response.Content = new StringContent(
                databaseInteractionHandler.GetColorPaletes(
                    filters,
                    pageIndex,
                    ChosePaletteViewModel.ColorPalettesPerPage
                )?.JSONSerialize());
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return response;
        }

        public HttpResponseMessage GetDatasetsList(int pageIndex, int itemsPerPage = -1)
        {
            if (itemsPerPage == -1) itemsPerPage = ChoseDatasetViewModel.DataPointsPerPage;

            var databaseInteractionHandler = new DatabaseInteractionHandler();
            var response = new HttpResponseMessage();

            bool isAnonymous = User.IsInRole(UserRoles.Anonymous.GetEnumString());

            List<Tuple<DataSetsFilters, string>> filters = buildFilters<DataSetsFilters>(Request.GetQueryNameValuePairs());

            //if is anonymous, only source filter should be considered
            if (isAnonymous)
            {
                filters = filters.Where(item => item.Item1 == DataSetsFilters.Source).ToList();
                filters.Add(new Tuple<DataSetsFilters, string>(DataSetsFilters.IsDemo, "1")); 
            }

            var model = databaseInteractionHandler.GetDataSets(
                    RouteConfig.CurrentUser.Username,
                    isAnonymous,
                    filters,
                    pageIndex,
                    itemsPerPage
                )?.Select(dataset => new
                {
                    dataset.Name,
                    dataset.ID,
                    dataset.Username,
                    Status = dataset.Status.GetEnumString(),
                    dataset.IsValid
                });

            response.Content = new StringContent(model?.JSONSerialize());
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return response;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetDatasetLimits(string username, string datasetName)
        {
            var databaseInteractionHandler = new DatabaseInteractionHandler();
            var response = new HttpResponseMessage();


            response.Content = new StringContent(databaseInteractionHandler.GetDataSet(username, datasetName).JSONSerialize());
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return response;
        }


        #region Private


        /// <summary>
        /// Build filters object using a query string.
        /// </summary>
        /// <typeparam name="T">Type of enum used for filtering</typeparam>
        /// <param name="query">A key-value list container. Required keys are: filtersCount, filter{i}, filterValue{i}, where i is an index between 0 and filtersCount.        
        /// </param>
        /// <returns></returns>
        private List<Tuple<T, string>> buildFilters<T>(IEnumerable<KeyValuePair<string, string>> query) where T : Enum
        {
            List<Tuple<T, string>> filters = new List<Tuple<T, string>>();

            if (query.Where(a => a.Key == "filtersCount").FirstOrDefault().Key != "filtersCount") return filters; 

            int filtersCount = Convert.ToInt32(query.First(item => item.Key == "filtersCount").Value);

            for (int i = 0; i < filtersCount; i++)
            {
                try
                {
                    filters.Add(new Tuple<T, string>(
                         (T)Enum.Parse(typeof(T), query.First(item => item.Key == $"filter{i}").Value, true),
                         query.First(item => item.Key == $"filterValue{i}").Value
                        ));
                }
                catch { }
            }

            return filters;
        }


        #endregion
    }


}