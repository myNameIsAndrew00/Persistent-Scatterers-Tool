/*! Section: Points settings utilities
 *
 * This section defines general functions for points settings (like dimension or position)
 *
 * */

import { PopupBuilderInstance } from '../popup.js';
import { UpdatePointsLayer } from '../map/map.js';

const sliderId = '#points_size_slider';
const sliderLabelId = '#points_size_slider_label';

var offset = 10;
var pointsDimensionScale = 30;

export function PointsDimensionScale() {
    return pointsDimensionScale + offset;
}

function changeDisplayedScaleValue() {
    $(sliderLabelId).text(pointsDimensionScale - offset);
}

export function InitialiseSlider() {
    $(sliderId).val(pointsDimensionScale - offset);
    changeDisplayedScaleValue();
}

window.ChangePointsSize = function () {
   
    pointsDimensionScale = parseInt($(sliderId).val()) + offset;
    changeDisplayedScaleValue();
}

window.ResetPointsSize = function () {
    UpdatePointsLayer();
}