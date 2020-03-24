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
    //EVERY points container must provide methods:
    //1.A constructor which accepts target map as parameter
    //2.RemoveLayers: method for unbind provider from map
    //3.InitialiseMapInteraction: to provide interaction between points and user
    //4.LoadPoints: (required, optionally used -- a callback used when map view changes)
    //5.UpdatePointsLayer: (required, used by application features like change dataset or color palette)

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

    if (PointsSectionsContainer != null) PointsSectionsContainer.InitialiseMapInteraction();

    if (button != null) {
        $('#' + selectedButtonId).removeClass('button-selected');

        button.classList.add('button-selected');
        selectedButtonId = button.id;
    }

}

window.ChangePointsSource = ChangePointsSource;

 