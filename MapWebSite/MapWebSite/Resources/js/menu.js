/*functions below are used to add interaction to the down-left menu*/

var currentMenuIconIndex = 0;
var menuIconsCount = 0;

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

async function requestSettingsPage(pageName) {
    await requestSettingsPageData();
    displayPage();
}


function displayPage() {    
    $('#color-picker-container').removeClass('color-picker-container-hide');
    $('#settings-layer').removeClass('settings-layer-hide');
    $('#main-menu').addClass('main-select-menu-nontransparent');
    $('#secondary-menu').addClass('secondary-menu-nontransparent');
}

async function requestSettingsPageData() {
    await $.get("/Login/Index", await function (data) {
       // alert(data);
    });
}