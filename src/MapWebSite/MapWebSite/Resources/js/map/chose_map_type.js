/*! Section: Map type selection
 *
 * This section handles functions and variables to change the map type
 *
 * */
import { SetMapType } from './map.js';

export var MapType = 'hybrid';

var selectedButtonId = 'chose_map_' + MapType;


export function RefreshSelectMapTypePopup() {

    $('#' + selectedButtonId).addClass('button-selected');

}


window.changeMapType = function changeMapType(button, selectedType) {
    MapType = selectedType;
    SetMapType(MapType);

    $('#' + selectedButtonId).removeClass('button-selected');

    button.classList.add('button-selected');
    selectedButtonId = button.id;

}