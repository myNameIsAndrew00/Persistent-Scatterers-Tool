var processedFiles = {};


/*send button click handling*/
window.uploadDataSets = function uploadDataSets() {
    uploadFile($('#uploadFile')[0].files[0], 'radarada45');
};



/*sending data functions*/

function uploadFile(file, dataSetName) {
    var fileChunks = [];

    var chunkSize = 1048576;

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
    while (chunk = fileChunks.shift())
        uploadChunk(chunk, dataSetName + '_' + chunkIndex++, dataSetName);

}



function uploadChunk(chunk, chunkName, dataSetName) {
    var formData = new FormData();
    formData.append('file', chunk, chunkName);

    $.ajax({
        type: 'POST',
        url: 'api/settings/UploadFileChunk',
        contentType: false,
        processData: false,
        data: formData,
        success: function (serverResponse) {
            //process server response

            processedFiles[dataSetName].chunksSent++;
            if (processedFiles[dataSetName].chunksSent == processedFiles[dataSetName].total)
                mergeChunks(dataSetName);
        }
    });
}


function mergeChunks(dataSetName) {
    $.ajax({
        type: 'POST',
        url: 'api/settings/MergeFileChunks',
        dataType: 'json',
        data: { fileName: dataSetName },
        success: function (serverResponse) {
            //process server response
            alert('file uploaded');
        }
    })
}