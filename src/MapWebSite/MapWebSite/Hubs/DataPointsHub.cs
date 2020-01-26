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
                                      dynamic[] existingRegions,
                                      string username,
                                      string datasetName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(datasetName)) return;

            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
             
            Action<IEnumerable<PointBase>, Tuple<string,int>, bool> callback = (pointsData, regionData, filled) =>
            {
                if(pointsData != null)
                    Clients.Caller.ProcessPoints(pointsData.DataContractJSONSerialize());
                if(regionData != null)
                    Clients.Caller.UpdateRegionsData( new
                    {
                        Region = regionData.Item1,
                        PointsCount = regionData.Item2,
                        Filled = filled
                    }.JSONSerialize());
            };

            Dictionary<string, int> existingRegionsDictionary = new Dictionary<string, int>();

            foreach (var region in existingRegions)
                existingRegionsDictionary.Add((string)region.RegionKey.Value,
                                              (int)region.RegionPointsCount.Value);
             

            databaseInteractionHandler.RequestPoints(new Tuple<decimal, decimal>(latitudeFrom, longitudeFrom),
                                      new Tuple<decimal, decimal>(latitudeTo, longitudeTo),
                                      username,
                                      datasetName, 
                                       existingRegionsDictionary, 
                                      callback);

            Clients.Caller.PointsProcessedNotification();
        }

    }
}