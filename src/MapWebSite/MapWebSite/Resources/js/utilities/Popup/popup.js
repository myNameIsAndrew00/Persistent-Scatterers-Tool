/*!Component: Popup builder
 * 
 * Handles popup drawing
 * 
 * */

class Popup {

    constructor(id, position) {

        function buildContainer(divId) {
            var container = document.createElement('div');
            var indicatorWidth = 25;
            var indicatorHeight = 12;

            container.id = divId;
            container.classList.add('popup-initial');
            container.classList.add('popup');
            container.classList.add('box');
            container.style.top = position.Y + indicatorHeight + 'px';
            container.style.left = position.X - indicatorWidth + 'px';
            return container;
        }

        function buildInnerContainer() {
            var innerContainer = document.createElement('div');
            innerContainer.classList.add('popup-content');

            return innerContainer;
        }

        function buildCloseButton() {
            var innerButton = document.createElement('button');
            innerButton.classList.add('popup-close-button');
            innerButton.textContent = 'X';

            return innerButton;
        }

        this.id = id;

        this.container = buildContainer(this.id);
        this.closeButton = buildCloseButton();
        this.innerContainer = buildInnerContainer();

        this.container.appendChild(this.innerContainer);
        this.container.appendChild(this.closeButton);
    }

    Display(visible) {
        if (visible) this.container.classList.remove('popup-initial');
        else {
            this.SetContent(null);
            this.container.classList.add('popup-initial');
        }
    }

    SetContent(content) {
        if (content == null) { this.innerContainer.innerHTML = ''; return }

        this.innerContainer.appendChild(content);
    }

    SetCloseButtonHandle(functionHandle) {
        this.closeButton.onclick = functionHandle;
    }

}

class PopupManager {
    constructor() {
        this.popupIdentifier = 0;
    }

    RemoveAll(containerId = '') {
        $(('#' + containerId)).find('[id^="popup-"]').remove();
    }

    Create(containerId = '', position, content) {

        //create the popup, add it inside the container, set the close button handler
        var popupId = 'popup-' + this.popupIdentifier++;
        var popup = new Popup(popupId, position);
        popup.SetCloseButtonHandle(function () {
            popup.Display(false);
            $(('#' + containerId)).children('#' + popupId).remove();
        })

        $(('#'+ containerId)).append(popup.container);

        //display the popup
        setTimeout(() => {
            popup.Display(true);
            popup.SetContent(content);
        }, 50);


        //add a event listener to the window for closing the popup if a click ocured outside popup
        window.addEventListener('click', function (event) {
            if (!$(event.target).closest(".popup").length) {
                popup.Display(false);
                this.setTimeout(function () {
                    $(('#' + containerId)).children('#' + popupId).remove();
                },300);
            }
        })

        return popupId;
    }
}

export const PopupBuilderInstance = new PopupManager();