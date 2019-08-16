function diplayPointInfo() {
    document.getElementById("point-info").style = ""; setTimeout(function () {
        document.getElementById("point-info").style.opacity = 1;
        document.getElementById("point-info").style.width = '60%';
    }, 150);
}

function hidePointInfo() {
    document.getElementById("point-info").style.opacity = 0;
    document.getElementById("point-info").style.width = "40%";
    setTimeout(function () {
        document.getElementById("point-info").style.display = "none";
    }, 200);
}