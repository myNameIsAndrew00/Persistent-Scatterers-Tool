/*! Module: Login
 *
 * Handles the login page client logic
 *
 * */

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

    changeBorderColor: function (inputName, display = true) {
        if (!display) {
            this.fields[inputName].removeClass('input-error');
            this.fields[inputName].removeClass('input-valid');
            return;
        }

        (this.fields[inputName].val().length == 0) ? this.fields[inputName].addClass('input-error') : this.fields[inputName].removeClass('input-error');
        (this.fields[inputName].val().length != 0) ? this.fields[inputName].addClass('input-valid') : this.fields[inputName].removeClass('input-valid');
    },

    validateInputs: function () {
        var isValid = true;
        var self = this;

        Object.keys(this.fields).forEach(function (inputName) {          
            self.changeBorderColor(inputName);
            isValid = isValid && (self.fields[inputName].val().length != 0);
        });

        isValid = isValid && (this.password.val() === this.confirmPassword.val());

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
        var self = this;
        this.fields.username.on('keyup', function () {
            self.changeBorderColor('username');
        });
        this.fields.firstName.on('keyup', function () {
            self.changeBorderColor('firstName');
        });
        this.fields.lastName.on('keyup', function () {
            self.changeBorderColor('lastName');
        });
        this.fields.password.on('keyup', function () {
            self.changeBorderColor('password');
        });
        this.fields.confirmPassword.on('keyup', function () {
            self.changeBorderColor('confirmPassword');            
        });
        this.fields.email.on('keyup', function () {
            self.changeBorderColor('email');
        });
    }
}

registerInputs.initialiseInputs();

/* Register / login handling section*/
/**
 * This function is responsable for changing the page displayed on login (register or login)
 * @param {string} pageName represents the page which must be displayed (posible values: 'Register' and 'Login')
 */

function changePage(pageName) {
    if(event !== undefined) event.preventDefault();

    registerInputs.resetInputs();

    var current = pageName == 'Register' ? $('#register-form') : $('#login-form');
    var previous = pageName == 'Register' ? $('#login-form') : $('#register-form');

    pageName == 'Register' ? $(logoImageId).addClass('main-logo-hidden') : $(logoImageId).removeClass('main-logo-hidden');

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
            $('#register-message').addClass(response.type != 'Success' ? 'register-error' : 'register-success');
            $('#register-message').removeClass(response.type != 'Success' ? 'register-success' : 'register-error');

            $('#register-message').text(response.message);
        }        
    })
}
