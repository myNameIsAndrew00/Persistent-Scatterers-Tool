/*! Component: PointsUpload
 *
 * This component maintains a upload page in the GUI
 *
 * */

import { DisplayOverlay } from './settings.js';
import { Router, endpoints } from '../api/api_router.js';

var processedFiles = {};

/*finaliser*/
export function PageFinaliser() {
    //finalise logic here
}

/*Style and aspect handling*/

window.changeFileInputAspect = function changeFileInputAspect() {
    $('#upload_points_container_form').children('#file-input-visible').html(
        $('#uploadFile')[0].files[0].name
    );
}

function notifyFileNotSelected() {
    $('#upload_points_container_form').children('#warning-text').css('visibility', 'visible');
}

window.enableDatasetSubmit = function enableDatasetSubmit() {
    $('#upload_points_container_form').children('#upload_points_submit').prop('disabled',
        $('#upload_points_container_form').children('#upload_points_dataset_name').val() === '');
}


/*****************************************************************************/
/*send button click handling*/


window.uploadDataSets = function uploadDataSets() {
    var dataSetName = $('#upload_points_container_form').children('#upload_points_dataset_name').val();
    if ($('#uploadFile')[0].files[0] === undefined) {
        notifyFileNotSelected();
        return;
    }
    checkDataSetName(dataSetName);
};


function checkDataSetName(dataSetName) {
    Router.Post(endpoints.Settings.CheckDatasetExistance,
        { fileName: dataSetName },
        function (serverResponse) {
            if (serverResponse.includes("Success"))
                uploadFile($('#uploadFile')[0].files[0],
                    dataSetName);
            else {
                DisplayOverlay(serverResponse);
            }
        });   
}

function enableUpload() {
    $('#upload_points_container_form').children('#current_percent').removeClass('current-percent-hidden');
    updatePercentText('0');
}

function updatePercentText(percent) {
      $('#upload_points_container_form').children('#current_percent').html(
        percent + '%');
}

/*****************************************************************************/
/*sending data functions*/

function uploadFile(file, dataSetName) {
    enableUpload();

    var fileChunks = [];

    var chunkSize = 10485760;

    var fileSize = file.size;
    var chunkBeginPointer = 0;
    var chunkEndPointer = chunkSize;

    while (chunkBeginPointer < fileSize) {
        fileChunks.push(file.slice(chunkBeginPointer, chunkEndPointer));
        chunkBeginPointer = chunkEndPointer;
        chunkEndPointer = chunkEndPointer + chunkSize;
    }

    processedFiles[dataSetName] = { chunksSent: 0, total: fileChunks.length };

    var chunkIndex = 0;
    var chunk = null;
    while (chunk = fileChunks.shift()) {
        uploadChunk(chunk, dataSetName + '_' + chunkIndex++, dataSetName);
        setTimeout(function () { }, 100);
    };

}



function uploadChunk(chunk, chunkName, dataSetName) {
    var formData = new FormData();
    formData.append('file', chunk, chunkName);

    $.ajax({
        type: 'POST',
        url: endpoints.Settings.UploadFileChunk,
        contentType: false,
        processData: false,
        data: formData,
        success: function (serverResponse) {
            //process server response

            processedFiles[dataSetName].chunksSent++;

            //update user interface
            updatePercentText( ((100 / processedFiles[dataSetName].total) * processedFiles[dataSetName].chunksSent).toFixed(2));

            if (processedFiles[dataSetName].chunksSent >= processedFiles[dataSetName].total) {
                updatePercentText('100');
                mergeChunks(dataSetName);
            }
        }
    });
}


function mergeChunks(dataSetName) {

    Router.Post(endpoints.Settings.MergeFileChunks,
        { fileName: dataSetName },
        function (serverResponse) {
            DisplayOverlay(serverResponse);
        });
}