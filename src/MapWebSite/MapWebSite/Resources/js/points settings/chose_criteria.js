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
    /*timeout is set to fix a problem with first selection (when the popup appeare first time)*/
    setTimeout(function () {
        $('#' + selectedButtonId).addClass('button-selected');
    }, 50);
}


window.changeVisualisationCriteria = function changeVisualisationCriteria(button, selectedCriteria) {
    SelectedCriteria = selectedCriteria;

    UpdatePointsLayer();

    $('#' + selectedButtonId).removeClass('button-selected');

    button.classList.add('button-selected');
    selectedButtonId = button.id;

}