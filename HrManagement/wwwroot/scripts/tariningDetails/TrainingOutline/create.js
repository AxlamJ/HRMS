var descriptionQuill = null 

$(document).ready(function () {

    hideSpinner();

    ///Post Outline Section
    $("#Tarining-Outline-Form").validate({
        rules: {
            'Outline-Title' : { required: true, maxlength: 140 },

        },
        messages: {
            'Outline-Title': "Title is required",
        },
        submitHandler: function (form) {
            var post_detail = {
                title: $("#Outline-Title").val(),
                description: $("#Outline-Description").val(),
                trainingStructureId: $("#Training-Structure-Id").val(),
                status: $('input[name="visibility"]:checked').val(),
                id: $("#Id").val()||0,
            };
            var url = sitePath + 'api/Trainings/UpsertTrainingOutline';
            debugger;
            console.log(post_detail);
            Ajax.post(url, post_detail, function (response) {
                console.log(response);
                if (response.StatusCode === 200) {
                    Swal.fire({
                        text: "Outline Added successfully.",
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
                            loadLessonData();
                           
                        }
                    }).then(function () {
                    });
                } else {
                    showErrorModal("Error occurred. Please contact your system administrator.");
                }
            });

        }
    });

    //Trigger validation and submission manually
    $("#btn-Submit-TrainingOutline").on("click", function (e) {
        e.preventDefault();
        $("#Tarining-Outline-Form").submit();
    });
});