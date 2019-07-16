


/*
This sample represents the base of lincence :D
Display a set of features on the map.

Required: longitude and latitue
*/

var pointsCount = 20;
var points = new Array(pointsCount);
var e = 18000000;
/*long,lat*/
var bucharestCoordonates = [26.102538390000063, 44.4267674];


var $map = ol.Map;
var $feature = ol.Feature;

/**
 * Here are created the points in the array
 */
for (var i = 0; i < pointsCount; i++) {
    points[i] = new $feature({
        'geometry': new ol.geom.Point(
            ol.proj.fromLonLat([bucharestCoordonates[0] + i, bucharestCoordonates[1] + i], 'EPSG:3857')),
        'i': i,
        'size': i % 2 ? 3 : 4
    });
    points[i].index = i;
    points[i].longitude = bucharestCoordonates[0] + i;
    points[i].latitude = bucharestCoordonates[1] + i;
}

/**
 * According to size, change the aspect of points
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
 * Fill the vector source for layer
 * TO DO: do a hash to get the coordonates function!!!!!
 */
var vectorSource = new ol.source.Vector({
    features: points,
    wrapX: false
});

/*represents the layer which contains the points*/
var vector = new ol.layer.Vector({
    source: vectorSource,
    style: function (feature) {
        return styles[feature.get('size')];
    }
});

/**
 * View for map
 */
var mapView = new ol.View({
    center: ol.proj.fromLonLat(bucharestCoordonates, 'EPSG:3857'),
    zoom: 10
})

/**
 * This functions handle the click on point. Parameter represents the feature
 */

function handleClickFunction(e) {
    selectedPointIndex = e.selected[0].index;

    document.getElementById("longitude").innerHTML = e.selected[0].longitude;
    document.getElementById("latitude").innerHTML = e.selected[0].latitude;
    /*change the point style*/
    e.selected[0].setStyle(new ol.style.Style({
        image: new ol.style.Circle(
            {
                radius: 3,
                fill: new ol.style.Fill({ color: '#FF0000' })
            }
        )
    }));
}

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


var select = new ol.interaction.Select();
map.addInteraction(select);
select.on('select', handleClickFunction);

/**
 * Function on map move
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


map.on('moveend', onMoveEnd); 