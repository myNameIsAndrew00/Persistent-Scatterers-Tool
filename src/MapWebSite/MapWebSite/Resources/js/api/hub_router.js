﻿/*! MODULE: HubRouter
 * 
 * This module is responsable for hub requests handling.
 * Do not use the hub without using this component
 *
 * */



class HubRouter {
    connection = null;
    hubProxy = null;
    enabled = false;
    callbacks = {
        ProcessPoints: 'ProcessPoints'
    }

    constructor() {
        this.enabled = false;
        this.connection = $.hubConnection();

      
        this.hubProxy = this.connection.createHubProxy('DataPointsHub');   

        this.SetCallback('Ping', function () { console.log('Connected with success.') });

      
    }

    EnableConnection() {
        var self = this;

        this.connection.start().done(() => {
            self.hubProxy.invoke('Ping');
            self.enabled = true;
        });
    }

    SetCallback(callbackName, callback) {
        this.hubProxy.on(callbackName, callback);        
    }

    RequestDataPoints(latitudeFrom,
        longitudeFrom,
        latitudeTo,
        longitudeTo,
        zoomLevel,
        cachedRegions,
        username,
        datasetName) {

        if (username == null || datasetName == null) return;
        if (!this.enabled) return;

        console.log('invoking hub method for ' + username + ':' + datasetName);

        this.hubProxy.invoke('RequestDataPoints',
            latitudeFrom,
            longitudeFrom,
            latitudeTo,
            longitudeTo,
            zoomLevel,
            cachedRegions,
            username,
            datasetName);
    }

    GetRegionKeys( latitudeFrom,
         longitudeFrom,
         latitudeTo,
         longitudeTo,
         zoomLevel,
         username,
         datasetName) {


        if (username == null || datasetName == null) return;
        if (!this.enabled) return;        

        this.hubProxy.invoke('GetRegionKeys',
            latitudeFrom,
            longitudeFrom,
            latitudeTo,
            longitudeTo,
            zoomLevel, 
            username,
            datasetName);

    }
}

export var HubRouterInstance = new HubRouter();
 