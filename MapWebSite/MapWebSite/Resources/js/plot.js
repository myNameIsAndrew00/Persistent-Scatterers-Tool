function hasAttribute(node, attributeName){
    var attribute = node.attributes[attributeName];
    return (typeof attribute !== 'undefined');
}

function attrValue(node,attributeName){
    return node.target.attributes[attributeName].value;
}

class PlotDrawer{
    constructor(containerID, popupID, oXLength, oYLength, oXInterval, oYInterval, oxLabel, oyLabel){
        this.containerID = containerID;
        this.popupID = popupID;

        this.origin = { X : 50, Y : 300 };
        this.length = { oX: oXLength, oY: oYLength };
        this.originAxesValue = { oX: oXInterval.Left, oY: oYInterval.Bottom };
        this.endAxesValue = { oX: oXInterval.Right, oY: oYInterval.Top };
        this.oxLabel = oxLabel;
        this.oyLabel = oyLabel;

        this.DrawAxis();     
        this.DrawReferences();  
        
        function showPopup(popupID, marginLeft, marginTop){                    
            $(popupID).css('left', marginLeft + 'px');                   
            $(popupID).css('top', marginTop + 'px');
            changePopupVisibility(popupID, true);        
        }

        function changePopupVisibility(popupID, isVisible){
            isVisible ? $(popupID).removeClass('plot-popup-hidden') :  $(popupID).addClass('plot-popup-hidden');
        }

        function changePopupContent(popupID, rtext){
           $(popupID).find('.plot-plopup-text')[0].innerHTML = rtext;
            
        }

        //set the events for interaction with plot
        
        $(containerID).click(function(e) {          
                        
            if (hasAttribute(e.target, 'graphPoint')){ 
                var data = JSON.parse(attrValue(e,'graphPoint'));
                changePopupContent(popupID, 'oX: ' + data.oX + ' <br /> oY: ' + data.oY );
                showPopup(popupID, e.clientX, e.clientY);
            }
        });
  
        $(popupID).find('.plot-popup-close')[0].onclick = function(){
                changePopupVisibility(popupID, false);
        };
    }

    /*
    *   Public methods
    */
    DrawAxis(){
         this.drawLine( this.origin, true, this.length.oX , '2', null);
         this.drawLine( this.origin, false, -this.length.oY - 5, '2', null);
         this.DrawAxisLabels();
    }

    DrawReferences(){
        var linesCount = Math.round(this.length.oY / 50);
        var origin = { X: this.origin.X, Y: this.origin.Y };
        for(var index = 0; index < linesCount; index++){
            origin.Y -= 50;
            this.drawLine(origin, true, this.length.oX - 10, '0.2', null );
        } 
    }

    DrawPoints(points, graphType) {

        //this is the 'bars' graphType.. cand be changed
        this.deleteGraphPoints();
        var origin = { X: this.origin.X, Y: this.origin.Y };
        var width = (this.length.oX / points.length) / 2;

        for(var index = 0;  index < points.length; index++){
            origin.X = this.origin.X + this.transformToXAxisValue(points[index].X);
            this.drawLine(origin, 
                          false, 
                          -(this.transformToYAxisValue(points[index].Y)), 
                          width, 
                          [ { key: 'graphPoint', value: '{ "oY": ' + points[index].Y + ', "oX": ' + points[index].X + '}' },
                            { key: 'class', value: 'graphPoint '}]);
        }
    }

    DrawAxisLabels(){
        this.drawText({ X:this.origin.X - 10, Y:this.origin.Y + 20 }, this.originAxesValue.oX);
        this.drawText({ X:this.origin.X + this.length.oX, Y:this.origin.Y + 20 }, this.endAxesValue.oX);

        this.drawText({ X:this.origin.X - 20, Y:this.origin.Y - this.length.oY + 5 }, this.endAxesValue.oY);
        this.drawText({ X: this.origin.X - 20, Y: this.origin.Y }, this.originAxesValue.oY);

        this.drawText({ X: this.origin.X + this.length.oX / 2 - 20, Y: this.origin.Y + 20 }, this.oxLabel);
        this.drawText({ X: this.origin.X, Y: this.origin.Y - this.length.oY - 10 }, this.oyLabel);
    }
    /*
    *   Private methods 
    */

    drawText(origin, toWrite){
        var text = document.createElementNS('http://www.w3.org/2000/svg',"text");  
        text.textContent =  toWrite;
        text.setAttribute('x', origin.X);
        text.setAttribute('y', origin.Y);
        text.setAttribute('fill', 'white');
        $(this.containerID).append(text);  
    }

    drawLine(origin, isHorizontaly, length, width, optionalAttributes ) {
        var newpath = document.createElementNS('http://www.w3.org/2000/svg',"path");  
        this.setAttributes(newpath, width, optionalAttributes);
        
        newpath.setAttributeNS(null, "d", 'M' + ' ' +
                                         origin.X + ' ' + 
                                         origin.Y + ' ' +
                                         (isHorizontaly ? 'h' : 'v') + ' ' +
                                        length);
  
        $(this.containerID).append(newpath); 
        
    }

    setAttributes(path, strokeWidth, optionalAttributes){
        path.setAttributeNS(null, "stroke", "white"); 
        path.setAttributeNS(null, "stroke-width", strokeWidth); 
        path.setAttributeNS(null, "fill", "none");  

        if(optionalAttributes !== null)
        for(var index = 0; index < optionalAttributes.length; index++)
             path.setAttributeNS(null, optionalAttributes[index].key, optionalAttributes[index].value);
    }

    transformToXAxisValue(value){
        return (value + Math.abs(this.originAxesValue.oX)) * this.length.oX / (this.endAxesValue.oX + Math.abs(this.originAxesValue.oX));
    }

    transformToYAxisValue(value){
        return (value + Math.abs(this.originAxesValue.oY)) * this.length.oY / (this.endAxesValue.oY + Math.abs(this.originAxesValue.oY));
    }

    deleteGraphPoints(){
        var nodes = $(this.containerID).children();
        for(var i = 0; i < nodes.length; i++)
            if(hasAttribute(nodes[i],'graphPoint'))
                nodes[i].remove();
    }

    
}
 
