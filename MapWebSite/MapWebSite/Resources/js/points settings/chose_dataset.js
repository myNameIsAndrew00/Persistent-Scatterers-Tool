import { Router, endpoints } from '../api/api_router.js';
 
/** Dataset request to fill the table **/

function resetTable(table) {
    $('#points-settings-layer-container-content').find('#currentDatasetIndex')[0].value = 0;
    var tableHeader = table.children[0].children[0].children[0];

    table.children[0].children[0].innerText = '';
    table.children[0].children[0].innerHtml = '';

    table.children[0].children[0].appendChild(tableHeader);
}


function fillTable(datasets, table) {


    function buildButtonsColumn(username, datasetName) {
        var useButton = document.createElement('button');
        useButton.classList.add('use');
        useButton.innerText = $('#_use-dataset-button-text').val();

        var previewButton = document.createElement('button');
        previewButton.classList.add('preview');
        previewButton.innerText = $('#_preview-dataset-button-text').val();

        return { useButton, previewButton };
    }


    for (var i = 0; i < datasets.length; i++) {        
        var row = document.createElement('tr');
        row.id = 'user_dataset_' + datasets[i].Item1 + '_' + datasets[i].Item2;

        var usernameColumn = document.createElement('td');
        usernameColumn.innerText = datasets[i].Item1;

        var datasetNameColumn = document.createElement('td');
        datasetNameColumn.innerText = datasets[i].Item2;

        var buttonsColumn = document.createElement('td');
        var buttons = buildButtonsColumn(datasets[i].Item1, datasets[i].Item2);
        buttonsColumn.appendChild(buttons.useButton);
        buttonsColumn.appendChild(buttons.previewButton);

        row.appendChild(usernameColumn);
        row.appendChild(datasetNameColumn);
        row.appendChild(buttonsColumn);

        table.appendChild(row);
    }
}

window.loadMorePointsDatasets = function loadMorePointsDatasets(resetPageIndex) {
    var table = $('#points-settings-layer-container-content').find('#ps_left')[0];
    if (resetPageIndex) resetTable(table);



    if (table.offsetHeight + table.scrollTop == table.scrollHeight) {
        var filterValue = $('#points-settings-layer-container-content').find('#datasetSearchValue')[0];
        var pageIndex = $('#points-settings-layer-container-content').find('#currentDatasetIndex')[0];
        var filter = $('#points-settings-layer-container-content').find('#datasetFilterValue')[0];
        
        Router.Get(
            endpoints.PointsSettingsApi.GetDatasetsList,
            { filterValue: filterValue.value, filter: filter[filter.selectedIndex].value, pageIndex: pageIndex.value },
            function (datasets) {
                if (datasets.length)
                    $('#points-settings-layer-container-content').find('#currentDatasetIndex')[0].value = parseInt(pageIndex.value) + 1;

                //fill the table tbody with rows
                fillTable(datasets, table.children[0].children[0]);
            });       
    }
}
