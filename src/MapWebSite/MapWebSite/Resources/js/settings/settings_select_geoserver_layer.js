
import { Router, endpoints } from '../api/api_router.js';
import { constants as settingsConstants, DisplayOverlay, HideOverlay } from './settings.js';
import { ListDrawer } from '../utilities/List/list_drawer.js';

const constants = {
    id : {
        name: '#name', 
        url: '#url',
        submitButton: '#geoserverSelectFinishButton', 
        defaultPaletteButton: '#selectDefaultPaletteButton'
    },
    class: {
        paletteListContainer: 'palette-list-container',
        paletteListContainerItem: 'palette-list-container-item',
        paletteListTable: 'palette-list-table'
    }

}


/**use this object to draw the palette list inside the modal*/
const paletteListContainerDrawer = {
    selectedPaletteUser: '',
    selectedPaletteName: '',
    listDrawer: function () {
        var self = this;

        return new ListDrawer({
            container: settingsConstants.id.settingsOverlay,
            class: constants.class.paletteListContainer,
            itemsPerPage: 10,
            onItemCreate: function (itemValue, itemIndex) {
                function drawListItem() {

                    function buildPaletteId(username, name) {
                        return username + '_' + name;
                    }

                    function appendInputs() {

                    }

                    function appendTitle() {
                        var title = $('<label></label>');
                        title.html('Geoserver ID: <b>' + buildPaletteId(itemValue.Item1,itemValue.Item2.Name));
                        
                        element.append(title);
                    }

                    function appendPalette() {
                        var palette = $('<div></div>');
                        palette.addClass(constants.class.paletteListTable);

                        for (var i = 0; i < itemValue.Item2.Intervals.length ; i++) {
                            var span = $('<span></span>');
                            span.css('background', itemValue.Item2.Intervals[i].Color);
                            palette.append(span);
                        }

                        element.append(palette);
                    }

                    var element = $('<div></div>');
                    element.addClass(constants.class.paletteListContainerItem);
                    element.on('click', function () {
                        self.selectedPaletteUser = itemValue.Item1;
                        self.selectedPaletteName = itemValue.Item2.Name;

                        $(constants.id.defaultPaletteButton).text(
                            buildPaletteId(self.selectedPaletteUser, self.selectedPaletteName));
                    });


                    appendTitle();
                    appendPalette();
                    appendInputs();

                    return element;
                }

                return drawListItem();
            },
            dataSourceCallback: function (pageIndex, itemsPerPage) {
                return new Promise(function (fullfill, reject) {
                    Router.Get(endpoints.PointsSettingsApi.GetColorPaletteList,
                        {
                            pageIndex: pageIndex
                        },
                        function (palette) {
                            fullfill(palette);
                        }
                    );
                });               
            },
            orientation: 'vertical'
        })
    },
    draw: function () {
        this.listDrawer().Draw();        
    }
}

/**
 * This function get triggered when finish button is pressed and send the information via the server API
 * */
window.uploadSelectedGeoserverLayer = function uploadSelectedGeoserverLayer() {
   
    Router.Post(endpoints.Settings.UploadGeoserverLayer,
        {
            name: $(constants.id.name).val(),
            apiUrl: $(constants.id.url).val(),
            defaultColorPaletteName: paletteListContainerDrawer.selectedPaletteName,
            defaultColorPaletteUser: paletteListContainerDrawer.selectedPaletteUser
        },
        function (serverResponse) {
            DisplayOverlay(serverResponse); 
        });

}

/**
 * This function get triggered when interaction with textboxes is detected
 * */
window.enableGeoserverUploadSubmit = function enableGeoserverUploadSubmit(){
   $(constants.id.submitButton).prop('disabled',
       $(constants.id.name).val() === '' || 
       $(constants.id.url).val() === '');
}

/**
 * This function open the modal for chosing color palette applied to dataset
 * */
window.openColorPaletteSelectModal = function openColorPaletteSelectModal() {

    paletteListContainerDrawer.draw();
    DisplayOverlay();
}
