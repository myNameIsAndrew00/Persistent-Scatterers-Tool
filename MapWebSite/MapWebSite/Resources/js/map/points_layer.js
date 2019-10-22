
var oldColor = [180, 140, 140];
var newColor = [255, 80, 80];


export class PointsLayer extends ol.layer.Vector {

    createRenderer() { 
        return new ol.renderer.webgl.PointsLayer(this, {     
            attributes: [
                {
                    name: 'red',
                    callback: function (feature) {
                        return feature.color.r / 255.0;
                    }
                },
                {
                    name: 'blue',
                    callback: function (feature) {
                        return feature.color.b / 255.0;
                    }
                },
                {
                    name: 'green',
                    callback: function (feature) {
                        return feature.color.g / 255.0;
                    }
                }

            ],
            vertexShader: `        
                    precision mediump float;       
                    uniform mat4 u_projectionMatrix;      
                    uniform mat4 u_offsetScaleMatrix;        
                    attribute vec2 a_position;    
                    attribute float a_index;    

                    attribute float a_red;
                    attribute float a_blue;
                    attribute float a_green;
                    
                    varying vec4 v_color;     
                    void main(void) {    
                        
                        mat4 offsetMatrix = u_offsetScaleMatrix;         
                        float offsetX = a_index == 0.0 || a_index == 3.0 ? -3.0 : 3.0 ;         
                        float offsetY = a_index == 0.0 || a_index == 1.0 ? -3.0 : 3.0 ;         
                        vec4 offsets = offsetMatrix * vec4(offsetX, offsetY, 0.0, 0.0);         
                        
                        gl_Position = u_projectionMatrix * vec4(a_position, 0.0, 1.0) + offsets;         
                        float u = a_index == 0.0 || a_index == 3.0 ? 0.0 : 1.0;     
                        float v = a_index == 0.0 || a_index == 1.0 ? 0.0 : 1.0;         
                                             
                        v_color = vec4(a_red,a_green,a_blue,1.0);
                    }`,
            fragmentShader: `precision mediump float;                    
                    varying vec4 v_color; 

                    void main(void) {                                   
                        gl_FragColor = v_color;     
                    }`,
            hitVertexShader: `        
                    precision mediump float;       
                    uniform mat4 u_projectionMatrix;      
                    uniform mat4 u_offsetScaleMatrix;        
                    attribute vec2 a_position;    
                    attribute float a_index;    

                    attribute float a_red;
                    attribute float a_blue;
                    attribute float a_green;
                    
                    varying vec4 v_color;     
                    void main(void) {    
                        
                        mat4 offsetMatrix = u_offsetScaleMatrix;         
                        float offsetX = a_index == 0.0 || a_index == 3.0 ? -3.0 : 3.0 ;         
                        float offsetY = a_index == 0.0 || a_index == 1.0 ? -3.0 : 3.0 ;         
                        vec4 offsets = offsetMatrix * vec4(offsetX, offsetY, 0.0, 0.0);         
                        
                        gl_Position = u_projectionMatrix * vec4(a_position, 0.0, 1.0) + offsets;         
                        float u = a_index == 0.0 || a_index == 3.0 ? 0.0 : 1.0;     
                        float v = a_index == 0.0 || a_index == 1.0 ? 0.0 : 1.0;         
                                             
                        v_color = vec4(a_red,a_green,a_blue,1.0);
                    }`,
            hitFragmentShader: `
                    precision mediump float;                    
                    varying vec4 v_color; 

                    void main(void) {                                              
                        gl_FragColor = v_color;    
                    }`
        });
    }
}