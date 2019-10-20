
var oldColor = [180, 140, 140];
var newColor = [255, 80, 80];


export class PointsLayer extends ol.layer.Vector {

    createRenderer() { 
        return new ol.renderer.webgl.PointsLayer(this, {         
            vertexShader: `        
                    precision mediump float;       
                    uniform mat4 u_projectionMatrix;      
                    uniform mat4 u_offsetScaleMatrix;       
                    uniform float u_size;    
                    attribute vec2 a_position;    
                    attribute float a_index;       
                    attribute float a_weight;       
                    varying vec2 v_texCoord;     
                    varying float v_weight;      
                    void main(void) {    
                        mat4 offsetMatrix = u_offsetScaleMatrix;         
                        float offsetX = a_index == 0.0 || a_index == 3.0 ? -3.0 : 3.0 ;         
                        float offsetY = a_index == 0.0 || a_index == 1.0 ? -3.0 : 3.0 ;         
                        vec4 offsets = offsetMatrix * vec4(offsetX, offsetY, 0.0, 0.0);         
                        gl_Position = u_projectionMatrix * vec4(a_position, 0.0, 1.0) + offsets;         
                        float u = a_index == 0.0 || a_index == 3.0 ? 0.0 : 1.0;     
                        float v = a_index == 0.0 || a_index == 1.0 ? 0.0 : 1.0;         
                        v_texCoord = vec2(u, v);
                        v_weight = a_weight;
                    }`,
            fragmentShader: `precision mediump float;
                    uniform float u_blurSlope;
                    varying vec2 v_texCoord;
                    varying float v_weight;
                    void main(void) {
                        vec2 texCoord = v_texCoord * 2.0 - vec2(1.0, 1.0);
                        float sqRadius = texCoord.x * texCoord.x + texCoord.y * texCoord.y;
                        float value = (1.0 - sqrt(sqRadius)) * u_blurSlope;
                        float alpha = smoothstep(0.0, 1.0, value) * v_weight;         
                    gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);        
                    }`          
        });
    }
}