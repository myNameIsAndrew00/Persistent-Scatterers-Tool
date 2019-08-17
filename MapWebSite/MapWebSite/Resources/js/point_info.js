function diplayPointInfo() {
    function display() {
        document.getElementById("point-info").style = "";
        document.getElementById("point-info").style.opacity = 1;
        document.getElementById("point-info").style.width = '60%';
        document.getElementById("point-info").style.visibility = "initial";
    }
    if (document.getElementById("point-info").style.visibility == "initial") {
        hidePointInfo();
        setTimeout(function () { display() }, 200);
    }
    else display();
  
}

function hidePointInfo() {
    document.getElementById("point-info").style.opacity = 0;
    document.getElementById("point-info").style.width = "40%";
   
    document.getElementById("point-info").style.visibility = "hidden"
 

}

function setPointInfoData(point) {
    var pointLayer = $('#_point-info');

    pointLayer.find("#ID").html(point.Number);
    pointLayer.find("#longitude").html(point.Longitude);
    pointLayer.find("#latitude").html(point.Latitude);
    pointLayer.find("#height").html(point.Height);
    pointLayer.find("#def_rate").html(point.DeformationRate);
    pointLayer.find("#std_dev").html(point.StandardDeviation);
    pointLayer.find("#est_height").html(point.EstimatedHeight);
    pointLayer.find("#est_def_rate").html(point.EstimatedDeformationRate);
}