/*! MODULE: ApiRouter
 * 
 * This module is responsable for api requests handling.
 * Do not use the api without using this component
 * 
 * */

const apiUrl = '/api';

export const endpoints = {
    Home: {
        RequestSettingsLayerContent: '/Home/RequestSettingsLayerContent',      
        RequestPointDetails: apiUrl + '/HomeApi/RequestPointDetails',
        RequestRegionsKeys: apiUrl + '/HomeApi/RequestRegionsKeys'
    },
    PointsSettings: {
        GetColorPalettePage: apiUrl + '/PointsSettings/GetColorPalettePage',
        GetChoseDatasetPage: apiUrl + '/PointsSettings/GetChoseDatasetPage'
    },
    PointsSettingsApi: {
        GetColorPalette: apiUrl + '/PointsSettingsApi/GetColorPalette',
        GetColorPaletteList: apiUrl +'/PointsSettingsApi/GetColorPaletteList',
        GetDatasetsList: apiUrl +'/PointsSettingsApi/GetDatasetsList'
    },
    Settings: {
        SaveColorsPalette: apiUrl +'/settings/SaveColorsPalette',
        UploadFileChunk: apiUrl + '/settings/UploadFileChunk',
        ClearFileChunks: apiUrl + '/settings/ClearFileChunks',
        MergeFileChunks: apiUrl +'/settings/MergeFileChunks',
        CheckDatasetExistance: apiUrl +'/settings/CheckDatasetExistance'

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
 