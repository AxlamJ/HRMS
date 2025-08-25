$(document).ready(function () {

    $('input[name="paymentType"]').change(function () {
        if ($('#paymentPaid').is(':checked')) {
            $('#paymentAmountContainer').removeClass('d-none');
        } else {
            $('#paymentAmountContainer').addClass('d-none');
            $('#paymentAmount').val('');
        }
    });

    hideSpinner();

    ///Detail Section
    $("#frmProductEditor").validate({
        rules: {
            txtTitle: { required: true, maxlength: 140 },
            txtDescription: { required: true },
            txtInstructorHeading: { required: true },
            txtInstructorName: { required: true },
            txtInstructorTitle: { required: true },
            txtInstructorBio: { required: true, maxlength: 1000 }
        },
        messages: {
            txtTitle: "Title is required",
            txtDescription: "Description is required",
            txtInstructorHeading: "Heading is required",
            txtInstructorName: "Name is required",
            txtInstructorTitle: "Title is required",
            txtInstructorBio: {
                required: "Bio is required",
                maxlength: "Max 1000 characters allowed"
            }
        },
        submitHandler: function (form) {
            var trainingdetail = {
                title: $("#txtTitle").val(),
                description: $("#txtDescription").val(),
                instructorHeading: $("#txtInstructorHeading").val(),
                instructorName: $("#txtInstructorName").val(),
                instructorTitle: $("#txtInstructorTitle").val(),
                instructorBio: $("#txtInstructorBio").val(),
                trainingId: $("#detail_trainingId").val(),
                trainingsDetailId: $("#TrainingsDetailId").val(),
                paymentType: $('#paymentPaid').is(':checked'), // true if Paid, false if Free
                amount: $('#paymentPaid').is(':checked') ? parseFloat($('#paymentAmount').val()) || 0 : 0
            };
            var url = sitePath + 'api/Trainings/UpsertTrainingsDetail';
            Ajax.post(url, trainingdetail, function (response) { // ✅ Corrected this line
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
                            loadTrainingData();
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
    $("#btnSaveChanges").on("click", function (e) {
        e.preventDefault();
        $("#frmProductEditor").submit();
    });



    //Cancel button
    $("#btnCancel").on("click", function () {
        window.history.back(); // or clear form
    });



    //Outline Section

    $("#add-training-structure-category-form").validate({
        rules: {
            training_structure_category_title: { required: true, maxlength: 140 },

        },
        messages: {
            training_structure_category_title: "Title is required",

        },
        submitHandler: function (form) {
            var training_outline_category = {
                title: $("#training_structure_category_title").val(),
                trainingId: $("#detail_trainingId").val(),

            };
            var url = sitePath + 'api/Trainings/UpsertTrainingStructure';
            Ajax.post(url, training_outline_category, function (response) { // ✅ Corrected this line
                console.log(response);
                if (response.StatusCode === 200) {
                    Swal.fire({
                        text: "Training Category added successfully.",
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
                            loadTrainingData();
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
    $("#add-outline-training-Category").on("click", function (e) {
        e.preventDefault();
        $("#add-training-structure-category-form").submit();
    });





    $("#add-training-lesson-category-form").validate({
        rules: {
            txt_AddLesson: { required: true, maxlength: 140 },

        },
        messages: {
            txt_AddLesson: "Title is required",

        },
        submitHandler: function (form) {
            var training_outline_category = {
                title: $("#txt_AddLesson").val(),
                type: $("#trainingtyep_Id").val(),
                trainingStructureId: $("#training_structureId").val(),
                trainingId: $("#detail_trainingId").val(),
                subCategoryId: $("#subCategoryId").val() || null,

            };

            console.log(training_outline_category);

            var url = sitePath + 'api/Trainings/UpsertTrainingStructureCategory';
            Ajax.post(url, training_outline_category, function (response) { // ✅ Corrected this line
                console.log(response);
                if (response.StatusCode === 200) {
                    Swal.fire({
                        text: "Training Category added successfully.",
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
                            loadTrainingData();
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
    $("#btn-AddLesson-add-training").on("click", function (e) {
        e.preventDefault();
        $("#add-training-lesson-category-form").submit();
    });


});


///Image Upload Product Thumbnail
const dropArea = document.getElementById('dropArea');
//const PostimageThumbnail = document.getElementById('Product-Thumbnail-Image');
const ProductThumbnailImage = document.getElementById('Product-Thumbnail-Image');
//const imagePreview = document.getElementById('ImagePreview-Product-Thumbnail');
const ImagePreviewProductThumbnail = document.getElementById('ImagePreview-Product-Thumbnail');
//const previewContainer = document.getElementById('previewContainer-Product-Thumbnail');
const previewContainerProductThumbnail = document.getElementById('previewContainer-Product-Thumbnail');
//const removeImageBtn = document.getElementById('Product-Thumbnail-RemoveBtn');
const ProductThumbnailRemoveBtn = document.getElementById('Product-Thumbnail-RemoveBtn');
const ProductThumbnailUploadBtn = document.getElementById('Product-Thumbnail-UploadBtn');

// Click on drop area triggers file input
dropArea.addEventListener('click', () => ProductThumbnailImage.click());

// Keyboard accessibility: Enter or Space triggers file input
dropArea.addEventListener('keydown', e => {
    if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        ProductThumbnailImage.click();
    }
});

// Handle file input change
ProductThumbnailImage.addEventListener('change', e => {
    const file = e.target.files[0];
    handleFile(file);
});

// Drag & Drop handlers
dropArea.addEventListener('dragover', e => {
    e.preventDefault();
    dropArea.classList.add('dragover');
});

dropArea.addEventListener('dragleave', e => {
    e.preventDefault();
    dropArea.classList.remove('dragover');
});

dropArea.addEventListener('drop', e => {
    e.preventDefault();
    dropArea.classList.remove('dragover');
    const file = e.dataTransfer.files[0];
    handleFile(file);
});

function handleFile(file) {
    if (!file) return;

    // Validate file type (image only)
    if (!file.type.startsWith('image/')) {
        alert('Only image files are allowed!');
        resetPreview();
        return;
    }

    // Preview the image
    const reader = new FileReader();
    reader.onload = e => {
        ImagePreviewProductThumbnail.src = e.target.result;
        previewContainerProductThumbnail.style.display = 'block';
        ProductThumbnailUploadBtn.disabled = false;
    };
    reader.readAsDataURL(file);
}

ProductThumbnailRemoveBtn.addEventListener('click', () => {
    resetPreview();
    ProductThumbnailImage.value = '';
    ProductThumbnailUploadBtn.disabled = true;
});

function resetPreview() {
    previewContainerProductThumbnail.style.display = 'none';
    ImagePreviewProductThumbnail.src = '';
}

ProductThumbnailUploadBtn.addEventListener('click', () => {
    const file = ProductThumbnailImage.files[0];
    if (!file) return alert('No file selected!');
    fileUpload();
    const modal = bootstrap.Modal.getInstance(document.getElementById('ProductThumbnailModal'));
    modal.hide();
    // Reset after upload
    resetPreview();
    ProductThumbnailImage.value = '';
    ProductThumbnailUploadBtn.disabled = true;
});

function fileUpload() {
    const formData = new FormData();
    formData.append('Id', $('#ProductThumbnailId').val() || 0);
    formData.append('ReferenceId', $('#outline_trainingId').val() || 0);
    formData.append('ModuleName', "3");
    formData.append('FolderName', "images/Training/Detail");
    formData.append('Type', "TrainingThumbnail");
    const imageFile = $('#Product-Thumbnail-Image')[0].files[0];
    formData.append('File', imageFile);


    var url = sitePath + 'api/Trainings/UploadMedia';
    const token = localStorage.getItem('jwtToken');
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

            if (res && res.Data != null) {
                console.log(res);
                $('#Image-Product-Thumbnail').attr('src', res.Data.FilePath);
                $('#ProductThumbnailId').val(res.Data.Id);
                $('#btnRemoveImage').removeClass('d-none');
                $('#btnReplaceImage').removeClass('d-none');
                $('#btnSelectImage').addClass('d-none');
            }


            swal.fire({
                text: "File added successfully.",
                icon: "success",
                buttonsstyling: false,
                confirmbuttontext: "ok",
                allowescapekey: false,
                allowoutsideclick: false,
                customclass: {
                    confirmbutton: "btn btn-primary",
                },
                didopen: function () {
                    hideSpinner();
                }
            }).then(function () {
            });
        },
        error: function (err) {
            showerrormodal("error occurred. please contact your system administrator.");
        },
        complete: function () {
            Ajax.dequeueSpinner();
        }
    });
}

///Headshot
const dropAreaProductHeadshot = document.getElementById('dropArea-ProductHeadshot');
const inputProductHeadshot = document.getElementById('ProductHeadshotImage');
const previewProductHeadshot = document.getElementById('ImagePreview-ProductHeadshot');
const previewContainerProductHeadshot = document.getElementById('previewContainer-ProductHeadshot');
const removeProductHeadshotBtn = document.getElementById('ProductHeadshotRemoveBtn');
const uploadProductHeadshotBtn = document.getElementById('ProductHeadshotUploadBtn');

// Open file dialog on click
dropAreaProductHeadshot.addEventListener('click', () => inputProductHeadshot.click());

dropAreaProductHeadshot.addEventListener('keydown', e => {
    if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        inputProductHeadshot.click();
    }
});

inputProductHeadshot.addEventListener('change', e => {
    const file = e.target.files[0];
    handleHeadshotFile(file);
});

dropAreaProductHeadshot.addEventListener('dragover', e => {
    e.preventDefault();
    dropAreaProductHeadshot.classList.add('dragover');
});

dropAreaProductHeadshot.addEventListener('dragleave', e => {
    e.preventDefault();
    dropAreaProductHeadshot.classList.remove('dragover');
});

dropAreaProductHeadshot.addEventListener('drop', e => {
    e.preventDefault();
    dropAreaProductHeadshot.classList.remove('dragover');
    const file = e.dataTransfer.files[0];
    handleHeadshotFile(file);
});

function handleHeadshotFile(file) {
    if (!file) return;

    if (!file.type.startsWith('image/')) {
        alert('Only image files are allowed!');
        resetHeadshotPreview();
        return;
    }

    const reader = new FileReader();
    reader.onload = e => {
        previewProductHeadshot.src = e.target.result;
        previewContainerProductHeadshot.style.display = 'block';
        uploadProductHeadshotBtn.disabled = false;
    };
    reader.readAsDataURL(file);
}

function resetHeadshotPreview() {
    previewContainerProductHeadshot.style.display = 'none';
    previewProductHeadshot.src = '';
    uploadProductHeadshotBtn.disabled = true;
}

removeProductHeadshotBtn.addEventListener('click', () => {
    resetHeadshotPreview();
    inputProductHeadshot.value = '';
});

uploadProductHeadshotBtn.addEventListener('click', () => {
    const file = inputProductHeadshot.files[0];
    if (!file) return alert('No file selected!');
    uploadProductHeadshot(file);
    const modal = bootstrap.Modal.getInstance(document.getElementById('ProductHeadshotModal'));
    modal.hide();
    resetHeadshotPreview();
    inputProductHeadshot.value = '';
});

function uploadProductHeadshot(file) {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('Id', $('#ProductHeadshotId').val() || 0);
    formData.append('ReferenceId', $('#outline_trainingId').val() || 0);
    formData.append('ModuleName', "3");
    formData.append('FolderName', "images/Product/Headshots");
    formData.append('Type', "ProductHeadshot");

    const url = sitePath + 'api/Trainings/UploadMedia';
    const token = localStorage.getItem('jwtToken');

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
                $('#Image-ProductHeadshot').attr('src', res.Data.FilePath);
                $('#ProductHeadshotId').val(res.Data.Id);
                $('#btnReplaceProductHeadshot').removeClass('d-none');
                $('#btnRemoveProductHeadshot').removeClass('d-none');
                $('#btnUploadProductHeadshot').addClass('d-none');
            }

            swal.fire({
                text: "Headshot uploaded successfully.",
                icon: "success",
                buttonsstyling: false,
                confirmbuttontext: "ok",
                allowescapekey: false,
                allowoutsideclick: false,
                customclass: {
                    confirmbutton: "btn btn-primary",
                },
                didopen: function () {
                    hideSpinner();
                }
            });
        },
        error: function () {
            showerrormodal("An error occurred. Please contact your system administrator.");
        },
        complete: function () {
            Ajax.dequeueSpinner();
        }
    });
}




const dropAreaProductLogoImage = document.getElementById('dropArea-ProductLogoImage');
const inputProductLogoImage = document.getElementById('ProductLogoImageFile');
const previewProductLogoImage = document.getElementById('ImagePreview-ProductLogoImage');
const previewContainerProductLogoImage = document.getElementById('previewContainer-ProductLogoImage');
const removeProductLogoImageBtn = document.getElementById('ProductLogoImageRemoveBtn');
const uploadProductLogoImageBtn = document.getElementById('ProductLogoImageUploadBtn');

// Open file dialog on click
dropAreaProductLogoImage.addEventListener('click', () => inputProductLogoImage.click());

dropAreaProductLogoImage.addEventListener('keydown', e => {
    if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        inputProductLogoImage.click();
    }
});

inputProductLogoImage.addEventListener('change', e => {
    const file = e.target.files[0];
    handleLogoImageFile(file);
});

dropAreaProductLogoImage.addEventListener('dragover', e => {
    e.preventDefault();
    dropAreaProductLogoImage.classList.add('dragover');
});

dropAreaProductLogoImage.addEventListener('dragleave', e => {
    e.preventDefault();
    dropAreaProductLogoImage.classList.remove('dragover');
});

dropAreaProductLogoImage.addEventListener('drop', e => {
    e.preventDefault();
    dropAreaProductLogoImage.classList.remove('dragover');
    const file = e.dataTransfer.files[0];
    handleLogoImageFile(file);
});

function handleLogoImageFile(file) {
    if (!file) return;

    if (!file.type.startsWith('image/')) {
        alert('Only image files are allowed!');
        resetLogoImagePreview();
        return;
    }

    const reader = new FileReader();
    reader.onload = e => {
        previewProductLogoImage.src = e.target.result;
        previewContainerProductLogoImage.style.display = 'block';
        uploadProductLogoImageBtn.disabled = false;
    };
    reader.readAsDataURL(file);
}

function resetLogoImagePreview() {
    previewContainerProductLogoImage.style.display = 'none';
    previewProductLogoImage.src = '';
    uploadProductLogoImageBtn.disabled = true;
}

removeProductLogoImageBtn.addEventListener('click', () => {
    resetLogoImagePreview();
    inputProductLogoImage.value = '';
});

uploadProductLogoImageBtn.addEventListener('click', () => {
    const file = inputProductLogoImage.files[0];
    if (!file) return alert('No file selected!');
    uploadProductLogoImage(file);
    const modal = bootstrap.Modal.getInstance(document.getElementById('ProductLogoImageModal'));
    modal.hide();
    resetLogoImagePreview();
    inputProductLogoImage.value = '';
});

function uploadProductLogoImage(file) {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('Id', $('#ProductLogoImageId').val() || 0);
    formData.append('ReferenceId', $('#outline_trainingId').val() || 0);
    formData.append('ModuleName', "3");
    formData.append('FolderName', "images/Product/Logo");
    formData.append('Type', "ProductLogoImage");

    const url = sitePath + 'api/Trainings/UploadMedia';
    const token = localStorage.getItem('jwtToken');
    debugger;

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
                $('#Image-ProductLogoImage').attr('src', res.Data.FilePath);
                $('#ProductLogoImageId').val(res.Data.Id);
                $('#btnReplaceProductLogoImage').removeClass('d-none');
                $('#btnRemoveProductLogoImage').removeClass('d-none');
                $('#btnUploadProductLogoImage').addClass('d-none');
            }

            swal.fire({
                text: "Logo uploaded successfully.",
                icon: "success",
                buttonsstyling: false,
                confirmbuttontext: "ok",
                allowescapekey: false,
                allowoutsideclick: false,
                customclass: {
                    confirmbutton: "btn btn-primary",
                },
                didopen: function () {
                    hideSpinner();
                }
            });
        },
        error: function () {
            showerrormodal("An error occurred. Please contact your system administrator.");
        },
        complete: function () {
            Ajax.dequeueSpinner();
        }
    });
}

/////////////////