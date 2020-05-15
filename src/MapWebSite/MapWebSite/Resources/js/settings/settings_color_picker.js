/*! Component: ColorPicker
 *
 * This component maintaint a color picker in the GUI 
 *
 * */

import { ColorList, ColorNode } from './color_picker_color_list.js';
import { DisplayOverlay } from './settings.js';
import { Router, endpoints } from '../api/api_router.js';

/*finaliser*/
export function PageFinaliser() {
    colorList = new ColorList(new ColorNode('dot-1'), SliderWidth, '#361f9c');  
    labelsObserver.dropAll();
}

/*use this variables for dots controll*/

var isMouseDown = false;
var dotsCount = 1;
var currentDot = 1;

/*the limits of the slider*/
const LeftMargin = 40
                + Number(window.getComputedStyle(document.getElementById('color-picker-container'), null).marginLeft.replace(/[a-zA-Z]/g, ""))
                + Number(window.getComputedStyle(document.getElementById('settings-layer'), null).paddingLeft.replace(/[a-zA-Z]/g, ""))
                + Number(window.getComputedStyle(document.getElementById('color-picker-content'), null).paddingLeft.replace(/[a-zA-Z]/g, ""))
                - Number(window.getComputedStyle(document.getElementById('slider'), null).left.replace(/[a-zA-Z]/g, ""));
const SliderWidth = Number(window.getComputedStyle(document.getElementById('slider'), null).width.replace(/[a-zA-Z]/g, ""));
const DotRadius = 10;

/*color mapping list*/
var colorList = new ColorList(new ColorNode('dot-1'), SliderWidth, '#361f9c');

/*use this object to manage displayed labels*/
var labelsObserver = {
    labels: [],
    addLabel:
        /**
         * Use this function to change an existing label
         * @param {any} position absolut position of the label
         * @param {any} dotIdentifier the identifier of the label
         * @param {any} create a boolean which indicate if the label must be created or not
         * */
        function (position, dotIdentifier, create) {
            var label = null;
            if (!create) {
                label = document.getElementById('dot-' + dotIdentifier + '-label');
            } else {
                label = document.createElement('label');
                label.id = 'dot-' + dotIdentifier + '-label';
                $('#dots-container').append(label);
            }

            label.style.left = position + 'px';
            this.display(label);
            this.labels.push(label);
        },
    rewriteLabels: function () {
        for (var i = 0; i < this.labels.length; i++) {
            this.display(this.labels[i]);
        }
    },
    display: function (label) {
        var percentage = colorList.GetPercentage(label.offsetLeft).toFixed(2);
        $(label).html(this.displayPercent() ?
            percentage + '%' :
            colorList.GetValue(percentage).toFixed(2));
    },
    displayPercent: function () {
        return $('#displayPercent').is(':checked');
    },
    dropAll: function () {
        this.labels = [];
    }
}

window.resetColorList = function resetColorList() {
    PageFinaliser();
}

window.changePosition = function changePosition() { 
    if(currentDot == 1) return;
    if(isMouseDown){
        var dotPosition = event.clientX - LeftMargin;
        var dot = document.getElementById('dot-' + currentDot); 
        var margins = colorList.GetPointMargins('dot-' + currentDot);
     
        if(dotPosition < margins.left  || dotPosition > margins.right ) return;

        //change the label content      
        labelsObserver.addLabel(dotPosition, currentDot, false);

        //change the point
        dot.style.left = dotPosition + DotRadius + 'px';
       
        colorList.SetPointPosition('dot-' + currentDot, dotPosition);       
        $('#slider').css({ background: colorList.BuildGradientString() }); 
        showColorPicker(event.clientX - 10);
    }  
}

window.minMaxValueChanged = function minMaxValueChanged() {
    changeColorListValues();
}

window.displayPercentChanged = function displayPercentChanged() {
    labelsObserver.rewriteLabels();
}

window.changeButtonState = function changeButtonState(state){ 
    isMouseDown = state;
}

window.usePaletteTemplate = function usePaletteTemplate(colorsString) {
    //remove current dots
    var keys = colorList.GetKeys();
    for (var i = 0; i < keys.length; i++)
        removeDot(keys[i]);

    const colors = JSON.parse(colorsString);

    changeSpanColor(colors[0].color, 'dot-1');

    for (var i = 1; i < colors.length; i++)
        addDot(colors[i].color, (colors[i].percent / 100) * SliderWidth, false);

}

window.changeSelectedDot = function changeSelectedDot(){
    var id = event.srcElement.id; 
    if(id.includes('dot-')) currentDot = id.split('-')[1];
    
    showColorPicker(event.clientX);
}

window.changeActivePalette = function changeActivePalette(itemId) {
    $("table[id^='picker']").removeClass('palette-active');
    $(`#picker-${itemId}`).addClass('palette-active');
}


/**
 * Use this function to change color list left and right values */
function changeColorListValues() {
    colorList.SetValues({
        left: parseFloat($('#pickerLeftValue').val()),
        right: parseFloat($('#pickerRightValue').val())
    });
    labelsObserver.rewriteLabels();
}


/**
 * Create a span and a label on slider
 * @param {any} dotPosition dot position on slider
 * @param {any} dotColor dot color
 * Returns span id
 */
function createSpan(dotPosition, dotColor) {

    dotsCount++;
    //set the current dot to be the newest one
    currentDot = dotsCount;
    
    var dot = document.createElement('span');
   
    dot.classList.add('dot');
    dot.id = 'dot-' + dotsCount;
    dot.style.left = dotPosition + DotRadius +'px';   
    dot.style.backgroundColor = dotColor;
    dot.draggable = false;
    dot.addEventListener('mousedown', changeSelectedDot); 

    $('#dots-container').append(dot);
    labelsObserver.addLabel(dotPosition, dotsCount, true);

    return dot.id;
}
 

/**
 * Delete a span and a label based on span id
 * @param {any} dotId id of span
 */
function deleteSpan(dotId) {
    //remove the point visualy
    dotId = '#' + dotId;
    var dotLabelId = dotId + '-label';

    $('#dots-container').children(dotId).remove();
    $('#dots-container').children(dotLabelId).remove();

    //redraw the slider and close the color picker
    $('#slider').css({ background: colorList.BuildGradientString() });
    changeColorPickerVisibility(false);
}

/*function which handles the points addition to the slider*/
window.addDot = function addDot(color, position, displayPicker = true) {

    if (color === undefined) color = '#' + (Math.random() * 0xFFFFFF << 0).toString(16);
    if (position === undefined) position = event.clientX - LeftMargin;

    if(displayPicker === true) showColorPicker(event.clientX);

    var spanID = createSpan(position, color);

    colorList.AddNode(position, color, spanID);

    $('#slider').css({ background: colorList.BuildGradientString() });

}

window.removeDot = function removeDot(dotId) {
    if (dotId === undefined) dotId = 'dot-' + currentDot;

    if (colorList.RemoveNode(dotId) === false) return;

    deleteSpan(dotId);    
}


window.changeSpanColor = function changeSpanColor(newColor, spanId) {
    if (spanId === undefined) spanId = 'dot-' + currentDot;

    colorList.SetPointColor(spanId, newColor);
    document.getElementById(spanId).style.backgroundColor = newColor;
    
    $('#slider').css({ background: colorList.BuildGradientString() });
     
}





function showColorPicker(horizontalPosition) { 
   changeColorPickerVisibility(true);

   $('#color-picker').css('left', horizontalPosition - LeftMargin + 'px');
}

window.changeColorPickerVisibility = function changeColorPickerVisibility(isVisible){
    isVisible ? $('#color-picker').removeClass('color-picker-hidden') :  $('#color-picker').addClass('color-picker-hidden');
}

window.enableSubmit = function enableSubmit() {    
    $('#finish-info-card').children('#send-palette-button').prop('disabled',
        $('#name-info-card').children('#color-palette-name').val() === '');
}

window.sendColorPalette = function sendColorPalette() {
    var paletteName = $('#name-info-card').children('#color-palette-name').val();

    if (paletteName === '') return;  

    changeColorListValues();

    Router.Post(endpoints.Settings.SaveColorsPalette,
        {
            Intervals: colorList.GetColorMap(),
            Name: paletteName,
        },
        function (receivedInfo) {
            DisplayOverlay(receivedInfo);
        },
        function (receivedInfo) {
            DisplayOverlay(receivedInfo);
        });   
}

