/*! Component: Points search
 *
 * This script contains code which handles searching on map  ( geolocation ), and renders the results on screen
 *
 * */

import { GeolocationRouter } from '../api/api_router.js';
import { GoTo } from './map.js';

function createInputLabel(text, iconUrl, latitude, longitude) {
    var outerDiv = document.createElement('div');

    var input = document.createElement('div');

    var label = document.createElement('label');
    var icon = document.createElement('img');

    var latLongLabel = document.createElement('label');

    label.innerHTML = text;
    latLongLabel.innerHTML = `latitude: ${latitude.toFixed(5)}, longitude: ${longitude.toFixed(5)}`; 
    latLongLabel.classList.add('info-label');
    icon.src = iconUrl;
    icon.alt = '|';

    input.classList.add('suggestions-item');
    input.appendChild(icon);
    input.appendChild(label);

    outerDiv.classList.add('outer-suggestions-item');
    outerDiv.onclick = function () { GoTo(latitude, longitude); }
    outerDiv.appendChild(input);
    outerDiv.appendChild(latLongLabel);
    return outerDiv;
}


export function LoadSuggestions(input, containerId) {
    GeolocationRouter.Get(input,
        function (locations) {
            for (var i = 0; i < locations.length; i++)
                $(containerId).append(createInputLabel(locations[i].display_name,
                    locations[i].icon,
                    parseFloat(locations[i].lat),
                    parseFloat(locations[i].lon)));
        }
    )
}