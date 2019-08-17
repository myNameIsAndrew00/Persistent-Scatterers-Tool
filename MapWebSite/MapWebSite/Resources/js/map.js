/*Shortcut for map and ol*/
var $map = ol.Map;
var $feature = ol.Feature;
var $control = ol.Control;
var points = []; 
var vector = null;

/**
 * According to size, change the aspect of points
 * THIS IS A EXAMPLE
 */
var styles = {
    '3': new ol.style.Style({
        image: new ol.style.Circle({
            radius: 3,
            fill: new ol.style.Fill({ color: '#FF0000' }),
        })
    }),
    '4': new ol.style.Style({
        image: new ol.style.Circle({
            radius: 3,
            fill: new ol.style.Fill({ color: '#00FF00' }),
        })
    })
};


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
 
var map = new $map({
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

    selectedPointIndex = e.selected[0].index;
    document.getElementById("longitude").innerHTML = e.selected[0].longitude;
    document.getElementById("latitude").innerHTML = e.selected[0].latitude;
    diplayPointInfo();

   
    /*change the point style*/
    /*e.selected[0].setStyle(new ol.style.Style({
        image: new ol.style.Circle(
            {
                radius: 3,
                fill: new ol.style.Fill({ color: '#0000FF' })
            }
        )
    }));*/
}


var select = new ol.interaction.Select();
map.addInteraction(select);
select.on('select', handleClickFunction);
 

/**
 * Section bellow contain the points request
 */

function loadData(pZoomLevel, pLatitudeFrom, pLongitudeFrom, pLatitudeTo, pLongitudeTo) {
    $.ajax({
        type: "GET",
        data: {
            zoomLevel: pZoomLevel,
            latitudeFrom: pLatitudeFrom,
            longitudeFrom: pLongitudeFrom,
            latitudeTo: pLatitudeTo,
            longitudeTo: pLongitudeTo
        },
        url: '/home/RequestDataPoints',
        success: function (receivedInfo) {
            points.splice(0, points.length);

            var requestedPoints = JSON.parse(receivedInfo.data);
            for (var i = 0; i < requestedPoints.length; i++) {
                points[i] = new $feature({
                    'geometry': new ol.geom.Point(
                        ol.proj.fromLonLat([requestedPoints[i].Longitude, requestedPoints[i].Latitude], 'EPSG:3857')),
                    'i': i,
                    'size': i % 2 ? 3 : 4
                });
                points[i].index = i;
                points[i].longitude = requestedPoints[i].Longitude;
                points[i].latitude = requestedPoints[i].Latitude;
            }

            requestedPoints.splice(0, requestedPoints.length);
 
            map.removeLayer(vector);

            if(vector != null) delete vector.vectorSource;
            delete vector;

            var vectorSource = new ol.source.Vector({
                features: points,
                wrapX: false
            });
          
            /*represents the layer which contains the points*/
            vector = new ol.layer.Vector({
                source: vectorSource,
                style: function (feature) {
                    return styles[feature.get('size')];
                }
            });
            map.addLayer(vector);           
        }
    });
}




function onMapPositionChanged(evt) {
    var viewBox = map.getView().calculateExtent(map.getSize());  
    var cornerCoordinates = ol.proj.transformExtent(viewBox, 'EPSG:3857', 'EPSG:4326');
    
    loadData(map.getView().getZoom(),
        cornerCoordinates[1],
        cornerCoordinates[0],
        cornerCoordinates[3],
        cornerCoordinates[2]
    )
}
map.on('moveend', onMapPositionChanged);

onMapPositionChanged(null);
