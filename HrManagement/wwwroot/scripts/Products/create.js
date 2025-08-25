


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



$("#add-online-training").on("click", function () {
    var training = {
        title: $("#training-title").val(),
        IsExternal: true,
        Url:$("#training-url").val()
    };
    console.log("Training object to send:", training);
    var url = sitePath + 'api/Trainings/UpsertTraining';
    Ajax.post(url, training, function (response) {
        console.log(response);
        if (response.StatusCode === 200) {

            const formData = new FormData();
            const id = response.Data.Id;
            formData.append('ReferenceId', id || 0);
            formData.append('ModuleName', "3");
            formData.append('FolderName', "images/Product/Logo");
            formData.append('Type', "TrainingThumbnail");

            var fileInput = $('#training-image')[0];
            if (fileInput.files.length > 0) {
                formData.append('File', fileInput.files[0]);
            }
            const token = localStorage.getItem('jwtToken');
            const url = sitePath + 'api/Trainings/UploadMedia';
            $.ajax({
                url: url,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                headers: {
                    'Authorization': `Bearer ${token}`
                },
                beforeSend: function () {
                    Ajax.enqueueSpinner();
                },
                success: function (res) {
                    if (res && res.Data) {
                        $('#VideoThumbnailId').val(res.Data.Id);
                        $('#videoThumbnailPreview_Img').attr('src', res.Data.FilePath);
                        Swal.fire('Success', 'Thumbnail uploaded successfully.', 'success');
                        let catId = $('#trainingcategory_Id').val();
                        window.location = `/Products/Index`;
                    } else {
                        Swal.fire('Error', 'Unexpected server response.', 'error');
                    }
                },
                error: function () {
                    showerrormodal('Upload failed. Please try again.');
                },
                complete: function () {
                    Ajax.dequeueSpinner();
                }
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

