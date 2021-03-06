﻿/*! Component: PointInfo
 *
 * This component is used for handling the point info slider actions and information 
 *
 * */

import { PlotDrawer } from '../utilities/Plot/plot.js';
import { CardsManager } from '../utilities/Card/cards_manager.js';
import { PopupBuilderInstance } from '../utilities/Popup/popup.js'; 
import { HtmlToElement } from '../utilities/utils.js';

import { UnselectFeatureOnMap } from '../map/map.js';
import { TooltipManagerInstance } from '../utilities/Tooltip/tooltip_manager.js';
import { endpoints } from '../api/api_router.js';
import { ChangeMenuMode } from '../navigation.js';

var cardsManager = new CardsManager('map-container');
var drawer = new PlotDrawer();
var mainContext = null;

var card = {
    context: null,
    contentId: ''
}

var currentDisplayedPoint = null;

const constants = {
    ids: {
        plot: '#plot',
        plotContainer: '#popup-plot-container',
        pointInfo: '#_point-info'
    },
    strings: {
        plotOxLabel: 'Time [days]',       
        plotOyLabel: 'Deformation [mm]'
    }
}

/**
 * Use this function to set the displayed information fields
 * @param {any} parameters  fields which must be set
 */
export function SetPointDetailsInfo(parameters) {
    var pointLayer = $('#_point-info');

    if (parameters == null || parameters === undefined)
        parameters = {
            number: 'N/A',
            longitude: 'N/A',
            latitude: 'N/A',
            height: 'N/A',
            deformation: 'N/A',
            standarddeviation: 'N/A',
            estimatedheight: 'N/A',
            estimateddeformation: 'N/A',
        }

    pointLayer.find("#ID").html(parameters.number);
    pointLayer.find("#longitude").html(parameters.longitude);
    pointLayer.find("#latitude").html(parameters.latitude);
    pointLayer.find("#height").html(parameters.height);
    pointLayer.find("#def_rate").html(parameters.deformation);
    pointLayer.find("#std_dev").html(parameters.standarddeviation);
    pointLayer.find("#est_height").html(parameters.estimatedheight);
    pointLayer.find("#est_def_rate").html(parameters.estimateddeformation);
}

export function DisplayPointInfo() {
    function display() {
        document.getElementById("point-info").style = "";   
        $("#point-info").css("opacity", 1);
        $("#point-info").css("width", "60%");
        $("#point-info").css("visibility", "initial");    
        $("#top-menu").addClass('top-menu-hiden');
    }
    if (document.getElementById("point-info").style.visibility == "initial") {
        HidePointInfo(false);
        setTimeout(function () { display() }, 200);
    }
    else display();
  
}

export function SetPointInfoData(point) {

    var pointLayer = $('#_point-info');
    var plotUnavailable = point.Displacements == null;
    if (plotUnavailable) {
        pointLayer.find("#no-plot-text").css("display", "block");
        pointLayer.find("#plot").css("display", "none");
        pointLayer.find("#plot-menu").css("display", "none");
    }
    else {
        var points = [];

        for (var index = 0; index < point.Displacements.length; index++)
            points[index] = {
                X: point.Displacements[index].DaysFromReference,
                Y: point.Displacements[index].Value
            };

        var oXRight = Math.max.apply(Math, points.map(function (o) {
            return o.X;
        }));

        var oXLeft = Math.min.apply(Math, points.map(function (o) {
            return o.X;
        }));

        var oYTop = Math.max.apply(Math, points.map(function (o) {
            return o.Y;
        }));

        var oYBottom = Math.min.apply(Math, points.map(function (o) {
            return o.Y;
        }));

        drawPlot(points, oXLeft, oXRight, oYBottom, oYTop);

        pointLayer.find("#no-plot-text").css("display", "none");
        pointLayer.find("#plot").css("display", "block");
        pointLayer.find("#plot-menu").css("display", "block");
    }
    currentDisplayedPoint = point;

    SetPointDetailsInfo({
        number: currentDisplayedPoint.Number,
        longitude: currentDisplayedPoint.Longitude,
        latitude: currentDisplayedPoint.Latitude,
        height: currentDisplayedPoint.Height,
        deformation: currentDisplayedPoint.DeformationRate,
        standarddeviation: currentDisplayedPoint.StandardDeviation,
        estimatedheight: currentDisplayedPoint.EstimatedHeight,
        estimateddeformation: currentDisplayedPoint.EstimatedDeformationRate,
    }); 
}






export function HidePointInfo(showTopMenu) {
    ChangeMenuMode(showTopMenu, { top: true });

    SetPointDetailsInfo(null);

    $("#point-info").css("opacity", 0);
    $("#point-info").css("width", "40%"); 
    $("#point-info").css("visibility", "hidden");  

    if(currentDisplayedPoint != null)
         UnselectFeatureOnMap(currentDisplayedPoint.Number);
}

export function AppendToPopupWindow() {
    card.context.points.push({
        data: mainContext.points[0].data,
        color: 'red',
        hoverColor: 'darkRed'
    });

    drawer.Draw(card.context,
        {
            erasePreviousPlot: false
        });
}

export function CreatePopupWindow() {

    const minPlotDimensions = { height: 250, width: 400 };

    //use the last drawer created when a point was clicked on map
    var cardOptions = { ...mainContext };
  
    //when the popup is resized, the plot must be redrawed
    card.contentId = cardsManager.Draw(true, function () {  
        drawPlot();
    });
     

    function drawPlot() {       
        function appendContainer() {
            var divContainer = $('<div></div>');
            divContainer.attr('id', 'popup-plot-container');
            divContainer.addClass('popup-plot-container');
            $(card.contentId).append(divContainer);
        }

        appendContainer();

        const height = Math.max(minPlotDimensions.height, $(card.contentId).height());
        const width = Math.max(minPlotDimensions.width, $(card.contentId).width());

        cardOptions.graphColor = 'black';
        cardOptions.length = {
            oX: width - 155,
            oY: height - 80
        };
        cardOptions.viewBoxPadding = 10;
        cardOptions.containerID = card.contentId + ' ' + constants.ids.plotContainer;

        card.context = drawer.CreateContext(cardOptions);
        drawer.Draw(card.context);                 
    }

    function drawText() {
        var textContent = document.createElement('div');
        textContent.classList.add('popup-data-container');
        var rowsLength = 5;
        var rows = [];
        for (var i = 0; i < rowsLength; i++) {
            rows[i] = {
                header: document.createElement('p'),
                label: document.createElement('label')
            };
        }

        rows[0].header.innerText = 'Latitude';
        rows[0].label.innerText = '~ ' + currentDisplayedPoint.Latitude.toFixed(6);
        rows[1].header.innerText = 'Longitude';
        rows[1].label.innerText = '~ ' + currentDisplayedPoint.Longitude.toFixed(6);
        rows[2].header.innerText = 'Height';
        rows[2].label.innerText = '~ ' + currentDisplayedPoint.Height.toFixed(6);
        rows[3].header.innerText = 'Deformation rate';
        rows[3].label.innerText = '~ ' + currentDisplayedPoint.DeformationRate.toFixed(6);
        rows[4].header.innerText = 'Std.dev. rate';
        rows[4].label.innerText = '~ ' + currentDisplayedPoint.StandardDeviation.toFixed(6);

        for (var i = 0; i < rowsLength; i++) {
            textContent.append(rows[i].header);
            textContent.append(rows[i].label);
        }

        $(card.contentId).append(textContent);
    }
     
    //delay for animation
    setTimeout(function () {
        drawPlot();
        drawText();
    }, 250);

}

export function ChangePlotType(plotType) {
    if (drawer == null) return;

    mainContext.plotType = plotType;
    drawer.Draw(mainContext);
}

export function DrawRegressionFunction(regressionType) {
    function buildCoefficientsList(values) {
        var list = '<ul>';
        for (var i = 0; i < values.length; i++)
            list = list + `<li><small>${values[i]}</small></li>`;
        list = list + '</ul>';

        return list;
    }
    drawer.DrawPlotRegression(mainContext, {
        type: regressionType,
        hideshow: true,
        color: 'red',
        hoverColor: 'orange',
        clickCallback: function (coefficients, position) {
            PopupBuilderInstance.Create('#map-container',
                position,
                HtmlToElement(buildCoefficientsList(coefficients)),
                {
                    preventClosing: true
                });
        }
    });
}

export function DrawReferenceAxis() {

    drawer.DrawCustomAxis(
        mainContext,
        {
            color: 'orange',
            value: 0,
            width: 1,
            identifier: 'reference',
            hideshow: true
        }
    );

}






function drawPlot(values, oXLeft, oXRight, oYBottom, oYTop) {
    var options = {
        containerID: constants.ids.plot,
        length: {
            oX: $(constants.ids.plot).width(),
            oY: $(constants.ids.plot).height()
        },
        originAxesValue: {
            oX: Math.round(oXLeft),
            oY: -30 //  Math.round(oYBottom) - 1
        },
        endAxesValue: {
            oX: Math.round(oXRight),
            oY: 30 //Math.round(oYTop)
        },
        labels: {
            oX: constants.strings.plotOxLabel,
            oY: constants.strings.plotOyLabel
        },
        plotType: 'points',
        graphColor: 'white', 
        margin: 40,
        viewBoxPadding: 40,
        points: [
            {
                data: values,
                hoverColor: 'orange',
                dataClickEvent: function (d, i, position) {
                 
                    PopupBuilderInstance.Create('#map-container',
                        position,
                        HtmlToElement(`<div>${d.X},${d.Y}</div>`),
                        {
                            preventClosing: true
                        });
                }
            }
        ]
    }
    /*Plot drawer object reset*/
    mainContext = drawer.CreateContext(options);

    drawer.Draw(mainContext); 
}






/*TOOLTIP initialisation*/

TooltipManagerInstance.Register({
    containerId: 'create_popup_window_button',
    delay: 2000,
    useRouter: true,
    routerData:
    {
        endpoint: endpoints.Miscellaneous.GetGifTooltip,
        tooltipId: 5
    },
    cursorSide: 'left',
    displayOverlay: true
});