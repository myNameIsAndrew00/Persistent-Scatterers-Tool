import { colorPalette } from './home.js';
import { DisplayPointInfo, SetPointInfoData } from './point_info.js';



var points = [];
var vector = null;

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
    controls: [],
    renderer: 'webgl'
});


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
 

function buildStyleFromPalette(featureValue, latitude, longitude) {

    function getRectangleCoordinates() {
        var rectangleWidth = 0;
        switch (map.getView().getZoom()) {
            case 19:
                rectangleWidth = 0.0001; break;                
            case 18:
                rectangleWidth = 0.00045; break;
            case 17:
                rectangleWidth = 0.00125; break;
            case 16:
                rectangleWidth = 0.0018; break;
            case 15:
                rectangleWidth = 0.0032; break;
            case 14:
                rectangleWidth = 0.00405; break;
            case 13:
                rectangleWidth = 0.00605; break;
            case 12:
                rectangleWidth = 0.00845; break;
            case 11:
                rectangleWidth = 0.01125; break;
            case 10:
                rectangleWidth = 0.01445; break;
            case 9:
                rectangleWidth = 0.01805; break;
            case 8:
                rectangleWidth = 0.02205; break; 
            case 7:
                rectangleWidth = 0.02645; break; 
            case 6:
                rectangleWidth = 0.03125; break;
            case 5:
                rectangleWidth = 0.03645; break;
            case 4:
                rectangleWidth = 0.04205; break;
            case 3:
                rectangleWidth = 0.04805; break;
            default:
                rectangleWidth =  0.5;
        }

        var polyCoords = [];

        polyCoords.push(ol.proj.transform([longitude + rectangleWidth, latitude - rectangleWidth * 2   ],
            'EPSG:4326',
            'EPSG:3857'));
        polyCoords.push(ol.proj.transform([longitude + rectangleWidth, latitude + rectangleWidth * 2  ],
            'EPSG:4326',
            'EPSG:3857'));
        polyCoords.push(ol.proj.transform([longitude - rectangleWidth, latitude + rectangleWidth * 2  ],
                'EPSG:4326',
            'EPSG:3857'));
        polyCoords.push(ol.proj.transform([longitude - rectangleWidth, latitude - rectangleWidth * 2  ],
                    'EPSG:4326',
            'EPSG:3857')); 
        return polyCoords;
    }

    var rangeMin = -2;
    var rangeMax = 80;

    var paletteColor = binarySearch(
        ((featureValue + Math.abs(rangeMin)) * 100) / (rangeMax + Math.abs(rangeMin)),
        0,
        colorPalette.length - 1
    );
     

    //select the shape based on zoom level
    if (map.getView().getZoom() > 16)
        return new ol.style.Style({
            image: new ol.style.Circle({
                radius: 3,
                fill: new ol.style.Fill({ color: paletteColor })
            })
        });
    else return new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: 'transparent',
            width: 0
        }),
        fill: new ol.style.Fill({ color: paletteColor + '3F' }),
        geometry: new ol.geom.Polygon([ getRectangleCoordinates() ] )
    });

}




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
                return buildStyleFromPalette(feature.get('colorCriteria'),
                                             feature.latitude,
                                             feature.longitude);
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
