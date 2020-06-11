/*! Component: ManageDatasets
 *
 * This component maintaint UI for datasets management
 *
 * */
import { TableDrawer, RowDrawingTypes, ColumnDrawingTypes } from '../utilities/Table/table_drawer.js';
import { Router, endpoints } from '../api/api_router.js';
import { constants as settingsConstants, DisplayOverlay, HideOverlay } from './settings.js';

var datapointsTableDrawer = null;

const constants = {
    id: {
        manageDatasetsContainerId: '#manage-datasets-container',
        datasetsTableId: '#manage-datasets-table',
		usersTablePagesCount: '#manage-datasets-container #pagesCount',
		itemsPerPage: '#manage-datasets-container #itemsPerPage'
    },
    class: {
        tableClass: 'default-table',
        tableContainerClass: 'default-table-container box',
        formClass: 'message-overlay-container edit-datasets-form',
        defaultInputClass: 'default-input-gray'
	},
	resources: {
		addIcon: '/Resources/resources/icons/add_black_icon.svg',
		editIcon: '/Resources/resources/icons/edit_black_icon.svg',
		deleteIcon: '/Resources/resources/icons/delete_black_icon.svg',
		exitIcon: '/Resources/resources/icons/close_icon.svg'
	},
	strings: {
		datasetTableTitle: 'Datasets'
	}
}

window.drawDatasetsTable = function drawDatasetsTable() {
 
		datapointsTableDrawer = new TableDrawer({
			title: constants.strings.datasetTableTitle,
			container: constants.id.manageDatasetsContainerId,
			editFormContainer: settingsConstants.id.settingsOverlay,
			class: constants.class.tableClass,
			tableContainerClass: constants.class.tableContainerClass,
			id: constants.id.datasetsTableId,
			itemsPerPage: parseInt($(constants.id.itemsPerPage).val()),
			rowDrawingType: RowDrawingTypes.raw,
			commandHeader: {
				addButtonIcon: constants.resources.addIcon,
				editButtonIcon: constants.resources.editIcon,
				deleteButtonIcon: constants.resources.deleteIcon
			},
			rowDrawingRules: [{
				drawingType: ColumnDrawingTypes.raw,
				columnName: 'ID'
			},
			{
				drawingType: ColumnDrawingTypes.raw,
				columnName: 'Name'
			},
			{
				drawingType: ColumnDrawingTypes.raw,
				columnName: 'Username'
			},
			{
				drawingType: ColumnDrawingTypes.raw,
				columnName: 'Status',
				ignoreEdit: true
			},
			{
				drawingType: ColumnDrawingTypes.raw,
				columnName: 'Source',
				ignoreEdit: true
			},
			{
				drawingType: ColumnDrawingTypes.raw,
				columnName: 'IsValid',
				displayName: 'Valid',
				width: 1,
				ignoreEdit: true
			}
			],
			dataSourceCallback: function (pageIndex, itemsPerPage) {
				return new Promise(function (fullfill, reject) {
					Router.Get(endpoints.Settings.GetDatasets,
						{
							pageIndex,
							itemsPerPage
						},
						function (data) {
							fullfill(data);
						}
					);
				});
			},
			deleteRowCallback: function (dataItem) {
				 
			},		
			form: {
				class: constants.class.formClass,
				closeButtonIcon: constants.resources.exitIcon,
				textInputClass: constants.class.defaultInputClass
			},
			preOpenEditFormCallback: function () {
				DisplayOverlay($(''));
			},
			postCloseEditFormCallback: function () {
				HideOverlay(false);
			},
		});

	Router.Get(endpoints.Settings.GetDatasets,
		{
			pageIndex: 0,
			itemsPerPage: parseInt($(constants.id.itemsPerPage).val())
		},
		function (users) {
			datapointsTableDrawer.Draw(users, parseInt($(constants.id.usersTablePagesCount).val()));
		}
	);
}


drawDatasetsTable();