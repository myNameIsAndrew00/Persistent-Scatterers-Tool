/*! Component: CardDrawer
 *
 * This component is used to create a card on GUI.
 * It is used as a helper for Cards Manager, but can be used standalone
 *
 * */

var currentZIndex = 100;

class CardDrawer {

    //todo: function which reset zIndex to 100 for all popups

    defaultDimensions = { oX: 400, oY: 260 };

    maxDimensions = { oX: 800, oY: 600 };

    ValidWidth(width) { return width > this.defaultDimensions.oX && width < this.maxDimensions.oX; }
    ValidHeight(height) { return height > this.defaultDimensions.oY && height < this.maxDimensions.oY; }

    constructor(manager) {
        this.manager = manager;
    }

    /**
     * Use this method to create and draw a window (card) on screen
     * @param {any} id Id of the card
     * @param {any} container The partent of the card
     * @param {any} info Info contained in the card
     * @param {any} closeHandler A handler which will be called when the window will be closed
     * @param {any} hidden This parameter can be used to disable the drawing of the window (on creation)
     * @param {any} resizeHandler A handler which will be called when the window will be resized
     */
    Draw(id, container, info, closeHandler, hidden, resizeHandler) {
        this.id = id;
        this.closeHandler = closeHandler; 

        var popup = this.drawPopup(hidden, resizeHandler);
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

        var minimiseSpan = document.createElement('span');
        minimiseSpan.classList.add('minimise-span');
        //todo: add handling for minimise

        var closeHandler = this.closeHandler;
        var id = this.id;
        var manager = this.manager;

        closeSpan.onclick = function (e) {
            closeHandler(id, manager);
        };

        header.appendChild(closeSpan);
        header.appendChild(minimiseSpan);
        popup.appendChild(header);
    }

    drawPopup(hidden, resizeHandler) {
        var self = this;
        var popup = document.createElement('div');

        popup.classList.add('point-info-popup');
        popup.classList.add('box');
        popup.style.zIndex = currentZIndex++;

        if (hidden == true) popup.classList.add('point-info-popup-hidden');

        function getPopupWidth() {
            return popup.style.width.replace(/\D/g, '');
        }

        function getPopupHeight() {
            return popup.style.height.replace(/\D/g, '');
        }


        function initialiseResizers() {
            function createResizer(positionClass, sign) {
                var resizer = document.createElement('div');
                resizer.classList.add('resizer');
                resizer.classList.add(positionClass);

                var initialWidth = 0;
                var initialHeight = 0;
                var initialX = 0;
                var initialY = 0;
                var initialMouseX = 0;
                var initialMouseY = 0;

                resizer.addEventListener('mousedown', function (e) {
                    e.preventDefault()
                    e.stopPropagation();

                    const initialTransition = popup.style.transition;
                    popup.style.transition = '0s';


                    initialWidth = parseFloat(getPopupWidth());
                    initialHeight = parseFloat(getPopupHeight());
                    initialX = popup.getBoundingClientRect().left;
                    initialY = popup.getBoundingClientRect().top;
                    initialMouseX = e.pageX;
                    initialMouseY = e.pageY;

                    window.addEventListener('mousemove', resize)
                    window.addEventListener('mouseup', function (e) { stopResize(initialTransition) });
                });

                function resize(e) {
                    const width = initialWidth + sign.width * (e.pageX - initialMouseX);
                    const height = initialHeight + sign.height * (e.pageY - initialMouseY)

                    if (self.ValidWidth(width)) popup.style.width = width + 'px';
                    if (self.ValidHeight(height)) popup.style.height = height + 'px';

                    if (sign.width == -1 && self.ValidWidth(width))
                        popup.style.left = initialX + (e.pageX - initialMouseX) + 'px';
                    if (sign.height == -1 && self.ValidHeight(height))
                        popup.style.top = initialY + (e.pageY - initialMouseY) + 'px';

                    if (resizeHandler != null) resizeHandler();
                }

                function stopResize(initialTransition) {
                    window.removeEventListener('mousemove', resize);
                    popup.style.transition = initialTransition;
                }

                return resizer;
            }

            var resizers = [
                function () {
                    return createResizer('top-left-resizer', { width: -1, height: -1 });
                },
                function () {
                    return createResizer('top-right-resizer', { width: 1, height: -1 });
                },
                function () {
                    return createResizer('bottom-left-resizer', { width: -1, height: 1 });
                },
                function () {
                    return createResizer('bottom-right-resizer', { width: 1, height: 1 });
                },
            ]

            for (var i = 0; i < resizers.length; i++)
                popup.appendChild(resizers[i]());
        }

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

            popup.onmousemove = function (e) {
                if (document.body.style.cursor != 'move') document.body.style.cursor = 'initial';
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
                document.onmousemove = popupMoveHandler;


                function resetHandlers() {
                    document.body.style.cursor = 'initial';

                    document.onmouseup = null;
                    document.onmousemove = null;
                };

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
        initialiseResizers();
        initialiseEvents();

        return popup;
    }

    addData(popup, info) {
        var content = document.createElement('div');
        content.id = 'window-body';
        content.classList.add('popup-body');

        popup.appendChild(content);
    }

}

export { CardDrawer };