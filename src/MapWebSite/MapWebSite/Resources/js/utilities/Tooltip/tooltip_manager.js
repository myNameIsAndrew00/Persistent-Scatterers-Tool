/*!Component: Tooltip builder
 *
 * Handles tooltip drawing
 *
 * */

import { Router } from '../../api/api_router.js';
import { HtmlToElement } from '../utils.js';

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
     * @member containerId: (mandatory) the id of the container which requires the tooltip
     * @member content: (optional) can be used to fill the tooltip container
     * @member contentCallback: (optional) a function which can be called for filling the tooltip. The function must have an parameter representing the Display method (of the tooltip)
     *                            an example of function is: function callback(displayFunction){
     *                                          ...
     *                                          displayFunction(content) // trigger the tooltip
     *                                          ...
     *                            }
     * @member delay: (optional) delay for the tooltip appeareance
     * @member displayOverlay: (optional) enable an overlay when the popup is displayed
     * @member useRouter: (optional) enable router for providing the tooltip content
     * @member routerData: (optional, mandatory if useRouter is used): info required to request the tooltip
     * @member cursorSide: (optional) the side of the cursor where the tooltip will be displayed. Default is right.
     */
    Register(registerInfo){
        var self = this;
        var mousePosition = { X: 0, Y: 0};
        var displayEnabled = false;

        $(`#${registerInfo.containerId}`).attr('data-hasTooltip',true); 
        
        $(`#${registerInfo.containerId}`).mousemove(function (evt) {
            evt.stopPropagation();
            //function used to display the content
            function display(content) {
                self.tooltipHandler.Display({ X: evt.clientX, Y: evt.clientY },
                    content,
                    registerInfo.displayOverlay,
                    registerInfo.cursorSide
                );
            }

            if(!self.enabled) return;

            mousePosition = { X: evt.clientX, Y: evt.clientY };       
            const innerPosition = mousePosition;
            self.tooltipHandler.Hide();  

            displayEnabled = true;

            setTimeout(
                function(){
                    if (innerPosition.X != mousePosition.X || innerPosition.Y != mousePosition.Y) return;
                    console.log(mousePosition.X + " " + mousePosition.Y);
                    if (!displayEnabled) return;
                    displayEnabled = false;

                    if (registerInfo.useRouter === true)
                        Router.Get(registerInfo.routerData.endpoint,
                            { tooltip: registerInfo.routerData.tooltipId },
                            function (html) {
                                display(HtmlToElement(html));
                            });

                    else if (registerInfo.contentCallback != null) {
                        registerInfo.contentCallback(function (content) {
                            display(content)
                        });
                    }
                    else display(registerInfo.content);
                  
                }, 
                registerInfo.delay);

        });

        function hide() {
            self.tooltipHandler.Hide();
            displayEnabled = false;
        }

        $(`#${registerInfo.containerId}`).mouseout(function(evt){
            hide();
        });
        $(`#${registerInfo.containerId}`).click(function (evt) {
            hide();
        });
        $(`#${registerInfo.containerId}`).keypress(function (evt) {
            hide();
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
        this.buildTooltipStructure();
        this.buildOverlay();

        $('body').append(this.overlay);
        $('body').append(this.container);
    }

    Display(position, content, displayOverlay, cursorSide){      
        this.display(position, content, displayOverlay, cursorSide);    
    }

    Hide() {  
        this.container.innerHTML = '';
        this.container.classList.add('tooltip-hidden');
        this.overlay.classList.add('tooltip-overlay-hidden');
         
    }

/*==Private=================================================*/

    display(coordinates, content, displayOverlay, position) {
        this.container.innerHTML = '';   
        this.container.appendChild(content);

        this.container.classList.remove('tooltip-hidden');

        this.container.style.left = coordinates.X - (position === 'left' ? 220 : 0) + 'px';
        this.container.style.top = coordinates.Y + 'px';
        if(displayOverlay === true) this.overlay.classList.remove('tooltip-overlay-hidden');
         
    }

    buildTooltipStructure(){        
        this.container = document.createElement('div');
        this.container.classList.add('tooltip');
        this.container.classList.add('box');
        this.container.classList.add('tooltip-hidden');         
    }

    buildOverlay() {
        this.overlay = document.createElement('div');
        this.overlay.classList.add('tooltip-overlay');
        this.overlay.classList.add('tooltip-overlay-hidden');
    }
}


export var TooltipManagerInstance = new TooltipManager();
 