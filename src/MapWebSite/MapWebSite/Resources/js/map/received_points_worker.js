//import { HubRouter } from '/Resources/js/api/hub_router.js';
//var hubRouter = null;
//self.importScripts('/Resources/js/api/hub_router.js');

self.addEventListener('message', function (e) {
    //hubRouter = hub.data;
    //console.log(e.data);
    self.postMessage('done');
    //hubRouter.SetCallback('ProcessPoints', processPoints);
});



//import { hubRouter } from '../map.js';
//import { UpdatePointsLayer } from '../map.js';

//alert('loaded');
 /*
async function processPoints(receivedInfo) {
    var points = [];
    var index = 0;

    var requestedPoints = JSON.parse(receivedInfo);

    //TODO: process the input. It must not be already displayed on map. 
    //Use the sessionStorage to find which points are already displayed.

    console.log('Hub result: ' + requestedPoints.length);

    for (var i = 0; i < requestedPoints.length; i++) {
        points[i + index] = new ol.Feature({
            'geometry': new ol.geom.Point(
                ol.proj.fromLonLat([requestedPoints[i].Longitude, requestedPoints[i].Latitude], 'EPSG:3857')),
            'colorCriteria': requestedPoints[i].OptionalField
        });
        points[i + index].ID = requestedPoints[i].Number;
        points[i + index].longitude = requestedPoints[i].Longitude;
        points[i + index].latitude = requestedPoints[i].Latitude;
    }

    requestedPoints.splice(0, requestedPoints.length);

    self.postMessage(points);
}
*/