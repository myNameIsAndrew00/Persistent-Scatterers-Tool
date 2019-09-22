import { SetColorPalette, colorPalette } from '../home.js';
import { UpdatePointsLayer } from '../map.js';

var __selected_palette_index = -1;


function changeSelectedRowOnMenu(id, visible) {
    var paletteRow = $('#points-settings-layer-container-content').find(id)[0];
    if (paletteRow === undefined) return;
    visible ? paletteRow.classList.add('selected-row') : paletteRow.classList.remove('selected-row');
}

window.useColorMap = async function useColorMap(paletteIndex, username, paletteName) {


    await $.get("/api/PointsSettingsApi/GetColorPalette", { username: username, paletteName: paletteName }, await function (palette) {
        SetColorPalette(palette);
        UpdatePointsLayer();

        var previousPaletteId = '#user_palette_index_' + __selected_palette_index;
        var paletteId = '#user_palette_index_' + paletteIndex;

        changeSelectedRowOnMenu(previousPaletteId, false);
        changeSelectedRowOnMenu(paletteId, true);

        __selected_palette_index = paletteIndex;
    });
}

/* ***************************************************************************************/
/*palettes request to display on table*/

function resetTable(table) {
    $('#points-settings-layer-container-content').find('#currentColorPaletteIndex')[0].value = 0;
    var tableHeader = table.children[0].children[0].children[0];

    table.children[0].children[0].innerText = '';
    table.children[0].children[0].innerHtml = '';

    table.children[0].children[0].appendChild(tableHeader);
}

function fillTable(colorPalettes, table) {


    function buildColorsTable(colors) {
        var colorsTable = document.createElement('table');
        var colorsTableBody = document.createElement('tbody');
        var colorsPerRow = $('#points-settings-layer-container-content').find('#colorsTableColorsPerRow')[0].value;
        var intervalsIndex = 0;

        colorsTable.classList.add('palette');

        var rowsCount = colors.length / colorsPerRow;

        for (var _index = 0; _index <= rowsCount; _index++) {
            var colorsTableRow = document.createElement('tr');
            for (var _rowIndex = 0; _rowIndex < colorsPerRow && intervalsIndex < colors.length; _rowIndex++ , intervalsIndex++) {
                var column = document.createElement('td');
                column.style.background = colors[intervalsIndex].Color;
                colorsTableRow.appendChild(column);
            }
            colorsTableBody.appendChild(colorsTableRow);
        }

        colorsTable.appendChild(colorsTableBody);
        return colorsTable;
    }

    function buildButtonsColumn(paletteUserName, paletteName) {
        var useButton = document.createElement('button');
        useButton.classList.add('use');
        useButton.onclick = function () { useColorMap(paletteUserName + '_' + paletteName, paletteUserName, paletteName); };
        useButton.innerText = 'Use';

        var previewButton = document.createElement('button');
        previewButton.classList.add('preview');
        previewButton.innerText = 'Preview';

        return { useButton, previewButton };
    }

    for (var index = 0; index < colorPalettes.length; index++) {
        var colorPalette = colorPalettes[index];

        var row = document.createElement('tr');
        row.id = 'user_palette_index_' + colorPalette.Item1 + '_' + colorPalette.Item2.Name;

        var userNameColumn = document.createElement('td');
        userNameColumn.innerText = colorPalette.Item1;

        var paletteNameColumn = document.createElement('td');
        paletteNameColumn.innerText = colorPalette.Item2.Name;

        var colorsTableColumn = document.createElement('td');
        colorsTableColumn.appendChild(buildColorsTable(colorPalette.Item2.Intervals));

        var buttonsColumn = document.createElement('td');
        var buttons = buildButtonsColumn(colorPalette.Item1, colorPalette.Item2.Name);
        buttonsColumn.appendChild(buttons.useButton);
        buttonsColumn.appendChild(buttons.previewButton);

        /*append the new row to the table*/
        row.appendChild(userNameColumn);
        row.appendChild(paletteNameColumn);
        row.appendChild(colorsTableColumn);
        row.appendChild(buttonsColumn);

        /*hover the row if the color palette is in use*/
        if (__selected_palette_index === colorPalette.Item1 + '_' + colorPalette.Item2.Name)
            row.classList.add('selected-row');

        table.appendChild(row);
    }

}

window.loadMorePalettes = function loadMorePalettes(resetPageIndex) {
    var table = $('#points-settings-layer-container-content').find('#ps_left')[0];
    if (resetPageIndex) resetTable(table);

    if (table.offsetHeight + table.scrollTop == table.scrollHeight) {
        var filterValue = $('#points-settings-layer-container-content').find('#colorPaletteSearchValue')[0];
        var pageIndex = $('#points-settings-layer-container-content').find('#currentColorPaletteIndex')[0];
        var filter = $('#points-settings-layer-container-content').find('#colorPaletteFilterValue')[0];

        $.get("/api/PointsSettingsApi/GetColorPaletteList", { filterValue: filterValue.value, filter: filter[filter.selectedIndex].value, pageIndex: pageIndex.value },
            function (palette) {
                if (palette.length)
                    $('#points-settings-layer-container-content').find('#currentColorPaletteIndex')[0].value = parseInt(pageIndex.value) + 1;

                //fill the table tbody with rows
                fillTable(palette, table.children[0].children[0]);
            });
    }
}


export function UpdateChosePaletteLayout() {
    changeSelectedRowOnMenu('#user_palette_index_' + __selected_palette_index, true);
}