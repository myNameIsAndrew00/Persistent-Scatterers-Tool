/*! MODULE: ApiRouter
 * 
 * This module is responsable for api requests handling.
 * Do not use any api without using this component
 * 
 * */

const applicationApiUrl = '/api';
const geolocationApiUrl = 'https://nominatim.openstreetmap.org/search/';

export const endpoints = {
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
        GetDatasetLimits: applicationApiUrl + '/PointsSettingsApi/GetDatasetLimits'
    },
    Settings: {
        SaveColorsPalette: applicationApiUrl + '/settings/SaveColorsPalette',
        UploadFileChunk: applicationApiUrl + '/settings/UploadFileChunk',
        ClearFileChunks: applicationApiUrl + '/settings/ClearFileChunks',
        MergeFileChunks: applicationApiUrl + '/settings/MergeFileChunks',
        CheckDatasetExistance: applicationApiUrl + '/settings/CheckDatasetExistance'
    },
    LoginApi: {
        ValidateUsername: applicationApiUrl + '/LoginApi/ValidateUsername',
        ValidateEmail: applicationApiUrl + '/LoginApi/ValidateEmail',
        ValidatePassword: applicationApiUrl + '/LoginApi/ValidatePassword'
    }

};

export class Router {
    /*internal methods*/

    static Get(endpoint, parameters, callback) {
        $.get(endpoint,
            parameters,
            callback);
    }

 

    static Post(endpoint, parameters, callback, errorCallback = function () { }) {
        $.ajax({
            type: "POST",
            data: parameters,
            url: endpoint,
            success: function (data) { callback(data) },
            error: function (data) { errorCallback(data) }
        });
    }
 
}

export class GeolocationRouter {

    static Get(location, callback) {
        $.ajax({
            type: "GET",
            url: geolocationApiUrl + location + '?format=json&limit=5',
            dataType: "json",
            success: function (data) { callback(data) }
        });
    }

}
 