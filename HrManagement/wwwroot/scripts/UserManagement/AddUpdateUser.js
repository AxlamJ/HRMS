var UserId = getParameterByName("UserId") || null;
var UserData = null;
HRMSUtil.onDOMContentLoaded(function () {

    PopulateDropDowns(function () {
        if (UserId) {
            $("#employee-code").attr("disabled", "disabled");
            $("#password").attr("disabled", "disabled").parent().parent().addClass('d-none');
            $("#confirm_password").attr("disabled", "disabled").parent().parent().addClass('d-none');
            loadData()
        }
    });


    unspin();
});

function PopulateDropDowns(cb) {

    PopulateSites(function () {
        PopulateRoles(function () {
            if (cb) {
                cb();
            }
        })
    });

}

function PopulateRoles(cb) {

    var ddRoles = document.querySelector('#roles');
    $(ddRoles).empty();
    var lstRoles = (DropDownsData.Roles || []);
    var option = new Option();
    ddRoles.appendChild(option);
    lstRoles.forEach(function (item) {

        var option = new Option(item.RoleName, item.RoleId, false, false);
        ddRoles.appendChild(option);
    });

    if (cb) {
        cb();
    }

}


function PopulateSites(cb) {

    var ddSites = document.querySelector('#sites');
    $(ddSites).empty();
    var lstSites = (DropDownsData.Sites || []);
    var option = new Option();
    ddSites.appendChild(option);
    lstSites.forEach(function (item) {

        var option = new Option(item.SiteName, item.Id, false, false);
        ddSites.appendChild(option);
    });

    if (cb) {
        cb();
    }

}


// These are the constraints used to validate the form
var constraints = {
    "employee-code": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "username": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "firstname": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "lastname": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "email": {
        // Site Time Zone is required
        presence: { message: "is required." },
        email: true
    },
    "phone": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "gender": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "roles": {
        // Site Time Zone is required
        presence: { message: " is required.", allowEmpty: false }
    },
    "sites": {
        // Site Time Zone is required
        presence: { message: "is required.", allowEmpty: false }
    },
    "password": {
        // Site Time Zone is required
        presence: { message: "is required." },
        length: {
            minimum: 8,
            message: "must be at least 8 characters"
        },
        format: {
            pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/,
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
            pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/,
            flags: "i",
            message: "must contain atleast one uppercase and one lower case letter, one digit and one special character"
        },
        length: {
            minimum: 8,
            message: "must be at least 8 characters"
        }
    }
};

// These are the constraints used to validate the form
var updateconstraints = {
    "employee-code": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "username": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "firstname": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "lastname": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "email": {
        // Site Time Zone is required
        presence: { message: "is required." },
        email: true
    },
    "phone": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "gender": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "roles": {
        // Site Time Zone is required
        presence: { message: " is required.", allowEmpty: false }
    },
    "sites": {
        // Site Time Zone is required
        presence: { message: "is required.", allowEmpty: false }
    }
};


$("#add-update-user").on("click", function () {
    var form = document.querySelector("#add-update-site-form");
    var ValidationConstraints = constraints;
    if (UserId != null) {
        ValidationConstraints = updateconstraints;
    }
    var errors = validate(form, ValidationConstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var user = {};

        user.employeeCode = ($("#employee-code").val() != '' && $("#employee-code").val() != null && $("#employee-code").val() != 0) ? parseInt($("#employee-code").val()) : null;
        user.userName = $("#username").val();
        user.firstName = $("#firstname").val();
        user.lastName = $("#lastname").val();
        user.gender = $("#gender").val();
        user.dob = $("#dob").val();
        user.email = $("#email").val();
        user.phoneNumber = $("#phone").val();
        user.userRoles = $("#roles").val();
        user.userSites = JSON.stringify($("#sites").val());
        user.password = $("#password").val();

        if (UserId != null) {
            user.userId = UserId;
        }
        //site.countryId

        console.log(user);
        var url = sitePath + 'api/UserManagementAPI/UpsertUsers';

        Ajax.post(url, user, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "User created successfully.",
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

                        window.location.href = "/UserManagement/ManageUsers";
                    }
                });
            }
            else if (response.StatusCode == 409) {
                Swal.fire({
                    text: "Enter valid Emplooye Code.",
                    icon: "warning",
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

                        window.location.href = "/UserManagement/ManageUsers";
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



function loadData() {
    var url = sitePath + "api/UserManagementAPI/GetUserById?UserId=" + UserId;
    Ajax.get(url, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            UserData = resp.User;
            $("#employee-code").val(UserData.EmployeeCode);
            $("#username").val(UserData.UserName);
            $("#firstname").val(UserData.FirstName);
            $("#lastname").val(UserData.LastName);
            $("#gender").val(UserData.Gender).trigger("change");

            $("#email").val(UserData.Email);
            $("#phone").val(UserData.PhoneNumber);
            $("#roles").val(UserData.UserRoles).trigger("change");
            $("#sites").val(JSON.parse(UserData.UserSites)).trigger("change");
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
