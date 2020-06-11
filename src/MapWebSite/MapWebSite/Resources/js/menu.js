/*! Component: Menu
 *
 * Thie component handles the menu (application navigation) logic
 *
 * */

import { ChangeSpinnerVisibility, DisplayPage } from './settings/settings.js';
import { UpdateChosePaletteLayout } from './points settings/chose_palette.js';
import { UpdateSelectedDatasetLayout } from './points settings/chose_dataset.js';
import { RefreshSelectCriteriaPopup } from './points settings/chose_criteria.js';
import { RefreshSelectPointsSourcePopup } from './map/chose_points_source.js';
import { InitialiseSlider } from './points settings/chose_points_size.js';
import { LoadSuggestions } from './map/points_search.js';
import { RefreshSelectMapTypePopup } from './map/chose_map_type.js';
import { Router, endpoints } from './api/api_router.js';
import { PopupBuilderInstance } from './utilities/Popup/popup.js';
import { TooltipManagerInstance } from './utilities/Tooltip/tooltip_manager.js';
import { HtmlToElement } from './utilities/utils.js';
import { map } from './map/map.js';

/**************FUNCTIONS BELOW ARE USED TO ADD INTERACTION TO THE DOWN-LEFT MENU******************/
/*************************************************************************************************/

var currentMenuIconIndex = 0;
var menuIconsCount = 0;

export const Ids = {
    buttons : {
        customizable: '.customizable-buttons',
        general: '.general-buttons',
        search: '.search-button'       
    },
    labels : {
        mousePosition: 'cursor-position-labels'
    },
    controls: {
        mainSelectMenuIcon: '#main-select-menu-icon',
        mainMenu: '#main-menu',
        secondaryMenu: '#secondary-menu',
        fixedMenu: '#fixed-menu',
        topMenu: '#top-menu'
    }
}

const classes = {
    labels: {
        mousePosition: 'mouse-position'
    },
    controls: {
        topMenuHidden: 'top-menu-hiden'
    }
} 

/*control for pointer position display*/
var mousePositionControl = new ol.control.MousePosition({
    coordinateFormat: ol.coordinate.createStringXY(4),
    projection: 'EPSG:4326',
    target: document.getElementById(Ids.labels.mousePosition),
    className: classes.labels.mousePosition,
    undefinedHTML: '[lat], [long]'
});

var MenuHandler = {
    topMenuVisible: true,
    ExpandMainMenu: function (expand) {
        expand ? $(Ids.controls.fixedMenu).addClass('fixed-menu-expanded') : $(Ids.controls.fixedMenu).removeClass('fixed-menu-expanded');
        expand ? $(Ids.controls.mainMenu).addClass('main-select-menu-expanded') : $(Ids.controls.mainMenu).removeClass('main-select-menu-expanded');
        expand ? $(Ids.controls.secondaryMenu).addClass('secondary-menu-hidden') : $(Ids.controls.secondaryMenu).removeClass('secondary-menu-hidden')
    }, 
    /**
     * Change the visibility of the menus, taking in consideration internal state. If secondary parameter is not supplied, all menus are affected
     * @param {any} visibleMode the state which should be visible (if internal state is in concordance)
     * @param {any} menus the menus which must be affected, all if not provided
     */
    ChangeMenuMode: function (visibleMode, menus) {
        if (menus === undefined)
            menus = {
                top: true,
                main: true,
                secondary: true
            };

        function doAction(remove, id, className) {
            remove ? $(id).removeClass(className) : $(id).addClass(className);
        }

        if (menus.main === true) doAction(visibleMode, Ids.controls.mainMenu, 'main-select-menu-nontransparent');
        if (menus.secondary === true) doAction(visibleMode, Ids.controls.secondaryMenu, 'secondary-menu-nontransparent');
        if (menus.top === true) doAction(visibleMode && this.topMenuVisible, Ids.controls.topMenu, classes.controls.topMenuHidden);
    },
    /**
     * Change the top menu visibility. If display parameter is not provided, only internal state will be considered
     * @param {any} display
     */
    ChangeTopMenuVisibility: function () {
        const id = Ids.controls.topMenu;
        this.topMenuVisible = !this.topMenuVisible;

        !this.topMenuVisible ? $(id).addClass(classes.controls.topMenuHidden) : $(id).removeClass(classes.controls.topMenuHidden);
    }
}


window.onload = function () {
    map.addControl(mousePositionControl);

    menuIconsCount = $(Ids.controls.mainSelectMenuIcon).find('object').length;

    //hide every content which is not displayed
    for (var menuIndex = 1; menuIndex < menuIconsCount; menuIndex++) {
        var menuIcon = document.getElementById('main-menu-icon-' + menuIndex);

        var svgObject = menuIcon.contentDocument;

        var innerImage = svgObject.getElementById('inner_image');
        var innerText = svgObject.getElementById('inner_text');

        hideIcon(innerImage, innerText, null);
    }
};

/*use this function to change the menu content*/

window.changeMenuContent = function changeMenuContent(direction, display = false) {
    var currentMenuIcon = document.getElementById('main-menu-icon-' + currentMenuIconIndex);
    var currentMenuList = $('[id^="secondary-menu-items-' + currentMenuIconIndex + '"]'); 

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
            currentMenuList.removeClass("current_displayed_menu");

            changeMenuContent('none', true);
        }, 500);

    }

}

window.changeTopMenuVisibility = function () { MenuHandler.ChangeTopMenuVisibility() };

export function ExpandMainMenu(expand) { MenuHandler.ExpandMainMenu(expand) };

export function ChangeMenuMode(mode, menus) { MenuHandler.ChangeMenuMode(mode, menus) };

export function ChangeContextualMenuVisibility(menuId, visible) {
    visible == false ? $(menuId).addClass('buttons-group-hidden') : $(menuId).removeClass('buttons-group-hidden')  ;
}

function hideIcon(innerImage, innerText, currentMenuList) {
    /*reset elements transition timer*/
    /*animation for side menu*/
    if (currentMenuList != null) $(Ids.controls.secondaryMenu).css('margin','0 0 0 -190px');

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


    currentMenuList.addClass('current_displayed_menu');
    currentMenuIcon.classList.add("current_displayed_icon");

    setTimeout(function () {
        /*animation for side menu. Remove all the custom styles*/
        $(Ids.controls.secondaryMenu).removeAttr("style");

        /*animation for main menu*/
        innerImage.style.transform = 'translate(0, 0) rotate(0)';
        innerImage.style.opacity = '1';
        innerText.style.opacity = '1';
    }, 200);

}

/*****************************************************************************************/
/*functions used for interaction with setting overlay*/

window.requestSettingsPage = async function requestSettingsPage(pageIdentifier, cssServerPath) {
    ChangeSpinnerVisibility(true);

    setTimeout(DisplayPage, 50, true);

    await requestPage(pageIdentifier, cssServerPath);

}


async function requestPage(pageIdentifier, cssServerPath) {


    if (cssServerPath != null) requestCss(cssServerPath);
   

    Router.Get(endpoints.Home.RequestSettingsLayerContent,
        { settingsPage: pageIdentifier },
        await function (data) {
            setTimeout(function () {
                ChangeSpinnerVisibility(false);

                $('#settings-layer').html(data);
            }, 1200)
        }
    );

}

function requestCss(cssServerPath) {
    var head = document.getElementsByTagName('head')[0];
    for (var i = 0; i < head.children.length; i++)
        if (head.children[i].localName == 'link' && head.children[i].href.includes(cssServerPath)) return;

    var style = document.createElement('link');
    style.href = cssServerPath;
    style.type = 'text/css';
    style.rel = 'stylesheet';
    head.append(style);
}


/**************FUNCTIONS BELOW ARE USED TO ADD INTERACTION TO THE TOP-RIGHT MENU******************/
/*****************************************************************************************/

/*functions used for points setting overlay*/
window.displayPointsLayerPage = async function displayPointsLayerPage(display, requestMethodName, callback) {

    function displayPage(display, serverData) {
        var container = $('#points-settings-layer-container');
        var innerContainer = $('#points-settings-layer-container').children("#points-settings-layer-container-content");

        innerContainer.html(serverData);

        //if the request is for 'chose palette control' update the layout
        if (requestMethodName == 'GetColorPalettePage') UpdateChosePaletteLayout();
        //if the request is for 'chose dataset control' update the layout
        if (requestMethodName == 'GetChoseDatasetPage') UpdateSelectedDatasetLayout();

        setTimeout(function () {
            display ? container.removeClass('points-settings-layer-container-hidden') : container.addClass('points-settings-layer-container-hidden');
            display ? container.addClass('points-settings-layer-sizes') : container.removeClass('points-settings-layer-sizes');
         
        }, 10);
    }

    if (!display)
        displayPage(display, '');
    else
        Router.Get(endpoints.PointsSettings[requestMethodName],
            null,
            await function (data) {
                displayPage(false, '');
                setTimeout(function () { displayPage(true, data) }, 150);
            });

}

function displayPopup(buttonId, url, callbackHandler, additionalContent) {
    const buttonPosition = $('#' + buttonId).offset();
    const buttonWidth = parseInt($('#' + buttonId).width(), 10);

    function fillPopup(content) {
        PopupBuilderInstance.Create('#map-container',
            { X: buttonPosition.left + buttonWidth / 2, Y: buttonPosition.top + 30 },
            content);

        if (callbackHandler != null) callbackHandler();
    }

    //display content requested from server to fill the popup
    if (url != null)
        Router.Get(url,null,
            function (data) {

                fillPopup(HtmlToElement(data))

            });
    //or fill the popup with content from caller
    else fillPopup(additionalContent);
}

//todo: remove in all html files 'onclick' and add click handling in js (like in the example bellow)
$('#notification_button').click(function (event) {
    displayPopup('notification_button', endpoints.Miscellaneous.GetNotificationsPage, null, null);
});

///Functions bellow handles interactions with buttons

//handle the click for selecting map type button
$('#map_type_button').click(function (event) {
    displayPopup('map_type_button',
        endpoints.Miscellaneous.GetChoseMapTypePage,
        function () {
            /*timeout is set to fix a problem with first selection (when the popup appeare first time)*/
            setTimeout(function () {
                RefreshSelectMapTypePopup();
            }, 50);
        },
        null);
});


//handle the click for changing the criteria of selecting points color
$('#map_criteria_button').click(function (event) {
    displayPopup('map_criteria_button',
        endpoints.PointsSettings.GetChoseDisplayCriteriaPage,
        function () {
            /*timeout is set to fix a problem with first selection (when the popup appeare first time)*/
            setTimeout(function () {
                RefreshSelectCriteriaPopup();
            }, 50);
        },
        null);
});

//handle the click for resizing the displayed points
$('#map_resize_points_button').click(function (event) {
    displayPopup('map_resize_points_button',
        endpoints.Miscellaneous.GetChangePointsSizePage,
        function () {
            /*timeout is set to fix a problem with first selection (when the popup appeare first time)*/
            setTimeout(function () {
                InitialiseSlider();
            }, 50);
        },
        null);
});


$('#points_server_button').click(function (event) {
    displayPopup('points_server_button',
        endpoints.Miscellaneous.GetChosePointsSourcePage,
        function () {
            setTimeout(function () {
                RefreshSelectPointsSourcePopup();
            }, 50);
        },
        null);
});

//display the input for searching locations on map
$('#map_search_button').click(function (event) {
    $('#map_autocomplete').hasClass('autocomplete-hidden') ?
        $('#map_autocomplete').removeClass('autocomplete-hidden') :
        $('#map_autocomplete').addClass('autocomplete-hidden');
});


//handle the event when user search something on map
var bouncer = 0;
$('#map_search_text').keyup(function (event) {
    const localBouncer = ++bouncer;

    setTimeout(function () {
        if (localBouncer != bouncer) return;
        $('#map_suggestions').empty();
        LoadSuggestions($('#map_search_text').val(), '#map_suggestions');
    }, 300);
});

//reset the input for the search (above)
$('#reset_button').click(function (event) {
    $('#map_search_text').val('');
    $('#map_suggestions').empty();
})


/**************FUNCTIONS BELOW ARE USED TO ENABLE TOOLTIPS******************/
/*****************************************************************************************/

var displayedElement = document.createElement('small');
displayedElement.innerHTML = 'in development...';

TooltipManagerInstance.Register({
    containerId: 'map_type_button',
    delay: 2000,
    useRouter: true, 
    routerData:
    {
        endpoint: endpoints.Miscellaneous.GetGifTooltip,
        tooltipId: 0
    },
    cursorSide: 'left',
    displayOverlay: true
});

TooltipManagerInstance.Register({
    containerId: 'map_resize_points_button',
    delay: 2000,
    useRouter: true,
    routerData:
    {
        endpoint: endpoints.Miscellaneous.GetGifTooltip,
        tooltipId: 1
    },
    cursorSide: 'left',
    displayOverlay: true
});


TooltipManagerInstance.Register({
    containerId: 'map_criteria_button',
    delay: 2000,
    useRouter: true,
    routerData:
    {
        endpoint: endpoints.Miscellaneous.GetGifTooltip,
        tooltipId: 2
    },
    cursorSide: 'left',
    displayOverlay: true
});


TooltipManagerInstance.Register({
    containerId: 'map_search_button',
    delay: 2000,
    useRouter: true,
    routerData:
    {
        endpoint: endpoints.Miscellaneous.GetGifTooltip,
        tooltipId: 3
    },
    cursorSide: 'left',
    displayOverlay: true
});

TooltipManagerInstance.Register({
    containerId: 'points_server_button',
    delay: 2000,
    useRouter: true,
    routerData:
    {
        endpoint: endpoints.Miscellaneous.GetGifTooltip,
        tooltipId: 4
    },
    cursorSide: 'left',
    displayOverlay: true
});