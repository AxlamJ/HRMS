var descriptionQuill = null 

$(document).ready(function () {

    hideSpinner();

    ///Post Detail Section
    $("#Post_Details_form").validate({
        rules: {
            'post-detail-title-input' : { required: true, maxlength: 140 },

        },
        messages: {
            'post-detail-title-input': "Title is required",
        },
        submitHandler: function (form) {
            var post_detail = {
                title: $("#post-detail-title-input").val(),
                description: descriptionQuill.root.innerHTML,
                categoryId: $("#trainingcategory_Id").val(),
                status: $('input[name="post_detail_status_radio"]:checked').val(),
                id: $("#Id").val()||0,
            };
            var url = sitePath + 'api/Trainings/UpsertTrainingPostDetails';
            debugger;
            console.log(post_detail);
            Ajax.post(url, post_detail, function (response) {
                console.log(response);
                if (response.StatusCode === 200) {
                    Swal.fire({
                        text: " added successfully.",
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
    $("#post_detail_save_btn").on("click", function (e) {
        e.preventDefault();
        $("#Post_Details_form").submit();
    });
});