/*! Section: Points source selection
 *
 * This section handles functions and variables to change the points source
 *
 * */

 

import { CassandraPointsSectionsContainer } from './points sources/cassandra_container.js';
import { GeoserverPointsSectionsContainer } from './points sources/geoserver_container.js';
import { map } from './map.js';

export var PointsSectionsContainer = null;

//id of the selected button ( top menu, change ChosePointsSource.cshtml)
var selectedButtonId = 'chose_points_source_cassandra';


export function RefreshSelectPointsSourcePopup() {

    $('#' + selectedButtonId).addClass('button-selected');

}


export function ChangePointsSource(button, sourceName) {
    if (PointsSectionsContainer != null) PointsSectionsContainer.RemoveLayers();

    switch (sourceName) {
        case 'geoserver':
            PointsSectionsContainer = new GeoserverPointsSectionsContainer(map);
            break;
        case 'cassandra':
            PointsSectionsContainer = new CassandraPointsSectionsContainer(map);
            break;
        default: break;
    }

    if (button != null) {
        $('#' + selectedButtonId).removeClass('button-selected');

        button.classList.add('button-selected');
        selectedButtonId = button.id;
    }

}

window.ChangePointsSource = ChangePointsSource;

 