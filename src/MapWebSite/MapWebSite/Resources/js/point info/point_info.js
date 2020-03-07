/*! Component: PointInfo
 *
 * This component is used for handling the point info slider actions and information 
 *
 * */

import { PlotDrawer } from '../utilities/Plot/plot.js';
import { CardsManager } from '../utilities/Card/cards_manager.js';
import { UnselectFeatureOnMap } from '../map/map.js';

var cardsManager = new CardsManager('map-container');
var currentDrawer = null;

var currentDisplayedPoint = null;

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

    pointLayer.find("#ID").html(currentDisplayedPoint.Number);
    pointLayer.find("#longitude").html(currentDisplayedPoint.Longitude);
    pointLayer.find("#latitude").html(currentDisplayedPoint.Latitude);
    pointLayer.find("#height").html(currentDisplayedPoint.Height);
    pointLayer.find("#def_rate").html(currentDisplayedPoint.DeformationRate);
    pointLayer.find("#std_dev").html(currentDisplayedPoint.StandardDeviation);
    pointLayer.find("#est_height").html(currentDisplayedPoint.EstimatedHeight);
    pointLayer.find("#est_def_rate").html(currentDisplayedPoint.EstimatedDeformationRate);
}






export function HidePointInfo(showTopMenu) {
    if (showTopMenu) $("#top-menu").removeClass('top-menu-hiden');

    $("#point-info").css("opacity", 0);
    $("#point-info").css("width", "40%"); 
    $("#point-info").css("visibility", "hidden");  

    if(currentDisplayedPoint != null)
         UnselectFeatureOnMap(currentDisplayedPoint.Number);
}

export function CreatePopupWindow() {

    const minPlotDimensions = { height: 250, width: 400 };

    //use the last drawer created when a point was clicked on map
    var popupPlotDrawer = currentDrawer;

    //when the popup is resized, the plot must be redrawed
    var cardContentId = cardsManager.Draw(true, drawPlot);
    var svgPlot = document.createElementNS('http://www.w3.org/2000/svg', 'svg');

    function initPlot() {
        svgPlot.id = 'window-plot';
        svgPlot.classList.add('plot');
        svgPlot.setAttributeNS(null, 'width', '100%');
        svgPlot.setAttributeNS(null, 'height', '100%');
        svgPlot.style.display = 'block';
    }

    function drawPlot() {       
        popupPlotDrawer.SetContainerObject(svgPlot);
        svgPlot.innerHTML = '';
        popupPlotDrawer.SetGraphColor('black');

        const height = Math.max(minPlotDimensions.height, $(cardContentId).height());
        const width = Math.max(minPlotDimensions.width, $(cardContentId).width());

        popupPlotDrawer.SetFontSize(10);
        popupPlotDrawer.SetOrigin(30, height - 50);
        popupPlotDrawer.SetLength( width - 155 , height - 80);

        popupPlotDrawer.DrawReferences();
        popupPlotDrawer.DrawAxis(true);
        popupPlotDrawer.RedrawPoints(true);
        
        popupPlotDrawer.ResetSetters();
         

        $(cardContentId).append(svgPlot);
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

        $(cardContentId).append(textContent);
    }

    initPlot();
    //delay for animation
    setTimeout(function () {
        drawPlot();
        drawText();
    }, 250);

}


function drawPlot(values, oXLeft, oXRight, oYBottom, oYTop) {
   
    /*Plot drawer object reset*/
    currentDrawer = new PlotDrawer('#plot', '#plot-popup', 450, 270,
        {
            Left: Math.round(oXLeft),
            Right: Math.round(oXRight)
        },
        {
            Bottom: Math.round(oYBottom) - 1,
            Top: Math.round(oYTop)
        },
        'Time [days]',
        'Deformation [mm]');


    currentDrawer.DrawPoints(values);
}

window.changePlotType = function changePlotType(plotType) {
    if (currentDrawer == null) return;

    currentDrawer.SetPlotType(plotType,true);
    currentDrawer.RedrawPoints();
}