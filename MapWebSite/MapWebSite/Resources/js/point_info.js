  

function diplayPointInfo() {
    function display() {
        document.getElementById("point-info").style = "";   
        $("#point-info").css("opacity", 1);
        $("#point-info").css("width", "60%");
        $("#point-info").css("visibility", "initial");    
    }
    if (document.getElementById("point-info").style.visibility == "initial") {
        hidePointInfo();
        setTimeout(function () { display() }, 200);
    }
    else display();
  
}

function hidePointInfo() {
    $("#point-info").css("opacity", 0);
    $("#point-info").css("width", "40%"); 
    $("#point-info").css("visibility", "hidden");    
}

function setPointInfoData(point) {

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

        oXRight = Math.max.apply(Math, points.map(function (o) {
            return o.X;
        }));

        oXLeft = Math.min.apply(Math, points.map(function (o) {
            return o.X;
        }));

        oYTop = Math.max.apply(Math, points.map(function (o) {
            return o.Y;
        }));

        oYBottom = Math.min.apply(Math, points.map(function (o) {
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


function drawPlot(values, oXLeft, oXRight, oYBottom, oYTop) {
   
    var drawer = new PlotDrawer('#plot', '#plot-popup', 450, 270,
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

    drawer.DrawPoints(values,'line');

}