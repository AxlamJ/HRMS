
$(document).on('click', '#btn-create-position', function () {
    $("#add-new-position-modal").modal('show');
});


$(document).on('click', '.btn-edit-position', function () {

    var $self = $(this);

    var Id = $self.data('id');
    var positionname = $self.data('positionname');
    $(document).find('#add-update-position').data('id', Id);
    $(document).find("#position-name").val(positionname);

    $("#add-new-position-modal").modal('show');
});

// These are the constraints used to validate the form
var constraints = {
    "position-name": {
        // Site Name is required
        presence: {
            message: " is required."
        }
    }
};

$("#add-update-position").on("click", function () {
    var form = document.querySelector("#add-update-position-form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {
        var position = {};

        position.positionName = $("#position-name").val();
        var Id = $(document).find('#add-update-position').data('id') || null;

        if (Id != null) {
            position.id = Id;
        }
        //site.countryId

        var url = sitePath + 'api/EmployeePositionAPI/UpsertPosition';

        Ajax.post(url, position, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Position saved successfully.",
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
                    window.location.href = "/Home/ManagePositions";
                    if (result.isConfirmed) { }
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

        //showSuccess();
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
