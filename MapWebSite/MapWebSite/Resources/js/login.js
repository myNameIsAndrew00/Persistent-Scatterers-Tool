/*! Module: Login
 *
 * Handles the login page client logic
 *
 * */

 document.getElementById('canvas').height = window.innerHeight;
 document.getElementById('canvas').width = window.innerWidth;

 var canvasContext = document.getElementById('canvas').getContext("2d");

/*Drawer for circles effect section*/
/**
 * 
 * This class handle the circle which is displayed
 * 
 * */
 class Circle{
     constructor(positionX, positionY, radius){            
        this.positionX = positionX;
        this.positionY = positionY;
        this.radius = radius;
     }

     Draw(canvasContext, Color, waveMaxScale) {
         
        if(this.radius == 0) return;
        
        canvasContext.save();
             
        canvasContext.beginPath();         
        canvasContext.arc(this.positionY, this.positionX, this.radius, 0, 2 * Math.PI);
        canvasContext.strokeStyle = Color;  
        canvasContext.stroke(); 
        
        canvasContext.restore();
     }

     ResetPosition(newX, newY,radius){
         this.positionX = newX;
         this.positionY = newY;
         this.radius = radius;
     }

 };

/**
 * 
 * This represents a arc which is displayed
 * 
 * */
 class Arc{
     constructor(sourceX, sourceY, destinationX, destinationY){
         this.sourceX = sourceX;
         this.sourceY = sourceY;
         this.destinationX = destinationX;
         this.destinationY = destinationY;
     }
     
     Draw(canvasContext, Color) {
         
        if(this.radius == 0) return;
        
        canvasContext.save();
            
        canvasContext.beginPath();         
        canvasContext.moveTo(this.sourceY, this.sourceX);        
        canvasContext.bezierCurveTo(this.sourceY, this.destinationY / 2,
                                    this.destinationY, this.destinationY / 2, this.destinationY, this.destinationX);
        canvasContext.strokeStyle = Color;  
        
        canvasContext.stroke(); 
        
        canvasContext.restore();
     }


 }

/*
 * This class handles a wave which is displayed on screen background (uses circle class)
 * */
 class Wave{    
    constructor(positionX, positionY, waveMaxScale, circlesCount){
        this.arc = null;
        
        if (positionX === undefined || positionX === null ||
            positionY === undefined || positionY === null ||
            waveMaxScale === undefined || waveMaxScale === null ||
            circlesCount === undefined || circlesCount === null ) 
            this.initialiseInnerCircles();
        else
            this.InitialiseWave(positionX, positionY, waveMaxScale, circlesCount);
    }
    
    InitialiseWave(positionX, positionY, waveMaxScale, circlesCount){
        this.circles = [];
        this.circlesCount = circlesCount;
        this.waveMaxScale = waveMaxScale;

        /*Create the circles which the wave contains. Only the biggest wave is visible ~ radius = 1*/
        for(var i = 0; i < this.circlesCount; i++)
            this.circles[ i ] = new Circle(positionX, positionY, i == this.circlesCount - 1 ? 1 : 0);

        this.circles[ this.circlesCount ] = new Circle(positionX, positionY, 2);
    }

    Draw(canvasContext){
        /*Draw every circle*/
        /*Circles with index circlesCount represents the point in the middle*/
        for(var i = 0; i <= this.circlesCount; i++)
            this.circles[ i ].Draw(canvasContext, 
                                   "rgba(255,255,255," + (1 - this.circles[ i % this.circlesCount ].radius / this.waveMaxScale) + ")",
                                   this.waveMaxScale);
        
        /*Draw arc if exist*/
        //if(this.arc != null)
        //    this.arc.Draw(canvasContext, "rgb(255,255,255,1)");

        /*Try to reset the circles to another position if the cicles is complete*/
        this.ResetCircles();

        /*do not increase the radius of the i circle if the wave i + 1 is not 20% of the MaxWaveRadius*/
        for(var i = 0; i < this.circlesCount; i++)
            if(i == this.circlesCount - 1)
                this.circles[ i ].radius += 0.5;
            else{
                if(this.circles[ i + 1].radius > 20 / 100 * this.waveMaxScale)
                    this.circles[ i ].radius += 0.5;
            }
        
    }
 
    ResetCircles(){
        /*if the most little circle has the MaxWaveRadius*/
        if(this.circles[0].radius >= this.waveMaxScale){
            this.initialiseInnerCircles();
        }
    }

    initialiseInnerCircles(){
        var newPositionX = (Math.random() * 1000) % ( document.getElementById('canvas').height);
        var newPositionY = (Math.random() * 1000) % ( document.getElementById('canvas').width);
        var waveMaxScale = (Math.random() * 100) + 40;
        var circlesCount = Math.floor(Math.random() * 100) % 5 + 1;
        
        if(this.circles != undefined &&
           this.circles[ 0 ].positionX != undefined && this.circles[ 0 ].positionX != null 
           && this.circles[ 0 ].positionY != undefined && this.circles[ 0 ].positionY != null)
            this.arc = new Arc(this.circles[ 0 ].positionX, this.circles[ 0 ].positionY, newPositionX, newPositionY);

        this.InitialiseWave(newPositionX, newPositionY, waveMaxScale, circlesCount);                
    }
 };

 

 var waves = [];
 var wavesCount = 5;
 for(var i =0; i < wavesCount; i++)
    waves[ i ] = new Wave();
 /**
  * This function draw the waves in background
  * */
 function draw(){ 

    canvasContext.clearRect(0,0,window.innerWidth, window.innerHeight);
       
    for(var i =0; i < wavesCount; i++)
        waves[ i ].Draw(canvasContext);
    
    window.requestAnimationFrame(draw);   
     
 }
 
window.requestAnimationFrame(draw);


/* Register / login handling section*/
/**
 * This function is responsable for changing the page displayed on login (register or login)
 * @param {string} pageName represents the page which must be displayed (posible values: 'Register' and 'Login')
 */

function changePage(pageName) {
    if(event !== undefined) event.preventDefault();

    var current = pageName == 'Register' ? $('#register-form') : $('#login-form');
    var previous = pageName == 'Register' ? $('#login-form') : $('#register-form');

    current.removeClass('form-hidden');
    previous.addClass('form-hidden');

}

/**
 * 
 * This function handles the register request on register button pressed
 * @param {string} registerPath the path to the uri which require register data
 */
function register(registerPath) {
    event.preventDefault();

    $.ajax({
        url: registerPath,
        type: "POST",
        data: {
            username: $('#register-form').children('input[name="username"]').val(),
            firstName: $('#register-form').children('input[name="firstName"]').val(),
            lastName: $('#register-form').children('input[name="lastName"]').val(),
            password: $('#register-form').children('input[name="password"]').val()
        },
        success: function (response) { 
            //change the visual color of the message and display the message
            $('#register-message').addClass(response.type != 'Success' ? 'register-error' : 'register-success');
            $('#register-message').removeClass(response.type != 'Success' ? 'register-success' : 'register-error');

            $('#register-message').text(response.message);
        }        
    })
}
