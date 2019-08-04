/*use this variables for dots controll*/ 
var isMouseDown = false;
var dotsCount = 1;
var currentDot = 1;

/*the limits of the slider*/
var LeftMargin = 30;
var RightMargin = 810;

/*color mapping */
var colorList = new ColorList(new ColorNode('dot-1'), LeftMargin, RightMargin, '#361f9c');

 

function changePosition(){
    if(currentDot == 1) return;
    if(isMouseDown){
        var dotPosition = event.clientX - LeftMargin;
        var dot = document.getElementById('dot-' + currentDot);
        var dotLabel = document.getElementById('dot-' + currentDot + '-label');
        var margins = colorList.GetPointMargins('dot-' + currentDot);
    
        if(dotPosition < margins.left || dotPosition > margins.right ) return;

        //change the label content
        dotLabel.innerText = colorList.GetPercentage(dotPosition).toFixed(2) + '%';
        dotLabel.style.left = dotPosition - 7 + 'px';

        //change the point
        dot.style.left = dotPosition +'px';
       
        colorList.SetPointPosition('dot-' + currentDot, dotPosition);       
        $('#slider').css({ background: colorList.BuildGradientString() }); 
        showColorPicker(event.clientX - 10);
    }  
}


function changeButtonState(state){ 
    isMouseDown = state;

}


function changeSelectedDot(){
    var id = event.srcElement.id; 
    if(id.includes('dot-')) currentDot = id.split('-')[1];
    
    showColorPicker(event.clientX);
}

/*function which handles the points addition to the slider*/  
function addDot(){

    var dotColor = '#'+(Math.random()*0xFFFFFF<<0).toString(16);
    var dotPosition = event.clientX - 20;      

    showColorPicker(event.clientX);
    var spanID = createSpan(dotPosition, dotColor);
  
    colorList.AddNode( dotPosition, dotColor, spanID);
     
    $('#slider').css({ background: colorList.BuildGradientString() });
 
}

function createSpan(dotPosition, dotColor){
    dotsCount++;
    //set the current dot to be the newest one
    currentDot = dotsCount;
    
    var dot = document.createElement('span');
    var label = document.createElement('label');

    dot.classList.add('dot');
    dot.id = 'dot-' + dotsCount;
    dot.style.left = dotPosition +'px';   
    dot.style.backgroundColor = dotColor;
    dot.draggable = false;
    dot.addEventListener('mousedown',changeSelectedDot);
 
    label.innerText = colorList.GetPercentage(dotPosition).toFixed(2) + '%';
    label.style.left = dotPosition - 7 + 'px';
    label.id = 'dot-' + dotsCount + '-label';

    $('#dots-container').append(dot);
    $('#dots-container').append(label);

    return dot.id;
}

function changeSpanColor(newColor){
    colorList.SetPointColor('dot-' + currentDot, newColor);
    document.getElementById('dot-' + currentDot).style.backgroundColor = newColor;
    
    $('#slider').css({ background: colorList.BuildGradientString() });
     
}

function showColorPicker(horizontalPosition){
   changeColorPickerVisibility(true);

   $('#color-picker').css('left', horizontalPosition - 25 + 'px');
}

function changeColorPickerVisibility(isVisible){
    isVisible ? $('#color-picker').removeClass('color-picker-hidden') :  $('#color-picker').addClass('color-picker-hidden');
}
