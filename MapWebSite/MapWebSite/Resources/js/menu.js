/*functions below are used to add interaction to the down-left menu*/

var currentMenuIconIndex = 0;
var menuIconsCount = 0;
var rotateSpinner = false;

window.onload = function () {
    menuIconsCount = $('#main-select-menu-icon').find('object').length;
};

/*use this function to change the menu content*/

function changeMenuContent(direction, display = false) {
    var currentMenuIcon = document.getElementById('main-menu-icon-' + currentMenuIconIndex);
    var currentMenuList = document.getElementById('secondary-menu-items-' + currentMenuIconIndex);

    var svgObject = currentMenuIcon.contentDocument;

    var innerImage = svgObject.getElementById('inner_image');
    var innerText = svgObject.getElementById('inner_text');


    /*first call will be with display = false. This means that the index will be changed and the current icon will be 'deleted'
     * another call will be made to show the index, with display = true*/
    if (display) {
        showIcon(innerImage, innerText, currentMenuIcon, currentMenuList);
    }
    else {
        hideIcon(innerImage, innerText, currentMenuList);

        currentMenuIconIndex = (currentMenuIconIndex + (direction == 'up' ? 1 : - 1)) % menuIconsCount;
        if (currentMenuIconIndex == -1) currentMenuIconIndex += menuIconsCount;

        setTimeout(function () {
            currentMenuIcon.classList.remove("current_displayed_icon");
            currentMenuList.classList.remove("current_displayed_menu");

            changeMenuContent('none', true);
        }, 500);

    }

}

function hideIcon(innerImage, innerText, currentMenuList) {
    /*reset elements transition timer*/
    /*animation for side menu*/
    currentMenuList.style.margin = '0 0 0 -50px';

    /*animation for main menu*/
    innerImage.style.transition = '0.5s';
    innerImage.style.WebkitTransition = '0.5s';
    innerText.style.WebkitTransition = '0.15s linear';

    innerImage.style.transform = 'translate(-30px, -50px) rotate(25deg)'
    innerImage.style.opacity = '0';

    innerText.style.opacity = '0';
}

function showIcon(innerImage, innerText, currentMenuIcon, currentMenuList) {
    hideIcon(innerImage, innerText, currentMenuList);


    currentMenuList.classList.add('current_displayed_menu');
    currentMenuIcon.classList.add("current_displayed_icon");

    setTimeout(function () {
        /*animation for side menu. Remove all the custom styles*/
        currentMenuList.style = "";

        /*animation for main menu*/
        innerImage.style.transform = 'translate(0, 0) rotate(0)';
        innerImage.style.opacity = '1';
        innerText.style.opacity = '1';
    }, 200);

}

/*****************************************************************************************/

/*functions used for setting overlay*/

async function requestSettingsPage(pageName, cssServerPath) {
    ChangeSpinnerVisibility(true);
    

    setTimeout( displayPage, 50, true); 
     
    await requestPage(pageName, cssServerPath);
     
}


function displayPage(display) {    
    $('#settings-layer').html('');

    function doAction(remove, id, className) {
        remove ? $(id).removeClass(className) : $(id).addClass(className);
    }
    doAction(display, '#settings-layer-container', 'settings-layer-container-hide');
    doAction(display, '#settings-layer', 'settings-layer-hide');
    doAction(!display, '#main-menu', 'main-select-menu-nontransparent');
    doAction(!display, '#secondary-menu', 'secondary-menu-nontransparent');
    doAction(!display, '#top-menu', 'top-menu-hiden');
}

async function requestPage(pageName, cssServerPath) {


    if(cssServerPath != null) requestCss(cssServerPath);
    await $.get("/Home/RequestSettingsLayerContent", { settingsPageName: pageName }, await function (data) {
        //TODO: check if the page was already loaded and do not request resources again. It generates errors
        setTimeout(function () {
            ChangeSpinnerVisibility(false);
            $('#settings-layer').html(data);  
        }, 1200);
               
    });
}

function requestCss(cssServerPath) {
    var head = document.getElementsByTagName('head')[0];
    for (var i = 0; i < head.children.length; i++)
        if (head.children[ i ].localName == 'link' && head.children[i].href.includes(cssServerPath)) return;

    var style = document.createElement('link');
    style.href = cssServerPath;
    style.type = 'text/css';
    style.rel = 'stylesheet';
    head.append(style);
}


/*****************************************************************************************/

/*functions used for spinner */

/*this functions rotate the settings page loading spinner */
function startSpinner() {
    if (rotateSpinner == false) return;
    //TODO: refactorize this code
    var spinnerIcon = document.getElementById('loading-icon');
    var inner_spin = spinnerIcon.contentDocument.getElementById('inner_circle');
    var middle_spin = spinnerIcon.contentDocument.getElementById('middle_circle');
    var outer_circle = spinnerIcon.contentDocument.getElementById('outer_circle');
    var globe = spinnerIcon.contentDocument.getElementById('Globe');

    var degrees = 0;
    var scaleValue = 0.801;
    var scaleDirection = 1;
     
    function spin() {

        inner_spin.style.transform = 'rotate(' + degrees * 2.4 + 'deg)';
        inner_spin.style.transformOrigin = 'center';

        middle_spin.style.transform = 'rotate(' + degrees * 1.8 + 'deg)';
        middle_spin.style.transformOrigin = 'center';

        outer_circle.style.transform = 'rotate(' + degrees * 1.2 + 'deg)';
        outer_circle.style.transformOrigin = 'center';

        globe.style.transform = 'scale(' + scaleValue + ')';
        globe.style.transformOrigin = 'center';

        scaleValue += scaleDirection * 0.0015;
        if (scaleValue >= 1 || scaleValue <= 0.8) scaleDirection = scaleDirection * -1;
        degrees = (degrees + 1) % 600;

        if (rotateSpinner) setTimeout(spin, 1);

    }
    spin();
}


function ChangeSpinnerVisibility(visible) {

    if (visible)
        document.getElementById('loading-icon').classList.remove('loading-icon-hide');
    else document.getElementById('loading-icon').classList.add('loading-icon-hide');

    rotateSpinner = visible;
    startSpinner();
}