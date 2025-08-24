var ContactDetails = null;


$(document).on("click", ".btn-edit-contact-details", function () {

    var form = document.querySelector("#contact-details-form");
    $(form).find("#office-phone-number").val(_EmployeeData.OfficePhoneNumber);
    $(form).find("#extension").val(_EmployeeData.Extension);
    $(form).find("#mobile-phone-number").val(_EmployeeData.PhoneNumber);
    $(form).find("#country").val(_EmployeeData.CountryId).trigger("change");
    $(form).find("#city").val(_EmployeeData.City);
    $(form).find("#state-region").val(_EmployeeData.State);
    $(form).find("#zip-postal-code").val(_EmployeeData.PostalCode);
    $(form).find("#address").val(_EmployeeData.Address);
    $('#modal-contact-info').modal('show');

});



// These are the constraints used to validate the form
var contactdetailsconstraints = {
    "office-phone-number": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "extension": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "mobile-phone-number": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "country": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "city": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "state-region": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "address": {
        // Site Time Zone is required
        presence: { message: "is required." }
    }

};




$("#btn-update-contact-details").on("click", function () {
    var form = document.querySelector("#contact-details-form");
    var errors = validate(form, contactdetailsconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var contact = {};

        //employee.Id = EmployeeData.Id;
        contact.EmployeeCode = parseInt(_EmployeeData.EmployeeCode);
        contact.officePhoneNumber = $(form).find("#office-phone-number").val();
        contact.extension = $(form).find("#extension").val();
        contact.phoneNumber = $(form).find("#mobile-phone-number").val();
        contact.country = $(form).find("#country :Selected").text();
        contact.countryId = $(form).find("#country").val();
        contact.city = $(form).find("#city").val();
        contact.state = $(form).find("#state-region").val();
        contact.postalCode = $(form).find("#zip-postal-code").val();
        contact.address = $(form).find("#address").val();

        if (Window.JustConsole) { console.log(contact); return; }
        var url = sitePath + 'api/EmployeeAPI/UpdateConatactDetails';

        Ajax.post(url, contact, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Employee contact details submitted successfully.",
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

$(document).on("click", "#nav-contact-information-tab", function () {
    $("#contact-email").text(_EmployeeData.Email);
    $("#contact-alter-email").text(_EmployeeData.AlternativeEmail);
    $("#contact-office-phone").text(_EmployeeData.PhoneNumber);
    $("#contact-extension").text(_EmployeeData.Extension);
    $("#contact-mobile").text(_EmployeeData.PhoneNumber);
    $("#contact-country").text(_EmployeeData.Country);
    $("#contact-address").text(_EmployeeData.Address);
    $("#contact-city").text(_EmployeeData.City);
    $("#contact-zip-code").text(_EmployeeData.PostalCode);
    $("#contact-state").text(_EmployeeData.State);

});



