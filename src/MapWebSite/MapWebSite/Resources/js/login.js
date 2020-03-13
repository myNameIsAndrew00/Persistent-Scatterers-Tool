/*! Module: Login
 *
 * Handles the login page client logic
 *
 * */
import { PopupBuilderInstance } from './utilities/Popup/popup.js';
import { Router, endpoints } from './api/api_router.js';

const logoImageId = '#mainlogo';    

var registerInputs = {

    fields: {
        username: $('#register-form').find('input[name="username"]'),
        firstName: $('#register-form').find('input[name="firstName"]'),
        lastName: $('#register-form').find('input[name="lastName"]'),
        password: $('#register-form').find('input[name="password"]'),
        confirmPassword: $('#register-form').find('input[name="confirmPassword"]'),
        email: $('#register-form').find('input[name="email"]')
    },

    /**
     * Change the border color of an input
     * @param {any} inputName the name of the input which must be changed
     * @param {any} display set to true if the border must be displayed
     * @param {any} invalid set to true if the border must represent an invalid input
     */
    changeBorderColor: function (inputName, display = true, invalid = false) {
        
        if (!display) {
            this.fields[inputName].removeClass('input-error');
            this.fields[inputName].removeClass('input-valid');
            return;
        }

        if (invalid) {
            this.fields[inputName].removeClass('input-valid');
            this.fields[inputName].addClass('input-error');
            return;
        }

        (this.fields[inputName].val().length == 0) ? this.fields[inputName].addClass('input-error') : this.fields[inputName].removeClass('input-error');
        (this.fields[inputName].val().length != 0) ? this.fields[inputName].addClass('input-valid') : this.fields[inputName].removeClass('input-valid');
    },

    validatePasswords: function () {
        var isValid = this.fields.password.val().length != 0 &&
            this.fields.confirmPassword.val().length != 0;

        isValid = isValid &&
            (this.fields.password.val() === this.fields.confirmPassword.val());

        return isValid;
    },

    validateInputs: function () {
        var isValid = true;
        var self = this;

        Object.keys(this.fields).forEach(function (inputName) {          
            self.changeBorderColor(inputName);
            isValid = isValid && (self.fields[inputName].val().length != 0);
        });

        isValid = isValid && this.validatePasswords();

        return isValid;
    },

    resetInputs: function () {
        var self = this;

        Object.keys(this.fields).forEach(function (inputName) {
            self.changeBorderColor(inputName, false);
            self.fields[inputName].val('');
        });
    },

    initialiseInputs: function () {

        function displayPopup(fieldName, message) {
            
            const inputPosition = self.fields[fieldName].offset();
            const inputWidth = parseInt(self.fields[fieldName].width(), 10);

            var div = document.createElement('p');
            div.innerHTML = message;

            PopupBuilderInstance.Create('main-content',
                    { X: inputPosition.left + inputWidth, Y: inputPosition.top + 30 },
                    div);            
        }

        var self = this;
        var writingFlag = 0;


        this.fields.username.on('keyup', function () {
            self.changeBorderColor('username');
            const tempWritingFlag = ++writingFlag;

            setTimeout(function () {
                if (tempWritingFlag != writingFlag) return;
                Router.Get(endpoints.LoginApi.ValidateUsername,
                    {
                        username : self.fields.username.val()
                    },
                    function (response) {
                        if (response.IsValid === false)
                            displayPopup(response.Message);
                    });
            }, 300);

        });

        this.fields.firstName.on('keyup', function () {
            self.changeBorderColor('firstName');
        });
        this.fields.lastName.on('keyup', function () {
            self.changeBorderColor('lastName');
        });
        this.fields.password.on('keyup', function () {              
            self.changeBorderColor('confirmPassword', true, !(self.validatePasswords()));
            self.changeBorderColor('password', true, !(self.validatePasswords()));            
        });
        this.fields.confirmPassword.on('keyup', function () {                  
            self.changeBorderColor('confirmPassword', true, !(self.validatePasswords()));
            self.changeBorderColor('password', true, !(self.validatePasswords()));             
        });
        this.fields.email.on('keyup', function () {
            self.changeBorderColor('email');
        });
    }
}

window.showPlainPassword = function showPlainPassword(inputName, button) {
    registerInputs.fields[inputName].prop('type', 'text');

    button.onmouseup = function () {
        registerInputs.fields[inputName].prop('type', 'password');
    }

}

registerInputs.initialiseInputs();





/* Register / login handling section*/
/**
 * This function is responsable for changing the page displayed on login (register or login)
 * @param {string} pageName represents the page which must be displayed (posible values: 'Register' and 'Login')
 */

window.changePage = function changePage(pageName) {
    if (event !== undefined) event.preventDefault();
    const pages = [{
        pageName: 'Register',
        id: '#register-form'
    },
    {
        pageName: 'Login',
        id: '#login-form'
    },
    {
        pageName: 'Message',
        id: '#messages-container'
    }];
  
    registerInputs.resetInputs();     
    pageName == 'Login' ? $(logoImageId).removeClass('main-logo-hidden') : $(logoImageId).addClass('main-logo-hidden');

    for (var i = 0; i < pages.length; i++) {
        pages[i].pageName == pageName ? $(pages[i].id).removeClass('form-hidden') : $(pages[i].id).addClass('form-hidden');        
    }

}


/**
 * 
 * This function handles the register request on register button pressed
 * @param {string} registerPath the path to the uri which require register data
 */
window.register = function register(registerPath) {
    event.preventDefault();

      

    if (!registerInputs.validateInputs()) return;


    //todo: further checks for user credentials
    if (!validateInputs()) return;

    $.ajax({
        url: registerPath,
        type: "POST",
        data: {
            username: registerInputs.username.val(),
            firstName: registerInputs.firstName.val(),
            lastName: registerInputs.lastName.val(),
            password: registerInputs.password.val(),
            email: registerInputs.email.val()
        },
        success: function (response) { 
            //change the visual color of the message and display the message
            if (response.type == 'Success') {
                $('#message-content').text(response.message);
                changePage('Message');
                return;
            }

            $('#register-message').addClass(response.type != 'Success' ? 'register-error' : 'register-success');
            $('#register-message').removeClass(response.type != 'Success' ? 'register-success' : 'register-error');

            $('#register-message').text(response.message);
        }        
    })
}
