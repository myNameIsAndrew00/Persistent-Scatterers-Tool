/*! Component: Map
 *
 * This map contains the logic used for map navigation and interaction with Map Server
 *
 * */

import { PointsSectionsContainer, ChangePointsSource, CurrentSource } from './chose_points_source.js';

var selectedPoints = [];
var maxExtent = [17.5, 42.0, 32.8, 49.0];
ol.IMAGE_RELOAD_ATTEMPTS = 5;

export var ProcessingEnabled = 0;

/*workers*/
//var receivedPointsWorker = new Worker('script.js', { type: "module" });

export function PointsProcessedNotificationHandler() {
    ProcessingEnabled = Math.max(0, ProcessingEnabled - 1);
    if (ProcessingEnabled == 0)
        DisplayProcessing(false);
}

export function DisplayProcessing(display) {
    if (display) ProcessingEnabled++;

    if (display)
        $('#hub-loading-spinner').removeClass('small-loading-spinner-hidden');
    else $('#hub-loading-spinner').addClass('small-loading-spinner-hidden');
}


/**
 * View for map
 */

/*map constants*/ 
const mapParameters = {
    center: ol.proj.fromLonLat([28.652880, 44.177269], 'EPSG:3857'),
    minZoom: 3,
    maxZoom: 20,
    initialZoom: 10
}

var mapView = new ol.View({
    center: mapParameters.center,
    extent: ol.proj.transformExtent(maxExtent, 'EPSG:4326', 'EPSG:3857'),
    zoom: mapParameters.initialZoom,
    minZoom: mapParameters.minZoom,
    maxZoom: mapParameters.maxZoom
})




/**
 * Here the map is rendered 
 */

//this variable contains the source tiles available to be displayed
//Types chosen for map must be key names in this object
const sources = {
    satelitte: new ol.layer.Tile({
        visible: true,
        source: new ol.source.TileJSON({
            url: 'https://api.maptiler.com/maps/hybrid/256/tiles.json?key=UKuFFRYp8bMMxfqZFhKJ',
            tileSize: 256,
            crossOrigin: 'anonymous',
            wrapX: false,
            noWrap: true

        })
    }),
    hybrid: new ol.layer.Tile({
        visible: false, 
        source: new ol.source.OSM({
            crossOrigin: 'anonymous',
            wrapX: false,
            noWrap: true,      
        })
    }),
    google: new ol.layer.Tile({
        visible: false,
        source: new ol.source.OSM({
            crossOrigin: 'anonymous',
            wrapX: false,
            noWrap: true,
        })
    }),
}

export function SetMapType(chosenType) {
    Object.keys(sources).forEach(function (type, typeIndex) {
        sources[type].setVisible(type == chosenType);
    });
} 

export function GoTo(latitude, longitude) {
    map.getView().animate({
        center: new ol.proj.fromLonLat([longitude, latitude], 'EPSG:3857'),
        duration: 1000,
        zoom: mapParameters.maxZoom - 2
    });

}

export function ZoomOut() {
    map.getView().animate({
        center: mapParameters.center,
        zoom: mapParameters.minZoom,
        duration: 1000
    })
}

export const map = new ol.Map({
    target: 'map',
    renderer: 'webgl', 
    layers: Object.values(sources),
    view: mapView,
    controls: []
});



map.on('moveend', initialisePointsRequest); 

export function UnselectFeatureOnMap(featureId) {
    //if (selectedPoints[featureId] === undefined) return;

    //vectorSource.removeFeature(selectedPoints[featureId]);
    //delete selectedPoints[featureId];

}


/**
 * Section below contain the points request handling
 */

export function UpdatePointsLayer(points) {
    PointsSectionsContainer.UpdatePointsLayer(points);
}

function initialisePointsRequest(evt) {
    //code bellow handles zoomin-out
    /*var newZoom = map.getView().getZoom();
    if (currentZoom != newZoom) {
        ClearMap();
        console.log('zoom end, new zoom: ' + newZoom);
        currentZoom = newZoom;
    }*/

    PointsSectionsContainer.LoadPoints();
}

//initialise map interactions


ChangePointsSource(null, CurrentSource);

/**********************************************/
/*this section contains context menu functions*/