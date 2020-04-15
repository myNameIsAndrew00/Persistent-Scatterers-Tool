
import { Router, endpoints } from '../api/api_router.js';
import { DisplayOverlay } from './settings.js';

window.uploadSelectedGeoserverLayer = function uploadSelectedGeoserverLayer() {
   
    Router.Post(endpoints.Settings.UploadGeoserverLayer,
        {
            name: $('#name').val(),
            url: $('#url').val(),
            colorPaletteId: 1
        },
        function (serverResponse) {
            DisplayOverlay(serverResponse); 
        });

}