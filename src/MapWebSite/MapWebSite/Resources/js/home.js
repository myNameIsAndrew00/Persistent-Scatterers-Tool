/*! Module: Home
 *
 * Handles the home page client logic
 *
 * */

import {
    HidePointInfo,
    CreatePopupWindow,
    AppendToPopupWindow,
    ChangePlotType,
    DrawRegressionFunction,
    DrawReferenceAxis
} from './point info/point_info.js';


window.HidePointInfo = HidePointInfo;
window.CreatePopupWindow = CreatePopupWindow;
window.AppendToPopupWindow = AppendToPopupWindow;
window.changePlotType = ChangePlotType;
window.drawRegressionFunction = DrawRegressionFunction;
window.drawReferenceAxis = DrawReferenceAxis;

var loadedScripts = [];

/**
 * Core method which loads javascript if it is not already loaded
 * */
window.getScript = function getScript(node, scriptServerPath, reload) {
    if (loadedScripts[scriptServerPath] === true && !(reload === true)) return;

    loadedScripts[scriptServerPath] = true;

    import(scriptServerPath)
        .then(module => {            
            if (module.PageInitialiser !== undefined) module.PageInitialiser();

            console.log(`${node} module loaded`);
        });
 
}


/*colorPalette used default*/
export var ColorPalette = {
    Set: function (palette, username, paletteName) {
        this.intervals = palette;
        this.user = username;
        this.name = paletteName;
    },
    name: null,
    user: null,
    isUnitialised: function () {
        return this.name == null || this.user == null || this.name == '' || this.user == '';
    },
    intervals: [
        {
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
    ]
};

