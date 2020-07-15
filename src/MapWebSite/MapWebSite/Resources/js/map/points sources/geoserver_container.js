/*!
 * Component: PointsSection
 * This script contains code which manages a points handler for an area of screen
 * Points source is Geoserver, sources details is PostgreSQL.
 * */

import { DisplayPointInfo, SetPointInfoData, SetPointDetailsInfo } from '../../point info/point_info.js';
import { SelectedDataset } from '../../points settings/chose_dataset.js';
import { ColorPalette } from '../../home.js';
import { Router, endpoints } from '../../api/api_router.js';
import { DisplayProcessing, PointsProcessedNotificationHandler } from '../map.js';

export class GeoserverPointsSectionsContainer {

    constructor(map) {
        this.map = map;
        this.pointsLayer = null;
    }

    LoadPoints() {

    }

    UpdatePointsLayer() {
        this.RemoveLayers();

        var self = this;

        if (ColorPalette.isUnitialised())
            self.initPointslayer('');
        else
            Router.Get(endpoints.PointsSettingsApi.ValidateGeoserverStyle,
                {
                    datasetName: SelectedDataset.name,
                    datasetUsername: SelectedDataset.user,
                    paletteName: ColorPalette.name,
                    paletteUsername: ColorPalette.user
                },
                function (response) {
                    self.initPointslayer(ColorPalette.user + '_' + ColorPalette.name);
                    self.InitialiseMapInteraction();
                });

    }

    InitialiseMapInteraction() {
        var self = this;

        this.map.on('click', function (evt) {
            if (self.pointsLayer == null) return;

            //self.getDetailsFromWmts(evt);
            self.getDetailsFromApplication(evt);
        });
    }

    RemoveLayers() {
        if (this.pointsLayer != null)
            this.map.removeLayer(this.pointsLayer);

        this.pointsLayer = null;
    }

    /*private region below*/

    getDetailsFromWmts(evt) {

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

            const displacementsCount = Object.keys(geoserverFeature).length - 10;

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
    }

    getDetailsFromApplication(evt) {
        const coordinates = ol.proj.toLonLat(evt.coordinate);

        Router.Get(endpoints.Home.RequestPointDetails,
            {
                zoomLevel: this.map.getView().getZoom(),
                latitude: coordinates[1],
                longitude: coordinates[0],
                identifier: 0,
                username: SelectedDataset.user,
                datasetName: SelectedDataset.name,
                pointsSource: 'Geoserver'
            }, function (receivedInfo) {
                SetPointInfoData(receivedInfo);
            }
        )

        SetPointDetailsInfo(null);
        DisplayPointInfo();
    }

    async initPointslayer(style) {
        const datasetData = await SelectedDataset.GetInnerData();

        this.pointsLayer = new ol.layer.Tile({
            visible: true,
            source: new ol.source.TileWMS({
                url: datasetData.OptionalData.ServerUrl,
                params: {
                    'FORMAT': 'image/png',
                    'VERSION': '1.1.1',
                    tiled: true,
                    "LAYERS": datasetData.Name,
                    "exceptions": 'application/vnd.ogc.se_inimage',
                    tilesOrigin: 26.744917 + "," + 43.1027705,
                    'STYLES': style,
                }
            })
        });
        this.pointsLayer.getSource().on('tileloadstart', function () {
            DisplayProcessing(true);
        });
        this.pointsLayer.getSource().on('tileloadend', function () {
            PointsProcessedNotificationHandler();
        });
        this.pointsLayer.getSource().on('tileloaderror', function () {
            PointsProcessedNotificationHandler();
        });

        this.map.addLayer(this.pointsLayer);

    }
}