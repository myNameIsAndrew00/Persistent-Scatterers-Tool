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

export class PlotDrawer {
    constructor(containerID, popupID, oXLength, oYLength, oXInterval, oYInterval, oxLabel, oyLabel) {
        this.containerObject = null;

        this.containerID = containerID;
        this.popupID = popupID;

        this.initialOrigin = this.origin = { X: 50, Y: 300 };
        this.initialLength = this.length = { oX: oXLength, oY: oYLength };
        this.initialOriginAxesValue = this.originAxesValue = { oX: oXInterval.Left, oY: oYInterval.Bottom };
        this.initialEndAxesValue = this.endAxesValue = { oX: oXInterval.Right, oY: oYInterval.Top };
        this.initialOxLabel = this.oxLabel = oxLabel;
        this.initialOyLabel = this.oyLabel = oyLabel;
        this.initialPlotType = this.plotType = 'line';
        this.initialCurrentPoints = this.currentPoints = null;

        this.initialGraphColor = this.graphColor = 'white';
        this.initialFontSize = this.fontSize = 14;

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
        this.drawLine(this.origin, true, this.length.oX, '2', null);
        this.drawLine(this.origin, false, -this.length.oY - 5, '2', null);
        this.DrawAxisLabels(keepExisting);
    }

    DrawReferences() {
        var linesCount = Math.round(this.length.oY / 50);
        var origin = { X: this.origin.X, Y: this.origin.Y };
        for (var index = 0; index < linesCount; index++) {
            origin.Y -= 50;
            this.drawLine(origin, true, this.length.oX - 10, '0.2', null);
        }
    }

    DrawPoints(points, keepExisting) {
        this.currentPoints = points;
        if(keepExisting != true) this.deleteGraphPoints();

        switch (this.plotType) {
            case 'bars':
                this.drawBarsPoints(this.currentPoints);
                break;
            case 'line':
                this.drawLinePoints(this.currentPoints);
                break;

        }
    }

    RedrawPoints(keepExisting) {
        if (this.currentPoints == null) return;

        if (keepExisting) this.DrawPoints(this.currentPoints, true);
        else this.DrawPoints(this.currentPoints);
    }

    DrawAxisLabels(keepExisting) {
        if(keepExisting != true) this.deleteLabels();
        this.drawText({ X: this.origin.X - 10, Y: this.origin.Y + 20 },
            this.originAxesValue.oX,
            [{ key: 'label', value: 'axis' }]);
        this.drawText({ X: this.origin.X + this.length.oX, Y: this.origin.Y + 20 }, this.endAxesValue.oX,
            [{ key: 'label', value: 'axis' }]);

        this.drawText({ X: this.origin.X - 20, Y: this.origin.Y - this.length.oY + 5 }, this.endAxesValue.oY,
            [{ key: 'label', value: 'axis' }]);
        this.drawText({ X: this.origin.X - 20, Y: this.origin.Y }, this.originAxesValue.oY,
            [{ key: 'label', value: 'axis' }]);

        this.drawText({ X: this.origin.X + this.length.oX / 2 - 20, Y: this.origin.Y + 20 }, this.oxLabel, null);
        this.drawText({ X: this.origin.X, Y: this.origin.Y - this.length.oY - 10 }, this.oyLabel, null);
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
        this.origin = { X: X, Y: Y };
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

    /*
    *   Private methods 
    */

    drawText(origin, toWrite, optionalAttributes) {
        var text = document.createElementNS('http://www.w3.org/2000/svg', "text");
        text.textContent = toWrite;
        text.setAttribute('x', origin.X);
        text.setAttribute('y', origin.Y);
        text.setAttribute('fill', this.graphColor);
        text.setAttribute('font-size', this.fontSize);

        if (optionalAttributes !== null)
            for (var index = 0; index < optionalAttributes.length; index++)
                text.setAttribute(optionalAttributes[index].key, optionalAttributes[index].value);

        if (this.containerObject != null) this.containerObject.append(text);
        else $(this.containerID).append(text);
    }

    drawBarsPoints(points) {
        //this is the 'bars' graphType.. cand be changed

        var origin = { X: this.origin.X, Y: this.origin.Y };
        var width = (this.length.oX / points.length) / 2;

        for (var index = 0; index < points.length; index++) {
            origin.X = this.origin.X + this.transformToXAxisValue(points[index].X);
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
        var origin = { X: this.origin.X, Y: this.origin.Y };

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
                var circle = document.createElementNS('http://www.w3.org/2000/svg', "circle");

                circle.setAttributeNS(null, 'cx', origin.X + object.transformToXAxisValue(points[index].X));
                circle.setAttributeNS(null, 'cy', origin.Y - object.transformToYAxisValue(points[index].Y));
                circle.setAttributeNS(null, "graphPoint", '{ "oY": ' + points[index].Y + ', "oX": ' + points[index].X + '}');
                circle.setAttributeNS(null, "class", "graphPoint graphPointDot");
                circle.setAttributeNS(null, 'r', 3);

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

