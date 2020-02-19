/*! Component: CardsManager
 *
 * This component is managing the cards (windows) inside the application
 *
 * */

import { CardDrawer } from './card_drawer.js';

class CardsManager {
    containerId = null;
    idIndex = 1;
    drawer = null;

    constructor(containerId) {
        this.drawer = new CardDrawer(this);
        this.containerId = containerId;
    }

    /**
     * Use this method to draw a card. 
     * @param {any} animate Specify if the card must be animated when it is drawed
     * @param {any} resizeEvent Specify a handler for popup resize event
     */
    Draw(animate, resizeEvent = null) { 
        var popupId = 'popup-window-' + this.idIndex;
        var resultId = '#popup-window-' + (this.idIndex++) + ' #window-body';
        this.drawer.Draw(popupId,
                         document.getElementById(this.containerId),
                         null,
                         this.erasePopup,
                         animate,
                         resizeEvent); 

        //delay for animation
        if(animate == true) setTimeout(function () {
            $('#' + popupId).removeClass('point-info-popup-hidden');
        }, 50);

        return resultId
    }

    erasePopup(id, manager) {
        var popupId = '#' + manager.containerId + ' #' + id;
        $(popupId).remove();
    }
}

export { CardsManager };