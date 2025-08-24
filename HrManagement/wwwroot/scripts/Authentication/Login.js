// These are the constraints used to validate the form
var constraints = {
    "username": {
        // Site Name is required
        presence: {
            message: " is required."
        }
    },
    "password": {
        // Site Time Zone is required
        presence: { message: " is required." }
    }
};

var $j = jQuery.noConflict();


$("#btn-login").on("click", function () {
    var form = document.querySelector("#sign_in_form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {

        var UserName = $("#username").val();
        var Password = $("#password").val();

        var url = sitePath + 'Home/Login?userName='+UserName+'&password='+Password;

        Ajax.authenticate(url, null, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                var token = response.jwtToken;
                localStorage.setItem("jwtToken", token);
                //Swal.fire({
                //    text: "User Authenticated successfully.",
                //    icon: "success",
                //    buttonsStyling: false,
                //    confirmButtonText: "Ok",
                //    allowEscapeKey: false,
                //    allowOutsideClick: false,
                //    customClass: {
                //        confirmButton: "btn btn-primary",
                //    },
                //    didOpen: function () {
                //        hideSpinner();

                //    }
                //}).then(function (result) {
                //    if (result.isConfirmed) {

                        window.location.href = "/Home/Index";
                    //}
                //});
            }
            else {
                Swal.fire({
                    text: "User Name or Password is Incorrect.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary",
                    },
                    didOpen: function () {
                        hideSpinner();
                    }
                });
            }
        });
    }
    else {
        Swal.fire({
            text: "Please complete mandatory fields.",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok",
            allowEscapeKey: false,
            allowOutsideClick: false,
            customClass: {
                confirmButton: "btn btn-primary",
            },
            didOpen: function () {
                hideSpinner();
            }
        });
    }
});

document.getElementById('togglePassword').addEventListener('click', function () {
    var passwordInput = document.getElementById('password');
    var icon = document.getElementById('eyeIcon');

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        passwordInput.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
});

// Forgot Password functionality
$("#forgot-password-link").on("click", function() {
    Swal.fire({
        title: 'Reset Password',
        html: `
            <div class="form-group">
                <input type="email" id="reset-email" class="form-control" placeholder="Enter your email address">
            </div>
        `,
        showCancelButton: true,
        confirmButtonText: 'Send Reset Link',
        cancelButtonText: 'Cancel',
        showLoaderOnConfirm: true,
        preConfirm: () => {
            const email = $('#reset-email').val();
            if (!email) {
                Swal.showValidationMessage('Please enter your email address');
            }
            return email;
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const email = result.value;
            const url = sitePath + 'Home/ForgotPassword?email='+email;
            
            Ajax.post(url, { email: email }, function(response) {
                if (response.StatusCode === 200) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'Password reset instructions have been sent to your email.',
                        icon: 'success'
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: response.Message || 'Failed to process your request.',
                        icon: 'error'
                    });
                }
            }, {
                error: function(error) {
                    Swal.fire({
                        title: 'Error!',
                        text: error.Message || 'An error occurred while processing your request.',
                        icon: 'error'
                    });
                }
            });
        }
    });
});