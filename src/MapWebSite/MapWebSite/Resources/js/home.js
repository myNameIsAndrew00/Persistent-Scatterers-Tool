/*! Module: Home
 *
 * Handles the home page client logic
 *
 * */

import { HidePointInfo, CreatePopupWindow } from './point info/point_info.js';

window.HidePointInfo = HidePointInfo;
window.CreatePopupWindow = CreatePopupWindow;

var loadedScripts = [];

/**
 * Core method which loads javascript if it is not already loaded
 * */
window.getScript = function getScript(node, scriptServerPath) {    
    if (loadedScripts[scriptServerPath] === true) return;

    loadedScripts[scriptServerPath] = true;
        
    var script = document.createElement('script');
    script.src = scriptServerPath;
    script.type = 'module';

    $(node).append(script);
}


/*colorPalette used default*/
export var colorPalette = [{
    Color: '#33ff00',
    Left: 0.0,
    Right: 10.0
},
{
    Color: '#33cc00',
    Left: 10.0,
    Right: 20.0
},
{
    Color: '#339900',
    Left: 20.0,
    Right: 30.0
},
{
    Color: '#ffff00',
    Left: 30.0,
    Right: 40.0
},
{
    Color: '#ffcc00',
    Left: 40.0,
    Right: 50.0
},
{
    Color: '#ff9900',
    Left: 50.0,
    Right: 60.0
},
{
    Color: '#ff6600',
    Left: 60.0,
    Right: 70.0
},
{
    Color: '#ff3300',
    Left: 70.0,
    Right: 80.0
},
{
    Color: '#ff0000',
    Left: 80.0,
    Right: 90.0
},
{
    Color: '#660000',
    Left: 90.0,
    Right: 100.0
}
];

export function SetColorPalette(palette){
    colorPalette = palette;
}

 