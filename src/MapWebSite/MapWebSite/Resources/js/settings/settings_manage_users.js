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
		manageUsersContainerId: '#manage-users-container',
		manageDatasetsContainerId: '#manage-datasets-container',
		usersTableId: '#manage-users-table',
		datasetsTableId: '#manage-datasets-table',
		usersTablePagesCount: '#manage-users-container #pagesCount',
		itemsPerPage: '#manage-users-container #itemsPerPage'
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
		usersTableTitle: 'Application users',
		datasetTableTitle: 'User datasets'
	}
};

window.drawTables = function drawTables() {

	usersTableDrawer = new TableDrawer({
		title: constants.strings.usersTableTitle,
		container: constants.id.manageUsersContainerId,
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
			width: 8,
			columnName: 'Datasets'
		}
		],
		dataSourceCallback: function (pageIndex, itemsPerPage) {
			return new Promise(function (fullfill, reject) {
				Router.Get(endpoints.Settings.GetUsers,
					{
						pageIndex,
						itemsPerPage
					},
					function (data) {
						fullfill(data);
					}
				);
			});
		}
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
	if (datapointsTableDrawer == null)
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
				return new Promise(function (fullfil, _) {
					Router.Get(endpoints.Settings.GetUserDatasets,
						{
							username: selectedUser,
							pageIndex,
							itemsPerPage
						},
						function (data) {
							fullfil(data)
						}
					);
				});
			},
			deleteRowCallback: function (dataItem) {
				return new Promise(function (fullfil, _) {
					Router.Post(endpoints.Settings.RemoveDatasetFromUser,
						{
							username: selectedUser,
							datasetName: dataItem.Name,
							datasetUser: dataItem.Username
						},
						function (data) {
							fullfil({ isValid: true });
						}
					);
				});
			},
			addRowCallback: function (dataItem) {
				return new Promise(function (fullfil, _) {
					Router.Post(endpoints.Settings.AddDatasetToUser,
						{
							username: selectedUser,
							datasetName: dataItem.Name,
							datasetUser: dataItem.Username
						},
						function (data) {
							fullfil({ isValid: true });
						}
					);
				});
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

	Router.Get(endpoints.Settings.GetUserAssociatedDatasetsCount,
		{
			username: selectedUser
		},
		function (datasetsCountResult) {
			const itemsPerPage = parseInt($(constants.id.itemsPerPage).val());
			Router.Get(endpoints.Settings.GetUserDatasets,
				{
					username: selectedUser,
					pageIndex: 0,
					itemsPerPage
				},
				function (datasets) {
					datapointsTableDrawer.ReplaceProperties({ title: (constants.strings.datasetTableTitle + ` (${selectedUser})`) });
					//datapointsTableDrawer.Clear();
					datapointsTableDrawer.Draw(datasets,
						(datasetsCountResult.count / itemsPerPage) + (datasetsCountResult.count % itemsPerPage == 0 ? 0 : 1));

					$(constants.id.manageDatasetsContainerId)[0].scrollIntoView({
						behavior: "smooth", // or "auto" or "instant"
						block: "end"
					});
				});
		}
	);
}

drawTables();