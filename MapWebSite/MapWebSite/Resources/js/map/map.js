/*! Component: Map
 *
 * This map contains the logic used for map navigation and interaction with Map Server
 *
 * */

import { colorPalette } from '../home.js';
import { DisplayPointInfo, SetPointInfoData } from '../point info/point_info.js';
import { Router, endpoints } from '../api/api_router.js';
import { HubRouter } from '../api/hub_router.js';
import { PointsLayer } from './points_layer.js';
import { PointsRegionsManager } from '../api/cache/points_regions_manager.js';
import { SelectedDataset } from '../points settings/chose_dataset.js';

//var SelectedDataset;
//var points = [];
var vector = null;
var vectorSource = null;
var pointsRegionsManager = new PointsRegionsManager();
var processingEnabled = 0;

var selectedPoints = [];
/*workers*/
//var receivedPointsWorker = new Worker('script.js', { type: "module" });

/*initialise the router*/
export const hubRouter = new HubRouter();

hubRouter.SetCallback('UpdateRegionsData', function (receivedInfo) {
    var regionData = JSON.parse(receivedInfo);
    pointsRegionsManager.AddRegion(regionData.Region, regionData.PointsCount, regionData.Filled);
});

hubRouter.SetCallback('ProcessPoints', async function (receivedInfo) {
    var points = [];
    var index = 0;

    var requestedPoints = JSON.parse(receivedInfo);
    console.log('Hub result: ' + requestedPoints.length);

    for (var i = 0; i < requestedPoints.length; i++) {
        points[i + index] = new ol.Feature({
            'geometry': new ol.geom.Point(
                ol.proj.fromLonLat([requestedPoints[i].Longitude, requestedPoints[i].Latitude], 'EPSG:3857')),
            'colorCriteria': requestedPoints[i].Height
        });
        points[i + index].setId(requestedPoints[i].Number);
        points[i + index].ID = requestedPoints[i].Number;
        points[i + index].longitude = requestedPoints[i].Longitude;
        points[i + index].latitude = requestedPoints[i].Latitude;
        points[i + index].color = buildStyleFromPalette(requestedPoints[i].Height);
    }

    requestedPoints.splice(0, requestedPoints.length);


    UpdatePointsLayer(points);
});

hubRouter.SetCallback('PointsProcessedNotification', function () {
    processingEnabled--;
    if (processingEnabled == 0)
        displayHubProcessing(false);
})

function displayHubProcessing(display) {
    if (display)
        $('#hub-loading-spinner').removeClass('small-loading-spinner-hidden');
    else $('#hub-loading-spinner').addClass('small-loading-spinner-hidden');
}

/**
 * View for map
 */
var mapView = new ol.View({
    center: ol.proj.fromLonLat([28.652880, 44.177269], 'EPSG:3857'),
    zoom: 16,
    minZoom: 3,
    maxZoom: 20
})

 


/**
 * Here the map is rendered 
 */

export const map = new ol.Map({
    target: 'map',
    renderer: 'webgl',
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM(
                {
                    /*Uncomment this to change the map*/
                    /*"url": 'https://{1-4}.aerial.maps.cit.api.here.com' +
                        '/maptile/2.1/maptile/newest/satellite.day/{z}/{x}/{y}/256/png' +
                        '?app_id=oYZx6OXtO1hWKT2ztoeb&app_code=D7b7B1XOmHpFzAFWaaejIRrVqzdDjsJxZYUf_S0mzVA'*/
                }
            )
        })
    ],
    view: mapView,
    controls: []
});



function binarySearch(value, left, right) {
    if (left == right) return colorPalette[right].Color;

    if (left == right - 1) {
        if (value < colorPalette[right].Left)
            return colorPalette[left].Color;
        return colorPalette[right].Color;
    }

    var middle = Math.floor((right + left) / 2);

    if (value < colorPalette[middle].Left)
        return binarySearch(value, left, middle);
    if (value > colorPalette[middle].Right)
        return binarySearch(value, middle, right);

    return colorPalette[middle].Color;
}


function buildStyleFromPalette(featureValue) {

    function hexToRgb(hex) {
        var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
        return result ? {
            r: parseInt(result[1], 16),
            g: parseInt(result[2], 16),
            b: parseInt(result[3], 16)
        } : null;
    }

    var rangeMin = -2;
    var rangeMax = 80;

    var paletteColor = binarySearch(
        ((featureValue + Math.abs(rangeMin)) * 100) / (rangeMax + Math.abs(rangeMin)),
        0,
        colorPalette.length - 1
    );

    return hexToRgb(paletteColor);
}




/*follow section handle the click on items*/
/**
 * This functions handle the click on point. Parameter represents the feature
 */

function handleClickFunction(point) {


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
    }

    Router.Get(endpoints.Home.RequestPointDetails,
        {
            zoomLevel: map.getView().getZoom(),
            latitude: point.latitude,
            longitude: point.longitude,
            identifier: point.ID,
            username: SelectedDataset.username,
            datasetName: SelectedDataset.datasetName
        }, function (receivedInfo) {
            var point = JSON.parse(receivedInfo.data);
            SetPointInfoData(point);
        }
    )

    selectFeatureOnMap();

    DisplayPointInfo();

}

export function UnselectFeatureOnMap(featureId) {
    if (selectedPoints[featureId] === undefined) return;

    vectorSource.removeFeature(selectedPoints[featureId]);
    delete selectedPoints[featureId];

}


/**
 * Section below contain the points request handling
 */
export function UpdatePointsLayer(points) {
    if (vector != null) {
        if (points == null) {
            vectorSource.refresh();
            pointsRegionsManager.ResetCache();
            initialisePointsRequest();
            return;
        }
        vectorSource.addFeatures(points);
        return;
    }

    vectorSource = new ol.source.Vector({
        features: points,
        wrapX: false
    });

    vector = new PointsLayer({
        source: vectorSource
    });
    map.addLayer(vector);
}

export function ClearMap() {
    vectorSource.refresh();
}

function loadDataPoints(pLatitudeFrom, pLongitudeFrom, pLatitudeTo, pLongitudeTo) {
    //do not load any points if the username and dataset is not set 
    if (SelectedDataset.username === null && SelectedDataset.datasetName === null) return;

    var existingRegions = pointsRegionsManager.GetRegions(
        {
            lat: pLatitudeFrom,
            long: pLongitudeFrom
        },
        {
            lat: pLatitudeTo,
            long: pLongitudeTo
        },
        //TODO: change the datasetId to be generic
        SelectedDataset.identifier);

    //*cache and check the data here*
    if (existingRegions === 'cached') return;

    if (processingEnabled == 0) displayHubProcessing(true);
    processingEnabled++;

    hubRouter.RequestDataPoints(
        pLatitudeFrom,
        pLongitudeFrom,
        pLatitudeTo,
        pLongitudeTo,
        existingRegions,
        SelectedDataset.username,
        SelectedDataset.datasetName
    );


}



function initialisePointsRequest(evt) {
    var viewBox = map.getView().calculateExtent(map.getSize());
    var cornerCoordinates = ol.proj.transformExtent(viewBox, 'EPSG:3857', 'EPSG:4326');

    loadDataPoints(
        cornerCoordinates[1],
        cornerCoordinates[0],
        cornerCoordinates[3],
        cornerCoordinates[2]
    )
}

//initialise map interactions
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


/**********************************************/
/*this section contains context menu functions*/