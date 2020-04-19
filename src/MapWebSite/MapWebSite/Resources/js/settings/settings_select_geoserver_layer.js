
import { Router, endpoints } from '../api/api_router.js';
import { DisplayOverlay } from './settings.js';

const constants = {
    id : {
        name: '#name', 
        submitButton: '#geoserverSelectFinishButton'
    }
}


window.uploadSelectedGeoserverLayer = function uploadSelectedGeoserverLayer() {
   
    Router.Post(endpoints.Settings.UploadGeoserverLayer,
        {
            name: $(constants.id.name).val()
        },
        function (serverResponse) {
            DisplayOverlay(serverResponse); 
        });

}

window.enableGeoserverUploadSubmit = function enableGeoserverUploadSubmit(){
   $(constants.id.submitButton).prop('disabled',
       $(constants.id.name).val() === '');
}
