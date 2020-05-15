/*! Component: ColorPickerList
*
* This component maintaint the logic of color picker used in GUI
*
* */

export class ColorNode{
    constructor(pointKey){
        this.pointKey = pointKey;

        this.nextColor = null;
        this.prevColor = null;
    }
} 

/*use this class to modelate the slider
*  Internals components: a list -> which manages the order of the hashmap
*                        a hashmap -> which manages the points and their valid positions 
*/
export class ColorList{
    constructor(firstColorNode, barWidth, firstColor){
        this.root = firstColorNode; 
        this.barWidth = barWidth; 

        this.leftValue = 0;
        this.rightValue = 100;

        /*initialise the dictionary*/
        this.pointsDictionary = {};
        this.pointsDictionary[ firstColorNode.pointKey ] = { 
                                                 color : firstColor,
                                                 position: 0, 
                                                 leftPointID: null,
                                                 rightPointID : null};
    }

    /*
    * private region
    */


    getPointPercentage(pointKey){
        return 100 * (this.pointsDictionary[pointKey].position) / this.barWidth;
    }

    findNodeById(id) {
        var currentNode = this.root;
        while (currentNode.pointKey != id && currentNode != null)
            currentNode = currentNode.nextColor;

        return currentNode;
    }

    findPreviousRoot(percent){
        var currentNode = this.root;
        var nextNode = currentNode.nextColor;

        var leftPercent = this.getPointPercentage(currentNode.pointKey);
        var rightPercent = nextNode == null ? 100 : this.getPointPercentage(nextNode.pointKey); 

        while(leftPercent > percent || rightPercent < percent){
            currentNode = nextNode;
            nextNode = currentNode.nextColor;
            
            leftPercent = this.getPointPercentage(currentNode.pointKey);
            rightPercent = nextNode == null ? 100 : this.getPointPercentage(nextNode.pointKey);

            if(currentNode == null) return null; 
        }

        return currentNode;
    }

    addPointToHashMap(pointKey, pointPosition, leftPointIDParam, rightPointIDParam, pointColor){
        this.pointsDictionary[pointKey] = { 
                                        position : pointPosition, 
                                        leftPointID : leftPointIDParam,
                                        rightPointID : rightPointIDParam,
                                        color : pointColor
                                    };
    }

    /*
    * public region
    */

    GetKeys() {
        var result = [];
        var itemIndex = 0;

        var currentNode = this.root;
        while (currentNode != null) {
            result[itemIndex++] = currentNode.pointKey;
            currentNode = currentNode.nextColor;
        }

        return result;
    }

    AddNode(pointPositionOnSlider, color, pointID){
        /*calculate the percent*/
        var percent = this.GetPercentage(pointPositionOnSlider);
       
        /*get node neighbours*/
        var previousColor = this.findPreviousRoot(percent);
        var nextColor = previousColor.nextColor;

        if(previousColor == null) return;

        var colorNode = new ColorNode(pointID);
        
        this.addPointToHashMap(
            pointID,
            pointPositionOnSlider,
            previousColor.pointKey,
            nextColor == null ? null : nextColor.pointKey,
            color
        );

        this.pointsDictionary[previousColor.pointKey].rightPointID = pointID;
        if(nextColor != null) this.pointsDictionary[nextColor.pointKey].leftPointID = pointID;

        colorNode.nextColor = previousColor.nextColor;
        colorNode.prevColor = previousColor;
        previousColor.nextColor = colorNode; 
        if(nextColor != null) nextColor.prevColor = colorNode;
    }


    RemoveNode(pointID) { 
        var node = this.findNodeById(pointID);
        var previousNode = node.prevColor;
        var nextNode = node.nextColor;

        if (previousNode == null) return false;

        previousNode.nextColor = nextNode;
        this.pointsDictionary[previousNode.pointKey].rightPointID = nextNode == null ? null : nextNode.pointKey;

        if (nextNode != null) {
            nextNode.prevColor = previousNode;
            this.pointsDictionary[nextNode.pointKey].leftPointID =
                        previousNode == null ? null : previousNode.pointKey;
        }

        delete this.pointsDictionary[pointID];       

        return true;
    }

    BuildGradientString(){
        var result = 'linear-gradient(to right';
        var currentNode = this.root;
        var nextNode = this.root.nextColor;

        while (currentNode != null){
            nextNode = currentNode.nextColor; 

            var leftPercentage = this.getPointPercentage(currentNode.pointKey);
            var rightPercent = nextNode == null ? 100 : this.getPointPercentage(nextNode.pointKey);
            var color =  this.pointsDictionary[currentNode.pointKey].color;

            result = result + ',' + color + ' ' + leftPercentage + '%' +
                              ',' + color + ' ' + rightPercent + '%';
            
            currentNode = nextNode;                 
        }

        result += ')';
        return result;
    }

    GetPointMargins(pointID){
        var leftPointID = this.pointsDictionary[pointID].leftPointID;
        var rightPointID = this.pointsDictionary[pointID].rightPointID;
        return {
            left: this.pointsDictionary[leftPointID].position,
            right: rightPointID == null ? this.barWidth : this.pointsDictionary[rightPointID].position
        };
    }

    SetPointPosition(pointID, newPosition){
        this.pointsDictionary[pointID].position = newPosition;      
    }

    SetPointColor(pointID, newColor){
        this.pointsDictionary[pointID].color = newColor;  
    }

    /**
     * Set values used asociated with 0% and 100%
     * */
    SetValues(values) {
        this.leftValue = values.left;
        this.rightValue = values.right;
    }

    GetPercentage(position){
        return 100 * (position) / this.barWidth;
    }

    GetValue(percentage) {
        return ((this.rightValue - this.leftValue) * percentage) / 100 + this.leftValue;
    }

    GetColorMap() {
        
        var index = 0;
        var colorMap = [];
        var currentNodeKey = this.root.pointKey;
        
        while (currentNodeKey != null) {
            var rightNodeKey = this.pointsDictionary[currentNodeKey].rightPointID;
            colorMap[index++] = {
                Color: this.pointsDictionary[currentNodeKey].color,
                Left: this.GetValue ( this.GetPercentage(this.pointsDictionary[currentNodeKey].position) ),
                Right: this.GetValue ( rightNodeKey == null ? 100 : this.GetPercentage(this.pointsDictionary[rightNodeKey].position))
            }
            currentNodeKey = rightNodeKey;
        }

        return colorMap;
    }
}
