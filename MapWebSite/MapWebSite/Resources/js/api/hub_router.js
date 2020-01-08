/*! MODULE: HubRouter
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
        this.connection = $.hubConnection();
        this.hubProxy = this.connection.createHubProxy('DataPointsHub');

        this.connection.start().done(() => {    
            this.enabled = true;
        });

    }

    SetCallback(callbackName, callback) {
        this.hubProxy.on(callbackName, callback);        
    }

    RequestDataPoints(latitudeFrom,
        longitudeFrom,
        latitudeTo,
        longitudeTo,
        existingRegions,
        optionalField,
        username,
        datasetName) {

        if (username == null || datasetName == null) return;
        if (!this.enabled) return;

        this.hubProxy.invoke('RequestDataPoints',
            latitudeFrom,
            longitudeFrom,
            latitudeTo,
            longitudeTo,
            existingRegions,
            optionalField,
            username,
            datasetName);
    }

}

export { HubRouter };