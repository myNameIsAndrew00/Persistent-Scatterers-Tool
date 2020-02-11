/*! Component: Map
 *
 * This map contains the logic used for map navigation and interaction with Map Server
 *
 * */

import { DisplayPointInfo, SetPointInfoData } from '../point info/point_info.js';
import { Router, endpoints } from '../api/api_router.js';
import { SelectedDataset } from '../points settings/chose_dataset.js';
import { MapType } from './chose_map_type.js';
import { PointsSectionsContainer } from './points_section_handler.js';


var selectedPoints = [];
export var ProcessingEnabled = 0;

/*workers*/
//var receivedPointsWorker = new Worker('script.js', { type: "module" });

export function PointsProcessedNotificationHandler() {
    ProcessingEnabled--;
    if (ProcessingEnabled == 0)
        DisplayHubProcessing(false);
}

export function DisplayHubProcessing(display) {
    if (display) ProcessingEnabled++;

    if (display)
        $('#hub-loading-spinner').removeClass('small-loading-spinner-hidden');
    else $('#hub-loading-spinner').addClass('small-loading-spinner-hidden');
}

/**
 * View for map
 */
var mapView = new ol.View({
    center: ol.proj.fromLonLat([28.652880, 44.177269], 'EPSG:3857'),
    zoom: 10,
    minZoom: 3,
    maxZoom: 20
})




/**
 * Here the map is rendered 
 */

//this variable contains the source tiles available to be displayed
const sources = {
    hybrid: new ol.layer.Tile({
        visible: true, 
        source: new ol.source.OSM({
            crossOrigin: 'anonymous',
            wrapX: false,
            noWrap: true,      
        })
    }),
    satellite: new ol.layer.Tile({
        visible: false, 
        source: new ol.source.TileJSON({
            url: 'https://api.maptiler.com/maps/hybrid/256/tiles.json?key=UKuFFRYp8bMMxfqZFhKJ',
            tileSize: 256,
            crossOrigin: 'anonymous',
            wrapX: false,
            noWrap: true
        
        })
    })
}

export function SetMapType(chosenType) {
    Object.keys(sources).forEach(function (type, typeIndex) {
        sources[type].setVisible(type == chosenType);
    });
}


export const map = new ol.Map({
    target: 'map',
    renderer: 'webgl', 
    layers: [
        sources['satellite'],
        sources['hybrid']
    ],
    view: mapView,
    controls: []
});

map.on('moveend', initialisePointsRequest);
map.on('click', function (evt) {
    if (map.getView().getInteracting()) {
        return;
    }
    var pixel = evt.pixel;

    map.forEachFeatureAtPixel(pixel, function (feature) {
        if (selectedPoints[feature.ID] === undefined) {
            handleClickFunction(feature);
        }
    });
});





/*follow section handle the click on items*/
/**
 * This functions handle the click on point. Parameter represents the feature
 */

function handleClickFunction(point) {

    /*
    function selectFeatureOnMap() {
        var selectFeatureId = point.ID;

        var feature = new ol.Feature({
            'geometry': new ol.geom.Point(
                ol.proj.fromLonLat([point.longitude, point.latitude], 'EPSG:3857')),
        });
        feature.setId(point.ID + 'selected');
        feature.ID = point.ID + 'selected';
        feature.longitude = point.longitude;
        feature.latitude = point.latitude;
        feature.color = { r: 0, g: 0, b: 255 }
        feature.size = 3;

        selectedPoints[selectFeatureId] = feature;

        vectorSource.addFeature(feature);
    }*/

    Router.Get(endpoints.Home.RequestPointDetails,
        {
            zoomLevel: map.getView().getZoom(),
            latitude: point.latitude,
            longitude: point.longitude,
            identifier: point.ID,
            username: SelectedDataset.username,
            datasetName: SelectedDataset.datasetName
        }, function (receivedInfo) {
            SetPointInfoData(receivedInfo);
        }
    )


    DisplayPointInfo();

}

export function UnselectFeatureOnMap(featureId) {
    //if (selectedPoints[featureId] === undefined) return;

    //vectorSource.removeFeature(selectedPoints[featureId]);
    //delete selectedPoints[featureId];

}


/**
 * Section below contain the points request handling
 */
export function UpdatePointsLayer(points) {
    pointsSectionsContainer.UpdatePointsLayer(points);

}

function initialisePointsRequest(evt) {
    //code bellow handles zoomin-out
    /*var newZoom = map.getView().getZoom();
    if (currentZoom != newZoom) {
        ClearMap();
        console.log('zoom end, new zoom: ' + newZoom);
        currentZoom = newZoom;
    }*/


    pointsSectionsContainer.LoadPoints();
}

//initialise map interactions


var pointsSectionsContainer = new PointsSectionsContainer(map);

/**********************************************/
/*this section contains context menu functions*/