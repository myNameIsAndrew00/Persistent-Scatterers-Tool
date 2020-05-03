/*! Section: Points source selection
 *
 * This section handles functions and variables to change the points source
 *
 * */

import { CassandraPointsSectionsContainer } from './points sources/cassandra_container.js';
import { GeoserverPointsSectionsContainer } from './points sources/geoserver_container.js';
import { map } from './map.js';
import { ChangeContextualMenuVisibility, Ids } from '../menu.js';

const sources = {
    cassandra: 'cassandra',
    geoserver: 'geoserver'
}


export var PointsSectionsContainer = null;
export var CurrentSource = sources.geoserver;
//id of the selected button ( top menu, change ChosePointsSource.cshtml)
var selectedButtonId = 'chose_points_source_' + CurrentSource;


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
        case sources.geoserver:
            PointsSectionsContainer = new GeoserverPointsSectionsContainer(map);
            break;
        case sources.cassandra:
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

    CurrentSource = sourceName;

    ChangeContextualMenuVisibility(Ids.buttons.customizable, CurrentSource != sources.geoserver);

}

window.ChangePointsSource = ChangePointsSource;
 

 