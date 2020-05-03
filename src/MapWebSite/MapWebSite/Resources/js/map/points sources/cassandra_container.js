/*!
 * Component: PointsSection
 * This script contains code which manages a points handler for an area of screen
 * Points source is Cassandra database. Communication is made via websockets - signalR
 * */


import { DisplayProcessing, ProcessingEnabled, PointsProcessedNotificationHandler } from '../map.js';
import { PointsRegionsManager } from '../../api/cache/points_regions_manager.js';
import { ColorPalette } from '../../home.js';
import { Router, endpoints } from '../../api/api_router.js';
import { PointsLayer } from '../points_layer.js';
import { HubRouterInstance } from '../../api/hub_router.js';
import { SelectedDataset } from '../../points settings/chose_dataset.js';
import { SelectedCriteria } from '../../points settings/chose_criteria.js';
import { PointsDimensionScale } from '../../points settings/chose_points_size.js';
import { DisplayPointInfo, SetPointInfoData } from '../../point info/point_info.js';

export const SectionsRowsCount = 1;
export const SectionsColumnsCount = 1;



export class CassandraPointsSectionsContainer {

    constructor(map) {
        this.sections = [];
        this.cache = new PointsRegionsManager();

        var sectionsCount = 0;

        for (var i = 0; i < SectionsRowsCount; i++)
            for (var j = 0; j < SectionsColumnsCount; j++)
                this.sections[sectionsCount++] = new PointsSectionHandler(i, j, map, this.cache);

        HubRouterInstance.EnableConnection();
    }

    LoadPoints() {
        for (var i = 0; i < this.sections.length; i++)
            this.sections[i].LoadPoints();
    }

    UpdatePointsLayer(points) {
        for (var i = 0; i < this.sections.length; i++)
            this.sections[i].UpdatePointsLayer(points);
    }

    InitialiseMapInteraction() {
        for (var i = 0; i < this.sections.length; i++)
            this.sections[i].InitialiseMapInteraction();
    }

    //use this method to delete points layers from map
    RemoveLayers() {
        for (var i = 0; i < this.sections.length; i++)
            this.sections[i].RemoveLayer();
    }
}


class PointsSectionHandler {

    constructor(rowIndex, columnIndex, map, localCache) {
        this.previousDisplayedPoints = null;

        var caller = this;

        this.rowIndex = rowIndex;
        this.columnIndex = columnIndex;

        /*initialise the router and the local cache*/
        this.hubRouter = HubRouterInstance;

        /*
        var pointsRegionsManager = new PointsRegionsManager();

        hubRouter.SetCallback('UpdateRegionsData', function (receivedInfo) {
            var regionData = JSON.parse(receivedInfo);
            pointsRegionsManager.AddRegion(regionData.Region, regionData.PointsCount, regionData.Filled);
        }); */


        this.hubRouter.SetCallback('ProcessPoints', function (receivedInfo) {
            console.log('received');
            caller.processPoints(receivedInfo, false)
        });
       
        this.hubRouter.SetCallback('PointsProcessedNotification', PointsProcessedNotificationHandler);

        /**Use two vector sources to display the data. Alternate this layers when displaying something on map*/
        this.secondaryVectorSource = new ol.source.Vector({
            wrapX: false
        });
        this.mainVectorSource = new ol.source.Vector({
            wrapX: false
        });

        this.mainVector = new PointsLayer({
            source: this.mainVectorSource
        });
        this.secondaryVector = new PointsLayer({
            source: this.secondaryVectorSource
        });


        this.map = map;
        this.map.addLayer(this.mainVector);
        this.map.addLayer(this.secondaryVector);

        this.localCache = localCache;
    }

    InitialiseMapInteraction() {
        var map = this.map;

        this.map.on('click', function (evt) {
            if (map.getView().getInteracting()) {
                return;
            }
            var pixel = evt.pixel;

            map.forEachFeatureAtPixel(pixel, function (feature) {
                //  if (selectedPoints[feature.ID] === undefined) {
                handleClickFunction(feature);
                // }
            });
        });

        /*follow section handle the click on items*/
        /**
         * This functions handle the click on point. Parameter represents the feature
         */

        function handleClickFunction(point) {

            /*
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
            }*/

            Router.Get(endpoints.Home.RequestPointDetails,
                {
                    zoomLevel: map.getView().getZoom(),
                    latitude: point.latitude,
                    longitude: point.longitude,
                    identifier: point.ID,
                    username: SelectedDataset.user,
                    datasetName: SelectedDataset.name
                }, function (receivedInfo) {
                    SetPointInfoData(receivedInfo);
                }
            )


            DisplayPointInfo();
        }
    }

    LoadPoints() {
        if (SelectedDataset.user === null && SelectedDataset.name === null) return;


        this.secondaryVectorSource.clear(true);
        /*switch the displayed layers*/
        this.switchLayers();

        DisplayProcessing(true);

        var coordinates = this.getCornerCoordinates();
        var caller = this;

        /*request the regions keys from server*/
        Router.Get(endpoints.Home.RequestRegionsKeys,
            {
                latitudeFrom: coordinates.topCorner.latitude,
                longitudeFrom: coordinates.topCorner.longitude,
                latitudeTo: coordinates.bottomCorner.latitude,
                longitudeTo: coordinates.bottomCorner.longitude,
                zoomLevel: Math.floor(this.map.getView().getZoom()),
                username: SelectedDataset.user,
                datasetName: SelectedDataset.name
            }, function (regionsKeys) {
                 
                var cachedRegionsKeys = caller.localCache.GetRegionsKeys(regionsKeys);

                /*if there are any cached regions on client side*/
                if (cachedRegionsKeys.length > 0) {
                    /*process them*/
                    var cachedPoints = caller.localCache.GetRegions(cachedRegionsKeys);
                    caller.processPoints(cachedPoints, true);

                    /*if the required data is cached, return */
                    if (regionsKeys.length == cachedRegionsKeys.length) {
                        PointsProcessedNotificationHandler();
                        return;
                    }
                }              

                caller.hubRouter.RequestDataPoints(
                    coordinates.topCorner.latitude,
                    coordinates.topCorner.longitude,
                    coordinates.bottomCorner.latitude,
                    coordinates.bottomCorner.longitude,
                    Math.floor(caller.map.getView().getZoom()),
                    cachedRegionsKeys,
                    SelectedDataset.user,
                    SelectedDataset.name
                );
            }
        );
            
        
      
    }

    UpdatePointsLayer(points) {
        console.log('received response');

        if (this.mainVector != null) {
            if (points == null) {
                this.mainVectorSource.clear(true); 

                // pointsRegionsManager.ResetCache();
                this.LoadPoints();
                return;
            }

            this.mainVector.hide();
            this.mainVectorSource.addFeatures(points);      
            this.mainVector.animateIn();

           
        }
    }

    RemoveLayer() {
        this.map.removeLayer(this.mainVector);
        this.map.removeLayer(this.secondaryVector);
    }

    /*private methods*/

    async processPoints(receivedInfo, fromCache) {

        function buildStyleFromPalette(featureValue) {

            function binarySearch(value, left, right) {
                if (left == right) return ColorPalette.intervals[right].Color;
                if (left >= ColorPalette.intervals.length - 1) return ColorPalette.intervals[ColorPalette.intervals.length - 1].Color;

                if (left == right - 1) {
                    if (value < ColorPalette.intervals[right].Left)
                        return ColorPalette.intervals[left].Color;
                    return ColorPalette.intervals[right].Color;
                }

                var middle = Math.floor((right + left) / 2);

                if (value < ColorPalette.intervals[middle].Left)
                    return binarySearch(value, left, middle);
                if (value > ColorPalette.intervals[middle].Right)
                    return binarySearch(value, middle, right);

                return ColorPalette.intervals[middle].Color;
            }

            function hexToRgb(hex) {
                var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
                return result ? {
                    r: parseInt(result[1], 16),
                    g: parseInt(result[2], 16),
                    b: parseInt(result[3], 16)
                } : null;
            }
             

            var paletteColor = binarySearch(
                featureValue,
                0,
                ColorPalette.intervals.length - 1
            );

            return hexToRgb(paletteColor);
        }


        var points = [];
        var index = 0;
        var pointsData = null;

        //check if data was received from hub or cache
        if (fromCache) {
            pointsData = receivedInfo;
             
        }
        else {

            var requestedData = JSON.parse(receivedInfo);
            this.localCache.AddRegion(requestedData.regionKey, requestedData.pointsData);

            pointsData = requestedData.pointsData; 
        }

        var index = 0;

        for (var i = 0; i < pointsData.length; i++) { // requestedPoints.length; i++) {
            if (pointsData[i] == null) continue;
            points[index] = new ol.Feature({
                geometry : new ol.geom.Point(
                    ol.proj.fromLonLat([pointsData[i].Longitude, pointsData[i].Latitude], 'EPSG:3857')),
                
            });
            points[index].setId(pointsData[i].Number);
            points[index].ID = pointsData[i].Number;
            points[index].longitude = pointsData[i].Longitude;
            points[index].latitude = pointsData[i].Latitude;
            points[index].color = buildStyleFromPalette(pointsData[i][SelectedCriteria]);
            points[index].size =  ( PointsDimensionScale() / 100 ) * 4.0
            index++;
        }

       

        this.UpdatePointsLayer(points);
    }


    getCornerCoordinates() {
        var viewBox = this.map.getView().calculateExtent(this.map.getSize());
        var cornerCoordinates = ol.proj.transformExtent(viewBox, 'EPSG:3857', 'EPSG:4326');
    
        //formula: latide(or longidue) = [ (latitudeTop - latitudeDown) / rowsCount ] * rowIndex + latTop;
        var sectionLength = {
            oX: (cornerCoordinates[3] - cornerCoordinates[1]) / SectionsRowsCount,
            oY: (cornerCoordinates[2] - cornerCoordinates[0]) / SectionsRowsCount
        };
        var topCorner = {
            latitude: sectionLength.oX * this.rowIndex + cornerCoordinates[3],
            longitude: sectionLength.oY * this.columnIndex + cornerCoordinates[0]
        };

        return {
            topCorner,
            bottomCorner: {
                latitude: topCorner.latitude - sectionLength.oX,
                longitude: topCorner.longitude + sectionLength.oY
            }
        }
    }

    switchLayers() {
        var aux = this.mainVectorSource;
        this.mainVectorSource = this.secondaryVectorSource;
        this.secondaryVectorSource = aux;

        aux = this.secondaryVector;
        this.secondaryVector = this.mainVector;
        this.mainVector = aux;
    }




}