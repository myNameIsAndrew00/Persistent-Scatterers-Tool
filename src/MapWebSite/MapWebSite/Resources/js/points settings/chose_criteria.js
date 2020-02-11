/*! Component: ChoseCriteria
 *
 * This component is responsable for chose dataset displaying criteria
 *
 * */

import { UpdatePointsLayer } from '../map/map.js';

//todo: bind this with enum
export var SelectedCriteria = 'Height';

var selectedButtonId = 'chose_criteria_' + SelectedCriteria;


export function RefreshSelectCriteriaPopup() {

    $('#' + selectedButtonId).addClass('button-selected');

}


window.changeVisualisationCriteria = function changeVisualisationCriteria(button, selectedCriteria) {
    SelectedCriteria = selectedCriteria;

    UpdatePointsLayer();

    $('#' + selectedButtonId).removeClass('button-selected');

    button.classList.add('button-selected');
    selectedButtonId = button.id;

}