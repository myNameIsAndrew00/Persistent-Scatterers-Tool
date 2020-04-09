/*!
 * Component: Points source manager
 * This script contains code which manages the source for the points layer
 * */
import { DisplayPointInfo, SetPointInfoData } from '../../point info/point_info.js';

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
                    "LAYERS": 'constanta:coasta_constanta_labeled',
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

    InitialiseMapInteraction() {
        var self = this;

        this.map.on('click', function (evt) {

            //adapt properties received from Geoserver to a format accepted by ' point_info '
            function adaptProperties(geoserverFeature) {
                var result = {};
                result.Number = geoserverFeature.Number;
                result.ReferenceX = geoserverFeature.ReferenceX;
                result.ReferenceY = geoserverFeature.ReferenceY;
                result.Longitude = geoserverFeature.Longitude;
                result.Latitude = geoserverFeature.Latitude;
                result.Height = geoserverFeature.Height;
                result.DeformationRate = geoserverFeature.Deformati;
                result.StandardDeviation = geoserverFeature.StandardDe;
                result.EstimatedHeight = geoserverFeature.EstimatedH;
                result.EstimatedDeformationRate = geoserverFeature.EstimatedD;
                result.Displacements = [];

                const displacementsCount = Object.keys(geoserverFeature).length;

                for (var index = 0; index < displacementsCount; index++)
                    result.Displacements[index] = {
                        DaysFromReference: index,
                        Value: geoserverFeature['D_' + index]
                    };

                return result;
            }

            var view = self.map.getView();
            var viewResolution = view.getResolution();
            var source = self.pointsLayer.getSource();
            var url = source.getFeatureInfoUrl(
                evt.coordinate, viewResolution, view.getProjection(),
                {
                    INFO_FORMAT: 'application/json',
                    FEATURE_COUNT: 1
                });
            if (url) {
                //make a request to that url to receive data
                $.ajax({
                    type: 'GET',           
                    crossDomain: true, 
                    url: url,
                    success: function (jsondata) {                       
                        var pointInfo = adaptProperties(jsondata.features[0].properties);

                        SetPointInfoData(pointInfo);

                        DisplayPointInfo();                        
                    }
                })
               

            }
        });
    }

    RemoveLayers() {
        this.map.removeLayer(this.pointsLayer);
    }
}