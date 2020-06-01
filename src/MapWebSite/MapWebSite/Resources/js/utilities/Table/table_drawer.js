/*!Component: Table drawer
 *
 * Handles table drawing and basic operations (add,update,insert,pagination)
 *
 * */
export const RowDrawingTypes = {
	raw: 'raw',
	function: 'function'
};

export const ColumnDrawingTypes = {
	raw: 'raw',
	function: 'function'
}

const constants = {
	class: {
		inputGroup: 'input-content',
		checkboxColumn: 'checkbox',
		paginationButtons: 'pagination-buttons',
		commandHeader: 'command-header',
		textButton: 'text-button',
		buttonSelected: 'button-selected',
		title: 'title',
		editForm: 'edit-form',
		editFormCloseButton: 'close-button',
		editFormFinishButon: 'finish-button'
	}
}



export class TableDrawer {

	constructor(properties) {
		this.properties = properties;

		//handles if a row can be edited or deleted
		this.editableRows = false;

		//handles current page
		this.currentPage = 0;

		//handles selected indexes which must update/deleted
		this.selectedIndexes = [];
		//handles the table data model
		this.data = [];


		this.drawTable();
	}

	Clear() {
		this.removeRows();
	}

	Remove() {
		$(this.properties.container).empty();
	}

	ReplaceProperties(properties) {
		var self = this;
		$.each(properties, function (key, value) {
			self.properties[key] = properties[key];
		});

		this.Remove();
		this.drawTable();
	}

	Draw(data, pagesCount) {
		this.data = data;
		this.currentPage = 0;

		this.appendRows();

		if (pagesCount != undefined && pagesCount != null)
			this.properties.pagesCount = pagesCount;

		//append buttons to buttons container
		this.appendPaginationButtons(true);
	}

	/** private region **/

	drawTable() {
		//handles the container of table 
		this.tableContainer = $("<div></div>")
			.addClass(this.properties.tableContainerClass);
		this.tableContainer.id = this.properties.id;

		//handle the table 
		this.table = $("<table></table>")
			.addClass(this.properties.class);
		//handle the pagination buttons container
		this.paginationButtonsContainer = $("<div></div>")
			.addClass(constants.class.paginationButtons);

		//handle the command header container
		this.commandHeaderContainer = $("<div></div>")
			.addClass(constants.class.commandHeader);


		//append command header
		this.appendCommandHeader();

		//append header to table
		this.appendTableHeader();

		this.tableContainer.append(this.commandHeaderContainer);
		this.tableContainer.append(this.table);
		this.tableContainer.append(this.paginationButtonsContainer);

		$(this.properties.container).append(this.tableContainer);
	}

	/**
	 * Use this method to draw a icon into a button
	 * @param {any} button The button which must be filled
	 * @param {any} path Path to the svg file which will provide the icon
	 * @param {any} text Textplaceholder if path is undefined
	 */
	drawButtonIcon(button, path, text) {
		if (path === undefined) {
			button.append(text);
			button.addClass(constants.class.textButton);
			return;
		}
		var svg = $('<object>');
		svg.attr('data', path);
		svg.attr('type', 'image/svg+xml');

		button.append(svg);
	}

	appendCommandHeader() {
		var self = this;


		function drawAddButton() {
			var button = $('<button>');

			button.click(function () {
				self.drawEditForm(null,
					function (response) {
						if (response.isValid) {
							$.when(self.properties.addRowCallback(response.dataItem)).then(
								function (validationResult) {
									if (validationResult.isValid === true) {
										self.closeEditForm();
										self.redraw();
									}
									//else validationResult.message
								});
						}
						else alert('nope');
					}
				);
			});

			self.drawButtonIcon(button, self.properties.commandHeader.addButtonIcon, 'Add');
			self.commandHeaderContainer.append(button);
		}

		function drawDeleteButton() {
			var button = $('<button>');

			button.click(function () {
				{
					var deletedCount = 0;

					for (var i = 0; i < self.selectedIndexes.length; i++)
						$.when(self.properties.deleteRowCallback(self.data[self.selectedIndexes[i]])).then(
							function (validationResult) {
								deletedCount++;
								if (deletedCount === self.selectedIndexes.length) {
									self.closeEditForm();
									self.redraw();
								}
								//else validationResult.message
							});

				}

			});

			self.drawButtonIcon(button, self.properties.commandHeader.deleteButtonIcon, 'Delete');
			self.commandHeaderContainer.append(button);
		}

		function drawUpdateButton() {
			var button = $('<button>');

			button.click(function () {
				if (self.selectedIndexes.length == 0)
					return;

				self.drawEditForm(self.data[self.selectedIndexes[0]],
					function (response) {
						if (response.isValid) {
							$.when(self.properties.updateRowCallback(response.dataItem)).then(
								function (validationResult) {
									if (validationResult.isValid === true) {
										self.closeEditForm();
										self.redraw();
									}
									//else validationResult.message
								});
						}
						else alert('nope');

						//self.properties.updateRowCallback
					}
				);
			});

			self.drawButtonIcon(button, self.properties.commandHeader.editButtonIcon, 'Edit');
			self.commandHeaderContainer.append(button);
		}

		function drawTitle() {
			var title = $('<h5></h5>')
				.addClass(constants.class.title)
				.html(self.properties.title);

			self.commandHeaderContainer.append(title);
		}

		drawTitle();
		if (this.properties.addRowCallback)
			drawAddButton();
		if (this.properties.deleteRowCallback)
			drawDeleteButton();
		if (this.properties.updateRowCallback)
			drawUpdateButton();

		this.editableRows = !(this.properties.updateRowCallback === undefined) || !(this.properties.deleteRowCallback === undefined);

	}

	/** append table header  */
	appendTableHeader() {
		function drawColumn(drawingRule) {
			var column = $("<th></th>");
			column.text(drawingRule.displayName ? drawingRule.displayName : drawingRule.columnName);
			if (drawingRule.width)
				column.width(drawingRule.width + '%');
			header.append(column);
		}

		var header = $("<tr></tr>");
		if (this.editableRows === true) {
			var checkBoxColumn = $("<th></th>");
			checkBoxColumn.addClass(constants.class.checkboxColumn);
			header.append(checkBoxColumn);
		}

		this.properties.rowDrawingRules.forEach(drawColumn);

		this.table.append(header);
	}

	/**
	 * append pagination buttons on the bottom of the table
	 * @param {any} redraw a boolean which represents if the paginations buttons should be redrawed
	 */
	appendPaginationButtons(redraw) {
		if (redraw === true) this.paginationButtonsContainer.html('');

		var self = this;

		function drawButton(pageIndex) {
			var button = $('<button>');
			button.text(pageIndex + 1);
			if (self.currentPage == pageIndex) button.addClass(constants.class.buttonSelected);

			button.click(function () {

				self.currentPage = pageIndex;
				self.redraw();
			});

			self.paginationButtonsContainer.append(button);
		}

		const neighboursCount = 3;
		const leftIndex = Math.max(0,
			self.currentPage - neighboursCount - Math.max(0, (self.currentPage + neighboursCount - self.properties.pagesCount + 1)));
		const rightIndex = Math.min(self.properties.pagesCount - 1,
			self.currentPage + neighboursCount - Math.min(0, (self.currentPage - neighboursCount)));

		if (leftIndex != 0) this.paginationButtonsContainer.append($('<span></span>').text('...'));
		for (var i = leftIndex; i <= rightIndex; i++)
			drawButton(i);
		if (rightIndex != self.properties.pagesCount - 1) this.paginationButtonsContainer.append($('<span></span>').text('...'));
	}

	/**
	 * append rows to the table from this.data
	 * */
	appendRows() {
		var self = this;
		var index = 0;

		function drawRow(dataItem) {

			function drawColumn(drawingRule) {
				var column = null;
				switch (drawingRule.drawingType) {
					case ColumnDrawingTypes.function:
						column = drawingRule.drawingFunction(dataItem);
						break;
					case ColumnDrawingTypes.raw:
					default:
						column = $('<td></td>');
						column.text(dataItem[drawingRule.columnName]);
						break;
				}

				row.append(column);
			}

			function drawRowCheckbox() {
				var input = $('<input>');
				var innerIndex = index;
				input.attr('type', 'checkbox');

				input.click(function () {
					self.selectItem(innerIndex, !($(this).is(':checked')));
				});

				var column = $('<td></td>');
				column.append(input);

				row.append(column);
			}

			var row = $('<tr></tr>');
			if (self.editableRows === true) drawRowCheckbox();

			switch (self.rowDrawingType) {
				case RowDrawingTypes.function:
					row = self.properties.rowDrawingFunction(dataItem);
					break;
				case RowDrawingTypes.raw:
				default:
					self.properties.rowDrawingRules.forEach(drawColumn);
					break;
			}

			self.table.append(row);
			index++;
		}

		this.data.forEach(drawRow);

	}

	/**
	 * redraw the table content using the data source callback 
	 * */
	redraw() {
		var self = this;

		$.when(this.properties.dataSourceCallback(
			this.currentPage,
			this.properties.itemsPerPage)).then(
				function (data) {
					self.data = data;
					self.selectedIndexes = [];


					self.removeRows();
					self.appendRows();
					self.appendPaginationButtons(true);
				}
			)
		/*this.properties.dataSourceCallback(
			this.currentPage,
			this.properties.itemsPerPage,
			function (data) {
				self.data = data;
				self.selectedIndexes = [];

				self.removeRows();
				self.appendRows();
				self.appendPaginationButtons(true);
			}
		);*/

	}

	/**
	 * removes all rows from table content
	 * */
	removeRows() {
		this.table.children('tr:not(:first)').remove();
	}

	selectItem(itemIndex, unselect) {
		if (unselect === true)
			this.selectedIndexes = this.selectedIndexes.filter(index => index != itemIndex);
		else this.selectedIndexes.push(itemIndex);
	}

	drawEditForm(dataItem, editAction) {
		var self = this;

		function drawForm() {
			var form = $('<form></form>')
				.addClass(self.properties.form.class)
				.addClass(constants.class.editForm);
			var title = $('<h5></h5>');
			var finishButton = $('<button>')
				.addClass(constants.class.editFormFinishButon);
			var closeButton = $('<button>')
				.addClass(constants.class.editFormCloseButton);

			self.drawButtonIcon(finishButton, self.properties.form.finishButtonIcon, 'Finish');
			self.drawButtonIcon(closeButton, self.properties.form.closeButtonIcon, 'Close');
			title.html(dataItem == null ?
				(self.properties.form.addTitle === undefined ? 'Add item' : self.properties.form.addTitle) :
				(self.properties.form.editTitle === undefined ? 'Edit item' : self.properties.form.editTitle)
			);

			function createInput(drawingRule) {
				if (drawingRule.ignoreEdit === true) return;

				var container = $('<div></div>');
				container.addClass(constants.class.inputGroup);

				var label = $('<label></label>');
				//todo: here can be other things that input. Make a builder for this
				var input = $('<input>');
				input.attr('name', drawingRule.columnName);
				if (!(self.properties.form.textInputClass === undefined))
					input.addClass(self.properties.form.textInputClass);


				if (dataItem != null)
					input.val(dataItem[drawingRule.columnName]);

				label.text(drawingRule.displayName ? drawingRule.displayName : drawingRule.columnName);
				container.append(label);
				container.append(input);

				form.append(container);
			}

			finishButton.click(function () {
				event.preventDefault();
				function parseFormObject(formObject) {
					var item = {};
					for (var i = 0; i < formObject.length; i++) {
						item[formObject[i].name] = formObject[i].value;
					}
					return item;
				}
				var formObject = $(form).serializeArray();

				editAction({
					isValid: true,
					dataItem: parseFormObject(formObject)
				});
			});

			closeButton.click(function () {
				self.closeEditForm();
			});



			form.append(title);
			self.properties.rowDrawingRules.forEach(createInput);

			form.append(finishButton);
			form.append(closeButton);
			$(self.properties.editFormContainer).append(form);
		}

		if (!(self.properties.preOpenEditFormCallback === undefined))
			$.when(self.properties.preOpenEditFormCallback()).then(drawForm);
		else drawForm();
		//edit action parameter is an object with isValid representing if action is valid
		//and dataItem the updated item


	}

	closeEditForm() {
		var self = this;
		if (!(self.properties.postCloseEditFormCallback === undefined))
			$.when(self.properties.postCloseEditFormCallback()).then(function () {
				$(self.properties.editFormContainer).html('');
			});
		else $(self.properties.editFormContainer).html('');
	}
}

/** Sample of use:
 * TableDrawer drawer = new TableDrawer({
			container: containerId,							--container which contains the table
			title: 'table title',
			editFormContainer: editFormContainerId,			--container which contains the form which will be opened when a row is update/add
			id: 'id'										--id of the table
			class: 'class',									--class of table
			tableContainerClass: 'class',					--class of table container
		 	itemsPerPage: 10,								--items per page
			rowDrawingType: 'raw'/'function',				--rule for drawing a row: using internal 'raw' or by calling a function defined by caller
			rowDrawingFunction: function(dataItem) which returns <tr> item --function used to draw a row if type is 'function'
			commandHeader : {
				addButtonIcon : 'path',
				editButtonIcon: 'path',
				deleteButtonIcon: 'path'
			},
			rowDrawingRules: [{								-- used only if drawingType is 'raw' for drawing rows
			   drawingType: 'raw'/'function'
			   draw: 'label'/'button',
			   width: x (percent),
			   drawingFunction: function(columnValue) returns a <td>
			   class: 'class',
			   columnName: 'name',
			   displayName: 'name',
			   ignoreEdit: true/false
				}
			],

			deleteRowCallback: function(dataItem)	,				--callback which is called when 'delete' is pressed; return = { isValid:, message: }
			addRowCallback: function(dataItem ),    --callback which is called when add is pressed can return promise; return = { isValid:, message: }
			updateRowCallback: function(dataItem ), --callback which is called when update is pressed can return promise; return = { isValid:, message: }

			dataSourceCallback: function(pageIndex, itemsPerPage)	--a function which provides data for redrawing returns data or a promise to return data
			},

			form : {												--edit form properties
				class: 'class',
				closeButtonIcon: 'path',					--icon for close button
				finishButtonIcon: 'path',					--icon for finish button
				addTitle: 'string',							--title for add
				editTitle: 'string'							--title for edit
				textInputClass: 'string'
			},
			preOpenEditFormCallback: function();		--trigers befor edit form is opened
			postCloseEditFormCallback: function();		--trigers after the edit form callback is closed
			);
drawer.Draw(data, pagesCount);

*/