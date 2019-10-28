var currentZIndex = 100;

class CardDrawer {

    //todo: function which reset zIndex to 100 for all popups

    defaultDimensions = { oX: 400, oY: 250 };

    maxDimensions = { oX: 600, oY: 400 };

    constructor(manager) {
        this.manager = manager;
    }

    Draw(id, container, info, closeHandler) {
        this.id = id;
        this.closeHandler = closeHandler;


        var popup = this.drawPopup();
        this.drawPopupHeader(popup);

        container.appendChild(popup);

        popup.id = id;

        this.addData(popup, info);
    }



    /*internals methods*/

    drawPopupHeader(popup) {
        var header = document.createElement('div');
        header.classList.add('popup-header');

        var closeSpan = document.createElement('span');
        closeSpan.classList.add('close-span');

        var closeHandler = this.closeHandler;
        var id = this.id;
        var manager = this.manager;

        closeSpan.onclick = function (e) {
            closeHandler(id, manager);
        };

        header.appendChild(closeSpan);
        popup.appendChild(header);
    }

    drawPopup() {
        var popup = document.createElement('div');
        popup.classList.add('point-info-popup');

        function initialisePosition(defaultDimensions) {
            /*center the intial position*/
            popup.style.left = '200px';
            popup.style.top = '100px';

        }

        function initialiseDimensions(defaultDimensions) {
            popup.style.height = defaultDimensions.oY + 'px';
            popup.style.width = defaultDimensions.oX + 'px';
        }

        function initialiseEvents() {

            const margin = 0;
            var currentPositionX = 0,
                currentPositionY = 0,
                initialPositionX = 0,
                initialPositionY = 0;
            var resizingStates = {
                left: 'none',
                right: 'none',
                top: 'none',
                bottom: 'none',

                disableAll: function () {
                    this.left = 'none'; this.right = 'none'; this.bottom = 'none'; this.top = 'none';
                },

                enabled: function () {
                    return this.left == 'resizing' || this.right == 'resizing' || this.top == 'resizing' || this.bottom == 'resizing';
                },

                enabledX: function () {
                    return this.left == 'resizing' || this.right == 'resizing';
                },
                enabledY: function () {
                    return this.top == 'resizing' || this.bottom == 'resizing';
                },
                switchStates: function (state, args) {
                    switch (state) {
                        case 'resizing': this[args] = 'resizing';
                            break;
                    }
                }

            };

            function getPopupWidth() {
                return popup.style.width.replace(/\D/g, '');
            }

            function getBorderWidth() {
                return popup.style.borderWidth.replace(/\D/g, '');
            }

            function getPopupHeight() {
                return popup.style.height.replace(/\D/g, '');
            }


            popup.onmousemove = function (e) {
                resizingStates.disableAll();
                if (document.body.style.cursor != 'move') document.body.style.cursor = 'initial';

                if (e.offsetX <= getBorderWidth() + 2) resizingStates.switchStates('resizing', 'left')
                if (e.offsetX >= getPopupWidth() - getBorderWidth() - 2) resizingStates.switchStates('resizing', 'right');
                if (e.offsetY <= getBorderWidth() + 2) resizingStates.switchStates('resizing', 'top');
                if (e.offsetY >= getPopupHeight() - getBorderWidth() - 2) resizingStates.switchStates('resizing', 'bottom');

                if (resizingStates.enabledX())
                    document.body.style.cursor = 'e-resize';
                if (resizingStates.enabledY())
                    document.body.style.cursor = 'n-resize';

            }

            popup.onmousedown = function (e) {
                popup.style.zIndex = currentZIndex++;
                e = e || window.event;
                e.preventDefault();
                // get the mouse cursor position at startup:
                initialPositionX = e.clientX;
                initialPositionY = e.clientY;

                document.onmouseup = resetHandlers;
                // call a function whenever the cursor moves:
                if (resizingStates.enabled())
                    document.onmousemove = popupResizeHandler;
                else
                    document.onmousemove = popupMoveHandler;


                function resetHandlers() {
                    document.body.style.cursor = 'initial';

                    document.onmouseup = null;
                    document.onmousemove = null;
                };

                function popupResizeHandler(e) {
                    console.log('Popup resize not implemented');
                }

                function popupMoveHandler(e) {
                    e = e || window.event;

                    e.preventDefault();
                    document.body.style.cursor = 'move';

                    // calculate the new cursor position:
                    currentPositionX = initialPositionX - e.clientX;
                    currentPositionY = initialPositionY - e.clientY;
                    initialPositionX = e.clientX;
                    initialPositionY = e.clientY;

                    // set the element's new position:
                    var popupPositionLeft = (parseInt(popup.style.left) - currentPositionX);
                    var popupPositionTop = (parseInt(popup.style.top) - currentPositionY);

                    if (popupPositionLeft + parseInt(getPopupWidth()) > screen.width - margin
                        || popupPositionLeft < margin
                        || popupPositionTop + parseInt(getPopupHeight()) > screen.height - margin
                        || popupPositionTop < margin)
                        return;

                    popup.style.top = popupPositionTop + "px";
                    popup.style.left = popupPositionLeft + "px";
                }
            };


        }

        initialisePosition(this.defaultDimensions);
        initialiseDimensions(this.defaultDimensions);
        initialiseEvents();

        return popup;
    }

    addData(popup, info) {

    }

}

export { CardDrawer };