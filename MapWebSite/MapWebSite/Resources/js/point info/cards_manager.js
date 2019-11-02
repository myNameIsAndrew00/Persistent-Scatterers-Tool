import { CardDrawer } from './card_drawer.js';

class CardsManager {
    containerId = null;
    idIndex = 1;
    drawer = null;

    constructor(containerId) {
        this.drawer = new CardDrawer(this);
        this.containerId = containerId;
    }

    Draw(animate) { 
        var popupId = 'popup-window-' + this.idIndex;
        this.drawer.Draw(popupId,
                         document.getElementById(this.containerId),
                         null,
                         this.erasePopup,
                         animate); 

        //delay for animation
        if(animate == true) setTimeout(function () {
            $('#' + popupId).removeClass('point-info-popup-hidden');
        }, 50);

        return '#popup-window-' + (this.idIndex++) + ' #window-body';;
    }

    erasePopup(id, manager) {
        var popupId = '#' + manager.containerId + ' #' + id;
        $(popupId).remove();
    }
}

export { CardsManager };