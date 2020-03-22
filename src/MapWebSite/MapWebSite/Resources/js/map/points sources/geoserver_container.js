/*!
 * Component: Points source manager
 * This script contains code which manages the source for the points layer
 * */

export class GeoserverPointsSectionsContainer {

    constructor(map) {
        this.map = map;

        this.pointsLayer = new ol.layer.Tile({
            visible: true,
            source: new ol.source.TileWMS({
                url: 'http://localhost:8080/geoserver/constanta/wms',
                params: {
                    'FORMAT': 'image/png',
                    'VERSION': '1.1.1',
                    tiled: true,
                    "LAYERS": 'constanta:coasta_constanta',
                    "exceptions": 'application/vnd.ogc.se_inimage',
                    tilesOrigin: 26.744917 + "," + 43.1027705
                }
            })
        });

        this.map.addLayer(this.pointsLayer);
    }

    LoadPoints() {
        
    }

    UpdatePointsLayer(points) {
       
    }

    RemoveLayers() {
        this.map.removeLayer(this.pointsLayer);
    }
}