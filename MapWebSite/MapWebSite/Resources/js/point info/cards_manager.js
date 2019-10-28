import { CardDrawer } from './card_drawer.js';

class CardsManager {
    containerId = null;
    idIndex = 1;
    drawer = null;

    constructor(containerId) {
        this.drawer = new CardDrawer(this);
        this.containerId = containerId;
    }

    Draw() { 

        this.drawer.Draw('popup-window-' + this.idIndex,
                         document.getElementById(this.containerId),
                         null,
                         this.erasePopup);
        this.idIndex++;
    }

    erasePopup(id, manager) {
        var popupId = '#' + manager.containerId + ' #' + id;
        $(popupId).remove();
    }
}

export { CardsManager };