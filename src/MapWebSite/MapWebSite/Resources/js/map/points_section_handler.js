/*!
 * Component: PointsSection
 * This script contains code which manages a handler for an area of screen
 * */


import { DisplayHubProcessing, ProcessingEnabled, PointsProcessedNotificationHandler } from './map.js';
import { colorPalette } from '../home.js';
import { PointsLayer } from './points_layer.js';
import { HubRouter } from '../api/hub_router.js';
import { SelectedDataset } from '../points settings/chose_dataset.js';

export const SectionsRowsCount = 1;
export const SectionsColumnsCount = 2;

export class PointsSectionsContainer {

    constructor(map) {
        this.sections = [];
        var sectionsCount = 0;

        for (var i = 0; i < SectionsRowsCount; i++)
            for (var j = 0; j < SectionsColumnsCount; j++)
                this.sections[sectionsCount++] = new PointsSectionHandler(i, j, map);
    }

    LoadPoints() {
        for (var i = 0; i < this.sections.length; i++)
            this.sections[i].LoadPoints();
    }

    UpdatePointsLayer(points) {
        for (var i = 0; i < this.sections.length; i++)
            this.sections[i].UpdatePointsLayer(points);
    }
}


class PointsSectionHandler {

    constructor(rowIndex, columnIndex, map) {
        this.previousDisplayedPoints = null;

        var caller = this;

        this.rowIndex = rowIndex;
        this.columnIndex = columnIndex;

        /*initialise the router*/
        this.hubRouter = new HubRouter();

        /*
        var pointsRegionsManager = new PointsRegionsManager();

        hubRouter.SetCallback('UpdateRegionsData', function (receivedInfo) {
            var regionData = JSON.parse(receivedInfo);
            pointsRegionsManager.AddRegion(regionData.Region, regionData.PointsCount, regionData.Filled);
        }); */


        this.hubRouter.SetCallback('ProcessPoints', async function (receivedInfo) { caller.processPoints(receivedInfo) });
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
    }


    LoadPoints() {

        this.secondaryVectorSource.clear(true);

        /*switch the displayed layers*/
        this.switchLayers();

        if (SelectedDataset.username === null && SelectedDataset.datasetName === null) return;


        /*var existingRegions = pointsRegionsManager.GetRegions(
            {
                lat: pLatitudeFrom,
                long: pLongitudeFrom
            },
            {
                lat: pLatitudeTo,
                long: pLongitudeTo
            },
            SelectedDataset.identifier);

        //*cache and check the data here*
        if (existingRegions === 'cached') {
       //     return;
        }*/
        //this can be used with caching
        var existingRegions = [];

        DisplayHubProcessing(true);


        var coordinates = this.getCornerCoordinates();

        this.hubRouter.RequestDataPoints(
            coordinates.topCorner.latitude,
            coordinates.topCorner.longitude,
            coordinates.bottomCorner.latitude,
            coordinates.bottomCorner.longitude,
            this.map.getView().getZoom(),
            existingRegions,
            SelectedDataset.username,
            SelectedDataset.datasetName
        );
    }

    UpdatePointsLayer(points) {
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

    /*private methods*/

    async processPoints(receivedInfo) {

        function buildStyleFromPalette(featureValue) {

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


        var points = [];
        var index = 0;

        var requestedPoints = JSON.parse(receivedInfo);
        console.log('Hub result: ' + requestedPoints.length);


        for (var i = 0; i < requestedPoints.length; i++) { // requestedPoints.length; i++) {
            points[i + index] = new ol.Feature({
                'geometry': new ol.geom.Point(
                    ol.proj.fromLonLat([requestedPoints[i].Longitude, requestedPoints[i].Latitude], 'EPSG:3857')),
                'colorCriteria': requestedPoints[i].Height
            });
            points[i + index].setId(requestedPoints[i].Number);
            points[i + index].ID = requestedPoints[i].Number;
            points[i + index].longitude = requestedPoints[i].Longitude;
            points[i + index].latitude = requestedPoints[i].Latitude;
            points[i + index].color = buildStyleFromPalette(requestedPoints[i].Height);
        }

        requestedPoints.splice(0, requestedPoints.length);
        this.UpdatePointsLayer(points);
    }


    getCornerCoordinates() {
        var viewBox = this.map.getView().calculateExtent(this.map.getSize());
        var cornerCoordinates = ol.proj.transformExtent(viewBox, 'EPSG:3857', 'EPSG:4326');
        console.log('latitude from: ' + cornerCoordinates[3]);
        console.log('longitude from: ' + cornerCoordinates[0]);
        console.log('latitude to: ' + cornerCoordinates[1]);
        console.log('longitude to: ' + cornerCoordinates[2]);

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