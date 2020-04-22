/*!Component: Table drawer
 *
 * Handles table drawing and basic operations (add,update,insert,pagination)
 *
 * */



/** Sample of use:
 * TableDrawer drawer = new TableDrawer({
			container: containerId,
			class: 'class',
			pagesCount: 10,
			rowDrawingFunction: function(dataItem) which returns <tr> item
			rowDrawingRules: [{
			   type: 'raw'/'function',
			   draw: 'label'/'button'
			   class: 'class',
			   itemName: 'name'
				}
			],

			deleteRowCallback: function(dataItem),
			addRowCallback: function(dataItem),
			updateRowCallback: function(dataItem),

			redrawCallback: function(pageIndex) which returns data
			}
			);
drawer.Draw(data);

*/