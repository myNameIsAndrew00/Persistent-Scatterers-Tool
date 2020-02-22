
export var TooltipManagerInstance = new TooltipManager();

/**
*Use this class to handle the drawing of tooltips
*/
class TooltipManager{
    tooltipHandler = new Tooltip();
    enabled = true;

    id = 0;

    /**
     * Use this method to register an item which require a tooltip
     * @param {*} registerInfo Register parameters which can be used to customize the tooltip.
     * Available properties for registerInfo are:
     * @memberof containerId: (mandatory) the id of the container which requires the tooltip
     * @memberof content: (optional) can be used to fill the tooltip container
     * @memberof contentCallback: (optional) a function which can be called for filling the tooltip. The function must have an parameter representing the display tooltip method
     * @memberof delay: (optional) delay for the tooltip appeareance
     */
    Register(registerInfo){
        var self = this;
        var mousePosition = { X: 0, Y: 0};
        var display = false;

        $(`#${registerInfo.containerId}`).attr('data-hasTooltip',true); 
        
        $(`#${registerInfo.containerId}`).mousemove(function(evt){
            if(!self.enabled) return;

            mousePosition = { X: evt.clientX, Y: evt.clientY };
            var currentMousePosition = mousePosition;
            self.tooltipHandler.Hide();  

            display = true;

            setTimeout(
                function(){
                    if(currentMousePosition.X != mousePosition.X || currentMousePosition.Y != mousePosition.Y) return;
                    if(!display) return;
                    
                    if(registerInfo.contentCallback != null)
                            contentCallback( function (content) {
                                self.tooltipHandler.Display({ X: evt.clientX, Y: evt.clientY }, 
                                                            content);                        
                            });
                    else self.tooltipHandler.Display({ X: evt.clientX, Y: evt.clientY }, 
                                                        registerInfo.content,
                                                        registerInfo.delay);
                }, 
                registerInfo.delay);

        });

        $(`#${registerInfo.containerId}`).mouseout(function(evt){
            self.tooltipHandler.Hide();  
            display = false;
        });

    }

    /**
     * Use this method to enable or disable the tooltip
     * @param {*} enable 
     */
    Enable(enable){
        this.enabled = enable;
    }
}

/**
 * Use this class to handle a tooltip
 * */
class Tooltip{
    constructor(){
        this.container = this.buildTooltipStructure();
        $('body').append( this.container);
    }

    Display(position, content){      
        this.display(position,content);    
    }

    Hide(){
        this.container.innerHTML = '';
        this.container.classList.add('tooltip-hidden');
    }

/*==Private=================================================*/

    display(position, content){        
        this.container.appendChild(content);
        this.container.style.left = position.X + 'px';
        this.container.style.top = position.Y + 'px';
        this.container.classList.remove('tooltip-hidden');
    }

    buildTooltipStructure(){        
        var tooltipBody = document.createElement('div');
        tooltipBody.classList.add('tooltip');
        tooltipBody.classList.add('tooltip-hidden');
        return tooltipBody;
    }
}