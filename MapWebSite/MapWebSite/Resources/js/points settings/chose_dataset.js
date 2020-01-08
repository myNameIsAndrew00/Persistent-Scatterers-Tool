/*! Component: ChoseDataset
 *
 * This component is responsable for chose dataset content ( inside points setting window )
 *
 * */

import { Router, endpoints } from '../api/api_router.js';
import { UpdatePointsLayer } from '../map.js';

const settingsLayerContainerId = '#points-settings-layer-container-content';

/** Dataset request to fill the table **/

/** Use this class the model a dataset of points inside the table*/
class PointsDataset {

    constructor(username, datasetName) {
        this.username = username;
        this.datasetName = datasetName;

        this.identifier = this.setIdentifier();
    }

    setIdentifier() {
        if (this.username === null || this.datasetName === null) return 0;

        let id = 'user_dataset_' + this.username + '_' + this.datasetName + '_id';
        return $(settingsLayerContainerId).find('[id=\'' + id + '\']')[0].value;
    }

}

var SelectedDataset = new PointsDataset(null,null);

export { SelectedDataset };


function changeSelectedRowOnMenu(id, visible) {
    var paletteRow = $(settingsLayerContainerId).find('[id=\'' + id + '\']')[0];
    if (paletteRow === undefined) return;
    visible ? paletteRow.classList.add('selected-row') : paletteRow.classList.remove('selected-row');
}

/**
 * Use this function to handle the selected dataset
 * @param {any} username name of the user
 * @param {any} datasetName name of the selected dataset
 */
window.useDataset = function useDataset(username, datasetName) {

   
    var previousDatasetRowId = 'user_dataset_' + SelectedDataset.username + '_' + SelectedDataset.datasetName;
    var datasetRowId = 'user_dataset_' + username + '_' + datasetName;

    changeSelectedRowOnMenu(previousDatasetRowId, false);
    changeSelectedRowOnMenu(datasetRowId, true);

    SelectedDataset = new PointsDataset(username, datasetName);

    UpdatePointsLayer();
}








function resetTable(table) {
    $(settingsLayerContainerId).find('#currentDatasetIndex')[0].value = 0;
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
        useButton.onclick = function () {
            useDataset(username, datasetName); 
        };

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

        /*hover the row if the color palette is in use*/
        if (SelectedDataset.username === datasets[i].Item1 && SelectedDataset.datasetName == datasets[i].Item2)
            row.classList.add('selected-row');


        table.appendChild(row);
    }
}

window.loadMorePointsDatasets = function loadMorePointsDatasets(resetPageIndex) {
    var table = $(settingsLayerContainerId).find('#ps_left')[0];
    if (resetPageIndex) resetTable(table);



    if (table.offsetHeight + table.scrollTop == table.scrollHeight) {
        var filterValue = $(settingsLayerContainerId).find('#datasetSearchValue')[0];
        var pageIndex = $(settingsLayerContainerId).find('#currentDatasetIndex')[0];
        var filter = $(settingsLayerContainerId).find('#datasetFilterValue')[0];

        Router.Get(
            endpoints.PointsSettingsApi.GetDatasetsList,
            { filterValue: filterValue.value, filter: filter[filter.selectedIndex].value, pageIndex: pageIndex.value },
            function (datasets) {
                if (datasets.length)
                    $(settingsLayerContainerId).find('#currentDatasetIndex')[0].value = parseInt(pageIndex.value) + 1;

                //fill the table tbody with rows
                fillTable(datasets, table.children[0].children[0]);
            });
    }
}

export function UpdateSelectedDatasetLayout() {
    changeSelectedRowOnMenu('user_dataset_' + SelectedDataset.username + '_' + SelectedDataset.datasetName, true);
}
 