/*! Component: ChoseDataset
 *
 * This component is responsable for chose dataset content ( inside points setting window )
 *
 * */

import { Router, endpoints } from '../api/api_router.js';
import { UpdatePointsLayer } from '../map/map.js';
import { CurrentSource } from '../map/chose_points_source.js';

const settingsLayerContainerId = '#points-settings-layer-container-content';
const settingsLayerContainerClass = '.points-settings-layer-container-palette-content';
const currentDatasetTextId = '#current-dataset-text';

//labels ids
const infoSectionId = '#ps_right';
const heightLimitLeftLabelId = '#height_limit_left';
const heightLimitRightLabelId = '#height_limit_right';
const stddevLimitLeftLabelId = '#stddev_limit_left';
const stddevLimitRightLabelId = '#stddev_limit_right';
const defrateLimitLeftLabelId = '#defrate_limit_left';
const defrateLimitRightLabelId = '#defrate_limit_right';

/** Dataset request to fill the table **/

/** Use this class to model a dataset of points inside the table*/
class PointsDataset {
    innerData = null;
    innerDataRequest = null;

    constructor(username, datasetName) {
        var self = this;

        this.user = username;
        this.name = datasetName;

        this.identifier = this.setIdentifier();

        this.innerDataRequest = Router.Get(endpoints.PointsSettingsApi.GetDatasetLimits,
            {
                username,
                datasetName
            },
            function (response) {
                self.innerData = response;
                console.log(response); 
            },
            function () {
                self.innerData = {};
            }
        );  
    }

    async GetInnerData() { 
        await this.innerDataRequest;

        return this.innerData;
    }

    setIdentifier() {
        if (this.user === null || this.name === null) return 0;

        let id = 'user_dataset_' + this.user + '_' + this.name + '_id';
        return $(settingsLayerContainerId).find('[id=\'' + id + '\']')[0].value;
    }

    setDatasetLimits() {
             
    }

    ///use this method to display dataset details inside ps_right menu 
    displayDatasetLimits(username, datasetName) {
        var self = this;
        Router.Get(endpoints.PointsSettingsApi.GetDatasetLimits,
            {
                username,
                datasetName
            },
            function (response) {
                self.innerData = response;

                console.log(response);
                $(infoSectionId).removeClass('ps_right-hidden');
                $(settingsLayerContainerClass).addClass('points-settings-layer-container-palette-content-expand');
            }
        );  
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

    var previousDatasetRowId = 'user_dataset_' + SelectedDataset.user + '_' + SelectedDataset.name;
    var datasetRowId = 'user_dataset_' + username + '_' + datasetName;

    SelectedDataset = new PointsDataset(username, datasetName);
   
    changeSelectedRowOnMenu(previousDatasetRowId, false);
    changeSelectedRowOnMenu(datasetRowId, true);

    $(currentDatasetTextId).text(datasetName);

    UpdatePointsLayer();
}







/**
 * This function can be used to reset the content of the displayed table wihout removing the header
 * @param {any} table
 */
function resetTable(table) {
    $(settingsLayerContainerId).find('#currentDatasetIndex')[0].value = 0;
    var tableHeader = table.children[0].children[0].children[0];

    table.children[0].children[0].innerText = '';
    table.children[0].children[0].innerHtml = '';

    table.children[0].children[0].appendChild(tableHeader);
}


function fillTable(datasets, table) {


    function buildButtonsColumn(username, datasetName, isValid) {
        var useButton = document.createElement('button');
        var previewButton = document.createElement('button');

        useButton.classList.add('use');
        useButton.innerText = $('#_use-dataset-button-text').val();
        useButton.onclick = function () {
            useDataset(username, datasetName);  
        };
        useButton.disabled = !isValid;

        previewButton.innerText = $('#_preview-dataset-button-text').val();
        previewButton.classList.add('preview');
        previewButton.onclick = function () {
            SelectedDataset.displayDatasetLimits(username, datasetName);
        };
        previewButton.disabled = !isValid;

        return { useButton, previewButton };
    }


    for (var i = 0; i < datasets.length; i++) {
        var row = document.createElement('tr');
        var hiddenInput = document.createElement('input');


        row.id = 'user_dataset_' + datasets[i].Username + '_' + datasets[i].Name;
        hiddenInput.id = row.id + '_id';
        hiddenInput.name = hiddenInput.id;     
        hiddenInput.value = datasets[i].ID; //store the value of dataset
        hiddenInput.type = 'hidden';

        var usernameColumn = document.createElement('td');
        usernameColumn.innerText = datasets[i].Username;
          
        var datasetNameColumn = document.createElement('td');
        datasetNameColumn.innerText = datasets[i].Name;

        var statusColumn = document.createElement('td');
        statusColumn.innerText = datasets[i].Status;

        var buttonsColumn = document.createElement('td');
        var buttons = buildButtonsColumn(datasets[i].Username, datasets[i].Name, datasets[i].IsValid);

        
        buttonsColumn.appendChild(buttons.useButton); 
        buttonsColumn.appendChild(buttons.previewButton);

        row.appendChild(usernameColumn);
        row.appendChild(datasetNameColumn);
        row.appendChild(statusColumn);
        row.appendChild(buttonsColumn);

        /*hover the row if the color palette is in use*/
        if (SelectedDataset.user === datasets[i].Username && SelectedDataset.name == datasets[i].Name)
            row.classList.add('selected-row');


        table.appendChild(row);
        table.appendChild(hiddenInput);
    }
}

//this variable is used internally to check if a user is writing in the textbox
var writing = 0;
window.loadMorePointsDatasets = function loadMorePointsDatasets(resetPageIndex) { 
    var endWriting = ++writing;

    setTimeout(
        function () {
            if (endWriting != writing) return;

            var table = $(settingsLayerContainerId).find('#ps_left')[0];
            if (resetPageIndex) resetTable(table);

            if (table.offsetHeight + table.scrollTop >= table.scrollHeight - 5) {
                var filterValue = $(settingsLayerContainerId).find('#datasetSearchValue')[0];
                var pageIndex = $(settingsLayerContainerId).find('#currentDatasetIndex')[0];
                var filter = $(settingsLayerContainerId).find('#datasetFilterValue')[0];

                const filter0 = filter === undefined ? 'None' : filter[filter.selectedIndex].value;
                const filterValue0 = filterValue === undefined ? '' : filterValue.value;

                Router.Get(
                    endpoints.PointsSettingsApi.GetDatasetsList,
                    {
                        filtersCount: 2,
                        filter0,
                        filterValue0,
                        filter1: 'Source',
                        filterValue1: CurrentSource,
                        pageIndex: pageIndex.value
                    },
                    function (datasets) {
                        if (datasets.length)
                            $(settingsLayerContainerId).find('#currentDatasetIndex')[0].value = parseInt(pageIndex.value) + 1;

                        //fill the table tbody with rows
                        fillTable(datasets, table.children[0].children[0]);
                    });
            }
        },
        400
    )
}

export function UpdateSelectedDatasetLayout() {
    changeSelectedRowOnMenu('user_dataset_' + SelectedDataset.user + '_' + SelectedDataset.name, true);
}
 