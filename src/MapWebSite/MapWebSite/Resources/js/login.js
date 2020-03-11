/*! Module: Login
 *
 * Handles the login page client logic
 *
 * */

const logoImageId = '#mainlogo';

/* Register / login handling section*/
/**
 * This function is responsable for changing the page displayed on login (register or login)
 * @param {string} pageName represents the page which must be displayed (posible values: 'Register' and 'Login')
 */

function changePage(pageName) {
    if(event !== undefined) event.preventDefault();

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

    //todo: further checks for user credentials

    $.ajax({
        url: registerPath,
        type: "POST",
        data: {
            username: $('#register-form').find('input[name="username"]').val(),
            firstName: $('#register-form').find('input[name="firstName"]').val(),
            lastName: $('#register-form').find('input[name="lastName"]').val(),
            password: $('#register-form').find('input[name="password"]').val(),
            email: $('#register-form').find('input[name="email"]').val()
        },
        success: function (response) { 
            //change the visual color of the message and display the message
            $('#register-message').addClass(response.type != 'Success' ? 'register-error' : 'register-success');
            $('#register-message').removeClass(response.type != 'Success' ? 'register-success' : 'register-error');

            $('#register-message').text(response.message);
        }        
    })
}
