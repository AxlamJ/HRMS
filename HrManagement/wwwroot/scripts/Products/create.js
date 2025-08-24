


var constraints = {
    "training-title": {
        presence: {
            message: " is required."
        }
    }
};


$("#add-update-training").on("click", function () {

    var training = {
        title: $("#training-title").val()
    };

    

        console.log("Training object to send:", training);

        var url = sitePath + 'api/Trainings/UpsertTraining';

    Ajax.post(url, training, function (response) { 
        console.log(response);
            if (response.StatusCode === 200) {
                Swal.fire({
                    text: "Training added successfully.",
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
                }).then(function () {
                    const id = response.Data.Id;

                    window.location.href = `/traning/course_detail/${id}`;
                });
            } else {
                showErrorModal("Error occurred. Please contact your system administrator.");
            }
        });
   
});


function showErrorModal(message) {
    Swal.fire({
        text: message,
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

