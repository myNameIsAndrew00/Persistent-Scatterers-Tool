import { colorPalette } from './home.js';
import { DisplayPointInfo, SetPointInfoData } from './point info/point_info.js';
import { Router, endpoints } from './api/api_router.js';
import { HubRouter } from './api/hub_router.js';
import { PointsLayer } from './map/points_layer.js';
import { PointsRegionsManager } from './api/cache/points_regions_manager.js';

//var points = [];
var vector = null;  
var vectorSource = null;
var pointsRegionsManager = new PointsRegionsManager();

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
            'colorCriteria': requestedPoints[i].OptionalField
        });
        points[i + index].setId(requestedPoints[i].Number);
        points[i + index].ID = requestedPoints[i].Number;
        points[i + index].longitude = requestedPoints[i].Longitude;
        points[i + index].latitude = requestedPoints[i].Latitude;   
        points[i + index].color = buildStyleFromPalette(requestedPoints[i].OptionalField);
    }

    requestedPoints.splice(0, requestedPoints.length);

   
    UpdatePointsLayer(points);       
});


/**
 * View for map
 */
var mapView = new ol.View({
    center: ol.proj.fromLonLat([28.652880, 44.177269], 'EPSG:3857'),
    zoom: 16,
    minZoom: 3,
    maxZoom: 20
})

/*
 * TODO: Define the navigation for map (zoom on double click must be disabled)
 */


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
    
    Router.Get(endpoints.Home.RequestPointDetails,
        {
            zoomLevel: map.getView().getZoom(),
            latitude: point.latitude,
            longitude: point.longitude,
            identifier: point.ID,
        }, function (receivedInfo) {
            var point = JSON.parse(receivedInfo.data);
            SetPointInfoData(point);
        }
    )     

    DisplayPointInfo();

}
 

/**
 * Section bellow contain the points request handling
 */
export function UpdatePointsLayer(points) {
    if (vector != null) {
        if (points == null) {
            vectorSource.refresh();           
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

function loadDataPoints(pLatitudeFrom, pLongitudeFrom, pLatitudeTo, pLongitudeTo) {
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
        1059);

    //*cache and check the data here*
    if (existingRegions === 'cached') return;
   
    hubRouter.RequestDataPoints(
        pLatitudeFrom,
        pLongitudeFrom,
        pLatitudeTo,
        pLongitudeTo,
        existingRegions,
        'Height'
    );


}



function onMapPositionChanged(evt) {
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
map.on('moveend', onMapPositionChanged);
map.on('click', function (evt) {
    if (map.getView().getInteracting()) {
        return;
    }
    var pixel = evt.pixel;

    map.forEachFeatureAtPixel(pixel, function (feature) {
        handleClickFunction(feature);
    });
});
 