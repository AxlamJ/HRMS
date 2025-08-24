var $j = jQuery.noConflict();

// These are the constraints used to validate the form
var constraints = {
    "password": {
        // Site Time Zone is required
        presence: { message: " is required." },
        length: {
            minimum: 8,
            message: "must be at least 8 characters"
        },
        format: {
            pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#%).(])[A-Za-z\d@$!%*?&#%).(]+$/,
            flags: "i",
            message: "must contain atleast one uppercase and one lower case letter, one digit and one special character"
        },
        equality: {
            attribute: "confirm_password",
            message: "do not match",
            comparator: function (v1, v2) {
                return JSON.stringify(v1) === JSON.stringify(v2);
            }
        }
    },
    "confirm_password": {
        // Site Time Zone is required
        presence: { message: " is required." },
        format: {
            pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#%).(])[A-Za-z\d@$!%*?&#%).(]+$/,
            flags: "i",
            message: "must contain atleast one uppercase and one lower case letter, one digit and one special character"
        },
        length: {
            minimum: 8,
            message: "must be at least 8 characters"
        }
    }
};


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


document.getElementById('toggleConfirmPassword').addEventListener('click', function () {
    var passwordInput = document.getElementById('confirm_password');
    var icon = document.getElementById('confirmeyeIcon');

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

$("#btn-update-password").on("click", function () {
    var form = document.querySelector("#update-password-form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {
        var user = {};
        user.password = $("#password").val();

        user.UserId = UserId;

        console.log(user);
        var url = sitePath + 'api/UserManagementAPI/UpdatePassword';

        Ajax.post(url, user, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Password Updated successfully.",
                    icon: "success",
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
                }).then(function (result) {
                    if (result.isConfirmed) {

                        window.location.reload();
                    }
                });
            }
            else {
                Swal.fire({
                    text: "Error occured. Please contact your system administrator.",
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
