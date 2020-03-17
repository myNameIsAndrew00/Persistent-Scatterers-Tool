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
     * Use this field as a checking flag for writing
     * */
    writingFlag: 0,

    /**
     * Represents the id of the container which will contain popups
     * */
    popupsContainerId: 'main-content',

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

    validatePasswordsConfirmed: function () {
        var isValid = this.fields.password.val().length != 0 &&
            this.fields.confirmPassword.val().length != 0;

        isValid = isValid &&
            (this.fields.password.val() === this.fields.confirmPassword.val());

        return isValid;
    },

    /**
     * General purpose function which check if a input is valid or not
     * @param {any} inputName name of the input
     * @param {any} apiEndpoint api endpoint which will make the checking
     * @param {any} apiParameters the parameters required by api
     */
    validateInputFromApi: function (inputName, apiEndpoint, apiParameters, callback) {
        const tempWritingFlag = ++this.writingFlag;
        var self = this;

        function displayPopup(fieldName, message) {

            const inputPosition = self.fields[fieldName].offset();
            const inputWidth = parseInt(self.fields[fieldName].width(), 10);

            var div = document.createElement('p');
            div.innerHTML = message;

            PopupBuilderInstance.Create(self.popupsContainerId,
                { X: inputPosition.left + inputWidth, Y: inputPosition.top + 30 },
                div);
        }

        setTimeout(function () {
            if (tempWritingFlag != self.writingFlag) return;

            Router.Get(apiEndpoint,
                apiParameters,
                function (response) {
                    if (response.IsValid === false)
                        displayPopup(inputName, response.Message);
                    self.changeBorderColor(inputName, true, !response.IsValid);

                    if (callback !== undefined) callback(response.IsValid);
                });           
        }, 300);

    },

    validateInputsForSubmit: function () {
        var isValid = true;
        var self = this;

        Object.keys(this.fields).forEach(function (inputName) {          
            self.changeBorderColor(inputName);
            isValid = isValid && (self.fields[inputName].val().length != 0);
        });

        isValid = isValid && this.validatePasswordsConfirmed();

        this.checkUsernameInput(function (functionResponse) {
            isValid = isValid && functionResponse;
            self.checkEmailInput(function (functionResponse) {
                isValid = isValid && functionResponse;
                self.checkPasswordInput(function (functionResponse) {
                    isValid = isValid && functionResponse;
                }, true);
            }, true);           
        }, true);

        return isValid;
    },

    resetInputs: function () {
        var self = this;

        Object.keys(this.fields).forEach(function (inputName) {
            self.changeBorderColor(inputName, false);
            self.fields[inputName].val('');
        });

        PopupBuilderInstance.RemoveAll(this.popupsContainerId);
    },

    /**use this helper function to check the username input. Visual changes will be made if the input is invalid */
    checkUsernameInput: function (responseCallback, keepPreviousPopups) {
        if (keepPreviousPopups !== true) PopupBuilderInstance.RemoveAll(this.popupsContainerId);

        this.validateInputFromApi('username', endpoints.LoginApi.ValidateUsername,
            {
                username: this.fields.username.val()
            },
            responseCallback);
    },

    /**use this helper function to check the email input. Visual changes will be made if the input is invalid */
    checkEmailInput: function (responseCallback, keepPreviousPopups) {
        if (keepPreviousPopups !== true) PopupBuilderInstance.RemoveAll(this.popupsContainerId);

        this.validateInputFromApi('email', endpoints.LoginApi.ValidateEmail,
            {
                email: this.fields.email.val()
            }, responseCallback); 
    },

    /**use this helper function to check the passwords inputs. Visual changes will be made if the input is invalid */
    checkPasswordInput: function (responseCallback, keepPreviousPopups) {
        if (keepPreviousPopups !== true) PopupBuilderInstance.RemoveAll(this.popupsContainerId);

        this.changeBorderColor('confirmPassword', true, !(this.validatePasswordsConfirmed()));
        this.changeBorderColor('password', true, !(this.validatePasswordsConfirmed()));

        this.validateInputFromApi('password', endpoints.LoginApi.ValidatePassword,
            {
                password: this.fields.password.val()
            }, responseCallback);
    },

    initialiseInputs: function () { 
      
        var self = this;

        this.fields.username.on('keyup', function () {
            self.checkUsernameInput();
        });

        this.fields.firstName.on('keyup', function () {
            self.changeBorderColor('firstName');
        });
        this.fields.lastName.on('keyup', function () {
            self.changeBorderColor('lastName');
        });

        this.fields.password.on('keyup', function () {
            self.checkPasswordInput();
           });
        this.fields.confirmPassword.on('keyup', function () {                  
            self.checkPasswordInput();
        });
        this.fields.email.on('keyup', function () {
            self.checkEmailInput();
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

    if (!registerInputs.validateInputsForSubmit()) return;

    $.ajax({
        url: registerPath,
        type: "POST",
        data: {
            username: registerInputs.fields.username.val(),
            firstName: registerInputs.fields.firstName.val(),
            lastName: registerInputs.fields.lastName.val(),
            password: registerInputs.fields.password.val(),
            email: registerInputs.fields.email.val()
        },
        success: function (response) { 
            //change the visual color of the message and display the message
            if (response.type == 'Success') {                
                $('#message-content').text(response.message);
                changePage('Message');
                return;
            }

            $('#register-message').addClass(response.type != 'Success' ? 'message-error' : 'message-success');
            $('#register-message').removeClass(response.type != 'Success' ? 'message-success' : 'message-error');

            $('#register-message').text(response.message);
        }        
    })
}
