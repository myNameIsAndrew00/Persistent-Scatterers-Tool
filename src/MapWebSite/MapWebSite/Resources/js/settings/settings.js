/*! Module: Settings
 *
 * This module handles a slider which contains features of application
 * 
 * */

import { ExpandMainMenu, ChangeMenuMode } from '../navigation.js';

export const constants = {
    id: {
        settingsLayerContainer: '#settings-layer-container',
        settingsOverlay: '#settings-layer-overlay',
        settingsLayer: '#settings-layer'
    }
}

/*functions used for spinner */
var rotateSpinner = false;

/*this functions rotate the settings page loading spinner */
function startSpinner() {
    if (rotateSpinner == false) return;
    //TODO: refactorize this code
    var spinnerIcon = document.getElementById('loading-icon');
    var inner_spin = spinnerIcon.contentDocument.getElementById('inner_circle');
    var middle_spin = spinnerIcon.contentDocument.getElementById('middle_circle');
    var outer_circle = spinnerIcon.contentDocument.getElementById('outer_circle');
    var globe = spinnerIcon.contentDocument.getElementById('Globe');

    var degrees = 0;
    var scaleValue = 0.801;
    var scaleDirection = 1;

    function spin() {

        inner_spin.style.transform = 'rotate(' + degrees * 2.4 + 'deg)';
        inner_spin.style.transformOrigin = 'center';

        middle_spin.style.transform = 'rotate(' + degrees * 1.8 + 'deg)';
        middle_spin.style.transformOrigin = 'center';

        outer_circle.style.transform = 'rotate(' + degrees * 1.2 + 'deg)';
        outer_circle.style.transformOrigin = 'center';

        globe.style.transform = 'scale(' + scaleValue + ')';
        globe.style.transformOrigin = 'center';

        scaleValue += scaleDirection * 0.0015;
        if (scaleValue >= 1 || scaleValue <= 0.8) scaleDirection = scaleDirection * -1;
        degrees = (degrees + 1) % 600;

        if (rotateSpinner) setTimeout(spin, 1);

    }
    spin();
}


export function ChangeSpinnerVisibility(visible) {

    if (visible)
        document.getElementById('loading-icon').classList.remove('loading-icon-hide');
    else document.getElementById('loading-icon').classList.add('loading-icon-hide');

    rotateSpinner = visible;
    startSpinner();
}

/*Use this function to display the overlay. The message must be in the right format, processed by server (check MessageBoxBuilder.cs)*/
export function DisplayOverlay(content) {
    $(document).keyup(function (event) {
        if (event.key == 'Escape') HideOverlay(false);
    });

    $(constants.id.settingsLayerContainer).children(constants.id.settingsOverlay).removeClass('message-overlay-hidden');

    if (!(content === undefined || content == null))
       $(constants.id.settingsLayerContainer).children(constants.id.settingsOverlay).html(content);
}


/**
 * Use this function to hide the settings overlay modal (todo: createa generic modal)
 * @param {any} closeSettingsPage set this parameter to true if closing settings page is required
 */
export function HideOverlay(closeSettingsPage) {
    $(constants.id.settingsLayerContainer).children(constants.id.settingsOverlay).addClass('message-overlay-hidden');
    $(constants.id.settingsLayerContainer).children(constants.id.settingsOverlay).empty();

    if (closeSettingsPage === true) DisplayPage(false);
}

window.hideOverlay = HideOverlay;

export function DisplayPage(display) {
    $(constants.id.settingsLayer).html('');

    function doAction(remove, id, className) {
        remove ? $(id).removeClass(className) : $(id).addClass(className);
    }

    doAction(display, constants.id.settingsLayerContainer, 'settings-layer-container-hide');
    doAction(display, constants.id.settingsLayer, 'settings-layer-hide');

    ExpandMainMenu(!display);
    ChangeMenuMode(!display);
}

window.DisplayPage = DisplayPage;
