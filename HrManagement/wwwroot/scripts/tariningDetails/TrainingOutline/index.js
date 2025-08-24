

$(document).ready(function () {

    hideSpinner();
    loadTrainingOutlineData();
});

function loadTrainingOutlineData() {
    const id = $("#Training-Structure-Id").val();
    const url = sitePath + `api/Trainings/GetTrainingOutline?id=${id}`;
    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {
            console.log(resp);
            outlineDeatil(resp);
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

function outlineDeatil(resp) {

    //console.log(resp);
    //descriptionQuill = new Quill('#post_detail_description_editor', {
    //    theme: 'snow'
    //});

    $("#Training-Structure-Id").val(resp.Data.TrainingStructureId);
    let Post = resp?.Data?.outlines?.[0] || null;
    console.log(Post);
    if (Post != null) {
        $("#Id").val(Post.id);
        $("#Outline-Title").val(Post.title);
        $("#Outline-Description").val(Post.description);
        $(`input[name="visibility"][value="${Post.status}"]`).prop('checked', true);  
    }
    if (Array.isArray(resp.Data.fileMediaUplaod) && resp.Data.fileMediaUplaod.length > 0) {
        resp.Data.fileMediaUplaod.forEach(files => {
            if (files.FileType == "OutlineThumbnail") {
                $('#OutlineThumbnailId').val(files.FileId);
                $('#Preview-image-Thumbnail').attr('src', files.FilePath);
            }
           
        });
    }   
}
///Image Upload
const dropArea = document.getElementById('dropArea');
const PostimageThumbnail = document.getElementById('Post-image-Thumbnail');
const imagePreview = document.getElementById('imagePreview');
const previewContainer = document.getElementById('previewContainer');
const removeImageBtn = document.getElementById('removeImageBtn');
const uploadBtn = document.getElementById('uploadBtn');

// Click on drop area triggers file input
dropArea.addEventListener('click', () => PostimageThumbnail.click());

// Keyboard accessibility: Enter or Space triggers file input
dropArea.addEventListener('keydown', e => {
    if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        PostimageThumbnail.click();
    }
});

// Handle file input change
PostimageThumbnail.addEventListener('change', e => {
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
        imagePreview.src = e.target.result;
        previewContainer.style.display = 'block';
        uploadBtn.disabled = false;
    };
    reader.readAsDataURL(file);
}

removeImageBtn.addEventListener('click', () => {
    resetPreview();
    PostimageThumbnail.value = '';
    uploadBtn.disabled = true;
});

function resetPreview() {
    previewContainer.style.display = 'none';
    imagePreview.src = '';
}

uploadBtn.addEventListener('click', () => {
    const file = PostimageThumbnail.files[0];
    if (!file) return alert('No file selected!');
    const formData = new FormData();

    formData.append('Id', $('#OutlineThumbnailId').val() || 0);
    formData.append('ReferenceId', $('#Training-Structure-Id').val() || 0);
    formData.append('ModuleName', "4");
    formData.append('FolderName', "images/Outline");
    formData.append('Type', "OutlineThumbnail");
    const imageFile = $('#Post-image-Thumbnail')[0].files[0];
    if (imageFile) {
        formData.append('File', imageFile);
        fileUpload(formData);
    }
    const modal = bootstrap.Modal.getInstance(document.getElementById('imageUploadModal'));
    modal.hide();

    // Reset after upload
    resetPreview();
    PostimageThumbnail.value = '';
    uploadBtn.disabled = true;
});


function fileUpload(formData) {
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

            //if (res && res.Data != null) {
            //    console.log(res);
            //    $('#' + PreviewId).attr('src', res.Data.FilePath);
            //    $('#' + Id).val(res.Data.Id);
            //    $('#LessonThumbnailReplaceBtn').removeClass('d-none');
            //    $('#LessonThumbnailRemoveBtn').removeClass('d-none');
            //    $('#LessonThumbnailUploadBtn').addClass('d-none');
            //}
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
                let StructureId = $('#Training-Structure-Id').val();
                window.location = `/Training/Edit_Outline/${StructureId}`;
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

