/*! MODULE: ApiRouter
 * 
 * This module is responsable for api requests handling.
 * Do not use any api without using this component
 * 
 * */

const applicationApiUrl = '/rpc';
const geolocationApiUrl = 'https://nominatim.openstreetmap.org/search/';


export class Router {
    routeTable = {
        Home: {
            RequestSettingsLayerContent: '/Home/RequestSettingsLayerContent',
            RequestPointDetails: applicationApiUrl + '/HomeApi/RequestPointDetails',
            RequestRegionsKeys: applicationApiUrl + '/HomeApi/RequestRegionsKeys'
        },
        Miscellaneous: {
            GetNotificationsPage: '/Miscellaneous/GetNotificationsPage',
            GetChoseMapTypePage: '/Miscellaneous/GetChoseMapTypePage',
            GetChangePointsSizePage: '/Miscellaneous/GetChangePointsSizePage',
            GetGifTooltip: '/Miscellaneous/GetGifTooltip',
            GetTooltip: '/Miscellaneous/GetTooltip',
            GetChosePointsSourcePage: '/Miscellaneous/GetChosePointsSourcePage'
        },
        PointsSettings: {
            GetColorPalettePage: '/PointsSettings/GetColorPalettePage',
            GetChoseDatasetPage: '/PointsSettings/GetChoseDatasetPage',
            GetChoseDisplayCriteriaPage: '/PointsSettings/GetChoseDisplayCriteriaPage'
        },
        PointsSettingsApi: {
            GetColorPalette: applicationApiUrl + '/PointsSettingsApi/GetColorPalette',
            GetColorPaletteList: applicationApiUrl + '/PointsSettingsApi/GetColorPaletteList',
            GetDatasetsList: applicationApiUrl + '/PointsSettingsApi/GetDatasetsList',
            GetDatasetLimits: applicationApiUrl + '/PointsSettingsApi/GetDatasetLimits',
            ValidateGeoserverStyle: applicationApiUrl + '/PointsSettingsApi/ValidateGeoserverStyle'
        },
        Settings: {
            SaveColorsPalette: applicationApiUrl + '/settings/SaveColorsPalette',
            UploadFileChunk: applicationApiUrl + '/settings/UploadFileChunk',
            ClearFileChunks: applicationApiUrl + '/settings/ClearFileChunks',
            MergeFileChunks: applicationApiUrl + '/settings/MergeFileChunks',
            CheckDatasetExistance: applicationApiUrl + '/settings/CheckDatasetExistance',
            UploadGeoserverLayer: applicationApiUrl + '/settings/UploadGeoserverLayer',
            GetUsers: applicationApiUrl + '/settings/GetUsers',
            GetUserDatasets: applicationApiUrl + '/settings/GetUserDatasets',
            GetUserAssociatedDatasetsCount: applicationApiUrl + '/settings/GetUserAssociatedDatasetsCount',
            AddDatasetToUser: applicationApiUrl + '/settings/AddDatasetToUser',
            RemoveDatasetFromUser: applicationApiUrl + '/settings/RemoveDatasetFromUser',
            GetDatasets: applicationApiUrl + '/settings/GetDatasets'
        },
        LoginApi: {
            ValidateUsername: applicationApiUrl + '/LoginApi/ValidateUsername',
            ValidateEmail: applicationApiUrl + '/LoginApi/ValidateEmail',
            ValidatePassword: applicationApiUrl + '/LoginApi/ValidatePassword'
        }

    };

    static Get(endpoint, parameters, callback, errorCallback = function () { }) {
        return $.ajax({
            type: "GET",
            data: parameters,
            url: endpoint,
            success: function (data) { callback(data) },
            error: function (data) { errorCallback(data) }
        });
       
    }

 

    static Post(endpoint, parameters, callback, errorCallback = function () { }) {
        return $.ajax({
            type: "POST",
            data: parameters,
            url: endpoint,
            success: function (data) { callback(data) },
            error: function (data) { errorCallback(data) }
        });
    }
 
}

export let endpoints = new Router().routeTable; 

export class GeolocationRouter {

    static Get(location, callback) {
        return $.ajax({
            type: "GET",
            url: geolocationApiUrl + location + '?format=json&limit=5',
            dataType: "json",
            success: function (data) { callback(data) }
        });
    }

}
 