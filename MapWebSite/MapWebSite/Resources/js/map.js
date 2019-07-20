/*Shortcut for map and ol*/
var $map = ol.Map;
var $feature = ol.Feature;

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
    zoom: 10
})


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

function diplayPointInfo() {
    document.getElementById("point-info").style = ""; setTimeout(function () {
        document.getElementById("point-info").style.opacity = 1;
        document.getElementById("point-info").style.width = '60%';
    }, 150);
}

function hidePointInfo() {
    document.getElementById("point-info").style.opacity = 0;
    document.getElementById("point-info").style.width = "40%";
    setTimeout(function () {
        document.getElementById("point-info").style.display = "none";
    }, 200);
}

var select = new ol.interaction.Select();
map.addInteraction(select);
select.on('select', handleClickFunction);



/**
 * Function on map move
 * THIS IS A EXAMPLE
 */
var layerAdded = false;
function onMoveEnd(evt) {
    if (mapView.getZoom() >= 6) {
        if (!layerAdded) {
            map.addLayer(vector);
            layerAdded = true;
        }
    }
    else if (layerAdded) {
        map.removeLayer(vector);
        layerAdded = false;
    }
}
//map.on('moveend', onMoveEnd); 

 

/**
 * Here are created the points in the array
 */

function loadData() {

    $.get('/home/RequestDataPoints', function (requestedPoints) {

        for (var i = 0; i < requestedPoints.length ; i++) {
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

        /**
        * Fill the vector source for layer
        * TO DO: do a hash to get the coordonates function!!!!!
        */

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
    });
}

loadData();

