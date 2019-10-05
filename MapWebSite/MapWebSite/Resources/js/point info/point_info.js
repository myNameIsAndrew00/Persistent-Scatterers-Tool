import { PlotDrawer } from '../plot.js';

var currentDrawer = null;

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

    pointLayer.find("#ID").html(point.Number);
    pointLayer.find("#longitude").html(point.Longitude);
    pointLayer.find("#latitude").html(point.Latitude);
    pointLayer.find("#height").html(point.Height);
    pointLayer.find("#def_rate").html(point.DeformationRate);
    pointLayer.find("#std_dev").html(point.StandardDeviation);
    pointLayer.find("#est_height").html(point.EstimatedHeight);
    pointLayer.find("#est_def_rate").html(point.EstimatedDeformationRate);
}






export function HidePointInfo(showTopMenu) {
    if (showTopMenu) $("#top-menu").removeClass('top-menu-hiden');

    $("#point-info").css("opacity", 0);
    $("#point-info").css("width", "40%"); 
    $("#point-info").css("visibility", "hidden");    
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
        'reference (days)',
        'value');


    currentDrawer.DrawPoints(values);
}

window.changePlotType = function changePlotType(plotType) {
    if (currentDrawer == null) return;

    currentDrawer.SetPlotType(plotType);
    currentDrawer.RedrawPoints();
}