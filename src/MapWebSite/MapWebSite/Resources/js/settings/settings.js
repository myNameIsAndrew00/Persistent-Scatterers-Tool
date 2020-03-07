/*! Module: Settings
 *
 * This module handles a slider which contains features of application
 * 
 * */

import { ExpandMainMenu } from '../menu.js';

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
export function DisplayOverlay(message) {
    $('#settings-layer-container').children('#settings-layer-overlay').removeClass('message-overlay-hidden');
    $('#settings-layer-container').children('#settings-layer-overlay').html(message);
}

export function DisplayPage(display) {
    $('#settings-layer').html('');

    ExpandMainMenu(!display);

    function doAction(remove, id, className) {
        remove ? $(id).removeClass(className) : $(id).addClass(className);
    }
    doAction(display, '#settings-layer-container', 'settings-layer-container-hide');
    doAction(display, '#settings-layer', 'settings-layer-hide');
    doAction(!display, '#main-menu', 'main-select-menu-nontransparent');
    doAction(!display, '#secondary-menu', 'secondary-menu-nontransparent');
    doAction(!display, '#top-menu', 'top-menu-hiden');
}

window.DisplayPage = DisplayPage;

window.hideOverlay = function hideOverlay() {
    $('#settings-layer-container').children('#settings-layer-overlay').addClass('message-overlay-hidden');
    $('#settings-layer-container').children('#settings-layer-overlay').empty();
}