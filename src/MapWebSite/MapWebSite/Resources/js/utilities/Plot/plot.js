/*!
 * Component: PlotDrawer
 * Handles plots drawing
 * */

function hasAttribute(node, attributeName) {
    var attribute = node.attributes[attributeName];
    return (typeof attribute !== 'undefined');
}

function attrValue(node, attributeName) {
    return node.target.attributes[attributeName].value;
}

/**
 * Use this class to draw a plot*/
export class PlotDrawer {
    /**
     *      
     * @param {any} containerID The id of the container which contains the plot
     * @param {any} popupID Id of the popup which can be used to display plot details
     * @param {any} oXLength The length of oX axis (px)
     * @param {any} oYLength The length of oY axis (px)
     * @param {any} oXInterval Interval displayed on the oX
     * @param {any} oYInterval Interval displayed on the oY
     * @param {any} oxLabel The label used for oX
     * @param {any} oyLabel The label used for oY
     */
    constructor(containerID, popupID, oXLength, oYLength, oXInterval, oYInterval, oxLabel, oyLabel) {
        var self = this;
        this.containerObject = null;

        this.containerID = containerID;
        this.popupID = popupID;

        this.initialOrigin = this.origin = { oX: 50, oY: 300 };
        this.initialLength = this.length = { oX: oXLength, oY: oYLength };
        this.initialOriginAxesValue = this.originAxesValue = { oX: oXInterval.Left, oY: -30 }; //oYInterval.Bottom };
        this.initialEndAxesValue = this.endAxesValue = { oX: oXInterval.Right, oY: 30 }; //oYInterval.Top };
        this.initialOxLabel = this.oxLabel = oxLabel;
        this.initialOyLabel = this.oyLabel = oyLabel;
        this.initialPlotType = this.plotType = 'line';
        this.initialCurrentPoints = this.currentPoints = [];

        this.initialGraphColor = this.graphColor = 'white';
        this.initialFontSize = this.fontSize = 14;

        this.distances = {
            betweenLabelsAndAxis: {
                oX: -16,
                oY: -23
            },
            betweenLabels: {
                oX: 80,
                oY: 50
            }
        }

        //define constants expressions used for drawing
        this.constants = {
            plotConstant: function (axis) {
                return ((self.endAxesValue[axis] - self.initialOriginAxesValue[axis]) / self.length[axis])
            },
            labelsCount: function (axis) {
                if (axis !== 'oX' && axis !== 'oY') axis = 'oX';
                if (axis === 'oY')
                    return Math.round((self.origin[axis] ) / self.distances.betweenLabels[axis]);      
                else return Math.round((self.origin[axis] + self.length[axis]) / self.distances.betweenLabels[axis]);           
            },

            labelsIntervalLogicalValue: function (axis) {
                if (axis !== 'oX' && axis !== 'oY') axis = 'oX';            
                return self.constants.plotConstant(axis) * self.distances.betweenLabels[axis];                
            }
        }

        this.DrawAxis();
        this.DrawReferences();
      

        function showPopup(popupID, marginLeft, marginTop) {
            $(popupID).css('left', marginLeft + 'px');
            $(popupID).css('top', marginTop + 'px');
            changePopupVisibility(popupID, true);
        }

        function changePopupVisibility(popupID, isVisible) {
            isVisible ? $(popupID).removeClass('plot-popup-hidden') : $(popupID).addClass('plot-popup-hidden');
        }

        function changePopupContent(popupID, rtext) {
            $(popupID).find('.plot-plopup-text')[0].innerHTML = rtext;

        }

        //set the events for interaction with plot

        $(containerID).click(function (e) {

            if (hasAttribute(e.target, 'graphPoint')) {
                var data = JSON.parse(attrValue(e, 'graphPoint'));
                if (data == null) return;
                changePopupContent(popupID, 'oX: ' + data.oX + ' <br /> oY: ' + data.oY);
                showPopup(popupID, e.clientX, e.clientY);
            }
        });

        $(popupID).find('.plot-popup-close')[0].onclick = function () {
            changePopupVisibility(popupID, false);
        };
    }

    /*
    *   Public methods
    */
    DrawAxis(keepExisting) {
        this.drawLine({ X: this.origin.oX, Y: this.origin.oY }, true, this.length.oX, '2', null);
        this.drawLine({ X: this.origin.oX, Y: this.origin.oY }, false, -this.length.oY - 5, '2', null);

        this.DrawVerticalAxis(0, '#ff7f7f');
        this.DrawAxisLabels(keepExisting);
    }

    DrawVerticalAxis(value, color) {
        const oxValue = (value + Math.abs(this.originAxesValue.oX)) / this.constants.plotConstant('oX');
        this.drawLine({ X: this.origin.oX + oxValue, Y: this.origin.oY }, false, - this.length.oY - 5, '1', [{ key: 'stroke', value: color }]);
    }

    DrawReferences() {

        function drawReference(axis, _this) {
            var linesCount = Math.ceil(_this.length[axis] / _this.distances.betweenLabels[axis]) - 1;
            var origin = { X: _this.origin.oX, Y: _this.origin.oY };

            if (axis === 'oX') origin.Y += 6;

            for (var index = 0; index < linesCount; index++) {
                if (axis === 'oY') origin.Y -= _this.distances.betweenLabels.oY;
                else origin.X += _this.distances.betweenLabels.oX;                  
                
                _this.drawLine(origin, axis === 'oY', axis == 'oY' ? _this.length.oX - 10 : -12, '0.2', null);
            }
        }

     
        drawReference('oY', this);
        drawReference('oX', this);
     
    }

    AddPoints(points) {
        this.currentPoints.push(points);
    }

    DrawPoints(keepExisting) {
        if(keepExisting != true) this.deleteGraphPoints();

        for (var i = 0; i < this.currentPoints.length; i++)
        switch (this.plotType) {
            case 'bars':
                this.drawBarsPoints(this.currentPoints[ i ]);
                break;
            case 'line':
                this.drawLinePoints(this.currentPoints[ i ]);
                break;

        }
    }

    RedrawPoints(keepExisting) {
        if (this.currentPoints == null) return;

        if (keepExisting) this.DrawPoints(true);
        else this.DrawPoints();
    }

    DrawAxisLabels(keepExisting) {

        function drawIntermediateLables(axis, _this) {
            const labelsCount = _this.constants.labelsCount(axis);
         
            for (var i = 1; i < labelsCount - 1; i++)
                _this.drawText({
                    X: _this.origin.oX + (axis === 'oX' ? _this.distances.betweenLabels[axis] * i : _this.distances.betweenLabelsAndAxis[axis]),
                    Y: _this.origin.oY - (axis  === 'oX' ? _this.distances.betweenLabelsAndAxis[axis] : (_this.distances.betweenLabels[axis] * i))
                },
                    (_this.originAxesValue[axis] + _this.constants.labelsIntervalLogicalValue(axis) * i).toFixed(4),
                    [{ key: 'label', value: 'axis' }], 0.6);

        }

        if(keepExisting != true) this.deleteLabels();
        this.drawText({ X: this.origin.oX - 10, Y: this.origin.oY - this.distances.betweenLabelsAndAxis.oX },
            this.originAxesValue.oX,
            [{ key: 'label', value: 'axis' }]);
        this.drawText({ X: this.origin.oX + this.length.oX, Y: this.origin.oY - this.distances.betweenLabelsAndAxis.oX }, this.endAxesValue.oX,
            [{ key: 'label', value: 'axis' }]);
        drawIntermediateLables('oX', this);
     
        this.drawText({ X: this.origin.oX + this.distances.betweenLabelsAndAxis.oY, Y: this.origin.oY - this.length.oY + 5 }, this.endAxesValue.oY,
            [{ key: 'label', value: 'axis' }]);
        this.drawText({ X: this.origin.oX + this.distances.betweenLabelsAndAxis.oY , Y: this.origin.oY }, this.originAxesValue.oY,
            [{ key: 'label', value: 'axis' }]);
        drawIntermediateLables('oY', this);

        this.drawText({ X: this.origin.oX + this.length.oX / 2 - 20, Y: this.origin.oY + 35 }, this.oxLabel, null);
        this.drawText({ X: this.origin.oX, Y: this.origin.oY - this.length.oY - 10 }, this.oyLabel, null);
    }

    SetPlotType(plotType, permanent) {
        this.plotType = plotType;
        if (permanent) this.initialPlotType = this.plotType;
    }

    SetContainerObject(containerObject) {
        this.containerObject = containerObject;
    }

    SetGraphColor(newColor) {
        this.graphColor = newColor; 
    }

    SetFontSize(fontSize) {
        this.fontSize = fontSize;
    }

    SetLength(oX, oY) {
        this.length = { oX: oX, oY: oY };
    }

    SetOrigin(X, Y) {
        this.origin = { oX: X, oY: Y };
    }

    /*Method use to cancel all changes made by setters*/
    ResetSetters() {
        this.containerObject = null;

        this.initialFontSize = this.fontSize;
        this.origin = this.initialOrigin;
        this.length = this.initialLength;
        this.plotType = this.initialPlotType;
        this.graphColor = this.initialGraphColor;
    }

    Clear() {
        $(this.containerID).empty();
    }

    /*
    *   Private methods 
    */

    drawText(origin, toWrite, optionalAttributes, fontSizeScale = 1) {
        var text = document.createElementNS('http://www.w3.org/2000/svg', "text");
        text.textContent = toWrite;
        text.setAttribute('x', origin.X);
        text.setAttribute('y', origin.Y);
        text.setAttribute('fill', this.graphColor);
        text.setAttribute('font-size', this.fontSize * fontSizeScale);

        if (optionalAttributes !== null)
            for (var index = 0; index < optionalAttributes.length; index++)
                text.setAttribute(optionalAttributes[index].key, optionalAttributes[index].value);

        if (this.containerObject != null) this.containerObject.append(text);
        else $(this.containerID).append(text);
    }

    drawBarsPoints(points) {
        //this is the 'bars' graphType.. cand be changed

        var origin = { X: this.origin.oX, Y: this.origin.oY };
        var width = (this.length.oX / points.length) / 2;

        for (var index = 0; index < points.length; index++) {
            origin.X = this.origin.oX + this.transformToXAxisValue(points[index].X);
            this.drawLine(origin,
                false,
                -(this.transformToYAxisValue(points[index].Y)),
                width,
                [{ key: 'graphPoint', value: '{ "oY": ' + points[index].Y + ', "oX": ' + points[index].X + '}' },
                { key: 'class', value: 'graphPoint ' }]);
        }
    }

    drawLinePoints(points) {
        points.sort((a, b) => (a.X > b.X) ? 1 : -1);
        var origin = { X: this.origin.oX, Y: this.origin.oY };

        function drawLine(object) {

            var line = document.createElementNS('http://www.w3.org/2000/svg', "path");
            var path = 'M' + ' ' + origin.X + ' ' + origin.Y;
            path = path + 'l' + ' ' + (object.transformToXAxisValue(points[0].X) - object.transformToXAxisValue(object.originAxesValue.oX))
                + ' ' + -(object.transformToYAxisValue(points[0].Y)) + ' ';

            for (var index = 1; index < points.length; index++) {
                path = path + 'l' + ' ' + (object.transformToXAxisValue(points[index].X) - object.transformToXAxisValue(points[index - 1].X))
                    + ' ' + -(object.transformToYAxisValue(points[index].Y) - object.transformToYAxisValue(points[index - 1].Y)) + ' ';
            }
            line.setAttributeNS(null, "d", path);
            line.setAttributeNS(null, "stroke-width", 1);
            line.setAttributeNS(null, "stroke", object.graphColor);
            line.setAttributeNS(null, "fill", "none");
            line.setAttributeNS(null, "graphPoint", null);

            if (object.containerObject != null) object.containerObject.append(line);
            else $(object.containerID).append(line);
        }

        function drawCircles(object) {
            for (var index = 0; index < points.length; index++) {
                var radius = Math.min( 8, 20 / Math.sqrt(points.length));

                var circle = document.createElementNS('http://www.w3.org/2000/svg', "circle");

                circle.setAttributeNS(null, 'cx', origin.X + object.transformToXAxisValue(points[index].X));
                circle.setAttributeNS(null, 'cy', origin.Y - object.transformToYAxisValue(points[index].Y));
                circle.setAttributeNS(null, "graphPoint", '{ "oY": ' + points[index].Y + ', "oX": ' + points[index].X + '}');
                circle.setAttributeNS(null, "class", "graphPoint graphPointDot");
                circle.setAttributeNS(null, 'r', radius);

                if (object.containerObject != null) object.containerObject.append(circle);
                else $(object.containerID).append(circle);
            }
        }

        drawLine(this);
        drawCircles(this);

    }

    drawLine(origin, isHorizontaly, length, width, optionalAttributes) {
        var newpath = document.createElementNS('http://www.w3.org/2000/svg', "path");
        this.setAttributes(newpath, width, optionalAttributes);

        newpath.setAttributeNS(null, "d", 'M' + ' ' +
            origin.X + ' ' +
            origin.Y + ' ' +
            (isHorizontaly ? 'h' : 'v') + ' ' +
            length);

        if (this.containerObject != null) this.containerObject.append(newpath);
        else $(this.containerID).append(newpath);

    }

    setAttributes(path, strokeWidth, optionalAttributes) {
        path.setAttributeNS(null, "stroke", this.graphColor);
        path.setAttributeNS(null, "stroke-width", strokeWidth);
        path.setAttributeNS(null, "fill", "none");

        if (optionalAttributes !== null)
            for (var index = 0; index < optionalAttributes.length; index++)
                path.setAttributeNS(null, optionalAttributes[index].key, optionalAttributes[index].value);
    }

    transformToXAxisValue(value) {
        return (value + Math.abs(this.originAxesValue.oX)) * this.length.oX / (this.endAxesValue.oX + Math.abs(this.originAxesValue.oX));
    }

    transformToYAxisValue(value) {
        return (value + Math.abs(this.originAxesValue.oY)) * this.length.oY / (this.endAxesValue.oY + Math.abs(this.originAxesValue.oY));
    }



    deleteGraphPoints() {
        var nodes = $(this.containerID).children();
        for (var i = 0; i < nodes.length; i++)
            if (hasAttribute(nodes[i], 'graphPoint'))
                nodes[i].remove();
    }

    deleteLabels() {
        var nodes = $(this.containerID).children();
        for (var i = 0; i < nodes.length; i++)
            if (hasAttribute(nodes[i], 'label'))
                nodes[i].remove();
    }


}

