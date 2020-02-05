/*! Section: Points settings utilities
 *
 * This section defines general functions for points settings
 *
 * */

import { PopupBuilderInstance } from '../popup.js';
 
//handle the click for notification button
$('#notification_button').click(function (event) {
    var buttonPosition = $('#notification_button').offset();

    var content = document.createElement('small');
    content.innerText = "No notification available";

    PopupBuilderInstance.Create('map-container', { X: buttonPosition.left + 10, Y: buttonPosition.top + 30 }, content);    
     
});

//handle the click for selecting map type button
$('#map_type_button').click(function (event) {

    var buttonPosition = $('#map_type_button').offset();

    var content = document.createElement('small');
    content.innerText = "No map mode available";

    PopupBuilderInstance.Create('map-container', { X: buttonPosition.left + 10, Y: buttonPosition.top + 30 }, content);    
});

$('#map_criteria_button').click(function (event) {

    var buttonPosition = $('#map_criteria_button').offset();

    var content = document.createElement('small');
    content.innerText = "No display criteria available";

    PopupBuilderInstance.Create('map-container', { X: buttonPosition.left + 10, Y: buttonPosition.top + 30 }, content);
});