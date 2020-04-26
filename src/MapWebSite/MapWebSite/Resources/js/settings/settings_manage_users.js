/*! Component: ManageUsers
 *
 * This component maintaint UI for users management
 *
 * */

import { TableDrawer, RowDrawingTypes, ColumnDrawingTypes } from '../utilities/Table/table_drawer.js';
import { Router, endpoints } from '../api/api_router.js';
import { constants as settingsConstants, DisplayOverlay, HideOverlay } from './settings.js';


var usersTableDrawer = null;
var datapointsTableDrawer = null;

var selectedUser = '';

const constants = {
	id: {
		manageUsersContainer: '#manage-users-container',
		usersTableId: '#manage-users-table',
		datasetsTableId: '#manage-datasets-table',
		usersTablePagesCount: '#pagesCount',
		itemsPerPage: '#itemsPerPage'
	},
	class: {
		tableClass: 'default-table',
		tableContainerClass: 'default-table-container box',
		formClass: 'message-overlay-container edit-datasets-form'
	},
	resources: {
		addIcon: '/Resources/resources/icons/add_black_icon.svg',
		editIcon: '/Resources/resources/icons/edit_black_icon.svg',
		deleteIcon: '/Resources/resources/icons/delete_black_icon.svg'
	},
	strings: {
		usersTableTitle: 'Application users',
		datasetTableTitle: 'User datasets'
	}
};

window.drawTables = function drawTables() {

	usersTableDrawer = new TableDrawer({
		title: constants.strings.usersTableTitle,
		container: constants.id.manageUsersContainer,
		class: constants.class.tableClass,
		tableContainerClass: constants.class.tableContainerClass,
		id: constants.id.usersTableId,
		itemsPerPage: parseInt($(constants.id.itemsPerPage).val()),
		rowDrawingType: RowDrawingTypes.raw,		
		rowDrawingRules: [{
			drawingType: ColumnDrawingTypes.raw,			 
			columnName: 'Username'
		},
		{
			drawingType: ColumnDrawingTypes.raw,			 
			columnName: 'FirstName',
			displayName: 'First name'
		},
		{
			drawingType: ColumnDrawingTypes.raw,			 
			columnName: 'LastName',
			displayName: 'Last name'
		},
		{
			drawingType: ColumnDrawingTypes.raw,			 
			columnName: 'Email'
		},
		{
			drawingType: ColumnDrawingTypes.function,
			drawingFunction: function (columnValue) {
				var displayDatasetsButton = $('<button>');
				displayDatasetsButton.text('Display datasets');

				displayDatasetsButton.click(function () {
					selectedUser = columnValue.Username;
					drawDatasetsTable();
				});
			
				return ($('<td></td>').append(displayDatasetsButton));
			},
			width: 10,
			columnName: 'Datasets'
		}
		],	
		dataSourceCallback: function (pageIndex, itemsPerPage, tableCallback) {
			Router.Get(endpoints.Settings.GetUsers,
				{
					pageIndex,
					itemsPerPage
				},
				tableCallback
			);
		}
	});

	datapointsTableDrawer = new TableDrawer({
		container: constants.id.manageUsersContainer,
		title: constants.strings.datasetTableTitle,
		editFormContainer: settingsConstants.id.settingsOverlay,
		class: constants.class.tableClass,
		tableContainerClass: constants.class.tableContainerClass,
		id: constants.id.datasetsTableId,
		itemsPerPage: parseInt($(constants.id.itemsPerPage).val()),
		rowDrawingType: RowDrawingTypes.raw,
		commandHeader: {
			addButtonIcon: constants.resources.addIcon ,
			editButtonIcon: constants.resources.editIcon,
			deleteButtonIcon: constants.resources.deleteIcon
		},	
		rowDrawingRules: [{
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
			columnName: 'IsValid',
			displayName: 'Valid',
			width: 8,
			ignoreEdit: true
		}
		],
		dataSourceCallback: function (pageIndex, itemsPerPage, tableCallback) {
			Router.Get(endpoints.Settings.GetUserDatasets,
				{
					username: selectedUser,
					pageIndex,
					itemsPerPage
				},
				tableCallback
			);
		},
		deleteRowCallback: function (dataItem, responseCallback) {
			responseCallback({ isValid: true });
		},
		addRowCallback: function (dataItem, responseCallback) {
			console.log(dataItem);
			responseCallback({ isValid: true });
		},
		updateRowCallback: function (dataItem, responseCallback) {
			console.log(dataItem);
			responseCallback({ isValid: true });
		},
		form: {
			class: constants.class.formClass
		},
		preOpenEditFormCallback: function (continueCallback) {
			DisplayOverlay($(''));
			continueCallback();
		},
		postCloseEditFormCallback: function (continueCallback) {
			HideOverlay(false);
			continueCallback();
		},
	});

	Router.Get(endpoints.Settings.GetUsers,
		{
			pageIndex: 0,
			itemsPerPage: parseInt($(constants.id.itemsPerPage).val())
		},
		function (users) {
			usersTableDrawer.Draw(users, parseInt($(constants.id.usersTablePagesCount).val()));
		}
	);
}

function drawDatasetsTable() {
	Router.Get(endpoints.Settings.GetUserDatasets,
		{
			username: selectedUser,
			pageIndex: 0,
			itemsPerPage: 100
		},
		function (datasets) {
			datapointsTableDrawer.Clear();
			datapointsTableDrawer.Draw(datasets, 1);
		}
	);
}

drawTables();