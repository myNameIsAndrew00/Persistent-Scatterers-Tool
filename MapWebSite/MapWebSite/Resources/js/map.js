//import * as Total from '../../FrameworkContent/OpenLayers/ol.js';
import { colorPalette } from './home.js';
import { DisplayPointInfo, SetPointInfoData } from './point_info.js';
//import { Map } from '../../FrameworkContent/OpenLayers/ol.js';

 
/*Shortcut for map and ol*/
 
 
var points = [];
var vector = null;


/**
 * According to size, change the aspect of points
 * THIS IS A EXAMPLE
 */

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
    var rangeMin = -2;
    var rangeMax = 80;

    var paletteColor = binarySearch(
        ((featureValue + Math.abs(rangeMin)) * 100) / (rangeMax + Math.abs(rangeMin)),
        0,
        colorPalette.length - 1
    );

    return new ol.style.Style({
        image: new ol.style.Circle({
            radius: 3,
            fill: new ol.style.Fill({ color: paletteColor }),
        })
    })
}


/**
 * View for map
 */
var mapView = new ol.View({
    center: ol.proj.fromLonLat([26.102538390000063, 44.4267674], 'EPSG:3857'),
    zoom: 3,
    minZoom: 3,
    maxZoom: 20
})

/*
 * TODO: Define the navigation for map (zoom on double click must be disabled)
 */


/**
 * Here the map is rendered 
 */

var map = new ol.Map({
    target: 'map',
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        })
    ],
    view: mapView,
    controls: [],
    renderer: 'webgl'
});


/*follow section handle the click on items*/
/**
 * This functions handle the click on point. Parameter represents the feature
 */

function handleClickFunction(e) {
    if (e.selected.length == 0) return;
    var point = e.selected[0];
    $.ajax({
        type: "GET",
        data: {
            zoomLevel: map.getView().getZoom(),
            latitude: point.latitude,
            longitude: point.longitude,
            identifier: point.ID,
        },
        url: '/home/RequestPointDetails',
        success: function (receivedInfo) {
            var point = JSON.parse(receivedInfo.data);
            SetPointInfoData(point);
        }
    });

    DisplayPointInfo();
 
}


var select = new ol.interaction.Select();
map.addInteraction(select);
select.on('select', handleClickFunction);


/**
 * Section bellow contain the points request
 */
export function UpdatePointsLayer() {
    var vectorSource = new ol.source.Vector({
        features: points,
        wrapX: false
    });


    if (vector != null) {
        vector.setOpacity(0);
        vector.setSource(vectorSource)
    }
    else {
        vector = new ol.layer.Vector({
            source: vectorSource,
            style: function (feature) {
                return buildStyleFromPalette(feature.get('colorCriteria'));
            },
            opacity: 0
        });
        map.addLayer(vector);
    }

    var animationFrames = 0;
    var opacity = 0.1;
    setTimeout(function () {
        function change() {
            if (animationFrames == 10) return;
            animationFrames++;
            opacity = animationFrames / 10;
            vector.setOpacity(opacity);
            setTimeout(function () { change() }, 30)
        }
        change();
    }, 50);
}

function loadDataPoints(pZoomLevel, pLatitudeFrom, pLongitudeFrom, pLatitudeTo, pLongitudeTo) {
    $.ajax({
        type: "GET",
        data: {
            zoomLevel: pZoomLevel,
            latitudeFrom: pLatitudeFrom,
            longitudeFrom: pLongitudeFrom,
            latitudeTo: pLatitudeTo,
            longitudeTo: pLongitudeTo,
            optionalField: 'Height'
        },
        url: '/home/RequestDataPoints',
        success: function (receivedInfo) {
            points.splice(0, points.length);

            var requestedPoints = JSON.parse(receivedInfo.data);
            for (var i = 0; i < requestedPoints.length; i++) {
                points[i] = new ol.Feature({
                    'geometry': new ol.geom.Point(
                        ol.proj.fromLonLat([requestedPoints[i].Longitude, requestedPoints[i].Latitude], 'EPSG:3857')),
                    'colorCriteria': requestedPoints[i].OptionalField
                });
                points[i].ID = requestedPoints[i].Number;
                points[i].longitude = requestedPoints[i].Longitude;
                points[i].latitude = requestedPoints[i].Latitude;
            }

            requestedPoints.splice(0, requestedPoints.length);

            UpdatePointsLayer();


        }
    });
}




function onMapPositionChanged(evt) {
    var viewBox = map.getView().calculateExtent(map.getSize());
    var cornerCoordinates = ol.proj.transformExtent(viewBox, 'EPSG:3857', 'EPSG:4326');

    loadDataPoints(map.getView().getZoom(),
        cornerCoordinates[1],
        cornerCoordinates[0],
        cornerCoordinates[3],
        cornerCoordinates[2]
    )
}
map.on('moveend', onMapPositionChanged);

onMapPositionChanged(null);
