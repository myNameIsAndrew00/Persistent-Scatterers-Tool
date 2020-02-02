using MapWebSite.Core;
using MapWebSite.Domain;
using MapWebSite.Model;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web;

namespace MapWebSite.Hubs
{
    [Authorize]
    public class DataPointsHub : Hub
    {
        public void RequestDataPoints(decimal latitudeFrom,
                                      decimal longitudeFrom,
                                      decimal latitudeTo,
                                      decimal longitudeTo,
                                      int zoomLevel,
                                      string[] cachedRegions,
                                      string username,
                                      string datasetName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(datasetName)) return;

            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();

            Action<IEnumerable<PointBase>, string> callback = (pointsData, regionKey) =>
             {
                 Clients.Caller.ProcessPoints(new
                 {
                     pointsData,
                     regionKey
                 }.JSONSerialize());
                  
             };


            databaseInteractionHandler.RequestPointsRegions(new Tuple<decimal, decimal>(latitudeFrom, longitudeFrom),
                                      new Tuple<decimal, decimal>(latitudeTo, longitudeTo),
                                      zoomLevel,
                                      username,
                                      datasetName,
                                      cachedRegions,
                                      callback);

            Clients.Caller.PointsProcessedNotification();
        }

      
    }
}