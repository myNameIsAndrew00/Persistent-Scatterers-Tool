using MapWebSite.Core;
using MapWebSite.Interaction;
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
        public void RequestDataPoints(decimal latitudeFrom, decimal longitudeFrom, decimal latitudeTo, decimal longitudeTo, decimal zoomLevel, string optionalField)
        {
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
             
            Action<IEnumerable<BasicPoint>> callback = (pointsData) =>
            {
                Clients.Caller.ProcessPoints(pointsData.DataContractJSONSerialize());
            };

            databaseInteractionHandler.RequestPoints(new Tuple<decimal, decimal>(latitudeFrom, longitudeFrom),
                                      new Tuple<decimal, decimal>(latitudeTo, longitudeTo),
                                      "woofwoof", //this will be changed and customized for current user
                                      "mainTest", //this will be changed and customized for current user
                                      0,
                                      (BasicPoint.BasicInfoOptionalField)Enum.Parse(typeof(BasicPoint.BasicInfoOptionalField), optionalField),
                                      callback);

        
        }

    }
}